---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/services/AnalysisService.java"
ref: HEAD
---

package services;

import exception.ParsingException;
import interfaces.IAnalysisService;
import interfaces.ISyntaxTree;
import model.IsolateResult;
import nodes.AbstractFunction;
import nodes.arguments.Argument;
import nodes.j.Variable;

import java.util.*;

/**
 * Service to assist in code analysis
 */
public final class AnalysisService implements IAnalysisService {

    private ExpansionStyle expansionStyle;

    /**
     * Returns current expansion style
     *
     * @return  Expansion style of service
     */
    public ExpansionStyle getExpansionStyle() {
        return expansionStyle;
    }

    /**
     * Sets current expansion style
     * @see IAnalysisService.ExpansionStyle
     *
     * @param expansionStyle    Expansion style
     */
    public void setExpansionStyle(ExpansionStyle expansionStyle) {
        this.expansionStyle = expansionStyle;
    }

    /**
     * Constructs object with default expansion style (passive)
     */
    public AnalysisService() {
        this.expansionStyle = ExpansionStyle.PASSIVE;
    }

    /**
     * Constructs object with custom expansion style
     *
     * @param expansionStyle    Expansion style (@see IAnalysisService.ExpansionStyle)
     */
    public AnalysisService(ExpansionStyle expansionStyle) {
        this.expansionStyle = expansionStyle;
    }

    /**
     * Runs recursive variable isolation (finding functions)
     *
     * @param tree              Syntax tree
     * @param variableName      Variable name to isolate
     * @param iterationCount    Current iteration count (0 = over)
     * @param recursiveResult   Recursive result to add to
     * @return                  Results of isolate command
     */
    private IsolateResult isolateVariable(ISyntaxTree tree, String variableName,
                                          int iterationCount, IsolateResult recursiveResult) {
        if (iterationCount == 0) {
            // Base case - no more iterations to perform.
            // This is not the only base case. The == is intended rather than <=
            return recursiveResult;
        } else {
            // Add the requested variable to be isolated to the isolation result
            Variable selectedVariable = selectVariable(tree, variableName);

            recursiveResult.addVariable(selectedVariable);
            // Find all functions that use this variable
            Set<AbstractFunction> newAddedFunctions = performFunctionExpansion(tree, variableName, recursiveResult);

            // Decrease iteration count because we just finished one
            iterationCount--;

            // Perform recursive calls. Note the implicit base case.
            performRecursiveFunctionSearch(tree, iterationCount, recursiveResult, newAddedFunctions);
            return recursiveResult;
        }
    }

    /**
     * Selects the desired variable from the syntax tree
     *
     * @param tree          Syntax tree
     * @param variableName  Variable name desired to retrieve
     * @return              Variable desired
     */
    private Variable selectVariable(ISyntaxTree tree, String variableName) {
        Variable selectedVariable = null;
        for (Variable variable : tree.getScript().getGlobalsSection().getGlobalVariables()) {
            if (variable.getName().equals(variableName)) {
                selectedVariable = variable;
            }
        }

        if (selectedVariable == null) {
            throw new ParsingException("Expected to be able to isolate on variable "
                    + variableName + " but was not found.");

        }
        return selectedVariable;
    }

    /**
     * Expands variables and discovers all functions using it
     *
     * @param tree              Syntax tree
     * @param variableName      Variable name to expand
     * @param recursiveResult   Result to add to
     * @return                  Set of discovered functions
     */
    private Set<AbstractFunction> performFunctionExpansion(ISyntaxTree tree, String variableName, IsolateResult recursiveResult) {
        Set<AbstractFunction> newAddedFunctions = new LinkedHashSet <>();
        for (AbstractFunction function : tree.getScript()
                .getFunctionsSection().getFunctions()) {
            for (Argument argument : function.getArguments()) {
                if (argument.toString().equals(variableName)) {
                    if (recursiveResult.addFunction(function)) {
                        newAddedFunctions.add(function);
                    }
                    break; // exit inner loop
                }
            }
        }
        return newAddedFunctions;
    }

