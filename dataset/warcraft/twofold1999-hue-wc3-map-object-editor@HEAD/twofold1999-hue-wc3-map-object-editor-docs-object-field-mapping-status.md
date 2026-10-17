---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "docs/object-field-mapping-status.md"
ref: HEAD
---

# Patchwork Object Field Mapping Status

Current first-pass mapping status for Patchwork JSON (`war3map.w3u.json`, `war3map.w3a.json`, `war3map.w3t.json`) into internal models.

## Units

Mapped fields:

- `unam` -> `name`
- `uhpm` -> `hitPoints`
- `umvs` -> `moveSpeed`
- `ugol` -> `goldCost`
- `udef` -> `armor`
- `uacq` -> `acquisitionRange`
- `uhpr` -> `hitPointRegen`
- `ua1b` -> `attackBaseDamage`
- `uaen` -> `attackEnabled`
- `ua1z` -> `secondaryAttackBase`
- `utc1` -> `turnRate`
- `ulum` -> `lumberCost`
- `umpi` -> `foodCost`
- `umpr` -> `movementPriority`

Notes:

- `combatType`, `race`, `attackType`, and `armorType` are still default/placeholder values.

## Abilities

Mapped fields:

- `anam` -> `name`
- `amcs` -> `manaCost`
- `acdn` -> `cooldown`
- `aran` -> `castRange`
- `alev` -> `maxLevel`
- `mls1` -> `levelValues` (repeated multi-level numeric values, preserved in order)

## Items

Mapped fields:

- `unam` -> `name`
- `igol` -> `goldCost`
- `ides` -> `description`
- `ihtp` -> `cooldown`
- `ipri` -> `priority`
- `istr` -> `strengthBonus`
- `ilev` -> `level`
- `iarm` -> `armorTypeText`

## Pending Fields

- Items: `ilvo`
