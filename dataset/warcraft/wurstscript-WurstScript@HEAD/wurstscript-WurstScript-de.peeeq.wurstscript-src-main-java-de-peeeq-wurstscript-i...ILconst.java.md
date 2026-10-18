---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/intermediatelang/ILconst.java"
ref: HEAD
---

package de.peeeq.wurstscript.intermediatelang;

public interface ILconst {

    String print();

    boolean isEqualTo(ILconst other);

    @Override
    String toString();

}
