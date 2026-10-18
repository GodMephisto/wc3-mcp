---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Geometry/TriangleMesh.cs"
ref: HEAD
---

using SharpDX;

namespace BananaMpq.Geometry
{
    public class TriangleMesh
    {
        public Vector3[] Vertices { get; set; }
        public IndexedTriangleWithNormal[] Triangles { get; set; }
    }
}