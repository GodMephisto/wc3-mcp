---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstio/jassinterpreter/mocks/QuestItemMock.java"
ref: HEAD
---

package de.peeeq.wurstio.jassinterpreter.mocks;

import de.peeeq.wurstscript.intermediatelang.ILconstBool;

public class QuestItemMock {
    public ILconstBool completed;

    public QuestItemMock() {
        this.completed = ILconstBool.FALSE;
    }
}
