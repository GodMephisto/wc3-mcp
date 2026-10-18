---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/activity/UndoActionListener.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.activity;

import com.hiveworkshop.wc3.gui.modeledit.UndoAction;

public interface UndoActionListener {
	void pushAction(UndoAction action);
}
