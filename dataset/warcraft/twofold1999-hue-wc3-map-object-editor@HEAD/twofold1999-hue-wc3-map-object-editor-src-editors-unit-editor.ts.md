---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/editors/unit-editor.ts"
ref: HEAD
---

import { UnitDetails } from "../models/unit.model";

export type EditableUnitField =
  | "name"
  | "race"
  | "hitPoints"
  | "attackType"
  | "armorType";

export const EDITABLE_UNIT_FIELDS: EditableUnitField[] = [
  "name",
  "race",
  "hitPoints",
  "attackType",
  "armorType",
];

export interface UnitFieldUpdate {
  field: EditableUnitField;
  oldValue: string | number;
  newValue: string | number;
}

export class UnitEditor {
  public setField(
    unit: UnitDetails,
    field: EditableUnitField,
    value: string
  ): UnitFieldUpdate {
    const oldValue = unit[field];

    if (field === "hitPoints") {
      const parsedValue = Number(value);

      if (!Number.isFinite(parsedValue) || parsedValue < 0) {
        throw new Error("hitPoints must be a non-negative number.");
      }

      unit.hitPoints = parsedValue;
      return { field, oldValue, newValue: unit.hitPoints };
    }

    unit[field] = value;
    return { field, oldValue, newValue: unit[field] };
  }
}
