---
type: vendor-doc
source_repo: "https://github.com/Indaxia/WLPM"
source_path: "Repository/Unzipper.cs"
ref: HEAD
---

using System.IO;
using System.IO.Compression;

namespace wlpm.Repository
{
    public class Unzipper
    {
        public static void unzipFile(string filePath, string targetDir, string unwrapFirstSubdirectoryTo = "")
        {
            ZipFile.ExtractToDirectory(filePath, targetDir);
            if(unwrapFirstSubdirectoryTo.Length > 0) {
                string [] subDirectories = Directory.GetDirectories(targetDir);
                if(subDirectories.Length > 0) {
                    Directory.Move(subDirectories[0], unwrapFirstSubdirectoryTo);
                }
            }
        }
    }
}