---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/gl/AudioExtension.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.gl;

import com.badlogic.gdx.audio.Sound;
import com.etheller.warsmash.viewer5.AudioContext;

public interface AudioExtension {
	AudioContext createContext(boolean world);

	float getDuration(Sound sound);

	long play(Sound buffer, final float volume, final float pitch, final float x, final float y, final float z,
			final boolean is3DSound, float maxDistance, float refDistance, boolean looping);
}
