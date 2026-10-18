---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/translation/imoptimizer/OptimizerPass.java"
ref: HEAD
---

package de.peeeq.wurstscript.translation.imoptimizer;

import de.peeeq.wurstscript.translation.imtranslation.ImTranslator;

public interface OptimizerPass {
    int optimize(ImTranslator trans);

    String getName();
}
