---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstscript/translation/imtranslation/GetAForB.java"
ref: HEAD
---

package de.peeeq.wurstscript.translation.imtranslation;

import com.google.common.collect.Maps;

import java.util.Map;

public abstract class GetAForB<B, A> {

    private final Map<B, A> thing = Maps.newLinkedHashMap();

    public abstract A initFor(B a);

    public A getFor(B a) {
        A existing = thing.get(a);
        if (existing != null) {
            return existing;
        }
        A created = initFor(a);
        thing.put(a, created);
        return created;
    }

}
