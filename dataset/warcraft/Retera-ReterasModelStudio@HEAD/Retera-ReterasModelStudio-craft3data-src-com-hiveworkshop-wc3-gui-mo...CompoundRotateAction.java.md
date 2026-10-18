---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/newstuff/actions/editor/CompoundRotateAction.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.newstuff.actions.editor;

import com.etheller.collections.ListView;
import com.hiveworkshop.wc3.gui.modeledit.UndoAction;
import com.hiveworkshop.wc3.gui.modeledit.newstuff.actions.util.GenericRotateAction;

public final class CompoundRotateAction implements GenericRotateAction {
	private final ListView<? extends GenericRotateAction> actions;
	private final String name;

	public CompoundRotateAction(final String name, final ListView<? extends GenericRotateAction> actions) {
		this.name = name;
		this.actions = actions;
	}

	@Override
	public void undo() {
		for (final UndoAction action : actions) {
			action.undo();
		}
	}

	@Override
	public void redo() {
		for (final UndoAction action : actions) {
			action.redo();
		}
	}

	@Override
	public String actionName() {
		return name;
	}

	@Override
	public GenericRotateAction updateRotation(final double radians) {
		for (final GenericRotateAction action : actions) {
			action.updateRotation(radians);
		}
		return this;
	}

}
