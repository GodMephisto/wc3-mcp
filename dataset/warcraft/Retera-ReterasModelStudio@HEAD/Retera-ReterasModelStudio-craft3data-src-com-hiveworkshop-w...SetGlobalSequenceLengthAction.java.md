---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/actions/componenttree/globalsequence/SetGlobalSequenceLengthAction.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.actions.componenttree.globalsequence;

import com.hiveworkshop.wc3.gui.modeledit.UndoAction;
import com.hiveworkshop.wc3.gui.modeledit.actions.newsys.ModelStructureChangeListener;
import com.hiveworkshop.wc3.mdl.EditableModel;

public class SetGlobalSequenceLengthAction implements UndoAction {
	private final EditableModel model;
	private final int globalSequenceId;
	private final Integer previousLength;
	private final Integer newLength;
	private final ModelStructureChangeListener structureChangeListener;

	public SetGlobalSequenceLengthAction(final EditableModel model, final int globalSequenceId, final Integer previousLength,
			final Integer newLength, final ModelStructureChangeListener structureChangeListener) {
		this.model = model;
		this.globalSequenceId = globalSequenceId;
		this.previousLength = previousLength;
		this.newLength = newLength;
		this.structureChangeListener = structureChangeListener;
	}

	@Override
	public void undo() {
		model.setGlobalSequenceLength(globalSequenceId, previousLength);
		structureChangeListener.globalSequenceLengthChanged(globalSequenceId, previousLength);
	}

	@Override
	public void redo() {
		model.setGlobalSequenceLength(globalSequenceId, newLength);
		structureChangeListener.globalSequenceLengthChanged(globalSequenceId, newLength);
	}

	@Override
	public String actionName() {
		return "change GlobalSequence length to " + newLength;
	}
}
