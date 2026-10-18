---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/ability.service.ts"
ref: HEAD
---

import {
  AbilityEditor,
  AbilityFieldUpdate,
  EditableAbilityField,
} from "../editors/ability-editor";
import { AbilityDetails, AbilitySummary } from "../models/ability.model";
import { MapDataStorageService } from "./map-data-storage.service";

export class AbilityService {
  private readonly abilityEditor = new AbilityEditor();
  private readonly mapDataStorageService = new MapDataStorageService();

  public listAbilities(): AbilitySummary[] {
    const abilities = this.mapDataStorageService.loadAbilities();
    return abilities.map(({ id, name }) => ({ id, name }));
  }

  public getAbilityById(abilityId: string): AbilityDetails | undefined {
    const abilities = this.mapDataStorageService.loadAbilities();
    return abilities.find((ability) => ability.id === abilityId);
  }

  public setAbilityField(
    abilityId: string,
    field: EditableAbilityField,
    value: string
  ): AbilityFieldUpdate {
    const abilities = this.mapDataStorageService.loadAbilities();
    const ability = abilities.find((entry) => entry.id === abilityId);

    if (!ability) {
      throw new Error(`Ability not found: ${abilityId}`);
    }

    const update = this.abilityEditor.setField(ability, field, value);

    if (update.oldValue !== update.newValue) {
      this.mapDataStorageService.saveAbilities(abilities);
    }

    return update;
  }
}
