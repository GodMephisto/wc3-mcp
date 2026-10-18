---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build/CompileResult.cs"
ref: HEAD
---

﻿namespace War3Net.Build
{
    public sealed class CompileResult
    {
        private readonly bool _success;
        private readonly ImmutableArray<Diagnostic> _diagnostics;

        internal CompileResult(EmitResult emitResult)
        {
            _success = emitResult.Success;
            _diagnostics = emitResult.Diagnostics;
        }

        internal CompileResult(bool success, IEnumerable<Diagnostic>? diagnostics)
        {
            _success = success;
            _diagnostics = diagnostics?.ToImmutableArray() ?? ImmutableArray<Diagnostic>.Empty;
        }

        public bool Success => _success;

        public ImmutableArray<Diagnostic> Diagnostics => _diagnostics;

        public static implicit operator bool(CompileResult compileResult)
        {
            return compileResult?.ToBoolean() ?? false;
        }

        public bool ToBoolean()
        {
            return _success;
        }
    }
}