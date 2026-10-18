---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Environment/PreviewIcon.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Environment
{
    public sealed partial class PreviewIcon
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewIcon"/> class.
        /// </summary>
        public PreviewIcon()
        {
        }

        public PreviewIconType IconType { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public Color Color { get; set; }

        public override string ToString() => IconType.ToString();
    }
}