    /**
     * Runs recursive function isolation
     *
     * @param tree              Syntax tree
     * @param functionName      Function name to isolate
     * @param iterationCount    Current iteration count (0 = over)
     * @param recursiveResult   Result to append to
     * @return                  Result created
     */
    private IsolateResult isolateFunction(ISyntaxTree tree, String functionName,
                                          int iterationCount, IsolateResult recursiveResult) {
        if (iterationCount == 0) {
            // Base case - no more iterations to perform
            // This is not the only base case. The == is intended rather than <=
            return recursiveResult;
        } else {
            // Find all functions/variables used by this function
            // First, let's grab this function's code.
            AbstractFunction selectedFunction = selectFunction(tree, functionName);

            recursiveResult.addFunction(selectedFunction);
            // Grab all used arguments from this function
            List<String> arguments = accumulateArguments(selectedFunction);

            // Set up values for storing new found usages
            Set<AbstractFunction> newAddedFunctions = new LinkedHashSet<>();
            Set<Variable> newAddedVariables = new LinkedHashSet<>();

            // Now scan all variables for usages in this function
            performVariableExpansion(tree, recursiveResult, arguments, newAddedVariables);

            // We do the same for functions (See if this function calls that function)
            performPassiveFunctionExpansion(tree, recursiveResult, selectedFunction, newAddedFunctions);

            // Now do the inverse of the above - check all functions and see if that function calls this function
            // This will expand VERY FAST so we have a feature flag for it
            performAggressiveFunctionExpansion(tree, functionName, recursiveResult, newAddedFunctions);

            // Decrease iteration count because we just finished one
            iterationCount--;

            // Perform recursive calls. Note the implicit base case.
            performRecursiveVariableSearch(tree, iterationCount, recursiveResult, newAddedVariables);
            performRecursiveFunctionSearch(tree, iterationCount, recursiveResult, newAddedFunctions);
            return recursiveResult;
        }
    }

    /**
     * Retrieves the desired function from syntax tree
     *
     * @param tree          Syntax tree
     * @param functionName  Function name to retrieve
     * @return              Function retrieved
     */
    private AbstractFunction selectFunction(ISyntaxTree tree, String functionName) {
        AbstractFunction selectedFunction = null;
        for (AbstractFunction function : tree.getScript().getFunctionsSection().getFunctions()) {
            if (function.getName().equals(functionName)) {
                selectedFunction = function;
                break;
            }
        }
        if (selectedFunction == null) {
            throw new ParsingException("Expected to be able to isolate on function "
                    + functionName + " but was not found.");
        }
        return selectedFunction;
    }

    /**
     * Collects all arguments used in this function
     *
     * @param selectedFunction  Function to retrieve arguments for
     * @return                  List of arguments
     */
    private List<String> accumulateArguments(AbstractFunction selectedFunction) {
        List<String> arguments = new ArrayList<>();
        for (Argument arg : selectedFunction.getArguments()) {
            arguments.add(arg.toString());
        }
        return arguments;
    }

    /**
     * Finds variables that are used by this function
     *
     * @param tree              Syntax tree
     * @param recursiveResult   Result to append to
     * @param arguments         Function arguments to check for variable usages in
     * @param newAddedVariables Set to store results (added variables)
     */
    private void performVariableExpansion(ISyntaxTree tree, IsolateResult recursiveResult,
                                          List<String> arguments, Set<Variable> newAddedVariables) {
        for (Variable variable : tree.getScript().getGlobalsSection().getGlobalVariables()) {
            if (arguments.contains(variable.getName())) {
                if (recursiveResult.getIsolatedVariables().add(variable)) {
                    newAddedVariables.add(variable);
                }
            }
        }
    }

    /**
     * Performs the first (passive) expansion step where we find
     * all functions that are used by this function
     *
     * @param tree                  Syntax tree
     * @param selectedFunction      Function to find other function usages from
     * @param recursiveResult       Result to add to
     * @param newAddedFunctions     Set to add discovered functions to
     */
    private void performPassiveFunctionExpansion(ISyntaxTree tree, IsolateResult recursiveResult,
                                                 AbstractFunction selectedFunction, Set<AbstractFunction>
                                                         newAddedFunctions) {
        if(expansionStyle != ExpansionStyle.INVERSE) {
            for (AbstractFunction function : tree.getScript().getFunctionsSection().getFunctions()) {
                for (Argument arg : selectedFunction.getArguments()) {
                    if (arg.calls(function.getName())) {
                        if (recursiveResult.getIsolatedFunctions().add(function)) {
                            newAddedFunctions.add(function);
                        }
                    }
                }
            }
        }
    }

