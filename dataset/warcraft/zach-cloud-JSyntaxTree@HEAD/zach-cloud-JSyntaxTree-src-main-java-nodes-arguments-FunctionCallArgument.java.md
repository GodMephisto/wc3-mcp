---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/arguments/FunctionCallArgument.java"
ref: HEAD
---

package nodes.arguments;

import nodes.functions.Function;
import nodes.functions.FunctionCall;
import tree.TreeContext;

import java.util.ArrayList;
import java.util.List;
import java.util.Scanner;

public class FunctionCallArgument extends ArgumentType {

    /**
     * A function call is an atomic call to a function that
     * may or may not return a value.
     * i.e. myFunction(5)
     */
    private FunctionCall functionCall;
    private TreeContext context;

    public FunctionCallArgument(String content, TreeContext context) {
        this.functionCall = new FunctionCall(new Scanner(content), context);
        this.context = context;
    }

    public FunctionCallArgument(FunctionCall functionCall, TreeContext context) {
        this.functionCall = functionCall;
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
        this.functionCall.renameFunction(oldFunctionName, newFunctionName);
    }

    /**
     * Renames the variable and all uses of this variable.
     *
     * @param oldVariableName Existing variable name
     * @param newVariableName Desired variable name
     */
    @Override
    public void renameVariable(String oldVariableName, String newVariableName) {
        functionCall.renameVariable(oldVariableName, newVariableName);
    }

    @Override
    public boolean calls(String functionName) {
        return functionCall.calls(functionName) ||
                usesAsFunction(functionName);
    }

    @Override
    public ArgumentType inline(String functionName, String newText) {
        return new FunctionCallArgument(functionCall.inline(functionName, newText), context);
    }

    @Override
    public boolean usesAsFunction(String functionName) {
        return functionCall.usesAsFunction(functionName);
    }

    @Override
    public List<Argument> getArguments() {
        List<Argument> arguments = new ArrayList<>();

        arguments.addAll(functionCall.getArguments());

        return arguments;
    }

    public String toString() {
        return functionCall.toString();
    }
}
