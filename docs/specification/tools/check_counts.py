"""Keep the specification's own counts honest.

Sections 3.1, 3.3 and 3.5 are guarded against the published exe by
measure_surface.py. This guards the other direction, the counts the document and
the README state about themselves:

  * the tool count in the README's opening paragraph, against tools/list;
  * the requirement, group and acceptance-check counts in the section 2 sentence
    "This version holds 179 requirements in 20 groups, and 41 acceptance checks
    that cover them.", against requirements.json and the AC rows in the markdown;
  * the same counts in the README paragraph "It states 179 requirements in 20
    groups, 41 acceptance checks, the 275-tool inventory, 56 gap rows, ...";
  * the gap row count in the README, against the `gaps` list in requirements.json;
  * the requirements the section 10 gap table names, against the requirements
    whose status is "gap" in requirements.json.

Every declared figure is parsed out of the prose, never hardcoded, so editing a
sentence to the wrong number fails the guard instead of passing it.

    python tools/check_counts.py
    python tools/check_counts.py --json
    python tools/check_counts.py --exe C:\\\\path\\\\to\\\\Wc3.Mcp.exe
    python tools/check_counts.py --markdown C:\\\\path\\\\to\\\\spec.md

Exit 0 when every figure agrees, 2 when one drifted, 1 when the exe or a file
cannot be read, including a prose sentence the guard depends on. The PDF build
runs this after measure_surface.py and refuses to render on a drift.
"""

import argparse
import json
import re
import sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
if str(HERE) not in sys.path:
    sys.path.insert(0, str(HERE))

# The MCP stdio client and the exe lookup are shared, not copied. The session is
# the only part of measure_surface.py this tool needs.
from measure_surface import MeasureError, Session, find_exe  # noqa: E402

DOCS = HERE.parent
SPEC = DOCS / "wc3-mcp-specification-v1.md"
README = DOCS / "README.md"
REQUIREMENTS = DOCS / "requirements.json"

# "This version holds 179 requirements in 20 groups, and 41 acceptance checks"
SPEC_COUNTS = re.compile(
    r"This version holds\s+(?P<requirements>[\d,]+)\s+requirements in\s+"
    r"(?P<groups>[\d,]+)\s+groups,?\s+and\s+(?P<acs>[\d,]+)\s+acceptance checks"
)

# "It states 179 requirements in 20 groups, 41 acceptance checks, the 275-tool
# inventory, 56 gap rows,"
README_COUNTS = re.compile(
    r"It states\s+(?P<requirements>[\d,]+)\s+requirements in\s+"
    r"(?P<groups>[\d,]+)\s+groups,\s+(?P<acs>[\d,]+)\s+acceptance checks,\s+the\s+"
    r"(?P<tools>[\d,]+)-tool inventory,\s+(?P<gaps>[\d,]+)\s+gap rows"
)

AC_ROW = re.compile(r"^\| AC-\d{2} \|", re.M)
GAP_ROW = re.compile(r"^\| G-\d{2} \|")
REQ_REF = re.compile(r"[A-Z]{2,4}-\d{2}")


class CountError(MeasureError):
    """A count the guard depends on could not be read, so it will not pass."""


def number(text: str) -> int:
    return int(text.replace(",", ""))


def section(md: str, number_: str) -> str:
    """The text of one `## N.` section, up to the next level-two heading."""
    m = re.search(r"^## " + re.escape(number_) + r"\. .*$", md, re.M)
    if not m:
        raise CountError(
            f"the document has no section {number_}. The count guard reads its figures "
            "from sections 2 and 10, so a renumber fails it rather than passing it"
        )
    rest = md[m.end() :]
    nxt = re.search(r"^## ", rest, re.M)
    return rest[: nxt.start()] if nxt else rest


def prose_counts(text: str, pattern: re.Pattern, where: str, what: str) -> dict:
    """The four or five numbers one sentence states, by name."""
    found = pattern.findall(text)
    if not found:
        raise CountError(
            f"{where} has no sentence matching the {what} the guard reads. "
            f"The guard will not pass counts it cannot parse. Expected something like "
            f"{pattern.pattern!r}"
        )
    if len(found) > 1:
        raise CountError(
            f"{where} states the {what} more than once ({len(found)} matches), so the "
            "guard cannot tell which sentence is the claim"
        )
    m = pattern.search(text)
    assert m is not None
    return {key: number(value) for key, value in m.groupdict().items()}


