---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/SingleModelScreen.java"
ref: HEAD
---

package com.etheller.warsmash;

import com.etheller.warsmash.viewer5.FogSettings;

public interface SingleModelScreen {
	void setModel(String path, FogSettings fogSettings);

	void alternateModelToBattlenet();

	void unAlternateModelBackToNormal();
}
