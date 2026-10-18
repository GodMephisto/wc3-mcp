---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Layer/Chunks/Chunk.cs"
ref: HEAD
---

using System.Collections.Generic;
using System.Reflection;
using BananaMpq.Visualization;

namespace BananaMpq.Layer.Chunks
{
    public class Chunk : IHasVisualizableProperties
    {
        public string Magic { get; private set; }
        public int Size { get; private set; }

        internal unsafe Chunk(ChunkHeader* header)
        {
            Magic = header->Magic;
            Size = header->Size;
        }

        public override string ToString()
        {
            return string.Format("{0}: 0x{1:X} bytes", Magic, Size);
        }

        #region Implementation of IHasVisualizableProperties

        public virtual IEnumerable<PropertyInfo> VisualizableProperties
        {
            get
            {
                var type = typeof(Chunk);
                return new[] { type.GetProperty("Magic"), type.GetProperty("Size") };
            }
        }

        #endregion
    }
}