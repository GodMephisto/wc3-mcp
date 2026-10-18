---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/W3xSceneLightManager.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x;

import com.etheller.warsmash.viewer5.gl.DataTexture;

public interface W3xSceneLightManager {
    DataTexture getUnitLightsTexture();

    int getUnitLightCount();

    DataTexture getTerrainLightsTexture();

    int getTerrainLightCount();
}
