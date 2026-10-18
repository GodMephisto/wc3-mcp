---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/interpreter/ast/visitors/ArgumentExpressionHandler.java"
ref: HEAD
---

package com.etheller.interpreter.ast.visitors;

public class ArgumentExpressionHandler {
	protected JassArgumentsVisitor argumentsVisitor;
	protected JassExpressionVisitor expressionVisitor;

	public void setJassArgumentsVisitor(final JassArgumentsVisitor jassArgumentsVisitor) {
		this.argumentsVisitor = jassArgumentsVisitor;
	}

	public void setJassExpressionVisitor(final JassExpressionVisitor jassExpressionVisitor) {
		this.expressionVisitor = jassExpressionVisitor;
	}

}
