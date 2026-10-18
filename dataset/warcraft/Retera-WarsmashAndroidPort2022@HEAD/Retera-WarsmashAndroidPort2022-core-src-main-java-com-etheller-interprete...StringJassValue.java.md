---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/value/StringJassValue.java"
ref: HEAD
---

package com.etheller.interpreter.ast.value;

public class StringJassValue implements JassValue {
	public static final JassValue EMPTY_STRING = StringJassValue.of("");
	private final String value;

	public static StringJassValue of(final String value) {
		// later this could do that dumb thing jass does with making sure we dont create
		// duplicate instances, maybe
		return new StringJassValue(value);
	}

	public StringJassValue(final String value) {
		this.value = value;
	}

	public String getValue() {
		return this.value;
	}

	@Override
	public <TYPE> TYPE visit(final JassValueVisitor<TYPE> visitor) {
		return visitor.accept(this);
	}
}
