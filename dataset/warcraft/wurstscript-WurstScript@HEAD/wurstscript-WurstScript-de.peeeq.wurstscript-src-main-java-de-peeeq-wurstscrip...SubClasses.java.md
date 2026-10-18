---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/attributes/SubClasses.java"
ref: HEAD
---

package de.peeeq.wurstscript.attributes;

import de.peeeq.wurstscript.ast.ClassDef;
import de.peeeq.wurstscript.ast.ConstructorDef;
import de.peeeq.wurstscript.ast.ConstructorDefs;
import de.peeeq.wurstscript.types.WurstTypeClass;
import org.eclipse.jdt.annotation.Nullable;

public class SubClasses {


    public static @Nullable ConstructorDef getSuperConstructor(ConstructorDef constr) {
        ClassDef c = constr.attrNearestClassDef();
        if (c == null) {
            return null;
        }
        WurstTypeClass ct = c.attrTypC();
        WurstTypeClass superClass = ct.extendedClass();
        if (superClass == null) {
            return null;
        }
        // call super constructor
        ClassDef superClassDef = superClass.getDef();
        ConstructorDefs constructors = superClassDef.getConstructors();
        return OverloadingResolver.resolveSuperCall(constructors, constr);
    }

}
