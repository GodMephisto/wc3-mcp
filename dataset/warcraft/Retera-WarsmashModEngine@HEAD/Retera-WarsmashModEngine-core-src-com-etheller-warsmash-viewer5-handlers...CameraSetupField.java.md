---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/camera/CameraSetupField.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.camera;

public class CameraSetupField {
	private final float value;
	private final float rate;

	public CameraSetupField(final float value, final float rate) {
		this.value = value;
		this.rate = rate;
	}

	public float getValue() {
		return this.value;
	}

	public float getRate() {
		return this.rate;
	}

	public float applyAtRate(final float previousValue) {
		return GameCameraManager.applyAtRate(previousValue, this.value, this.rate);
	}
}
