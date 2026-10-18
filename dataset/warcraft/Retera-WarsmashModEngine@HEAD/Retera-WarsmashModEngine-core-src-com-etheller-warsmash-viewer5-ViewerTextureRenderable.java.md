---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/ViewerTextureRenderable.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5;

import com.badlogic.gdx.graphics.Texture;

public interface ViewerTextureRenderable {
	// TODO bind method makes more sense here

	int getGlTarget();

	int getGlHandle();

	class GdxViewerTextureRenderable implements ViewerTextureRenderable {
		private final com.badlogic.gdx.graphics.Texture gdxTexture;

		public GdxViewerTextureRenderable(final Texture texture) {
			this.gdxTexture = texture;
		}

		@Override
		public int getGlTarget() {
			return this.gdxTexture.glTarget;
		}

		@Override
		public int getGlHandle() {
			return this.gdxTexture.getTextureObjectHandle();
		}
	}
}
