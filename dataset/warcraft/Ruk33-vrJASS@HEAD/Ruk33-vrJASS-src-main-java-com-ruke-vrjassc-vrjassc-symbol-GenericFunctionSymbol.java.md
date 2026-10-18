---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/symbol/GenericFunctionSymbol.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.symbol;

import org.antlr.v4.runtime.Token;

public class GenericFunctionSymbol extends FunctionSymbol {

    public GenericFunctionSymbol(Symbol original, Symbol generic, Scope scope, Token token) {
        super(original.getName() + "_" + generic.getName(), scope, token);
        this.setGeneric(generic);
    }

}
