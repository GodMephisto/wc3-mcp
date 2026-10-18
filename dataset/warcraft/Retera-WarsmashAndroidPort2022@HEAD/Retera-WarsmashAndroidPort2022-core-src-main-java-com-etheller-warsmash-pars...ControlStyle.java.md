---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/parsers/fdf/datamodel/ControlStyle.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.fdf.datamodel;

import java.util.EnumSet;

public enum ControlStyle {
	AUTOTRACK,
	HIGHLIGHTONFOCUS,
	HIGHLIGHTONMOUSEOVER;

	public static EnumSet<ControlStyle> parseControlStyle(final String controlStyles) {
		final EnumSet<ControlStyle> set = EnumSet.noneOf(ControlStyle.class);
		for (final String flag : controlStyles.split("\\|")) {
			if (!"".equals(flag)) {
				set.add(ControlStyle.valueOf(flag));
			}
		}
		return set;
	}
}
