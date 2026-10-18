---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstio/jassinterpreter/providers/RegionProvider.java"
ref: HEAD
---

package de.peeeq.wurstio.jassinterpreter.providers;

import de.peeeq.wurstio.jassinterpreter.mocks.RegionMock;
import de.peeeq.wurstscript.intermediatelang.IlConstHandle;
import de.peeeq.wurstscript.intermediatelang.interpreter.AbstractInterpreter;

public class RegionProvider extends Provider {


    public RegionProvider(AbstractInterpreter interpreter) {
        super(interpreter);
    }

    public IlConstHandle CreateRegion() {
        return new IlConstHandle(NameProvider.getRandomName("region"), new RegionMock());
    }

    public void RemoveRegion(IlConstHandle region) {
    }

    public void RegionAddRect(IlConstHandle region, IlConstHandle rect) {
        // TODO
    }

}
