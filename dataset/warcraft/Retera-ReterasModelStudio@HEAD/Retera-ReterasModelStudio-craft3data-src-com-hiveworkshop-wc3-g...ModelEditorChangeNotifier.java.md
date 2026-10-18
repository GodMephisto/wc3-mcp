---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/newstuff/listener/ModelEditorChangeNotifier.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.newstuff.listener;

import com.etheller.util.SubscriberSetNotifier;
import com.hiveworkshop.wc3.gui.modeledit.newstuff.ModelEditor;

public class ModelEditorChangeNotifier extends SubscriberSetNotifier<ModelEditorChangeListener>
		implements ModelEditorChangeListener {

	@Override
	public void modelEditorChanged(final ModelEditor newModelEditor) {
		for (final ModelEditorChangeListener listener : set) {
			listener.modelEditorChanged(newModelEditor);
		}
	}

}
