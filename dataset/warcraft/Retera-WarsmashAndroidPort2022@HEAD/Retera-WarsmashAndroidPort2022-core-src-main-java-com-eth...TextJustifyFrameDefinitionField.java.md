---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/parsers/fdf/datamodel/fields/TextJustifyFrameDefinitionField.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.fdf.datamodel.fields;

import com.etheller.warsmash.parsers.fdf.datamodel.TextJustify;

public class TextJustifyFrameDefinitionField implements FrameDefinitionField {
	private final TextJustify value;

	public TextJustifyFrameDefinitionField(final TextJustify value) {
		this.value = value;
	}

	public TextJustify getValue() {
		return this.value;
	}

	@Override
	public <TYPE> TYPE visit(final FrameDefinitionFieldVisitor<TYPE> visitor) {
		return visitor.accept(this);
	}

}
