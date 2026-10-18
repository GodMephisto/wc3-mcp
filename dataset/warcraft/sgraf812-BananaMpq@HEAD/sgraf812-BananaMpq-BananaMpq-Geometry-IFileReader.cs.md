---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Geometry/IFileReader.cs"
ref: HEAD
---

namespace BananaMpq.Geometry
{
    public interface IFileReader
    {
        byte[] Read(string fileName);
    }
}