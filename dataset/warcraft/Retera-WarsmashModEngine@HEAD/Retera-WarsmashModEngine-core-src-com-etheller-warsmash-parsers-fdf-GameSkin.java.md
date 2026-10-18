---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/parsers/fdf/GameSkin.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.fdf;

import com.etheller.warsmash.units.DataTable;
import com.etheller.warsmash.units.Element;

public class GameSkin {
	private final Element skin;
	private final DataTable skinsTable;

	public GameSkin(final Element skin, final DataTable skinsTable) {
		this.skin = skin;
		this.skinsTable = skinsTable;
	}

	public Element getSkin() {
		return this.skin;
	}

	public DataTable getSkinsTable() {
		return this.skinsTable;
	}
}
