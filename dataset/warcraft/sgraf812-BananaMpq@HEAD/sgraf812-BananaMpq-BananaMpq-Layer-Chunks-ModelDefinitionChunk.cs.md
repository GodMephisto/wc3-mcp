---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Layer/Chunks/ModelDefinitionChunk.cs"
ref: HEAD
---

using System.Collections.Generic;
using System.Linq;
using BananaMpq.Layer.AdtRelated;

namespace BananaMpq.Layer.Chunks
{
    public class ModelDefinitionChunk : Chunk
    {
        protected const float MapOriginOffset = 32.0f*Adt.AdtWidth;

        public IList<IModelDefinition> Definitions { get; protected set; }

        internal unsafe ModelDefinitionChunk(ChunkHeader* header) : base(header)
        {
        }

        public override IEnumerable<System.Reflection.PropertyInfo> VisualizableProperties
        {
            get { return base.VisualizableProperties.Concat(new[] { typeof(MddfChunk).GetProperty("Definitions") }); }
        }
    }
}