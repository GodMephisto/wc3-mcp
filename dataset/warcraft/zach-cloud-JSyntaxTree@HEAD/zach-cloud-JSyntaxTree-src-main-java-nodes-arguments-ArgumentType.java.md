---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/arguments/ArgumentType.java"
ref: HEAD
---

package nodes.arguments;

import interfaces.IFunctionRenameable;
import interfaces.IVariableRenameable;
import nodes.AbstractReadable;

import java.util.List;

public abstract class ArgumentType extends AbstractReadable implements IVariableRenameable, IFunctionRenameable {

    public abstract String toString();

    public abstract ArgumentType inline(String functionName, String newText);

    public abstract boolean calls(String functionName);

    public abstract boolean usesAsFunction(String functionName);

    public abstract List<Argument> getArguments();
}
