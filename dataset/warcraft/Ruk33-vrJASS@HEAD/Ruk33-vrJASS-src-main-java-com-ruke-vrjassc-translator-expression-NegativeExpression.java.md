---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/translator/expression/NegativeExpression.java"
ref: HEAD
---

package com.ruke.vrjassc.translator.expression;

public class NegativeExpression extends Expression {

	protected Expression expression;
	
	public NegativeExpression(Expression expression) {
		this.expression = expression;
		this.expression.setParent(this);
	}
	
	@Override
	public String translate() {
		return "-" + this.expression.translate();
	}

}
