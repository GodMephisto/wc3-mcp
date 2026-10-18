---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Object/SimpleObjectModification.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Object
{
    public sealed partial class SimpleObjectModification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleObjectModification"/> class.
        /// </summary>
        public SimpleObjectModification()
        {
        }

        public int OldId { get; set; }

        public int NewId { get; set; }

        public List<int> Unk { get; init; } = new();

        public List<SimpleObjectDataModification> Modifications { get; init; } = new();

        public override string ToString() => NewId == 0 ? OldId.ToRawcode() : $"{NewId.ToRawcode()}:{OldId.ToRawcode()}";
    }
}