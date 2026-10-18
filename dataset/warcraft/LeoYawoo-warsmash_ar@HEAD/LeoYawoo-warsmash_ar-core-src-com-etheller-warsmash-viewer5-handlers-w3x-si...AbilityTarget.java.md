---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/simulation/abilities/targeting/AbilityTarget.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.simulation.abilities.targeting;

public interface AbilityTarget {
    float getX();

    float getY();

    <T> T visit(AbilityTargetVisitor<T> visitor);
}
