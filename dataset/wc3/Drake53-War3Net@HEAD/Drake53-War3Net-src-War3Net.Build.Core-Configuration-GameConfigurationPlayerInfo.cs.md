---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Configuration/GameConfigurationPlayerInfo.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Configuration
{
    public sealed partial class GameConfigurationPlayerInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameConfigurationPlayerInfo"/> class.
        /// </summary>
        public GameConfigurationPlayerInfo()
        {
        }

        public int PlayerSlotId { get; set; }

        public int ForceId { get; set; }

        public GameConfigurationPlayerRace PlayerRace { get; set; }

        public KnownPlayerColor PlayerColor { get; set; }

        public int Handicap { get; set; }

        public GameConfigurationPlayerInfoFlags PlayerInfoFlags { get; set; }

        public AIDifficulty AIDifficulty { get; set; }

        public string CustomAIFilePath { get; set; }

        public override string ToString() => $"Player slot #{PlayerSlotId + 1}";
    }
}