"""Keep requirements.json and the Status column in the specification in step.

The JSON is the source of status and test names. The command parses the requirement
tables and the gap table from the markdown, merges them with what is already in the
JSON, writes the JSON, then rebuilds the Status column to match.

    python tools/sync_requirements.py
    python tools/sync_requirements.py --json
    python tools/sync_requirements.py --selftest

A new requirement starts as "gap" when a row in section 10 names it, and
"unassessed" otherwise. To mark one met, set its status and test in the JSON and run
the command again.

This is the only writer of requirements.json and of the Status column, so a second
process editing either by hand is a second writer. Both files are written to a
temporary name beside them and renamed into place, so a reader running at the same
time sees the old file whole or the new file whole, never half of either.

One update is two files, so it is guarded three ways. A lock beside the JSON is
created with O_EXCL and names its pid, a token and an expiry, so a second writer is
refused rather than interleaved and a lock left by a dead or expired owner is
reclaimed. The bytes of both inputs are kept from the read and compared again before
either file is replaced, so a write over a file that moved while this ran is refused
instead of losing that change. If the markdown cannot be replaced after the JSON was,
the JSON is restored from the bytes that were read, so a failure leaves the previous
state rather than a half updated one.
"""

from __future__ import annotations

import argparse
import json
import os
import re
import secrets
import subprocess
import sys
import tempfile
import time
from pathlib import Path

HERE = Path(__file__).resolve().parent
SPEC = HERE.parent / "wc3-mcp-specification-v1.md"
DATA = HERE.parent / "requirements.json"
ID = re.compile(r"^[A-Z]{2,4}-\d{2,3}$")

# A lock older than this is taken to be abandoned even when its pid still answers.
LOCK_SECONDS = 600
LOCK_SUFFIX = ".lock"


class SyncError(RuntimeError):
    """Another writer holds the state, or changed it under this run, so nothing was written."""


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


def write_bytes_atomic(path: Path, payload: bytes) -> None:
    """Restore exact bytes, for the rollback, without a decode round trip."""
    temporary = path.with_name(path.name + ".tmp-" + str(os.getpid()))
    temporary.write_bytes(payload)
    os.replace(temporary, path)


def lock_path(data: Path) -> Path:
    return data.with_name(data.name + LOCK_SUFFIX)


def read_lock(path: Path) -> dict | None:
    try:
        document = json.loads(path.read_text(encoding="utf-8"))
    except (OSError, ValueError):
        return None
    return document if isinstance(document, dict) else None


def pid_alive(pid: int) -> bool:
    """True when the process that wrote a lock is still there."""
    if not isinstance(pid, int) or pid <= 0:
        return False
    if os.name == "nt":
        import ctypes

        kernel32 = ctypes.windll.kernel32
        handle = kernel32.OpenProcess(0x1000, False, pid)  # query limited information
        if not handle:
            return False
        kernel32.CloseHandle(handle)
        return True
    try:
        os.kill(pid, 0)
    except OSError:
        return False
    return True


def lock_is_stale(held: dict | None) -> bool:
    """A lock is stale when its expiry passed or its owner is gone. Both are checked."""
    if not held:
        return True
    expires = held.get("expires")
    if not isinstance(expires, (int, float)) or time.time() > expires:
        return True
    pid = held.get("pid")
    return not pid_alive(pid if isinstance(pid, int) else 0)


def acquire(data: Path) -> dict:
    """Take the lock atomically, with the owner written before any work, or refuse."""
    path = lock_path(data)
    for _ in range(2):
        token = secrets.token_hex(8)
        record = {"pid": os.getpid(), "token": token, "expires": int(time.time()) + LOCK_SECONDS}
        try:
            handle = os.open(path, os.O_CREAT | os.O_EXCL | os.O_WRONLY, 0o644)
        except FileExistsError:
            held = read_lock(path)
            if not lock_is_stale(held):
                owner = held.get("pid") if held else "unknown"
                raise SyncError(
                    f"another writer holds {path} with pid {owner}, so nothing was "
                    "written. Run it again when that one has finished"
                )
            # Reclaiming moves the stale file to a name only this process uses, so two
            # reclaimers cannot both take the lock and neither can delete the other's record.
            graveyard = path.with_name(f"{path.name}.stale-{os.getpid()}-{token}")
            try:
                os.replace(path, graveyard)
            except FileNotFoundError:
                continue
            graveyard.unlink(missing_ok=True)
            continue
        try:
            with os.fdopen(handle, "w", encoding="utf-8", newline="\n") as stream:
                json.dump(record, stream)
                stream.flush()
                os.fsync(stream.fileno())
        except OSError:
            path.unlink(missing_ok=True)
            raise
        return record
    raise SyncError(f"{path} could not be taken, another writer kept reclaiming it")


