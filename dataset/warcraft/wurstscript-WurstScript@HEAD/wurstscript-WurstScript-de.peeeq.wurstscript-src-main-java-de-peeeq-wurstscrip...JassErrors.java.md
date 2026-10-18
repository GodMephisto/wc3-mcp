---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/frotty/jassValidator/JassErrors.java"
ref: HEAD
---

package de.peeeq.wurstscript.frotty.jassValidator;

import com.google.common.collect.Lists;

import java.util.List;

public class JassErrors {

    static final private List<String> errors = Lists.newArrayList();

    public static void addError(String error, int line) {
        errors.add("Line : " + line + " - " + error);
    }


    public static List<String> getErrors() {
        return Lists.newArrayList(errors);
    }

}
