---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/creator/ToolbenchCategory.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.creator;

import java.util.List;

public final class ToolbenchCategory {
	private final List<PlaceableModelElement> modelElements;

	public ToolbenchCategory(final List<PlaceableModelElement> modelElements) {
		this.modelElements = modelElements;
	}
}
