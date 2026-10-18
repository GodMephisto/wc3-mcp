---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/intermediatelang/ILconstAbstract.java"
ref: HEAD
---

package de.peeeq.wurstscript.intermediatelang;

import org.eclipse.jdt.annotation.Nullable;

public abstract class ILconstAbstract implements ILconst {

    @Override
    public abstract String print();


    @Override
    public String toString() {
        return print();
    }


    @Override
    public boolean equals(@Nullable Object other) {
        if (other instanceof ILconst iLconst) {
            return isEqualTo(iLconst);
        }
        return false;
    }


    @Override
    public int hashCode() {
        throw new Error("hashcode not implemented for " + getClass() + "/" + this);
    }

}
