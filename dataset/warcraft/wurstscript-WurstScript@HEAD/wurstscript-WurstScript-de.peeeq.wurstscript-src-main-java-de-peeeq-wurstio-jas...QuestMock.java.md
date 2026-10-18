---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstio/jassinterpreter/mocks/QuestMock.java"
ref: HEAD
---

package de.peeeq.wurstio.jassinterpreter.mocks;

import de.peeeq.wurstscript.intermediatelang.ILconstBool;

public class QuestMock {
    public ILconstBool completed;
    public ILconstBool discovered;
    public ILconstBool enabled;
    public ILconstBool failed;
    public ILconstBool required;

    public QuestMock() {
        this.completed = ILconstBool.FALSE;
        this.discovered = ILconstBool.FALSE;
        this.enabled = ILconstBool.FALSE;
        this.failed = ILconstBool.FALSE;
        this.required = ILconstBool.FALSE;
    }
}
