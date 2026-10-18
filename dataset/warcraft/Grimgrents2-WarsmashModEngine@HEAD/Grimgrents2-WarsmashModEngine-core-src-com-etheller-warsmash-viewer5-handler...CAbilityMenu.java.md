---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/simulation/abilities/menu/CAbilityMenu.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.simulation.abilities.menu;

import com.etheller.warsmash.viewer5.handlers.w3x.simulation.abilities.CAbility;

public interface CAbilityMenu extends CAbility {
	// the base order ID of the menu
	int getBaseOrderId();
}
