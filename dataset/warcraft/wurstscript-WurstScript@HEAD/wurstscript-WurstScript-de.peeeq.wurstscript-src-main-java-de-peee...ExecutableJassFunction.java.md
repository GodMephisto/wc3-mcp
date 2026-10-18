---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstio/jassinterpreter/ExecutableJassFunction.java"
ref: HEAD
---

package de.peeeq.wurstio.jassinterpreter;

import de.peeeq.wurstscript.WLogger;
import de.peeeq.wurstscript.intermediatelang.ILconst;
import de.peeeq.wurstscript.jassAst.JassFunction;

public interface ExecutableJassFunction {

    ILconst execute(JassInterpreter jassInterpreter, ILconst... arguments);


}

class UserDefinedJassFunction implements ExecutableJassFunction {

    private final JassFunction jassFunction;

    public UserDefinedJassFunction(JassFunction f) {
        this.jassFunction = f;
    }

    @Override
    public ILconst execute(JassInterpreter jassInterpreter, ILconst[] arguments) {
        return jassInterpreter.executeJassFunction(jassFunction, arguments);
    }

}




class UnknownJassFunction implements ExecutableJassFunction {

    private final String name;

    public UnknownJassFunction(String name) {
        this.name = name;
    }

    @Override
    public ILconst execute(JassInterpreter jassInterpreter, ILconst[] arguments) {
        WLogger.info("Function " + name + " could not be found.");
        throw new InterpreterException("Function " + name + " could not be found.");
    }

}

