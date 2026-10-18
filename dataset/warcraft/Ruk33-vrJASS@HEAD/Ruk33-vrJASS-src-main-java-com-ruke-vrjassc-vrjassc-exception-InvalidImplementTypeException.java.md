---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/exception/InvalidImplementTypeException.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.exception;

import com.ruke.vrjassc.vrjassc.symbol.Symbol;
import org.antlr.v4.runtime.Token;

public class InvalidImplementTypeException extends CompileException {

	private Symbol symbol;

	public InvalidImplementTypeException(Token token, Symbol symbol) {
		super(token);
		this.symbol = symbol;
	}

	@Override
	public String getErrorMessage() {
		// TODO Auto-generated method stub
		return "Element <" + this.symbol.getName() + "> is not a valid implementable interface";
	}

}
