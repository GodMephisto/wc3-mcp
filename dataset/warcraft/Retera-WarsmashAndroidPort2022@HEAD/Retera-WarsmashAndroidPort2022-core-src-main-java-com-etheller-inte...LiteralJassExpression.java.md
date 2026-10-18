---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/expression/LiteralJassExpression.java"
ref: HEAD
---

package com.etheller.interpreter.ast.expression;

import com.etheller.interpreter.ast.scope.GlobalScope;
import com.etheller.interpreter.ast.scope.LocalScope;
import com.etheller.interpreter.ast.scope.TriggerExecutionScope;
import com.etheller.interpreter.ast.value.JassValue;

public class LiteralJassExpression implements JassExpression {
	private final JassValue value;

	public LiteralJassExpression(final JassValue value) {
		this.value = value;
	}

	@Override
	public JassValue evaluate(final GlobalScope globalScope, final LocalScope localScope,
			final TriggerExecutionScope triggerScope) {
		return this.value;
	}

}
