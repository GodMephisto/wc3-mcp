---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/exception/InvalidStatementException.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.exception;

import org.antlr.v4.runtime.Token;

public class InvalidStatementException extends CompileException {

	private String message;
	
	public InvalidStatementException(String message, Token token) {
		super(token);
		this.message = message;
	}

	@Override
	public String getErrorMessage() {
		return this.message;
	}

}
