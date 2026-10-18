---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/rendersim/LockTarget.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.rendersim;

import com.badlogic.gdx.math.Quaternion;
import com.etheller.warsmash.viewer5.GenericNode;

public interface LockTarget {
	float getX();

	float getY();

	float getZ();

	void apply(GenericNode sourceNode, Quaternion overrideWorldRotation);
}
