---
type: vendor-doc
source_repo: "https://github.com/HanabishiRecca/FastMDX"
source_path: "FastMDX/src/Objects/Helper.cs"
ref: HEAD
---

﻿namespace FastMDX {
    public struct Helper : IDataRW {
        public Node Node;

        void IDataRW.ReadFrom(DataStream ds) => ds.ReadData(ref Node);

        void IDataRW.WriteTo(DataStream ds) => ds.WriteData(ref Node);
    }
}
