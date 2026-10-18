---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Widget/InventoryItemData.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Widget
{
    public sealed partial class InventoryItemData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryItemData"/> class.
        /// </summary>
        public InventoryItemData()
        {
        }

        // 0-indexed
        public int Slot { get; set; }

        // 0x00000000 == none
        public int ItemId { get; set; }

        public override string ToString() => ItemId.ToRawcode();
    }
}