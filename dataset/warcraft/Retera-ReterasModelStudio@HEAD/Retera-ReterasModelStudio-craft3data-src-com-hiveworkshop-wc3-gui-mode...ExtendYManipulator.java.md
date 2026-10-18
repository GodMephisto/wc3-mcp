---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/newstuff/manipulator/ExtendYManipulator.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.newstuff.manipulator;

import java.awt.geom.Point2D.Double;

import com.hiveworkshop.wc3.gui.modeledit.newstuff.ModelEditor;

public final class ExtendYManipulator extends AbstractExtendManipulator {

	public ExtendYManipulator(final ModelEditor modelEditor) {
		super(modelEditor);
	}

	@Override
	protected void buildMoveVector(final Double mouseStart, final Double mouseEnd, final byte dim1, final byte dim2) {
		moveVector.setCoord(dim2, mouseEnd.y - mouseStart.y);
	}

}
