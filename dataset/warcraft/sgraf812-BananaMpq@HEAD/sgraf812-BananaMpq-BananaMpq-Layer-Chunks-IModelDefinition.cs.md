---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Layer/Chunks/IModelDefinition.cs"
ref: HEAD
---

using System.Collections.Generic;
using BananaMpq.Layer.WmoRelated;
using SharpDX;

namespace BananaMpq.Layer.Chunks
{
    public interface IModelDefinition
    {
        int? Id { get; }
        Vector3 Position { get; }
        Matrix GetTranform();
        string GetModelReference(IList<StringReference> references);
        IEnumerable<IModelDefinition> FilterDoodadSetDefinitions(IList<DoodadSet> sets, IEnumerable<IModelDefinition> doodadDefs);
    }
}