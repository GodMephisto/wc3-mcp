---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstio/AbortCompilationException.java"
ref: HEAD
---

package de.peeeq.wurstio;

public class AbortCompilationException extends RuntimeException {
    private static final long serialVersionUID = 4984246286230018420L;

    public AbortCompilationException(String msg) {
        super(msg);
    }
}
