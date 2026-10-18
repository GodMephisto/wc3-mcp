---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/BatchedInstance.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5;

/**
 * A batched model instance.
 */
public abstract class BatchedInstance extends ModelInstance {

	public BatchedInstance(final Model model) {
		super(model);
	}

	@Override
	public boolean isBatched() {
		return true;
	}
}
