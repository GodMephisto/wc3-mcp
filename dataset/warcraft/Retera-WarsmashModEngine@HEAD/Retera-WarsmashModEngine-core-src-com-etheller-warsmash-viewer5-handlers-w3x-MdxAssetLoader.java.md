---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/MdxAssetLoader.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x;

import java.awt.image.BufferedImage;

import com.etheller.warsmash.viewer5.handlers.mdx.MdxModel;

public interface MdxAssetLoader {
	public MdxModel loadModelMdx(final String path);

	public BufferedImage loadPathingTexture(String pathingTexture);
}
