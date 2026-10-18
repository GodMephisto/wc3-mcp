---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/attributes/AttrConstructorDef.java"
ref: HEAD
---

package de.peeeq.wurstscript.attributes;

import de.peeeq.wurstscript.ast.ClassDef;
import de.peeeq.wurstscript.ast.ConstructorDef;
import de.peeeq.wurstscript.ast.ExprNewObject;
import de.peeeq.wurstscript.ast.TypeDef;
import org.eclipse.jdt.annotation.Nullable;

import java.util.List;


/**
 * find the constructor for a "new" call
 */
public class AttrConstructorDef {

    public static @Nullable ConstructorDef calculate(final ExprNewObject node) {

        TypeDef typeDef = node.attrTypeDef();


        if (typeDef instanceof ClassDef classDef) {


            List<ConstructorDef> constructors = classDef.getConstructors();

            ConstructorDef constr = OverloadingResolver.resolveExprNew(constructors, node);

            if (constr != null && constr.attrIsPrivate() && !node.isSubtreeOf(classDef)) {
                node.addError("This constructor for class " + classDef.getName() + " is not visible here.");
            }

            return constr;
        } else {
            node.addError("Can only create instances of classes.");
            return null;
        }

    }


}
