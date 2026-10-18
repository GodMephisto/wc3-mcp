---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq.View/Rendering/ISceneRenderer.cs"
ref: HEAD
---

using BananaMpq.Geometry;

namespace BananaMpq.View.Rendering
{
    public interface ISceneRenderer
    {
        void Render(Scene scene);
    }
}