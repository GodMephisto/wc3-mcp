---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/patchwork-abilities-adapter.ts"
ref: HEAD
---

import { AbilityDetails } from "../models/ability.model";
import { StringResolverService } from "./string-resolver.service";

interface PatchworkModification {
  id?: unknown;
  value?: unknown;
}

interface PatchworkObjectFile {
  custom?: Record<string, PatchworkModification[]>;
}

export class PatchworkAbilitiesAdapter {
  public constructor(
    private readonly stringResolver = new StringResolverService()
  ) {}

  public mapToAbilities(input: unknown): AbilityDetails[] {
    const custom = this.readCustom(input);
    const abilities: AbilityDetails[] = [];

    for (const [compositeId, modifications] of Object.entries(custom)) {
      const { id, baseId } = this.readObjectIds(compositeId);
      const ability = this.mapSingleAbility(id, baseId, modifications);
      abilities.push(ability);
    }

    return abilities;
  }

  private mapSingleAbility(
    abilityId: string,
    baseId: string | undefined,
    modifications: PatchworkModification[]
  ): AbilityDetails {
    const fieldValues = this.collectFieldValues(modifications);
    const levelValues = this.collectRepeatedNumbers(modifications, "mls1");
    const manaCostLevels = this.collectRepeatedNumbers(modifications, "amcs");
    const areaValues = this.collectRepeatedNumbers(modifications, "aare");
    const inf1Values = this.collectRepeatedNumbers(modifications, "Inf1");

    return {
      id: abilityId,
      baseId,
      name: this.stringResolver.resolve(
        this.readString(fieldValues.get("anam"), abilityId)
      ),
      manaCost: this.readNumber(fieldValues.get("amcs"), 0),
      manaCostLevels: manaCostLevels.length > 0 ? manaCostLevels : undefined,
      cooldown: this.readNumber(fieldValues.get("acdn"), 0),
      castRange: this.readNumber(fieldValues.get("aran"), 0),
      areaValues: areaValues.length > 0 ? areaValues : undefined,
      maxLevel: this.readOptionalNumber(fieldValues.get("alev")),
      levelValues: levelValues.length > 0 ? levelValues : undefined,
      inf1Values: inf1Values.length > 0 ? inf1Values : undefined,
      inf2Value: this.readOptionalNumber(fieldValues.get("Inf2")),
      inf3Value: this.readOptionalNumber(fieldValues.get("Inf3")),
    };
  }

  private readCustom(input: unknown): Record<string, PatchworkModification[]> {
    if (!input || typeof input !== "object" || Array.isArray(input)) {
      throw new Error("Invalid Patchwork abilities JSON: expected root object.");
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

  private collectRepeatedNumbers(
    modifications: PatchworkModification[],
    fieldId: string
  ): number[] {
    const values: number[] = [];

    for (const modification of modifications) {
      if (modification?.id !== fieldId) {
        continue;
      }

      if (
        typeof modification.value === "number" &&
        Number.isFinite(modification.value)
      ) {
        values.push(modification.value);
      }
    }

    return values;
  }
}
