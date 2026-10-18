---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/exception/SyntaxErrorException.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.exception;

public class SyntaxErrorException extends CompileException {

	protected String msg;
	
	public SyntaxErrorException(int line, int charPos, String msg) {
		super(null);
		
		this.line = line;
		this.charPos = charPos;
		this.msg = msg;
	}
	
	@Override
	public String getErrorMessage() {
		return "Syntax error " + this.msg;
	}

}
