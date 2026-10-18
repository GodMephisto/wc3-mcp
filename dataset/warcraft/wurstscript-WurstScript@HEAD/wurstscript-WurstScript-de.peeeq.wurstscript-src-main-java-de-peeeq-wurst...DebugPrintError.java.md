---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstio/jassinterpreter/DebugPrintError.java"
ref: HEAD
---

package de.peeeq.wurstio.jassinterpreter;

import de.peeeq.wurstscript.intermediatelang.interpreter.ILStackFrame;
import de.peeeq.wurstscript.jassIm.ImStmt;
import de.peeeq.wurstscript.parser.WPos;
import org.eclipse.jdt.annotation.Nullable;

public class DebugPrintError extends Error {
    private @Nullable ILStackFrame stackFrame;
    private @Nullable ImStmt stmt;

    public DebugPrintError(String val) {
        super(val);
    }


    private static final long serialVersionUID = 629417346498474872L;

    @Override
    public String toString() {
        final ILStackFrame sf = stackFrame;
        ImStmt s = stmt;
        String err = "Wurst Error: " + getMessage();
        if (s != null) {
            WPos src = s.attrTrace().attrSource();
            err += " at line " + src.getLine();
        }
        if (sf != null) {
            err += sf.getMessage();
        }
        return err;
    }


}
