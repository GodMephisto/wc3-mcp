---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/editors/object-editor.ts"
ref: HEAD
---

import { UnitSummary } from "../models/unit.model";

export class ObjectEditor {
  public rename(unit: UnitSummary, name: string): UnitSummary {
    return { ...unit, name };
  }
}
