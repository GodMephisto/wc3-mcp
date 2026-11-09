"""Verify a published binary by starting it and reading what it says.

AUT-08 says a published binary SHALL be verified by starting it and reading its
version and, for the server, its tool count, and the check SHALL fail on a stale
version or timestamp. The release smoke started an exe once. A local publish did
not start it at all, so a folder built before the last source change looked
exactly like a folder built after it.

This starts the artifact, reads the version line it prints, counts the tools the
server lists over stdio, compares the version with the one `Directory.Build.props`
declares, compares the tool count with the section 3.1 figure when the document is
at hand, and compares the timestamp of the exe with the newest source file in the
tree the exe sits in. It writes nothing and it edits no guard.

    python tools/verify_publish.py
    python tools/verify_publish.py --exe C:\\path\\to\\wc3-mcp.exe
    python tools/verify_publish.py --expect-version 0.5.16
    python tools/verify_publish.py --json
    python tools/verify_publish.py --selftest

Exit 0 when every check agrees, 2 when one disagrees, 1 when the artifact cannot
be read or the process cannot be started. A server exe is recognised by its name,
so a CLI exe is verified for its version and its timestamp and no tool count is
claimed for it.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import os
import re
import subprocess
import sys
from datetime import datetime, timezone
from pathlib import Path

HERE = Path(__file__).resolve().parent
if str(HERE) not in sys.path:
    sys.path.insert(0, str(HERE))

# The stdio session and the section 3.1 reader are shared, not copied. They are
# what measure_surface.py uses against the same exe.
from measure_surface import (  # noqa: E402
    MeasureError,
    Session,
    first_int,
    row_text,
    section,
)

REPO = HERE.parent.parent.parent
SPEC = HERE.parent / "wc3-mcp-specification-v1.md"
BUILD_PROPS = "Directory.Build.props"
VERSION = re.compile(r"(\d+\.\d+\.\d+(?:\.\d+)?)")
NO_WINDOW = subprocess.CREATE_NO_WINDOW if os.name == "nt" else 0


class VerifyError(MeasureError):
    """The artifact or its source could not be read, so no verdict is honest."""


def find_artifact(explicit: str | None) -> Path:
    """The published binary, from the flag, the environment, or a usual folder."""
    if explicit:
        path = Path(explicit)
        if not path.is_file():
            raise VerifyError(f"no artifact at {path}")
        return path

    from_env = os.environ.get("WC3_PUBLISH_EXE")
    if from_env:
        path = Path(from_env)
        if not path.is_file():
            raise VerifyError(f"WC3_PUBLISH_EXE points at {path}, which is not a file")
        return path

    names = ("wc3-mcp.exe", "Wc3.Mcp.exe", "wc3-mcp", "Wc3.Mcp")
    candidates = [REPO / "dist-mcp" / name for name in names]
    candidates += [REPO / "dist" / name for name in names]
    for candidate in candidates:
        if candidate.is_file():
            return candidate

    looked = "\n  ".join(str(c) for c in candidates)
    raise VerifyError(
        "no published artifact found. Looked for\n  "
        + looked
        + "\n\nPublish it first, then pass --exe or set WC3_PUBLISH_EXE"
    )


def sha256_of(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for block in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(block)
    return digest.hexdigest()


def repo_of(exe: Path) -> Path:
    """The tree an artifact sits in, so its source can be found beside it."""
    return exe.parent.parent if exe.parent.name in ("dist", "dist-mcp") else exe.parent


def declared_version(repo: Path) -> str | None:
    """The version the source declares, which the tag and the exe have to match."""
    props = repo / BUILD_PROPS
    if not props.is_file():
        return None
    text = props.read_text(encoding="utf-8", errors="replace")
    found = re.search(r"<Version>\s*([^<\s]+)\s*</Version>", text)
    return found.group(1) if found else None


def declared_tools(markdown: Path | None) -> int | None:
    """The tool count the document states in section 3.1, when it can be read."""
    if markdown is None or not markdown.is_file():
        return None
    try:
        text = markdown.read_text(encoding="utf-8")
        return first_int(row_text(section(text, "3.1"), "Tools", "3.1"), "Tools")
    except MeasureError:
        return None


def newest_source(repo: Path) -> tuple[Path, float] | None:
    """The newest file the exe would have been built from, and its timestamp."""
    best: tuple[Path, float] | None = None
    root = repo / "src"
    if root.is_dir():
        for path in root.rglob("*"):
            if path.suffix.lower() not in (".cs", ".csproj"):
                continue
            # obj and bin hold generated files, which no publish waits for.
            if {"obj", "bin"} & {part.lower() for part in path.parts}:
                continue
            if not path.is_file():
                continue
            stamp = path.stat().st_mtime
            if best is None or stamp > best[1]:
                best = (path, stamp)
    props = repo / BUILD_PROPS
    if props.is_file():
        stamp = props.stat().st_mtime
        if best is None or stamp > best[1]:
            best = (props, stamp)
    return best


def probe_version(exe: Path, timeout: float) -> str:
    """Start the exe with --version and read the version it prints."""
    try:
        done = subprocess.run(
            [str(exe), "--version"],
            capture_output=True,
            text=True,
            encoding="utf-8",
            errors="replace",
            timeout=timeout,
            creationflags=NO_WINDOW,
        )
    except subprocess.TimeoutExpired as exc:
        raise VerifyError(f"{exe.name} did not answer --version within {timeout} seconds") from exc
    except OSError as exc:
        raise VerifyError(f"{exe.name} did not start, {exc}") from exc

    text = (done.stdout or "") + "\n" + (done.stderr or "")
    found = VERSION.search(text)
    if not found:
        first = text.strip().splitlines()[0] if text.strip() else "nothing"
        raise VerifyError(f"{exe.name} printed no version on --version, it said {first!r}")
    return found.group(1)


def count_tools(exe: Path, timeout: float) -> int:
    """How many tools the server lists, paginated the way measure_surface reads it."""
    try:
        session = Session(exe, timeout)
    except MeasureError as exc:
        raise VerifyError(f"{exe.name} did not start as a server, {exc}") from exc
    try:
        session.request(
            "initialize",
            {
                "protocolVersion": "2025-06-18",
                "capabilities": {},
                "clientInfo": {"name": "verify_publish", "version": "1"},
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
    except MeasureError as exc:
        raise VerifyError(f"{exe.name} did not list its tools, {exc}") from exc
    finally:
        session.close()


def is_server(exe: Path) -> bool:
    return "mcp" in exe.stem.lower()


def iso(stamp: float) -> str:
    return datetime.fromtimestamp(stamp, timezone.utc).strftime("%Y-%m-%d %H:%M UTC")


def judge(facts: dict) -> list[dict]:
    """One row per check, with what the source says beside what the exe said."""

    def row(check: str, expected, found, ok: bool, detail: str = "") -> dict:
        return {
            "check": check,
            "expected": expected,
            "found": found,
            "ok": ok,
            "detail": detail,
        }

    rows: list[dict] = []

    declared = facts.get("declared_version")
    printed = facts.get("version")
    if declared and printed:
        agree = declared == printed
        rows.append(
            row(
                "Version",
                declared,
                printed,
                agree,
                "" if agree else f"the exe prints {printed} and the source declares {declared}",
            )
        )
    else:
        rows.append(
            row(
                "Version",
                declared or "no source beside it",
                printed or "not read",
                False,
                "a version that was not compared with its source proves nothing",
            )
        )

    tools = facts.get("tools")
    if tools is not None:
        expected = facts.get("declared_tools")
        if expected is None:
            rows.append(
                row("Tool count", "not stated by the document", str(tools), True,
                    "read from tools/list, with no figure to compare it with")
            )
        else:
            agree = tools == expected
            rows.append(
                row(
                    "Tool count",
                    str(expected),
                    str(tools),
                    agree,
                    "" if agree else f"the server lists {tools} and section 3.1 states {expected}",
                )
            )

    newest = facts.get("newest")
    if newest is not None:
        path, stamp = newest
        built = facts["mtime"]
        fresh = built >= stamp
        rows.append(
            row(
                "Timestamp",
                f"{path.name} at {iso(stamp)}",
                f"{facts.get('name')} at {iso(built)}",
                fresh,
                ""
                if fresh
                else f"{path.name} is newer than the exe, so the folder was published before the source",
            )
        )
    return rows


def exit_code(rows: list[dict]) -> int:
    return 0 if all(r["ok"] for r in rows) else 2


def render(rows: list[dict], artifact: dict) -> str:
    head = (
        f"Artifact  {artifact['path']}\n"
        f"Bytes     {artifact['bytes']:,}\n"
        f"SHA-256   {artifact['sha256']}\n"
        f"Built     {iso(artifact['mtime'])}\n"
    )
    widths = {
        "check": max([len("Check")] + [len(r["check"]) for r in rows]),
        "expected": max([len("Expected")] + [len(str(r["expected"])) for r in rows]),
        "found": max([len("Found")] + [len(str(r["found"])) for r in rows]),
    }
    lines = [
        f"{'Check'.ljust(widths['check'])}  {'Expected'.ljust(widths['expected'])}  "
        f"{'Found'.ljust(widths['found'])}  Verdict",
        "-" * (sum(widths.values()) + 16),
    ]
    for r in rows:
        lines.append(
            f"{r['check'].ljust(widths['check'])}  "
            f"{str(r['expected']).ljust(widths['expected'])}  "
            f"{str(r['found']).ljust(widths['found'])}  "
            f"{'ok' if r['ok'] else 'DRIFT'}"
        )
    drifted = [r for r in rows if not r["ok"]]
    lines.append("")
    if drifted:
        lines.append(
            f"{len(drifted)} of {len(rows)} checks disagree, so the published folder is not "
            "what the source describes"
        )
        for r in drifted:
            if r["detail"]:
                lines.append("  " + r["detail"])
        lines.append("Publish again, then run this command against the new artifact")
    else:
        lines.append(
            f"All {len(rows)} checks agree, the artifact was started and it matches its source"
        )
    return head + "\n" + "\n".join(lines)


def selftest() -> int:
    """Prove the verdicts bite, and prove a real process can be started and read."""
    failures: list[str] = []
    base = {
        "name": "wc3-mcp.exe",
        "version": "0.5.16",
        "declared_version": "0.5.16",
        "tools": 275,
        "declared_tools": 275,
        "mtime": 2_000.0,
        "newest": (Path("src/A.cs"), 1_000.0),
    }

    if exit_code(judge(dict(base))) != 0:
        failures.append("a matching artifact did not pass")

    stale_version = dict(base, version="0.5.12")
    rows = judge(stale_version)
    if exit_code(rows) != 2:
        failures.append("a stale version did not fail")
    if not any("0.5.16" in r["detail"] and "0.5.12" in r["detail"] for r in rows):
        failures.append("the stale version was not named against the declared one")

    stale_stamp = dict(base, mtime=500.0)
    rows = judge(stale_stamp)
    if exit_code(rows) != 2:
        failures.append("a stale timestamp did not fail")
    if not any("newer than the exe" in r["detail"] for r in rows):
        failures.append("the newer source file was not named")

    short_count = dict(base, tools=267)
    rows = judge(short_count)
    if exit_code(rows) != 2:
        failures.append("a tool count that disagrees with the document did not fail")

    no_source = dict(base)
    no_source.pop("declared_version")
    if exit_code(judge(no_source)) != 2:
        failures.append("an artifact with no source beside it passed")

    # The probe runs a real process and reads what it prints, which is the part of
    # this tool a canned fact cannot prove.
    try:
        printed = probe_version(Path(sys.executable), 60)
    except VerifyError as exc:
        failures.append(f"the version probe could not start a real process, {exc}")
    else:
        if not VERSION.fullmatch(printed):
            failures.append(f"the version probe read {printed!r} out of a version line")

    if failures:
        for failure in failures:
            print("selftest failed, " + failure, file=sys.stderr)
        return 1
    print(
        "selftest ok, a match passes, a stale version, a stale timestamp and a short "
        "tool count each fail, and the version probe read a real process"
    )
    return 0


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(
        description="Start a published binary and check its version, tools and timestamp."
    )
    parser.add_argument("--exe", help="path to the published artifact")
    parser.add_argument("--expect-version", help="the version the artifact must print")
    parser.add_argument("--markdown", default=str(SPEC), help="the specification markdown")
    parser.add_argument("--timeout", type=float, default=120.0, help="seconds allowed to start and answer")
    parser.add_argument("--no-tools", action="store_true", help="do not count the tools it lists")
    parser.add_argument("--json", action="store_true", help="print the result as JSON")
    parser.add_argument("--selftest", action="store_true", help="run the built in selftest")
    args = parser.parse_args(argv)

    if args.selftest:
        return selftest()

    try:
        exe = find_artifact(args.exe)
        repo = repo_of(exe)
        artifact = {
            "path": str(exe),
            "bytes": exe.stat().st_size,
            "sha256": sha256_of(exe),
            "mtime": exe.stat().st_mtime,
        }
        declared = args.expect_version or declared_version(repo)
        if not declared:
            raise VerifyError(
                f"no {BUILD_PROPS} beside {exe} and no --expect-version, so the version "
                "the exe prints cannot be judged"
            )
        facts = {
            "name": exe.name,
            "version": probe_version(exe, args.timeout),
            "declared_version": declared,
            "declared_tools": declared_tools(Path(args.markdown)),
            "mtime": artifact["mtime"],
            "newest": newest_source(repo),
            "tools": None,
        }
        if is_server(exe) and not args.no_tools:
            facts["tools"] = count_tools(exe, args.timeout)
        rows = judge(facts)
    except (MeasureError, OSError) as exc:
        print(str(exc), file=sys.stderr)
        return 1

    code = exit_code(rows)
    if args.json:
        print(
            json.dumps(
                {
                    "ok": code == 0,
                    "exit": code,
                    "artifact": artifact,
                    "version": facts["version"],
                    "declared_version": facts["declared_version"],
                    "tools": facts["tools"],
                    "declared_tools": facts["declared_tools"],
                    "checks": rows,
                },
                indent=2,
                ensure_ascii=False,
            )
        )
    else:
        print(render(rows, artifact))
    return code


if __name__ == "__main__":
    sys.exit(main())
