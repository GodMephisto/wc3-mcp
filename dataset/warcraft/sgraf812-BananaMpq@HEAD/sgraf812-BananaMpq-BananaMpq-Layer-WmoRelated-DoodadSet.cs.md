---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Layer/WmoRelated/DoodadSet.cs"
ref: HEAD
---

﻿using System.Collections.Generic;
using System.Reflection;
using BananaMpq.Visualization;

namespace BananaMpq.Layer.WmoRelated
{
    public class DoodadSet : IHasVisualizableProperties
    {
        public string Name { get; set; }
        public int FirstDefinition { get; set; }
        public int DefinitionCount { get; set; }

        #region IHasVisualizableProperties Members

        public IEnumerable<PropertyInfo> VisualizableProperties
        {
            get
            {
                var t = typeof(DoodadSet);
                return new[]
                {
                    t.GetProperty("Name"),
                    t.GetProperty("FirstDefinition"),
                    t.GetProperty("DefinitionCount")
                };
            }
        }

        #endregion

    }
}