---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/functions/SetStatement.java"
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
 * Represents a Set Statement, like "set x = 2"
 */
public final class SetStatement extends AbstractStatement implements IFunctionRenameable, IVariableRenameable {

    private Argument variable;
    private Argument variableArgument;

    /**
     * Sets up this node with a scanner to receive words.
     *
     * @param inputScanner Scanner containing JASS code
     */
    public SetStatement(Scanner inputScanner, TreeContext context) {
        super(inputScanner, context);
    }

    private SetStatement(Argument variable, Argument variableArgument, TreeContext context) {
        super(context);
        this.variable = variable;
        this.variableArgument = variableArgument;
    }

    /**
     * Converts this node back to its original form.
     * Indentation is not added.
     *
     * @return Original form of this node (code or string)
     */
    @Override
    public final String toString() {
        return "set " + variable + " = " + variableArgument.toString();
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
     * Parse the JASS code contained in the Scanner into a model object
     */
    @Override
    protected final void readNode() {
        String line = readLine();
        if(!line.startsWith("set ") || !line.contains("=")) {
            throw new ParsingException("Not a set statement: " + line);
        }
        line = line.substring(4);
        String[] parts = line.split("=");
        variable = new Argument(new Scanner(parts[0].trim()), context);
        StringBuilder value = new StringBuilder();
        //Handle case of this SetStatement containing multiple equals (like set x = (a <= b))
        for(int i = 1; i < parts.length; i++) {
            value.append(parts[i]).append("=");
        }
        removeFinalCharacter(value);
        this.variableArgument = new Argument(new Scanner(value.toString()), context);
    }

    /**
     * Renames the variable and all uses of this variable.
     *
     * @param oldVariableName   Existing variable name
     * @param newVariableName   Desired variable name
     */
    @Override
    public final void renameVariable(String oldVariableName, String newVariableName) {
        variable.renameVariable(oldVariableName, newVariableName);
        variableArgument.renameVariable(oldVariableName, newVariableName);
    }

    /**
     * Renames a function and uses to a new name
     *
     * @param oldFunctionName   Existing function name
     * @param newFunctionName   Desired function name
     */
    @Override
    public final void renameFunction(String oldFunctionName, String newFunctionName) {
        variable.renameFunction(oldFunctionName, newFunctionName);
        variableArgument.renameFunction(oldFunctionName, newFunctionName);
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
        return new SetStatement(variable, (variableArgument.inline(functionName, newText)), context);
    }

    public boolean usesAsFunction(String functionName) {
        return variableArgument.usesAsFunction(functionName);
    }

    public final String getVariable() {
        return variable.toString();
    }

    public final String getValue() {
        return variableArgument.toString();
    }

    public final List<Argument> getArguments() {
        List<Argument> arguments = new ArrayList<>();
        arguments.addAll(variableArgument.getArguments());
        arguments.add(variable);
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
        SetStatement other = (SetStatement) obj;
        return this.toString().equals(other.toString());
    }
}
