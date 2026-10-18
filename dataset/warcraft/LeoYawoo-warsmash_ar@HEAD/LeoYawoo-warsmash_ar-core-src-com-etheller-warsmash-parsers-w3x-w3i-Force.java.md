---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/parsers/w3x/w3i/Force.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.w3x.w3i;

import com.etheller.warsmash.util.ParseUtils;
import com.google.common.io.LittleEndianDataInputStream;
import com.google.common.io.LittleEndianDataOutputStream;

import java.io.IOException;

public class Force {
    private long flags;
    private long playerMasks;
    private String name;

    public void load(final LittleEndianDataInputStream stream) throws IOException {
        this.flags = ParseUtils.readUInt32(stream);
        this.playerMasks = ParseUtils.readUInt32(stream);
        this.name = ParseUtils.readUntilNull(stream);
    }

    public void save(final LittleEndianDataOutputStream stream) throws IOException {
        ParseUtils.writeUInt32(stream, this.flags);
        ParseUtils.writeUInt32(stream, this.playerMasks);
        ParseUtils.writeWithNullTerminator(stream, this.name);
    }

    public long getFlags() {
        return this.flags;
    }

    public long getPlayerMasks() {
        return this.playerMasks;
    }

    public String getName() {
        return this.name;
    }

    public int getByteLength() {
        return 9 + this.name.length();
    }

}
