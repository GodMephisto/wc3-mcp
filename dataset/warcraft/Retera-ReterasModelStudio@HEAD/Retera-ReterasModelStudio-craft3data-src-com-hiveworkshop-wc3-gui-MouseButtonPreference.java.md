---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/MouseButtonPreference.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui;

import java.awt.event.MouseEvent;

import javax.swing.SwingUtilities;

public enum MouseButtonPreference {
	RIGHT() {

		@Override
		public boolean isButton(final MouseEvent event) {
			return SwingUtilities.isRightMouseButton(event);
		}

	},
	LEFT() {

		@Override
		public boolean isButton(final MouseEvent event) {
			return SwingUtilities.isLeftMouseButton(event);
		}

	},
	MIDDLE() {

		@Override
		public boolean isButton(final MouseEvent event) {
			return SwingUtilities.isMiddleMouseButton(event);
		}

	};

	public abstract boolean isButton(MouseEvent event);
}
