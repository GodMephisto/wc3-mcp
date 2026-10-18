---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/simulation/abilities/targeting/AbilityPointTarget.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.simulation.abilities.targeting;

import com.badlogic.gdx.math.Vector2;

public class AbilityPointTarget extends Vector2 implements AbilityTarget {

    public AbilityPointTarget() {
        super();
    }

    public AbilityPointTarget(final float x, final float y) {
        super(x, y);
    }

    public AbilityPointTarget(final Vector2 v) {
        super(v);
    }

    @Override
    public float getX() {
        return this.x;
    }

    @Override
    public float getY() {
        return this.y;
    }

    @Override
    public <T> T visit(final AbilityTargetVisitor<T> visitor) {
        return visitor.accept(this);
    }

}
