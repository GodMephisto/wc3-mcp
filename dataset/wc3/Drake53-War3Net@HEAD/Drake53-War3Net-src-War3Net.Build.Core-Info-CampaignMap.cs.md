---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Info/CampaignMap.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Info
{
    public sealed partial class CampaignMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignMap"/> class.
        /// </summary>
        public CampaignMap()
        {
        }

        public string Unk { get; set; }

        public string MapFilePath { get; set; }

        public override string ToString() => MapFilePath;
    }
}