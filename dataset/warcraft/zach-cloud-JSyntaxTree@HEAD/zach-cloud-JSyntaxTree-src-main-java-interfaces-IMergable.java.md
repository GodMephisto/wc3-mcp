---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/interfaces/IMergable.java"
ref: HEAD
---

package interfaces;

import nodes.AbstractNode;

/**
 * Represents a Node's behavior of being able to be merged with
 * another node of the same type without causing error.
 */
public interface IMergable {

    /**
     * Combines this AST Node with another and then checks
     * for errors. Gracefully handles function main.
     *
     * @param other Other AST node to combine
     */
    void merge(AbstractNode other);
}
