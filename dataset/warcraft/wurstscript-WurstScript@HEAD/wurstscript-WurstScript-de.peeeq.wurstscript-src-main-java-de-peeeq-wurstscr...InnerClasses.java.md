---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/attributes/InnerClasses.java"
ref: HEAD
---

package de.peeeq.wurstscript.attributes;

import de.peeeq.wurstscript.ast.ClassDef;

public class InnerClasses {

    public static boolean isInnerClass(ClassDef classDef) {
        return classDef.getParent().attrNearestClassOrModule() != null;
    }

}
