---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/value/visitor/ArrayTypeVisitor.java"
ref: HEAD
---

package com.etheller.interpreter.ast.value.visitor;

import com.etheller.interpreter.ast.value.ArrayJassType;
import com.etheller.interpreter.ast.value.HandleJassType;
import com.etheller.interpreter.ast.value.JassTypeVisitor;
import com.etheller.interpreter.ast.value.PrimitiveJassType;

public class ArrayTypeVisitor implements JassTypeVisitor<ArrayJassType> {
	private static final ArrayTypeVisitor INSTANCE = new ArrayTypeVisitor();

	public static ArrayTypeVisitor getInstance() {
		return INSTANCE;
	}

	@Override
	public ArrayJassType accept(final PrimitiveJassType primitiveType) {
		return null;
	}

	@Override
	public ArrayJassType accept(final ArrayJassType arrayType) {
		return arrayType;
	}

	@Override
	public ArrayJassType accept(final HandleJassType type) {
		return null;
	}

}
