---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Script/TriggerData/DefaultTriggerCategory.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Script
{
    public sealed partial class TriggerData
    {
        public sealed class DefaultTriggerCategory
        {
            internal DefaultTriggerCategory(string categoryName)
            {
                CategoryName = categoryName;
            }

            public string CategoryName { get; }

            public override string ToString() => CategoryName;
        }
    }
}