---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/activity/ActivityDescriptor.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.activity;

import com.hiveworkshop.wc3.gui.modeledit.newstuff.ModelEditorManager;
import com.hiveworkshop.wc3.mdl.v2.ModelView;

public interface ActivityDescriptor {
	ModelEditorViewportActivity createActivity(ModelEditorManager modelEditorManager, ModelView modelView,
			UndoActionListener undoActionListener);
}
