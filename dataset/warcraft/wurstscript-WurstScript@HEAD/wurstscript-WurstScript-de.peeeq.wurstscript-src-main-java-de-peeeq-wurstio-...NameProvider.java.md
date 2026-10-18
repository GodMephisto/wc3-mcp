---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstio/jassinterpreter/providers/NameProvider.java"
ref: HEAD
---

package de.peeeq.wurstio.jassinterpreter.providers;

import java.util.Random;

public class NameProvider {
    private static final Random random = new Random();

    public static String getRandomName(String string) {
        return string + random.nextInt(1000);
    }
}
