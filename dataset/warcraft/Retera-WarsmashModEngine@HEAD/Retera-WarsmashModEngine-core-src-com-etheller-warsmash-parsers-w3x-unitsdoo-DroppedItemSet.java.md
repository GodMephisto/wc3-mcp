---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/parsers/w3x/unitsdoo/DroppedItemSet.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.w3x.unitsdoo;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import com.etheller.warsmash.util.ParseUtils;
import com.google.common.io.LittleEndianDataInputStream;
import com.google.common.io.LittleEndianDataOutputStream;

/**
 * A dropped item set.
 */
public class DroppedItemSet {
	private final List<DroppedItem> items = new ArrayList<>();

	public void load(final LittleEndianDataInputStream stream) throws IOException {
		for (long i = 0, l = ParseUtils.readUInt32(stream); i < l; i++) {
			final DroppedItem item = new DroppedItem();

			item.load(stream);

			this.items.add(item);
		}
	}

	public void save(final LittleEndianDataOutputStream stream) throws IOException {
		ParseUtils.writeUInt32(stream, this.items.size());

		for (final DroppedItem item : this.items) {
			item.save(stream);
		}
	}

	public int getByteLength() {
		return 4 + (this.items.size() * 8);
	}
}
