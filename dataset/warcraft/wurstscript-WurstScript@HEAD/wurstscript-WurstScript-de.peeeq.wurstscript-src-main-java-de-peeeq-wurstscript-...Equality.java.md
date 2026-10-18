---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/translation/imtojass/Equality.java"
ref: HEAD
---

package de.peeeq.wurstscript.translation.imtojass;

import de.peeeq.wurstscript.jassIm.*;

public class Equality {

    public static boolean equalValue(ImBoolVal v, ImConst other) {
        if (other instanceof ImBoolVal ov) {
            return v.getValB() == ov.getValB();
        }
        return false;
    }

    public static boolean equalValue(ImFuncRef v, ImConst other) {
        if (other instanceof ImFuncRef ov) {
            return v.getFunc() == ov.getFunc();
        }
        return false;
    }

    public static boolean equalValue(ImIntVal v, ImConst other) {
        if (other instanceof ImIntVal ov) {
            return v.getValI() == ov.getValI();
        }
        return false;
    }

    public static boolean equalValue(ImNull v, ImConst other) {
        return other instanceof ImNull;
    }

    public static boolean equalValue(ImRealVal v, ImConst other) {
        if (other instanceof ImRealVal ov) {
            return v.getValR().equals(ov.getValR());
        }
        return false;
    }

    public static boolean equalValue(ImStringVal v, ImConst other) {
        if (other instanceof ImStringVal ov) {
            return v.getValS().equals(ov.getValS());
        }
        return false;
    }

}
