---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/newstuff/actions/util/DoNothingMoveActionAdapter.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.newstuff.actions.util;

import com.hiveworkshop.wc3.gui.modeledit.UndoAction;

public class DoNothingMoveActionAdapter implements GenericMoveAction {

	private final UndoAction delegate;

	public DoNothingMoveActionAdapter(final UndoAction delegate) {
		this.delegate = delegate;
	}

	@Override
	public void undo() {
		delegate.undo();
	}

	@Override
	public void redo() {
		delegate.redo();
	}

	@Override
	public String actionName() {
		return delegate.actionName();
	}

	@Override
	public void updateTranslation(final double deltaX, final double deltaY, final double deltaZ) {
	}

}
