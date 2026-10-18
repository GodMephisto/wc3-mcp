---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/symbol/DeprecatedPropertySymbol.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.symbol;

public class DeprecatedPropertySymbol extends Symbol {

	public DeprecatedPropertySymbol(VariableSymbol variable) {
		super(variable.getName(), variable.getParentScope(), variable.getToken());
	}

}
