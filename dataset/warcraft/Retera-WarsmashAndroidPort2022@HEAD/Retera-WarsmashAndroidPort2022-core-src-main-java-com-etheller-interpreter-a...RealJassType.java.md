---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/value/RealJassType.java"
ref: HEAD
---

package com.etheller.interpreter.ast.value;

public class RealJassType extends PrimitiveJassType {

	public RealJassType(final String name, final JassValue nullValue) {
		super(name, nullValue);
	}

	@Override
	public boolean isAssignableFrom(final JassType value) {
		if (value == JassType.INTEGER) {
			return true;
		}
		return super.isAssignableFrom(value);
	}
}
