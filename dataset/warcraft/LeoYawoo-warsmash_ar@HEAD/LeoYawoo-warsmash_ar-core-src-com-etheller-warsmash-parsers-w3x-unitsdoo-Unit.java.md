---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/parsers/w3x/unitsdoo/Unit.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.w3x.unitsdoo;

import com.etheller.warsmash.parsers.w3x.w3i.War3MapW3i;
import com.etheller.warsmash.util.ParseUtils;
import com.etheller.warsmash.util.War3ID;
import com.google.common.io.LittleEndianDataInputStream;
import com.google.common.io.LittleEndianDataOutputStream;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

public class Unit {
    private final float[] location = new float[3];
    private final float[] scale = new float[3];
    private final List<DroppedItemSet> droppedItemSets = new ArrayList<>();
    private final List<InventoryItem> itemsInInventory = new ArrayList<>();
    private final List<ModifiedAbility> modifiedAbilities = new ArrayList<>();
    private final short[] level = new short[3];
    private final List<RandomUnit> randomUnitTables = new ArrayList<>();
    private War3ID id;
    private int variation;
    private float angle;
    private short flags;
    private int player;
    private int unknown;
    private int hitpoints = -1;
    private int mana = -1;
    /**
     * @since 8
     */
    private int droppedItemTable = 0;
    private int goldAmount;
    private float targetAcquisition;
    private int heroLevel;
    /**
     * @since 8
     */
    private int heroStrength;
    /**
     * @since 8
     */
    private int heroAgility;
    /**
     * @since 8
     */
    private int heroIntelligence;
    private int randomFlag;
    private short itemClass;
    private long unitGroup;
    private long positionInGroup;
    private int customTeamColor;
    private int waygate;
    private int creationNumber;

    public void load(final LittleEndianDataInputStream stream, final int version, final War3MapW3i mapInformation)
            throws IOException {
        this.id = ParseUtils.readWar3ID(stream);
        this.variation = stream.readInt();
        ParseUtils.readFloatArray(stream, this.location);
        this.angle = stream.readFloat();
        ParseUtils.readFloatArray(stream, this.scale);

        this.flags = ParseUtils.readUInt8(stream);
        this.player = stream.readInt();
        this.unknown = ParseUtils.readUInt16(stream);
        this.hitpoints = stream.readInt();
        this.mana = stream.readInt();

        if (version > 7) {
            this.droppedItemTable = stream.readInt();
        }

        for (int i = 0, l = stream.readInt(); i < l; i++) {
            final DroppedItemSet set = new DroppedItemSet();

            set.load(stream);

            this.droppedItemSets.add(set);
        }

        this.goldAmount = stream.readInt();
        this.targetAcquisition = stream.readFloat();
        this.heroLevel = stream.readInt();

        if (version > 7) {
            this.heroStrength = stream.readInt();
            this.heroAgility = stream.readInt();
            this.heroIntelligence = stream.readInt();
        }

        for (int i = 0, l = stream.readInt(); i < l; i++) {
            final InventoryItem item = new InventoryItem();

            item.load(stream);

            this.itemsInInventory.add(item);
        }

        for (int i = 0, l = stream.readInt(); i < l; i++) {
            final ModifiedAbility modifiedAbility = new ModifiedAbility();

            modifiedAbility.load(stream);

            this.modifiedAbilities.add(modifiedAbility);
        }

        this.randomFlag = stream.readInt();

        if (this.randomFlag == 0) {
            ParseUtils.readUInt8Array(stream, this.level);
            this.itemClass = ParseUtils.readUInt8(stream);
        } else if (this.randomFlag == 1) {
            this.unitGroup = ParseUtils.readUInt32(stream);
            this.positionInGroup = ParseUtils.readUInt32(stream);
        } else if (this.randomFlag == 2) {
            for (int i = 0, l = stream.readInt(); i < l; i++) {
                final RandomUnit randomUnit = new RandomUnit();

                randomUnit.load(stream);

                this.randomUnitTables.add(randomUnit);
            }
        }

        this.customTeamColor = stream.readInt();
        this.waygate = stream.readInt();
        this.creationNumber = stream.readInt();
    }

