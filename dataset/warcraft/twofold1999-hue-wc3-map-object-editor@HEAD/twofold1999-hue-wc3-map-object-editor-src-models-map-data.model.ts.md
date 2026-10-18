---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/models/map-data.model.ts"
ref: HEAD
---

import { AbilityDetails } from "./ability.model";
import { ItemDetails } from "./item.model";
import { UnitDetails } from "./unit.model";

export interface MapDataMeta {
  source: string;
  version: string;
  mapName?: string;
}

export interface MapDataFile {
  meta: MapDataMeta;
  units: UnitDetails[];
  abilities?: AbilityDetails[];
  items?: ItemDetails[];
}
