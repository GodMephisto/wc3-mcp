---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Common/GameBuild.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Common
{
    public class GameBuild
    {
        [JsonInclude]
        public GameExpansion GameExpansion { get; private set; }

        [JsonInclude]
        public GameReleaseType GameReleaseType { get; private set; }

        [JsonInclude]
        public DateTime? ReleaseDate { get; private set; }

        [JsonInclude]
        public GamePatch? GamePatch { get; private set; }

        [JsonInclude]
        public Version Version { get; private set; }

        [JsonInclude]
        public EditorVersion? EditorVersion { get; private set; }

        public override string ToString() => $"{GameExpansion} v{Version}";
    }
}