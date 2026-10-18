---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build/Extensions/DoodadDataExtensions.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Extensions
{
    public static class DoodadDataExtensions
    {
        public static string GetVariableName(this DoodadData doodadData)
        {
            return $"gg_dest_{doodadData.TypeId.ToRawcode()}_{doodadData.CreationNumber:D4}";
        }

        public static string GetDropItemsFunctionName(this DoodadData doodadData, int id)
        {
            return doodadData.MapItemTableId == -1
                ? $"Doodad{id:D6}_DropItems"
                : $"ItemTable{doodadData.MapItemTableId:D6}_DropItems";
        }
    }
}