def load_requirements(path: Path) -> dict:
    """The requirement and gap lists, with the shape the guard relies on."""
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except OSError as exc:
        raise CountError(f"no requirements data at {path}, {exc}") from exc
    except json.JSONDecodeError as exc:
        raise CountError(f"{path} is not valid JSON, {exc}") from exc

    requirements = data.get("requirements") if isinstance(data, dict) else None
    gaps = data.get("gaps") if isinstance(data, dict) else None
    if not isinstance(requirements, list) or not isinstance(gaps, list):
        raise CountError(
            f"{path} must hold a 'requirements' list and a 'gaps' list, it holds "
            f"{sorted(data) if isinstance(data, dict) else type(data).__name__}"
        )
    for req in requirements:
        if not isinstance(req, dict) or not req.get("id"):
            raise CountError(f"{path} has a requirement with no id, {req!r}")
    return data


def gap_requirement_ids(sec10: str) -> set[str]:
    """Every requirement ID named in the Requirement cell of a section 10 gap row."""
    ids: set[str] = set()
    rows = 0
    for line in sec10.splitlines():
        if not GAP_ROW.match(line):
            continue
        rows += 1
        cells = line.strip().strip("|").split("|")
        if len(cells) < 3:
            raise CountError(
                f"the gap row starting {cells[0].strip()!r} has no Requirement cell, "
                "so the guard cannot tell which requirements it names"
            )
        ids.update(REQ_REF.findall(cells[2]))
    if rows == 0:
        raise CountError(
            "section 10 has no gap rows. The guard reads the requirements they name, "
            "so an empty section fails it rather than passing it"
        )
    return ids


def live_tool_count(exe: Path, timeout: float) -> int:
    """How many tools the published server lists, paginated like measure_surface."""
    session = Session(exe, timeout)
    try:
        session.request(
            "initialize",
            {
                "protocolVersion": "2025-06-18",
                "capabilities": {},
                "clientInfo": {"name": "check_counts", "version": "1"},
            },
        )
        session.notify("notifications/initialized")
        total = 0
        cursor = None
        while True:
            page = session.request("tools/list", {"cursor": cursor} if cursor else {})
            total += len(page.get("tools") or [])
            cursor = page.get("nextCursor")
            if not cursor:
                return total
    finally:
        session.close()


def build_rows(spec: dict, readme: dict, actual: dict) -> list[dict]:
    """One row per declared figure, with where it is stated and where it is held."""

    def row(measure, noun, source, declared, held_by, current) -> dict:
        return {
            "measure": measure,
            "noun": noun,
            "source": source,
            "declared": declared,
            "holds": held_by,
            "actual": current,
            "ok": declared == current,
        }

    return [
        row("Tools", "tools in the inventory", "README.md", readme["tools"],
            "the server lists", actual["tools"]),
        row("Requirements", "requirements", "section 2", spec["requirements"],
            "requirements.json holds", actual["requirements"]),
        row("Requirements", "requirements", "README.md", readme["requirements"],
            "requirements.json holds", actual["requirements"]),
        row("Requirement groups", "groups", "section 2", spec["groups"],
            "requirements.json holds", actual["groups"]),
        row("Requirement groups", "groups", "README.md", readme["groups"],
            "requirements.json holds", actual["groups"]),
        row("Acceptance checks", "acceptance checks", "section 2", spec["acs"],
            "the markdown holds", actual["acs"]),
        row("Acceptance checks", "acceptance checks", "README.md", readme["acs"],
            "the markdown holds", actual["acs"]),
        row("Gap rows", "gap rows", "README.md", readme["gaps"],
            "requirements.json holds", actual["gaps"]),
        row("Requirements with status gap", "requirements with status gap",
            "section 10 gap rows", actual["gap_named"],
            "requirements.json marks as gap", actual["gap_status"]),
    ]


def fmt(value) -> str:
    return f"{value:,}" if isinstance(value, int) else str(value)


