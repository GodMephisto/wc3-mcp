---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/model/IsolateResult.java"
ref: HEAD
---

package model;

import nodes.AbstractFunction;
import nodes.j.Variable;

import java.util.HashSet;
import java.util.Set;

public class IsolateResult {

    private Set<AbstractFunction> isolatedFunctions;
    private Set<Variable> isolatedVariables;

    public IsolateResult() {
        this.isolatedFunctions = new HashSet<>();
        this.isolatedVariables = new HashSet<>();
    }

    public boolean addFunction(AbstractFunction function) {
        return isolatedFunctions.add(function);
    }

    public boolean addVariable(Variable variable) {
        return isolatedVariables.add(variable);
    }

    public Set<AbstractFunction> getIsolatedFunctions() {
        return isolatedFunctions;
    }

    public Set<Variable> getIsolatedVariables() {
        return isolatedVariables;
    }
}
