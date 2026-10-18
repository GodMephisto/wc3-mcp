---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/interfaces/ISyntaxTree.java"
ref: HEAD
---

package interfaces;

import nodes.AbstractFunction;
import nodes.functions.Function;
import nodes.functions.TypeDeclaration;
import nodes.j.Script;
import nodes.j.Variable;
import services.RandomNameGeneratorService;

import java.io.File;
import java.util.List;

/**
 * Represents an abstract Syntax Tree root that contains a Script
 */
public interface ISyntaxTree {

    /**
     * Returns the base Script file of the tree.
     *
     * @return  Script file
     */
    Script getScript();

    /**
     * Returns all "type" declarations in the script
     *
     * @return  List of type declarations
     */
    List<TypeDeclaration> getTypes();

    /**
     * Returns all global variables in the script
     *
     * @return  List of all global variables
     */
    List<Variable> getGlobalVariables();

    /**
     * Returns all functions in the script
     *
     * @return  List of all functions
     */
    List<AbstractFunction> getFunctions();

    /**
     * Returns the Tree as a formatted String
     *
     * @return  Formatted tree
     */
    String getFormatted();

    /**
     * Returns the Tree as a non-formatted String
     *
     * @return  Non-formatted tree
     */
    String getString();

    /**
     * Adds a blank function main to make the script compile
     */
    void addFunctionMain();

    /**
     * Writes this SyntaxTree out to a file.
     *
     * @param file  File path to write to
     */
    void write(File file);

    /**
     * Combines this SyntaxTree with another and then checks
     * for errors. Gracefully handles function main.
     *
     * @param other Other syntax tree to combine
     */
    void merge(ISyntaxTree other);

    /**
     * Changes the name of a variable from old to new name
     *
     * @param oldVariableName   Existing variable name
     * @param newVariableName   New variable name
     */
    void renameVariable(String oldVariableName, String newVariableName);

    /**
     * Changes the name of a function from old to new name
     *
     * @param oldFunctionName Existing function name
     * @param newFunctionName New function name
     */
    void renameFunction(String oldFunctionName, String newFunctionName);

    /**
     * Cleans up the code slightly
     *
     * @return Cleaned up syntax tree
     */
    ISyntaxTree postprocess();

    /**
     * Changes the name of a local variable from old to new name
     *
     * @param containingFunction   The function that has the local variable inside it
     * @param oldLocalVariableName Existing local variable name
     * @param newLocalVariableName New local variable name
     */
    void renameLocalVariable(String containingFunction, String oldLocalVariableName, String newLocalVariableName);

    /**
     * Generates a new Tree with randomized variable and function names
     *
     * @param generator Random name generator to make variable names
     */
    void deduplicate(IRandomNameGeneratorService generator);


}
