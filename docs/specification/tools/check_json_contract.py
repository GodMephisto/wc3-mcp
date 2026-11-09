"""Run every script in this folder with `--json` and report the object it answers.

AUT-07 says every automation command SHALL answer `--json` with a stable object,
so an agent can drive the same loop a person drives. This runs each script with
the flag, reads stdout as one JSON object, and names the keys it must carry. A
script that prints anything but one object, or an object missing a key it
promised, is a gap row with its name on it.

    python tools/check_json_contract.py
    python tools/check_json_contract.py --json
    python tools/check_json_contract.py --only check_versions.py
    python tools/check_json_contract.py --selftest

Exit 0 when every script answers one object with its declared keys, 2 when one
does not, 1 when this check cannot read the folder.

The object's own verdict is not what this judges. A guard that finds drift exits
2 and still answers with an object, and that object is what an agent reads. The
exit code is reported beside the verdict, not folded into it.
"""

from __future__ import annotations

import argparse
import json
import os
import subprocess
import sys
import tempfile
from pathlib import Path

HERE = Path(__file__).resolve().parent
DOCS = HERE.parent
NO_WINDOW = subprocess.CREATE_NO_WINDOW if os.name == "nt" else 0

# One row per script. `argv` is what the flag is run with, `{temp}` is a scratch
# folder this check fills before the run, and `keys` is the object it must hold.
# `build_pdf.py` takes no flag, which is the row AUT-07 is about.
CONTRACT = (
    {
        "script": "check_automation.py",
        "argv": ["--json"],
        "keys": ["ok", "exit", "checks"],
        "timeout": 900,
    },
    {
        "script": "check_changelog_ids.py",
        "argv": ["--json"],
        "keys": ["ok", "changelog", "entries", "unknown"],
        "timeout": 180,
    },
    {
        "script": "check_counts.py",
        "argv": ["--json"],
        "keys": ["ok", "drifted", "exe", "rows"],
        "timeout": 300,
    },
    {
        "script": "check_provenance.py",
        "argv": ["--json"],
        "keys": ["ok", "tag", "commit"],
        "timeout": 180,
    },
    {
        "script": "check_versions.py",
        "argv": ["--json"],
        "keys": ["ok", "reference", "rows"],
        "timeout": 180,
    },
    {
        "script": "measure_surface.py",
        "argv": ["--json"],
        "keys": ["ok", "drifted", "rows"],
        "timeout": 600,
    },
    {
        "script": "release_record.py",
        "argv": [
            "write",
            "--json",
            "--version",
            "0.0.0",
            "--commit",
            "0000000000000000000000000000000000000000",
            "--artifact",
            "{temp}/artifact.bin",
            "--root",
            "{temp}",
            "--out",
            "{temp}/releases",
        ],
        "keys": ["record", "version", "tag", "commit", "artifacts"],
        "timeout": 180,
    },
    {
        "script": "sync_requirements.py",
        "argv": ["--json", "--markdown", "{temp}/spec.md", "--requirements", "{temp}/requirements.json"],
        "keys": ["ok", "requirements", "groups", "gaps", "status"],
        "timeout": 180,
    },
    {
        "script": "verify_publish.py",
        "argv": ["--json"],
        "keys": ["ok", "exit", "artifact", "checks"],
        "timeout": 300,
    },
    {
        "script": "check_json_contract.py",
        "argv": ["--json", "--only", "check_versions.py"],
        "keys": ["ok", "exit", "answered", "scripts", "rows"],
        "timeout": 180,
    },
    {
        "script": "build_pdf.py",
        "argv": None,
        "keys": [],
        "timeout": 0,
        "why": "takes no --json flag, and this check does not rebuild the PDF",
    },
    {
        "script": "fuzz_corpus.py",
        "argv": ["--json", "--no-record", "--state", "{temp}/state"],
        "keys": ["ok", "exit", "summary", "rows"],
        "timeout": 300,
    },
)


