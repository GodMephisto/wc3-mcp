---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Layer/WdtRelated/MphdChunk.cs"
ref: HEAD
---

﻿using System.Collections.Generic;
using System.Linq;
using BananaMpq.Layer.Chunks;

namespace BananaMpq.Layer.WdtRelated
{
    public class MphdChunk : Chunk
    {
        public int Flags { get; private set; }

        internal unsafe MphdChunk(ChunkHeader* header) : base(header)
        {
            Flags  = *(int*)ChunkHeader.ChunkBegin(header);
        }

        public override IEnumerable<System.Reflection.PropertyInfo> VisualizableProperties
        {
            get
            {
                return base.VisualizableProperties.Concat(new[]
                {
                    typeof(MphdChunk).GetProperty("Flags")
                });
            }
        }
    }
}