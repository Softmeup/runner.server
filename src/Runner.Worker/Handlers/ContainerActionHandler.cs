using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GitHub.DistributedTask.Pipelines.ContextData;
using GitHub.DistributedTask.WebApi;
using GitHub.Runner.Common;
using GitHub.Runner.Sdk;
using GitHub.Runner.Worker.Container;
using GitHub.Runner.Worker.Container.ContainerHooks;
using Pipelines = GitHub.DistributedTask.Pipelines;

namespace GitHub.Runner.Worker.Handlers
{
    [ServiceLocator(Default = typeof(ContainerActionHandler))]
    public interface IContainerActionHandler : IHandler
    {
        ContainerActionExecutionData Data { get; set; }
    }

    public sealed class ContainerActionHandler : Handler, IContainerActionHandler
    {
        private static string GetHostOS() {
            if(System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux)) {
                return "linux";
            } else if(System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)) {
                return "windows";
            } else if(System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX)) {
                return "osx";
            }
            return null;
        }

        public ContainerActionExecutionData Data { get; set; }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously (method has async logic on only certain platforms)
        public async Task RunAsync(ActionRunStage stage)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            // Validate args.
            Trace.Entering();
            ArgUtil.NotNull(Data, nameof(Data));
            ArgUtil.NotNull(ExecutionContext, nameof(ExecutionContext));

            RunnerContext runnerctx = null;
            try {
                runnerctx = ExecutionContext.ExpressionValues["runner"] as RunnerContext;
            } catch {

            }

            // Update the env dictionary.
            AddInputsToEnvironment();

            IDockerCommandManager dockerManager = null;
            IContainerHookManager containerHookManager = null;
            if (FeatureManager.IsContainerHooksEnabled(ExecutionContext.Global.Variables))
            {
                containerHookManager = HostContext.GetService<IContainerHookManager>();
            }
            else
            {
                dockerManager = HostContext.GetService<IDockerCommandManager>();
            }

            string dockerFile = null;

            // container image haven't built/pull
            if (Data.Image.StartsWith("docker://", StringComparison.OrdinalIgnoreCase))
            {
                Data.Image = Data.Image.Substring("docker://".Length);
            }
            else if (DockerUtil.IsDockerfile(Data.Image))
            {
                // ensure docker file exist
                dockerFile = Path.Combine(ActionDirectory, Data.Image);
                ArgUtil.File(dockerFile, nameof(Data.Image));
                if (!FeatureManager.IsContainerHooksEnabled(ExecutionContext.Global.Variables))
                {
                    ExecutionContext.Output($"##[group]Building docker image");
                    ExecutionContext.Output($"Dockerfile for action: '{dockerFile}'.");
                    var imageName = $"{dockerManager.DockerInstanceLabel}:{ExecutionContext.Id.ToString("N")}";
                    var buildExitCode = await dockerManager.DockerBuild(
                        ExecutionContext,
                        ExecutionContext.GetGitHubContext("workspace"),
                        dockerFile,
                        Directory.GetParent(dockerFile).FullName,
                        imageName);
                    ExecutionContext.Output("##[endgroup]");

                    if (buildExitCode != 0)
                    {
                        throw new InvalidOperationException($"Docker build failed with exit code {buildExitCode}");
                    }

                    Data.Image = imageName;
                }
            }

            string type = Action.Type == Pipelines.ActionSourceType.Repository ? "Dockerfile" : "DockerHub";
            // Set extra telemetry base on the current context.
            if (stage == ActionRunStage.Main)
            {
                ExecutionContext.StepTelemetry.HasPreStep = Data.HasPre;
                ExecutionContext.StepTelemetry.HasPostStep = Data.HasPost;
            }
            ExecutionContext.StepTelemetry.Type = type;

            // run container
            var container = new ContainerInfo(HostContext)
            {
                ContainerImage = Data.Image,
                ContainerName = ExecutionContext.Id.ToString("N"),
                ContainerDisplayName = $"{Pipelines.Validation.NameValidation.Sanitize(Data.Image)}_{Guid.NewGuid().ToString("N").Substring(0, 6)}",
            };
            string[] plat = new [] { "linux", "" };
            for(int i = 0; i < 2; i++) {
                try {
                    var platform = (await dockerManager.DockerInspect(ExecutionContext, container.ContainerImage, "--format=\"{{.Os}}/{{.Architecture}}\"")).FirstOrDefault();
                    if(string.IsNullOrEmpty(platform) || !platform.Contains('/')) {
                        throw new Exception("Failed to determine container platform");
                    }
                    plat = platform.Split('/', 2);
                    break;
                } catch(Exception ex) {
                    ExecutionContext.Error(ex);
                }
                // Just in case docker inspect fails, because we ran as an local action
                await dockerManager.DockerPull(ExecutionContext, container.ContainerImage);
            }
            container.Os = plat[0];
            container.Arch = plat[1];

            if (stage == ActionRunStage.Main)
            {
                if (!string.IsNullOrEmpty(Data.EntryPoint))
                {
                    // use entrypoint from action.yml
                    container.ContainerEntryPoint = Data.EntryPoint;
                }
                else
                {
                    // use entrypoint input, this is for action v1 which doesn't have action.yml
                    container.ContainerEntryPoint = Inputs.GetValueOrDefault("entryPoint");
                }
            }
            else if (stage == ActionRunStage.Pre)
            {
                container.ContainerEntryPoint = Data.Pre;
            }
            else if (stage == ActionRunStage.Post)
            {
                container.ContainerEntryPoint = Data.Post;
            }

            // create inputs context for template evaluation
            var inputsContext = new DictionaryContextData();
            if (this.Inputs != null)
            {
                foreach (var input in Inputs)
                {
                    inputsContext.Add(input.Key, new StringContextData(input.Value));
                }
            }

            var extraExpressionValues = new Dictionary<string, PipelineContextData>(StringComparer.OrdinalIgnoreCase);
            extraExpressionValues["inputs"] = inputsContext;

            var manifestManager = HostContext.GetService<IActionManifestManager>();
            if (Data.Arguments != null)
            {
                container.ContainerEntryPointArgs = "";
                var evaluatedArgs = manifestManager.EvaluateContainerArguments(ExecutionContext, Data.Arguments, extraExpressionValues);
                foreach (var arg in evaluatedArgs)
                {
                    if (!string.IsNullOrEmpty(arg))
                    {
                        container.ContainerEntryPointArgs = container.ContainerEntryPointArgs + $" {DockerUtil.EscapeString(arg)}";
                    }
                    else
                    {
                        container.ContainerEntryPointArgs = container.ContainerEntryPointArgs + " \"\"";
                    }
                }
            }
            else
            {
                container.ContainerEntryPointArgs = Inputs.GetValueOrDefault("args");
            }

            if (Data.Environment != null)
            {
                var evaluatedEnv = manifestManager.EvaluateContainerEnvironment(ExecutionContext, Data.Environment, extraExpressionValues);
                foreach (var env in evaluatedEnv)
                {
                    if (!this.Environment.ContainsKey(env.Key))
                    {
                        this.Environment[env.Key] = env.Value;
                    }
                }
            }

            if (ExecutionContext.JobContext.Container.TryGetValue("network", out var networkContextData) && networkContextData is StringContextData networkStringData)
            {
                container.ContainerNetwork = networkStringData.ToString();
            }

            var defaultWorkingDirectory = ExecutionContext.GetGitHubContext("workspace");
            var tempDirectory = HostContext.GetDirectory(WellKnownDirectory.Temp);

            ArgUtil.NotNullOrEmpty(defaultWorkingDirectory, nameof(defaultWorkingDirectory));
            ArgUtil.NotNullOrEmpty(tempDirectory, nameof(tempDirectory));

            var tempHomeDirectory = Path.Combine(tempDirectory, "_github_home");
            Directory.CreateDirectory(tempHomeDirectory);
            this.Environment["HOME"] = tempHomeDirectory;

            var tempFileCommandDirectory = Path.Combine(tempDirectory, "_runner_file_commands");
            ArgUtil.Directory(tempFileCommandDirectory, nameof(tempFileCommandDirectory));

            var tempWorkflowDirectory = Path.Combine(tempDirectory, "_github_workflow");
            ArgUtil.Directory(tempWorkflowDirectory, nameof(tempWorkflowDirectory));

            var containerDaemonSocket = System.Environment.GetEnvironmentVariable("RUNNER_CONTAINER_DAEMON_SOCKET");
            Func<string, string> mountPath;
            if(container.Os == "windows") {
                mountPath = s => ("C:" + s).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                container.MountVolumes.Add(new MountVolume(containerDaemonSocket ?? @"\\.\pipe\docker_engine", @"\\.\pipe\docker_engine"));
            } else {
                mountPath = s => s;
                container.MountVolumes.Add(new MountVolume(containerDaemonSocket ?? "/var/run/docker.sock", "/var/run/docker.sock"));
            }
            container.MountVolumes.Add(new MountVolume(tempHomeDirectory, mountPath("/github/home")));
            container.MountVolumes.Add(new MountVolume(tempWorkflowDirectory, mountPath("/github/workflow")));
            container.MountVolumes.Add(new MountVolume(tempFileCommandDirectory, mountPath("/github/file_commands")));
            container.MountVolumes.Add(new MountVolume(defaultWorkingDirectory, mountPath("/github/workspace")));

            container.AddPathTranslateMapping(tempHomeDirectory, mountPath("/github/home"));
            container.AddPathTranslateMapping(tempWorkflowDirectory, mountPath("/github/workflow"));
            container.AddPathTranslateMapping(tempFileCommandDirectory, mountPath("/github/file_commands"));
            container.AddPathTranslateMapping(defaultWorkingDirectory, mountPath("/github/workspace"));

            container.ContainerWorkDirectory = mountPath("/github/workspace");

            // expose context to environment
            foreach (var context in ExecutionContext.ExpressionValues)
            {
                if (context.Value is IEnvironmentContextData runtimeContext && runtimeContext != null)
                {
                    foreach (var env in runtimeContext.GetRuntimeEnvironmentVariables())
                    {
                        Environment[env.Key] = env.Value;
                    }
                }
            }

            // Add Actions Runtime server info
            var systemConnection = ExecutionContext.Global.Endpoints.Single(x => string.Equals(x.Name, WellKnownServiceEndpointNames.SystemVssConnection, StringComparison.OrdinalIgnoreCase));
            Environment["ACTIONS_RUNTIME_URL"] = systemConnection.Url.AbsoluteUri;
            Environment["ACTIONS_RUNTIME_TOKEN"] = systemConnection.Authorization.Parameters[EndpointAuthorizationParameters.AccessToken];
            if (systemConnection.Data.TryGetValue("CacheServerUrl", out var cacheUrl) && !string.IsNullOrEmpty(cacheUrl))
            {
                Environment["ACTIONS_CACHE_URL"] = cacheUrl;
            }
            if (systemConnection.Data.TryGetValue("PipelinesServiceUrl", out var pipelinesServiceUrl) && !string.IsNullOrEmpty(pipelinesServiceUrl))
            {
                Environment["ACTIONS_RUNTIME_URL"] = pipelinesServiceUrl;
            }
            if (systemConnection.Data.TryGetValue("GenerateIdTokenUrl", out var generateIdTokenUrl) && !string.IsNullOrEmpty(generateIdTokenUrl))
            {
                Environment["ACTIONS_ID_TOKEN_REQUEST_URL"] = generateIdTokenUrl;
                Environment["ACTIONS_ID_TOKEN_REQUEST_TOKEN"] = systemConnection.Authorization.Parameters[EndpointAuthorizationParameters.AccessToken];
            }
            if (systemConnection.Data.TryGetValue("ResultsServiceUrl", out var resultsUrl) && !string.IsNullOrEmpty(resultsUrl))
            {
                Environment["ACTIONS_RESULTS_URL"] = resultsUrl;
            }

            foreach (var variable in this.Environment)
            {
                container.ContainerEnvironmentVariables[variable.Key] = container.TranslateToContainerPath(variable.Value);
            }

            if (FeatureManager.IsContainerHooksEnabled(ExecutionContext.Global.Variables))
            {
                await containerHookManager.RunContainerStepAsync(ExecutionContext, container, dockerFile);
            }
            else
            {
                using (var stdoutManager = new OutputManager(ExecutionContext, ActionCommandManager, container))
                using (var stderrManager = new OutputManager(ExecutionContext, ActionCommandManager, container))
                {
                    var runExitCode = await dockerManager.DockerRun(ExecutionContext, container, stdoutManager.OnDataReceived, stderrManager.OnDataReceived);
                    ExecutionContext.Debug($"Docker Action run completed with exit code {runExitCode}");
                    if (runExitCode != 0)
                    {
                        ExecutionContext.Result = TaskResult.Failed;
                    }
                }
            }
        }
    }
}