def prepare(temp: Path) -> None:
    """The files the probes read, written before any probe runs."""
    (temp / "artifact.bin").write_bytes(b"a stand in artifact\n")
    spec = DOCS / "wc3-mcp-specification-v1.md"
    data = DOCS / "requirements.json"
    if spec.is_file():
        (temp / "spec.md").write_text(spec.read_text(encoding="utf-8"), encoding="utf-8")
    if data.is_file():
        (temp / "requirements.json").write_text(data.read_text(encoding="utf-8"), encoding="utf-8")
    (temp / "state").mkdir(exist_ok=True)


def probe(script: Path, argv: list[str], keys: list[str], timeout: float, temp: Path) -> dict:
    """Run one script, read its stdout as one object, and judge the keys it promised."""
    row = {
        "script": script.name,
        "probe": " ".join(a if not a.startswith(str(temp)) else a.replace(str(temp), "<temp>") for a in argv),
        "code": None,
        "object": False,
        "found": [],
        "missing": list(keys),
        "verdict": "FAIL",
        "detail": "",
    }
    if not script.is_file():
        row["verdict"] = "MISSING"
        row["detail"] = f"no script at {script}"
        return row

    resolved = [a.replace("{temp}", str(temp)) for a in argv]
    try:
        done = subprocess.run(
            [sys.executable, str(script), *resolved],
            capture_output=True,
            text=True,
            encoding="utf-8",
            errors="replace",
            timeout=timeout,
            cwd=str(script.parent),
            creationflags=NO_WINDOW,
        )
    except subprocess.TimeoutExpired:
        row["verdict"] = "TIMEOUT"
        row["detail"] = f"no answer within {timeout} seconds"
        return row
    except OSError as exc:
        row["verdict"] = "ERROR"
        row["detail"] = f"could not be started, {exc}"
        return row

    row["code"] = done.returncode
    text = (done.stdout or "").strip()
    try:
        document = json.loads(text)
    except ValueError as exc:
        detail = (done.stderr or done.stdout or "").strip().splitlines()
        row["detail"] = f"stdout is not one JSON object, {exc}. It said {detail[-1] if detail else 'nothing'}"
        return row
    if not isinstance(document, dict):
        row["detail"] = f"stdout is a {type(document).__name__}, not an object"
        return row

    row["object"] = True
    row["found"] = sorted(document)
    row["missing"] = [key for key in keys if key not in document]
    if row["missing"]:
        row["detail"] = "the object carries no " + ", ".join(row["missing"])
        return row
    row["verdict"] = "ok"
    row["detail"] = "one object, " + ", ".join(keys)
    return row


def run(contract, temp: Path, only: str | None = None) -> list[dict]:
    rows = []
    for entry in contract:
        if only and entry["script"] != only:
            continue
        if entry["argv"] is None:
            rows.append(
                {
                    "script": entry["script"],
                    "probe": "none",
                    "code": None,
                    "object": False,
                    "found": [],
                    "missing": list(entry.get("keys") or []),
                    "verdict": "NO FLAG",
                    "detail": entry.get("why", "takes no --json flag"),
                }
            )
            continue
        rows.append(probe(HERE / entry["script"], entry["argv"], entry["keys"], entry["timeout"], temp))
    return rows


def exit_code(rows: list[dict]) -> int:
    if not rows:
        return 1
    answered = [row for row in rows if row["verdict"] == "ok"]
    if len(answered) == len(rows):
        return 0
    if not answered and any(row["verdict"] in ("MISSING", "ERROR", "TIMEOUT") for row in rows):
        # Nothing answered and one probe could not even start, so this check
        # could not do its job rather than finding a script that answers nothing.
        return 1
    return 2


