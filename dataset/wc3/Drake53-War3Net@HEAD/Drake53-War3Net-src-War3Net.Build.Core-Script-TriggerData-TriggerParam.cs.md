---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Script/TriggerData/TriggerParam.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Script
{
    public sealed partial class TriggerData
    {
        public sealed class TriggerParam
        {
            internal TriggerParam(
                string parameterName,
                int gameVersion,
                string variableType,
                string scriptText,
                string displayText)
            {
                ParameterName = parameterName;
                GameVersion = gameVersion;
                VariableType = variableType;
                ScriptText = scriptText;
                DisplayText = displayText;
            }

            public string ParameterName { get; }

            public int GameVersion { get; }

            public string VariableType { get; }

            public string ScriptText { get; }

            public string DisplayText { get; }

            public override string ToString() => ParameterName;
        }
    }
}