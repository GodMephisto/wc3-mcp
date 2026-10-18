---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/translator/expression/ExitWhenStatement.java"
ref: HEAD
---

package com.ruke.vrjassc.translator.expression;

public class ExitWhenStatement extends Statement {

	protected Expression condition;
	
	public ExitWhenStatement(Expression condition) {
		this.condition = condition;
		this.condition.setParent(this);
	}
	
	@Override
	public String translate() {
		return "exitwhen " + this.condition.translate();
	}

}
