---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/value/ArrayJassType.java"
ref: HEAD
---

package com.etheller.interpreter.ast.value;

public class ArrayJassType implements JassType {
	private final JassType primitiveType;
	private final String name;

	public ArrayJassType(final JassType primitiveType) {
		this.primitiveType = primitiveType;
		this.name = primitiveType.getName() + " array";
	}

	@Override
	public boolean isAssignableFrom(final JassType value) {
		return value == this;
	}

	public JassType getPrimitiveType() {
		return this.primitiveType;
	}

	@Override
	public String getName() {
		return this.name;
	}

	@Override
	public <TYPE> TYPE visit(final JassTypeVisitor<TYPE> visitor) {
		return visitor.accept(this);
	}

	@Override
	public boolean isNullable() {
		return false;
	}

	@Override
	public JassValue getNullValue() {
		return null;
	}
}
