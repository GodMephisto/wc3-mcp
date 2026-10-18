---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/models/ability.model.ts"
ref: HEAD
---

export interface AbilitySummary {
  id: string;
  baseId?: string;
  name: string;
}

export interface AbilityDetails extends AbilitySummary {
  manaCost: number;
  manaCostLevels?: number[];
  cooldown: number;
  castRange: number;
  areaValues?: number[];
  maxLevel?: number;
  levelValues?: number[];
  inf1Values?: number[];
  inf2Value?: number;
  inf3Value?: number;
}
