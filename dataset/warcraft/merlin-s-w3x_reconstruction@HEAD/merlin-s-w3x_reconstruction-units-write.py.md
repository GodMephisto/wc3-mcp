---
type: vendor-doc
source_repo: "https://github.com/merlin-s/w3x_reconstruction"
source_path: "units/write.py"
ref: HEAD
---

from shared import Blob
from typing import List
from ._base import _Base


def _blob_with_header(count: int) -> Blob():
    output = Blob()
    output.append_char4("W3do")
    output.append_int(8)  # major version
    output.append_int(0xB)  # minor version (often set to [0B 00 00 00]h)
    output.append_int(count)
    return output


def serialize_as_blob(objects: List[_Base]) -> Blob:
    """
    Data:
        Each unit/item is defined by a block of bytes (variable length) organized like this:
        char[4]: type ID (iDNR = random item, uDNR = random unit)
        int: variation
        float: coordinate X
        float: coordinate Y
        float: coordinate Z
        float: rotation angle
        float: scale X
        float: scale Y
        float: scale Z
        byte: flags*
        int: player number (owner) (player1 = 0, 16=neutral passive)
        byte: unknown (0)
        byte: unknown (0)
        int: hit points (-1 = use default)
        int: mana points (-1 = use default, 0 = unit doesn't have mana)
        int: map item table pointer (for dropped items on death)
        if -1 => no item table used
        if >= 0 => the item table with this number will be dropped on death
        int: number "s" of dropped item sets (can only be greater 0 if the item table pointer was -1)
        then we have s times a dropped item sets structures (see below)
        int: gold amount (default = 12500)
        float: target acquisition (-1 = normal, -2 = camp)
        int: hero level (set to1 for non hero units and items)
        int: strength of the hero (0 = use default)
        int: agility of the hero (0 = use default)
        int: intelligence of the hero (0 = use default)
        int: number "n" of items in the inventory
        then there is n times a inventory item structure (see below)
        int: number "n" of modified abilities for this unit
        then there is n times a ability modification structure (see below)
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

        int: custom color (-1 = none, 0 = red, 1=blue,...)
        int: Waygate: active destination number (-1 = deactivated, else it's the creation number of the target rect as in war3map.w3r)
        int: creation number
        *flags: may be similar to the war3map.doo flags
    """
    b = _blob_with_header(count=len(objects))
    for o in objects:
        b.append_char4(o.type_id)
        b.append_int(o.variation)
        b.append_float(o.x)
        b.append_float(o.y)
        b.append_float(o.z)
        b.append_float(o.rotation)
        b.append_float(o.x_scale)
        b.append_float(o.y_scale)
        b.append_float(o.z_scale)
        b.append_byte(0)  # flags
        b.append_int(o.owner)
        b.append_byte(0)  # unknown
        b.append_byte(0)  # unknown
        b.append_int(o.hp)
        b.append_int(o.mp)
        assert o.item_table_pointer == -1 and not o.dropped_item_sets  # other stuff not supported
        b.append_int(o.item_table_pointer)
        b.append_int(o.gold_amount)
        b.append_float(o.target_acquisition)
        b.append_int(o.hero_level)
        b.append_int(o.strength)
        b.append_int(o.agility)
        b.append_int(o.intelligence)
        assert not o.items, "serializing inventories is not supported"
        b.append_int(len(o.items))
        assert not o.modified_abilities, "serializing modified abilities is not supported"
        b.append_int(len(o.modified_abilities))
        b.append_int(o.color)
        b.append_int(o.way_gate)
        b.append_int(o.creation_number)
    return b


"""

Dropped item set format
int: number "d" of dropable items
"d" times dropable items structures:
char[4]: item ID ([00 00 00 00]h = none)
this can also be a random item id (see below)
int: % chance to be dropped

Inventory item format
int: inventory slot (this is the actual slot - 1, so 1 => 0)
char[4]: item id (as in ItemData.slk) 0x00000000 = none
this can also be a random item id (see below)

Ability modification format
char[4]: ability id (as in AbilityData.slk)
int: active for autocast abilities, 0 = no, 1 = active
int: level for hero abilities

Random unit format
char[4]: unit id (as in UnitUI.slk)
this can also be a random unit id (see below)
int: percentual chance of choice

Random item ids
random item ids are of the type char[4] where the 1st letter is "Y" and the 3rd letter is "I"
the 2nd letter narrows it down to items of a certain item types
"Y" = any type
"i" to "o" = item of this type, the letters are in order of the item types in the dropdown box ("i" = charged)
the 4th letter narrows it down to items of a certain level
"/" = any level (ASCII 47)
"0" ... = specific level (this is ASCII 48 + level, so level 10 will be ":" and level 15 will be "?" and so on)

Random unit ids
random unit ids are of the type char[4] where the 1st three letters are "YYU"
the 4th letter narrows it down to units of a certain level
"/" = any level (ASCII 47)
"0" ... = specific level (this is ASCII 48 + level, so level 10 will be ":" and level 15 will be "?" and so on)
"""
