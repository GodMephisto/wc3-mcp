---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/AudioBufferSource.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5;

import com.badlogic.gdx.audio.Sound;
import com.etheller.warsmash.viewer5.gl.Extensions;

public class AudioBufferSource {
	public Sound buffer;
	private AudioPanner panner;

	public void connect(final AudioPanner panner) {
		this.panner = panner;
	}

	public long start(final int value, final float volume, final float pitch, final boolean looping) {
		if (this.buffer != null) {
			if (!this.panner.listener.is3DSupported() || this.panner.isWithinListenerDistance()) {
				return Extensions.audio.play(this.buffer, volume, pitch, this.panner.x, this.panner.y, this.panner.z,
						this.panner.listener.is3DSupported(), this.panner.maxDistance, this.panner.refDistance,
						looping);
			}
		}
		return -1;
	}
}
