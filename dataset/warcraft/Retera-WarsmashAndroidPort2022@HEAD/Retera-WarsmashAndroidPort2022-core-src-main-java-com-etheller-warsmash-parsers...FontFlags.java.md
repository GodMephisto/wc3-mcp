---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/parsers/fdf/datamodel/FontFlags.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.fdf.datamodel;

import java.util.EnumSet;

public enum FontFlags {
	FIXEDSIZE,
	PASSWORDFIELD;

	public static EnumSet<FontFlags> parseFontFlags(final String cornerFlags) {
		final EnumSet<FontFlags> set = EnumSet.noneOf(FontFlags.class);
		for (final String flag : cornerFlags.split("\\|")) {
			if (!"".equals(flag)) {
				set.add(FontFlags.valueOf(flag));
			}
		}
		return set;
	}
}
