---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/arguments/NotArgument.java"
ref: HEAD
---

package nodes.arguments;

import tree.TreeContext;

import java.util.ArrayList;
import java.util.List;
import java.util.Scanner;

public class NotArgument extends ArgumentType {

    /**
     * Represents the argument of not (notPart)
     */
    private Argument notPart;
    private TreeContext context;

    public NotArgument(String content, TreeContext context) {
        this.notPart = new Argument(new Scanner(content), context);
        this.context = context;
    }

    public NotArgument(Argument notPart, TreeContext context) {
        this.notPart = notPart;
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
        notPart.renameFunction(oldFunctionName, newFunctionName);
    }

    /**
     * Renames the variable and all uses of this variable.
     *
     * @param oldVariableName Existing variable name
     * @param newVariableName Desired variable name
     */
    @Override
    public void renameVariable(String oldVariableName, String newVariableName) {
        notPart.renameVariable(oldVariableName, newVariableName);
    }

    @Override
    public ArgumentType inline(String functionName, String newText) {
        return new NotArgument(notPart.inline(functionName, newText), context);
    }

    @Override
    public boolean calls(String functionName) {
        return notPart.calls(functionName);
    }

    @Override
    public boolean usesAsFunction(String functionName) {
        return notPart.usesAsFunction(functionName);
    }

    @Override
    public List<Argument> getArguments() {
        List<Argument> arguments = new ArrayList<>();

        arguments.addAll(notPart.getArguments());

        return arguments;
    }

    public String toString() {
        return "not (" + notPart.toString() + ")";
    }

    public Argument getNotPart() {
        return notPart;
    }
}
