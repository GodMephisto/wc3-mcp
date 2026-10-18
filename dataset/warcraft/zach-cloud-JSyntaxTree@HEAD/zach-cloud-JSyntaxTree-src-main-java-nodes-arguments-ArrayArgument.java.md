---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/arguments/ArrayArgument.java"
ref: HEAD
---

package nodes.arguments;

import tree.TreeContext;

import java.util.ArrayList;
import java.util.List;
import java.util.Scanner;

public class ArrayArgument extends ArgumentType {

    /**
     * An array name is the outer part of the call to an array
     * Such as ybx[Abc], ybx would be the arrayName
     */
    private Argument arrayName;
    /**
     * An array call is the inner section to an array.
     * Such as ybx[Abc], Abc would be the arrayCall
     */
    private Argument arrayCall;
    private TreeContext context;

    public ArrayArgument(String arrayName, String arrayCall, TreeContext context) {
        this.arrayName = new Argument(new Scanner(arrayName), context);
        this.arrayCall = new Argument(new Scanner(arrayCall), context);
        this.context = context;
    }

    /**
     * Renames a function and uses to a new name
     *
     * @param oldFunctionName Existing function name
     * @param newFunctionName Desired function name
     */
    @Override
    public void renameFunction(String oldFunctionName, String newFunctionName) {
        arrayCall.renameFunction(oldFunctionName, newFunctionName);
        arrayName.renameFunction(oldFunctionName, newFunctionName);
    }

    /**
     * Renames the variable and all uses of this variable.
     *
     * @param oldVariableName Existing variable name
     * @param newVariableName Desired variable name
     */
    @Override
    public void renameVariable(String oldVariableName, String newVariableName) {
        arrayCall.renameVariable(oldVariableName, newVariableName);
        arrayName.renameVariable(oldVariableName, newVariableName);
    }

    @Override
    public ArgumentType inline(String functionName, String newText) {
        // Cannot be inlined.
        return this;
    }

    @Override
    public boolean calls(String functionName) {
        return arrayCall.calls(functionName) ||
                arrayName.calls(functionName) ||
                usesAsFunction(functionName);
    }

    @Override
    public boolean usesAsFunction(String functionName) {
        return arrayCall.usesAsFunction(functionName) ||
                arrayName.usesAsFunction(functionName);
    }

    @Override
    public List<Argument> getArguments() {
        List<Argument> arguments = new ArrayList<>();

        arguments.addAll(arrayCall.getArguments());
        arguments.addAll(arrayName.getArguments());

        return arguments;
    }

    public String toString() {
        return arrayName.toString() + "[" + arrayCall + "]";
    }
}
