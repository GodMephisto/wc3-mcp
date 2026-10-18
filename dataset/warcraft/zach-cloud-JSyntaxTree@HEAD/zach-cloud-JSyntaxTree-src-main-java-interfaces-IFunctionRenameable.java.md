---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/interfaces/IFunctionRenameable.java"
ref: HEAD
---

package interfaces;

import nodes.AbstractNode;

/**
 * Declates that this node can rename functions/function uses
 */
public interface IFunctionRenameable {

    /**
     * Renames a function and uses to a new name
     *
     * @param oldFunctionName   Existing function name
     * @param newFunctionName   Desired function name
     */
    void renameFunction(String oldFunctionName, String newFunctionName);
}
