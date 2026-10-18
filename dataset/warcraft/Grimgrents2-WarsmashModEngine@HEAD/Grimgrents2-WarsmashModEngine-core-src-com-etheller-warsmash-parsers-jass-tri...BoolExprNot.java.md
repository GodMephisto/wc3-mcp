---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/parsers/jass/triggers/BoolExprNot.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.jass.triggers;

import com.etheller.interpreter.ast.scope.GlobalScope;
import com.etheller.interpreter.ast.scope.TriggerExecutionScope;
import com.etheller.interpreter.ast.scope.trigger.TriggerBooleanExpression;

public class BoolExprNot implements TriggerBooleanExpression {
	private final TriggerBooleanExpression operand;

	public BoolExprNot(final TriggerBooleanExpression operand) {
		this.operand = operand;
	}

	@Override
	public boolean evaluate(final GlobalScope globalScope, final TriggerExecutionScope triggerScope) {
		return this.operand.evaluate(globalScope, triggerScope);
	}

}
