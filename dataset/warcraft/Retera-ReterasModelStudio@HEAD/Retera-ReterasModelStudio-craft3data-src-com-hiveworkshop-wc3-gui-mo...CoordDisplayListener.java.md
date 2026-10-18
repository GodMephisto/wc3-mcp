---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/CoordDisplayListener.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit;

public interface CoordDisplayListener {
	void notifyUpdate(byte dimension1, byte dimension2, double coord1, double coord2);
}
