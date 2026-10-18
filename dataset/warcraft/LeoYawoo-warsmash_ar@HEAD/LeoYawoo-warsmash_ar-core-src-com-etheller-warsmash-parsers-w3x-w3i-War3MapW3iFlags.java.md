---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/parsers/w3x/w3i/War3MapW3iFlags.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.w3x.w3i;

public class War3MapW3iFlags {
    // definitions for these are based on what was found online located at:
    // wc3maps.com/InsideTheW3M.html
    public static final int HIDE_MINIMAP_IN_PREVIEW_SCREENS = 0x0001;
    public static final int MELEE_MAP = 0x0004;
    public static final int FIXED_PLAYER_SETTINGS_FOR_CUSTOM_FORCES = 0x0020;
    public static final int SHOW_WATER_WAVES_ON_CLIFF_SHORES = 0x0800;
    public static final int SHOW_WATER_WAVES_ON_ROLLING_SHORES = 0x1000;
}
