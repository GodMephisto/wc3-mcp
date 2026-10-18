---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/newstuff/actions/util/GenericMoveAction.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.newstuff.actions.util;

import com.hiveworkshop.wc3.gui.modeledit.UndoAction;

public interface GenericMoveAction extends UndoAction {
	void updateTranslation(double deltaX, double deltaY, double deltaZ);
}
