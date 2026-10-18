---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/simulation/CUnitEnumFunction.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.simulation;

public interface CUnitEnumFunction {
    /**
     * Operates on a unit, returning true if we should stop execution.
     *
     */
    boolean call(CUnit unit);
}
