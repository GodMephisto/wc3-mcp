---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Script/TriggerStrings.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Script
{
    public sealed partial class TriggerStrings
    {
        public const string FileExtension = ".wts";
        public const string CampaignFileName = "war3campaign.wts";
        public const string MapFileName = "war3map.wts";

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerStrings"/> class.
        /// </summary>
        public TriggerStrings()
        {
        }

        public List<TriggerString> Strings { get; init; } = new();

        public override string ToString() => $"{FileExtension} file";
    }
}