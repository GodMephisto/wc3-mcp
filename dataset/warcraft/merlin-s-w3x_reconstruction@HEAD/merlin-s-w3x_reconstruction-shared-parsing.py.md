---
type: vendor-doc
source_repo: "https://github.com/merlin-s/w3x_reconstruction"
source_path: "shared/parsing.py"
ref: HEAD
---

def f(s: str) -> float:
    return float(s.replace(" ", ""))


def i(s: str) -> float:
    return int(s.replace(" ", ""))


def char4(s: str) -> str:
    r = s.strip().replace("'", "")
    assert len(r) == 4
    return r