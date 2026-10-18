---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/jassinterpreter/TestSuccessException.java"
ref: HEAD
---

package de.peeeq.wurstscript.jassinterpreter;


public class TestSuccessException extends Error {
    static public final TestSuccessException instance = new TestSuccessException();

    private TestSuccessException() {
    }

}
