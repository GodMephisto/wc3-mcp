---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/simulation/abilities/generic/AbstractGenericNoIconAbility.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.simulation.abilities.generic;

import com.etheller.warsmash.util.War3ID;
import com.etheller.warsmash.viewer5.handlers.w3x.simulation.abilities.CAbilityVisitor;

public abstract class AbstractGenericNoIconAbility extends AbstractGenericAliasedAbility
        implements GenericNoIconAbility {

    public AbstractGenericNoIconAbility(final int handleId, final War3ID alias) {
        super(handleId, alias);
    }

    @Override
    public <T> T visit(final CAbilityVisitor<T> visitor) {
        return visitor.accept(this);
    }

}
