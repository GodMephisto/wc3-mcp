---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq/Visualization/IHasVisualizableProperties.cs"
ref: HEAD
---

using System.Collections.Generic;
using System.Reflection;

namespace BananaMpq.Visualization
{
    public interface IHasVisualizableProperties
    {
        IEnumerable<PropertyInfo> VisualizableProperties { get; }
    }
}