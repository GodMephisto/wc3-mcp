---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/nodes/vjass/ScriptContainerInitializer.java"
ref: HEAD
---

package nodes.vjass;

import tree.TreeContext;

import java.util.Scanner;

public abstract class ScriptContainerInitializer extends ScriptContainer {

    private String initializer;

    /**
     * Sets up this abstract node with a scanner to receive words.
     *
     * @param inputScanner Scanner containing JASS code
     * @param context Tree context
     */
    public ScriptContainerInitializer(Scanner inputScanner, TreeContext context) {
        super(inputScanner, context);
    }

    /**
     * No-args constructor used for creating from an existing
     *
     * @param context Tree context
     */
    public ScriptContainerInitializer(TreeContext context) {
        super(context);
    }

    /**
     * Parses a line containing the name of the ScriptContainer
     * Captures initializers
     *
     * @param line  Name line
     */
    @Override
    protected void parseNameLine(String line) {
        if (line.contains("initializer ")) {
            this.initializer = line.substring(line.indexOf("initializer ") + "initializer ".length());
        }
    }

    /**
     * Creates needed flags to turn this ScriptContainer into
     * a String
     *
     * @param builder   StringBuilder to add to
     */
    @Override
    protected void buildFlags(StringBuilder builder) {
        if (initializer != null && !initializer.isEmpty()) {
            builder.append(" initializer ").append(initializer);
        }
    }
}
