"""Run a named fuzz corpus against the readers this folder can reach.

SEC-08 says the parsers SHALL have a named fuzz corpus and a stated residual
risk, and that a malformed input SHALL return a tool error rather than terminate
the process. This names the corpus, runs every case in a child process so a
process that dies is visible, and records what happened.

The corpus is `fuzz/wc3-spec-parsers-v1.json`, one case per reader call. The
residual risk is `fuzz/wc3-spec-parsers-v1.md`, which names what the corpus does
not reach. The map, object and trigger readers are C# over native CascLib, and no
case here touches them.

Each case runs in its own interpreter. A case that raises a named exception is a
tool error and the process survives. A case that ends the interpreter without its
result line is a crash, whatever the exit code, and the run fails.

The run record is state, so it lives outside this corpus. One line is appended to
`fuzz-runs.jsonl` in the state folder, under a lock that carries an owner, a token
and an expiry, and the append is a single write. The folder is `WC3_SPEC_STATE`
when that is set, else `<LOCALAPPDATA>/wc3-mcp/spec-state`.

    python tools/fuzz_corpus.py
    python tools/fuzz_corpus.py --json
    python tools/fuzz_corpus.py --no-record
    python tools/fuzz_corpus.py --selftest

Exit 0 when every case behaved as the corpus says, 2 when one did not, 1 when the
corpus, a reader or the state folder cannot be read.
"""

from __future__ import annotations

import argparse
import json
import os
import subprocess
import sys
import tempfile
import time
import uuid
from datetime import datetime, timedelta, timezone
from pathlib import Path

HERE = Path(__file__).resolve().parent
if str(HERE) not in sys.path:
    sys.path.insert(0, str(HERE))

CORPUS = HERE.parent / "fuzz" / "wc3-spec-parsers-v1.json"
MARKER = "FUZZ-RESULT "
NO_WINDOW = subprocess.CREATE_NO_WINDOW if os.name == "nt" else 0


class CorpusError(RuntimeError):
    """The corpus, a reader or the state folder cannot be read."""


class LockBusy(RuntimeError):
    """Another process holds the run log, and its lock has not expired."""


# The readers. Each takes the case file and a work folder, and returns a short
# string that the case can match, or raises the error the reader itself raises.
# Nothing here catches, so a reader that raises is a reader that raised.


def _json_file(case_file: Path, work: Path) -> str:
    from check_versions import load_json

    document = load_json(case_file)
    return f"parsed {type(document).__name__}"


def _record_file(case_file: Path, work: Path) -> str:
    from release_record import load

    document = load(case_file)
    return f"record {document.get('record')}"


def _requirements_file(case_file: Path, work: Path) -> str:
    from check_counts import load_requirements

    document = load_requirements(case_file)
    return f"requirements {len(document['requirements'])}, gaps {len(document['gaps'])}"


def _markdown_tables(case_file: Path, work: Path) -> str:
    from sync_requirements import parse

    requirements, gaps = parse(case_file.read_text(encoding="utf-8"))
    return f"{len(requirements)} requirements, {len(gaps)} gaps"


def _prose_counts(case_file: Path, work: Path) -> str:
    from check_counts import SPEC_COUNTS, prose_counts

    text = case_file.read_text(encoding="utf-8")
    found = prose_counts(text, SPEC_COUNTS, case_file.name, "counts sentence")
    return f"requirements {found['requirements']}, groups {found['groups']}"


def _digest_sidecar(case_file: Path, work: Path) -> str:
    from check_provenance import read_digest

    return "digest " + read_digest(case_file)[:16]


def _provenance_record(case_file: Path, work: Path) -> str:
    from check_provenance import record_provenance

    assets = work / "assets"
    assets.mkdir(exist_ok=True)
    (assets / "release.json").write_bytes(case_file.read_bytes())
    commit, reason = record_provenance(assets, "v0.5.12")
    if commit:
        return f"commit {commit[:12]}"
    return f"refused, {reason}"


def _pe_version(case_file: Path, work: Path) -> str:
    from check_versions import read_exe_version

    file_version, product_version = read_exe_version(case_file)
    return f"file {file_version}, product {product_version}"


def _version_scan(case_file: Path, work: Path) -> str:
    from check_versions import json_versions

    document = json.loads(case_file.read_text(encoding="utf-8"))
    found = json_versions(document, "")
    return f"{len(found)} version strings"


