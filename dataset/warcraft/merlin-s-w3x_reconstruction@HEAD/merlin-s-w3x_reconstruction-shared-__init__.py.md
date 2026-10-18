---
type: vendor-doc
source_repo: "https://github.com/merlin-s/w3x_reconstruction"
source_path: "shared/__init__.py"
ref: HEAD
---


from .blob import Blob  # noqa: F401
from collections import namedtuple as _namedtuple
from .testcase import TestCase

Color = _namedtuple("Color", "r, g, b")
