---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/TextTagConfig.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x;

import com.badlogic.gdx.graphics.Color;

public class TextTagConfig {
	private final Color color;
	private final float[] velocity;
	private final float lifetime;
	private final float fadeStart;
	private final float height;

	public TextTagConfig(final Color color, final float[] velocity, final float lifetime, final float fadeStart,
			final float height) {
		this.color = color;
		this.velocity = velocity;
		this.lifetime = lifetime;
		this.fadeStart = fadeStart;
		this.height = height;
	}

	public Color getColor() {
		return this.color;
	}

	public float[] getVelocity() {
		return this.velocity;
	}

	public float getLifetime() {
		return this.lifetime;
	}

	public float getFadeStart() {
		return this.fadeStart;
	}

	public float getHeight() {
		return this.height;
	}
}
