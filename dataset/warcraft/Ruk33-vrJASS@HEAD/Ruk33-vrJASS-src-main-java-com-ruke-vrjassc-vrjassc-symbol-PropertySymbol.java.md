---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/symbol/PropertySymbol.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.symbol;

import org.antlr.v4.runtime.Token;

/**
 * @deprecated
 * @author Ruke
 *
 */
public class PropertySymbol extends Symbol {

	public PropertySymbol(String name, Scope scope, Token token) {
		super(name, scope, token);
		this.setModifier(Modifier.MEMBER, true);
	}

}
