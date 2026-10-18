---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/symbol/InitTrigSymbol.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.symbol;

public class InitTrigSymbol extends FunctionSymbol {

	public InitTrigSymbol(String name, Scope scope) {
		super(name, scope, null);
	}
	
	public boolean mustBeDeleted() {
		return this.getChilds().isEmpty();
	}

}
