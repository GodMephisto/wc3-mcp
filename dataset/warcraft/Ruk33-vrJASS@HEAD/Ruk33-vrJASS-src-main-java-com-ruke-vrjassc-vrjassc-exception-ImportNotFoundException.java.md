---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/exception/ImportNotFoundException.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.exception;

import org.antlr.v4.runtime.Token;

public class ImportNotFoundException extends CompileException {

    private String path;

    public ImportNotFoundException(Token token, String path) {
        super(token);
        this.path = path;
    }

    @Override
    public String getErrorMessage() {
        return "Import " + this.path + " was not found.";
    }
}
