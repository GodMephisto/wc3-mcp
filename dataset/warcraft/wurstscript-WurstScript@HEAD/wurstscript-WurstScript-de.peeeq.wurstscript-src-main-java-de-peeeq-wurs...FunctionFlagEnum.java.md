---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/translation/imtranslation/FunctionFlagEnum.java"
ref: HEAD
---

package de.peeeq.wurstscript.translation.imtranslation;

public enum FunctionFlagEnum implements FunctionFlag {
    IS_BJ,
    IS_NATIVE,
    IS_TEST,
    IS_COMPILETIME_NATIVE,
    IS_EXTERN,
    IS_VARARG,
    PRESERVE_NAME,
    /** A Jass function which returns an integer as a real (legacy targets only); inlined, the integer would land in an operand. */
    RETURNS_INT_AS_REAL

}
