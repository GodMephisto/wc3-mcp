---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/patchwork-items-exporter.ts"
ref: HEAD
---

import { ItemDetails } from "../models/item.model";

interface PatchworkFieldModification {
  id: string;
  type: "string" | "int";
  level: 0 | 1;
  column: number;
  value: string | number;
}

interface PatchworkObjectFile {
  original: Record<string, never>;
  custom: Record<string, PatchworkFieldModification[]>;
}

export class PatchworkItemsExporter {
  public export(items: ItemDetails[]): PatchworkObjectFile {
    const custom: Record<string, PatchworkFieldModification[]> = {};

    for (const item of items) {
      const baseId = item.baseId ?? item.id;
      const compositeId = `${item.id}:${baseId}`;
      const modifications: PatchworkFieldModification[] = [
        {
          id: "unam",
          type: "string",
          level: 0,
          column: 0,
          value: item.name,
        },
        {
          id: "igol",
          type: "int",
          level: 0,
          column: 0,
          value: item.goldCost,
        },
        {
          id: "ides",
          type: "string",
          level: 0,
          column: 0,
          value: item.description,
        },
        {
          id: "ihtp",
          type: "int",
          level: 0,
          column: 0,
          value: item.cooldown,
        },
      ];

      if (item.priority !== undefined) {
        modifications.push({
          id: "ipri",
          type: "int",
          level: 0,
          column: 0,
          value: item.priority,
        });
      }
      if (item.strengthBonus !== undefined) {
        modifications.push({
          id: "istr",
          type: "int",
          level: 0,
          column: 0,
          value: item.strengthBonus,
        });
      }
      if (item.level !== undefined) {
        modifications.push({
          id: "ilev",
          type: "int",
          level: 0,
          column: 0,
          value: item.level,
        });
      }
      if (item.levelVariance !== undefined) {
        modifications.push({
          id: "ilvo",
          type: "int",
          level: 0,
          column: 0,
          value: item.levelVariance,
        });
      }
      if (item.tooltipText !== undefined) {
        modifications.push({
          id: "utub",
          type: "string",
          level: 0,
          column: 0,
          value: item.tooltipText,
        });
      }
      if (item.abilityId !== undefined) {
        modifications.push({
          id: "iabi",
          type: "string",
          level: 0,
          column: 0,
          value: item.abilityId,
        });
      }
      if (item.armorTypeText !== undefined) {
        modifications.push({
          id: "iarm",
          type: "string",
          level: 0,
          column: 0,
          value: item.armorTypeText,
        });
      }

      custom[compositeId] = modifications;
    }

    return {
      original: {},
      custom,
    };
  }
}
