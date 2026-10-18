---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/parsers/jass/triggers/TriggerAction.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.jass.triggers;

import com.etheller.interpreter.ast.scope.trigger.Trigger;

public class TriggerAction {
    private final Trigger trigger;

    public TriggerAction(final Trigger trigger) {
        this.trigger = trigger;
    }

    public Trigger getTrigger() {
        return this.trigger;
    }

}
