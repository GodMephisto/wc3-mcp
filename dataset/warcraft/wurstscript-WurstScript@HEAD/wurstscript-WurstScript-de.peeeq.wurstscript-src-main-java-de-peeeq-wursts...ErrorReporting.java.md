---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/ErrorReporting.java"
ref: HEAD
---

package de.peeeq.wurstscript;


public class ErrorReporting {

    public static ErrorReporting instance = new ErrorReporting();


    public void handleSevere(Throwable t, String sourcecode) {
        System.err.println(t);
        WLogger.severe(t);
    }

    public boolean sendErrorReport(Throwable t, String sourcecode) {
        return false;
    }

}
