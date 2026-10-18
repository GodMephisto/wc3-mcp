---
type: vendor-doc
source_repo: "https://github.com/overtools/TACTLib"
source_path: "TACTLib/Core/Product/Fenris/SnoFile.cs"
ref: HEAD
---

namespace TACTLib.Core.Product.Fenris;

public record SnoFile(uint Id, SnoType Type, uint SubId = 0);
