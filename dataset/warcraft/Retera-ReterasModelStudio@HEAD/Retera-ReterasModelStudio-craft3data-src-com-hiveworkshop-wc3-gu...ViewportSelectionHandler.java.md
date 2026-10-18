---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/newstuff/ViewportSelectionHandler.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.newstuff;

import java.awt.Point;
import java.awt.geom.Rectangle2D;

import com.hiveworkshop.wc3.gui.modeledit.CoordinateSystem;
import com.hiveworkshop.wc3.gui.modeledit.UndoAction;

public interface ViewportSelectionHandler {
	UndoAction selectRegion(Rectangle2D region, CoordinateSystem coordinateSystem);

	boolean canSelectAt(Point point, CoordinateSystem axes);
}
