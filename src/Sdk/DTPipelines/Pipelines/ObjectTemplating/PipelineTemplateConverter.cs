using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using GitHub.DistributedTask.Expressions2;
using GitHub.DistributedTask.Expressions2.Sdk;
using GitHub.DistributedTask.Expressions2.Sdk.Functions;
using GitHub.DistributedTask.ObjectTemplating;
using GitHub.DistributedTask.ObjectTemplating.Tokens;
using GitHub.DistributedTask.Pipelines.ContextData;
using GitHub.DistributedTask.Pipelines.Validation;
using GitHub.Services.Common;
using Newtonsoft.Json.Linq;

namespace GitHub.DistributedTask.Pipelines.ObjectTemplating
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class PipelineTemplateConverter
    {
        public static List<Step> ConvertToSteps(
            TemplateContext context,
            TemplateToken steps)
        {
            var stepsSequence = steps.AssertSequence($"job {PipelineTemplateConstants.Steps}");

            var result = new List<Step>();
            var nameBuilder = new ReferenceNameBuilder();
            foreach (var stepsItem in stepsSequence)
            {
                var step = ConvertToStep(context, stepsItem, nameBuilder);
                if (step != null) // step = null means we are hitting error during step conversion, there should be an error in context.errors
                {
                    if (step.Enabled)
                    {
                        result.Add(step);
                    }
                }
            }

            // Generate context name if empty
            foreach (ActionStep step in result)
            {
                if (!String.IsNullOrEmpty(step.ContextName))
                {
                    continue;
                }

                var name = default(string);
                switch (step.Reference.Type)
                {
                    case ActionSourceType.ContainerRegistry:
                        var containerReference = step.Reference as ContainerRegistryReference;
                        name = containerReference.Image;
                        break;
                    case ActionSourceType.Repository:
                        var repositoryReference = step.Reference as RepositoryPathReference;
                        name = !String.IsNullOrEmpty(repositoryReference.Name) ? repositoryReference.Name : PipelineConstants.SelfAlias;
                        break;
                }

                if (String.IsNullOrEmpty(name))
                {
                    name = "run";
                }

                nameBuilder.AppendSegment($"__{name}");
                step.ContextName = nameBuilder.Build();
            }

            return result;
        }

        internal static Boolean ConvertToIfResult(
            TemplateContext context,
            TemplateToken ifResult)
        {
            var expression = ifResult.Traverse().FirstOrDefault(x => x is ExpressionToken);
            if (expression != null)
            {
                throw new ArgumentException($"Unexpected type '{expression.GetType().Name}' encountered while reading 'if'.");
            }

            var evaluationResult = EvaluationResult.CreateIntermediateResult(null, ifResult);
            return evaluationResult.IsTruthy;
        }

        internal static Boolean? ConvertToStepContinueOnError(
            TemplateContext context,
            TemplateToken token,
            Boolean allowExpressions = false)
        {
            if (allowExpressions && token is ExpressionToken)
            {
                return null;
            }

            var booleanToken = token.AssertBoolean($"step {PipelineTemplateConstants.ContinueOnError}");
            return booleanToken.Value;
        }

        internal static String ConvertToStepDisplayName(
            TemplateContext context,
            TemplateToken token,
            Boolean allowExpressions = false)
        {
            if (allowExpressions && token is ExpressionToken)
            {
                return null;
            }

            var stringToken = token.AssertString($"step {PipelineTemplateConstants.Name}");
            return stringToken.Value;
        }

        internal static Dictionary<String, String> ConvertToStepEnvironment(
            TemplateContext context,
            TemplateToken environment,
            StringComparer keyComparer,
            Boolean allowExpressions = false)
        {
            var result = new Dictionary<String, String>(keyComparer);

            // Expression
            if (allowExpressions && environment is ExpressionToken)
            {
                return result;
            }

            // Mapping
            var mapping = environment.AssertMapping("environment");

            foreach (var pair in mapping)
            {
                // Expression key
                if (allowExpressions && pair.Key is ExpressionToken)
                {
                    continue;
                }

                // String key
                var key = pair.Key.AssertString("environment key");

                // Expression value
                if (allowExpressions && pair.Value is ExpressionToken)
                {
                    continue;
                }

                // String value
                var value = pair.Value.AssertString("environment value");
                result[key.Value] = value.Value;
            }

            return result;
        }

        internal static Dictionary<String, String> ConvertToStepInputs(
            TemplateContext context,
            TemplateToken inputs,
            Boolean allowExpressions = false)
        {
            var result = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

            // Expression
            if (allowExpressions && inputs is ExpressionToken)
            {
                return result;
            }

            // Mapping
            var mapping = inputs.AssertMapping("inputs");

            foreach (var pair in mapping)
            {
                // Expression key
                if (allowExpressions && pair.Key is ExpressionToken)
                {
                    continue;
                }

                // Literal key
                var key = pair.Key.AssertString("inputs key");

                // Expression value
                if (allowExpressions && pair.Value is ExpressionToken)
                {
                    continue;
                }

                // Literal value
                var value = pair.Value.AssertString("inputs value");
                result[key.Value] = value.Value;
            }

            return result;
        }

        internal static Int32? ConvertToStepTimeout(
            TemplateContext context,
            TemplateToken token,
            Boolean allowExpressions = false)
        {
            if (allowExpressions && token is ExpressionToken)
            {
                return null;
            }

            var numberToken = token.AssertNumber($"step {PipelineTemplateConstants.TimeoutMinutes}");
            return (Int32)numberToken.Value;
        }

        internal static ContainerRegistryCredentials ConvertToContainerCredentials(TemplateToken token)
        {
            var credentials = token.AssertMapping(PipelineTemplateConstants.Credentials);
            var result = new ContainerRegistryCredentials();
            foreach (var credentialProperty in credentials)
            {
                var propertyName = credentialProperty.Key.AssertString($"{PipelineTemplateConstants.Credentials} key");
                switch (propertyName.Value)
                {
                    case PipelineTemplateConstants.Username:
                        result.Username = credentialProperty.Value.AssertString(PipelineTemplateConstants.Username).Value;
                        break;
                    case PipelineTemplateConstants.Password:
                        result.Password = credentialProperty.Value.AssertString(PipelineTemplateConstants.Password).Value;
                        break;
                    default:
                        propertyName.AssertUnexpectedValue($"{PipelineTemplateConstants.Credentials} key {propertyName}");
                        break;
                }
            }

            return result;
        }

        internal static JobContainer ConvertToJobContainer(
            TemplateContext context,
            TemplateToken value,
            bool allowExpressions = false)
        {
            var result = new JobContainer();
            if (allowExpressions && value.Traverse().Any(x => x is ExpressionToken))
            {
                return result;
            }

            if (value is StringToken containerLiteral)
            {
                if (String.IsNullOrEmpty(containerLiteral.Value))
                {
                    return null;
                }

                result.Image = containerLiteral.Value;
            }
            else
            {
                var containerMapping = value.AssertMapping($"{PipelineTemplateConstants.Container}");
                foreach (var containerPropertyPair in containerMapping)
                {
                    var propertyName = containerPropertyPair.Key.AssertString($"{PipelineTemplateConstants.Container} key");

                    switch (propertyName.Value)
                    {
                        case PipelineTemplateConstants.Image:
                            result.Image = containerPropertyPair.Value.AssertString($"{PipelineTemplateConstants.Container} {propertyName}").Value;
                            break;
                        case PipelineTemplateConstants.Env:
                            var env = containerPropertyPair.Value.AssertMapping($"{PipelineTemplateConstants.Container} {propertyName}");
                            var envDict = new Dictionary<String, String>(env.Count);
                            foreach (var envPair in env)
                            {
                                var envKey = envPair.Key.ToString();
                                var envValue = envPair.Value.AssertString($"{PipelineTemplateConstants.Container} {propertyName} {envPair.Key.ToString()}").Value;
                                envDict.Add(envKey, envValue);
                            }
                            result.Environment = envDict;
                            break;
                        case PipelineTemplateConstants.Options:
                            result.Options = containerPropertyPair.Value.AssertString($"{PipelineTemplateConstants.Container} {propertyName}").Value;
                            break;
                        case PipelineTemplateConstants.Ports:
                            var ports = containerPropertyPair.Value.AssertSequence($"{PipelineTemplateConstants.Container} {propertyName}");
                            var portList = new List<String>(ports.Count);
                            foreach (var portItem in ports)
                            {
                                var portString = portItem.AssertString($"{PipelineTemplateConstants.Container} {propertyName} {portItem.ToString()}").Value;
                                portList.Add(portString);
                            }
                            result.Ports = portList;
                            break;
                        case PipelineTemplateConstants.Volumes:
                            var volumes = containerPropertyPair.Value.AssertSequence($"{PipelineTemplateConstants.Container} {propertyName}");
                            var volumeList = new List<String>(volumes.Count);
                            foreach (var volumeItem in volumes)
                            {
                                var volumeString = volumeItem.AssertString($"{PipelineTemplateConstants.Container} {propertyName} {volumeItem.ToString()}").Value;
                                volumeList.Add(volumeString);
                            }
                            result.Volumes = volumeList;
                            break;
                        case PipelineTemplateConstants.Credentials:
                            result.Credentials = ConvertToContainerCredentials(containerPropertyPair.Value);
                            break;
                        default:
                            propertyName.AssertUnexpectedValue($"{PipelineTemplateConstants.Container} key");
                            break;
                    }
                }
            }

            if (result.Image.StartsWith("docker://", StringComparison.Ordinal))
            {
                result.Image = result.Image.Substring("docker://".Length);
            }

            if (String.IsNullOrEmpty(result.Image))
            {
                return null;
            }

            return result;
        }

        internal static List<KeyValuePair<String, JobContainer>> ConvertToJobServiceContainers(
            TemplateContext context,
            TemplateToken services,
            bool allowExpressions = false)
        {
            var result = new List<KeyValuePair<String, JobContainer>>();

            if (allowExpressions && services.Traverse().Any(x => x is ExpressionToken))
            {
                return result;
            }

            var servicesMapping = services.AssertMapping("services");

            foreach (var servicePair in servicesMapping)
            {
                var networkAlias = servicePair.Key.AssertString("services key").Value;
                var container = ConvertToJobContainer(context, servicePair.Value);
                result.Add(new KeyValuePair<String, JobContainer>(networkAlias, container));
            }

            return result;
        }

        internal static Snapshot ConvertToJobSnapshotRequest(TemplateContext context, TemplateToken token)
        {
            string imageName = null;
            string version = "1.*";
            string versionString = string.Empty;
            var condition = $"{PipelineTemplateConstants.Success}()";

            if (token is StringToken snapshotStringLiteral)
            {
                imageName = snapshotStringLiteral.Value;
            }
            else
            {
                var snapshotMapping = token.AssertMapping($"{PipelineTemplateConstants.Snapshot}");
                foreach (var snapshotPropertyPair in snapshotMapping)
                {
                    var propertyName = snapshotPropertyPair.Key.AssertString($"{PipelineTemplateConstants.Snapshot} key");
                    var propertyValue = snapshotPropertyPair.Value;
                    switch (propertyName.Value)
                    {
                        case PipelineTemplateConstants.ImageName:
                            imageName = snapshotPropertyPair.Value.AssertString($"{PipelineTemplateConstants.Snapshot} {propertyName}").Value;
                            break;
                        case PipelineTemplateConstants.If:
                            condition = ConvertToIfCondition(context, propertyValue, false);
                            break;
                        case PipelineTemplateConstants.CustomImageVersion:
                            versionString = propertyValue.AssertString($"job {PipelineTemplateConstants.Snapshot} {PipelineTemplateConstants.CustomImageVersion}").Value;
                            version = IsSnapshotImageVersionValid(versionString) ? versionString : null;
                            break;
                        default:
                            propertyName.AssertUnexpectedValue($"{PipelineTemplateConstants.Snapshot} key");
                            break;
                    }
                }
            }

            if (String.IsNullOrEmpty(imageName))
            {
                return null;
            }

            return new Snapshot(imageName)
            {
                Condition = condition,
                Version = version
            };
        }

        private static bool IsSnapshotImageVersionValid(string versionString)
        {
            var versionSegments = versionString.Split(".");

            if (versionSegments.Length != 2 ||
                !versionSegments[1].Equals("*") ||
                !Int32.TryParse(versionSegments[0], NumberStyles.None, CultureInfo.InvariantCulture, result: out int parsedMajor) ||
                parsedMajor < 0)
            {
                return false;
            }

            return true;
        }

        private static ActionStep ConvertToStep(
            TemplateContext context,
            TemplateToken stepsItem,
            ReferenceNameBuilder nameBuilder)
        {
            var step = stepsItem.AssertMapping($"{PipelineTemplateConstants.Steps} item");
            var continueOnError = default(ScalarToken);
            var env = default(TemplateToken);
            var id = default(StringToken);
            var ifCondition = default(String);
            var ifToken = default(ScalarToken);
            var name = default(ScalarToken);
            var run = default(ScalarToken);
            var timeoutMinutes = default(ScalarToken);
            var uses = default(StringToken);
            var with = default(TemplateToken);
            var workingDir = default(ScalarToken);
            var shell = default(ScalarToken);

            foreach (var stepProperty in step)
            {
                var propertyName = stepProperty.Key.AssertString($"{PipelineTemplateConstants.Steps} item key");

                switch (propertyName.Value)
                {
                    case PipelineTemplateConstants.ContinueOnError:
                        ConvertToStepContinueOnError(context, stepProperty.Value, allowExpressions: true); // Validate early if possible
                        continueOnError = stepProperty.Value.AssertScalar($"{PipelineTemplateConstants.Steps} {PipelineTemplateConstants.ContinueOnError}");
                        break;

                    case PipelineTemplateConstants.Env:
                        ConvertToStepEnvironment(context, stepProperty.Value, StringComparer.Ordinal, allowExpressions: true); // Validate early if possible
                        env = stepProperty.Value;
                        break;

                    case PipelineTemplateConstants.Id:
                        id = stepProperty.Value.AssertString($"{PipelineTemplateConstants.Steps} item {PipelineTemplateConstants.Id}");
                        if (!String.IsNullOrEmpty(id.Value))
                        {
                            if (!nameBuilder.TryAddKnownName(id.Value, out var error))
                            {
                                context.Error(id, error);
                            }
                        }
                        break;

                    case PipelineTemplateConstants.If:
                        ifToken = stepProperty.Value.AssertScalar($"{PipelineTemplateConstants.Steps} item {PipelineTemplateConstants.If}");
                        break;

                    case PipelineTemplateConstants.Name:
                        name = stepProperty.Value.AssertScalar($"{PipelineTemplateConstants.Steps} item {PipelineTemplateConstants.Name}");
                        break;

                    case PipelineTemplateConstants.Run:
                        run = stepProperty.Value.AssertScalar($"{PipelineTemplateConstants.Steps} item {PipelineTemplateConstants.Run}");
                        break;

                    case PipelineTemplateConstants.Shell:
                        shell = stepProperty.Value.AssertScalar($"{PipelineTemplateConstants.Steps} item {PipelineTemplateConstants.Shell}");
                        break;

                    case PipelineTemplateConstants.TimeoutMinutes:
                        ConvertToStepTimeout(context, stepProperty.Value, allowExpressions: true); // Validate early if possible
                        timeoutMinutes = stepProperty.Value.AssertScalar($"{PipelineTemplateConstants.Steps} item {PipelineTemplateConstants.TimeoutMinutes}");
                        break;

                    case PipelineTemplateConstants.Uses:
                        uses = stepProperty.Value.AssertString($"{PipelineTemplateConstants.Steps} item {PipelineTemplateConstants.Uses}");
                        break;

                    case PipelineTemplateConstants.With:
                        ConvertToStepInputs(context, stepProperty.Value, allowExpressions: true); // Validate early if possible
                        with = stepProperty.Value;
                        break;

                    case PipelineTemplateConstants.WorkingDirectory:
                        workingDir = stepProperty.Value.AssertScalar($"{PipelineTemplateConstants.Steps} item {PipelineTemplateConstants.WorkingDirectory}");
                        break;

                    default:
                        propertyName.AssertUnexpectedValue($"{PipelineTemplateConstants.Steps} item key"); // throws
                        break;
                }
            }

            // Fixup the if-condition
            ifCondition = ConvertToIfCondition(context, ifToken, false);

            if (run != null)
            {
                var result = new ActionStep
                {
                    ContextName = id?.Value,
                    ContinueOnError = continueOnError,
                    DisplayNameToken = name,
                    Condition = ifCondition,
                    TimeoutInMinutes = timeoutMinutes,
                    Environment = env,
                    Reference = new ScriptReference(),
                };

                var inputs = new MappingToken(null, null, null);
                inputs.Add(new StringToken(null, null, null, PipelineConstants.ScriptStepInputs.Script), run);

                if (workingDir != null)
                {
                    inputs.Add(new StringToken(null, null, null, PipelineConstants.ScriptStepInputs.WorkingDirectory), workingDir);
                }

                if (shell != null)
                {
                    inputs.Add(new StringToken(null, null, null, PipelineConstants.ScriptStepInputs.Shell), shell);
                }

                result.Inputs = inputs;

                return result;
            }
            else
            {
                uses.AssertString($"{PipelineTemplateConstants.Steps} item {PipelineTemplateConstants.Uses}");
                var result = new ActionStep
                {
                    ContextName = id?.Value,
                    ContinueOnError = continueOnError,
                    DisplayNameToken = name,
                    Condition = ifCondition,
                    TimeoutInMinutes = timeoutMinutes,
                    Inputs = with,
                    Environment = env,
                };

                if (uses.Value.StartsWith("docker://", StringComparison.Ordinal))
                {
                    var image = uses.Value.Substring("docker://".Length);
                    result.Reference = new ContainerRegistryReference { Image = image };
                }
                else if (uses.Value.StartsWith("./") || uses.Value.StartsWith(".\\") || System.IO.Path.IsPathFullyQualified(uses.Value))
                {
                    result.Reference = new RepositoryPathReference
                    {
                        RepositoryType = PipelineConstants.SelfAlias,
                        Path = uses.Value
                    };
                }
                else
                {
                    var usesSegments = uses.Value.Split('@');
                    var pathSegments = usesSegments[0].Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    var gitRef = usesSegments.Length == 2 ? usesSegments[1] : String.Empty;
                    string token = null;
                    if(usesSegments.Length >= 2 && usesSegments.Length <= 3 && context.AbsoluteActions)
                    {
                        foreach (var proto in new [] {"http://", "https://"})
                        {
                            if (usesSegments[0].StartsWith(proto))
                            {
                                // Includes basic auth
                                if (usesSegments.Length == 3)
                                {
                                    usesSegments = new string[] { string.Join("@", usesSegments.Take(2)), usesSegments[2] };
                                    // Remove auth part and extract the token for loading
                                    var absoluteUrl = new Uri(usesSegments[0]);
                                    usesSegments[0] = absoluteUrl.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.UserInfo, UriFormat.UriEscaped);
                                    token = string.Join(":", absoluteUrl.UserInfo?.Split(':').Skip(1));
                                    gitRef = usesSegments[1];
                                }
                                pathSegments = usesSegments[0].Substring(proto.Length).Split('/');
                                pathSegments = pathSegments.Skip(2).Prepend((proto + string.Join("/", pathSegments.Take(2))).Replace(':', '~')).ToArray();
                                break;
                            }
                        }
                    }

                    if (usesSegments.Length != 2 ||
                        pathSegments.Length < 2 ||
                        String.IsNullOrEmpty(pathSegments[0]) ||
                        String.IsNullOrEmpty(pathSegments[1]) ||
                        String.IsNullOrEmpty(gitRef))
                    {
                        // todo: loc
                        context.Error(uses, $"Expected format {{org}}/{{repo}}[/path]@ref. Actual '{uses.Value}'");
                    }
                    else
                    {
                        var repositoryName = $"{pathSegments[0]}/{pathSegments[1]}";
                        var directoryPath = pathSegments.Length > 2 ? String.Join("/", pathSegments.Skip(2)) : String.Empty;

                        result.Reference = new RepositoryPathReference
                        {
                            RepositoryType = RepositoryTypes.GitHub,
                            Name = repositoryName,
                            Ref = gitRef,
                            Path = directoryPath,
                            Token = token,
                        };
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// When empty, default to "success()".
        /// When a status function is not referenced, format as "success() &amp;&amp; &lt;CONDITION&gt;".
        /// </summary>
        public static String ConvertToIfCondition(
            TemplateContext context,
            TemplateToken token,
            Boolean isJob)
        {
            String condition;
            if (token is null)
            {
                condition = null;
            }
            else if (token is BasicExpressionToken expressionToken)
            {
                condition = expressionToken.Expression;
            }
            else
            {
                var stringToken = token.AssertString($"{(isJob ? "job" : "step")} {PipelineTemplateConstants.If}");
                condition = stringToken.Value;
            }

            if (String.IsNullOrWhiteSpace(condition))
            {
                return $"{PipelineTemplateConstants.Success}()";
            }

            var expressionParser = new ExpressionParser() { Flags = context.Flags };

            // Create dummy named values and functions
            var namedValues = new List<INamedValueInfo>();
            var functions = new List<IFunctionInfo>();
            var allowedContext = context.Schema.GetDefinition(isJob ? "job-if" : "step-if").ReaderContext;
            if (allowedContext?.Length > 0)
            {
                foreach (var contextItem in allowedContext)
                {
                    var match = s_function.Match(contextItem);
                    if (match.Success)
                    {
                        var functionName = match.Groups[1].Value;
                        var minParameters = Int32.Parse(match.Groups[2].Value, NumberStyles.None, CultureInfo.InvariantCulture);
                        var maxParametersRaw = match.Groups[3].Value;
                        var maxParameters = String.Equals(maxParametersRaw, TemplateConstants.MaxConstant, StringComparison.Ordinal)
                            ? Int32.MaxValue
                            : Int32.Parse(maxParametersRaw, NumberStyles.None, CultureInfo.InvariantCulture);
                        functions.Add(new FunctionInfo<NoOperation>(functionName, minParameters, maxParameters));
                    }
                    else
                    {
                        namedValues.Add(new NamedValueInfo<NoOperationNamedValue>(contextItem));
                    }
                }
            }

            var node = default(ExpressionNode);
            try
            {
                node = expressionParser.CreateTree(condition, null, namedValues, functions) as ExpressionNode;
            }
            catch (Exception ex)
            {
                context.Error(token, ex);
                return null;
            }

            if (node == null)
            {
                return $"{PipelineTemplateConstants.Success}()";
            }

            var hasStatusFunction = node.Traverse().Any(x =>
            {
                if (x is Function function)
                {
                    return String.Equals(function.Name, PipelineTemplateConstants.Always, StringComparison.OrdinalIgnoreCase) ||
                        String.Equals(function.Name, PipelineTemplateConstants.Cancelled, StringComparison.OrdinalIgnoreCase) ||
                        String.Equals(function.Name, PipelineTemplateConstants.Failure, StringComparison.OrdinalIgnoreCase) ||
                        String.Equals(function.Name, PipelineTemplateConstants.Success, StringComparison.OrdinalIgnoreCase);
                }

                return false;
            });

            return hasStatusFunction ? condition : $"{PipelineTemplateConstants.Success}() && ({condition})";
        }

        private static readonly Regex s_function = new Regex(@"^([a-zA-Z0-9_]+)\(([0-9]+),([0-9]+|MAX)\)$", RegexOptions.Compiled);
        private static readonly INamedValueInfo[] s_jobIfNamedValues = new INamedValueInfo[]
        {
            new NamedValueInfo<NoOperationNamedValue>(PipelineTemplateConstants.GitHub),
            new NamedValueInfo<NoOperationNamedValue>(PipelineTemplateConstants.Needs),
            new NamedValueInfo<NoOperationNamedValue>(PipelineTemplateConstants.Vars),
        };
        private static readonly INamedValueInfo[] s_stepNamedValues = new INamedValueInfo[]
        {
            new NamedValueInfo<NoOperationNamedValue>(PipelineTemplateConstants.Strategy),
            new NamedValueInfo<NoOperationNamedValue>(PipelineTemplateConstants.Matrix),
            new NamedValueInfo<NoOperationNamedValue>(PipelineTemplateConstants.Steps),
            new NamedValueInfo<NoOperationNamedValue>(PipelineTemplateConstants.GitHub),
            new NamedValueInfo<NoOperationNamedValue>(PipelineTemplateConstants.Inputs),
            new NamedValueInfo<NoOperationNamedValue>(PipelineTemplateConstants.Job),
            new NamedValueInfo<NoOperationNamedValue>(PipelineTemplateConstants.Runner),
            new NamedValueInfo<NoOperationNamedValue>(PipelineTemplateConstants.Env),
            new NamedValueInfo<NoOperationNamedValue>(PipelineTemplateConstants.Needs),
            new NamedValueInfo<NoOperationNamedValue>(PipelineTemplateConstants.Vars),
        };
        private static readonly IFunctionInfo[] s_stepConditionFunctions = new IFunctionInfo[]
        {
            new FunctionInfo<NoOperation>(PipelineTemplateConstants.Always, 0, 0),
            new FunctionInfo<NoOperation>(PipelineTemplateConstants.Cancelled, 0, 0),
            new FunctionInfo<NoOperation>(PipelineTemplateConstants.Failure, 0, 0),
            new FunctionInfo<NoOperation>(PipelineTemplateConstants.Success, 0, 0),
            new FunctionInfo<NoOperation>(PipelineTemplateConstants.HashFiles, 1, Byte.MaxValue),
        };
    }
}
