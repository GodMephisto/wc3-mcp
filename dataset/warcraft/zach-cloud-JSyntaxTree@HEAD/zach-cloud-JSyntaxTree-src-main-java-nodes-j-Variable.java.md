---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/j/Variable.java"
ref: HEAD
---

package nodes.j;

import interfaces.IFunctionRenameable;
import interfaces.IVariableRenameable;
import nodes.AbstractNode;
import nodes.arguments.Argument;
import tree.TreeContext;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Scanner;

/**
 * Represents a JASS variable
 */
public final class Variable extends AbstractNode implements IFunctionRenameable, IVariableRenameable {

    private String type; // variable type
    private Argument initialValue; // what the variable is initially set to
    private String name; // name of variable
    private boolean isConstant; // true if constant variable; false if not.
    private boolean isArray; // true if array variable; false if not. jass arrays have no explicit size.

    /**
     * Sets up this node with a scanner to receive words.
     *
     * @param inputScanner Scanner containing JASS code
     */
    public Variable(Scanner inputScanner, TreeContext context) {
        super(inputScanner, context);
    }

    public Variable(String type, Argument initialValue, String name, boolean isConstant, boolean isArray, TreeContext context) {
        super(context);
        this.type = type;
        this.initialValue = initialValue;
        this.name = name;
        this.isConstant = isConstant;
        this.isArray = isArray;
    }

    /**
     * Renames the variable and all uses of this variable.
     *
     * @param oldVariableName Existing variable name
     * @param newVariableName Desired variable name
     */
    @Override
    public final void renameVariable(String oldVariableName, String newVariableName) {
        if (this.name.equals(oldVariableName)) {
            this.name = newVariableName;
        } else {
            if (initialValue != null) {
                initialValue.renameVariable(oldVariableName, newVariableName);
            }
        }
    }

    public Variable inline(String functionName, String newText) {
        if (initialValue != null) {
            return new Variable(type, (Argument) initialValue.inline(functionName, newText), name, isConstant, isArray, context);
        }
        return this;
    }

    /**
     * Renames a function and uses to a new name
     *
     * @param oldFunctionName Existing function name
     * @param newFunctionName Desired function name
     */
    @Override
    public final void renameFunction(String oldFunctionName, String newFunctionName) {
        if (initialValue != null) {
            initialValue.renameFunction(oldFunctionName, newFunctionName);
        }
    }

    /**
     * Parse the JASS code contained in the Scanner into a model object
     */
    @Override
    protected final void readNode() {
        List<String> variableParts = new ArrayList<>();
        // Separate by spaces
        Collections.addAll(variableParts, readLine().split("\\s"));

        // Pull out "constant", if applies
        if (variableParts.get(0).equals("constant")) {
            isConstant = true;
            variableParts.remove(0);
        }

        // Type always comes here, before array flag
        type = variableParts.get(0);
        variableParts.remove(0);

        // Pull out "array", if applies
        if (variableParts.get(0).equals("array")) {
            isArray = true;
            variableParts.remove(0);
        }

        // Rejoin before we parse by equal sign
        String reCollected = String.join(" ", variableParts);
        if (reCollected.contains("=")) {
            // Initial value was specified
            int index = reCollected.indexOf("=");
            String namePart = reCollected.substring(0, index);
            String valuePart = reCollected.substring(index + 1);

            name = namePart.trim();
            initialValue = new Argument(new Scanner(valuePart.trim()), context);
        } else {
            // Initial value was not specified
            name = reCollected.trim();
            initialValue = null;
        }
    }

    public boolean usesAsFunction(String functionName) {
        if(initialValue != null) {
            return initialValue.usesAsFunction(functionName);
        } else {
            return false;
        }
    }

    /**
     * Converts this node back to its original form.
     * Indentation is not added.
     *
     * @return Original form of this node (code or string)
     */
    @Override
    public final String toString() {
        // Put the String back together in the same way we pulled it apart.
        StringBuilder converted = new StringBuilder();
        if (isConstant()) {
            converted.append("constant ");
        }
        converted.append(type).append(" ");
        if (isArray()) {
            converted.append("array ");
        }
        converted.append(name);
        if (initialValue != null) {
            converted.append("=").append(initialValue);
        }
        return converted.toString();
    }

    /**
     * Converts this node back to its original form.
     *
     * @param indentationLevel Current indentation level
     * @return Original form of this node (code or string) with indentation
     */
    @Override
    public String toFormattedString(int indentationLevel) {
        return this.toString();
    }

    public final String getType() {
        return type;
    }

    public final Argument getInitialValue() {
        return initialValue;
    }

    public final String getName() {
        return name;
    }

    public final boolean isConstant() {
        return isConstant;
    }

    public final boolean isArray() {
        return isArray;
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
        Variable other = (Variable) obj;
        return this.toString().equals(other.toString());
    }
}
