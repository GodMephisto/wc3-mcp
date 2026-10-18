---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/scope/trigger/RemovableTriggerEvent.java"
ref: HEAD
---

package com.etheller.interpreter.ast.scope.trigger;

public interface RemovableTriggerEvent {
	void remove();

	RemovableTriggerEvent DO_NOTHING = new RemovableTriggerEvent() {
		@Override
		public void remove() {
		}
	};
}
