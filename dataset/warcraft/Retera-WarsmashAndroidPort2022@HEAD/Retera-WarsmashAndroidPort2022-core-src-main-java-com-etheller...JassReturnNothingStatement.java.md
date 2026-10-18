---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/statement/JassReturnNothingStatement.java"
ref: HEAD
---

package com.etheller.interpreter.ast.statement;

import com.etheller.interpreter.ast.scope.GlobalScope;
import com.etheller.interpreter.ast.scope.LocalScope;
import com.etheller.interpreter.ast.scope.TriggerExecutionScope;
import com.etheller.interpreter.ast.value.JassValue;
import com.etheller.interpreter.ast.value.StringJassValue;

public class JassReturnNothingStatement implements JassStatement {
	public static final StringJassValue RETURN_NOTHING_NOTICE = new StringJassValue("nothing");

	@Override
	public JassValue execute(final GlobalScope globalScope, final LocalScope localScope,
			final TriggerExecutionScope triggerScope) {
		return RETURN_NOTHING_NOTICE;
	}

}
