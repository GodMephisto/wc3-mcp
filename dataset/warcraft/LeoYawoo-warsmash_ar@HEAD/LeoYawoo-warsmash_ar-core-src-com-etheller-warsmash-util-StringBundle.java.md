---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/util/StringBundle.java"
ref: HEAD
---

package com.etheller.warsmash.util;

public interface StringBundle {
    StringBundle EMPTY = string -> string;

    String getString(String string);
}
