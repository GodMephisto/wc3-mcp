---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/units/LMUnit.java"
ref: HEAD
---

package com.etheller.warsmash.units;

import java.util.LinkedHashMap;

public class LMUnit extends Element {

    public LMUnit(final String id, final DataTable table) {
        super(id, table);
        this.fields = new LinkedHashMap<>();
    }

}
