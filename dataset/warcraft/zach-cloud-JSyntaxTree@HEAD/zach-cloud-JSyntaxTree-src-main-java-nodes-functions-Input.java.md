---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/functions/Input.java"
ref: HEAD
---

package nodes.functions;

import nodes.AbstractNode;
import nodes.j.Variable;
import exception.ParsingException;
import tree.TreeContext;

import java.util.Scanner;

/**
 * Represents a single atomic Input for example "string s"
 */
public final class Input extends AbstractNode {

    private Variable inputVariable;

    /**
     * Sets up this node with a scanner to receive words.
     *
     * @param inputScanner Scanner containing JASS code
     */
    public Input(Scanner inputScanner, TreeContext context) {
        super(inputScanner, context);
    }

    /**
     * Converts this node back to its original form.
     * Indentation is not added.
     *
     * @return Original form of this node (code or string)
     */
    @Override
    public final String toString() {
        return inputVariable.toString();
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

    /**
     * Parse the JASS code contained in the Scanner into a model object
     */
    @Override
    protected final void readNode() {
        inputVariable = new Variable(new Scanner(readLine()), context);
        if(inputVariable.isConstant()) {
            throw new ParsingException("Constant input not allowed");
        }
        if(inputVariable.isArray()) {
            throw new ParsingException("Array input not allowed");
        }
        if(inputVariable.getInitialValue() != null) {
            throw new ParsingException("Initial value input not allowed");
        }
    }

    public final String getType() {
        return inputVariable.getType();
    }

    public final String getName() {
        return inputVariable.getName();
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
        Input other = (Input) obj;
        return this.toString().equals(other.toString());
    }
}
