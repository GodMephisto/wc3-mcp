---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/parsers/fdf/datamodel/FontDefinition.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.fdf.datamodel;

public class FontDefinition {
	private final String fontName;
	private final float fontSize;
	private final String extra;

	public FontDefinition(final String fontName, final float fontSize, final String extra) {
		this.fontName = fontName;
		this.fontSize = fontSize;
		this.extra = extra;
	}

	public String getFontName() {
		return this.fontName;
	}

	public float getFontSize() {
		return this.fontSize;
	}

	public String getExtra() {
		return this.extra;
	}
}
