---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/vjass/Method.java"
ref: HEAD
---

package nodes.vjass;

import exception.ParsingException;
import interfaces.IFunctionRenameable;
import interfaces.IMergable;
import interfaces.IVariableRenameable;
import nodes.AbstractFunction;
import nodes.AbstractNode;
import nodes.arguments.Argument;
import nodes.functions.Statements;
import tree.TreeContext;

import java.util.ArrayList;
import java.util.List;
import java.util.Scanner;

/**
 * Represents a Function code block from the function declaration to endfunction
 */
public final class Method extends AbstractFunction implements IMergable, IFunctionRenameable, IVariableRenameable {

    private MethodDeclaration functionDeclaration;
    private Statements statements;

    /**
     * Sets up this node with a scanner to receive words.
     *
     * @param inputScanner Scanner containing JASS code
     * @param context Tree context
     */
    public Method(Scanner inputScanner, TreeContext context) {
        super(inputScanner, context);
    }

    /**
     * Copy constructor for Method
     *
     * @param functionDeclaration   Declaration line
     * @param statements    Statements section
     * @param context Tree context
     */
    public Method(MethodDeclaration functionDeclaration, Statements statements, TreeContext context) {
        super(context);
        this.functionDeclaration = functionDeclaration;
        this.statements = statements;
    }

    /**
     * Renames the variable and all uses of this variable.
     *
     * @param oldVariableName   Existing variable name
     * @param newVariableName   Desired variable name
     */
    @Override
    public final void renameVariable(String oldVariableName, String newVariableName) {
        statements.renameVariable(oldVariableName, newVariableName);
    }

    /**
     * Renames a function and uses to a new name
     *
     * @param oldFunctionName   Existing function name
     * @param newFunctionName   Desired function name
     */
    @Override
    public final void renameFunction(String oldFunctionName, String newFunctionName) {
        this.functionDeclaration.renameFunction(oldFunctionName, newFunctionName);
        this.statements.renameFunction(oldFunctionName, newFunctionName);
    }

    /**
     * Converts this node back to its original form.
     * Indentation is not added.
     *
     * @return Original form of this node (code or string)
     */
    @Override
    public final String toString() {
        StringBuilder builder = new StringBuilder();
        builder.append(functionDeclaration.toString()).append("\n");
        builder.append(statements.toString()).append("\n");
        builder.append("endmethod");
        return builder.toString();
    }

    /**
     * Converts this node back to its original form.
     *
     * @param indentationLevel Current indentation level
     * @return Original form of this node (code or string) with indentation
     */
    @Override
    public String toFormattedString(int indentationLevel) {
        StringBuilder builder = new StringBuilder();
        addTabs(builder, indentationLevel-1);
        builder.append(functionDeclaration.toString()).append("\n");
        builder.append(statements.toFormattedString(indentationLevel)).append("\n");
        addTabs(builder, indentationLevel-1);
        builder.append("endmethod");
        return builder.toString();
    }

    /**
     * Parse the JASS code contained in the Scanner into a model object
     */
    @Override
    protected final void readNode() {
        // Get first line as function declaration
        this.functionDeclaration = readFunctionDeclaration(readLine());
        // Get lines up to endfunction
        StringBuilder lines = new StringBuilder();
        while(hasNextLine()) {
            String line = readLine();
            if(!line.startsWith("endmethod")) {
                lines.append(line).append("\n");
            }
        }
        if(lines.length() > 0) {
            lines.setLength(lines.length()-1);
        }
        this.statements = readStatements(lines.toString());
    }

    /**
     * Reads declaration line
     *
     * @param line  Declaration line
     * @return  Declaration object
     */
    private MethodDeclaration readFunctionDeclaration(String line) {
        MethodDeclaration declaration = new MethodDeclaration(new Scanner(line), context);
        return declaration;
    }

    /**
     * Reads statements section
     *
     * @param lines Lines to read
     * @return  Statements section
     */
    private Statements readStatements(String lines) {
        Statements statements = new Statements(new Scanner(lines), context);
        return statements;
    }

    /**
     * Returns function declaration.
     *
     * @return  Declaration
     */
    public final MethodDeclaration getFunctionDeclaration() {
        return functionDeclaration;
    }

    /**
     * Gets the statements section
     *
     * @return  Statements
     */
    public final Statements getStatements() {
        return statements;
    }

    /**
     * Gets name of this method
     *
     * @return  Name of method as a String
     */
    @Override
    public String getName() {
        return functionDeclaration.getName();
    }

    /**
     * Combines this AST Node with another and then checks
     * for errors. Gracefully handles function main.
     *
     * @param other Other AST node to combine
     */
    @Override
    public final void merge(AbstractNode other) {
        Method otherFunction = (Method)other;
        if(!this.functionDeclaration.toString().equals(((Method) other).functionDeclaration.toString())) {
            throw new ParsingException("Incompatible merge: " + this.functionDeclaration.toString() + ", " + ((Method) other).functionDeclaration.toString());
        }

        Statements otherStatements = (Statements)otherFunction.statements;
        this.statements.merge(otherStatements);
    }

    /**
     * Detects whether this method uses a function
     *
     * @param functionName  Function name to check
     * @return              True if uses; false if does not use.
     */
    public boolean usesAsFunction(String functionName) {
        return statements.usesAsFunction(functionName);
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
        Method other = (Method) obj;
        return this.toString().equals(other.toString());
    }

    /**
     * Gets all Arguments used by this method
     *
     * @return  All arguments
     */
    public final List<Argument> getArguments() {
        List<Argument> arguments = new ArrayList<>();
        arguments.addAll(statements.getArguments());
        return arguments;
    }
}
