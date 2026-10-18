---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/scope/variableevent/CLimitOp.java"
ref: HEAD
---

package com.etheller.interpreter.ast.scope.variableevent;

import com.etheller.interpreter.ast.util.CHandle;

public enum CLimitOp implements CHandle {
	LESS_THAN,
	LESS_THAN_OR_EQUAL,
	EQUAL,
	GREATER_THAN_OR_EQUAL,
	GREATER_THAN,
	NOT_EQUAL;

	public static CLimitOp[] VALUES = values();

	@Override
	public int getHandleId() {
		return ordinal();
	}
}
