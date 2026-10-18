---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/models/item.model.ts"
ref: HEAD
---

export interface ItemSummary {
  id: string;
  baseId?: string;
  name: string;
}

export interface ItemDetails extends ItemSummary {
  goldCost: number;
  cooldown: number;
  description: string;
  tooltipText?: string;
  abilityId?: string;
  priority?: number;
  strengthBonus?: number;
  level?: number;
  levelVariance?: number;
  armorTypeText?: string;
}
