---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/simulation/abilities/types/CAbilityTypeLevelData.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.simulation.abilities.types;

import com.etheller.warsmash.viewer5.handlers.w3x.simulation.combat.CTargetType;

import java.util.EnumSet;

public class CAbilityTypeLevelData {
    private final EnumSet<CTargetType> targetsAllowed;

    public CAbilityTypeLevelData(final EnumSet<CTargetType> targetsAllowed) {
        this.targetsAllowed = targetsAllowed;
    }

    public EnumSet<CTargetType> getTargetsAllowed() {
        return this.targetsAllowed;
    }
}
