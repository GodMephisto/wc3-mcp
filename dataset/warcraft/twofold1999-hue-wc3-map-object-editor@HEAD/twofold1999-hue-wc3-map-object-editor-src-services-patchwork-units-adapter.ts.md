---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/patchwork-units-adapter.ts"
ref: HEAD
---

import { UnitDetails } from "../models/unit.model";
import { StringResolverService } from "./string-resolver.service";

interface PatchworkModification {
  id?: unknown;
  value?: unknown;
}

interface PatchworkObjectFile {
  custom?: Record<string, PatchworkModification[]>;
}

export class PatchworkUnitsAdapter {
  public constructor(
    private readonly stringResolver = new StringResolverService()
  ) {}

  public mapToUnits(input: unknown): UnitDetails[] {
    const custom = this.readCustom(input);
    const units: UnitDetails[] = [];

    for (const [compositeId, modifications] of Object.entries(custom)) {
      const { id, baseId } = this.readObjectIds(compositeId);
      const unit = this.mapSingleUnit(id, baseId, modifications);
      units.push(unit);
    }

    return units;
  }

  private mapSingleUnit(
    unitId: string,
    baseId: string | undefined,
    modifications: PatchworkModification[]
  ): UnitDetails {
    const fieldValues = this.collectFieldValues(modifications);

    return {
      id: unitId,
      baseId,
      name: this.stringResolver.resolve(
        this.readString(fieldValues.get("unam"), unitId)
      ),
      combatType: "Unknown",
      race: "Unknown",
      hitPoints: this.readNumber(fieldValues.get("uhpm"), 0),
      moveSpeed: this.readOptionalNumber(fieldValues.get("umvs")),
      goldCost: this.readOptionalNumber(fieldValues.get("ugol")),
      armor: this.readOptionalNumber(fieldValues.get("udef")),
      acquisitionRange: this.readOptionalNumber(fieldValues.get("uacq")),
      hitPointRegen: this.readOptionalNumber(fieldValues.get("uhpr")),
      attackBaseDamage: this.readOptionalNumber(fieldValues.get("ua1b")),
      attackEnabled: this.readOptionalNumber(fieldValues.get("uaen")),
      secondaryAttackBase: this.readOptionalNumber(fieldValues.get("ua1z")),
      primaryAttackRange: this.readOptionalNumber(fieldValues.get("ua1r")),
      secondaryAttackRange: this.readOptionalNumber(fieldValues.get("ua2r")),
      primaryAttackSpeed: this.readOptionalNumber(fieldValues.get("usr1")),
      secondaryAttackSpeed: this.readOptionalNumber(fieldValues.get("usr2")),
      primaryWeaponTypeText: this.readOptionalString(fieldValues.get("ua1w")),
      secondaryWeaponTypeText: this.readOptionalString(fieldValues.get("ua2w")),
      secondaryAttackTypeText: this.readOptionalString(fieldValues.get("ua2t")),
      primaryTargetFlags: this.readOptionalStringList(fieldValues.get("ua1g")),
      primaryTargetPreferences: this.readOptionalStringList(
        fieldValues.get("ua1p")
      ),
      turnRate: this.readOptionalNumber(fieldValues.get("utc1")),
      secondaryTurnRate: this.readOptionalNumber(fieldValues.get("utc2")),
      minimumAttackRange: this.readOptionalNumber(fieldValues.get("uamn")),
      primaryDiceSides: this.readOptionalNumber(fieldValues.get("usd1")),
      secondaryDiceSides: this.readOptionalNumber(fieldValues.get("usd2")),
      lumberCost: this.readOptionalNumber(fieldValues.get("ulum")),
      foodCost: this.readOptionalNumber(fieldValues.get("umpi")),
      movementPriority: this.readOptionalNumber(fieldValues.get("umpr")),
      attackType: "Unknown",
      armorType: "Unknown",
    };
  }

  private readCustom(input: unknown): Record<string, PatchworkModification[]> {
    if (!input || typeof input !== "object" || Array.isArray(input)) {
      throw new Error("Invalid Patchwork units JSON: expected root object.");
    }

    const file = input as PatchworkObjectFile;
    if (!file.custom || typeof file.custom !== "object" || Array.isArray(file.custom)) {
      return {};
    }

    return file.custom;
  }

  private readObjectIds(
    compositeId: string
  ): { id: string; baseId: string | undefined } {
    const separatorIndex = compositeId.indexOf(":");
    if (separatorIndex === -1) {
      return { id: compositeId, baseId: undefined };
    }

    const id = compositeId.slice(0, separatorIndex);
    const baseId = compositeId.slice(separatorIndex + 1);
    return {
      id: id.length > 0 ? id : compositeId,
      baseId: baseId.length > 0 ? baseId : undefined,
    };
  }

  private collectFieldValues(
    modifications: PatchworkModification[]
  ): Map<string, unknown> {
    const fields = new Map<string, unknown>();

    for (const modification of modifications) {
      const fieldId = modification?.id;
      if (typeof fieldId !== "string") {
        continue;
      }

      fields.set(fieldId, modification.value);
    }

    return fields;
  }

  private readString(value: unknown, fallback: string): string {
    return typeof value === "string" ? value : fallback;
  }

  private readNumber(value: unknown, fallback: number): number {
    if (typeof value === "number" && Number.isFinite(value)) {
      return value;
    }

    return fallback;
  }

  private readOptionalNumber(value: unknown): number | undefined {
    if (typeof value === "number" && Number.isFinite(value)) {
      return value;
    }

    return undefined;
  }

  private readOptionalString(value: unknown): string | undefined {
    return typeof value === "string" ? value : undefined;
  }

  private readOptionalStringList(value: unknown): string[] | undefined {
    if (typeof value !== "string") {
      return undefined;
    }

    const items = value
      .split(",")
      .map((item) => item.trim())
      .filter((item) => item.length > 0);

    return items.length > 0 ? items : undefined;
  }
}
