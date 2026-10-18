---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/vjass/MethodDeclaration.java"
ref: HEAD
---

package nodes.vjass;

import interfaces.IFunctionRenameable;
import interfaces.IVariableRenameable;
import nodes.AbstractNode;
import nodes.functions.GenericDeclaration;
import nodes.functions.Inputs;
import nodes.functions.Output;
import tree.TreeContext;

import java.util.Scanner;

/**
 * Function Declaration line (i.e. function x takes y returns z)
 */
public final class MethodDeclaration extends GenericDeclaration implements IFunctionRenameable, IVariableRenameable {

    /**
     * Sets up this node with a scanner to receive words.
     *
     * @param inputScanner Scanner containing JASS code
     * @param context Tree context
     */
    public MethodDeclaration(Scanner inputScanner, TreeContext context) {
        super(inputScanner, context);
        setStartText("method");
    }

    /**
     * No-args constructor used for creating from an existing
     *
     * @param context Tree context
     */
    public MethodDeclaration(TreeContext context, String name, Inputs inputs, Output output, boolean constant, String accessModifier) {
        super(context, name, inputs, output, constant, accessModifier);
        setStartText("method");
    }

    /**
     * Sets up initial variables
     */
    @Override
    protected void setupVariables() {
        super.setupVariables();
        setStartText("method");
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
        this.setName(rename(this.getName(), oldFunctionName, newFunctionName));
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
        MethodDeclaration other = (MethodDeclaration) obj;
        return this.toString().equals(other.toString());
    }
}
