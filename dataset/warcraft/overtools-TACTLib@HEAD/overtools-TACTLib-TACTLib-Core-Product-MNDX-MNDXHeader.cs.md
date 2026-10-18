---
type: vendor-doc
source_repo: "https://github.com/overtools/TACTLib"
source_path: "TACTLib/Core/Product/MNDX/MNDXHeader.cs"
ref: HEAD
---

﻿// https://raw.githubusercontent.com/WoW-Tools/CascLib/master/CascLib/RootHandlers/MNDXRootHandler.cs

namespace TACTLib.Core.Product.MNDX
{
    internal struct MNDXHeader
    {
        public int Signature;                            // 'MNDX'
        public int HeaderVersion;                        // Must be <= 2
        public int FormatVersion;
    }
}
