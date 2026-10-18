---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/functions/LoopStatement.java"
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
 * Represents a block of code starting with loop and going until endloop
 */
public final class LoopStatement extends AbstractStatement implements IFunctionRenameable, IVariableRenameable {

    private Statements statements;

    /**
     * Sets up this node with a scanner to receive words.
     *
     * @param inputScanner Scanner containing JASS code
     */
    public LoopStatement(Scanner inputScanner, TreeContext context) {
        super(inputScanner, context);
    }

    /**
     * No-args constructor used for creating from an existing
     */
    public LoopStatement(Statements statements, TreeContext context) {
        super(context);
        this.statements = statements;
    }

    /**
     * Converts this node back to its original form.
     * Indentation is not added.
     *
     * @return Original form of this node (code or string)
     */
    @Override
    public final String toString() {
        StringBuilder built = new StringBuilder();
        built.append("loop").append("\n");
        built.append(statements.toString()).append("\n");
        built.append("endloop");
        return built.toString();
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
        built.append("loop").append("\n");
        built.append(statements.toFormattedString(indentationLevel+1)).append("\n");
        addTabs(built, indentationLevel);
        built.append("endloop");
        return built.toString();
    }

    /**
     * Renames the variable and all uses of this variable.
     *
     * @param oldVariableName   Existing variable name
     * @param newVariableName   Desired variable name
     */
    public final void renameVariable(String oldVariableName, String newVariableName) {
        statements.renameVariable(oldVariableName, newVariableName);
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
        return new LoopStatement((Statements)statements.inline(functionName, newText), context);
    }

    /**
     * Renames a function and uses to a new name
     *
     * @param oldFunctionName   Existing function name
     * @param newFunctionName   Desired function name
     */
    @Override
    public final void renameFunction(String oldFunctionName, String newFunctionName) {
        statements.renameFunction(oldFunctionName, newFunctionName);
    }

    public boolean usesAsFunction(String functionName) {
        return statements.usesAsFunction(functionName);
    }

    /**
     * Parse the JASS code contained in the Scanner into a model object
     */
    @Override
    protected final void readNode() {
        StringBuilder loopLines = new StringBuilder();
        boolean readingLoop = false;
        int loopLevel = 0; // There can be nested loops!
        while(hasNextLine()) {
            String line = readLine();
            if(line.startsWith("loop")) {
                loopLevel++;
                if(loopLevel == 1) {
                    readingLoop = true;
                } else {
                    loopLines.append(line).append("\n"); // It's a nested loop
                }

            } else if(line.startsWith("endloop")) {
                if(loopLevel == 1) {
                    if (readingLoop) {
                        readingLoop = false;
                    } else {
                        throw new ParsingException("Found endloop before loop");
                    }
                } else {
                    // If not, then it's an endloop for a nested loop
                    loopLines.append(line).append("\n");
                    loopLevel--;
                }
            } else if(readingLoop) {
                loopLines.append(line).append("\n");
            }
        }
        removeFinalCharacter(loopLines);
        statements = new Statements(new Scanner(loopLines.toString()), context);
    }

    public final Argument getExitCondition() {
        for(AbstractStatement statement: statements.getStatements()) {
            if(statement instanceof ExitWhenStatement) {
                return ((ExitWhenStatement)statement).getExitwhenCondition();
            }
        }
        return null;
    }

    public final Statements getStatements() {
        return statements;
    }

    public final List<Argument> getArguments() {
        List<Argument> arguments = new ArrayList<>();
        arguments.addAll(statements.getArguments());
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
        LoopStatement other = (LoopStatement) obj;
        return this.toString().equals(other.toString());
    }
}
