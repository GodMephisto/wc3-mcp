---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq.View/Rendering/IRenderBatch.cs"
ref: HEAD
---

namespace BananaMpq.View.Rendering
{
    public interface IRenderBatch
    {
        void Record<TIndex, TVertex>(TIndex[] indices, TVertex[] vertices)
            where TIndex : struct
            where TVertex : struct;
        void Record<TIndex>(TIndex[] indices, IRenderBatch useVerticesFrom)
            where TIndex : struct;
        void Render();
    }
}