---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/creator/PlaceableModelElement.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.creator;

import javax.swing.JPanel;

public interface PlaceableModelElement {
	JPanel getPanel();

	String getElementTypeName();
}
