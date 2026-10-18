---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/animedit/TimeBoundChangeListener.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.animedit;

import com.etheller.util.SubscriberSetNotifier;

public interface TimeBoundChangeListener {
	void timeBoundsChanged(int start, int end);

	public static final class TimeBoundChangeNotifier extends SubscriberSetNotifier<TimeBoundChangeListener>
			implements TimeBoundChangeListener {
		@Override
		public void timeBoundsChanged(final int start, final int end) {
			for (final TimeBoundChangeListener listener : set) {
				listener.timeBoundsChanged(start, end);
			}
		}
	}
}
