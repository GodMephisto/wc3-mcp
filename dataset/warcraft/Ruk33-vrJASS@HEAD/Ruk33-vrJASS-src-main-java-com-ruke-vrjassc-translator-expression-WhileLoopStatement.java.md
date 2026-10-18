---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/translator/expression/WhileLoopStatement.java"
ref: HEAD
---

package com.ruke.vrjassc.translator.expression;

public class WhileLoopStatement extends LoopStatement {
	
	public WhileLoopStatement(Expression condition) {
		Expression notCondition = new NotExpression(condition);
		Statement exit = new ExitWhenStatement(notCondition);
		
		this.add(exit);
	}

}
