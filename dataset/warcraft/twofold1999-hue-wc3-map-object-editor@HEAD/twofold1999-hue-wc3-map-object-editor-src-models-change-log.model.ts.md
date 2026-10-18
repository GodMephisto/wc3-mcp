---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/models/change-log.model.ts"
ref: HEAD
---

export interface ChangeLogEntry {
  objectType: "unit";
  objectId: string;
  field: string;
  oldValue: string | number;
  newValue: string | number;
  changedAt: string;
}
