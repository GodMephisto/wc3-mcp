---
type: vendor-doc
source_repo: "https://github.com/overtools/TACTLib"
source_path: "TACTLib/Core/Product/Fenris/SnoChild.cs"
ref: HEAD
---

using System.Runtime.InteropServices;

namespace TACTLib.Core.Product.Fenris;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x8)]
public record struct SnoChild {
	public uint SnoId { get; set; }
	public uint SubId { get; set; }
}
