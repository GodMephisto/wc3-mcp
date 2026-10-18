---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/WarsmashGdxMultiScreenGame.java"
ref: HEAD
---

package com.etheller.warsmash;

import com.badlogic.gdx.Game;
import com.badlogic.gdx.Gdx;
import com.etheller.warsmash.viewer5.CanvasProvider;

public class WarsmashGdxMultiScreenGame extends Game implements CanvasProvider {

	@Override
	public void create() {
	}

	@Override
	public float getWidth() {
		return Gdx.graphics.getWidth();
	}

	@Override
	public float getHeight() {
		return Gdx.graphics.getHeight();
	}

}