def release(data: Path, record: dict) -> None:
    """Drop the lock only when it is still the one this run wrote."""
    path = lock_path(data)
    held = read_lock(path)
    if held and held.get("token") == record.get("token"):
        path.unlink(missing_ok=True)


def commit(
    markdown: Path,
    data: Path,
    md_before: bytes,
    data_before: bytes | None,
    md_after: str,
    json_after: str,
) -> None:
    """Replace both files, refusing when either moved since it was read."""
    if markdown.read_bytes() != md_before:
        raise SyncError(f"{markdown} changed while this ran, so nothing was written. Run it again")
    if (data.read_bytes() if data.exists() else None) != data_before:
        raise SyncError(f"{data} changed while this ran, so nothing was written. Run it again")
    write_atomic(data, json_after)
    try:
        write_atomic(markdown, md_after)
    except OSError:
        if data_before is None:
            data.unlink(missing_ok=True)
        else:
            write_bytes_atomic(data, data_before)
        raise


def sync(markdown: Path, data_path: Path) -> dict:
    """Read both files, merge, and write them back under the lock."""
    md_before = markdown.read_bytes()
    data_before = data_path.read_bytes() if data_path.exists() else None
    md = md_before.decode("utf-8")
    old = {}
    if data_before is not None:
        old = {r["id"]: r for r in json.loads(data_before.decode("utf-8"))["requirements"]}

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
    json_after = json.dumps(data, indent=2, ensure_ascii=False) + "\n"

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
    md_after = md[:start] + "\n".join(out) + md[end:]

    commit(markdown, data_path, md_before, data_before, md_after, json_after)

    counts = {}
    for r in reqs.values():
        counts[r["status"]] = counts.get(r["status"], 0) + 1
    return {
        "ok": True,
        "requirements": len(reqs),
        "groups": len({r["group"] for r in reqs.values()}),
        "gaps": len(gaps),
        "status": counts,
        "markdown": markdown,
        "requirements_json": data_path,
    }


