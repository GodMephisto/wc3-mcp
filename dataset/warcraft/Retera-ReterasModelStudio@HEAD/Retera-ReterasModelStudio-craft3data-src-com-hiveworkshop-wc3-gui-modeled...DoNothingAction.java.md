---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/newstuff/actions/util/DoNothingAction.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.newstuff.actions.util;

import com.hiveworkshop.wc3.gui.modeledit.UndoAction;

public final class DoNothingAction implements UndoAction {
	private final String actionName;

	public DoNothingAction(final String actionName) {
		this.actionName = actionName;
	}

	@Override
	public void undo() {

	}

	@Override
	public void redo() {

	}

	@Override
	public String actionName() {
		return actionName;
	}

}
