---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/gui/WurstGuiLogger.java"
ref: HEAD
---

package de.peeeq.wurstscript.gui;


public class WurstGuiLogger extends WurstGui {

    @Override
    public void sendProgress(String whatsRunningNow) {
        // ignore
    }

    @Override
    public void sendFinished() {
        // ignore
    }

    @Override
    public void showInfoMessage(String message) {
        System.out.println(message);
    }

}
