---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/simulation/abilities/generic/CLevelingAbility.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.simulation.abilities.generic;

import com.etheller.warsmash.viewer5.handlers.w3x.simulation.abilities.CAbility;

public interface CLevelingAbility extends CAbility {
    int getLevel();

    void setLevel(int level);
}
