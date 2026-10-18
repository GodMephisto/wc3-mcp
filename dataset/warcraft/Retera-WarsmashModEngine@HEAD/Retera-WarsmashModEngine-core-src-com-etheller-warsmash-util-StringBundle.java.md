---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/util/StringBundle.java"
ref: HEAD
---

package com.etheller.warsmash.util;

public interface StringBundle {
	String getString(String string);

	String getStringCaseSensitive(final String key);

	StringBundle EMPTY = new StringBundle() {
		@Override
		public String getStringCaseSensitive(final String key) {
			return key;
		}

		@Override
		public String getString(final String string) {
			return string;
		}
	};
}
