---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/exception/InterfaceCastException.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.exception;

import com.ruke.vrjassc.vrjassc.symbol.Symbol;
import org.antlr.v4.runtime.Token;

public class InterfaceCastException extends CompileException {

    private Symbol _interface;

    public InterfaceCastException(Symbol _interface, Token token) {
        super(token);
        this._interface = _interface;
    }

    public String getErrorMessage() {
        return "Cannot cast to " + this._interface.getName() + " since it is an interface";
    }

}
