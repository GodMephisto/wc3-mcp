---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/interfaces/IAbstractNode.java"
ref: HEAD
---

package interfaces;

/**
 * Represents the most basic abstract syntax tree entry possible
 */
public interface IAbstractNode {

    /**
     * Converts this node back to its original form.
     * Indentation is not added.
     *
     * @return Original form of this node (code or string)
     */
    String toString();
}
