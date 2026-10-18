---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/modeledit/ActiveViewportWatcher.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.modeledit;

public class ActiveViewportWatcher implements ViewportListener {
	private Viewport viewport;

	@Override
	public void viewportChanged(final Viewport viewport) {
		this.viewport = viewport;

	}

	public Viewport getViewport() {
		return viewport;
	}
}
