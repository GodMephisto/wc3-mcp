---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/value/StringJassType.java"
ref: HEAD
---

package com.etheller.interpreter.ast.value;

public class StringJassType extends PrimitiveJassType {

	public StringJassType(final String name) {
		super(name, new StringJassValue(null));
	}

	@Override
	public boolean isNullable() {
		return true;
	}
}
