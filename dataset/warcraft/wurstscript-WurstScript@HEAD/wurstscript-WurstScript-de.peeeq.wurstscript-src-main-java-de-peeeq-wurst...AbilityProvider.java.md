---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstio/jassinterpreter/providers/AbilityProvider.java"
ref: HEAD
---

package de.peeeq.wurstio.jassinterpreter.providers;

import de.peeeq.wurstscript.intermediatelang.ILconstInt;
import de.peeeq.wurstscript.intermediatelang.ILconstString;
import de.peeeq.wurstscript.intermediatelang.interpreter.AbstractInterpreter;

public class AbilityProvider extends Provider {
    public AbilityProvider(AbstractInterpreter interpreter) {
        super(interpreter);
    }

    public ILconstString BlzGetAbilityIcon(ILconstInt abilCode) {
        return ILconstString.fromText("");
    }

    public ILconstString BlzGetAbilityExtendedTooltip(ILconstInt abilCode, ILconstInt level) {
        return ILconstString.fromText("");
    }
}
