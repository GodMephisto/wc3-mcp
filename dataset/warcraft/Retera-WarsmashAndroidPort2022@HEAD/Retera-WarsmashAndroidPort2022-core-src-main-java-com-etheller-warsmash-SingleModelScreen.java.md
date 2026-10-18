---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/SingleModelScreen.java"
ref: HEAD
---

package com.etheller.warsmash;

public interface SingleModelScreen {
	void setModel(String path);

	void alternateModelToBattlenet();

	void unAlternateModelBackToNormal();
}
