---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq.View/Views/IChunkHierarchyView.cs"
ref: HEAD
---

using System.Collections.Generic;
using BananaMpq.View.Models;

namespace BananaMpq.View.Views
{
    public interface IChunkHierarchyView
    {
        IEnumerable<ChunkHierarchyModel> Hierarchy { set; }
    }
}