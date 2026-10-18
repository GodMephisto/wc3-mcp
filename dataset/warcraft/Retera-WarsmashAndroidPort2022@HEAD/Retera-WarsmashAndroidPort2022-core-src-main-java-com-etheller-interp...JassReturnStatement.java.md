---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/statement/JassReturnStatement.java"
ref: HEAD
---

package com.etheller.interpreter.ast.statement;

import com.etheller.interpreter.ast.expression.JassExpression;
import com.etheller.interpreter.ast.scope.GlobalScope;
import com.etheller.interpreter.ast.scope.LocalScope;
import com.etheller.interpreter.ast.scope.TriggerExecutionScope;
import com.etheller.interpreter.ast.value.JassValue;

public class JassReturnStatement implements JassStatement {
	private final JassExpression expression;

	public JassReturnStatement(final JassExpression expression) {
		this.expression = expression;
	}

	@Override
	public JassValue execute(final GlobalScope globalScope, final LocalScope localScope,
			final TriggerExecutionScope triggerScope) {
		final JassValue returnVal = this.expression.evaluate(globalScope, localScope, triggerScope);
		if (returnVal == null) {
			return JassReturnNothingStatement.RETURN_NOTHING_NOTICE;
		}
		return returnVal;
	}

}
