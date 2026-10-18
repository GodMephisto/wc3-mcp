---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Geometry/IBVHNode.cs"
ref: HEAD
---

﻿using SharpDX;

namespace BananaMpq.Geometry
{
    public interface IBVHNode
    {
        TriangleWithNormal Triangle { get; }
        MaterialFlags Flags { get; }
        BoundingBox Bounds { get; }
        Vector3? GetIntersectingPoint(ref Ray ray);
    }
}