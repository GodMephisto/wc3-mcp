---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/EmittedObject.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5;

public abstract class EmittedObject<MODEL_INSTANCE extends ModelInstance, EMITTER extends Emitter<MODEL_INSTANCE, ? extends EmittedObject<MODEL_INSTANCE, EMITTER>>> {
    public float health;
    public final EMITTER emitter;
    public int index;

    public EmittedObject(final EMITTER emitter) {
        this.emitter = emitter;
    }

    public abstract void update(float dt);

    protected abstract void bind(int flags);
}
