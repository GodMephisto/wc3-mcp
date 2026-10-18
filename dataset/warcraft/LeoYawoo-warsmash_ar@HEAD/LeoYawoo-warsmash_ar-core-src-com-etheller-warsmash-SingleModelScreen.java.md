---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/SingleModelScreen.java"
ref: HEAD
---

package com.etheller.warsmash;

public interface SingleModelScreen {
    void setModel(String path);

    void alternateModelToBattlenet();

    void unAlternateModelBackToNormal();
}
