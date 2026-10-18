---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Extensions/StringReaderExtensions.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Extensions
{
    public static class StringReaderExtensions
    {
        public static TriggerData ReadTriggerData(this StringReader reader) => new TriggerData(reader);
    }
}