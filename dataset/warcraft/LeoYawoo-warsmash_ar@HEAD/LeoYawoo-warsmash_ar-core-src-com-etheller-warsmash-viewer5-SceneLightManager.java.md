---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/SceneLightManager.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5;

public interface SceneLightManager {
    void add(final SceneLightInstance lightInstance);

    void remove(final SceneLightInstance lightInstance);

    void update();
}
