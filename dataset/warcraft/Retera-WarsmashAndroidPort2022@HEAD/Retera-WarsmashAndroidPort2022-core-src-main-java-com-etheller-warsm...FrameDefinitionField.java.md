---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/parsers/fdf/datamodel/fields/FrameDefinitionField.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.fdf.datamodel.fields;

public interface FrameDefinitionField {
	<TYPE> TYPE visit(FrameDefinitionFieldVisitor<TYPE> visitor);
}
