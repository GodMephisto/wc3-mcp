---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Script/CustomTextTrigger.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Script
{
    public sealed partial class CustomTextTrigger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTextTrigger"/> class.
        /// </summary>
        public CustomTextTrigger()
        {
        }

        public string? Code { get; set; }

        public override string? ToString() => Code;
    }
}