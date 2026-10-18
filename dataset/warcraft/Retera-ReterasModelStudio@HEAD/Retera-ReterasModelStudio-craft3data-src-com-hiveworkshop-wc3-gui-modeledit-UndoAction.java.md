---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/UndoAction.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit;

/**
 * Write a description of class UndoAction here.
 * 
 * @author (your name)
 * @version (a version number or a date)
 */
public interface UndoAction {
	void undo();

	void redo();

	String actionName();
}
