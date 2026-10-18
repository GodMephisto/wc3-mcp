---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/parsers/w3x/w3i/Player.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.w3x.w3i;

import com.etheller.warsmash.util.ParseUtils;
import com.google.common.io.LittleEndianDataInputStream;
import com.google.common.io.LittleEndianDataOutputStream;

import java.io.IOException;

/**
 * A player.
 */
public class Player {
    private final float[] startLocation = new float[2];
    private int id;
    private int type;
    private int race;
    private int isFixedStartPosition;
    private String name;
    private long allyLowPriorities;
    private long allyHighPriorities;

    public void load(final LittleEndianDataInputStream stream) throws IOException {
        this.id = (int) ParseUtils.readUInt32(stream);
        this.type = stream.readInt();
        this.race = stream.readInt();
        this.isFixedStartPosition = stream.readInt();
        this.name = ParseUtils.readUntilNull(stream);
        ParseUtils.readFloatArray(stream, this.startLocation);
        this.allyLowPriorities = ParseUtils.readUInt32(stream);
        this.allyHighPriorities = ParseUtils.readUInt32(stream);
    }

    public void save(final LittleEndianDataOutputStream stream) throws IOException {
        ParseUtils.writeUInt32(stream, this.id);
        stream.writeInt(this.type);
        stream.writeInt(this.race);
        stream.writeInt(this.isFixedStartPosition);
        ParseUtils.writeWithNullTerminator(stream, this.name);
        ParseUtils.writeFloatArray(stream, this.startLocation);
        ParseUtils.writeUInt32(stream, this.allyLowPriorities);
        ParseUtils.writeUInt32(stream, this.allyHighPriorities);
    }

    public int getByteLength() {
        return 33 + this.name.length();
    }

    public int getId() {
        return this.id;
    }

    public int getType() {
        return this.type;
    }

    public int getRace() {
        return this.race;
    }

    public String getName() {
        return this.name;
    }

}
