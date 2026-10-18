---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/functions/NativeFunction.java"
ref: HEAD
---

package nodes.functions;

import interfaces.IFunctionRenameable;
import interfaces.IVariableRenameable;
import nodes.AbstractFunction;
import exception.ParsingException;
import nodes.arguments.Argument;
import tree.TreeContext;

import java.util.ArrayList;
import java.util.List;
import java.util.Scanner;

/**
 * Represents a Native Function. Looks like a function, but begins with "native" rather than "function"
 */
public final class NativeFunction extends AbstractFunction implements IFunctionRenameable, IVariableRenameable {

    private String name;
    private Inputs inputs;
    private Output output;
    private boolean constant;

    /**
     * Sets up this node with a scanner to receive words.
     *
     * @param inputScanner Scanner containing JASS code
     */
    public NativeFunction(Scanner inputScanner, TreeContext context) {
        super(inputScanner, context);
    }

    /**
     * No-args constructor used for creating from an existing
     *
     * @param context
     */
    public NativeFunction(TreeContext context, String name, Inputs inputs, Output output, boolean constant) {
        super(context);
        this.name = name;
        this.inputs = inputs;
        this.output = output;
        this.constant = constant;
    }

    /**
     * Parse the JASS code contained in the Scanner into a model object
     */
    @Override
    protected final void readNode() {
        String line = readLine();
        line = line.replace("\t", " ");
        while(line.contains("  ")) {
            line = line.replace("  ", " ");
        }
        if(line.startsWith("constant ")) {
            line = line.substring("constant ".length());
            this.constant = true;
        } else {
            this.constant = false;
        }
        if(!line.startsWith("native ")) {
            throw new ParsingException("Not a native declaration: " + line);
        }
        line = line.substring("native ".length());
        String name = line.substring(0, line.indexOf(" "));
        line = line.substring(name.length());
        String inputs = line.substring(0, line.indexOf("returns"));
        line = line.substring(inputs.length());
        String outputs = line;

        this.name = name;
        this.inputs = new Inputs(new Scanner(inputs), context);
        this.output = new Output(new Scanner(outputs), context);
    }

    /**
     * Renames the variable and all uses of this variable.
     *
     * @param oldVariableName   Existing variable name
     * @param newVariableName   Desired variable name
     */
    @Override
    public final void renameVariable(String oldVariableName, String newVariableName) {
        // do nothing
    }

    /**
     * Renames a function and uses to a new name
     *
     * @param oldFunctionName   Existing function name
     * @param newFunctionName   Desired function name
     */
    @Override
    public final void renameFunction(String oldFunctionName, String newFunctionName) {
        this.name = rename(name, oldFunctionName, newFunctionName);
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
        if(constant) {
            builder.append("constant ");
        }
        builder.append("native ").append(name).append(" ").append(inputs.toString()).append(" ").append(output.toString());
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
        return this.toString();
    }

    @Override
    public final String getName() {
        return name;
    }

    public final Inputs getInputs() {
        return inputs;
    }

    public final Output getOutput() {
        return output;
    }

    public final List<Argument> getArguments() {
        List<Argument> arguments = new ArrayList<>();
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
        NativeFunction other = (NativeFunction) obj;
        return this.toString().equals(other.toString());
    }
}
