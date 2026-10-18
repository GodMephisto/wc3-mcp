---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/value/CodeJassValue.java"
ref: HEAD
---

package com.etheller.interpreter.ast.value;

import com.etheller.interpreter.ast.function.JassFunction;

public class CodeJassValue implements JassValue {
	private final JassFunction value;

	public CodeJassValue(final JassFunction value) {
		this.value = value;
	}

	public JassFunction getValue() {
		return value;
	}

	@Override
	public <TYPE> TYPE visit(final JassValueVisitor<TYPE> visitor) {
		return visitor.accept(this);
	}

}
