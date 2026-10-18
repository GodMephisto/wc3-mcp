---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Serialization/Binary/Configuration/GameConfiguration.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Configuration
{
    public sealed partial class GameConfiguration
    {
        internal GameConfiguration(BinaryReader reader)
        {
            ReadFrom(reader);
        }

        internal void ReadFrom(BinaryReader reader)
        {
            FormatVersion = reader.ReadInt32<GameConfigurationFormatVersion>();
            Flags = reader.ReadInt32<GameConfigurationFlags>();
            GameSpeedMultiplier = reader.ReadUInt32();
            MapPath = reader.ReadChars();

            nint playerInfoCount = reader.ReadInt32();
            for (nint i = 0; i < playerInfoCount; i++)
            {
                PlayerInfo.Add(reader.ReadGameConfigurationPlayerInfo(FormatVersion));
            }
        }

        internal void WriteTo(BinaryWriter writer)
        {
            writer.Write((int)FormatVersion);
            writer.Write((int)Flags);
            writer.Write(GameSpeedMultiplier);
            writer.WriteString(MapPath);

            writer.Write(PlayerInfo.Count);
            foreach (var playerInfo in PlayerInfo)
            {
                writer.Write(playerInfo, FormatVersion);
            }
        }
    }
}