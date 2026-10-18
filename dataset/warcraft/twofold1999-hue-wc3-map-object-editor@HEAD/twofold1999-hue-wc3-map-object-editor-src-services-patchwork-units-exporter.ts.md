---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/patchwork-units-exporter.ts"
ref: HEAD
---

import { UnitDetails } from "../models/unit.model";

interface PatchworkFieldModification {
  id: string;
  type: "string" | "int" | "unreal";
  level: 0 | 1;
  column: number;
  value: string | number;
}

interface PatchworkObjectFile {
  original: Record<string, never>;
  custom: Record<string, PatchworkFieldModification[]>;
}

export class PatchworkUnitsExporter {
  public export(units: UnitDetails[]): PatchworkObjectFile {
    const custom: Record<string, PatchworkFieldModification[]> = {};

    for (const unit of units) {
      const baseId = unit.baseId ?? unit.id;
      const compositeId = `${unit.id}:${baseId}`;
      const modifications: PatchworkFieldModification[] = [
        {
          id: "unam",
          type: "string",
          level: 0,
          column: 0,
          value: unit.name,
        },
        {
          id: "uhpm",
          type: "int",
          level: 0,
          column: 0,
          value: unit.hitPoints,
        },
      ];

      if (unit.moveSpeed !== undefined) {
        modifications.push({
          id: "umvs",
          type: this.getNumberType(unit.moveSpeed),
          level: 0,
          column: 0,
          value: unit.moveSpeed,
        });
      }
      if (unit.goldCost !== undefined) {
        modifications.push({
          id: "ugol",
          type: this.getNumberType(unit.goldCost),
          level: 0,
          column: 0,
          value: unit.goldCost,
        });
      }
      if (unit.armor !== undefined) {
        modifications.push({
          id: "udef",
          type: this.getNumberType(unit.armor),
          level: 0,
          column: 0,
          value: unit.armor,
        });
      }
      if (unit.acquisitionRange !== undefined) {
        modifications.push({
          id: "uacq",
          type: this.getNumberType(unit.acquisitionRange),
          level: 0,
          column: 0,
          value: unit.acquisitionRange,
        });
      }
      if (unit.hitPointRegen !== undefined) {
        modifications.push({
          id: "uhpr",
          type: this.getNumberType(unit.hitPointRegen),
          level: 0,
          column: 0,
          value: unit.hitPointRegen,
        });
      }
      if (unit.attackBaseDamage !== undefined) {
        modifications.push({
          id: "ua1b",
          type: this.getNumberType(unit.attackBaseDamage),
          level: 0,
          column: 0,
          value: unit.attackBaseDamage,
        });
      }
      if (unit.attackEnabled !== undefined) {
        modifications.push({
          id: "uaen",
          type: this.getNumberType(unit.attackEnabled),
          level: 0,
          column: 0,
          value: unit.attackEnabled,
        });
      }
      if (unit.secondaryAttackBase !== undefined) {
        modifications.push({
          id: "ua1z",
          type: this.getNumberType(unit.secondaryAttackBase),
          level: 0,
          column: 0,
          value: unit.secondaryAttackBase,
        });
      }
      if (unit.primaryAttackRange !== undefined) {
        modifications.push({
          id: "ua1r",
          type: this.getNumberType(unit.primaryAttackRange),
          level: 0,
          column: 0,
          value: unit.primaryAttackRange,
        });
      }
      if (unit.secondaryAttackRange !== undefined) {
        modifications.push({
          id: "ua2r",
          type: this.getNumberType(unit.secondaryAttackRange),
          level: 0,
          column: 0,
          value: unit.secondaryAttackRange,
        });
      }
      if (unit.primaryAttackSpeed !== undefined) {
        modifications.push({
          id: "usr1",
          type: this.getNumberType(unit.primaryAttackSpeed),
          level: 0,
          column: 0,
          value: unit.primaryAttackSpeed,
        });
      }
      if (unit.secondaryAttackSpeed !== undefined) {
        modifications.push({
          id: "usr2",
          type: this.getNumberType(unit.secondaryAttackSpeed),
          level: 0,
          column: 0,
          value: unit.secondaryAttackSpeed,
        });
      }
      if (unit.primaryWeaponTypeText !== undefined) {
        modifications.push({
          id: "ua1w",
          type: "string",
          level: 0,
          column: 0,
          value: unit.primaryWeaponTypeText,
        });
      }
      if (unit.secondaryWeaponTypeText !== undefined) {
        modifications.push({
          id: "ua2w",
          type: "string",
          level: 0,
          column: 0,
          value: unit.secondaryWeaponTypeText,
        });
      }
      if (unit.secondaryAttackTypeText !== undefined) {
        modifications.push({
          id: "ua2t",
          type: "string",
          level: 0,
          column: 0,
          value: unit.secondaryAttackTypeText,
        });
      }
      if (unit.primaryTargetFlags && unit.primaryTargetFlags.length > 0) {
        modifications.push({
          id: "ua1g",
          type: "string",
          level: 0,
          column: 0,
          value: unit.primaryTargetFlags.join(","),
        });
      }
      if (
        unit.primaryTargetPreferences &&
        unit.primaryTargetPreferences.length > 0
      ) {
        modifications.push({
          id: "ua1p",
          type: "string",
          level: 0,
          column: 0,
          value: unit.primaryTargetPreferences.join(","),
        });
      }
      if (unit.turnRate !== undefined) {
        modifications.push({
          id: "utc1",
          type: this.getNumberType(unit.turnRate),
          level: 0,
          column: 0,
          value: unit.turnRate,
        });
      }
      if (unit.secondaryTurnRate !== undefined) {
        modifications.push({
          id: "utc2",
          type: this.getNumberType(unit.secondaryTurnRate),
          level: 0,
          column: 0,
          value: unit.secondaryTurnRate,
        });
      }
      if (unit.minimumAttackRange !== undefined) {
        modifications.push({
          id: "uamn",
          type: this.getNumberType(unit.minimumAttackRange),
          level: 0,
          column: 0,
          value: unit.minimumAttackRange,
        });
      }
      if (unit.primaryDiceSides !== undefined) {
        modifications.push({
          id: "usd1",
          type: this.getNumberType(unit.primaryDiceSides),
          level: 0,
          column: 0,
          value: unit.primaryDiceSides,
        });
      }
      if (unit.secondaryDiceSides !== undefined) {
        modifications.push({
          id: "usd2",
          type: this.getNumberType(unit.secondaryDiceSides),
          level: 0,
          column: 0,
          value: unit.secondaryDiceSides,
        });
      }
      if (unit.lumberCost !== undefined) {
        modifications.push({
          id: "ulum",
          type: this.getNumberType(unit.lumberCost),
          level: 0,
          column: 0,
          value: unit.lumberCost,
        });
      }
      if (unit.foodCost !== undefined) {
        modifications.push({
          id: "umpi",
          type: this.getNumberType(unit.foodCost),
          level: 0,
          column: 0,
          value: unit.foodCost,
        });
      }
      if (unit.movementPriority !== undefined) {
        modifications.push({
          id: "umpr",
          type: this.getNumberType(unit.movementPriority),
          level: 0,
          column: 0,
          value: unit.movementPriority,
        });
      }

      custom[compositeId] = modifications;
    }

    return {
      original: {},
      custom,
    };
  }

  private getNumberType(value: number): "int" | "unreal" {
    return Number.isInteger(value) ? "int" : "unreal";
  }
}
