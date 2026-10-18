---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/expression/ReferenceJassExpression.java"
ref: HEAD
---

package com.etheller.interpreter.ast.expression;

import com.etheller.interpreter.ast.Assignable;
import com.etheller.interpreter.ast.scope.GlobalScope;
import com.etheller.interpreter.ast.scope.LocalScope;
import com.etheller.interpreter.ast.scope.TriggerExecutionScope;
import com.etheller.interpreter.ast.value.JassValue;

public class ReferenceJassExpression implements JassExpression {
	private final String identifier;

	public ReferenceJassExpression(final String identifier) {
		this.identifier = identifier;
	}

	@Override
	public JassValue evaluate(final GlobalScope globalScope, final LocalScope localScope,
			final TriggerExecutionScope triggerScope) {
		final Assignable local = localScope.getAssignableLocal(this.identifier);
		if (local == null) {
			return globalScope.getGlobal(this.identifier);
		}
		return local.getValue();
	}

}