    public void save(final LittleEndianDataOutputStream stream, final int version) throws IOException {
        ParseUtils.writeWar3ID(stream, this.id);
        stream.writeInt(this.variation);
        ParseUtils.writeFloatArray(stream, this.location);
        stream.writeFloat(this.angle);
        ParseUtils.writeFloatArray(stream, this.scale);
        ParseUtils.writeUInt8(stream, this.flags);
        stream.writeInt(this.player);
        ParseUtils.writeUInt16(stream, this.unknown);
        stream.writeInt(this.hitpoints);
        stream.writeInt(this.mana);

        if (version > 7) {
            stream.writeInt(this.droppedItemTable);
        }

        stream.writeInt(this.droppedItemSets.size());

        for (final DroppedItemSet droppedItemSet : this.droppedItemSets) {
            droppedItemSet.save(stream);
        }

        stream.writeInt(this.goldAmount);
        stream.writeFloat(this.targetAcquisition);
        stream.writeInt(this.heroLevel);

        if (version > 7) {
            stream.writeInt(this.heroStrength);
            stream.writeInt(this.heroAgility);
            stream.writeInt(this.heroIntelligence);
        }

        stream.writeInt(this.itemsInInventory.size());

        for (final InventoryItem itemInInventory : this.itemsInInventory) {
            itemInInventory.save(stream);
        }

        stream.writeInt(this.modifiedAbilities.size());

        for (final ModifiedAbility modifiedAbility : this.modifiedAbilities) {
            modifiedAbility.save(stream);
        }

        stream.writeInt(this.randomFlag);

        if (this.randomFlag == 0) {
            ParseUtils.writeUInt8Array(stream, this.level);
            ParseUtils.writeUInt8(stream, this.itemClass);
        } else if (this.randomFlag == 1) {
            ParseUtils.writeUInt32(stream, this.unitGroup);
            ParseUtils.writeUInt32(stream, this.positionInGroup);
        } else if (this.randomFlag == 2) {
            stream.writeInt(this.randomUnitTables.size());

            for (final RandomUnit randomUnitTable : this.randomUnitTables) {
                randomUnitTable.save(stream);
            }
        }

        stream.writeInt(this.customTeamColor);
        stream.writeInt(this.waygate);
        stream.writeInt(this.creationNumber);
    }

    public int getByteLength(final int version) {
        int size = 91;

        if (version > 7) {
            size += 16;
        }

        for (final DroppedItemSet droppedItemSet : this.droppedItemSets) {
            size += droppedItemSet.getByteLength();
        }

        size += this.itemsInInventory.size() * 8;

        size += this.modifiedAbilities.size() * 12;

        if (this.randomFlag == 0) {
            size += 4;
        } else if (this.randomFlag == 1) {
            size += 8;
        } else if (this.randomFlag == 2) {
            size += 4 + (this.randomUnitTables.size() * 8);
        }

        return size;
    }

    public War3ID getId() {
        return this.id;
    }

    public void setId(final War3ID id) {
        this.id = id;
    }

    public int getVariation() {
        return this.variation;
    }

    public void setVariation(final int variation) {
        this.variation = variation;
    }

    public float[] getLocation() {
        return this.location;
    }

    public float getAngle() {
        return this.angle;
    }

    public void setAngle(final float angle) {
        this.angle = angle;
    }

    public float[] getScale() {
        return this.scale;
    }

    public short getFlags() {
        return this.flags;
    }

    public void setFlags(final short flags) {
        this.flags = flags;
    }

    public int getPlayer() {
        return this.player;
    }

    public void setPlayer(final int player) {
        this.player = player;
    }

    public int getUnknown() {
        return this.unknown;
    }

    public void setUnknown(final int unknown) {
        this.unknown = unknown;
    }

    public int getHitpoints() {
        return this.hitpoints;
    }

    public int getGoldAmount() {
        return this.goldAmount;
    }

    public short[] getLevel() {
        return this.level;
    }

    public int getCustomTeamColor() {
        return this.customTeamColor;
    }

}
