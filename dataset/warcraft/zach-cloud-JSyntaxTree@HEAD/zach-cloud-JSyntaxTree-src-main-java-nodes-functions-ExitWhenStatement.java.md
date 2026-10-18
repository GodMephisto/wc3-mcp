---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/functions/ExitWhenStatement.java"
ref: HEAD
---

package nodes.functions;

import interfaces.IFunctionRenameable;
import interfaces.IVariableRenameable;
import nodes.AbstractStatement;
import exception.ParsingException;
import nodes.arguments.Argument;
import tree.TreeContext;

import java.util.ArrayList;
import java.util.List;
import java.util.Scanner;

/**
 * Represents an exitwhen + condition statement
 */
public final class ExitWhenStatement extends AbstractStatement implements IFunctionRenameable, IVariableRenameable {

    /**
     * Declares when this loop should be exited
     */
    private Argument exitwhenCondition;

    /**
     * Sets up this node with a scanner to receive words.
     *
     * @param inputScanner Scanner containing JASS code
     */
    public ExitWhenStatement(Scanner inputScanner, TreeContext context) {
        super(inputScanner, context);
    }

    /**
     * No-args constructor used for creating from an existing
     */
    public ExitWhenStatement(Argument exitwhenCondition, TreeContext context) {
        super(context);
        this.exitwhenCondition = exitwhenCondition;
    }

    /**
     * Converts this node back to its original form.
     * Indentation is not added.
     *
     * @return Original form of this node (code or string)
     */
    @Override
    public final String toString() {
        return "exitwhen " + exitwhenCondition.toString();
    }

    /**
     * Converts this node back to its original form.
     *
     * @param indentationLevel Current indentation level
     * @return Original form of this node (code or string) with indentation
     */
    @Override
    public String toFormattedString(int indentationLevel) {
        StringBuilder built = new StringBuilder();
        addTabs(built, indentationLevel);
        built.append(this.toString());
        return built.toString();
    }

    /**
     * Renames the variable and all uses of this variable.
     *
     * @param oldVariableName   Existing variable name
     * @param newVariableName   Desired variable name
     */
    @Override
    public final void renameVariable(String oldVariableName, String newVariableName) {
        exitwhenCondition.renameVariable(oldVariableName, newVariableName);
    }

    /**
     * Converts the given function name into an inline function.
     * Replaces usages of function
     *
     * @param functionName Function name to replace
     * @param newText      Function text to replace with
     * @return Replaced statements
     */
    @Override
    public AbstractStatement inline(String functionName, String newText) {
        return new ExitWhenStatement((Argument)exitwhenCondition.inline(functionName, newText), context);
    }

    public boolean usesAsFunction(String functionName) {
        return exitwhenCondition.usesAsFunction(functionName);
    }

    /**
     * Renames a function and uses to a new name
     *
     * @param oldFunctionName   Existing function name
     * @param newFunctionName   Desired function name
     */
    @Override
    public final void renameFunction(String oldFunctionName, String newFunctionName) {
        this.exitwhenCondition.renameFunction(oldFunctionName, newFunctionName);
    }

    /**
     * Parse the JASS code contained in the Scanner into a model object
     */
    @Override
    protected final void readNode() {
        String line = readLine();
        if(!line.startsWith("exitwhen ")) {
            throw new ParsingException("Not an exitwhen line: " + line);
        }
        line = line.substring("exitwhen ".length());
        exitwhenCondition = new Argument(new Scanner(line), context);
    }

    public final Argument getExitwhenCondition() {
        return exitwhenCondition;
    }

    public final List<Argument> getArguments() {
        List<Argument> arguments = new ArrayList<>();
        arguments.addAll(exitwhenCondition.getArguments());
        return arguments;
    }

    @Override
    public int hashCode() {
        return this.toString().hashCode();
    }

    @Override
    public boolean equals(Object obj) {
        if (obj == null) { return false; }
        if (obj == this) { return true; }
        if (obj.getClass() != getClass()) {
            return false;
        }
        ExitWhenStatement other = (ExitWhenStatement) obj;
        return this.toString().equals(other.toString());
    }
}
