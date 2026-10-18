---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Script/TriggerData/TriggerTypeDefault.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Script
{
    public sealed partial class TriggerData
    {
        public sealed class TriggerTypeDefault
        {
            internal TriggerTypeDefault(
                string variableType,
                string scriptText,
                string? displayText)
            {
                VariableType = variableType;
                ScriptText = scriptText;
                DisplayText = displayText;
            }

            public string VariableType { get; }

            public string ScriptText { get; }

            public string? DisplayText { get; }

            public override string ToString() => VariableType;
        }
    }
}