---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/attributes/names/Visibility.java"
ref: HEAD
---

package de.peeeq.wurstscript.attributes.names;

public enum Visibility {
    PRIVATE_OTHER, PUBLIC, LOCAL, PRIVATE_HERE, PROTECTED_HERE, PROTECTED_OTHER;

    public boolean isInherited() {
        return this == PUBLIC || this == PROTECTED_HERE || this == PROTECTED_OTHER;
    }

    public boolean isPublic() {
        return this == PUBLIC;
    }

}
