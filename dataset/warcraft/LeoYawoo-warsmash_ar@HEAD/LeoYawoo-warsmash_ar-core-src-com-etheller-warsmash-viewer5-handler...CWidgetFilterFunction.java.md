---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/simulation/CWidgetFilterFunction.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.simulation;

public interface CWidgetFilterFunction {
    CWidgetFilterFunction ACCEPT_ALL = unit -> true;
    CWidgetFilterFunction ACCEPT_ALL_LIVING = unit -> !unit.isDead();

    boolean call(CWidget unit);
}