def render(rows: list[dict]) -> str:
    widths = {
        "script": max([len("Script")] + [len(r["script"]) for r in rows]),
        "probe": max([len("Probe")] + [len(r["probe"]) for r in rows]),
        "code": len("Code"),
        "verdict": max([len("Verdict")] + [len(r["verdict"]) for r in rows]),
    }
    head = (
        f"{'Script'.ljust(widths['script'])}  {'Probe'.ljust(widths['probe'])}  "
        f"{'Code'.rjust(widths['code'])}  {'Verdict'.ljust(widths['verdict'])}  Keys"
    )
    lines = [head, "-" * len(head)]
    for row in rows:
        code = "-" if row["code"] is None else str(row["code"])
        lines.append(
            f"{row['script'].ljust(widths['script'])}  {row['probe'].ljust(widths['probe'])}  "
            f"{code.rjust(widths['code'])}  {row['verdict'].ljust(widths['verdict'])}  "
            f"{','.join(row['found']) if row['found'] else row['detail']}"
        )
    answered = [r for r in rows if r["verdict"] == "ok"]
    lines.append("")
    lines.append(f"{len(answered)} of {len(rows)} scripts answer --json with one object carrying the keys they promise")
    for row in rows:
        if row["verdict"] != "ok":
            lines.append(f"  {row['script']}  {row['verdict']}  {row['detail']}")
    return "\n".join(lines)


def selftest() -> int:
    """Prove an object, a missing key, prose where JSON belongs and a missing script are told apart."""
    import contextlib
    import io

    failures: list[str] = []
    with tempfile.TemporaryDirectory(prefix="json-contract-") as folder:
        root = Path(folder)
        (root / "good.py").write_text('import json\nprint(json.dumps({"ok": True, "rows": []}))\n', encoding="utf-8")
        (root / "prose.py").write_text("print('all good, no json here')\n", encoding="utf-8")
        (root / "short.py").write_text('import json\nprint(json.dumps({"ok": True}))\n', encoding="utf-8")
        (root / "list.py").write_text('import json\nprint(json.dumps([1, 2]))\n', encoding="utf-8")

        good = probe(root / "good.py", [], ["ok", "rows"], 60, root)
        if good["verdict"] != "ok":
            failures.append(f"an object with its keys was not ok, {good['detail']}")
        prose = probe(root / "prose.py", [], ["ok"], 60, root)
        if prose["verdict"] != "FAIL" or "not one JSON object" not in prose["detail"]:
            failures.append("prose where an object belongs was not named as such")
        short = probe(root / "short.py", [], ["ok", "rows"], 60, root)
        if short["verdict"] != "FAIL" or short["missing"] != ["rows"]:
            failures.append(f"a missing key was not named, it reported {short['missing']}")
        listed = probe(root / "list.py", [], ["ok"], 60, root)
        if listed["verdict"] != "FAIL" or "not an object" not in listed["detail"]:
            failures.append("a JSON list was accepted as an object")
        absent = probe(root / "nowhere.py", [], ["ok"], 60, root)
        if absent["verdict"] != "MISSING":
            failures.append("a script that is not there was not named")

        if exit_code([good]) != 0:
            failures.append("one answering script did not exit 0")
        if exit_code([good, prose]) != 2:
            failures.append("a script that answers nothing did not exit 2")

        text = render([good, prose])
        if "good.py" not in text or "1 of 2" not in text:
            failures.append("the table did not count the scripts that answer")

    if failures:
        for failure in failures:
            print("selftest failed, " + failure, file=sys.stderr)
        return 1
    print(
        "selftest ok, an object with its keys passes, prose, a bare list, a missing key and a "
        "missing script each fail with the reason named"
    )
    return 0


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description="Check that every script answers --json with one object.")
    parser.add_argument("--only", help="run one script by name")
    parser.add_argument("--json", action="store_true", help="print the result as JSON")
    parser.add_argument("--selftest", action="store_true", help="run the built in selftest")
    args = parser.parse_args(argv)

    if args.selftest:
        return selftest()

    with tempfile.TemporaryDirectory(prefix="json-contract-run-") as folder:
        temp = Path(folder)
        prepare(temp)
        rows = run(CONTRACT, temp, args.only)

    code = exit_code(rows)
    if args.json:
        print(
            json.dumps(
                {
                    "ok": code == 0,
                    "exit": code,
                    "answered": sum(1 for r in rows if r["verdict"] == "ok"),
                    "scripts": len(rows),
                    "rows": rows,
                },
                indent=2,
            )
        )
    else:
        print(render(rows))
    return code


if __name__ == "__main__":
    sys.exit(main())
