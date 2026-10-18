---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/parsers/fdf/datamodel/fields/FloatFrameDefinitionField.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.fdf.datamodel.fields;

public class FloatFrameDefinitionField implements FrameDefinitionField {
	private final float value;

	public FloatFrameDefinitionField(final float value) {
		this.value = value;
	}

	public float getValue() {
		return this.value;
	}

	@Override
	public <TYPE> TYPE visit(final FrameDefinitionFieldVisitor<TYPE> visitor) {
		return visitor.accept(this);
	}

}
