---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/attributes/prettyPrint/MaxOneSpacer.java"
ref: HEAD
---

package de.peeeq.wurstscript.attributes.prettyPrint;

public class MaxOneSpacer implements Spacer {

    public void addSpace(StringBuilder sb) {
        String lastChar = sb.substring(sb.length() - 1);
        if (!lastChar.equals(" ") && !lastChar.equals("(") && !lastChar.equals(".")) {
            sb.append(" ");
        }
    }

}
