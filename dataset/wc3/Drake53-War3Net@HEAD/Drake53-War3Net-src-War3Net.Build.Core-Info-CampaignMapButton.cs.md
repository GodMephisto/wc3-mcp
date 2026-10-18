---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Info/CampaignMapButton.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Info
{
    public sealed partial class CampaignMapButton
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignMapButton"/> class.
        /// </summary>
        public CampaignMapButton()
        {
        }

        public int IsVisibleInitially { get; set; }

        public string Chapter { get; set; }

        public string Title { get; set; }

        public string MapFilePath { get; set; }

        public override string ToString() => Title;
    }
}