---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Extensions/TriggerStringsExtensions.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Extensions
{
    public static class TriggerStringsExtensions
    {
        public static bool TryGetValue(this TriggerStrings triggerStrings, string? trigstr, out string? value)
        {
            if (trigstr is not null &&
                trigstr.StartsWith("TRIGSTR_", StringComparison.Ordinal) &&
                uint.TryParse(trigstr["TRIGSTR_".Length..], out var key))
            {
                var triggerString = triggerStrings.Strings.FirstOrDefault(s => s.Key == key);
                if (triggerString is not null)
                {
                    value = triggerString.Value;
                    return true;
                }
            }

            value = null;
            return false;
        }
    }
}