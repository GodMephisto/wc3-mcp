---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/editors/ability-editor.ts"
ref: HEAD
---

import { AbilityDetails } from "../models/ability.model";

export type EditableAbilityField = "name" | "manaCost" | "cooldown" | "castRange";

export const EDITABLE_ABILITY_FIELDS: EditableAbilityField[] = [
  "name",
  "manaCost",
  "cooldown",
  "castRange",
];

export interface AbilityFieldUpdate {
  field: EditableAbilityField;
  oldValue: string | number;
  newValue: string | number;
}

export class AbilityEditor {
  public setField(
    ability: AbilityDetails,
    field: EditableAbilityField,
    value: string
  ): AbilityFieldUpdate {
    const oldValue = ability[field];

    if (field === "manaCost" || field === "cooldown" || field === "castRange") {
      const parsedValue = Number(value);

      if (!Number.isFinite(parsedValue) || parsedValue < 0) {
        throw new Error(`${field} must be a non-negative number.`);
      }

      ability[field] = parsedValue;
      return { field, oldValue, newValue: ability[field] };
    }

    ability[field] = value;
    return { field, oldValue, newValue: ability[field] };
  }
}