def selftest() -> int:
    """Two writers, a stale lock, a lock still held, and a write over a moved file."""
    failures: list[str] = []
    with tempfile.TemporaryDirectory(prefix="sync-requirements-") as folder:
        root = Path(folder)
        spec = root / "spec.md"
        data = root / "requirements.json"

        def fresh() -> None:
            spec.write_text(
                "# spec\n\n## 4. Requirements\n\n"
                "| ID | Statement | Basis | Status |\n|---|---|---|---|\n"
                "| AUT-01 | One thing | Measured | met |\n"
                "| AUT-02 | Another thing | Measured | gap |\n\n"
                "## 5. Flows\n\n| G-01 | Missing | AUT-02 | Somewhere |\n",
                encoding="utf-8",
            )
            data.write_text(
                json.dumps(
                    {"requirements": [{"id": "AUT-01", "group": "AUT", "statement": "One thing",
                                       "basis": "Measured", "status": "met", "test": "T"},
                                      {"id": "AUT-02", "group": "AUT", "statement": "Another thing",
                                       "basis": "Measured", "status": "gap", "test": None}],
                     "gaps": [{"id": "G-01", "what": "Missing", "requirements": "AUT-02",
                               "where": "Somewhere"}]},
                    indent=2,
                ),
                encoding="utf-8",
            )

        def statuses() -> dict:
            return {
                r["id"]: r["status"]
                for r in json.loads(data.read_text(encoding="utf-8"))["requirements"]
            }

        fresh()
        result = sync(spec, data)
        if result["requirements"] != 2 or result["gaps"] != 1:
            failures.append(f"a two row document synced to {result}")
        if statuses().get("AUT-01") != "met":
            failures.append("a met requirement lost its status")

        # A write over a file another writer moved is refused, and the other writer's
        # bytes are the ones left on disk.
        fresh()
        md_before = spec.read_bytes()
        data_before = data.read_bytes()
        moved = json.loads(data_before)
        moved["requirements"][0]["status"] = "gap"
        moved_bytes = (json.dumps(moved, indent=2) + "\n").encode("utf-8")
        data.write_bytes(moved_bytes)
        try:
            commit(spec, data, md_before, data_before, md_before.decode("utf-8"), json.dumps(moved))
        except SyncError:
            pass
        else:
            failures.append("a write over a file that moved under it was allowed")
        if data.read_bytes() != moved_bytes:
            failures.append("a refused write overwrote the other writer's bytes")

        # A failed markdown replace puts the JSON back, so the update is all or nothing.
        # The markdown write is made to fail by taking the temporary name it writes under.
        fresh()
        md_before = spec.read_bytes()
        data_before = data.read_bytes()
        blocker = spec.with_name(spec.name + ".tmp-" + str(os.getpid()))
        blocker.mkdir()
        try:
            commit(spec, data, md_before, data_before, "anything", "{}\n")
        except OSError:
            pass
        else:
            failures.append("a markdown that could not be replaced was not reported")
        finally:
            blocker.rmdir()
        if data.read_bytes() != data_before:
            failures.append("a failed second write left the first file updated")

        # A lock whose owner is gone and whose expiry passed is reclaimed.
        fresh()
        held = {"pid": 999999, "token": "dead", "expires": int(time.time()) - 1}
        lock_path(data).write_text(json.dumps(held), encoding="utf-8")
        try:
            record = acquire(data)
            release(data, record)
        except SyncError as exc:
            failures.append(f"a stale lock was not reclaimed, {exc}")
        if lock_path(data).exists():
            failures.append("the lock was left behind after a clean run")

        # A lock whose owner is alive is refused, and the holder's lock is left alone.
        fresh()
        live = {"pid": os.getpid(), "token": "live", "expires": int(time.time()) + 60}
        lock_path(data).write_text(json.dumps(live), encoding="utf-8")
        try:
            acquire(data)
        except SyncError:
            pass
        else:
            failures.append("a lock held by a live writer was taken")
        if read_lock(lock_path(data)) != live:
            failures.append("a refused writer changed the holder's lock")
        lock_path(data).unlink(missing_ok=True)

        # Two writers at once. At least one finishes, neither leaves half a file, and the
        # statuses in the JSON are the ones the markdown carries.
        fresh()
        command = [
            sys.executable,
            str(Path(__file__).resolve()),
            "--markdown",
            str(spec),
            "--requirements",
            str(data),
        ]
        writers = [
            subprocess.Popen(
                command,
                stdout=subprocess.PIPE,
                stderr=subprocess.PIPE,
                text=True,
            )
            for _ in range(2)
        ]
        codes = sorted(process.wait(timeout=120) for process in writers)
        for process in writers:
            if process.stdout:
                process.stdout.close()
            if process.stderr:
                process.stderr.close()
        if 0 not in codes:
            failures.append(f"two writers left no winner, exit codes {codes}")
        if any(code not in (0, 2) for code in codes):
            failures.append(f"two writers exited {codes}, expected 0 or 2 for each")
        try:
            final = json.loads(data.read_text(encoding="utf-8"))
        except ValueError as exc:
            failures.append(f"two writers left the JSON unreadable, {exc}")
            final = {"requirements": []}
        in_json = {r["id"]: r["status"] for r in final.get("requirements", [])}
        md_text = spec.read_text(encoding="utf-8")
        for line in md_text.split("\n"):
            if not line.startswith("| AUT-"):
                continue
            c = cells(line)
            if len(c) == 4 and in_json.get(c[0]) != c[3]:
                failures.append(
                    f"after the race {c[0]} is {in_json.get(c[0])} in the JSON and {c[3]} "
                    "in the markdown"
                )

    if failures:
        for failure in failures:
            print("selftest failed, " + failure, file=sys.stderr)
        return 1
    print(
        "selftest ok, a write over a file that moved is refused, a failed second write puts "
        "the first back, a stale lock is reclaimed, a live lock is refused, and two writers "
        "leave one whole state with no lost status"
    )
    return 0


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description="Refresh requirements.json and the Status column.")
    parser.add_argument("--markdown", default=str(SPEC), help="the specification markdown")
    parser.add_argument("--requirements", default=str(DATA), help="the requirements JSON")
    parser.add_argument("--json", action="store_true", help="print the result as one JSON object")
    parser.add_argument("--selftest", action="store_true", help="run the built in selftest")
    args = parser.parse_args(argv)

    if args.selftest:
        return selftest()

    markdown = Path(args.markdown)
    data_path = Path(args.requirements)

    record = acquire(data_path)
    try:
        result = sync(markdown, data_path)
    except SyncError as exc:
        print(str(exc), file=sys.stderr)
        return 2
    finally:
        release(data_path, record)

    counts = result["status"]
    if args.json:
        print(
            json.dumps(
                {
                    "ok": True,
                    "requirements": result["requirements"],
                    "groups": result["groups"],
                    "gaps": result["gaps"],
                    "status": counts,
                    "markdown": str(markdown),
                    "requirements_json": str(data_path),
                },
                indent=2,
            )
        )
    else:
        print(
            f"{result['requirements']} requirements, {result['gaps']} gaps, "
            + ", ".join(f"{k} {v}" for k, v in sorted(counts.items()))
        )
    return 0


if __name__ == "__main__":
    sys.exit(main())
