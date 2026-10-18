---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/parsers/fdf/datamodel/MenuItem.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.fdf.datamodel;

public class MenuItem {
	private final String text;
	private final int numericValue;

	public MenuItem(final String text, final int numericValue) {
		this.text = text;
		this.numericValue = numericValue;
	}

	public String getText() {
		return this.text;
	}

	public int getNumericValue() {
		return this.numericValue;
	}
}
