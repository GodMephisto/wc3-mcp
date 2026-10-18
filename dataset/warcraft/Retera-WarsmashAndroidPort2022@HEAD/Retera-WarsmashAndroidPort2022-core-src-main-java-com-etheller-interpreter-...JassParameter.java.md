---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/function/JassParameter.java"
ref: HEAD
---

package com.etheller.interpreter.ast.function;

import com.etheller.interpreter.ast.value.JassType;
import com.etheller.interpreter.ast.value.JassValue;
import com.etheller.interpreter.ast.value.visitor.JassTypeGettingValueVisitor;

public class JassParameter {
	private final JassType type;
	private final String identifier;

	public JassParameter(final JassType type, final String identifier) {
		this.type = type;
		this.identifier = identifier;
	}

	public String getIdentifier() {
		return this.identifier;
	}

	public JassType getType() {
		return this.type;
	}

	public boolean matchesType(final JassValue value) {
		if (value == null) {
			return (this.type == JassType.NOTHING) || this.type.isNullable();
		}
		final JassType valueType = value.visit(JassTypeGettingValueVisitor.getInstance());
		return this.type.isAssignableFrom(valueType);
	}
}
