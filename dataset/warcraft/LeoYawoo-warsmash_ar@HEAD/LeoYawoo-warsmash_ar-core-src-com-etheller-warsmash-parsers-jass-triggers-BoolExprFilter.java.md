---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/parsers/jass/triggers/BoolExprFilter.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.jass.triggers;

import com.etheller.interpreter.ast.debug.JassException;
import com.etheller.interpreter.ast.function.JassFunction;
import com.etheller.interpreter.ast.scope.GlobalScope;
import com.etheller.interpreter.ast.scope.TriggerExecutionScope;
import com.etheller.interpreter.ast.scope.trigger.TriggerBooleanExpression;
import com.etheller.interpreter.ast.util.JassSettings;
import com.etheller.interpreter.ast.value.JassValue;
import com.etheller.interpreter.ast.value.visitor.BooleanJassValueVisitor;

import java.util.Collections;
import java.util.Objects;

public class BoolExprFilter implements TriggerBooleanExpression {
    private final JassFunction takesNothingReturnsBooleanFunction;

    public BoolExprFilter(final JassFunction returnsBooleanFunction) {
        this.takesNothingReturnsBooleanFunction = returnsBooleanFunction;
    }

    @Override
    public boolean evaluate(final GlobalScope globalScope, final TriggerExecutionScope triggerScope) {
        final JassValue booleanJassReturnValue;
        if (this.takesNothingReturnsBooleanFunction == null) {
            return false;
        }
        try {
            booleanJassReturnValue = this.takesNothingReturnsBooleanFunction.call(Collections.EMPTY_LIST, globalScope,
                    triggerScope);
        } catch (final Exception e) {
            throw new JassException(globalScope, "Exception during BoolExprFilter.evaluate()", e);
        }
        if ((booleanJassReturnValue == null) && JassSettings.CONTINUE_EXECUTING_ON_ERROR) {
            return false;
        }
        return Objects.requireNonNull(booleanJassReturnValue).visit(BooleanJassValueVisitor.getInstance());
    }

}
