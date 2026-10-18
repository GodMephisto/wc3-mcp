---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Script/TriggerData/TriggerCategory.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Script
{
    public sealed partial class TriggerData
    {
        public sealed class TriggerCategory
        {
            internal TriggerCategory(
                string categoryIdentifier,
                string displayText,
                string iconImageFile,
                bool hideCategoryName)
            {
                CategoryIdentifier = categoryIdentifier;
                DisplayText = displayText;
                IconImageFile = iconImageFile;
                HideCategoryName = hideCategoryName;
            }

            public string CategoryIdentifier { get; }

            public string DisplayText { get; }

            public string IconImageFile { get; }

            public bool HideCategoryName { get; }

            public override string ToString() => CategoryIdentifier;
        }
    }
}