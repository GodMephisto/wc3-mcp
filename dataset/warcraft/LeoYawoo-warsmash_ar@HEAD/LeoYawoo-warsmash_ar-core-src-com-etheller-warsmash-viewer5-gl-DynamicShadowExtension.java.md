---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/gl/DynamicShadowExtension.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.gl;

public interface DynamicShadowExtension {
    void glFramebufferTexture(int target, int attachment, int texture, int level);

    void glDrawBuffer(int mode);
}
