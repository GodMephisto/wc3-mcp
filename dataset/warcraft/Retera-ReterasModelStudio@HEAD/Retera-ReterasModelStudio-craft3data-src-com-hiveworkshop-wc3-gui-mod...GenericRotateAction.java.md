---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/newstuff/actions/util/GenericRotateAction.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.newstuff.actions.util;

import com.hiveworkshop.wc3.gui.modeledit.UndoAction;

public interface GenericRotateAction extends UndoAction {
	GenericRotateAction updateRotation(double radians);
}
