---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "README.md"
ref: HEAD
---

# JSyntaxTree
This project contains is a syntax tree for the JASS language. The purpose is to read a single .j file (or text file containing JASS code) and parse it into a logical representation. This project is not runnable and only of interest to programmers who wish to modify/read JASS code using an external application.

If you're a non-programmer who is just looking for something to run, you probably want [JAST](https://github.com/zach-cloud/JAST/issues)

VJass is supported in an experimental form. It is not guaranteed to work for every application of VJass but it does handle the cases I could think of.

# Maven Import

To use this project, include the following dependency in your pom file:

```
<dependency>
    <groupId>com.github.zach-cloud</groupId>
    <artifactId>jsyntaxtree</artifactId>
    <version>1.8</version>
</dependency>
```

Versions below 1.3 are unstable.

# Usage

To create a syntax tree from a file on disk:

```
ISyntaxTree tree = SyntaxTree.readTree(selectedFile);
```

To create a syntax tree from a String:

```
ISyntaxTree tree = SyntaxTree.readTree(myString);
```

The syntax tree provides several methods to perform simple operations on code. These operations are listed below. If none of these operations satisfy your use case, you can use the getScript() command to retrieve the Map Script as an object, and then act on that Script however you'd like.
Here's an example of how I used this in order to inline single-line return methods:

```
    /**
     * Inlines one-line return statements
     *
     * @param whichFunction Function to inline
     * @param original      The tree containing the function
     * @return Syntax tree with inlined functions
     */
    @Override
    public ISyntaxTree inline(Function whichFunction, ISyntaxTree original) {
        ReturnStatement thisStatement = (ReturnStatement)whichFunction.getStatements().getStatements().get(0);
        FunctionsSection section = original.getScript().getFunctionsSection();
        List<AbstractFunction> newFunctions = new ArrayList<>();

        for(AbstractFunction function : section.getFunctions()) {
            if(!function.getName().equals(whichFunction.getName())) {
                if(function instanceof Function) {
                    Function entity = (Function)function;
                    Statements newStatements = entity.getStatements().inline(whichFunction.getName(), thisStatement.getReturnBody());
                    newFunctions.add(new Function(entity.getFunctionDeclaration(), newStatements, new TreeContext()));
                } else {
                    newFunctions.add(function);
                }
            }
        }
        return new SyntaxTree(new Script(original.getScript().getGlobalsSection(), new FunctionsSection(newFunctions, new TreeContext()), original.getTypes(), new TreeContext()));
    }
```

If you'd like support on how to implement a specific use case using the syntax tree, please feel free to open an issue and I'll help you out.

# Basis functions provided

You can perform the following operations on the syntax tree with one line of code:

Any function on the SyntaxTree class not listed here should be assume to be unsupported.

```
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
     */
    void postprocess();

    /**
     * Generates a new Tree with randomized variable and function names
     *
     * @param generator Random name generator to make variable names
     */
    void deduplicate(IRandomNameGeneratorService generator);


```

# Services

There are several services included in the "services" package that can perform operations on the syntax tree.

Right now, the primary external facing service is the AnalysisService which can perform code analysis

# Building

Clone the repository, run mvn clean, run mvn package.
