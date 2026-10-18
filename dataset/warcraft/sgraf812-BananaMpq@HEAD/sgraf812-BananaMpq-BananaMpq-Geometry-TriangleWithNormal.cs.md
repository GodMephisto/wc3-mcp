---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Geometry/TriangleWithNormal.cs"
ref: HEAD
---

﻿using SharpDX;

namespace BananaMpq.Geometry
{
    public struct TriangleWithNormal
    {
        public Vector3 A, B, C;
        public Vector3 Normal;
    }
}