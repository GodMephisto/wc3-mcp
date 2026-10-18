---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Extensions/ForceDataExtensions.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Extensions
{
    public static class ForceDataExtensions
    {
        public static void SetPlayers(this ForceData force, params PlayerData[] players)
        {
            force.Players = new Bitmask32(players.Select(player => player.Id).ToArray());
        }
    }
}