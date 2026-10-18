---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/animedit/TimeBoundProvider.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.animedit;

public interface TimeBoundProvider extends BasicTimeBoundProvider {

	void addChangeListener(TimeBoundChangeListener listener);
}
