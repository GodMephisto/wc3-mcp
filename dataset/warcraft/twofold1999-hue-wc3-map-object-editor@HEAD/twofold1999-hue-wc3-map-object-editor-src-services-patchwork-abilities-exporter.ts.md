---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/patchwork-abilities-exporter.ts"
ref: HEAD
---

import { AbilityDetails } from "../models/ability.model";

interface PatchworkFieldModification {
  id: string;
  type: "string" | "int" | "unreal";
  level: number;
  column: number;
  value: string | number;
}

interface PatchworkObjectFile {
  original: Record<string, never>;
  custom: Record<string, PatchworkFieldModification[]>;
}

export class PatchworkAbilitiesExporter {
  public export(abilities: AbilityDetails[]): PatchworkObjectFile {
    const custom: Record<string, PatchworkFieldModification[]> = {};

    for (const ability of abilities) {
      const baseId = ability.baseId ?? ability.id;
      const compositeId = `${ability.id}:${baseId}`;
      const modifications: PatchworkFieldModification[] = [
        {
          id: "anam",
          type: "string",
          level: 0,
          column: 0,
          value: ability.name,
        },
        {
          id: "acdn",
          type: this.getNumberType(ability.cooldown),
          level: 1,
          column: 0,
          value: ability.cooldown,
        },
        {
          id: "aran",
          type: this.getNumberType(ability.castRange),
          level: 1,
          column: 0,
          value: ability.castRange,
        },
      ];

      if (ability.manaCostLevels && ability.manaCostLevels.length > 0) {
        for (let i = 0; i < ability.manaCostLevels.length; i += 1) {
          const value = ability.manaCostLevels[i];
          modifications.push({
            id: "amcs",
            type: this.getNumberType(value),
            level: i + 1,
            column: 0,
            value,
          });
        }
      } else {
        modifications.push({
          id: "amcs",
          type: this.getNumberType(ability.manaCost),
          level: 1,
          column: 0,
          value: ability.manaCost,
        });
      }

      if (ability.maxLevel !== undefined) {
        modifications.push({
          id: "alev",
          type: this.getNumberType(ability.maxLevel),
          level: 0,
          column: 0,
          value: ability.maxLevel,
        });
      }

      if (ability.levelValues) {
        for (let i = 0; i < ability.levelValues.length; i += 1) {
          const value = ability.levelValues[i];
          modifications.push({
            id: "mls1",
            type: this.getNumberType(value),
            level: i + 1,
            column: 1,
            value,
          });
        }
      }

      if (ability.inf1Values) {
        for (let i = 0; i < ability.inf1Values.length; i += 1) {
          const value = ability.inf1Values[i];
          modifications.push({
            id: "Inf1",
            type: this.getNumberType(value),
            level: i + 1,
            column: 1,
            value,
          });
        }
      }

      if (ability.inf2Value !== undefined) {
        modifications.push({
          id: "Inf2",
          type: this.getNumberType(ability.inf2Value),
          level: 1,
          column: 2,
          value: ability.inf2Value,
        });
      }

      if (ability.inf3Value !== undefined) {
        modifications.push({
          id: "Inf3",
          type: this.getNumberType(ability.inf3Value),
          level: 1,
          column: 3,
          value: ability.inf3Value,
        });
      }

      if (ability.areaValues) {
        for (let i = 0; i < ability.areaValues.length; i += 1) {
          const value = ability.areaValues[i];
          modifications.push({
            id: "aare",
            type: this.getNumberType(value),
            level: i + 1,
            column: 0,
            value,
          });
        }
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
