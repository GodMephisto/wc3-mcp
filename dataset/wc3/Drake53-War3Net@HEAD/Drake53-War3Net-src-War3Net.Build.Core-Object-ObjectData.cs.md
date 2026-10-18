---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Object/ObjectData.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Object
{
    public sealed partial class ObjectData
    {
        public const string FileExtension = ".w3o";

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectData"/> class.
        /// </summary>
        /// <param name="formatVersion"></param>
        public ObjectData(ObjectDataFormatVersion formatVersion)
        {
            FormatVersion = formatVersion;
        }

        public ObjectDataFormatVersion FormatVersion { get; set; }

        public UnitObjectData? UnitData { get; set; }

        public ItemObjectData? ItemData { get; set; }

        public DestructableObjectData? DestructableData { get; set; }

        public DoodadObjectData? DoodadData { get; set; }

        public AbilityObjectData? AbilityData { get; set; }

        public BuffObjectData? BuffData { get; set; }

        public UpgradeObjectData? UpgradeData { get; set; }

        public override string ToString() => $"{FileExtension} file";
    }
}