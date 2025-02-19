﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GitHub.DistributedTask.Expressions2;
using GitHub.DistributedTask.Expressions2.Sdk.Functions.v1;
using GitHub.DistributedTask.Expressions2.Tokens;
using GitHub.DistributedTask.ObjectTemplating.Schema;
using GitHub.DistributedTask.ObjectTemplating.Tokens;
using GitHub.DistributedTask.Pipelines.ContextData;

namespace GitHub.DistributedTask.ObjectTemplating
{
    internal class AutoCompleteEntry {
        public int Depth { get; set; }
        public TemplateToken Token { get; set; }
        public Definition[] Definitions { get; set; }
        public string[] AllowedContext { get; set; }
        public List<Token> Tokens { get; set; }
        public int Index { get; set; } = -1;
        public bool SemTokensOnly { get; set; }
        public (int, int)[] Mapping { get; set; }
        public Dictionary<(int, int), int> RMapping { get; internal set; }
        public string Description { get; internal set; }
    }

    /// <summary>
    /// Context object that is flowed through while loading and evaluating object templates
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class TemplateContext
    {
        public ExpressionFlags Flags { get; set; }
        public bool AbsoluteActions { get; set; }
        internal CancellationToken CancellationToken { get; set; }

        public int? Column { get; set; }
        public int? Row { get; set; }
        internal List<AutoCompleteEntry> AutoCompleteMatches { get; set; }
        public List<int> SemTokens { get; set; } = new List<int>();
        public int LastRow { get; set; } = 1;
        public int LastColumn { get; set; } = 1;

        public Func<TemplateContext, MappingToken, DictionaryContextData, Task> EvaluateVariable { get; set; }

        public SkipErrorDisposable SkopedErrorLevel(TemplateValidationErrors skipError = null) {
            m_fatal_errors ??= Errors;
            return new SkipErrorDisposable(this, skipError);
        }

        public void AddSemToken(int row, int column, int len, int type, int mod) {
            if(row - LastRow < 0 || ((row - LastRow) != 0 ? column - 1: column - LastColumn) < 0) {
                // Insert
                int i = 0;
                int r = 1;
                while(r + SemTokens[i * 5] < row) {
                    r += SemTokens[i++ * 5];
                }
                int c = 1;
                while(c + SemTokens[i * 5 + 1] <= column && r + SemTokens[i * 5] == row) {
                    c += SemTokens[i * 5 + 1];
                    r += SemTokens[i * 5];
                    i++;
                }
                if(SemTokens[i * 5] == 0) {
                    SemTokens[i * 5 + 1] -= (row - r) != 0 ? column - 1: column - c;
                } else {
                    SemTokens[i * 5] -= row - r;
                }
                SemTokens.InsertRange(i * 5, new int[] { row - r, (row - r) != 0 ? column - 1: column - c, len, type, mod });
            } else {
                SemTokens.AddRange(new int[] { row - LastRow, (row - LastRow) != 0 ? column - 1: column - LastColumn, len, type, mod});
                LastRow = row;
                LastColumn = column;
            }
        }

        internal TemplateValidationErrors FatalErrors
        {
            get
            {
                if (m_fatal_errors == null)
                {
                    m_fatal_errors = new TemplateValidationErrors();
                }

                return m_fatal_errors;
            }

            set
            {
                m_fatal_errors = value;
            }
        }

        internal TemplateValidationErrors Errors
        {
            get
            {
                if (m_errors == null)
                {
                    m_errors = new TemplateValidationErrors();
                }

                return m_errors;
            }

            set
            {
                m_errors = value;
            }
        }

        internal TemplateValidationErrors Warnings
        {
            get
            {
                if (m_warnings == null)
                {
                    m_warnings = new TemplateValidationErrors();
                }

                return m_warnings;
            }

            set
            {
                m_warnings = value;
            }
        }

        /// <summary>
        /// Available functions within expression contexts
        /// </summary>
        internal IList<IFunctionInfo> ExpressionFunctions
        {
            get
            {
                if (m_expressionFunctions == null)
                {
                    m_expressionFunctions = new List<IFunctionInfo>();
                }

                return m_expressionFunctions;
            }
        }

        /// <summary>
        /// Available values within expression contexts
        /// </summary>
        internal IDictionary<String, Object> ExpressionValues
        {
            get
            {
                if (m_expressionValues == null)
                {
                    m_expressionValues = new Dictionary<String, Object>(StringComparer.OrdinalIgnoreCase);
                }

                return m_expressionValues;
            }
        }

        internal TemplateMemory Memory { get; set; }

        internal TemplateSchema Schema { get; set; }

        internal IDictionary<String, Object> State
        {
            get
            {
                if (m_state == null)
                {
                    m_state = new Dictionary<String, Object>(StringComparer.OrdinalIgnoreCase);
                }

                return m_state;
            }
        }

        internal ITraceWriter TraceWriter { get; set; }

        private IDictionary<String, Int32> FileIds
        {
            get
            {
                if (m_fileIds == null)
                {
                    m_fileIds = new Dictionary<String, Int32>(StringComparer.OrdinalIgnoreCase);
                }

                return m_fileIds;
            }
            set
            {
                m_fileIds = value;
            }
        }

        private List<String> FileNames
        {
            get
            {
                if (m_fileNames == null)
                {
                    m_fileNames = new List<String>();
                }

                return m_fileNames;
            }
            set
            {
                m_fileNames = value;
            }
        }

        internal void Error(TemplateValidationError error)
        {
            Errors.Add(error);
            TraceWriter.Error(error.Message);
        }

        internal void Error(
            TemplateToken value,
            Exception ex)
        {
            Error(value?.FileId, value?.Line, value?.Column, ex);
        }

        internal void Error(
            Int32? fileId,
            Int32? line,
            Int32? column,
            Exception ex)
        {
            var prefix = GetErrorPrefix(fileId, line, column);
            Errors.Add(prefix, ex);
            TraceWriter.Error(prefix, ex);
        }

        internal void Error(
            TemplateToken value,
            String message)
        {
            Error(value?.FileId, value?.Line, value?.Column, message);
        }

        internal void Error(
            Int32? fileId,
            Int32? line,
            Int32? column,
            String message)
        {
            var prefix = GetErrorPrefix(fileId, line, column);
            if (!String.IsNullOrEmpty(prefix))
            {
                message = $"{prefix} {message}";
            }

            Errors.Add(message);
            TraceWriter.Error(message);
        }

        internal INamedValueInfo[] GetExpressionNamedValues()
        {
            if (m_expressionValues?.Count > 0)
            {
                return m_expressionValues.Keys.Select(x => new NamedValueInfo<ContextValueNode>(x)).ToArray();
            }

            return null;
        }

        internal Int32 GetFileId(String file)
        {
            if (!FileIds.TryGetValue(file, out Int32 id))
            {
                id = FileIds.Count + 1;
                FileIds.Add(file, id);
                FileNames.Add(file);
                Memory.AddBytes(file);
            }

            return id;
        }

        internal String GetFileName(Int32 fileId)
        {
            return FileNames.Count >= fileId ? FileNames[fileId - 1] : null;
        }

        internal IReadOnlyList<String> GetFileTable()
        {
            return FileNames.AsReadOnly();
        }

        private String GetErrorPrefix(
            Int32? fileId,
            Int32? line,
            Int32? column)
        {
            var fileName = fileId.HasValue ? GetFileName(fileId.Value) : null;
            if (!String.IsNullOrEmpty(fileName))
            {
                if (line != null && column != null)
                {
                    return $"{fileName} {TemplateStrings.LineColumn(line, column)}:";
                }
                else
                {
                    return $"{fileName}:";
                }
            }
            else if (line != null && column != null)
            {
                return $"{TemplateStrings.LineColumn(line, column)}:";
            }
            else
            {
                return String.Empty;
            }
        }

        private TemplateValidationErrors m_fatal_errors;
        private TemplateValidationErrors m_errors;
        private TemplateValidationErrors m_warnings;
        private IList<IFunctionInfo> m_expressionFunctions;
        private IDictionary<String, Object> m_expressionValues;
        private IDictionary<String, Int32> m_fileIds;
        private List<String> m_fileNames;
        private IDictionary<String, Object> m_state;
    }
}
