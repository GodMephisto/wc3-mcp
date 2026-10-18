---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/parsers/terrain/TilePathing.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.terrain;

public class TilePathing {
	boolean unwalkable = false;
	boolean unflyable = false;
	boolean unbuildable = false;

	public byte mask() {
		byte mask = 0;
		mask |= this.unwalkable ? 0b00000010 : 0;
		mask |= this.unflyable ? 0b00000100 : 0;
		mask |= this.unbuildable ? 0b00001000 : 0;
		return mask;
	}
}