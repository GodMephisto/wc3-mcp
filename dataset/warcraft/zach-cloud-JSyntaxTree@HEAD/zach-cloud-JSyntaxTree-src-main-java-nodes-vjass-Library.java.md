---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/vjass/Library.java"
ref: HEAD
---

package nodes.vjass;

import tree.TreeContext;

import java.util.Scanner;

public final class Library extends ScriptContainerInitializer {


    /**
     * Sets up this abstract node with a scanner to receive words.
     *
     * @param inputScanner Scanner containing JASS code
     * @param context Tree context
     */
    public Library(Scanner inputScanner, TreeContext context) {
        super(inputScanner, context);
        this.startText = "library";
        this.endText = "endlibrary";
    }

    /**
     * No-args constructor used for creating from an existing
     *
     * @param context Tree context
     */
    public Library(TreeContext context) {
        super(context);
        this.startText = "library";
        this.endText = "endlibrary";
    }

    /**
     * Sets up initial variables
     */
    @Override
    protected void setupVariables() {
        super.setupVariables();
        this.startText = "library";
        this.endText = "endlibrary";
    }
}
