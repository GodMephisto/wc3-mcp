---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/simulation/CUnitClassification.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.simulation;

import java.util.HashMap;
import java.util.Map;

/**
 * We think in the original WC3 sourcecode, these are probably referred to as
 * "Unit Types", but the community turn of phrase "Unit Type" has come to refer
 * to what WC3 sourcecode calls "Unit Class", hence this is now named "Unit
 * Classification" instead of "Unit Type" or "Unit Class" to disambiguate. This
 * is consistent with the World Editor naming: "Stats - Unit Classification".
 */
public enum CUnitClassification {
    GIANT("giant", "GiantClass"),
    UNDEAD("undead", "UndeadClass"),
    SUMMONED("summoned"),
    MECHANICAL("mechanical", "MechanicalClass"),
    PEON("peon"),
    SAPPER("sapper"),
    TOWNHALL("townhall"),
    TREE("tree"),
    WARD("ward"),
    ANCIENT("ancient"),
    STANDON("standon"),
    NEURAL("neutral"),
    TAUREN("tauren", "TaurenClass");
    private static final Map<String, CUnitClassification> UNIT_EDITOR_KEY_TO_CLASSIFICATION = new HashMap<>();

    static {
        for (final CUnitClassification unitClassification : values()) {
            UNIT_EDITOR_KEY_TO_CLASSIFICATION.put(unitClassification.getUnitDataKey(), unitClassification);
        }
    }

    private final String localeKey;
    private final String unitDataKey;
    private String displayName;

    CUnitClassification(final String unitDataKey, final String localeKey) {
        this.unitDataKey = unitDataKey;
        this.localeKey = localeKey;
    }

    CUnitClassification(final String unitDataKey) {
        this.unitDataKey = unitDataKey;
        this.localeKey = null;
    }

    public static CUnitClassification parseUnitClassification(final String unitEditorKey) {
        return UNIT_EDITOR_KEY_TO_CLASSIFICATION.get(unitEditorKey.toLowerCase());
    }

    public String getUnitDataKey() {
        return this.unitDataKey;
    }

    public String getLocaleKey() {
        return this.localeKey;
    }

    public String getDisplayName() {
        return this.displayName;
    }

    public void setDisplayName(final String displayName) {
        this.displayName = displayName;
    }
}
