---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/viewer5/handlers/ModelInstanceDescriptor.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers;

import com.etheller.warsmash.viewer5.Model;
import com.etheller.warsmash.viewer5.ModelInstance;

public interface ModelInstanceDescriptor {
	ModelInstance create(Model model);
}
