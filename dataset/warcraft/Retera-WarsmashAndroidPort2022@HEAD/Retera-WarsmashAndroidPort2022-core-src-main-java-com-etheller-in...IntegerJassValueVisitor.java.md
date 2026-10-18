---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/value/visitor/IntegerJassValueVisitor.java"
ref: HEAD
---

package com.etheller.interpreter.ast.value.visitor;

import com.etheller.interpreter.ast.value.ArrayJassValue;
import com.etheller.interpreter.ast.value.BooleanJassValue;
import com.etheller.interpreter.ast.value.CodeJassValue;
import com.etheller.interpreter.ast.value.HandleJassValue;
import com.etheller.interpreter.ast.value.IntegerJassValue;
import com.etheller.interpreter.ast.value.JassValueVisitor;
import com.etheller.interpreter.ast.value.RealJassValue;
import com.etheller.interpreter.ast.value.StringJassValue;

public class IntegerJassValueVisitor implements JassValueVisitor<Integer> {
	private static final IntegerJassValueVisitor INSTANCE = new IntegerJassValueVisitor();

	public static IntegerJassValueVisitor getInstance() {
		return INSTANCE;
	}

	@Override
	public Integer accept(final IntegerJassValue value) {
		return value.getValue();
	}

	@Override
	public Integer accept(final RealJassValue value) {
		return (int) value.getValue();
	}

	@Override
	public Integer accept(final BooleanJassValue value) {
		return 0;
	}

	@Override
	public Integer accept(final StringJassValue value) {
		return 0;
	}

	@Override
	public Integer accept(final CodeJassValue value) {
		return 0;
	}

	@Override
	public Integer accept(final ArrayJassValue value) {
		return 0;
	}

	@Override
	public Integer accept(final HandleJassValue value) {
		return 0;
	}

}
