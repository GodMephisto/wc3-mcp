---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/parsers/w3x/objectdata/Warcraft3MapObjectData.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.w3x.objectdata;

import com.etheller.warsmash.datasources.DataSource;
import com.etheller.warsmash.units.DataTable;
import com.etheller.warsmash.units.StandardObjectData;
import com.etheller.warsmash.units.StandardObjectData.WarcraftData;
import com.etheller.warsmash.units.custom.ObjectDataChangeEntry;
import com.etheller.warsmash.units.custom.WTS;
import com.etheller.warsmash.units.custom.WTSFile;
import com.etheller.warsmash.units.custom.War3ObjectDataChangeset;
import com.etheller.warsmash.units.manager.MutableObjectData;
import com.etheller.warsmash.units.manager.MutableObjectData.WorldEditorDataType;
import com.etheller.warsmash.util.War3ID;
import com.etheller.warsmash.util.WorldEditStrings;
import com.google.common.io.LittleEndianDataInputStream;

import java.io.IOException;
import java.util.*;
import java.util.Map.Entry;

public final class Warcraft3MapObjectData {
    private final MutableObjectData units;
    private final MutableObjectData items;
    private final MutableObjectData destructibles;
    private final MutableObjectData doodads;
    private final MutableObjectData abilities;
    private final MutableObjectData buffs;
    private final MutableObjectData upgrades;
    private final WTS wts;

    public Warcraft3MapObjectData(final MutableObjectData units, final MutableObjectData items,
                                  final MutableObjectData destructibles, final MutableObjectData doodads, final MutableObjectData abilities,
                                  final MutableObjectData buffs, final MutableObjectData upgrades, final WTS wts) {
        this.units = units;
        this.items = items;
        this.destructibles = destructibles;
        this.doodads = doodads;
        this.abilities = abilities;
        this.buffs = buffs;
        this.upgrades = upgrades;
        this.wts = wts;
    }

    public static WTS loadWTS(final DataSource dataSource) throws IOException {
        return dataSource.has("war3map.wts") ? new WTSFile(dataSource.getResourceAsStream("war3map.wts"))
                : WTS.DO_NOTHING;
    }

    public static Warcraft3MapObjectData load(final DataSource dataSource, final boolean inlineWTS) throws IOException {
        final WTS wts = loadWTS(dataSource);
        return load(dataSource, inlineWTS, wts);
    }

