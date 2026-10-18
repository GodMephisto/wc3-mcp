---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/arguments/AggregationArgument.java"
ref: HEAD
---

package nodes.arguments;

import tree.TreeContext;

import java.util.ArrayList;
import java.util.List;

public class AggregationArgument extends ArgumentType {

    /**
     * An aggregation is a combination of two or more
     * basic arguments and/or function calls.
     * i.e. myFunction + 5 + 3
     */
    private List<Argument> aggregation;
    /**
     * The separator between the aggregation entries.
     */
    private String operator;
    private TreeContext context;

    public AggregationArgument(List<Argument> aggregation, String operator, TreeContext context) {
        this.aggregation = aggregation;
        this.operator = operator;
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
        for (Argument arg : aggregation) {
            arg.renameFunction(oldFunctionName, newFunctionName);
        }
    }

    /**
     * Renames the variable and all uses of this variable.
     *
     * @param oldVariableName Existing variable name
     * @param newVariableName Desired variable name
     */
    @Override
    public void renameVariable(String oldVariableName, String newVariableName) {
        for (Argument arg : aggregation) {
            arg.renameVariable(oldVariableName, newVariableName);
        }
    }

    @Override
    public ArgumentType inline(String functionName, String newText) {
        List<Argument> newAggregation = new ArrayList<>();
        for (Argument arg : aggregation) {
            newAggregation.add(arg.inline(functionName, newText));
        }
        return new AggregationArgument(newAggregation, operator, context);
    }

    public String toString() {
        StringBuilder built = new StringBuilder();
        for (Argument aggregate : aggregation) {
            built.append(aggregate.toString()).append(" ").append(operator).append(" ");
        }
        for(int i = 0; i < operator.length() + 2; i++) {
            if(built.length() > 0) {
                built.deleteCharAt(built.length() - 1);
            }
        }
        return built.toString();
    }

    @Override
    public boolean calls(String functionName) {
        return false;
    }

    @Override
    public List<Argument> getArguments() {
        List<Argument> arguments = new ArrayList<>();

        for(Argument argument : aggregation) {
            arguments.addAll(argument.getArguments());
        }

        return arguments;
    }

    @Override
    public boolean usesAsFunction(String functionName) {
        for (Argument arg : aggregation) {
            if(arg.usesAsFunction(functionName)) {
                return true;
            }
        }
        return false;
    }
}
