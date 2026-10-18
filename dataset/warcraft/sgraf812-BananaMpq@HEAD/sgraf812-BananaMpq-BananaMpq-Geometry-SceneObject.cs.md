---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Geometry/SceneObject.cs"
ref: HEAD
---

using SharpDX;

namespace BananaMpq.Geometry
{
    public class SceneObject
    {
        public BoundingBox Bounds { get; set; }
        public MaterialFlags MaterialProperties { get; set; }
        public TriangleMesh Geometry { get; set; }
        public string Description { get; set; }
    }
}