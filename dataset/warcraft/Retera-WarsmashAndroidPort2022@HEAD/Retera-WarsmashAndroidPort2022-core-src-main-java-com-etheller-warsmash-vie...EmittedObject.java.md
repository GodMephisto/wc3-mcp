---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/viewer5/EmittedObject.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5;

public abstract class EmittedObject<MODEL_INSTANCE extends ModelInstance, EMITTER extends Emitter<MODEL_INSTANCE, ? extends EmittedObject<MODEL_INSTANCE, EMITTER>>> {
	public abstract void update(float dt);

	public float health;
	public EMITTER emitter;
	public int index;

	public EmittedObject(final EMITTER emitter) {
		this.emitter = emitter;
	}

	protected abstract void bind(int flags);
}
