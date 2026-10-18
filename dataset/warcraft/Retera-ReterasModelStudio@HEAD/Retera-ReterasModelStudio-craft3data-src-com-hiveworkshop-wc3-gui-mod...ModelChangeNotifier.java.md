---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/actions/newsys/ModelChangeNotifier.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit.actions.newsys;

import java.util.HashSet;
import java.util.Set;

public final class ModelChangeNotifier implements ModelChangeListener {
	private final Set<ModelChangeListener> listeners;

	public ModelChangeNotifier() {
		listeners = new HashSet<>();
	}

	public void subscribe(final ModelChangeListener listener) {
		listeners.add(listener);
	}

	@Override
	public void modelChanged() {
		for (final ModelChangeListener listener : listeners) {
			listener.modelChanged();
		}
	}
}
