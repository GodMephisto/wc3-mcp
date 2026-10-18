---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/parsers/jass/triggers/TriggerAction.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.jass.triggers;

import com.etheller.interpreter.ast.scope.trigger.Trigger;
import com.etheller.interpreter.ast.value.CodeJassValue;

public class TriggerAction {
	private final Trigger trigger;
	private final CodeJassValue actionFunc;
	private final int actionIndex;

	public TriggerAction(final Trigger trigger, final CodeJassValue actionFunc, final int actionIndex) {
		this.trigger = trigger;
		this.actionFunc = actionFunc;
		this.actionIndex = actionIndex;
	}

	public Trigger getTrigger() {
		return this.trigger;
	}

	public CodeJassValue getActionFunc() {
		return this.actionFunc;
	}

	public int getActionIndex() {
		return this.actionIndex;
	}
}
