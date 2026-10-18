---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Extensions/StreamReaderExtensions.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Extensions
{
    public static class StreamReaderExtensions
    {
        public static TriggerStrings ReadTriggerStrings(this StreamReader reader) => new TriggerStrings(reader);

        public static TriggerString ReadTriggerString(this StreamReader reader) => new TriggerString(reader);
    }
}