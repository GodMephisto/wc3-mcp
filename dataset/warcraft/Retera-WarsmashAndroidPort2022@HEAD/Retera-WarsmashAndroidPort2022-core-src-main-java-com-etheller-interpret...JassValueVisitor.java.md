---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/value/JassValueVisitor.java"
ref: HEAD
---

package com.etheller.interpreter.ast.value;

public interface JassValueVisitor<TYPE> {
	TYPE accept(IntegerJassValue value);

	TYPE accept(RealJassValue value);

	TYPE accept(BooleanJassValue value);

	TYPE accept(StringJassValue value);

	TYPE accept(CodeJassValue value);

	TYPE accept(ArrayJassValue value);

	TYPE accept(HandleJassValue value);
}
