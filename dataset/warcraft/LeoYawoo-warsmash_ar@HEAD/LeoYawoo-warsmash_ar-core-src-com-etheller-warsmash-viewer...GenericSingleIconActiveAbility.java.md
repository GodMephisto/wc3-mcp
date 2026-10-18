---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/simulation/abilities/generic/GenericSingleIconActiveAbility.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.simulation.abilities.generic;

import com.etheller.warsmash.util.War3ID;

public interface GenericSingleIconActiveAbility extends CLevelingAbility {
    War3ID getAlias();

    int getBaseOrderId();

    boolean isToggleOn();

    int getUIGoldCost();

    int getUILumberCost();

    int getUIManaCost();
}
