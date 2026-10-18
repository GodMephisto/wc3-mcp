---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/parsers/fdf/datamodel/BackdropCornerFlags.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.fdf.datamodel;

import java.util.EnumSet;

public enum BackdropCornerFlags {
	UL,
	UR,
	BL,
	BR,
	T,
	L,
	B,
	R;

	public static EnumSet<BackdropCornerFlags> parseCornerFlags(final String cornerFlags) {
		final EnumSet<BackdropCornerFlags> set = EnumSet.noneOf(BackdropCornerFlags.class);
		for (final String flag : cornerFlags.split("\\|")) {
			if (!"".equals(flag)) {
				set.add(BackdropCornerFlags.valueOf(flag));
			}
		}
		return set;
	}
}
