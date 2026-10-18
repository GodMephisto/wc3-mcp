---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/exception/IncorrectArgumentCountException.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.exception;

import com.ruke.vrjassc.vrjassc.symbol.Symbol;
import org.antlr.v4.runtime.Token;

public class IncorrectArgumentCountException extends CompileException {

	private Symbol function;

	public IncorrectArgumentCountException(Token token, Symbol function) {
		super(token);
		this.function = function;
	}

	@Override
	public String getErrorMessage() {
		return "Incorrect amount of arguments passed to function <" + function.getName() + ">";
	}

}
