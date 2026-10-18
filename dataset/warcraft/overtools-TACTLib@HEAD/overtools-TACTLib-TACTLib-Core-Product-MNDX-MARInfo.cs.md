---
type: vendor-doc
source_repo: "https://github.com/overtools/TACTLib"
source_path: "TACTLib/Core/Product/MNDX/MARInfo.cs"
ref: HEAD
---

﻿// https://raw.githubusercontent.com/WoW-Tools/CascLib/master/CascLib/RootHandlers/MNDXRootHandler.cs

namespace TACTLib.Core.Product.MNDX
{
    internal struct MARInfo
    {
        public int MarIndex;
        public int MarDataSize;
        public int MarDataSizeHi;
        public int MarDataOffset;
        public int MarDataOffsetHi;
    }
}
