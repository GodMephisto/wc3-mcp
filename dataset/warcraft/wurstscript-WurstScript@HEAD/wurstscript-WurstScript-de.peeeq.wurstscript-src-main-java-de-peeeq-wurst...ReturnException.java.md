---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/jassinterpreter/ReturnException.java"
ref: HEAD
---

package de.peeeq.wurstscript.jassinterpreter;

import de.peeeq.wurstscript.intermediatelang.ILconst;
import org.eclipse.jdt.annotation.Nullable;

public class ReturnException extends Error {

    private @Nullable ILconst val;

    public ReturnException(@Nullable ILconst value) {
        super(null, null, true, false);
        this.val = value;
    }

    public @Nullable ILconst getVal() {
        return val;
    }

    public ReturnException setVal(@Nullable ILconst val) {
        this.val = val;
        return this;
    }
}
