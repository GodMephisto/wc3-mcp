---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/simulation/CWidgetVisitor.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.simulation;

public interface CWidgetVisitor<T> {
    T accept(CUnit target);

    T accept(CDestructable target);

    T accept(CItem target);
}
