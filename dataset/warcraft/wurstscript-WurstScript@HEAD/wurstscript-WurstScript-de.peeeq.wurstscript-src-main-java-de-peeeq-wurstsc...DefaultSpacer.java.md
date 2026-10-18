---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/attributes/prettyPrint/DefaultSpacer.java"
ref: HEAD
---

package de.peeeq.wurstscript.attributes.prettyPrint;

public class DefaultSpacer implements Spacer {

    @Override
    public void addSpace(StringBuilder sb) {
        sb.append(" ");
    }
}
