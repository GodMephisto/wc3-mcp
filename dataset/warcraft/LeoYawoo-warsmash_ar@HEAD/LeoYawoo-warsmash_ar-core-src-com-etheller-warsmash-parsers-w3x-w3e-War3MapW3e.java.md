---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/parsers/w3x/w3e/War3MapW3e.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.w3x.w3e;

import com.etheller.warsmash.util.ParseUtils;
import com.etheller.warsmash.util.War3ID;
import com.google.common.io.LittleEndianDataInputStream;
import com.google.common.io.LittleEndianDataOutputStream;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

/**
 * war3map.w3e - the environment file.
 */
public class War3MapW3e {
    private static final War3ID MAGIC_NUMBER = War3ID.fromString("W3E!");
    private final List<War3ID> groundTiles = new ArrayList<>();
    private final List<War3ID> cliffTiles = new ArrayList<>();
    private final int[] mapSize = new int[2];
    private final float[] centerOffset = new float[2];
    private int version;
    private char tileset = 'A';
    private int hasCustomTileset;
    private Corner[][] corners;

    public War3MapW3e(final LittleEndianDataInputStream stream) throws IOException {
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
        this.tileset = (char) stream.read();
        this.hasCustomTileset = stream.readInt();

        for (int i = 0, l = stream.readInt(); i < l; i++) {
            this.groundTiles.add(ParseUtils.readWar3ID(stream));
        }

        for (int i = 0, l = stream.readInt(); i < l; i++) {
            this.cliffTiles.add(ParseUtils.readWar3ID(stream));
        }

        ParseUtils.readInt32Array(stream, this.mapSize);
        ParseUtils.readFloatArray(stream, this.centerOffset);

        this.corners = new Corner[this.mapSize[1]][];
        for (int row = 0, rows = this.mapSize[1]; row < rows; row++) {
            this.corners[row] = new Corner[this.mapSize[0]];

            for (int column = 0, columns = this.mapSize[0]; column < columns; column++) {
                final Corner corner = new Corner();

                corner.load(stream);

                this.corners[row][column] = corner;
            }
        }

    }

    public void save(final LittleEndianDataOutputStream stream) throws IOException {
        ParseUtils.writeWar3ID(stream, MAGIC_NUMBER);
        stream.writeInt(this.version);
        stream.write(this.tileset);
        stream.writeInt(this.hasCustomTileset);
        ParseUtils.writeUInt32(stream, this.groundTiles.size());

        for (final War3ID groundTile : this.groundTiles) {
            ParseUtils.writeWar3ID(stream, groundTile);
        }

        ParseUtils.writeUInt32(stream, this.cliffTiles.size());

        for (final War3ID cliffTile : this.cliffTiles) {
            ParseUtils.writeWar3ID(stream, cliffTile);
        }

        ParseUtils.writeInt32Array(stream, this.mapSize);
        ParseUtils.writeFloatArray(stream, this.centerOffset);

        for (final Corner[] row : this.corners) {
            for (final Corner corner : row) {
                corner.save(stream);
            }
        }
    }

    public int getByteLength() {
        return 37 + (this.groundTiles.size() * 4) + (this.cliffTiles.size() * 4)
                + (this.mapSize[0] * this.mapSize[1] * 7);
    }

    public int getVersion() {
        return this.version;
    }

    public void setVersion(final int version) {
        this.version = version;
    }

    public char getTileset() {
        return this.tileset;
    }

    public List<War3ID> getGroundTiles() {
        return this.groundTiles;
    }

    public List<War3ID> getCliffTiles() {
        return this.cliffTiles;
    }

    public int[] getMapSize() {
        return this.mapSize;
    }

    public float[] getCenterOffset() {
        return this.centerOffset;
    }

    public Corner[][] getCorners() {
        return this.corners;
    }

}
