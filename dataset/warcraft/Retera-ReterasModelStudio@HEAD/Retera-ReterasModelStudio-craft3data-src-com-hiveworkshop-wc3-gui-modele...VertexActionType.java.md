---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/actions/VertexActionType.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.actions;

public enum VertexActionType {
	MOVE, ROTATE, SCALE, UNKNOWN;

	public static VertexActionType fromLegacyId(final int id) {
		switch (id) {
		case 3:
			return MOVE;
		case 4:
			return ROTATE;
		case 5:
			return SCALE;
		}
		return UNKNOWN;
	}
}
