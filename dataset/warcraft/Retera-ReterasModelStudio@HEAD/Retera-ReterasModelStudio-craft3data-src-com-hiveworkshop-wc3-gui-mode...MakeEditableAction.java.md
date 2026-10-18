---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/newstuff/actions/selection/MakeEditableAction.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.newstuff.actions.selection;

import com.hiveworkshop.wc3.gui.modeledit.UndoAction;
import com.hiveworkshop.wc3.gui.modeledit.newstuff.listener.EditabilityToggleHandler;

public final class MakeEditableAction implements UndoAction {
	private final EditabilityToggleHandler editabilityToggleHandler;

	public MakeEditableAction(final EditabilityToggleHandler editabilityToggleHandler) {
		this.editabilityToggleHandler = editabilityToggleHandler;
	}

	@Override
	public void undo() {
		editabilityToggleHandler.makeNotEditable();
	}

	@Override
	public void redo() {
		editabilityToggleHandler.makeEditable();
	}

	@Override
	public String actionName() {
		return "toggle visibility";
	}

}
