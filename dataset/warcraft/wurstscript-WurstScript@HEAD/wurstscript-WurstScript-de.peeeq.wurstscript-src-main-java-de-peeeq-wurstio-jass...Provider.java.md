---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstio/jassinterpreter/providers/Provider.java"
ref: HEAD
---

package de.peeeq.wurstio.jassinterpreter.providers;

import com.google.common.base.Preconditions;
import de.peeeq.wurstscript.intermediatelang.interpreter.AbstractInterpreter;

public abstract class Provider {
    protected AbstractInterpreter interpreter;

    public Provider(AbstractInterpreter interpreter) {
        Preconditions.checkNotNull(interpreter);
        this.interpreter = interpreter;
    }
}
