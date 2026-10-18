---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/ReplaceableIds.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

import com.etheller.warsmash.util.WarsmashConstants;

import java.util.HashMap;
import java.util.Map;

public class ReplaceableIds {
    private static final Map<Long, String> ID_TO_STR = new HashMap<>();
    private static final Map<Long, String> REPLACEABLE_ID_TO_STR = new HashMap<>();

    static {
        for (int i = 0; i < WarsmashConstants.MAX_PLAYERS; i++) {
            ID_TO_STR.put((long) i, String.format("%2d", i).replace(' ', '0'));
        }
        REPLACEABLE_ID_TO_STR.put(1L, "TeamColor\\TeamColor");
        REPLACEABLE_ID_TO_STR.put(2L, "TeamGlow\\TeamGlow");
        REPLACEABLE_ID_TO_STR.put(11L, "Cliff\\Cliff0");
        REPLACEABLE_ID_TO_STR.put(21L, ""); // Used by all cursor models (HumanCursor, OrcCursor,
        // UndeadCursor, NightElfCursor)
        REPLACEABLE_ID_TO_STR.put(31L, "LordaeronTree\\LordaeronSummerTree");
        REPLACEABLE_ID_TO_STR.put(32L, "AshenvaleTree\\AshenTree");
        REPLACEABLE_ID_TO_STR.put(33L, "BarrensTree\\BarrensTree");
        REPLACEABLE_ID_TO_STR.put(34L, "NorthrendTree\\NorthTree");
        REPLACEABLE_ID_TO_STR.put(35L, "Mushroom\\MushroomTree");
        REPLACEABLE_ID_TO_STR.put(36L, "RuinsTree\\RuinsTree");
        REPLACEABLE_ID_TO_STR.put(37L, "OutlandMushroomTree\\MushroomTree");
    }

    public static void main(final String[] args) {
        System.out.println(ID_TO_STR);
    }

    public static String getIdString(final long replaceableId) {
        return ID_TO_STR.get(replaceableId);
    }

    public static String getPathString(final long replaceableId) {
        return REPLACEABLE_ID_TO_STR.get(replaceableId);
    }
}
