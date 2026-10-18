---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/ProgramPreferencesChangeListener.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui;

import com.etheller.util.SubscriberSetNotifier;

public interface ProgramPreferencesChangeListener {
	void preferencesChanged();

	public final class ProgramPreferencesChangeNotifier extends SubscriberSetNotifier<ProgramPreferencesChangeListener>
			implements ProgramPreferencesChangeListener {

		@Override
		public void preferencesChanged() {
			for (final ProgramPreferencesChangeListener listener : set) {
				listener.preferencesChanged();
			}
		}

	}
}