---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/functions/Inputs.java"
ref: HEAD
---

package nodes.functions;

import nodes.AbstractNode;
import exception.ParsingException;
import tree.TreeContext;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Scanner;

/**
 * Represents the Inputs section for a Function, which contains multiple Input entities
 */
public final class Inputs extends AbstractNode {

    private List<Input> inputs;

    /**
     * Sets up this node with a scanner to receive words.
     *
     * @param inputScanner Scanner containing JASS code
     */
    public Inputs(Scanner inputScanner, TreeContext context) {
        super(inputScanner, context);
    }

    /**
     * Sets up any class-level variables before
     * performing the node reading.
     */
    @Override
    protected void setupVariables() {
        this.inputs = new ArrayList<>();
    }

    /**
     * Converts this node back to its original form.
     * Indentation is not added.
     *
     * @return Original form of this node (code or string)
     */
    @Override
    public String toString() {
        if(inputs.isEmpty()) {
            return "takes nothing";
        } else {
            StringBuilder built = new StringBuilder();
            built.append("takes ");
            for(Input input: inputs) {
                built.append(input.toString()).append(",");
            }
            built.setLength(built.length()-1);
            return built.toString();
        }
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
    protected void readNode() {
        String line = readLine();
        if(line.equals("takes nothing")) {
            return;
        }
        if(!line.startsWith("takes ")) {
            throw new ParsingException("Not an inputs line: " + line);
        }
        line = line.substring("takes ".length());
        for(String part : line.split(",")) {
            Input input = new Input(new Scanner(part), context);
            inputs.add(input);
        }
    }

    public List<Input> getInputs() {
        return Collections.unmodifiableList(inputs);
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
        Inputs other = (Inputs) obj;
        return this.toString().equals(other.toString());
    }
}
