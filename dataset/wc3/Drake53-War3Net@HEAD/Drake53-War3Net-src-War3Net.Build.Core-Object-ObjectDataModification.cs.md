---
type: vendor-doc
source_repo: "https://github.com/Drake53/War3Net"
source_path: "src/War3Net.Build.Core/Object/ObjectDataModification.cs"
ref: HEAD
---

﻿namespace War3Net.Build.Object
{
    public abstract partial class ObjectDataModification
    {
        internal ObjectDataModification()
        {
        }

        public int Id { get; set; }

        public ObjectDataType Type { get; set; }

        public object Value { get; set; }

        internal int SanityCheck { get; set; }

        public int ValueAsInt => Value is int i ? i : throw new InvalidOperationException();

        public float ValueAsFloat => Value is float f ? f : throw new InvalidOperationException();

        public string ValueAsString => Value is string s ? s : throw new InvalidOperationException();

        [Obsolete]
        public bool ValueAsBool => Value is bool b ? b : throw new InvalidOperationException();

        [Obsolete]
        public char ValueAsChar => Value is char c ? c : throw new InvalidOperationException();

        public override string ToString() => Id.ToRawcode();
    }
}