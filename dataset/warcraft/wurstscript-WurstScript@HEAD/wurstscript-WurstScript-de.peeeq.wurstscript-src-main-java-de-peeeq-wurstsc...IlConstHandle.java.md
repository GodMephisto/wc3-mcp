---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/intermediatelang/IlConstHandle.java"
ref: HEAD
---

package de.peeeq.wurstscript.intermediatelang;

public class IlConstHandle implements ILconst {

    private final String name;
    private final Object obj;


    public IlConstHandle(String name, Object obj) {
        this.name = name;
        this.obj = obj;
    }

    @Override
    public String print() {
        return name;
    }

    @Override
    public boolean isEqualTo(ILconst other) {
        return other == this;
    }

    @Override
    public String toString() {
        return name;
    }

    public Object getObj() {
        return obj;
    }

}
