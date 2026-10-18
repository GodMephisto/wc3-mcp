---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/exception/InvalidStringConcatenationException.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.exception;

import org.antlr.v4.runtime.Token;

public class InvalidStringConcatenationException extends CompileException {

	public InvalidStringConcatenationException(Token token) {
		super(token);
	}

	@Override
	public String getErrorMessage() {
		return "Invalid string concatenation";
	}

}
