---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Visualization/IChunkCollector.cs"
ref: HEAD
---

using System.Collections.Generic;
using BananaMpq.Layer.Chunks;

namespace BananaMpq.Visualization
{
    public interface IChunkCollector
    {
        IEnumerable<Chunk> Chunks { get; }
    }
}