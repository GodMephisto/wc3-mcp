---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Text/Script/TriggerStrings.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Script
{
    public sealed partial class TriggerStrings
    {
        internal TriggerStrings(StreamReader reader)
        {
            ReadFrom(reader);
        }

        internal void ReadFrom(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                Strings.Add(reader.ReadTriggerString());
            }

            // _encoding = reader.CurrentEncoding;
        }

        internal void WriteTo(StreamWriter writer)
        {
            foreach (var triggerString in Strings)
            {
                writer.WriteTriggerString(triggerString);
            }
        }
    }
}