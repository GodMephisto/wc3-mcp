---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/value/CodeJassType.java"
ref: HEAD
---

package com.etheller.interpreter.ast.value;

public class CodeJassType extends PrimitiveJassType {

	public CodeJassType(final String name) {
		super(name, new CodeJassValue(null));
	}

	@Override
	public boolean isNullable() {
		return true;
	}
}
