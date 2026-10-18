---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/parsers/fdf/frames/TextButtonFrame.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.fdf.frames;

public class TextButtonFrame extends GlueTextButtonFrame {
	private float buttonPushedTextOffsetX;
	private float buttonPushedTextOffsetY;

	public TextButtonFrame(final String name, final UIFrame parent) {
		super(name, parent);
	}

	public void setButtonPushedTextOffsetX(final float buttonPushedTextOffsetX) {
		this.buttonPushedTextOffsetX = buttonPushedTextOffsetX;
	}

	public void setButtonPushedTextOffsetY(final float buttonPushedTextOffsetY) {
		this.buttonPushedTextOffsetY = buttonPushedTextOffsetY;
	}
}
