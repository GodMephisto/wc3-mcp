---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq.View/Presenters/NavigationMeshRenderersEventArgs.cs"
ref: HEAD
---

using System.Collections.Generic;
using BananaMpq.View.Rendering;

namespace BananaMpq.View.Presenters
{
    class NavigationMeshRenderersEventArgs
    {
        public NavigationMeshRenderersEventArgs(IList<INavigationMeshRenderer> navMeshRenderers)
        {
            NavMeshRenderers = navMeshRenderers;
        }

        public IList<INavigationMeshRenderer> NavMeshRenderers { get; private set; }
    }
}