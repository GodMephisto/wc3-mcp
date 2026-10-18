---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/value/visitor/ObjectJassValueVisitor.java"
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

public class ObjectJassValueVisitor<T> implements JassValueVisitor<T> {
	private static final ObjectJassValueVisitor<?> INSTANCE = new ObjectJassValueVisitor();

	public static <V> ObjectJassValueVisitor<V> getInstance() {
		return (ObjectJassValueVisitor<V>) INSTANCE;
	}

	@Override
	public T accept(final IntegerJassValue value) {
		return null;
	}

	@Override
	public T accept(final RealJassValue value) {
		return null;
	}

	@Override
	public T accept(final BooleanJassValue value) {
		return null;
	}

	@Override
	public T accept(final StringJassValue value) {
		return null;
	}

	@Override
	public T accept(final CodeJassValue value) {
		return null;
	}

	@Override
	public T accept(final ArrayJassValue value) {
		return null;
	}

	@Override
	public T accept(final HandleJassValue value) {
		return (T) value.getJavaValue();
	}

}
