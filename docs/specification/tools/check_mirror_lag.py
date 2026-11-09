"""Turn the reference lag into a number.

TST-12 says a pull request that changes the surface SHALL fail when the count in
the README, the generated reference or this specification disagrees with the live
count. `check_counts.py` covers the README and this specification. The generated
reference is `docs/mcp-reference.md`, and its generator `wc3ctl docs mcp` lives in
the wc3ctl repository, so the committed copy is a mirror export of a surface built
somewhere else and nothing here compared it with the exe until now.

This reads the published exe, names every tool it lists, reads the mirror export,
and reports the lag as three numbers. How many tools the exe lists, how many the
export names, and how many names are on one side and not the other.

    python tools/check_mirror_lag.py
    python tools/check_mirror_lag.py --json
    python tools/check_mirror_lag.py --reference C:\\\\path\\\\to\\\\mcp-reference.md
    python tools/check_mirror_lag.py --exe C:\\\\path\\\\to\\\\Wc3.Mcp.exe

Exit 0 when the export names the same tools as the exe, 2 when it lags or carries a
tool the exe does not, 1 when the exe or the export cannot be read. A missing export
is exit 1 with the paths it looked in, never a pass.
"""

from __future__ import annotations

import argparse
import json
import os
import re
import sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
if str(HERE) not in sys.path:
    sys.path.insert(0, str(HERE))

from measure_surface import MeasureError, find_exe  # noqa: E402
from surface import read_surface  # noqa: E402

ROOT = HERE.parent.parent.parent
HEADING = re.compile(r"^## (.+?)\s*$", re.M)
DECLARED = re.compile(r"exposes\s+(?P<count>[\d,]+)\s+tool")


class LagError(RuntimeError):
    """The exe or the mirror export cannot be read, so no lag can be stated."""


def find_reference(explicit: str | None) -> Path:
    """The mirror export, from the flag, the environment, or a usual folder."""
    if explicit:
        path = Path(explicit)
        if not path.is_file():
            raise LagError(f"no reference at {path}")
        return path

    from_env = os.environ.get("WC3_MCP_REFERENCE")
    if from_env:
        path = Path(from_env)
        if not path.is_file():
            raise LagError(f"WC3_MCP_REFERENCE points at {path}, which is not a file")
        return path

    candidates = (
        ROOT.parent / "Wc3_CLI" / "docs" / "mcp-reference.md",
        ROOT / "docs" / "mcp-reference.md",
    )
    for candidate in candidates:
        if candidate.is_file():
            return candidate

    looked = "\n  ".join(str(c) for c in candidates)
    raise LagError(
        "no mirror export found. Looked for\n  "
        + looked
        + "\n\nThe export is generated in the wc3ctl repository by `wc3ctl docs mcp`, so this"
        "\ntree holds none of its own. Point at a copy with --reference or WC3_MCP_REFERENCE."
    )


def read_reference(path: Path) -> dict:
    """Every tool heading, the count sentence, and the body of each tool."""
    try:
        text = path.read_text(encoding="utf-8")
    except OSError as exc:
        raise LagError(f"no reference at {path}, {exc}") from exc
    names = [match.group(1).strip() for match in HEADING.finditer(text)]
    if not names:
        raise LagError(f"{path} has no `## tool` headings, so it names no tools")
    declared = None
    match = DECLARED.search(text)
    if match:
        declared = int(match.group("count").replace(",", ""))
    return {
        "names": names,
        "declared": declared,
        "bytes": len(text),
        "path": path,
    }


def compare(exe_names: list[str], reference: dict) -> dict:
    exe_set = set(exe_names)
    ref_set = set(reference["names"])
    missing = sorted(exe_set - ref_set)
    extra = sorted(ref_set - exe_set)
    duplicated = sorted(name for name in ref_set if reference["names"].count(name) > 1)
    return {
        "exe_count": len(exe_names),
        "reference_count": len(reference["names"]),
        "reference_unique": len(ref_set),
        "missing": missing,
        "extra": extra,
        "duplicated": duplicated,
    }


def build_rows(exe_names: list[str], reference: dict, result: dict) -> list[dict]:
    def row(measure, declared, actual, ok, detail="") -> dict:
        return {
            "measure": measure,
            "declared": declared,
            "actual": actual,
            "ok": ok,
            "detail": detail,
        }

    declared = reference["declared"]
    return [
        row(
            "Tools the exe lists against the count the export states",
            declared,
            result["exe_count"],
            declared == result["exe_count"],
            "the export states no count" if declared is None else "",
        ),
        row(
            "Tools the exe lists against the headings the export carries",
            result["reference_count"],
            result["exe_count"],
            result["reference_count"] == result["exe_count"],
        ),
        row(
            "Tools the exe lists and the export omits",
            0,
            len(result["missing"]),
            not result["missing"],
            ", ".join(result["missing"][:12]),
        ),
        row(
            "Tools the export names and the exe does not list",
            0,
            len(result["extra"]),
            not result["extra"],
            ", ".join(result["extra"][:12]),
        ),
        row(
            "Duplicate headings in the export",
            0,
            len(result["duplicated"]),
            not result["duplicated"],
            ", ".join(result["duplicated"][:12]),
        ),
    ]


