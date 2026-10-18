---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/arguments/BasicArgument.java"
ref: HEAD
---

package nodes.arguments;

import tree.TreeContext;

import java.util.ArrayList;
import java.util.List;

public class BasicArgument extends ArgumentType {

    private String content;
    private TreeContext context;

    public BasicArgument(String content, TreeContext context) {
        this.content = content;
        this.context = context;
    }

    /**
     * Renames a function and uses to a new name
     *
     * @param oldFunctionName Existing function name
     * @param newFunctionName Desired function name
     */
    @Override
    public void renameFunction(String oldFunctionName, String newFunctionName) {
        this.content = rename(content, oldFunctionName, newFunctionName);
    }

    /**
     * Renames the variable and all uses of this variable.
     *
     * @param oldVariableName Existing variable name
     * @param newVariableName Desired variable name
     */
    @Override
    public void renameVariable(String oldVariableName, String newVariableName) {
        this.content = rename(content, oldVariableName, newVariableName);
    }

    @Override
    public ArgumentType inline(String functionName, String newText) {
        // Cannot be inlined.
        return this;
    }

    @Override
    public boolean calls(String functionName) {
        return false;
    }

    @Override
    public boolean usesAsFunction(String functionName) {
        return content.contains("function " + functionName);
    }

    @Override
    public List<Argument> getArguments() {
        return new ArrayList<>();
    }

    public String toString() {
        return content;
    }
}
