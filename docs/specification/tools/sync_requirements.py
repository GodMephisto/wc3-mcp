"""Keep requirements.json and the Status column in the specification in step.

The JSON is the source of status and test names. The command parses the requirement
tables and the gap table from the markdown, merges them with what is already in the
JSON, writes the JSON, then rebuilds the Status column to match.

    python tools/sync_requirements.py
    python tools/sync_requirements.py --json

A new requirement starts as "gap" when a row in section 10 names it, and
"unassessed" otherwise. To mark one met, set its status and test in the JSON and run
the command again.

This is the only writer of requirements.json and of the Status column, so a second
process editing either by hand is a second writer. Both files are written to a
temporary name beside them and renamed into place, so a reader running at the same
time sees the old file whole or the new file whole, never half of either.
"""

from __future__ import annotations

import argparse
import json
import os
import re
import sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
SPEC = HERE.parent / "wc3-mcp-specification-v1.md"
DATA = HERE.parent / "requirements.json"
ID = re.compile(r"^[A-Z]{2,4}-\d{2,3}$")


def cells(line: str):
    return [c.strip() for c in line.strip().strip("|").split("|")]


def parse(md: str):
    reqs, gaps = {}, {}
    for line in md.split("\n"):
        if not line.startswith("|"):
            continue
        c = cells(line)
        if len(c) >= 3 and ID.match(c[0]) and not c[0].startswith(("AC-", "G-")):
            basis = re.sub(r"(\s{2,}(?:gap|unassessed|met|present))+\s*$", "", c[2]).strip()
            reqs[c[0]] = {
                "id": c[0],
                "group": c[0].split("-")[0],
                "statement": c[1],
                "basis": basis,
            }
        elif len(c) >= 4 and re.match(r"^G-\d{2,3}$", c[0]):
            gaps[c[0]] = {"id": c[0], "what": c[1], "requirements": c[2], "where": c[3]}
            for rid in re.findall(r"[A-Z]{2,4}-\d{2,3}", c[2]):
                if rid in reqs:
                    reqs[rid]["gap"] = c[0]
    return reqs, gaps


def write_atomic(path: Path, text: str) -> None:
    """Write beside the file, then rename, so no reader sees a half written file."""
    temporary = path.with_name(path.name + ".tmp-" + str(os.getpid()))
    temporary.write_text(text, encoding="utf-8", newline="\n")
    os.replace(temporary, path)


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description="Refresh requirements.json and the Status column.")
    parser.add_argument("--markdown", default=str(SPEC), help="the specification markdown")
    parser.add_argument("--requirements", default=str(DATA), help="the requirements JSON")
    parser.add_argument("--json", action="store_true", help="print the result as one JSON object")
    args = parser.parse_args(argv)

    markdown = Path(args.markdown)
    data_path = Path(args.requirements)
    md = markdown.read_text(encoding="utf-8")
    old = {}
    if data_path.exists():
        old = {r["id"]: r for r in json.loads(data_path.read_text(encoding="utf-8"))["requirements"]}

    reqs, gaps = parse(md)
    for rid, req in reqs.items():
        prev = old.get(rid, {})
        status = prev.get("status")
        if status in (None, "present", "unassessed") and req.get("gap"):
            status = "gap"
        if status in (None, "present"):
            status = "unassessed"
        req["status"] = status
        req["test"] = prev.get("test")

    data = {
        "requirements": [reqs[k] for k in sorted(reqs)],
        "gaps": [gaps[k] for k in sorted(gaps)],
    }
    write_atomic(data_path, json.dumps(data, indent=2, ensure_ascii=False) + "\n")

    start = md.index("## 4. Requirements")
    end = md.index("## 5. Flows")
    out = []
    for line in md[start:end].split("\n"):
        if line.startswith("|"):
            c = cells(line)
            if c and c[0] in reqs:
                r = reqs[c[0]]
                line = f'| {r["id"]} | {r["statement"]} | {r["basis"]} | {r["status"]} |'
        out.append(line)
    write_atomic(markdown, md[:start] + "\n".join(out) + md[end:])

    counts = {}
    for r in reqs.values():
        counts[r["status"]] = counts.get(r["status"], 0) + 1
    summary = f"{len(reqs)} requirements, {len(gaps)} gaps, " + ", ".join(
        f"{k} {v}" for k, v in sorted(counts.items())
    )
    if args.json:
        print(
            json.dumps(
                {
                    "ok": True,
                    "requirements": len(reqs),
                    "groups": len({r["group"] for r in reqs.values()}),
                    "gaps": len(gaps),
                    "status": counts,
                    "markdown": str(markdown),
                    "requirements_json": str(data_path),
                },
                indent=2,
            )
        )
    else:
        print(summary)
    return 0


if __name__ == "__main__":
    sys.exit(main())
