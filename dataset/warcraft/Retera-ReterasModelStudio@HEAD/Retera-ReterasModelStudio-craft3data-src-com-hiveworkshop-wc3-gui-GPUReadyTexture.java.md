---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/GPUReadyTexture.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui;

import java.nio.ByteBuffer;

public class GPUReadyTexture {
	private final ByteBuffer buffer;
	private final int width;
	private final int height;

	public GPUReadyTexture(final ByteBuffer buffer, final int width, final int height) {
		this.buffer = buffer;
		this.width = width;
		this.height = height;
	}

	public ByteBuffer getBuffer() {
		return buffer;
	}

	public int getWidth() {
		return width;
	}

	public int getHeight() {
		return height;
	}

}
