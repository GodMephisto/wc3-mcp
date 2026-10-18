---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "scripts/gen-mod-field-labels.py"
ref: HEAD
---

#!/usr/bin/env python3
"""Build mod field id -> slug/comment map from WC3 extracted MPQ metadata."""
import json
import re
from collections.abc import Callable
from pathlib import Path

EXTRACTED = Path(__file__).resolve().parent.parent / "extracted-mpq"
STRINGS = EXTRACTED / "UI" / "WorldEditStrings.txt"
DEFAULT_OUT = (
    Path(__file__).resolve().parent.parent.parent
    / "wotlk-campaign"
    / "common"
    / "scripts"
    / "object-data-sync"
    / "mod-field-labels.json"
)

ABILITY_META = EXTRACTED / "war3.w3mod" / "units" / "abilitymetadata.slk"
UNIT_META = EXTRACTED / "war3.w3mod" / "units" / "unitmetadata.slk"
BUFF_META = EXTRACTED / "war3.w3mod" / "units" / "abilitybuffmetadata.slk"
DEST_META = EXTRACTED / "war3.w3mod" / "units" / "destructablemetadata.slk"
UPGRADE_META = EXTRACTED / "war3.w3mod" / "units" / "upgrademetadata.slk"
DOODAD_META = EXTRACTED / "war3.w3mod" / "doodads" / "doodadmetadata.slk"


def parse_strings(text: str) -> dict[str, str]:
    out: dict[str, str] = {}
    for line in text.splitlines():
        line = line.strip()
        if not line or line.startswith("[") or "=" not in line:
            continue
        key, _, value = line.partition("=")
        out[key.strip()] = value.strip().strip('"')
    return out


def parse_slk(text: str) -> list[dict[int, str]]:
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


def field_to_label(field_name: str) -> str:
    parts = re.sub(r"([a-z])([A-Z])", r"\1 \2", field_name)
    parts = re.sub(r"(\d+)", r" \1", parts)
    return parts.strip()


def section_prefix(section: str) -> str:
    if section.endswith("Data"):
        section = section[: -len("Data")]
    return section


def build_entry(
    row: dict[int, str],
    strings: dict[str, str],
    *,
    display_col: int,
    section_col: int,
    field_col: int = 2,
) -> tuple[str, dict[str, str]] | None:
    code = row.get(1, "")
    field_name = row.get(field_col, "")
    section = row.get(section_col, "")
    display_key = row.get(display_col, "")
    if not code or len(code) != 4:
        return None

    we_label = strings.get(display_key) if display_key else ""
    if not we_label:
        we_label = field_to_label(field_name) or display_key or code

    slug = f"{section}_{field_name}" if section and field_name else field_name or code

    prefix = section_prefix(section) if section else ""
    if prefix:
        comment = f"{prefix} {we_label.lower()}"
    else:
        comment = we_label

    return code, {"slug": slug, "comment": comment}


def merge_rows(
    rows: list[dict[int, str]],
    strings: dict[str, str],
    *,
    display_col: int,
    section_col: int,
    row_filter: Callable[[dict[int, str]], bool] | None = None,
) -> dict[str, dict[str, str]]:
    out: dict[str, dict[str, str]] = {}
    for row in rows:
        if row_filter is not None and not row_filter(row):
            continue
        entry = build_entry(row, strings, display_col=display_col, section_col=section_col)
        if entry:
            code, payload = entry
            out[code] = payload
    return out


def unit_row(row: dict[int, str]) -> bool:
    return row.get(17) == "1" or row.get(18) == "1" or row.get(19) == "1"


def item_row(row: dict[int, str]) -> bool:
    return row.get(20) == "1"


def main() -> None:
    import argparse

    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--out", type=Path, default=DEFAULT_OUT)
    args = parser.parse_args()

    strings = parse_strings(STRINGS.read_text(encoding="utf-8", errors="replace"))
    unit_rows = parse_slk(UNIT_META.read_text(encoding="utf-8", errors="replace"))

    payload: dict[str, object] = {
        "version": 2,
        "abilities": merge_rows(
            parse_slk(ABILITY_META.read_text(encoding="utf-8", errors="replace")),
            strings,
            display_col=8,
            section_col=3,
        ),
        "items": merge_rows(unit_rows, strings, display_col=6, section_col=3, row_filter=item_row),
        "units": merge_rows(unit_rows, strings, display_col=6, section_col=3, row_filter=unit_row),
        "buffs": merge_rows(
            parse_slk(BUFF_META.read_text(encoding="utf-8", errors="replace")),
            strings,
            display_col=6,
            section_col=3,
        ),
        "destructibles": merge_rows(
            parse_slk(DEST_META.read_text(encoding="utf-8", errors="replace")),
            strings,
            display_col=6,
            section_col=3,
        ),
        "doodads": merge_rows(
            parse_slk(DOODAD_META.read_text(encoding="utf-8", errors="replace")),
            strings,
            display_col=7,
            section_col=3,
        ),
        "upgrades": merge_rows(
            parse_slk(UPGRADE_META.read_text(encoding="utf-8", errors="replace")),
            strings,
            display_col=10,
            section_col=3,
        ),
    }

    for key in payload:
        if key == "version":
            continue
        payload[key] = dict(sorted(payload[key].items()))  # type: ignore[index]

    args.out.parent.mkdir(parents=True, exist_ok=True)
    args.out.write_text(f"{json.dumps(payload, indent=2, ensure_ascii=False)}\n", encoding="utf-8")

    counts = ", ".join(f"{k}={len(payload[k])}" for k in payload if k != "version")  # type: ignore[arg-type]
    print(f"Wrote mod field labels ({counts}) to {args.out}")


if __name__ == "__main__":
    main()
