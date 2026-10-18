---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/symbol/VariableSymbol.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.symbol;

import org.antlr.v4.runtime.Token;

public class VariableSymbol extends Symbol {

	public VariableSymbol(String name, Scope scope, Token token) {
		super(name, scope, token);
	}

}
