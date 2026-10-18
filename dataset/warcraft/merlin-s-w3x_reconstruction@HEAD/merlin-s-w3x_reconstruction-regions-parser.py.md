---
type: vendor-doc
source_repo: "https://github.com/merlin-s/w3x_reconstruction"
source_path: "regions/parser.py"
ref: HEAD
---

import re
from typing import List, Union, Optional

from regions.region import Region
from shared import Color

_base_regex = r"gg_rct_([a-zA-Z0-9_]+)\s*=\s*Rect\(([^)]*)\)"
_lua_regex = re.compile(f"^{_base_regex}$")
_jass_regex = re.compile(fr"^set\s+{_base_regex}$")


class _ColorPicker:
    def __init__(self):
        self._replace_digits = re.compile("[0-9]+")
        self.static_color = None

    def pick(self, name):
        if self.static_color:
            return self.static_color
        grouped = self._replace_digits.sub("_", name)
        h = int(hash(grouped))
        r = h % 256
        g = (h >> 8) % 256
        b = (h >> 16) % 256
        return Color(r, g, b)


def parse_script(script: Union[List[str], str], is_lua: bool, static_color: Optional[Color] = None) -> List[Region]:
    """

    parses a list of code or code lines of the CreateRegions function in either Jass or Lua
    and returns a list of Region objects

    Note that the region color is an editor only artifact and cannot be reconstructed from the script

    TODO: handle ambient sounds and weather effects

    Args:
        script:
        is_lua:
        static_color:

    Returns:

    """

    if type(script) is str:
        script = script.split("\n")

    regions: List[Region] = []

    color_picker = _ColorPicker()
    color_picker.static_color = static_color
    construct_rect = _lua_regex if is_lua else _jass_regex

    for line in script:
        if m := construct_rect.match(line):
            name = m.group(1).replace("_", " ")
            values = list(map(float, m.group(2).split(',')))
            color = color_picker.pick(name)
            regions.append(Region(name, *values, color))

    return regions
