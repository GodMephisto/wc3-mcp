---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstio/jassinterpreter/Implements.java"
ref: HEAD
---

package de.peeeq.wurstio.jassinterpreter;

import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;

@Retention(RetentionPolicy.RUNTIME)
public @interface Implements {
    String[] funcNames();
}
