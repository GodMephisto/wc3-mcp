---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/interfaces/IAnalysisService.java"
ref: HEAD
---

package interfaces;

import model.IsolateResult;
import services.AnalysisService;

/**
 * Service to assist in code analysis
 */
public interface IAnalysisService {

    /**
     * Aggressive = two sided function expansion (finds both
     * usages of function, and places where function is used)
     *
     * Passive = one sided function expansion (finds usages of function)
     *
     * Inverse = reverse one sided function expansion (finds
     * places where function is used)
     */
    enum ExpansionStyle {
        AGGRESSIVE,
        PASSIVE,
        INVERSE
    }

    /**
     * Unused = variable is never used (can be deleted)
     *
     * Single use = one use, can be local
     *
     * Global = correct usage of a global variable
     */
    enum VariableScope {
        UNUSED,
        SINGLE_USE,
        GLOBAL
    }

    /**
     * Retrieves all functions that use this variable.
     * Variables are stored in an IsolateResult.
     *
     * @param tree           Syntax tree to work from
     * @param variableName   Variable name to isolate
     * @param iterationCount Remaining iterations
     * @return               Isolate result containing only variables
     */
    IsolateResult isolateVariable(ISyntaxTree tree, String variableName, int iterationCount);

    /**
     * Retrieves all functions that are called by this
     * function, and all variables that are used by this
     * function.
     *
     * @param tree           Syntax tree to work from
     * @param variableName   Function name to isolate
     * @param iterationCount Remaining iterations
     * @return               Isolate result containing only variables
     */
    IsolateResult isolateFunction(ISyntaxTree tree, String variableName, int iterationCount);

    /**
     * Sets expansion style of this service.
     *
     * @param style Expansion style
     */
    void setExpansionStyle(AnalysisService.ExpansionStyle style);

    /**
     * Retrieves current expansion style
     *
     * @return  Expansion style
     */
    ExpansionStyle getExpansionStyle();

    /**
     * Finds scope of desired variable
     *
     * @param tree          Syntax tree
     * @param variableName  Variable name to find scope of
     * @return              Variable Scope enum representing variable usage
     */
    VariableScope findVariableScope(ISyntaxTree tree, String variableName);
}
