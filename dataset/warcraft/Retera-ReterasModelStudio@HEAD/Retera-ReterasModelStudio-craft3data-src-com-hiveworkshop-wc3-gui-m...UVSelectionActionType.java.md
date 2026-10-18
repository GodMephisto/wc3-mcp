---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/actions/UVSelectionActionType.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.actions;

public enum UVSelectionActionType {
	SELECT, ADD, DESELECT, SELECT_ALL, INVERT_SELECTION, EXPAND_SELECTION, SELECT_FROM_VIEWER;
	;

	public static UVSelectionActionType fromLegacyId(final int selectionType) {
		switch (selectionType) {
		case 0:
			return SELECT;
		case 1:
			return ADD;
		case 2:
			return DESELECT;
		case 3:
			return SELECT_ALL;
		case 4:
			return INVERT_SELECTION;
		case 5:
			return EXPAND_SELECTION;
		case 6:
			return SELECT_FROM_VIEWER;
		}
		return null;
	}
}
