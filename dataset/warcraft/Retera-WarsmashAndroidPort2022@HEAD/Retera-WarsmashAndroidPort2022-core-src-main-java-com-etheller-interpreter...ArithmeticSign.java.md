---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/expression/ArithmeticSign.java"
ref: HEAD
---

package com.etheller.interpreter.ast.expression;

import com.etheller.interpreter.ast.value.BooleanJassValue;
import com.etheller.interpreter.ast.value.CodeJassValue;
import com.etheller.interpreter.ast.value.HandleJassValue;
import com.etheller.interpreter.ast.value.IntegerJassValue;
import com.etheller.interpreter.ast.value.JassValue;
import com.etheller.interpreter.ast.value.RealJassValue;

public interface ArithmeticSign {
	JassValue apply(BooleanJassValue left, BooleanJassValue right);

	JassValue apply(RealJassValue left, RealJassValue right);

	JassValue apply(RealJassValue left, IntegerJassValue right);

	JassValue apply(IntegerJassValue left, RealJassValue right);

	JassValue apply(IntegerJassValue left, IntegerJassValue right);

	JassValue apply(String left, String right);

	JassValue apply(HandleJassValue left, HandleJassValue right);

	JassValue apply(CodeJassValue left, CodeJassValue right);
}
