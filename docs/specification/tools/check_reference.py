"""Check that the generated MCP reference carries what DOC-01 says it must.

DOC-01 says the generated reference SHALL include each tool's family, description,
every parameter with type and default, the result shape, the error codes and the
hints, and CI SHALL fail when the committed file is stale. That is a completeness
claim about one file, so this reads the published exe and the reference and judges
each clause per tool instead of trusting the sentence in the document.

The reference is generated in the wc3ctl repository by `wc3ctl docs mcp`, so this
tree holds a copy rather than the generator. Point at it with --reference or
WC3_MCP_REFERENCE, and this guard reports the file it read. A full staleness check
needs the generator's hash, and the generator is out of tree, so the staleness
clause is judged by the two things a regeneration would move, the count sentence
and the tool names.

A tool counts for a field only when the field is there and agrees with the exe. A
family line that names a different family than the catalog does not count. A
description that differs from the tool list does not count. A parameter counts only
when its own line carries its name, its type and its default where the schema
declares one.

    python tools/check_reference.py
    python tools/check_reference.py --json
    python tools/check_reference.py --reference C:\\\\path\\\\to\\\\mcp-reference.md
    python tools/check_reference.py --selftest

Exit 0 when every tool carries all six fields and the counts agree, 2 when one is
missing or disagrees, 1 when the exe or the reference cannot be read.

The title is reported beside the six. DOC-01 does not name it, so it does not move
the verdict.
"""

from __future__ import annotations

import argparse
import json
import re
import sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
if str(HERE) not in sys.path:
    sys.path.insert(0, str(HERE))

from check_mirror_lag import find_reference  # noqa: E402
from measure_surface import MeasureError, find_exe  # noqa: E402
from surface import parameter_specs, read_surface  # noqa: E402

HEADING = re.compile(r"^## (.+?)\s*$", re.M)
DECLARED = re.compile(r"exposes\s+(?P<count>[\d,]+)\s+tool")

# The label starts a line and is followed by a stop, so prose that happens to hold
# the word does not count as the section.
FAMILY = re.compile(r"^(?:Family|Tool family)\s*[.:]\s*(?P<value>\S.*)$", re.M)
TITLE = re.compile(r"^Title\s*[.:]\s*(?P<value>\S.*)$", re.M)
ANNOTATIONS = re.compile(r"^Annotations\s*[.:]\s*(?P<value>.+)$", re.M)
PARAMETERS = re.compile(r"^(?:Parameters|Inputs|Arguments)\s*[.:]", re.M)
RESULT = re.compile(r"^(?:Result|Result shape|Returns|Output)\s*[.:]", re.M)
ERRORS = re.compile(r"^(?:Errors|Error codes)\s*[.:]", re.M)

# The four hints an annotation line has to carry, by the word the reference uses.
HINTS = ("read-only", "destructive", "idempotent", "open-world")

FIELDS = (
    ("family", "Family"),
    ("description", "Description"),
    ("parameters", "Parameters with type and default"),
    ("result", "Result shape"),
    ("errors", "Error codes"),
    ("hints", "Hints"),
)


class ReferenceError(MeasureError):
    """The reference cannot be read as a reference."""


def read_blocks(path: Path) -> dict:
    """The count sentence and the body of every tool, by name."""
    try:
        text = path.read_text(encoding="utf-8")
    except OSError as exc:
        raise ReferenceError(f"no reference at {path}, {exc}") from exc

    parts = HEADING.split(text)
    blocks: dict[str, str] = {}
    order: list[str] = []
    for index in range(1, len(parts) - 1, 2):
        name = parts[index].strip()
        body = parts[index + 1]
        if name in blocks:
            blocks[name] += "\n" + body
        else:
            blocks[name] = body
            order.append(name)
    if not order:
        raise ReferenceError(f"{path} has no `## tool` headings, so it names no tools")

    match = DECLARED.search(text)
    declared = int(match.group("count").replace(",", "")) if match else None
    return {"blocks": blocks, "order": order, "declared": declared, "path": path, "bytes": len(text)}


