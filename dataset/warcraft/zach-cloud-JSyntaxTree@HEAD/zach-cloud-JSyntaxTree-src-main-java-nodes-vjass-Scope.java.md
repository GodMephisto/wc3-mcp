---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/vjass/Scope.java"
ref: HEAD
---

package nodes.vjass;

import tree.TreeContext;

import java.util.Scanner;

public final class Scope extends ScriptContainerInitializer {


    /**
     * Sets up this abstract node with a scanner to receive words.
     *
     * @param inputScanner Scanner containing JASS code
     * @param context
     */
    public Scope(Scanner inputScanner, TreeContext context) {
        super(inputScanner, context);
        this.startText = "scope";
        this.endText = "endscope";
    }

    /**
     * No-args constructor used for creating from an existing
     *
     * @param context Tree context
     */
    public Scope(TreeContext context) {
        super(context);
        this.startText = "scope";
        this.endText = "endscope";
    }

    /**
     * Sets up initial variables
     */
    @Override
    protected void setupVariables() {
        super.setupVariables();
        this.startText = "scope";
        this.endText = "endscope";
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
        Scope other = (Scope) obj;
        return this.toString().equals(other.toString());
    }
}
