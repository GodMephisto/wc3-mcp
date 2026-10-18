---
type: vendor-doc
source_repo: "https://github.com/merlin-s/w3x_reconstruction"
source_path: "units/parser.py"
ref: HEAD
---

import re
from typing import List, Union
from shared.parsing import *
from ._base import Unit, Item

_create_regex = re.compile(r"^.*BlzCreateUnitWithSkin\(([^)]*)\)")
_player_regex = re.compile(r"^.*Player\((\d+)\)")


# BlzCreateUnitWithSkin(p, 'hpea', - 191.2, 75.9, 277.402, 'hpea')


def parse_script(script: Union[List[str], str], is_lua: bool) -> List[Union[Unit, Item]]:
    """

    parses a list of code or code lines ...
    Args:
        script:
        is_lua:

    Returns:

    """

    if type(script) is str:
        script = script.split("\n")

    result: List[Union[Unit, Item]] = []

    player = None

    for line in script:
        if m := _player_regex.match(line):
            player = int(m.group(1).strip())
        elif m := _create_regex.match(line):
            assert player is not None
            args = m.group(1).split(',')
            _, type_id, x, y, rotation, _ = args
            result.append(Unit(char4(type_id), f(x), f(y), player))

    return result
