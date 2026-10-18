---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/symbol/LocalVariableSymbol.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.symbol;

import org.antlr.v4.runtime.Token;

/**
 * @deprecated
 * @author Ruke
 *
 */
public class LocalVariableSymbol extends Symbol {

	public LocalVariableSymbol(String name, Scope scope, Token token) {
		super(name, scope, token);
		this.setModifier(Modifier.PRIVATE, true);
		this.setModifier(Modifier.LOCAL, true);
	}

}
