---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/components/ComponentPanel.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.components;

import com.hiveworkshop.wc3.gui.modeledit.actions.newsys.ModelStructureChangeListener;
import com.hiveworkshop.wc3.gui.modeledit.activity.UndoActionListener;
import com.hiveworkshop.wc3.mdl.EditableModel;

public interface ComponentPanel {
	void save(EditableModel model, UndoActionListener undoListener, ModelStructureChangeListener changeListener);
}