def description_of(body: str) -> str:
    """The first paragraph line that is not a labeled section."""
    for line in body.splitlines():
        line = line.strip()
        if not line:
            continue
        if re.match(r"^(?:Family|Tool family|Title|Annotations|Parameters|Inputs|Arguments|Result|Result shape|Returns|Output|Errors|Error codes)\s*[.:]", line):
            continue
        if line.startswith(("#", "|", "-", "*")):
            continue
        return " ".join(line.split())
    return ""


def normalise(text: str) -> str:
    return " ".join((text or "").split())


def parameter_ok(body: str, specs: list[dict]) -> bool:
    """Every parameter on its own line, with its type, and its default where declared."""
    if not PARAMETERS.search(body):
        return False
    for spec in specs:
        line = next(
            (l for l in body.splitlines() if re.search(r"\b" + re.escape(spec["name"]) + r"\b", l)),
            "",
        )
        if not line:
            return False
        lowered = line.lower()
        type_words = {part.strip().lower() for part in spec["type"].split("|")}
        if not any(word and word in lowered for word in type_words):
            return False
        if spec["has_default"]:
            default = spec["default"]
            if "default" not in lowered:
                return False
            if default is not None and str(default).lower() not in lowered:
                return False
    return True


def judge_tool(name: str, body: str, tool: dict, family: str) -> dict:
    """Which of the six DOC-01 clauses this tool carries, and whether each agrees."""
    description = description_of(body)
    family_match = FAMILY.search(body)
    annotations = ANNOTATIONS.search(body)
    specs = parameter_specs(tool.get("inputSchema"))

    row = {
        "tool": name,
        "family": bool(family_match) and normalise(family_match.group("value")) == normalise(family),
        "description": bool(description) and normalise(description) == normalise(tool.get("description") or ""),
        "parameters": parameter_ok(body, specs),
        "result": bool(RESULT.search(body)),
        "errors": bool(ERRORS.search(body)),
        "hints": bool(annotations)
        and all(hint in annotations.group("value").lower() for hint in HINTS),
        "title": bool(TITLE.search(body)),
        "parameters_expected": len(specs),
    }
    return row


def build_rows(surface: dict, reference: dict) -> tuple[list[dict], dict]:
    tools = surface["tools"]
    rows = []
    for tool in tools:
        name = tool.get("name")
        body = reference["blocks"].get(name)
        if body is None:
            rows.append(
                {
                    "tool": name,
                    "family": False,
                    "description": False,
                    "parameters": False,
                    "result": False,
                    "errors": False,
                    "hints": False,
                    "title": False,
                    "parameters_expected": len(parameter_specs(tool.get("inputSchema"))),
                }
            )
            continue
        rows.append(judge_tool(name, body, tool, surface["families"].get(name)))

    held = {key: sum(1 for row in rows if row[key]) for key, _ in FIELDS}
    observed = {"title": sum(1 for row in rows if row["title"])}
    counts = {
        "exe": len(tools),
        "reference": len(reference["order"]),
        "declared": reference["declared"],
        "named": sum(1 for row in rows if row["tool"] in reference["blocks"]),
        "extra": sorted(set(reference["blocks"]) - {tool.get("name") for tool in tools}),
        "parameters_expected": sum(row["parameters_expected"] for row in rows),
        "held": held,
        "observed": observed,
    }
    return rows, counts


def render(rows: list[dict], counts: dict, exe: Path, reference: dict) -> str:
    head = f"{'DOC-01 clause'.ljust(34)}  {'Tools'.rjust(5)}  {'of'.rjust(4)}  Verdict"
    lines = [head, "-" * len(head)]
    for key, label in FIELDS:
        held = counts["held"][key]
        verdict = "ok" if held == counts["exe"] else "MISSING"
        lines.append(f"{label.ljust(34)}  {str(held).rjust(5)}  {str(counts['exe']).rjust(4)}  {verdict}")
    lines.append(
        f"{'Title, reported and not required'.ljust(34)}  {str(counts['observed']['title']).rjust(5)}  "
        f"{str(counts['exe']).rjust(4)}  observed"
    )
    lines.append("")
    lines.append(
        f"the exe lists {counts['exe']} tools, the reference names {counts['named']}, "
        f"and states {counts['declared']}"
    )
    if counts["extra"]:
        lines.append("only in the reference, " + ", ".join(counts["extra"][:12]))
    missing = [row["tool"] for row in rows if not row["description"]]
    if missing:
        lines.append(
            f"{len(missing)} tools carry no description that matches the exe, "
            + ", ".join(missing[:12])
        )
    short = {key: counts["exe"] - counts["held"][key] for key, _ in FIELDS}
    short = {key: value for key, value in short.items() if value}
    if short:
        lines.append(
            "the reference is incomplete on "
            + ", ".join(f"{label} for {short[key]} tools" for key, label in FIELDS if key in short)
        )
    else:
        lines.append("every tool carries all six clauses and every count agrees")
    lines.append(f"server    {exe}")
    lines.append(f"reference {reference['path']}")
    return "\n".join(lines)


