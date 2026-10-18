---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Geometry/Scene.cs"
ref: HEAD
---

using System.Collections.Generic;
using BananaMpq.Layer;

namespace BananaMpq.Geometry
{
    public class Scene
    {
        public Adt Adt { get; set; }
        public IEnumerable<SceneObject> Terrain { get; set; }
        public IEnumerable<SceneObject> Liquids { get; set; }
        public IEnumerable<SceneObject> Doodads { get; set; }
        public IEnumerable<SceneObject> Wmos { get; set; }
    }
}