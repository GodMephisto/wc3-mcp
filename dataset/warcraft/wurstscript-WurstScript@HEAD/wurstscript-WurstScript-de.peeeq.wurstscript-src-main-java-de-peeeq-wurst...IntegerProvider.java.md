---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstio/jassinterpreter/providers/IntegerProvider.java"
ref: HEAD
---

package de.peeeq.wurstio.jassinterpreter.providers;

import de.peeeq.wurstscript.intermediatelang.ILconstInt;
import de.peeeq.wurstscript.intermediatelang.interpreter.AbstractInterpreter;

public class IntegerProvider extends Provider {

    public IntegerProvider(AbstractInterpreter interpreter) {
        super(interpreter);
    }

    public ILconstInt BlzBitOr(ILconstInt x, ILconstInt y) {
        return ILconstInt.create(x.getVal() | y.getVal());
    }

    public ILconstInt BlzBitAnd(ILconstInt x, ILconstInt y) {
        return ILconstInt.create(x.getVal() & y.getVal());
    }

    public ILconstInt BlzBitXor(ILconstInt x, ILconstInt y) {
        return ILconstInt.create(x.getVal() ^ y.getVal());
    }

}
