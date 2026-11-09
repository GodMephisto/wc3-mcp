"""Keep requirements.json and the Status column in the specification in step.

The JSON is the source of status and test names. The command parses the requirement
tables and the gap table from the markdown, merges them with what is already in the
JSON, writes the JSON, then rebuilds the Status column to match.

    python tools/sync_requirements.py

A new requirement starts as "gap" when a row in section 10 names it, and
"unassessed" otherwise. To mark one met, set its status and test in the JSON and run
the command again.
"""

import json
import re
import sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
SPEC = HERE.parent / "wc3-mcp-specification-v1.md"
DATA = HERE.parent / "requirements.json"
ID = re.compile(r"^[A-Z]{2,4}-\d{2}$")


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
            for rid in re.findall(r"[A-Z]{2,4}-\d{2}", c[2]):
                if rid in reqs:
                    reqs[rid]["gap"] = c[0]
    return reqs, gaps


def main() -> int:
    md = SPEC.read_text(encoding="utf-8")
    old = {}
    if DATA.exists():
        old = {r["id"]: r for r in json.loads(DATA.read_text(encoding="utf-8"))["requirements"]}

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
    DATA.write_text(json.dumps(data, indent=2, ensure_ascii=False) + "\n", encoding="utf-8", newline="\n")

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
    SPEC.write_text(md[:start] + "\n".join(out) + md[end:], encoding="utf-8", newline="\n")

    counts = {}
    for r in reqs.values():
        counts[r["status"]] = counts.get(r["status"], 0) + 1
    print(f"{len(reqs)} requirements, {len(gaps)} gaps, " + ", ".join(f"{k} {v}" for k, v in sorted(counts.items())))
    return 0


if __name__ == "__main__":
    sys.exit(main())
