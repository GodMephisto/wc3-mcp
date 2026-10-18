---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/parsers/fdf/frames/FramePointAssignment.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.fdf.frames;

import com.badlogic.gdx.utils.viewport.Viewport;
import com.etheller.warsmash.parsers.fdf.GameUI;

public interface FramePointAssignment {
	float getX(GameUI gameUI, Viewport uiViewport);

	float getY(GameUI gameUI, Viewport uiViewport);
}
