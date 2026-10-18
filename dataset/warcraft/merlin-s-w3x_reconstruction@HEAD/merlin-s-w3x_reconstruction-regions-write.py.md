---
type: vendor-doc
source_repo: "https://github.com/merlin-s/w3x_reconstruction"
source_path: "regions/write.py"
ref: HEAD
---

from typing import List

from regions.region import Region
from shared import Blob


def serialize_as_blob(regions: List[Region]) -> Blob:
    b = Blob()
    format_version = 5
    b.append_int(format_version)
    b.append_int(len(regions))
    for i, r in enumerate(regions):
        b.append_float(r.left)
        b.append_float(r.bottom)
        b.append_float(r.right)
        b.append_float(r.top)
        b.append_plain_str(r.name)
        b.append_int(i)
        weather_effect = 0
        b.append_int(weather_effect)
        ambient_sound = ""
        b.append_plain_str(ambient_sound)
        b.append_color(*r.color)
        b.append_byte(255)
    return b
