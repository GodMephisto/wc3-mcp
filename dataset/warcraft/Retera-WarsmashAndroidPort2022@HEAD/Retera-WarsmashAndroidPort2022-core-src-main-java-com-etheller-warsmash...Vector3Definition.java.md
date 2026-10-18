---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/parsers/fdf/datamodel/Vector3Definition.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.fdf.datamodel;

public class Vector3Definition {
	private final float x, y, z;

	public Vector3Definition(final float x, final float y, final float z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public float getX() {
		return this.x;
	}

	public float getY() {
		return this.y;
	}

	public float getZ() {
		return this.z;
	}
}
