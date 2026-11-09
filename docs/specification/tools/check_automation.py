"""Run every automation guard this specification names and print one verdict each.

AUT-10 says the changelog is checked the way the reference documents are. This
is the one command that runs the whole set, so a release has a single thing to
call rather than a list to remember. It runs, in order

  * check_versions.py, IDN-02, six places carry one version
  * check_provenance.py, PKG-13, every artifact names commit, digest and tag
  * check_changelog_ids.py, DOC-04 and AUT-02, every note names a known id
  * check_counts.py, the document's own figures against requirements.json
  * measure_surface.py, the body figures against the published server

and prints one verdict line per check, with the check's own closing line as the
detail. Nothing is summarised away, every line names the file that spoke.

    python tools/check_automation.py
    python tools/check_automation.py --json
    python tools/check_automation.py --timeout 600
    python tools/check_automation.py --selftest

Exit 0 when every check passes, 2 when one fails, 1 when one cannot run at all.
The guard scripts own their own exit codes and this command never edits them.
"""

from __future__ import annotations

import argparse
import json
import subprocess
import sys
import tempfile
from pathlib import Path

HERE = Path(__file__).resolve().parent

# The order is the order the release runs them, cheapest first.
CHECKS = (
    ("check_versions.py", "IDN-02"),
    ("check_provenance.py", "PKG-13"),
    ("check_changelog_ids.py", "DOC-04, AUT-02"),
    ("check_counts.py", "section 2 and the README counts"),
    ("measure_surface.py", "sections 3.1, 3.3 and 3.5"),
)


def last_line(text: str) -> str:
    for line in reversed(text.splitlines()):
        stripped = line.strip()
        if stripped:
            return stripped
    return "no output"


def run_one(script: Path, requirement: str, timeout: int) -> dict:
    """Run one guard and turn its exit code into a verdict."""
    if not script.is_file():
        return {
            "check": script.name,
            "requirement": requirement,
            "code": None,
            "verdict": "MISSING",
            "detail": f"no script at {script}",
            "output": "",
        }
    try:
        result = subprocess.run(
            [sys.executable, str(script)],
            capture_output=True,
            text=True,
            timeout=timeout,
            cwd=str(script.parent),
        )
    except subprocess.TimeoutExpired:
        return {
            "check": script.name,
            "requirement": requirement,
            "code": None,
            "verdict": "TIMEOUT",
            "detail": f"no verdict within {timeout} seconds",
            "output": "",
        }
    except OSError as exc:
        return {
            "check": script.name,
            "requirement": requirement,
            "code": None,
            "verdict": "ERROR",
            "detail": f"could not be started, {exc}",
            "output": "",
        }

    combined = (result.stdout or "") + ("\n" + result.stderr if result.stderr else "")
    if result.returncode == 0:
        verdict = "PASS"
    elif result.returncode == 2:
        verdict = "FAIL"
    else:
        verdict = "ERROR"
    detail = last_line(result.stdout or result.stderr or "")
    return {
        "check": script.name,
        "requirement": requirement,
        "code": result.returncode,
        "verdict": verdict,
        "detail": detail,
        "output": combined.strip(),
    }


def run(checks, timeout: int) -> list[dict]:
    return [run_one(HERE / name, requirement, timeout) for name, requirement in checks]


def exit_code(rows: list[dict]) -> int:
    if any(row["verdict"] in ("FAIL", "MISSING") for row in rows):
        return 2
    if any(row["verdict"] in ("ERROR", "TIMEOUT") or row["code"] not in (0, 2) for row in rows):
        return 1
    return 0


def render(rows: list[dict]) -> str:
    widths = {
        "check": max([len("Check")] + [len(row["check"]) for row in rows]),
        "requirement": max([len("Requirement")] + [len(row["requirement"]) for row in rows]),
        "verdict": max([len("Verdict")] + [len(row["verdict"]) for row in rows]),
    }
    head = (
        f"{'Check'.ljust(widths['check'])}  "
        f"{'Requirement'.ljust(widths['requirement'])}  "
        f"{'Verdict'.ljust(widths['verdict'])}  Code  Detail"
    )
    lines = [head, "-" * len(head)]
    for row in rows:
        code = "-" if row["code"] is None else str(row["code"])
        detail = row["detail"]
        if len(detail) > 110:
            detail = detail[:107] + "..."
        lines.append(
            f"{row['check'].ljust(widths['check'])}  "
            f"{row['requirement'].ljust(widths['requirement'])}  "
            f"{row['verdict'].ljust(widths['verdict'])}  {code:<4}  {detail}"
        )
    return "\n".join(lines)


def selftest() -> int:
    """Run three stand in guards and prove a pass, a fail and an error are named."""
    failures: list[str] = []
    with tempfile.TemporaryDirectory(prefix="check-automation-") as folder:
        root = Path(folder)
        (root / "pass.py").write_text("print('all good')\n", encoding="utf-8")
        (root / "fail.py").write_text(
            "import sys\nprint('refused, a version alone is not provenance')\nsys.exit(2)\n",
            encoding="utf-8",
        )
        (root / "error.py").write_text(
            "import sys\nprint('cannot read the tree', file=sys.stderr)\nsys.exit(1)\n",
            encoding="utf-8",
        )

        rows = [
            run_one(root / name, requirement, 60)
            for name, requirement in (
                ("pass.py", "TEST"),
                ("fail.py", "TEST"),
                ("error.py", "TEST"),
                ("missing.py", "TEST"),
            )
        ]
        verdicts = [row["verdict"] for row in rows]
        if verdicts != ["PASS", "FAIL", "ERROR", "MISSING"]:
            failures.append(f"expected PASS, FAIL, ERROR, MISSING and got {verdicts}")
        if rows[1]["detail"] != "refused, a version alone is not provenance":
            failures.append("the failing check's own line was not the detail")
        if exit_code(rows[:1]) != 0:
            failures.append("one pass did not exit 0")
        if exit_code(rows[1:2]) != 2:
            failures.append("a fail did not exit 2")
        if exit_code(rows[2:3]) != 1:
            failures.append("an error did not exit 1")

        clean = [run_one(root / "pass.py", "TEST", 60)]
        text = render(clean)
        if "PASS" not in text or "pass.py" not in text:
            failures.append("the verdict table did not name the check")

    if failures:
        for failure in failures:
            print("selftest failed, " + failure, file=sys.stderr)
        return 1
    print("selftest ok, a pass, a fail, an error and a missing script each get their own verdict")
    return 0


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description="Run every automation guard and print one verdict each.")
    parser.add_argument("--timeout", type=int, default=300, help="seconds allowed for one check")
    parser.add_argument("--json", action="store_true", help="print every result as JSON, output included")
    parser.add_argument("--selftest", action="store_true", help="run the built in selftest")
    args = parser.parse_args(argv)

    if args.selftest:
        return selftest()

    rows = run(CHECKS, args.timeout)
    code = exit_code(rows)

    if args.json:
        print(json.dumps({"ok": code == 0, "exit": code, "checks": rows}, indent=2, ensure_ascii=False))
    else:
        print(render(rows))
        print()
        passed = sum(1 for row in rows if row["verdict"] == "PASS")
        print(f"{passed} of {len(rows)} checks pass")
        for row in rows:
            if row["verdict"] != "PASS" and row["output"]:
                print()
                print(f"{row['check']} said, last lines")
                for line in row["output"].splitlines()[-6:]:
                    print("  " + line)

    return code


if __name__ == "__main__":
    sys.exit(main())
