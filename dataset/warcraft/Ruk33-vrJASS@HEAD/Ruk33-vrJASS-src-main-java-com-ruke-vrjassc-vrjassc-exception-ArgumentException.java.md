---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS"
source_path: "src/main/java/com/ruke/vrjassc/vrjassc/exception/ArgumentException.java"
ref: HEAD
---

package com.ruke.vrjassc.vrjassc.exception;

import com.ruke.vrjassc.vrjassc.symbol.Type;
import org.antlr.v4.runtime.Token;

public class ArgumentException extends CompileException {

    private Type expected;
    private Type given;

    public ArgumentException(Token token, Type expected, Type given) {
        super(token);

        this.expected = expected;
        this.given = given;
    }

    @Override
    public String getErrorMessage() {
        // Thanks Aniki for the error message suggestion
        return String.format(
            "Mismatched type, expected <%s> but found <%s>",
            this.expected.getName(),
            this.given.getName()
        );
    }
}
