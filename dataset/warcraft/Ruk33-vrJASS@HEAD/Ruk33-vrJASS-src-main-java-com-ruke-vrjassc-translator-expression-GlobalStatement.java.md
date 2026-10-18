---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/translator/expression/GlobalStatement.java"
ref: HEAD
---

package com.ruke.vrjassc.translator.expression;

public class GlobalStatement extends StatementList {

	@Override
	public String translate() {
		return "globals\n" + super.translate() + "endglobals";
	}
	
}
