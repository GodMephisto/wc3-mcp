---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Geometry/Builders/WmoBuildResults.cs"
ref: HEAD
---

﻿using System.Collections.Generic;

namespace BananaMpq.Geometry.Builders
{
    public class WmoBuildResults
    {
        public IEnumerable<SceneObject> GroupObjects { get; set; }
        public IEnumerable<SceneObject> Liquids { get; set; }
        public IEnumerable<SceneObject> Doodads { get; set; }
    }
}