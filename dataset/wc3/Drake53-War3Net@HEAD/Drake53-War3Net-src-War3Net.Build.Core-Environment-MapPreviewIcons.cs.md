---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Environment/MapPreviewIcons.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Environment
{
    public sealed partial class MapPreviewIcons
    {
        public const string FileExtension = ".mmp";
        public const string FileName = "war3map.mmp";

        /// <summary>
        /// Initializes a new instance of the <see cref="MapPreviewIcons"/> class.
        /// </summary>
        /// <param name="formatVersion"></param>
        public MapPreviewIcons(MapPreviewIconsFormatVersion formatVersion)
        {
            FormatVersion = formatVersion;
        }

        public MapPreviewIconsFormatVersion FormatVersion { get; set; }

        public List<PreviewIcon> Icons { get; init; } = new();

        public override string ToString() => FileName;
    }
}