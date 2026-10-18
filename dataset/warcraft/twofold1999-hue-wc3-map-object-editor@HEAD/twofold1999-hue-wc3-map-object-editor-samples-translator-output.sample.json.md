---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "samples/translator-output.sample.json"
ref: HEAD
---

{
  "map": {
    "name": "Sample WC3 Map"
  },
  "objects": {
    "units": [
      {
        "rawcode": "hfoo",
        "name": "Footman",
        "race": "Human",
        "combatType": "Melee",
        "hp": 420,
        "attack_type": "Normal",
        "armor_type": "Heavy"
      }
    ],
    "abilities": [
      {
        "rawcode": "AHhb",
        "name": "Holy Bolt",
        "mana_cost": 65,
        "cooldown": 6,
        "cast_range": 800
      }
    ],
    "items": [
      {
        "rawcode": "I001",
        "name": "Healing Salve",
        "gold_cost": 100,
        "cooldown": 0,
        "description": "Regenerates hit points over time."
      }
    ]
  }
}
