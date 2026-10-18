---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/parsers/terrain/Corner.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.terrain;

public class Corner {
	boolean mapEdge;

	int groundTexture;
	float height;
	float waterHeight;
	boolean ramp;
	boolean blight;
	boolean water;
	boolean boundary;
	boolean cliff;
	boolean romp;
	int groundVariation;
	int cliffVariation;
	int cliffTexture;
	int layerHeight;

	public float finalGroundHeight() {
		return 0;
	}

	public float finalWaterHeight() {
		return 0;
	}
}