def _ends_process(case_file: Path, work: Path) -> str:
    """A reader that ends its interpreter. Only the selftest uses it, so a dead
    child is proven to be seen as a crash rather than as an error."""
    os._exit(9)


ADAPTERS = {
    "ends_process": _ends_process,
    "json_file": _json_file,
    "record_file": _record_file,
    "requirements_file": _requirements_file,
    "markdown_tables": _markdown_tables,
    "prose_counts": _prose_counts,
    "digest_sidecar": _digest_sidecar,
    "provenance_record": _provenance_record,
    "pe_version": _pe_version,
    "version_scan": _version_scan,
}


def load_corpus(path: Path) -> dict:
    try:
        document = json.loads(path.read_text(encoding="utf-8"))
    except OSError as exc:
        raise CorpusError(f"no corpus at {path}, {exc}") from exc
    except ValueError as exc:
        raise CorpusError(f"{path} is not valid JSON, {exc}") from exc
    if not isinstance(document, dict) or not isinstance(document.get("cases"), list):
        raise CorpusError(f"{path} holds no cases list")
    for case in document["cases"]:
        if not isinstance(case, dict) or not case.get("id") or not case.get("parser"):
            raise CorpusError(f"{path} has a case with no id or no parser, {case!r}")
        if case["parser"] not in ADAPTERS:
            raise CorpusError(f"case {case['id']} names parser {case['parser']!r}, which no adapter here runs")
        if case.get("expect") not in ("error", "value"):
            raise CorpusError(f"case {case['id']} expects {case.get('expect')!r}, which is neither error nor value")
    return document


def case_bytes(case: dict) -> bytes:
    payload = case.get("payload", "")
    if case.get("encoding") == "hex":
        return bytes.fromhex(payload)
    return payload.encode("utf-8")


def run_case(corpus_path: Path, case_id: str, work: Path) -> int:
    """One case, in the child interpreter. Prints one result line and exits 0."""
    document = load_corpus(corpus_path)
    case = next((c for c in document["cases"] if c["id"] == case_id), None)
    if case is None:
        print(f"no case {case_id} in {corpus_path}", file=sys.stderr)
        return 1

    payload = work / "payload.bin"
    payload.write_bytes(case_bytes(case))
    try:
        value = ADAPTERS[case["parser"]](payload, work)
    except Exception as exc:  # the point of the run, a reader error is a result
        result = {
            "case": case_id,
            "outcome": "error",
            "error": type(exc).__name__,
            "detail": str(exc).splitlines()[0][:160] if str(exc) else "",
        }
    else:
        result = {"case": case_id, "outcome": "value", "error": "", "detail": str(value)[:160]}
    print(MARKER + json.dumps(result))
    return 0


# The run record is state, kept outside this folder and written under a lock.


def state_dir() -> Path:
    named = os.environ.get("WC3_SPEC_STATE")
    if named:
        return Path(named)
    base = os.environ.get("LOCALAPPDATA") or os.environ.get("XDG_STATE_HOME")
    if base:
        return Path(base) / "wc3-mcp" / "spec-state"
    return Path.home() / ".local" / "state" / "wc3-mcp"