    public static Warcraft3MapObjectData load(final DataSource dataSource, final boolean inlineWTS, final WTS wts)
            throws IOException {

        final StandardObjectData standardObjectData = new StandardObjectData(dataSource);
        final WarcraftData standardUnits = standardObjectData.getStandardUnits();
        final WarcraftData standardItems = standardObjectData.getStandardItems();
        final WarcraftData standardDoodads = standardObjectData.getStandardDoodads();
        final WarcraftData standardDestructables = standardObjectData.getStandardDestructables();
        final WarcraftData abilities = standardObjectData.getStandardAbilities();
        final WarcraftData standardAbilityBuffs = standardObjectData.getStandardAbilityBuffs();
        final WarcraftData standardUpgrades = standardObjectData.getStandardUpgrades();

        final DataTable standardUnitMeta = standardObjectData.getStandardUnitMeta();
        final DataTable standardDoodadMeta = standardObjectData.getStandardDoodadMeta();
        final DataTable standardDestructableMeta = standardObjectData.getStandardDestructableMeta();
        final DataTable abilityMeta = standardObjectData.getStandardAbilityMeta();
        final DataTable standardAbilityBuffMeta = standardObjectData.getStandardAbilityBuffMeta();
        final DataTable standardUpgradeMeta = standardObjectData.getStandardUpgradeMeta();

        final War3ObjectDataChangeset unitChangeset = new War3ObjectDataChangeset('u');
        final War3ObjectDataChangeset itemChangeset = new War3ObjectDataChangeset('t');
        final War3ObjectDataChangeset doodadChangeset = new War3ObjectDataChangeset('d');
        final War3ObjectDataChangeset destructableChangeset = new War3ObjectDataChangeset('b');
        final War3ObjectDataChangeset abilityChangeset = new War3ObjectDataChangeset('a');
        final War3ObjectDataChangeset buffChangeset = new War3ObjectDataChangeset('h');
        final War3ObjectDataChangeset upgradeChangeset = new War3ObjectDataChangeset('q');

        if (dataSource.has("war3map.w3u")) {
            unitChangeset.load(new LittleEndianDataInputStream(dataSource.getResourceAsStream("war3map.w3u")), wts,
                    inlineWTS);
            // push unit changes to items.... as a Reign of Chaos support...
            Iterator<Entry<War3ID, ObjectDataChangeEntry>> entryIterator = unitChangeset.getOriginal().iterator();
            while (entryIterator.hasNext()) {
                final Entry<War3ID, ObjectDataChangeEntry> entry = entryIterator.next();
                final String oldIdString = entry.getValue().getOldId().toString();
                if ((standardUnits.get(oldIdString) == null) && (standardItems.get(oldIdString) != null)) {
                    itemChangeset.getOriginal().put(entry.getKey(), entry.getValue());
                    entryIterator.remove();
                }
            }
            entryIterator = unitChangeset.getCustom().iterator();
            while (entryIterator.hasNext()) {
                final Entry<War3ID, ObjectDataChangeEntry> entry = entryIterator.next();
                final String oldIdString = entry.getValue().getOldId().toString();
                if ((standardUnits.get(oldIdString) == null) && (standardItems.get(oldIdString) != null)) {
                    itemChangeset.getCustom().put(entry.getKey(), entry.getValue());
                    entryIterator.remove();
                }
            }
        }
        if (dataSource.has("war3map.w3t")) {
            itemChangeset.load(new LittleEndianDataInputStream(dataSource.getResourceAsStream("war3map.w3t")), wts,
                    inlineWTS);
        }
        if (dataSource.has("war3map.w3d")) {
            doodadChangeset.load(new LittleEndianDataInputStream(dataSource.getResourceAsStream("war3map.w3d")), wts,
                    inlineWTS);
        }
        if (dataSource.has("war3map.w3b")) {
            destructableChangeset.load(new LittleEndianDataInputStream(dataSource.getResourceAsStream("war3map.w3b")),
                    wts, inlineWTS);
        }
        if (dataSource.has("war3map.w3a")) {
            abilityChangeset.load(new LittleEndianDataInputStream(dataSource.getResourceAsStream("war3map.w3a")), wts,
                    inlineWTS);
        }
        if (dataSource.has("war3map.w3h")) {
            buffChangeset.load(new LittleEndianDataInputStream(dataSource.getResourceAsStream("war3map.w3h")), wts,
                    inlineWTS);
        }
        if (dataSource.has("war3map.w3q")) {
            upgradeChangeset.load(new LittleEndianDataInputStream(dataSource.getResourceAsStream("war3map.w3q")), wts,
                    inlineWTS);
        }

        final WorldEditStrings worldEditStrings = standardObjectData.getWorldEditStrings();
        final MutableObjectData unitData = new MutableObjectData(worldEditStrings, WorldEditorDataType.UNITS,
                standardUnits, standardUnitMeta, unitChangeset);
        final MutableObjectData itemData = new MutableObjectData(worldEditStrings, WorldEditorDataType.ITEM,
                standardItems, standardUnitMeta, itemChangeset);
        final MutableObjectData doodadData = new MutableObjectData(worldEditStrings, WorldEditorDataType.DOODADS,
                standardDoodads, standardDoodadMeta, doodadChangeset);
        final MutableObjectData destructableData = new MutableObjectData(worldEditStrings,
                WorldEditorDataType.DESTRUCTIBLES, standardDestructables, standardDestructableMeta,
                destructableChangeset);
        final MutableObjectData abilityData = new MutableObjectData(worldEditStrings, WorldEditorDataType.ABILITIES,
                abilities, abilityMeta, abilityChangeset);
        final MutableObjectData buffData = new MutableObjectData(worldEditStrings, WorldEditorDataType.BUFFS_EFFECTS,
                standardAbilityBuffs, standardAbilityBuffMeta, buffChangeset);
        final MutableObjectData upgradeData = new MutableObjectData(worldEditStrings, WorldEditorDataType.UPGRADES,
                standardUpgrades, standardUpgradeMeta, upgradeChangeset);

        return new Warcraft3MapObjectData(unitData, itemData, destructableData, doodadData, abilityData, buffData,
                upgradeData, wts);
    }

    public MutableObjectData getUnits() {
        return this.units;
    }

    public MutableObjectData getItems() {
        return this.items;
    }

    public MutableObjectData getDestructibles() {
        return this.destructibles;
    }

    public MutableObjectData getDoodads() {
        return this.doodads;
    }

    public MutableObjectData getAbilities() {
        return this.abilities;
    }

    public MutableObjectData getBuffs() {
        return this.buffs;
    }

    public MutableObjectData getUpgrades() {
        return this.upgrades;
    }

    public WTS getWts() {
        return this.wts;
    }
}