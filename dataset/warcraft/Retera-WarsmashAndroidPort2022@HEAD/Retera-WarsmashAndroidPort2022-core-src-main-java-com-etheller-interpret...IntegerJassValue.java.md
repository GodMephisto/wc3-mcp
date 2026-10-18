---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/value/IntegerJassValue.java"
ref: HEAD
---

package com.etheller.interpreter.ast.value;

public class IntegerJassValue implements JassValue {
	public static final JassValue ZERO = new IntegerJassValue(0);
	private final int value;

	public IntegerJassValue(final int value) {
		this.value = value;
	}

	public int getValue() {
		return this.value;
	}

	@Override
	public <TYPE> TYPE visit(final JassValueVisitor<TYPE> visitor) {
		return visitor.accept(this);
	}

	@Override
	public String toString() {
		return Integer.toString(this.value);
	}
}
