---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/patchwork-items-adapter.ts"
ref: HEAD
---

import { ItemDetails } from "../models/item.model";
import { StringResolverService } from "./string-resolver.service";

interface PatchworkModification {
  id?: unknown;
  value?: unknown;
}

interface PatchworkObjectFile {
  custom?: Record<string, PatchworkModification[]>;
}

export class PatchworkItemsAdapter {
  public constructor(
    private readonly stringResolver = new StringResolverService()
  ) {}

  public mapToItems(input: unknown): ItemDetails[] {
    const custom = this.readCustom(input);
    const items: ItemDetails[] = [];

    for (const [compositeId, modifications] of Object.entries(custom)) {
      const { id, baseId } = this.readObjectIds(compositeId);
      const item = this.mapSingleItem(id, baseId, modifications);
      items.push(item);
    }

    return items;
  }

  private mapSingleItem(
    itemId: string,
    baseId: string | undefined,
    modifications: PatchworkModification[]
  ): ItemDetails {
    const fieldValues = this.collectFieldValues(modifications);

    return {
      id: itemId,
      baseId,
      name: this.stringResolver.resolve(
        this.readString(fieldValues.get("unam"), itemId)
      ),
      goldCost: this.readNumber(fieldValues.get("igol"), 0),
      cooldown: this.readNumber(fieldValues.get("ihtp"), 0),
      description: this.stringResolver.resolve(
        this.readString(fieldValues.get("ides"), "")
      ),
      tooltipText: this.resolveOptionalString(fieldValues.get("utub")),
      abilityId: this.readOptionalString(fieldValues.get("iabi")),
      priority: this.readOptionalNumber(fieldValues.get("ipri")),
      strengthBonus: this.readOptionalNumber(fieldValues.get("istr")),
      level: this.readOptionalNumber(fieldValues.get("ilev")),
      levelVariance: this.readOptionalNumber(fieldValues.get("ilvo")),
      armorTypeText: this.readOptionalString(fieldValues.get("iarm")),
    };
  }

  private readCustom(input: unknown): Record<string, PatchworkModification[]> {
    if (!input || typeof input !== "object" || Array.isArray(input)) {
      throw new Error("Invalid Patchwork items JSON: expected root object.");
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

  private resolveOptionalString(value: unknown): string | undefined {
    if (typeof value !== "string") {
      return undefined;
    }

    return this.stringResolver.resolve(value);
  }
}
