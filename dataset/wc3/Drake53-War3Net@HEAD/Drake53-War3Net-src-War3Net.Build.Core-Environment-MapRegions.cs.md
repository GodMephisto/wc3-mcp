---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Environment/MapRegions.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Environment
{
    public sealed partial class MapRegions
    {
        public const string FileExtension = ".w3r";
        public const string FileName = "war3map.w3r";

        private static readonly int ProtectionSignature = "FUCK".FromRawcode();

        /// <summary>
        /// Initializes a new instance of the <see cref="MapRegions"/> class.
        /// </summary>
        /// <param name="formatVersion"></param>
        public MapRegions(MapRegionsFormatVersion formatVersion)
        {
            FormatVersion = formatVersion;
        }

        public MapRegionsFormatVersion FormatVersion { get; set; }

        public bool Protected { get; set; }

        public List<Region> Regions { get; init; } = new();

        public override string ToString() => FileName;
    }
}