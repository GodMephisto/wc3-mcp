---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/attributes/PathDescription.java"
ref: HEAD
---

package de.peeeq.wurstscript.attributes;

import de.peeeq.wurstscript.ast.Element;
import de.peeeq.wurstscript.utils.Utils;

public class PathDescription {
    public static String get(Element e) {
        String result = Utils.printElement(e);
        if (e.getParent() != null) {
            result = e.getParent().attrPathDescription() + "/" + result;
        }
        return result;
    }
}
