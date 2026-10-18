---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/translator/expression/ElseStatement.java"
ref: HEAD
---

package com.ruke.vrjassc.translator.expression;

public class ElseStatement extends StatementBody {

	@Override
	public String translate() {
		return "else\n" + super.translate();
	}
	
}
