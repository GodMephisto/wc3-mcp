---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Environment/MapCameras.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Environment
{
    public sealed partial class MapCameras
    {
        public const string FileExtension = ".w3c";
        public const string FileName = "war3map.w3c";

        /// <summary>
        /// Initializes a new instance of the <see cref="MapCameras"/> class.
        /// </summary>
        /// <param name="formatVersion"></param>
        public MapCameras(MapCamerasFormatVersion formatVersion, bool useNewFormat)
        {
            FormatVersion = formatVersion;
            UseNewFormat = useNewFormat;
        }

        public MapCamerasFormatVersion FormatVersion { get; set; }

        public bool UseNewFormat { get; set; }

        public List<Camera> Cameras { get; init; } = new();

        public override string ToString() => FileName;
    }
}