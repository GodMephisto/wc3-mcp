---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq.View/Views/ISceneView.cs"
ref: HEAD
---

using System;

namespace BananaMpq.View.Views
{
    public interface ISceneView
    {
        IntPtr WindowHandle { get; }
        IDisposable StartRenderPass(IntPtr surfacePointer);
        void OpenTileSelectionDialog();
    }
}