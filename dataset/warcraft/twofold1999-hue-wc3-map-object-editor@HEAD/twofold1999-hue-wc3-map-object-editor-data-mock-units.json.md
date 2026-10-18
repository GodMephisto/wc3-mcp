---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "data/mock-units.json"
ref: HEAD
---

{
  "meta": {
    "source": "local-json",
    "version": "1.0"
  },
  "units": [
    {
      "id": "hfoo",
      "name": "Footman",
      "combatType": "Melee",
      "race": "Human",
      "hitPoints": 961,
      "attackType": "Normal",
      "armorType": "Heavy"
    },
    {
      "id": "hrif",
      "name": "Rifleman",
      "combatType": "Ranged",
      "race": "Human",
      "hitPoints": 505,
      "attackType": "Pierce",
      "armorType": "Medium"
    },
    {
      "id": "ogru",
      "name": "Grunt",
      "combatType": "Melee",
      "race": "Orc",
      "hitPoints": 700,
      "attackType": "Normal",
      "armorType": "Heavy"
    }
  ],
  "abilities": [
    {
      "id": "AHhb",
      "name": "Holy Bolt",
      "manaCost": 65,
      "cooldown": 6,
      "castRange": 800
    },
    {
      "id": "AOsh",
      "name": "Shockwave",
      "manaCost": 100,
      "cooldown": 9,
      "castRange": 700
    }
  ],
  "items": [
    {
      "id": "I001",
      "name": "Healing Salve",
      "goldCost": 100,
      "cooldown": 0,
      "description": "Regenerates hit points over time."
    },
    {
      "id": "I002",
      "name": "Scroll of Speed",
      "goldCost": 50,
      "cooldown": 60,
      "description": "Increases movement speed for nearby units."
    }
  ]
}
