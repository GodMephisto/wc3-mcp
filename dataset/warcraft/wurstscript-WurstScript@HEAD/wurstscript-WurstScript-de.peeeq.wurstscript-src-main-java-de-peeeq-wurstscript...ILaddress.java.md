---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/intermediatelang/ILaddress.java"
ref: HEAD
---

package de.peeeq.wurstscript.intermediatelang;

/**
 *
 */
public interface ILaddress {
    void set(ILconst value);
    ILconst get();
}
