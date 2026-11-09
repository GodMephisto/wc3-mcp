"""Check that every release note names requirement IDs this specification holds.

DOC-04 says every claim in a release note names the requirement it satisfies or
the defect it closes. Section 15.1.1 fixes the form on a line of its own,
`Requirement, SAF-05. Defect, none.` AUT-02 says the changelog carries the
requirement IDs a commit names, so the same evidence serves both.

This reads every markdown file in the changelog folder, takes each
requirement-shaped ID it finds, and asserts the ID exists in
`requirements.json`. An entry that names no requirement at all is a warning,
not a failure, because a note can legitimately close a defect instead, but a
warning is printed so the count is visible.

    python tools/check_changelog_ids.py
    python tools/check_changelog_ids.py --changelog C:\\path\\to\\changelog
    python tools/check_changelog_ids.py --json
    python tools/check_changelog_ids.py --selftest

Exit 0 when every named ID exists, 2 when one does not, 1 when the changelog
folder or requirements.json cannot be read. A warning never changes the exit
code.
"""

from __future__ import annotations

import argparse
import json
import re
import sys
import tempfile
from pathlib import Path

HERE = Path(__file__).resolve().parent
REQUIREMENTS = HERE.parent / "requirements.json"
DEFAULT_CHANGELOG = Path(r"C:\playground\Programming\Wc3_CLI\changelog")

# Section 15.1.1 fixes this line. Requirement first, then the defect, each
# ending in a full stop.
CLAIM_RE = re.compile(r"^\s*(?:[-*+]\s+)?Requirements?\s*[,:]\s*(?P<body>.+?)\s*$", re.IGNORECASE)
DEFECT_SPLIT = re.compile(r"\bDefect\b", re.IGNORECASE)
ID_RE = re.compile(r"\b([A-Z]{2,4}-\d{2,3})\b")
NONE_RE = re.compile(r"^\s*(?:none|no requirement)\b", re.IGNORECASE)


class ChangelogError(RuntimeError):
    """A file that cannot be read at all."""


def load_ids(path: Path) -> set[str]:
    try:
        document = json.loads(path.read_text(encoding="utf-8-sig"))
    except OSError as exc:
        raise ChangelogError(f"{path} could not be read, {exc}") from exc
    except ValueError as exc:
        raise ChangelogError(f"{path} is not valid JSON, {exc}") from exc
    requirements = document.get("requirements") if isinstance(document, dict) else None
    if not isinstance(requirements, list):
        raise ChangelogError(f"{path} holds no requirements list")
    ids = {str(item.get("id")).strip() for item in requirements if isinstance(item, dict) and item.get("id")}
    if not ids:
        raise ChangelogError(f"{path} holds no requirement id")
    return ids


def claim_line(text: str) -> str | None:
    """The section 15.1.1 requirement line, if the note carries one."""
    for line in text.splitlines():
        match = CLAIM_RE.match(line)
        if match:
            return match.group("body")
    return None


def names_none(body: str) -> bool:
    """True when the requirement half of the line says none."""
    head = DEFECT_SPLIT.split(body, maxsplit=1)[0]
    return bool(NONE_RE.match(head.strip())) and not ID_RE.search(head)


def scan(folder: Path, known: set[str]) -> dict:
    """One record per note, with its IDs and every one that is unknown."""
    if not folder.is_dir():
        raise ChangelogError(f"no changelog folder at {folder}")
    notes = sorted(path for path in folder.iterdir() if path.is_file() and path.suffix.lower() == ".md")
    if not notes:
        raise ChangelogError(f"no release note in {folder}")

    entries = []
    unknown: list[tuple[str, str]] = []
    for note in notes:
        try:
            text = note.read_text(encoding="utf-8-sig", errors="replace")
        except OSError as exc:
            raise ChangelogError(f"{note} could not be read, {exc}") from exc
        ids = sorted(set(ID_RE.findall(text)))
        for identifier in ids:
            if identifier not in known:
                unknown.append((note.name, identifier))
        body = claim_line(text)
        entries.append(
            {
                "file": note.name,
                "ids": ids,
                "claim_line": body is not None,
                "names_none": (body is not None and names_none(body)) or not ids,
                "unknown": [identifier for identifier in ids if identifier not in known],
            }
        )
    return {"entries": entries, "unknown": unknown}


