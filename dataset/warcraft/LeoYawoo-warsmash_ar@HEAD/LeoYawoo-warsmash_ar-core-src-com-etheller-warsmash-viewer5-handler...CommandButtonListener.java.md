---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/rendersim/commandbuttons/CommandButtonListener.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.rendersim.commandbuttons;

import com.badlogic.gdx.graphics.Texture;

public interface CommandButtonListener {
    void commandButton(int buttonPositionX, int buttonPositionY, Texture icon, int abilityHandleId, int orderId,
                       int autoCastOrderId, boolean active, boolean autoCastActive, boolean menuButton, String tip, String uberTip,
                       char hotkey, int goldCost, int lumberCost, int foodCost, int manaCost, float cooldownRemaining,
                       float cooldownMax, int numberOverlay);
}
