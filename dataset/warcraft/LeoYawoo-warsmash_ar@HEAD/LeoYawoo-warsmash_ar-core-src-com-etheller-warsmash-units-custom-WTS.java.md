---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/units/custom/WTS.java"
ref: HEAD
---

package com.etheller.warsmash.units.custom;

public interface WTS {
    WTS DO_NOTHING = key -> "TRIGSTR_" + key;

    String get(int key);
}
