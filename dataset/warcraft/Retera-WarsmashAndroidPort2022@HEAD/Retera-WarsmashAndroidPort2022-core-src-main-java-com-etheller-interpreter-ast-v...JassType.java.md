---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/value/JassType.java"
ref: HEAD
---

package com.etheller.interpreter.ast.value;

import com.etheller.interpreter.ast.statement.JassReturnNothingStatement;

public interface JassType {
	<TYPE> TYPE visit(JassTypeVisitor<TYPE> visitor);

	String getName(); // used for error messages

	boolean isAssignableFrom(JassType value);

	boolean isNullable();

	JassValue getNullValue();

	public static final PrimitiveJassType INTEGER = new PrimitiveJassType("integer", IntegerJassValue.ZERO);
	public static final PrimitiveJassType STRING = new StringJassType("string");
	public static final PrimitiveJassType CODE = new CodeJassType("code");
	public static final PrimitiveJassType REAL = new RealJassType("real", RealJassValue.ZERO);
	public static final PrimitiveJassType BOOLEAN = new PrimitiveJassType("boolean", BooleanJassValue.FALSE);
	public static final PrimitiveJassType NOTHING = new PrimitiveJassType("nothing",
			JassReturnNothingStatement.RETURN_NOTHING_NOTICE);
}
