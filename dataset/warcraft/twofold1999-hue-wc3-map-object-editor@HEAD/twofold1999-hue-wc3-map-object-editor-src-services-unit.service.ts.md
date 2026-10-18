---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/unit.service.ts"
ref: HEAD
---

import { UnitDetails, UnitSummary } from "../models/unit.model";
import {
  EditableUnitField,
  UnitEditor,
  UnitFieldUpdate,
} from "../editors/unit-editor";
import { MapDataStorageService } from "./map-data-storage.service";

export class UnitService {
  private readonly unitEditor = new UnitEditor();
  private readonly mapDataStorageService = new MapDataStorageService();

  public listUnits(): UnitSummary[] {
    const units = this.mapDataStorageService.loadUnits();

    return units.map(({ id, name, combatType }) => ({
      id,
      name,
      combatType,
    }));
  }

  public getUnitById(unitId: string): UnitDetails | undefined {
    const units = this.mapDataStorageService.loadUnits();
    return units.find((unit) => unit.id === unitId);
  }

  public searchUnits(keyword: string): UnitSummary[] {
    const normalizedKeyword = keyword.toLowerCase();
    const units = this.mapDataStorageService.loadUnits();

    return units
      .filter((unit) => {
        return (
          unit.id.toLowerCase().includes(normalizedKeyword) ||
          unit.name.toLowerCase().includes(normalizedKeyword) ||
          unit.race.toLowerCase().includes(normalizedKeyword) ||
          unit.combatType.toLowerCase().includes(normalizedKeyword)
        );
      })
      .map(({ id, name, combatType }) => ({ id, name, combatType }));
  }

  public setUnitField(
    unitId: string,
    field: EditableUnitField,
    value: string
  ): UnitFieldUpdate {
    const units = this.mapDataStorageService.loadUnits();
    const unit = units.find((entry) => entry.id === unitId);

    if (!unit) {
      throw new Error(`Unit not found: ${unitId}`);
    }

    const update = this.unitEditor.setField(unit, field, value);

    if (update.oldValue !== update.newValue) {
      this.mapDataStorageService.saveUnits(units);
    }

    return update;
  }
}
