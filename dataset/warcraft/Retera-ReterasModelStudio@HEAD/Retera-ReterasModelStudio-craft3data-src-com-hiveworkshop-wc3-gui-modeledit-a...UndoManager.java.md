---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/activity/UndoManager.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.activity;

public interface UndoManager extends UndoActionListener {
	void undo();

	void redo();

	boolean isUndoListEmpty();

	boolean isRedoListEmpty();

	String getUndoText();

	String getRedoText();
}
