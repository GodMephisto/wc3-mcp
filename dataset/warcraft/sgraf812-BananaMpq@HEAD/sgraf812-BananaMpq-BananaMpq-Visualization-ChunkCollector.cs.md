---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Visualization/ChunkCollector.cs"
ref: HEAD
---

using System.Collections.Generic;
using BananaMpq.Layer.Chunks;

namespace BananaMpq.Visualization
{
    unsafe delegate Chunk ChunkFactory(ChunkHeader* header);

    unsafe class ChunkCollector
    {
        public static IEnumerable<Chunk> CreateChunks(byte* cur, byte* end, ChunkFactory factory)
        {
            var chunks = new List<Chunk>();
            while (cur < end)
            {
                var header = (ChunkHeader*)cur;
                var chunk = factory(header);
                if (chunk != null)
                {
                    chunks.Add(chunk);
                }
                cur = header->NextChunk(cur);
            }
            return chunks;
        }
    }
}