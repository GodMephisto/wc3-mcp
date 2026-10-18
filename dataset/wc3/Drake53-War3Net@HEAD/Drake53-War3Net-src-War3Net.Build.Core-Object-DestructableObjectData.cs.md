---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Object/DestructableObjectData.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Object
{
    public sealed partial class DestructableObjectData
    {
        public const string FileExtension = ".w3b";
        public const string CampaignFileName = "war3campaign.w3b";
        public const string CampaignSkinFileName = "war3campaignSkin.w3b";
        public const string MapFileName = "war3map.w3b";
        public const string MapSkinFileName = "war3mapSkin.w3b";

        /// <summary>
        /// Initializes a new instance of the <see cref="DestructableObjectData"/> class.
        /// </summary>
        /// <param name="formatVersion"></param>
        public DestructableObjectData(ObjectDataFormatVersion formatVersion)
        {
            FormatVersion = formatVersion;
        }

        public ObjectDataFormatVersion FormatVersion { get; set; }

        public List<SimpleObjectModification> BaseDestructables { get; init; } = new();

        public List<SimpleObjectModification> NewDestructables { get; init; } = new();

        public override string ToString() => $"{FileExtension} file";
    }
}