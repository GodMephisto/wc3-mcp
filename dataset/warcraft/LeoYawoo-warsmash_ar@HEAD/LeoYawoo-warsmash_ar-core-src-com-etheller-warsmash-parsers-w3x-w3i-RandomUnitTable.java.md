---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/parsers/w3x/w3i/RandomUnitTable.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.w3x.w3i;

import com.etheller.warsmash.util.ParseUtils;
import com.google.common.io.LittleEndianDataInputStream;
import com.google.common.io.LittleEndianDataOutputStream;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

public class RandomUnitTable {
    private final List<RandomUnit> units = new ArrayList<>();
    private int id;
    private String name;
    private int positions;
    private int[] columnTypes;

    public void load(final LittleEndianDataInputStream stream) throws IOException {
        this.id = stream.readInt(); // TODO is this a War3ID?
        this.name = ParseUtils.readUntilNull(stream);
        this.positions = stream.readInt();
        this.columnTypes = ParseUtils.readInt32Array(stream, this.positions);

        for (long i = 0, l = ParseUtils.readUInt32(stream); i < l; i++) {
            final RandomUnit unit = new RandomUnit();

            unit.load(stream, this.positions);

            this.units.add(unit);
        }
    }

    public void save(final LittleEndianDataOutputStream stream) throws IOException {
        stream.writeInt(this.id);
        ParseUtils.writeWithNullTerminator(stream, this.name);
        stream.writeInt(this.positions);
        ParseUtils.writeInt32Array(stream, this.columnTypes);
        ParseUtils.writeUInt32(stream, this.units.size());

        for (final RandomUnit unit : this.units) {
            unit.save(stream);
        }
    }

    public int getByteLength() {
        return 13 + this.name.length() + (this.columnTypes.length * 4)
                + (this.units.size() * (4 + (4 * this.positions)));
    }
}