def selftest() -> int:
    """Build a changelog and a requirements file, then prove both verdicts."""
    failures: list[str] = []
    with tempfile.TemporaryDirectory(prefix="check-changelog-") as folder:
        root = Path(folder)
        changelog = root / "changelog"
        changelog.mkdir()
        requirements = root / "requirements.json"
        requirements.write_text(
            json.dumps({"requirements": [{"id": "SAF-05"}, {"id": "IDN-02"}]}),
            encoding="utf-8",
        )
        (changelog / "0.5.0.md").write_text(
            "# 0.5.0\n\nRequirement, SAF-05, IDN-02. Defect, none.\n", encoding="utf-8"
        )
        (changelog / "0.5.1.md").write_text(
            "# 0.5.1\n\nA fix with no requirement line at all.\n", encoding="utf-8"
        )
        (changelog / "0.5.2.md").write_text(
            "# 0.5.2\n\nRequirement, ZZZ-99. Defect, none.\n", encoding="utf-8"
        )

        known = load_ids(requirements)
        result = scan(changelog, known)
        if len(result["entries"]) != 3:
            failures.append(f"expected 3 entries, got {len(result['entries'])}")
        if [item for item in result["unknown"] if item[1] != "ZZZ-99"]:
            failures.append("a known id was reported unknown")
        if ("0.5.2.md", "ZZZ-99") not in result["unknown"]:
            failures.append("an unknown id was not reported")
        silent = [item["file"] for item in result["entries"] if item["names_none"]]
        if "0.5.1.md" not in silent:
            failures.append("an entry naming nothing was not warned")
        if "0.5.0.md" in silent:
            failures.append("an entry naming ids was reported as naming none")

        (changelog / "0.5.2.md").write_text(
            "# 0.5.2\n\nRequirement, IDN-02. Defect, none.\n", encoding="utf-8"
        )
        if scan(changelog, known)["unknown"]:
            failures.append("a clean changelog still reported an unknown id")

    if failures:
        for failure in failures:
            print("selftest failed, " + failure, file=sys.stderr)
        return 1
    print("selftest ok, a known id passes, an unknown id fails and an entry naming none is warned")
    return 0


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description="Assert every release note names a known requirement id.")
    parser.add_argument("--changelog", default=str(DEFAULT_CHANGELOG), help="the release note folder")
    parser.add_argument("--requirements", default=str(REQUIREMENTS), help="requirements.json to read")
    parser.add_argument("--json", action="store_true", help="print the result as JSON")
    parser.add_argument("--selftest", action="store_true", help="run the built in selftest")
    args = parser.parse_args(argv)

    if args.selftest:
        return selftest()

    try:
        known = load_ids(Path(args.requirements))
        result = scan(Path(args.changelog), known)
    except ChangelogError as exc:
        print(str(exc), file=sys.stderr)
        return 1

    entries = result["entries"]
    naming = [item for item in entries if item["ids"]]
    silent = [item for item in entries if not item["ids"]]
    unknown = result["unknown"]

    if args.json:
        print(
            json.dumps(
                {
                    "ok": not unknown,
                    "changelog": args.changelog,
                    "requirements": args.requirements,
                    "entries": len(entries),
                    "naming_an_id": len(naming),
                    "naming_none": len(silent),
                    "unknown": [{"file": f, "id": i} for f, i in unknown],
                    "detail": entries,
                },
                indent=2,
                ensure_ascii=False,
            )
        )
    else:
        print(f"changelog {args.changelog}")
        print(f"requirements {args.requirements}")
        print(f"entries {len(entries)}")
        print(f"entries naming a requirement id {len(naming)}")
        print(f"entries naming none {len(silent)}")
        for item in silent:
            print(f"  warn {item['file']} names no requirement id")
        if unknown:
            print(f"unknown ids {len(unknown)}")
            for name, identifier in unknown:
                print(f"  {name} names {identifier}, which requirements.json does not hold")
        else:
            print("unknown ids 0")
        print(
            f"entries {len(entries)}, naming an id {len(naming)}, "
            f"naming none {len(silent)}, unknown {len(unknown)}"
        )

    return 2 if unknown else 0


if __name__ == "__main__":
    sys.exit(main())
