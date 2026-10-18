---
type: vendor-doc
source_repo: "https://github.com/Darithos/W3ModelViewer"
source_path: "src/Wc3ModelViewer/App.xaml.cs"
ref: HEAD
---

using System.Windows;

namespace Wc3ModelViewer;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        BlpJpegCodec.Install();
    }
}
