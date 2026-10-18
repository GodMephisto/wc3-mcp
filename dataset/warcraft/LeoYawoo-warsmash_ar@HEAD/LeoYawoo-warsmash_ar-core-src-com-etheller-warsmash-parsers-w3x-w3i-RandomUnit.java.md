---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/parsers/w3x/w3i/RandomUnit.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.w3x.w3i;

import com.etheller.warsmash.util.ParseUtils;
import com.etheller.warsmash.util.War3ID;
import com.google.common.io.LittleEndianDataInputStream;
import com.google.common.io.LittleEndianDataOutputStream;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

public class RandomUnit {
    private final List<War3ID> ids = new ArrayList<>();
    private int chance;

    public void load(final LittleEndianDataInputStream stream, final int positions) throws IOException {
        this.chance = stream.readInt();

        for (int i = 0; i < positions; i++) {
            this.ids.add(ParseUtils.readWar3ID(stream));
        }
    }

    public void save(final LittleEndianDataOutputStream stream) throws IOException {
        stream.writeInt(this.chance);

        for (final War3ID id : this.ids) {
            ParseUtils.writeWar3ID(stream, id);
        }
    }
}