class Lock:
    """One lock file, with an owner, a token and an expiry the thief can read."""

    def __init__(self, path: Path, seconds: float = 60.0):
        self.path = path
        self.seconds = seconds
        self.token = uuid.uuid4().hex
        self.stole = False

    def _document(self) -> dict:
        return {
            "owner": os.getpid(),
            "token": self.token,
            "acquired": datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
            "expires": (datetime.now(timezone.utc) + timedelta(seconds=self.seconds)).strftime(
                "%Y-%m-%dT%H:%M:%SZ"
            ),
            "seconds": self.seconds,
        }

    def _write(self, document: dict) -> None:
        temporary = self.path.with_name(self.path.name + "." + self.token)
        temporary.write_text(json.dumps(document) + "\n", encoding="utf-8")
        os.replace(temporary, self.path)

    def acquire(self, wait: float = 30.0) -> "Lock":
        """Take the lock, waiting while a live holder has it, stealing an expired one."""
        self.path.parent.mkdir(parents=True, exist_ok=True)
        document = self._document()
        deadline = time.monotonic() + max(wait, 0.0)
        while True:
            try:
                handle = os.open(self.path, os.O_CREAT | os.O_EXCL | os.O_WRONLY)
            except FileExistsError:
                held = self.read()
                if not held or self.expired(held):
                    # A dead process leaves its lock behind. The expiry is what
                    # lets a second writer take it, and the token says who did.
                    self.stole = True
                    try:
                        self.path.unlink()
                    except FileNotFoundError:
                        pass
                    continue
                if time.monotonic() >= deadline:
                    raise LockBusy(
                        f"{self.path} is held by pid {held.get('owner')} "
                        f"until {held.get('expires')}"
                    ) from None
                time.sleep(0.05)
                continue
            with os.fdopen(handle, "w", encoding="utf-8") as stream:
                json.dump(document, stream)
                stream.write("\n")
                stream.flush()
                os.fsync(stream.fileno())
            return self

    def read(self) -> dict:
        try:
            return json.loads(self.path.read_text(encoding="utf-8"))
        except (OSError, ValueError):
            return {}

    def expired(self, document: dict) -> bool:
        stamp = document.get("expires")
        if not isinstance(stamp, str):
            return True
        try:
            when = datetime.strptime(stamp, "%Y-%m-%dT%H:%M:%SZ").replace(tzinfo=timezone.utc)
        except ValueError:
            return True
        return when <= datetime.now(timezone.utc)

    def release(self) -> None:
        held = self.read()
        if held.get("token") != self.token:
            return
        try:
            self.path.unlink()
        except FileNotFoundError:
            pass


def append_record(folder: Path, record: dict, seconds: float = 60.0, wait: float = 30.0) -> Path:
    """One line, appended under the lock, in a single write. A waiter queues."""
    folder.mkdir(parents=True, exist_ok=True)
    log = folder / "fuzz-runs.jsonl"
    lock = Lock(folder / "fuzz-runs.lock", seconds).acquire(wait)
    try:
        line = json.dumps(record, ensure_ascii=False) + "\n"
        with log.open("a", encoding="utf-8", newline="\n") as stream:
            stream.write(line)
            stream.flush()
            os.fsync(stream.fileno())
    finally:
        lock.release()
    return log


def append_many(folder: str, count: int) -> int:
    """The two writer test needs a second process that writes the same file."""
    for index in range(count):
        append_record(
            Path(folder),
            {
                "run": datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
                "corpus": "two-writer",
                "owner": os.getpid(),
                "index": index,
            },
        )
    return 0


def judge(case: dict, result: dict) -> tuple[bool, str]:
    expected = case["expect"]
    outcome = result.get("outcome")
    if expected == "error":
        if outcome == "error":
            return True, result.get("error", "")
        return False, f"expected an error and got {outcome}, {result.get('detail', '')}"
    if outcome != "value":
        return False, f"expected a value and got {outcome}, {result.get('error', '')}"
    wanted = case.get("expect_text")
    if wanted and wanted not in (result.get("detail") or ""):
        return False, f"expected {wanted!r} in {result.get('detail')!r}"
    return True, result.get("detail", "")


def run_corpus(corpus_path: Path, limit: float = 60.0) -> tuple[list[dict], dict]:
    document = load_corpus(corpus_path)
    rows: list[dict] = []
    for case in document["cases"]:
        work = Path(tempfile.mkdtemp(prefix="fuzz-case-"))
        started = time.perf_counter()
        try:
            done = subprocess.run(
                [sys.executable, str(Path(__file__).resolve()), "--run-case", str(corpus_path), case["id"], str(work)],
                capture_output=True,
                text=True,
                encoding="utf-8",
                errors="replace",
                timeout=limit,
                creationflags=NO_WINDOW,
            )
            elapsed = time.perf_counter() - started
            line = next(
                (l for l in reversed((done.stdout or "").splitlines()) if l.startswith(MARKER)),
                "",
            )
            if not line:
                result = {
                    "case": case["id"],
                    "outcome": "crash",
                    "error": f"exit {done.returncode}",
                    "detail": (done.stderr or done.stdout or "no output").strip().splitlines()[-1][:160]
                    if (done.stderr or done.stdout or "").strip()
                    else "no output",
                }
            else:
                result = json.loads(line[len(MARKER):])
        except subprocess.TimeoutExpired:
            elapsed = time.perf_counter() - started
            result = {"case": case["id"], "outcome": "crash", "error": "timeout", "detail": f"no result within {limit} seconds"}
        ok, detail = judge(case, result)
        rows.append(
            {
                "case": case["id"],
                "parser": case["parser"],
                "expected": case["expect"],
                "outcome": result.get("outcome"),
                "error": result.get("error", ""),
                "detail": detail,
                "seconds": round(elapsed, 2),
                "ok": ok,
            }
        )
    summary = {
        "corpus": document.get("corpus", corpus_path.stem),
        "cases": len(rows),
        "ok": sum(1 for r in rows if r["ok"]),
        "failed": [r["case"] for r in rows if not r["ok"]],
        "crashed": [r["case"] for r in rows if r["outcome"] == "crash"],
        "errors": sum(1 for r in rows if r["outcome"] == "error"),
    }
    return rows, summary


