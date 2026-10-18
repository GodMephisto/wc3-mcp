---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/SdArrayDescriptor.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

public interface SdArrayDescriptor<TYPE> {
    SdArrayDescriptor<Object> GENERIC = Object[]::new;
    SdArrayDescriptor<float[]> FLOAT_ARRAY = float[][]::new;
    SdArrayDescriptor<long[]> LONG_ARRAY = long[][]::new;

    TYPE[] create(int size);
}
