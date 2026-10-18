---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/exception/InvalidTypeException.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.exception;

import com.ruke.vrjassc.vrjassc.symbol.Symbol;
import org.antlr.v4.runtime.Token;

public class InvalidTypeException extends CompileException {

	private Symbol requesting;
	private String typeName;

	public InvalidTypeException(Token token, Symbol requesting, String typeName) {
		super(token);
		this.requesting = requesting;
		this.typeName = typeName;
	}

	@Override
	public String getErrorMessage() {
		return String.format(
			"<%s> is either not a valid type or it could not be reached by <%s>",
			this.typeName,
			this.requesting.getName()
		);
	}

}
