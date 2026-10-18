---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/symbol/GlobalVariableSymbol.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.symbol;

import org.antlr.v4.runtime.Token;

/**
 * @deprecated
 * @author Ruke
 *
 */
public class GlobalVariableSymbol extends Symbol {

	public GlobalVariableSymbol(String name, Scope scope, Token token) {
		super(name, scope, token);
	}

}
