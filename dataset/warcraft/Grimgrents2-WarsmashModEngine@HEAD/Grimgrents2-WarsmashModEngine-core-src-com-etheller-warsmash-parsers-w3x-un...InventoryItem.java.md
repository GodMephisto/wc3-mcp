---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/parsers/w3x/unitsdoo/InventoryItem.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.w3x.unitsdoo;

import java.io.IOException;

import com.etheller.warsmash.util.ParseUtils;
import com.etheller.warsmash.util.War3ID;
import com.google.common.io.LittleEndianDataInputStream;
import com.google.common.io.LittleEndianDataOutputStream;

/**
 * An inventory item.
 */
public class InventoryItem {
	private int slot;
	private War3ID id;

	public void load(final LittleEndianDataInputStream stream) throws IOException {
		this.slot = stream.readInt();
		this.id = ParseUtils.readWar3ID(stream);
	}

	public void save(final LittleEndianDataOutputStream stream) throws IOException {
		stream.writeInt(this.slot);
		ParseUtils.writeWar3ID(stream, this.id);
	}
}
