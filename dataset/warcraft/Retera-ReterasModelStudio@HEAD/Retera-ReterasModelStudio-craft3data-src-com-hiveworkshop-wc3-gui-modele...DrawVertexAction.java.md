---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/creator/actions/DrawVertexAction.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.creator.actions;

import com.hiveworkshop.wc3.gui.modeledit.UndoAction;
import com.hiveworkshop.wc3.mdl.GeosetVertex;

public class DrawVertexAction implements UndoAction {
	private final GeosetVertex vertex;

	public DrawVertexAction(final GeosetVertex vertex) {
		this.vertex = vertex;
	}

	@Override
	public void undo() {
		vertex.getGeoset().remove(vertex);
	}

	@Override
	public void redo() {
		vertex.getGeoset().add(vertex);
	}

	@Override
	public String actionName() {
		return "add vertex";
	}

}
