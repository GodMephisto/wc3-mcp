---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/parsers/fdf/datamodel/fields/StringPairFrameDefinitionField.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.fdf.datamodel.fields;

public class StringPairFrameDefinitionField implements FrameDefinitionField {
	private final String first;
	private final String second;

	public StringPairFrameDefinitionField(final String first, final String second) {
		this.first = first;
		this.second = second;
	}

	public String getFirst() {
		return this.first;
	}

	public String getSecond() {
		return this.second;
	}

	@Override
	public <TYPE> TYPE visit(final FrameDefinitionFieldVisitor<TYPE> visitor) {
		return visitor.accept(this);
	}
}