def render(rows: list[dict], summary: dict) -> str:
    widths = {
        "case": max([len("Case")] + [len(r["case"]) for r in rows]),
        "parser": max([len("Reader")] + [len(r["parser"]) for r in rows]),
        "outcome": max([len("Outcome")] + [len(str(r["outcome"])) for r in rows]),
        "error": max([len("Error")] + [len(r["error"] or "-") for r in rows]),
    }
    head = (
        f"{'Case'.ljust(widths['case'])}  {'Reader'.ljust(widths['parser'])}  "
        f"{'Outcome'.ljust(widths['outcome'])}  {'Error'.ljust(widths['error'])}  Verdict"
    )
    lines = [head, "-" * len(head)]
    for row in rows:
        lines.append(
            f"{row['case'].ljust(widths['case'])}  {row['parser'].ljust(widths['parser'])}  "
            f"{str(row['outcome']).ljust(widths['outcome'])}  {(row['error'] or '-').ljust(widths['error'])}  "
            f"{'ok' if row['ok'] else 'FAIL'}"
        )
    lines.append("")
    if summary["failed"]:
        lines.append(
            f"{len(summary['failed'])} of {summary['cases']} cases did not behave as the corpus says, "
            + ", ".join(summary["failed"])
        )
        for row in rows:
            if not row["ok"]:
                lines.append(f"  {row['case']}  {row['detail']}")
    else:
        lines.append(
            f"all {summary['cases']} cases behaved, {summary['errors']} malformed inputs each returned a tool "
            "error and no interpreter ended without its result line"
        )
    return "\n".join(lines)


