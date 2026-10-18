---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/AbstractFunction.java"
ref: HEAD
---

package nodes;

import interfaces.IFunctionRenameable;
import interfaces.IVariableRenameable;
import nodes.arguments.Argument;
import tree.TreeContext;

import java.util.List;
import java.util.Scanner;

/**
 * Represents a Function-like entity. Can be a Function itself or a Native Function.
 */
public abstract class AbstractFunction extends AbstractNode implements IFunctionRenameable, IVariableRenameable {

    /**
     * Sets up this abstract node with a scanner to receive words.
     *
     * @param fileScanner   Scanner pointing to file
     * @param context Tree context
     */
    public AbstractFunction(Scanner fileScanner, TreeContext context) {
        super(fileScanner, context);
    }

    /**
     * No-args constructor used for creating from an existing
     *
     * @param context Tree context
     */
    public AbstractFunction(TreeContext context) {
        super(context);
    }

    /**
     * Renames the variable and all uses of this variable.
     *
     * @param oldVariableName   Existing variable name
     * @param newVariableName   Desired variable name
     */
    public abstract void renameVariable(String oldVariableName, String newVariableName);

    /**
     * Renames a function and uses to a new name
     *
     * @param oldFunctionName   Existing function name
     * @param newFunctionName   Desired function name
     */
    public abstract void renameFunction(String oldFunctionName, String newFunctionName);

    /**
     * Returns function name.
     *
     * @return  Function name.
     */
    public abstract String getName();

    public abstract List<Argument> getArguments();
}
