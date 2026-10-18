---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/newstuff/actions/util/ReversedAction.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.newstuff.actions.util;

import com.hiveworkshop.wc3.gui.modeledit.UndoAction;

public class ReversedAction implements UndoAction {
	private final UndoAction delegate;
	private final String actionName;

	public ReversedAction(final String actionName, final UndoAction delegate) {
		this.actionName = actionName;
		this.delegate = delegate;
	}

	@Override
	public void undo() {
		delegate.redo();
	}

	@Override
	public void redo() {
		delegate.undo();
	}

	@Override
	public String actionName() {
		return actionName;
	}

}
