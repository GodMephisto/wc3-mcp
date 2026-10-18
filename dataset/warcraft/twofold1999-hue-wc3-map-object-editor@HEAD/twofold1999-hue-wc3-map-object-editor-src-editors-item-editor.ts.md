---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/editors/item-editor.ts"
ref: HEAD
---

import { ItemDetails } from "../models/item.model";

export type EditableItemField = "name" | "goldCost" | "cooldown" | "description";

export const EDITABLE_ITEM_FIELDS: EditableItemField[] = [
  "name",
  "goldCost",
  "cooldown",
  "description",
];

export interface ItemFieldUpdate {
  field: EditableItemField;
  oldValue: string | number;
  newValue: string | number;
}

export class ItemEditor {
  public setField(
    item: ItemDetails,
    field: EditableItemField,
    value: string
  ): ItemFieldUpdate {
    const oldValue = item[field];

    if (field === "goldCost" || field === "cooldown") {
      const parsedValue = Number(value);

      if (!Number.isFinite(parsedValue) || parsedValue < 0) {
        throw new Error(`${field} must be a non-negative number.`);
      }

      item[field] = parsedValue;
      return { field, oldValue, newValue: item[field] };
    }

    item[field] = value;
    return { field, oldValue, newValue: item[field] };
  }
}
