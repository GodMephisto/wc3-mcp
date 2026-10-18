---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Geometry/IBVHTree.cs"
ref: HEAD
---

﻿using System.Collections.Generic;
using SharpDX;

namespace BananaMpq.Geometry
{
    public interface IBVHTree
    {
        BoundingBox Bounds { get; }
        IEnumerable<IBVHNode> GetIntersections(ref BoundingBox bounds);
        IBVHNode GetIntersectedNode(ref Ray ray); 
    }
}