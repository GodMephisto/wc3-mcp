---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/animedit/TimeSliderTimeListener.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.animedit;

import com.etheller.util.SubscriberSetNotifier;

public interface TimeSliderTimeListener {
	void timeChanged(int currentTime);

	public static final class TimeSliderTimeNotifier extends SubscriberSetNotifier<TimeSliderTimeListener>
			implements TimeSliderTimeListener {
		@Override
		public void timeChanged(final int currentTime) {
			for (final TimeSliderTimeListener listener : set) {
				listener.timeChanged(currentTime);
			}
		}
	}
}
