---
type: vendor-doc
source_repo: "https://github.com/whiteqitiandi/GetObjectsInJ"
source_path: "src/GetObjectsInJ/Program.cs"
ref: HEAD
---

namespace GetObjectsInJ;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
