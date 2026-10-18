---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build/ArchiveBuildingEventArgs.cs"
ref: HEAD
---

﻿namespace War3Net.Build
{
    public sealed class ArchiveBuildingEventArgs : EventArgs
    {
        public ArchiveBuildingEventArgs(ICollection<MpqFile> mpqFiles)
        {
            MpqFiles = mpqFiles;
        }

        public ICollection<MpqFile> MpqFiles { get; set; }
    }
}