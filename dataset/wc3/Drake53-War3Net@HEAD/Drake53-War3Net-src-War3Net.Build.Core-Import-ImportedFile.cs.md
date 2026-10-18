---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Import/ImportedFile.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Import
{
    public sealed partial class ImportedFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportedFile"/> class.
        /// </summary>
        public ImportedFile()
        {
        }

        public ImportedFileFlags Flags { get; set; }

        public string FullPath { get; set; }

        public override string ToString() => FullPath;
    }
}