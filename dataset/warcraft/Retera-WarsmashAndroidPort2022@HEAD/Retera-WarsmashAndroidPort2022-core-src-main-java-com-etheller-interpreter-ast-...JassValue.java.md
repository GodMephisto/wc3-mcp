---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/value/JassValue.java"
ref: HEAD
---

package com.etheller.interpreter.ast.value;

public interface JassValue {
	<TYPE> TYPE visit(JassValueVisitor<TYPE> visitor);
}
