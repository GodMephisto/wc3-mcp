---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/expression/NegateJassExpression.java"
ref: HEAD
---

package com.etheller.interpreter.ast.expression;

import com.etheller.interpreter.ast.scope.GlobalScope;
import com.etheller.interpreter.ast.scope.LocalScope;
import com.etheller.interpreter.ast.scope.TriggerExecutionScope;
import com.etheller.interpreter.ast.value.JassValue;
import com.etheller.interpreter.ast.value.visitor.NegateJassValueVisitor;

public class NegateJassExpression implements JassExpression {
	private final JassExpression expression;

	public NegateJassExpression(final JassExpression expression) {
		this.expression = expression;
	}

	@Override
	public JassValue evaluate(final GlobalScope globalScope, final LocalScope localScope,
			final TriggerExecutionScope triggerScope) {
		return this.expression.evaluate(globalScope, localScope, triggerScope)
				.visit(NegateJassValueVisitor.getInstance());
	}
}
