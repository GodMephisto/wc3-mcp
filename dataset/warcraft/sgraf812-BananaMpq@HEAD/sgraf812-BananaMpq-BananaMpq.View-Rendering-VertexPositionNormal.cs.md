---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq.View/Rendering/VertexPositionNormal.cs"
ref: HEAD
---

using SharpDX;
using SharpDX.Direct3D9;

namespace BananaMpq.View.Rendering
{
    public struct VertexPositionNormal
    {
        public const VertexFormat Format = VertexFormat.Position | VertexFormat.Normal;
        public Vector3 Position;
        public Vector3 Normal;
    }
}