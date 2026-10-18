---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstio/languageserver/ModelManagerException.java"
ref: HEAD
---

package de.peeeq.wurstio.languageserver;

public class ModelManagerException extends RuntimeException {
    private static final long serialVersionUID = 1L;

    public ModelManagerException(Exception e) {
        super(e);
    }

}
