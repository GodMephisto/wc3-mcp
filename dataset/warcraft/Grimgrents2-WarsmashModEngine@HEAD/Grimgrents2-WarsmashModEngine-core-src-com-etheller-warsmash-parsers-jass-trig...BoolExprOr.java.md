---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/parsers/jass/triggers/BoolExprOr.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.jass.triggers;

import com.etheller.interpreter.ast.scope.GlobalScope;
import com.etheller.interpreter.ast.scope.TriggerExecutionScope;
import com.etheller.interpreter.ast.scope.trigger.TriggerBooleanExpression;

public class BoolExprOr implements TriggerBooleanExpression {
	private final TriggerBooleanExpression operandA;
	private final TriggerBooleanExpression operandB;

	public BoolExprOr(final TriggerBooleanExpression operandA, final TriggerBooleanExpression operandB) {
		this.operandA = operandA;
		this.operandB = operandB;
	}

	@Override
	public boolean evaluate(final GlobalScope globalScope, final TriggerExecutionScope triggerScope) {
		return this.operandA.evaluate(globalScope, triggerScope) || this.operandB.evaluate(globalScope, triggerScope);
	}

}
