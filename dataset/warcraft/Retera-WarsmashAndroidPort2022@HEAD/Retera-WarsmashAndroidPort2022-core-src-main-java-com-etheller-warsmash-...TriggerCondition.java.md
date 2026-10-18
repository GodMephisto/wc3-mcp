---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/parsers/jass/triggers/TriggerCondition.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.jass.triggers;

import com.etheller.interpreter.ast.scope.trigger.Trigger;
import com.etheller.interpreter.ast.scope.trigger.TriggerBooleanExpression;

public class TriggerCondition {
	private final TriggerBooleanExpression boolexpr;
	private final Trigger trigger;
	private final int conditionIndex;

	public TriggerCondition(final TriggerBooleanExpression boolexpr, final Trigger trigger, final int index) {
		this.boolexpr = boolexpr;
		this.trigger = trigger;
		this.conditionIndex = index;
	}

	public TriggerBooleanExpression getBoolexpr() {
		return this.boolexpr;
	}

	public Trigger getTrigger() {
		return this.trigger;
	}

	public int getConditionIndex() {
		return this.conditionIndex;
	}
}
