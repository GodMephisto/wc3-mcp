---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/actions/SelectionActionType.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.actions;

public enum SelectionActionType {
	SELECT, ADD, DESELECT, SELECT_ALL, INVERT_SELECTION, EXPAND_SELECTION;
	;

	public static SelectionActionType fromLegacyId(final int selectionType) {
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
		}
		return null;
	}
}
