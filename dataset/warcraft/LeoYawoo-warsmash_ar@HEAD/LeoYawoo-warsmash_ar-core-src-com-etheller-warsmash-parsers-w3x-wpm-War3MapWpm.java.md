---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/parsers/w3x/wpm/War3MapWpm.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.w3x.wpm;

import com.etheller.warsmash.util.ParseUtils;
import com.etheller.warsmash.util.War3ID;
import com.google.common.io.LittleEndianDataInputStream;

import java.io.IOException;

public class War3MapWpm {
    private static final War3ID MAGIC_NUMBER = War3ID.fromString("MP3W");
    private final int[] size = new int[2];
    private int version;
    private short[] pathing;

    public War3MapWpm(final LittleEndianDataInputStream stream) throws IOException {
        if (stream != null) {
            this.load(stream);
        }
    }

    private void load(final LittleEndianDataInputStream stream) throws IOException {
        final War3ID firstId = ParseUtils.readWar3ID(stream);
        if (!MAGIC_NUMBER.equals(firstId)) {
            return;
        }

        this.version = stream.readInt();
        ParseUtils.readInt32Array(stream, this.size);
        this.pathing = ParseUtils.readUInt8Array(stream, this.size[0] * this.size[1]);

    }

    public int getVersion() {
        return this.version;
    }

    public void setVersion(final int version) {
        this.version = version;
    }

    public int[] getSize() {
        return this.size;
    }

    public short[] getPathing() {
        return this.pathing;
    }

    public void setPathing(final short[] pathing) {
        this.pathing = pathing;
    }

}
