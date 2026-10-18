---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/newstuff/manipulator/Manipulator.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.newstuff.manipulator;

import java.awt.Graphics2D;
import java.awt.geom.Point2D;

import com.hiveworkshop.wc3.gui.modeledit.CoordinateSystem;
import com.hiveworkshop.wc3.gui.modeledit.UndoAction;

public interface Manipulator {
	void start(Point2D.Double mouseStart, byte dim1, byte dim2);

	void update(Point2D.Double mouseStart, Point2D.Double mouseEnd, byte dim1, byte dim2);

	UndoAction finish(Point2D.Double mouseStart, Point2D.Double mouseEnd, byte dim1, byte dim2);

	void render(Graphics2D graphics, CoordinateSystem coordinateSystem);
}
