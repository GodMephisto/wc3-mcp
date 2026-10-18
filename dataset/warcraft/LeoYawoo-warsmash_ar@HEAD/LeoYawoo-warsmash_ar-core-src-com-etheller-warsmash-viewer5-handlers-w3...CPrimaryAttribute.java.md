---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/simulation/abilities/hero/CPrimaryAttribute.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.simulation.abilities.hero;

public enum CPrimaryAttribute {
    STRENGTH,
    INTELLIGENCE,
    AGILITY;

    public static CPrimaryAttribute parsePrimaryAttribute(final String targetTypeString) {
        if (targetTypeString == null) {
            return STRENGTH;
        }
        switch (targetTypeString.toUpperCase()) {
            case "INT":
                return INTELLIGENCE;
            case "AGI":
                return AGILITY;
            case "STR":
            default:
                return STRENGTH;
        }
    }
}
