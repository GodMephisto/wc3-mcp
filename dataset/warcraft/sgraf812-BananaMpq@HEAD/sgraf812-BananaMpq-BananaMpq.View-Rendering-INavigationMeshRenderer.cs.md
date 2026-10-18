---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq.View/Rendering/INavigationMeshRenderer.cs"
ref: HEAD
---

namespace BananaMpq.View.Rendering
{
    public interface INavigationMeshRenderer
    {
        void Render(object buildResult);
        void ClearCache();
    }
}