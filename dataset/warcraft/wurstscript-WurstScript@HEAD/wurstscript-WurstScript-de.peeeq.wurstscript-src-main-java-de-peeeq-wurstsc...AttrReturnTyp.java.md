---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/attributes/AttrReturnTyp.java"
ref: HEAD
---

package de.peeeq.wurstscript.attributes;

import de.peeeq.wurstscript.ast.FunctionDefinition;
import de.peeeq.wurstscript.ast.TupleDef;
import de.peeeq.wurstscript.types.WurstType;

/**
 *
 */
public class AttrReturnTyp {
    public static WurstType calculate(FunctionDefinition f) {
        return f.getReturnTyp().attrTyp().dynamic();
    }

    public static WurstType calculate(TupleDef f) {
        return f.attrTyp();
    }
}
