---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/simulation/CDestructableEnumFunction.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.simulation;

public interface CDestructableEnumFunction {
    /**
     * Operates on a destructable, returning true if we should stop execution.
     *
     */
    boolean call(CDestructable destructable);
}
