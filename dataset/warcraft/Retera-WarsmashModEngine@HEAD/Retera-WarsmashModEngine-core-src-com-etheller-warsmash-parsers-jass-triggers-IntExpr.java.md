---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/parsers/jass/triggers/IntExpr.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.jass.triggers;

import com.etheller.interpreter.ast.scope.GlobalScope;
import com.etheller.interpreter.ast.scope.TriggerExecutionScope;
import com.etheller.interpreter.ast.scope.trigger.TriggerIntegerExpression;
import com.etheller.interpreter.ast.value.CodeJassValue;
import com.etheller.interpreter.ast.value.IntegerJassValue;
import com.etheller.interpreter.ast.value.JassValue;
import com.etheller.interpreter.ast.value.visitor.IntegerJassValueVisitor;

public class IntExpr implements TriggerIntegerExpression {
	private final CodeJassValue takesNothingReturnsIntegerFunction;

	public IntExpr(final CodeJassValue takesNothingReturnsIntegerFunction) {
		this.takesNothingReturnsIntegerFunction = takesNothingReturnsIntegerFunction;
	}

	@Override
	public int evaluate(final GlobalScope globalScope, final TriggerExecutionScope triggerScope) {
		final JassValue jassReturnValue = this.takesNothingReturnsIntegerFunction
				.callAndExecuteCapturingReturnValue(globalScope, triggerScope, "IntExpr", IntegerJassValue.ZERO);
		final Integer integerReturnValue = jassReturnValue.visit(IntegerJassValueVisitor.getInstance());
		return integerReturnValue.intValue();
	}

}
