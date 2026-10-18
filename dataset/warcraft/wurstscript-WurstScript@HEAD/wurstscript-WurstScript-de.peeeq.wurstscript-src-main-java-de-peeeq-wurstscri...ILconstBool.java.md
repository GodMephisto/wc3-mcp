---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/intermediatelang/ILconstBool.java"
ref: HEAD
---

package de.peeeq.wurstscript.intermediatelang;

import de.peeeq.wurstscript.types.WurstType;
import de.peeeq.wurstscript.types.WurstTypeBool;

import java.util.Objects;

public class ILconstBool extends ILconstAbstract {

    private final boolean val;

    public final static ILconstBool FALSE = new ILconstBool(false);
    public final static ILconstBool TRUE = new ILconstBool(true);

    public static ILconstBool instance(boolean value) {
        return value ? TRUE : FALSE;
    }

    private ILconstBool(boolean b) {
        val = b;
    }

    public boolean getVal() {
        return val;
    }

    @Override
    public String print() {
        return val ? "true" : "false";
    }


    public WurstType getType() {
        return WurstTypeBool.instance();
    }

    public ILconst negate() {
        return val ? FALSE : TRUE;
    }

    @Override
    public boolean isEqualTo(ILconst other) {
        return other == this;
    }

    @Override
    public int hashCode() {
        return Objects.hash(val);
    }
}