def selftest() -> int:
    """Prove a pass, a fail and a crash are told apart, and that two writers keep every line."""
    import contextlib
    import io

    failures: list[str] = []
    with tempfile.TemporaryDirectory(prefix="fuzz-corpus-") as folder:
        root = Path(folder)
        corpus = root / "corpus.json"
        corpus.write_text(
            json.dumps(
                {
                    "corpus": "selftest",
                    "cases": [
                        {"id": "empty-json", "parser": "json_file", "payload": "", "expect": "error"},
                        {"id": "valid-json", "parser": "json_file", "payload": "{\"a\": 1}", "expect": "value"},
                        {"id": "returns-not-raises", "parser": "markdown_tables", "payload": "no table here", "expect": "error"},
                    ],
                }
            ),
            encoding="utf-8",
        )
        rows, summary = run_corpus(corpus)
        if summary["ok"] != 2:
            failures.append(f"expected 2 cases to hold, got {summary['ok']} of {summary['cases']}")
        if "returns-not-raises" not in summary["failed"]:
            failures.append("a reader that returned where an error was expected was not a failure")
        if any(row["outcome"] == "crash" for row in rows):
            failures.append("a working reader was reported as a crash")

        dead = root / "crash-corpus.json"
        dead.write_text(
            json.dumps(
                {
                    "corpus": "selftest-crash",
                    "cases": [{"id": "kills-itself", "parser": "ends_process", "payload": "", "expect": "error"}],
                }
            ),
            encoding="utf-8",
        )
        rows, summary = run_corpus(dead)
        if not rows or rows[0]["outcome"] != "crash":
            failures.append("a reader that ended the process was not reported as a crash")
        if rows and rows[0]["ok"]:
            failures.append("a crash passed the corpus")

        state = root / "state"
        lock = Lock(state / "run.lock", 60).acquire()
        try:
            try:
                Lock(state / "run.lock", 60).acquire(wait=0.2)
            except LockBusy:
                pass
            else:
                failures.append("a held lock was taken by a second writer")
            if lock.read().get("owner") != os.getpid():
                failures.append("the lock names no owner")
            if not lock.read().get("expires"):
                failures.append("the lock carries no expiry")
        finally:
            lock.release()

        stale = Lock(state / "stale.lock", 60)
        stale.acquire()
        document = stale.read()
        document["expires"] = (datetime.now(timezone.utc) - timedelta(seconds=5)).strftime("%Y-%m-%dT%H:%M:%SZ")
        (state / "stale.lock").write_text(json.dumps(document), encoding="utf-8")
        thief = Lock(state / "stale.lock", 60)
        try:
            thief.acquire()
            if not thief.stole:
                failures.append("an expired lock was taken without saying it was stolen")
        except LockBusy:
            failures.append("an expired lock could not be taken")
        finally:
            thief.release()

        two = root / "two-writer"
        workers = [
            subprocess.Popen(
                [sys.executable, str(Path(__file__).resolve()), "--append-many", str(two), "5"],
                stdout=subprocess.DEVNULL,
                stderr=subprocess.PIPE,
                text=True,
                creationflags=NO_WINDOW,
            )
            for _ in range(2)
        ]
        for worker in workers:
            worker.wait(timeout=120)
        log = two / "fuzz-runs.jsonl"
        lines = log.read_text(encoding="utf-8").splitlines() if log.is_file() else []
        if len(lines) != 10:
            failures.append(f"two writers appending 5 each left {len(lines)} lines, so an update was lost")
        owners = {json.loads(line)["owner"] for line in lines}
        if len(owners) != 2:
            failures.append(f"the two writers wrote as {len(owners)} owner(s)")
        if (two / "fuzz-runs.lock").exists():
            failures.append("a lock outlived the run that took it")

    if failures:
        for failure in failures:
            print("selftest failed, " + failure, file=sys.stderr)
        return 1
    print(
        "selftest ok, a pass, a failing expectation and a killed interpreter are told apart, a held "
        "lock is refused, an expired one is stolen with a new owner, and two writers keep every line"
    )
    return 0


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description="Run the named fuzz corpus and record what happened.")
    parser.add_argument("--corpus", default=str(CORPUS), help="the corpus file")
    parser.add_argument("--state", default=None, help="the state folder the run log lives in")
    parser.add_argument("--no-record", action="store_true", help="run the cases and write no run record")
    parser.add_argument("--limit", type=float, default=60.0, help="seconds one case may take")
    parser.add_argument("--json", action="store_true", help="print the result as JSON")
    parser.add_argument("--selftest", action="store_true", help="run the built in selftest")
    parser.add_argument("--run-case", nargs=3, metavar=("CORPUS", "CASE", "WORK"), help=argparse.SUPPRESS)
    parser.add_argument("--append-many", nargs=2, metavar=("FOLDER", "COUNT"), help=argparse.SUPPRESS)
    args = parser.parse_args(argv)

    if args.selftest:
        return selftest()
    if args.run_case:
        return run_case(Path(args.run_case[0]), args.run_case[1], Path(args.run_case[2]))
    if args.append_many:
        return append_many(args.append_many[0], int(args.append_many[1]))

    try:
        rows, summary = run_corpus(Path(args.corpus), args.limit)
    except CorpusError as exc:
        print(str(exc), file=sys.stderr)
        return 1

    code = 0 if not summary["failed"] else 2
    log = None
    if not args.no_record:
        folder = Path(args.state) if args.state else state_dir()
        record = {
            "run": datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
            "corpus": summary["corpus"],
            "cases": summary["cases"],
            "ok": summary["ok"],
            "errors": summary["errors"],
            "crashed": summary["crashed"],
            "failed": summary["failed"],
            "verdict": "ok" if code == 0 else "fail",
            "owner": os.getpid(),
        }
        try:
            log = append_record(folder, record)
        except (OSError, LockBusy) as exc:
            print(f"the run was not recorded, {exc}", file=sys.stderr)
            return 1

    if args.json:
        print(json.dumps({"ok": code == 0, "exit": code, "summary": summary, "rows": rows, "log": str(log) if log else None}, indent=2))
    else:
        print(render(rows, summary))
        print()
        print(f"corpus {summary['corpus']}")
        print(f"log    {log}" if log else "log    not written, --no-record")
    return code


if __name__ == "__main__":
    sys.exit(main())
