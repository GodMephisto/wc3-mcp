---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/j/GlobalsSection.java"
ref: HEAD
---

package nodes.j;

import interfaces.IFunctionRenameable;
import interfaces.IMergable;
import interfaces.IVariableRenameable;
import nodes.AbstractNode;
import exception.ParsingException;
import tree.TreeContext;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Scanner;

/**
 * Represents a code fragment starting with "globals" and ending with "endglobals"
 */
public final class GlobalsSection extends AbstractNode implements IMergable, IVariableRenameable {

    private List<Variable> globalVariables;

    /**
     * Sets up this node with a scanner to receive words.
     *
     * @param inputScanner Scanner containing JASS code
     */
    public GlobalsSection(Scanner inputScanner, TreeContext context) {
        super(inputScanner, context);
    }

    public GlobalsSection(List<Variable> globalVariables, TreeContext context) {
        super(context);
        this.globalVariables = new ArrayList<>();
        this.globalVariables.addAll(globalVariables);
    }

    /**
     * Sets up any class-level variables before
     * performing the node reading.
     */
    @Override
    protected void setupVariables() {
        globalVariables = new ArrayList<>();
    }

    /**
     * Renames the variable and all uses of this variable.
     *
     * @param oldVariableName   Existing variable name
     * @param newVariableName   Desired variable name
     */
    @Override
    public final void renameVariable(String oldVariableName, String newVariableName) {
        for(Variable variable : globalVariables) {
            variable.renameVariable(oldVariableName, newVariableName);
        }
    }

    /**
     * Parse the JASS code contained in the Scanner into a model object
     */
    @Override
    protected void readNode() {
        boolean reading = true;
        String inputLine = "";
        boolean readingGlobals = false;
        while (reading) {
            inputLine = readLine();
            if (inputLine.equals("globals")) {
                if (readingGlobals) {
                    throw new ParsingException("Found globals twice");
                } else {
                    readingGlobals = true;
                }
            } else if (inputLine.equals("endglobals")) {
                if (!readingGlobals) {
                    throw new ParsingException("Found endglobals before globals");
                } else {
                    readingGlobals = false;
                    reading = false;
                }
            } else if (!inputLine.isEmpty()) {
                if (readingGlobals) {
                    // Each line should be a variable, if it's not empty.
                    Variable variable = new Variable(new Scanner(inputLine), context);
                    globalVariables.add(variable);
                }
            }
        }
    }

    /**
     * Converts this node back to its original form.
     * Indentation is not added.
     *
     * @return Original form of this node (code or string)
     */
    @Override
    public String toString() {
        StringBuilder builtString = new StringBuilder();
        builtString.append("globals").append("\n");
        for (Variable globalVariable : globalVariables) {
            builtString.append(globalVariable.toString()).append("\n");
        }
        builtString.append("endglobals");
        return builtString.toString();
    }

    /**
     * Converts this node back to its original form.
     *
     * @param indentationLevel Current indentation level
     * @return Original form of this node (code or string) with indentation
     */
    @Override
    public String toFormattedString(int indentationLevel) {
        StringBuilder builtString = new StringBuilder();
        builtString.append("globals").append("\n");
        for (Variable globalVariable : globalVariables) {
            addTabs(builtString, indentationLevel);
            builtString.append(globalVariable.toFormattedString(indentationLevel+1)).append("\n");
        }
        builtString.append("endglobals");
        return builtString.toString();
    }

    public List<Variable> getGlobalVariables() {
        return Collections.unmodifiableList(globalVariables);
    }

    /**
     * Combines this AST Node with another and then checks
     * for errors. Gracefully handles function main.
     *
     * @param other Other AST node to combine
     */
    public final void merge(AbstractNode other) {
        globalVariables.addAll(((GlobalsSection)other).globalVariables);
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
        GlobalsSection other = (GlobalsSection) obj;
        return this.toString().equals(other.toString());
    }
}
