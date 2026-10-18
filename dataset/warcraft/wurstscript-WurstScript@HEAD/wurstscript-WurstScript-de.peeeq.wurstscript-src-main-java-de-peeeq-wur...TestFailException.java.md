---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/jassinterpreter/TestFailException.java"
ref: HEAD
---

package de.peeeq.wurstscript.jassinterpreter;


public class TestFailException extends Error {

    private final String test;

    public TestFailException(String name) {
        test = name;
    }

    public String getVal() {
        return test;
    }

    @Override
    public String toString() {
        return getMessage();
    }

    @Override
    public String getMessage() {
        return "Test failed: " + test;
    }
}