def selftest() -> int:
    """Prove a complete block passes and each missing clause is named."""
    failures: list[str] = []
    tool = {
        "name": "sample",
        "description": "Do the sample thing.",
        "inputSchema": {
            "type": "object",
            "properties": {
                "map": {"type": "string"},
                "limit": {"type": "integer", "default": 10},
            },
            "required": ["map"],
        },
    }
    family = "maps"
    complete = (
        "\nFamily. maps\n\nDo the sample thing.\n\n"
        "Parameters.\n\n"
        "map, string, required.\n\nlimit, integer, default 10.\n\n"
        "Result. the sample shape.\n\n"
        "Errors. SAMPLE_CODE.\n\n"
        "Annotations. read-only yes, destructive no, idempotent yes, open-world no.\n"
    )
    row = judge_tool("sample", complete, tool, family)
    if not all(row[key] for key, _ in FIELDS):
        failures.append(f"a complete block did not pass, {row}")

    sparse = "\nDo the sample thing.\n\nAnnotations. read-only yes, destructive no, idempotent yes, open-world no.\n"
    row = judge_tool("sample", sparse, tool, family)
    for key in ("family", "parameters", "result", "errors"):
        if row[key]:
            failures.append(f"a block with no {key} passed that clause")

    wrong = complete.replace("Family. maps", "Family. scripts")
    if judge_tool("sample", wrong, tool, family)["family"]:
        failures.append("a family that disagrees with the catalog passed")

    no_default = complete.replace("limit, integer, default 10.", "limit, integer.")
    if judge_tool("sample", no_default, tool, family)["parameters"]:
        failures.append("a parameter with no default passed where the schema declares one")

    if failures:
        for failure in failures:
            print("selftest failed, " + failure, file=sys.stderr)
        return 1
    print(
        "selftest ok, a complete block passes, and a missing family, parameter line, result shape, "
        "error code, a wrong family and a dropped default are each named"
    )
    return 0


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(
        description="Check the generated MCP reference against DOC-01 and the published exe."
    )
    parser.add_argument("--exe", help="path to the published Wc3.Mcp executable")
    parser.add_argument("--reference", help="path to the generated mcp-reference.md")
    parser.add_argument("--timeout", type=float, default=180.0, help="seconds per request")
    parser.add_argument("--json", action="store_true", help="print the check as JSON")
    parser.add_argument("--selftest", action="store_true", help="run the built in selftest")
    args = parser.parse_args(argv)

    if args.selftest:
        return selftest()

    try:
        exe = find_exe(args.exe)
        reference_path = find_reference(args.reference)
        reference = read_blocks(reference_path)
        surface = read_surface(exe, args.timeout, client="check_reference")
        rows, counts = build_rows(surface, reference)
    except MeasureError as exc:
        print(str(exc), file=sys.stderr)
        return 1

    ok = all(counts["held"][key] == counts["exe"] for key, _ in FIELDS) and counts[
        "declared"
    ] == counts["exe"]
    code = 0 if ok else 2

    if args.json:
        print(
            json.dumps(
                {
                    "ok": ok,
                    "exit": code,
                    "exe": str(exe),
                    "reference": str(reference_path),
                    "tools": counts["exe"],
                    "declared": counts["declared"],
                    "named": counts["named"],
                    "fields": {key: counts["held"][key] for key, _ in FIELDS},
                    "parameters_expected": counts["parameters_expected"],
                    "observed": counts["observed"],
                    "extra": counts["extra"],
                    "rows": rows,
                },
                indent=2,
            )
        )
    else:
        print(render(rows, counts, exe, reference))
    return code


if __name__ == "__main__":
    sys.exit(main())
