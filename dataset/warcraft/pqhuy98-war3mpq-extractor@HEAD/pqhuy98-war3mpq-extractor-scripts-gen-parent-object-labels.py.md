---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "scripts/gen-parent-object-labels.py"
ref: HEAD
---

#!/usr/bin/env python3
"""Build parent object rawcode -> display name from default WC3 object SLK data."""
import json
import re
from pathlib import Path

EXTRACTED = Path(__file__).resolve().parent.parent / "extracted-mpq"
ABILITY_DATA = EXTRACTED / "war3.w3mod" / "units" / "abilitydata.slk"
ITEM_DATA = EXTRACTED / "war3.w3mod" / "units" / "itemdata.slk"
UNIT_DATA = EXTRACTED / "war3.w3mod" / "units" / "unitdata.slk"
BUFF_DATA = EXTRACTED / "war3.w3mod" / "units" / "abilitybuffdata.slk"
DEST_DATA = EXTRACTED / "war3.w3mod" / "units" / "destructabledata.slk"
DOODAD_DATA = EXTRACTED / "war3.w3mod" / "doodads" / "doodads.slk"
UPGRADE_DATA = EXTRACTED / "war3.w3mod" / "units" / "upgradedata.slk"
DEFAULT_OUT = (
    Path(__file__).resolve().parent.parent.parent
    / "wotlk-campaign"
    / "common"
    / "scripts"
    / "object-data-sync"
    / "parent-object-labels.json"
)


def parse_slk_rows(text: str) -> list[dict[int, str]]:
    rows_by_y: dict[int, dict[int, str]] = {}
    current_y: int | None = None
    current: dict[int, str] = {}

    def flush_row() -> None:
        nonlocal current_y, current
        if current_y is not None and current_y != 1:
            rows_by_y[current_y] = dict(current)

    for line in text.splitlines():
        match = re.match(r'C;X(\d+);(?:Y(\d+);)?K"?([^"]*)"?', line)
        if not match:
            continue
        x = int(match.group(1))
        if match.group(2):
            flush_row()
            current_y = int(match.group(2))
            current = {}
        if current_y is None:
            continue
        current[x] = match.group(3)

    flush_row()
    return [rows_by_y[y] for y in sorted(rows_by_y)]


def ability_display_name(comments: str) -> str:
    text = comments.strip()
    if " - " in text:
        return text.rsplit(" - ", 1)[-1].strip()
    return text


def code_name_rows(rows: list[dict[int, str]], code_col: int, name_col: int) -> dict[str, str]:
    out: dict[str, str] = {}
    for row in rows:
        code = row.get(code_col, "")
        if not code or len(code) != 4:
            continue
        name = row.get(name_col, "")
        if not name:
            continue
        out[code] = name.strip()
    return out


def build_ability_labels(rows: list[dict[int, str]]) -> dict[str, str]:
    out: dict[str, str] = {}
    for row in rows:
        code = row.get(1, "")
        if not code or len(code) != 4:
            continue
        comments = row.get(3, "")
        if not comments:
            continue
        out[code] = ability_display_name(comments)
    return out


def build_buff_labels(rows: list[dict[int, str]]) -> dict[str, str]:
    out: dict[str, str] = {}
    for row in rows:
        code = row.get(1, "")
        if not code or len(code) != 4:
            continue
        name = row.get(3, "")
        if not name:
            continue
        out[code] = name.strip()
    return out


def main() -> None:
    import argparse

    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--out", type=Path, default=DEFAULT_OUT)
    args = parser.parse_args()

    payload = {
        "version": 2,
        "abilities": dict(sorted(build_ability_labels(parse_slk_rows(ABILITY_DATA.read_text(encoding="utf-8", errors="replace"))).items())),
        "items": dict(sorted(code_name_rows(parse_slk_rows(ITEM_DATA.read_text(encoding="utf-8", errors="replace")), 1, 2).items())),
        "units": dict(sorted(code_name_rows(parse_slk_rows(UNIT_DATA.read_text(encoding="utf-8", errors="replace")), 1, 3).items())),
        "buffs": dict(sorted(build_buff_labels(parse_slk_rows(BUFF_DATA.read_text(encoding="utf-8", errors="replace"))).items())),
        "destructibles": dict(sorted(code_name_rows(parse_slk_rows(DEST_DATA.read_text(encoding="utf-8", errors="replace")), 1, 7).items())),
        "doodads": dict(sorted(code_name_rows(parse_slk_rows(DOODAD_DATA.read_text(encoding="utf-8", errors="replace")), 1, 6).items())),
        "upgrades": dict(sorted(code_name_rows(parse_slk_rows(UPGRADE_DATA.read_text(encoding="utf-8", errors="replace")), 1, 2).items())),
    }

    args.out.parent.mkdir(parents=True, exist_ok=True)
    args.out.write_text(f"{json.dumps(payload, indent=2, ensure_ascii=False)}\n", encoding="utf-8")

    counts = ", ".join(f"{k}={len(payload[k])}" for k in payload if k != "version")
    print(f"Wrote parent object labels ({counts}) to {args.out}")


if __name__ == "__main__":
    main()
