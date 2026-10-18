---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/scope/TriggerExecutionScope.java"
ref: HEAD
---

package com.etheller.interpreter.ast.scope;

import com.etheller.interpreter.ast.scope.trigger.Trigger;

public class TriggerExecutionScope {
	public static final TriggerExecutionScope EMPTY = new TriggerExecutionScope(null);

	private final Trigger triggeringTrigger;

	public TriggerExecutionScope(final Trigger triggeringTrigger) {
		this.triggeringTrigger = triggeringTrigger;
	}

	public Trigger getTriggeringTrigger() {
		return this.triggeringTrigger;
	}

}
