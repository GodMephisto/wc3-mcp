---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/scope/trigger/TriggerBooleanExpression.java"
ref: HEAD
---

package com.etheller.interpreter.ast.scope.trigger;

import com.etheller.interpreter.ast.scope.GlobalScope;
import com.etheller.interpreter.ast.scope.TriggerExecutionScope;

public interface TriggerBooleanExpression {
	boolean evaluate(GlobalScope globalScope, TriggerExecutionScope triggerScope);
}
