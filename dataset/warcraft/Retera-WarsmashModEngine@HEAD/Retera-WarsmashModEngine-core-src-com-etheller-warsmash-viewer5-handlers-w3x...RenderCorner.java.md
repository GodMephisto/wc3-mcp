---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/environment/RenderCorner.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.environment;

import com.etheller.warsmash.parsers.w3x.w3e.Corner;

public class RenderCorner extends Corner {
	public boolean cliff;
	public boolean romp;
	public float rampAdjust;
	public float depth;
	public boolean hideCliff;

	public RenderCorner(final Corner corner) {
		super(corner);
	}

}
