---
type: vendor-doc
source_repo: "https://github.com/HanabishiRecca/FastMDX"
source_path: "FastMDX/src/Objects/TextureCoordinateSet.cs"
ref: HEAD
---

﻿namespace FastMDX {
    using static InnerBlocks;

    public struct TextureCoordinateSet : IDataRW {
        public Vec2[] TextureCoordinates;

        void IDataRW.ReadFrom(DataStream ds) {
            ds.CheckTag(UVBS);
            TextureCoordinates = ds.ReadStructArray<Vec2>();
        }

        void IDataRW.WriteTo(DataStream ds) {
            ds.WriteStruct(UVBS);
            ds.WriteStructArray(TextureCoordinates);
        }
    }
}
