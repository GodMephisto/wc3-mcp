---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/rendersim/RenderEffect.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.rendersim;

import com.etheller.warsmash.viewer5.handlers.w3x.War3MapViewer;

public interface RenderEffect {
    boolean updateAnimations(final War3MapViewer war3MapViewer, float deltaTime);
}