def stale_line(row: dict) -> str:
    return (
        f"  {row['source']} says {fmt(row['declared'])} {row['noun']}, "
        f"{row['holds']} {fmt(row['actual'])}"
    )


def render_table(rows: list[dict], exe: Path) -> str:
    header = ("Measure", "Source", "Declared", "Actual", "Verdict")
    body = [
        (
            r["measure"],
            r["source"],
            fmt(r["declared"]),
            fmt(r["actual"]),
            "ok" if r["ok"] else "DRIFT",
        )
        for r in rows
    ]
    widths = [max(len(x[i]) for x in [header] + body) for i in range(5)]
    out = [
        f"{header[0]:<{widths[0]}}  {header[1]:<{widths[1]}}  "
        f"{header[2]:>{widths[2]}}  {header[3]:>{widths[3]}}  {header[4]}",
        "-" * (sum(widths) + 8),
    ]
    for line in body:
        out.append(
            f"{line[0]:<{widths[0]}}  {line[1]:<{widths[1]}}  "
            f"{line[2]:>{widths[2]}}  {line[3]:>{widths[3]}}  {line[4]}"
        )
    drifted = [r for r in rows if not r["ok"]]
    out.append("")
    if drifted:
        out.append(
            f"{len(drifted)} of {len(rows)} declared figures drifted. "
            "The document no longer describes itself."
        )
        out.extend(stale_line(r) for r in drifted)
        out.append(
            "Change each Declared figure where it is stated, in the markdown or the README."
        )
    else:
        out.append(f"All {len(rows)} declared figures agree with their source of truth.")
    out.append(f"server {exe}")
    return "\n".join(out)


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(
        description="Compare the specification's own counts with the data and the exe."
    )
    parser.add_argument("--exe", help="path to the published Wc3.Mcp executable")
    parser.add_argument("--markdown", default=str(SPEC), help="the specification markdown")
    parser.add_argument("--readme", default=str(README), help="the folder README")
    parser.add_argument(
        "--requirements", default=str(REQUIREMENTS), help="the requirements JSON"
    )
    parser.add_argument("--timeout", type=float, default=180.0, help="seconds per request")
    parser.add_argument("--json", action="store_true", help="print the comparison as JSON")
    args = parser.parse_args(argv)

    try:
        exe = find_exe(args.exe)
        markdown = Path(args.markdown)
        readme_path = Path(args.readme)
        requirements_path = Path(args.requirements)
        for label, path in (
            ("specification markdown", markdown),
            ("README", readme_path),
        ):
            if not path.is_file():
                raise CountError(f"no {label} at {path}")

        md = markdown.read_text(encoding="utf-8")
        readme_text = readme_path.read_text(encoding="utf-8")
        data = load_requirements(requirements_path)

        spec = prose_counts(md, SPEC_COUNTS, markdown.name, "counts sentence in section 2")
        readme = prose_counts(
            readme_text, README_COUNTS, readme_path.name, "counts paragraph"
        )

        requirements = data["requirements"]
        gaps = data["gaps"]
        actual = {
            "tools": live_tool_count(exe, args.timeout),
            "requirements": len(requirements),
            "groups": len({r.get("group") for r in requirements if r.get("group")}),
            "acs": len(AC_ROW.findall(md)),
            "gaps": len(gaps),
            "gap_named": len(gap_requirement_ids(section(md, "10"))),
            "gap_status": len(
                {r["id"] for r in requirements if r.get("status") == "gap"}
            ),
        }
        rows = build_rows(spec, readme, actual)
    except MeasureError as exc:
        print(str(exc), file=sys.stderr)
        return 1

    ok = all(r["ok"] for r in rows)
    if args.json:
        print(
            json.dumps(
                {
                    "ok": ok,
                    "drifted": sum(1 for r in rows if not r["ok"]),
                    "exe": str(exe),
                    "markdown": str(markdown),
                    "readme": str(readme_path),
                    "requirements_json": str(requirements_path),
                    "declared": {"section_2": spec, "readme": readme},
                    "actual": actual,
                    "rows": rows,
                },
                indent=2,
                ensure_ascii=False,
            )
        )
    else:
        print(render_table(rows, exe))
    return 0 if ok else 2


if __name__ == "__main__":
    sys.exit(main())
