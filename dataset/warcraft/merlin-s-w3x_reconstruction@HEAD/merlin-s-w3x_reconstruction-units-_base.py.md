---
type: vendor-doc
source_repo: "https://github.com/merlin-s/w3x_reconstruction"
source_path: "units/_base.py"
ref: HEAD
---

from typing import List


class _Base:
    def __init__(self, type_id, x, y, owner, color=-1):
        self.type_id = type_id
        self.variation: int = 0
        self.x: float = x
        self.y: float = y
        self.z: float = 0.0
        self.rotation: float = 0.0
        self.x_scale: float = 1.0
        self.y_scale: float = 1.0
        self.z_scale: float = 1.0
        # byte: flags*
        self.owner: int = owner
        # byte: unknown (0)
        # byte: unknown (0)
        self.hp: int = -1
        self.mp: int = 0
        self.item_table_pointer: int = -1
        self.dropped_item_sets: List = []
        self.gold_amount: int = 12500
        self.target_acquisition: float = -1  # -1 = normal, -2 = camp
        self.hero_level: int = 1
        self.strength: int = 0
        self.agility: int = 0
        self.intelligence: int = 0
        self.items: List = []
        self.modified_abilities: List = []
        self.color = color
        self.way_gate: int = -1
        self.creation_number: int = 0
        """
            int: random unit/item flag "r" (for uDNR units and iDNR items)
                0 = Any neutral passive building/item, in this case we have
                  byte[3]: level of the random unit/item,-1 = any (this is actually interpreted as a 24-bit number)
                  byte: item class of the random item, 0 = any, 1 = permanent ... (this is 0 for units)
                  r is also 0 for non random units/items so we have these 4 bytes anyway (even if the id wasn't uDNR or iDNR)
                1 = random unit from random group (defined in the w3i), in this case we have
                  int: unit group number (which group from the global table)
                  int: position number (which column of this group)
                  the column should of course have the item flag set (in the w3i) if this is a random item
                2 = random unit from custom table, in this case we have
                  int: number "n" of different available units
                  then we have n times a random unit structure
        """


Unit = _Base
Item = _Base
