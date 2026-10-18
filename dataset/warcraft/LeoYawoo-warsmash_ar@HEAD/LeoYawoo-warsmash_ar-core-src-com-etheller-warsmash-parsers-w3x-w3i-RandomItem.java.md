---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/parsers/w3x/w3i/RandomItem.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.w3x.w3i;

import com.etheller.warsmash.util.ParseUtils;
import com.etheller.warsmash.util.War3ID;
import com.google.common.io.LittleEndianDataInputStream;
import com.google.common.io.LittleEndianDataOutputStream;

import java.io.IOException;

public class RandomItem {
    private int chance;
    private War3ID id;

    public void load(final LittleEndianDataInputStream stream) throws IOException {
        this.chance = stream.readInt();
        this.id = ParseUtils.readWar3ID(stream);
    }

    public void save(final LittleEndianDataOutputStream stream) throws IOException {
        stream.writeInt(this.chance);
        ParseUtils.writeWar3ID(stream, this.id);
    }

    public War3ID getId() {
        return this.id;
    }

    public void setId(final War3ID id) {
        this.id = id;
    }
}
