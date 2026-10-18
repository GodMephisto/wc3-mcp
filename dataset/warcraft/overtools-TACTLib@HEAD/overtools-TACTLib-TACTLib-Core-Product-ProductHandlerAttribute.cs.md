---
type: vendor-doc
source_repo: "https://github.com/overtools/TACTLib"
source_path: "TACTLib/Core/Product/ProductHandlerAttribute.cs"
ref: HEAD
---

using System;

namespace TACTLib.Core.Product {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ProductHandlerAttribute : Attribute {
        public TACTProduct Product;

        public ProductHandlerAttribute(TACTProduct product) {
            Product = product;
        }
    }
}
