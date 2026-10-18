---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build/BuildResult.cs"
ref: HEAD
---

﻿namespace War3Net.Build
{
    public sealed class BuildResult
    {
        private readonly bool _success;
        private readonly ImmutableArray<Diagnostic> _diagnostics;

        internal BuildResult(bool success, CompileResult? compileResult, IEnumerable<Diagnostic> diagnostics)
        {
            if (compileResult is null)
            {
                _success = success;
                _diagnostics = diagnostics.ToImmutableArray();
            }
            else
            {
                _success = success && compileResult.Success;
                _diagnostics = compileResult.Diagnostics.Concat(diagnostics).ToImmutableArray();
            }
        }

        public bool Success => _success;

        public ImmutableArray<Diagnostic> Diagnostics => _diagnostics;

        public static implicit operator bool(BuildResult buildResult)
        {
            return buildResult?.ToBoolean() ?? false;
        }

        public bool ToBoolean()
        {
            return _success;
        }
    }
}