---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/parsers/w3x/w3i/RandomItemSet.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.w3x.w3i;

import com.etheller.warsmash.util.ParseUtils;
import com.google.common.io.LittleEndianDataInputStream;
import com.google.common.io.LittleEndianDataOutputStream;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

public class RandomItemSet {
    private final List<RandomItem> items = new ArrayList<>();

    public void load(final LittleEndianDataInputStream stream) throws IOException {
        for (long i = 0, l = ParseUtils.readUInt32(stream); i < l; i++) {
            final RandomItem item = new RandomItem();

            item.load(stream);

            this.items.add(item);
        }
    }

    public void save(final LittleEndianDataOutputStream stream) throws IOException {
        ParseUtils.writeUInt32(stream, this.items.size());

        for (final RandomItem item : this.items) {
            item.save(stream);
        }
    }

    public int getByteLength() {
        return 4 + (this.items.size() * 8);
    }

    public List<RandomItem> getItems() {
        return this.items;
    }
}
