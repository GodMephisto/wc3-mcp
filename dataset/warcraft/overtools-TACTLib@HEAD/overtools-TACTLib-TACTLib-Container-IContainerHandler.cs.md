---
type: vendor-doc
source_repo: "https://github.com/overtools/TACTLib"
source_path: "TACTLib/Container/IContainerHandler.cs"
ref: HEAD
---

using System;

namespace TACTLib.Container {
    public interface IContainerHandler {
        ArraySegment<byte>? OpenEKey(FullEKey ekey, int eSize);
        bool CheckResidency(FullEKey ekey);
    }
}