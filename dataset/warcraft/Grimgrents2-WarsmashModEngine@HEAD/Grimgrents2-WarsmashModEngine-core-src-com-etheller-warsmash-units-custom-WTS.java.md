---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/units/custom/WTS.java"
ref: HEAD
---

package com.etheller.warsmash.units.custom;

public interface WTS {
	String get(int key);

	WTS DO_NOTHING = new WTS() {
		@Override
		public String get(final int key) {
			return "TRIGSTR_" + key;
		}
	};
}
