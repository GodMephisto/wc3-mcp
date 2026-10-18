---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Script/TriggerItem.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Script
{
    public abstract partial class TriggerItem
    {
        internal TriggerItem(TriggerItemType triggerItemType)
        {
            Type = triggerItemType;
        }

        public TriggerItemType Type { get; private init; }

        public string Name { get; set; }

        public int Id { get; set; }

        public int ParentId { get; set; }

        public override string ToString() => Name;
    }
}