def render(rows: list[dict], exe: Path, reference: dict) -> str:
    head = f"{'Measure'.ljust(62)}  {'Export'.rjust(7)}  {'Exe'.rjust(6)}  Verdict"
    lines = [head, "-" * len(head)]
    for row in rows:
        declared = "-" if row["declared"] is None else str(row["declared"])
        lines.append(
            f"{row['measure'].ljust(62)}  {declared.rjust(7)}  "
            f"{str(row['actual']).rjust(6)}  {'ok' if row['ok'] else 'LAG'}"
        )
        if row["detail"] and not row["ok"]:
            lines.append(f"{'':62}  {row['detail']}")
    lag = [row for row in rows if not row["ok"]]
    lines.append("")
    if lag:
        lines.append(
            f"{len(lag)} of {len(rows)} comparisons disagree, so the export and the surface are not "
            "the same list."
        )
        lines.append(
            f"  the export names {len(reference['names'])} tools, the exe lists "
            f"{rows[0]['actual']}"
        )
        if rows[2]["actual"]:
            lines.append("  missing from the export, " + ", ".join(rows[2]["detail"].split(", ")))
        if rows[3]["actual"]:
            lines.append("  only in the export, " + ", ".join(rows[3]["detail"].split(", ")))
        difference = len(reference["names"]) - rows[0]["actual"]
        if difference > 0:
            lines.append(f"  the export is ahead of the exe by {difference}")
        elif difference < 0:
            lines.append(f"  the export lags the exe by {-difference}")
        lines.append("  regenerate it in the wc3ctl repository with `wc3ctl docs mcp --force`")
    else:
        lines.append(
            f"All {len(rows)} comparisons agree. The export names {len(reference['names'])} "
            f"tools and the exe lists {rows[0]['actual']}, so the lag is 0."
        )
    lines.append(f"server    {exe}")
    lines.append(f"reference {reference['path']}")
    return "\n".join(lines)


def selftest() -> int:
    """Prove the comparison names a lag, a missing tool and a duplicate."""
    failures: list[str] = []
    exe_names = ["alpha", "beta", "gamma", "delta"]
    stale = {"names": ["alpha", "beta", "gamma", "gamma"], "declared": 3, "bytes": 1, "path": Path("selftest")}
    result = compare(exe_names, stale)
    rows = build_rows(exe_names, stale, result)
    if result["missing"] != ["delta"]:
        failures.append(f"a missing tool was not named, it reported {result['missing']}")
    if result["duplicated"] != ["gamma"]:
        failures.append(f"a duplicated heading was not named, it reported {result['duplicated']}")
    if result["exe_count"] - result["reference_count"] != 0:
        failures.append("the count difference did not follow the headings")
    if all(row["ok"] for row in rows):
        failures.append("a stale export passed every comparison")

    current = {"names": list(exe_names), "declared": 4, "bytes": 1, "path": Path("selftest")}
    result = compare(exe_names, current)
    rows = build_rows(exe_names, current, result)
    if not all(row["ok"] for row in rows):
        failures.append("an export naming every tool did not pass")

    if failures:
        for failure in failures:
            print("selftest failed, " + failure, file=sys.stderr)
        return 1
    print(
        "selftest ok, an export that names every tool passes, and a missing tool, a lagging "
        "count and a duplicate heading are each named"
    )
    return 0


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(
        description="Compare the published exe's tool list with the mirror's generated reference."
    )
    parser.add_argument("--exe", help="path to the published Wc3.Mcp executable")
    parser.add_argument("--reference", help="path to the generated mcp-reference.md")
    parser.add_argument("--timeout", type=float, default=180.0, help="seconds per request")
    parser.add_argument("--json", action="store_true", help="print the comparison as JSON")
    parser.add_argument("--selftest", action="store_true", help="run the built in selftest")
    args = parser.parse_args(argv)

    if args.selftest:
        return selftest()

    try:
        exe = find_exe(args.exe)
        reference_path = find_reference(args.reference)
        reference = read_reference(reference_path)
        surface = read_surface(
            exe, args.timeout, client="check_mirror_lag", require_families=False
        )
    except MeasureError as exc:
        print(str(exc), file=sys.stderr)
        return 1

    exe_names = [tool.get("name") for tool in surface["tools"]]
    result = compare(exe_names, reference)
    rows = build_rows(exe_names, reference, result)
    ok = all(row["ok"] for row in rows)
    code = 0 if ok else 2

    if args.json:
        print(
            json.dumps(
                {
                    "ok": ok,
                    "exit": code,
                    "exe": str(exe),
                    "reference": str(reference_path),
                    "declared": reference["declared"],
                    "exe_count": result["exe_count"],
                    "reference_count": result["reference_count"],
                    "lag": result["exe_count"] - result["reference_count"],
                    "missing": result["missing"],
                    "extra": result["extra"],
                    "duplicated": result["duplicated"],
                    "rows": rows,
                },
                indent=2,
            )
        )
    else:
        print(render(rows, exe, reference))
    return code


if __name__ == "__main__":
    sys.exit(main())
