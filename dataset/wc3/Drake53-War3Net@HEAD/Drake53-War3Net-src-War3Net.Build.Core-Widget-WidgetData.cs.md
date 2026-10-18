---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Widget/WidgetData.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Widget
{
    public abstract class WidgetData
    {
        internal WidgetData()
        {
        }

        public int TypeId { get; set; }

        public int Variation { get; set; }

        public Vector3 Position { get; set; }

        /// <summary>
        /// The unit's facing angle (in radians).
        /// </summary>
        public float Rotation { get; set; }

        public Vector3 Scale { get; set; }

        public int SkinId { get; set; }

        public int MapItemTableId { get; set; } = -1;

        public List<RandomItemSet> ItemTableSets { get; init; } = new();

        public int CreationNumber { get; set; }

        public override string ToString() => TypeId.ToRawcode();
    }
}