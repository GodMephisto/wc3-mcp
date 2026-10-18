---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/intermediatelang/interpreter/NoSuchNativeException.java"
ref: HEAD
---

package de.peeeq.wurstscript.intermediatelang.interpreter;

public class NoSuchNativeException extends Exception {

    public NoSuchNativeException(String msg) {
        super(msg, null, true, false);
    }
}