    /**
     * Finds all functions that use this functions.
     * This will grow the size of the isolation extremely quickly
     * (generally selecting all code with enough iterations)
     * and so is only enabled when aggressive style is selected
     *
     * @param tree                  Syntax tree
     * @param functionName          Function name to find usages of
     * @param recursiveResult       Result to add to
     * @param newAddedFunctions     Set to add discovered functions to
     */
    private void performAggressiveFunctionExpansion(ISyntaxTree tree, String functionName, IsolateResult
            recursiveResult, Set<AbstractFunction> newAddedFunctions) {
        if (expansionStyle == ExpansionStyle.AGGRESSIVE || expansionStyle == ExpansionStyle.INVERSE) {
            for (AbstractFunction function : tree.getScript().getFunctionsSection().getFunctions()) {
                for (Argument arg : function.getArguments()) {
                    if (arg.calls(functionName)) {
                        if (recursiveResult.getIsolatedFunctions().add(function)) {
                            newAddedFunctions.add(function);
                        }
                    }
                }
            }
        }
    }

    /**
     * Recursively attempts to isolate all variables that we
     * discovered through our previous isolation.
     *
     * @param tree              Syntax tree
     * @param iterationCount    Remaining iterations
     * @param recursiveResult   Result to append to
     * @param newAddedVariables Variables to isolate
     */
    private void performRecursiveVariableSearch(ISyntaxTree tree, int iterationCount, IsolateResult
            recursiveResult, Set<Variable> newAddedVariables) {
        for (Variable variable : newAddedVariables) {
            isolateVariable(tree, variable.getName(), iterationCount, recursiveResult);
        }
    }

    /**
     * Recursively attempts to isolate all functions that we
     * discovered through our previous isolation.
     *
     * @param tree              Syntax tree
     * @param iterationCount    Remaining iterations
     * @param recursiveResult   Result to append to
     * @param newAddedFunctions Functions to isolate
     */
    private void performRecursiveFunctionSearch(ISyntaxTree tree, int iterationCount, IsolateResult
            recursiveResult, Set<AbstractFunction> newAddedFunctions) {
        for (AbstractFunction function : newAddedFunctions) {
            isolateFunction(tree, function.getName(), iterationCount, recursiveResult);
        }
    }

    /**
     * Retrieves all functions that use this variable.
     * Variables are stored in an IsolateResult.
     *
     * @param tree           Syntax tree to work from
     * @param variableName   Variable name to isolate
     * @param iterationCount Total iterations (-1 = infinite)
     * @return Isolate result containing only variables
     */
    @Override
    public IsolateResult isolateVariable(ISyntaxTree tree, String variableName, int iterationCount) {
        return isolateVariable(tree, variableName, iterationCount, new IsolateResult());
    }

    /**
     * Retrieves all functions that are called by this
     * function, and all variables that are used by this
     * function.
     *
     * @param tree           Syntax tree to work from
     * @param variableName   Function name to isolate
     * @param iterationCount Total iterations (-1 = infinite)
     * @return Isolate result containing variables/functions
     */
    @Override
    public IsolateResult isolateFunction(ISyntaxTree tree, String variableName, int iterationCount) {
        return isolateFunction(tree, variableName, iterationCount, new IsolateResult());
    }

    /**
     * Discovers scope of the specified variable name
     *
     * @param tree          Syntax tree
     * @param variableName  Variable name to find scope of
     * @return              Variable Scope enum representing variable usage
     */
    public VariableScope findVariableScope(ISyntaxTree tree, String variableName) {
        IsolateResult result = isolateVariable(tree, variableName, 1);
        int size = result.getIsolatedFunctions().size();
        if(size == 0) {
            return VariableScope.UNUSED;
        } else if(size == 1) {
            return VariableScope.SINGLE_USE;
        } else {
            return VariableScope.GLOBAL;
        }
    }

}
