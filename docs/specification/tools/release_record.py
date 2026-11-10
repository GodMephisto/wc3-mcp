"""Write, verify and resolve the machine readable record of a release.

AUT-11 says the release SHALL write a record of the source commit, the artifact
digests and the tag, and that a published version SHALL resolve to that record.
PKG-13 says a version number alone is not provenance. The npm package at 0.5.19
has no tag that explains it, so a reader who installs it cannot tell which commit
it came from or whether the bytes on disk are the bytes that were built.

This writes one JSON record per release, named for the tag, holding the version,
the tag, the source commit and a sha256 for every artifact. Verify recomputes the
digests and refuses a file that changed. Resolve answers which record explains a
published version, and refuses a version no record names.

    python tools/release_record.py write --version 0.5.16 --artifact dist-mcp/wc3-mcp.exe
    python tools/release_record.py verify releases/v0.5.16.json
    python tools/release_record.py resolve --version 0.5.16
    python tools/release_record.py resolve --version 0.5.21 --repo ../..
    python tools/release_record.py resolve --version 0.5.19
    python tools/release_record.py --selftest

An artifact path that is an http or https URL is a published artifact, and the
record for a tag is the record of what the release served. For one of those the
digest is read from the `.sha256` published beside the asset, which is 93 bytes,
so a version resolves without downloading a 55 MB bundle. When the read fails,
the row says the claim was not checked, and the exit is 2, because a digest that
could not be read is not a digest that agreed.

With `--repo` the record's commit is also compared with the commit the tag points
at, so resolve answers whether the published bytes came from the tagged commit
rather than from a later build of the same version.

Exit 0 when the record is written or agrees, 2 when a digest, a tag or a version
does not, 1 when a file or the commit cannot be read. The record is written only
when --force is absent and no record of that tag exists, so a tag is one record.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import os
import re
import shutil
import subprocess
import sys
from datetime import datetime, timezone
from pathlib import Path
from urllib.request import Request, urlopen

HERE = Path(__file__).resolve().parent
DEFAULT_OUT = HERE.parent / "releases"
SCHEMA = "wc3-release-record/1"
NO_WINDOW = subprocess.CREATE_NO_WINDOW if os.name == "nt" else 0
VERSION = re.compile(r"^\d+\.\d+\.\d+(?:\.\d+)?$")
PUBLISHED = re.compile(r"^https?://", re.IGNORECASE)
SHA256 = re.compile(r"\b([0-9a-fA-F]{64})\b")


class RecordError(RuntimeError):
    """A record, an artifact or the commit could not be read, so nothing is claimed."""


def sha256_of(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for block in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(block)
    return digest.hexdigest()


def git_binary() -> str:
    """The git to run, from the environment, the PATH, or the usual Windows install."""
    git = os.environ.get("GIT") or shutil.which("git")
    if not git and os.name == "nt":
        fallback = Path("C:/Program Files/Git/bin/git.exe")
        if fallback.is_file():
            git = str(fallback)
    if not git:
        raise RecordError("no git found, pass --commit with the commit the artifacts were built from")
    return git


def head_commit(root: Path) -> str:
    """The commit the release is cut from, read from git, never guessed."""
    git = git_binary()
    try:
        done = subprocess.run(
            [git, "-C", str(root), "rev-parse", "HEAD"],
            capture_output=True,
            text=True,
            timeout=60,
            creationflags=NO_WINDOW,
        )
    except (OSError, subprocess.TimeoutExpired) as exc:
        raise RecordError(f"git could not be run, {exc}") from exc
    commit = (done.stdout or "").strip()
    if done.returncode != 0 or not re.fullmatch(r"[0-9a-f]{7,40}", commit):
        first = (done.stderr or done.stdout or "").strip().splitlines()
        raise RecordError(
            f"git could not name a commit in {root}, it said {first[0] if first else 'nothing'}"
        )
    return commit


def commit_for_tag(repo: Path, tag: str) -> str:
    """The commit a tag points at, so a record can be compared with the tag it names."""
    git = git_binary()
    try:
        done = subprocess.run(
            [git, "-C", str(repo), "rev-list", "-n", "1", tag],
            capture_output=True,
            text=True,
            timeout=60,
            creationflags=NO_WINDOW,
        )
    except (OSError, subprocess.TimeoutExpired) as exc:
        raise RecordError(f"git could not be run in {repo}, {exc}") from exc
    commit = (done.stdout or "").strip()
    if done.returncode != 0 or not re.fullmatch(r"[0-9a-f]{40}", commit):
        first = (done.stderr or done.stdout or "").strip().splitlines()
        raise RecordError(
            f"{tag} names no commit in {repo}, git said {first[0] if first else 'nothing'}"
        )
    return commit


def http_fetch(url: str, timeout: float = 30.0) -> str:
    """Read one published file as text, for the digest sidecar beside an artifact."""
    try:
        request = Request(url, headers={"User-Agent": "wc3-release-record/1"})
        with urlopen(request, timeout=timeout) as reply:
            return reply.read(65536).decode("utf-8", "replace")
    except (OSError, ValueError) as exc:
        raise RecordError(f"{url} could not be read, {exc}") from exc


def published_digest(url: str, fetch=None) -> str:
    """The sha256 the release published beside an artifact, from its .sha256 sidecar."""
    sidecar = url.rstrip("/") + ".sha256"
    text = (fetch or http_fetch)(sidecar)
    match = SHA256.search(text)
    if not match:
        raise RecordError(f"{sidecar} holds no sha256 digest")
    return match.group(1).lower()


def is_published(path: str) -> bool:
    return bool(isinstance(path, str) and PUBLISHED.match(path))


def as_relative(path: Path, root: Path) -> str:
    try:
        return path.resolve().relative_to(root.resolve()).as_posix()
    except ValueError:
        return path.resolve().as_posix()


def build_record(version: str, tag: str, commit: str, artifacts: list[Path], root: Path) -> dict:
    """One record, with a digest per artifact, refusing an artifact it cannot read."""
    if not VERSION.match(version):
        raise RecordError(f"{version!r} is not a version, so no record can be named for it")
    if tag != "v" + version:
        raise RecordError(
            f"the tag {tag} does not name the version {version}. One release is one tag and "
            "one version, so the record would break the rule it exists to prove"
        )
    if not artifacts:
        raise RecordError("no artifact was named, so the record would carry no digest")

    entries = []
    for path in artifacts:
        if not path.is_file():
            raise RecordError(f"no artifact at {path}")
        entries.append(
            {
                "name": path.name,
                "path": as_relative(path, root),
                "bytes": path.stat().st_size,
                "sha256": sha256_of(path),
            }
        )

    return {
        "record": SCHEMA,
        "version": version,
        "tag": tag,
        "commit": commit,
        "written": datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
        "artifacts": entries,
    }


def record_path(out: Path, tag: str) -> Path:
    return out / f"{tag}.json"


def write(args) -> int:
    root = Path(args.root).resolve()
    out = Path(args.out)
    artifacts = [Path(a) if Path(a).is_absolute() else (root / a) for a in args.artifact]
    tag = args.tag or f"v{args.version}"
    commit = args.commit or head_commit(root)
    record = build_record(args.version, tag, commit, artifacts, root)

    out.mkdir(parents=True, exist_ok=True)
    destination = record_path(out, tag)
    payload = json.dumps(record, indent=2, ensure_ascii=False) + "\n"
    if args.force:
        temporary = destination.with_name(destination.name + ".tmp-" + str(os.getpid()))
        temporary.write_text(payload, encoding="utf-8", newline="\n")
        os.replace(temporary, destination)
    else:
        # One tag is one record, so the file is created, not checked and then written. Two
        # writers racing on the same tag cannot both create it, and a write that fails removes
        # its own partial file so the tag is not left burned by a half record.
        try:
            handle = os.open(destination, os.O_CREAT | os.O_EXCL | os.O_WRONLY, 0o644)
        except FileExistsError as exc:
            raise RecordError(f"{destination} already exists, pass --force to replace it") from exc
        try:
            with os.fdopen(handle, "w", encoding="utf-8", newline="\n") as stream:
                stream.write(payload)
                stream.flush()
                os.fsync(stream.fileno())
        except OSError:
            destination.unlink(missing_ok=True)
            raise

    if args.json:
        print(json.dumps(record, indent=2, ensure_ascii=False))
        return 0
    print(f"record    {destination}")
    print(f"version   {record['version']}")
    print(f"tag       {record['tag']}")
    print(f"commit    {record['commit']}")
    for entry in record["artifacts"]:
        print(f"artifact  {entry['path']}  {entry['bytes']:,} bytes  {entry['sha256']}")
    print(f"one record names {len(record['artifacts'])} artifact digests, the tag and the commit")
    return 0


def load(path: Path) -> dict:
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except OSError as exc:
        raise RecordError(f"no record at {path}, {exc}") from exc
    except json.JSONDecodeError as exc:
        raise RecordError(f"{path} is not valid JSON, {exc}") from exc
    if not isinstance(data, dict) or data.get("record") != SCHEMA:
        raise RecordError(f"{path} is not a {SCHEMA} record, it says {data.get('record')!r}")
    return data


def judge(record: dict, root: Path, fetch=None, tag_commit: str | None = None) -> list[dict]:
    """One row per claim the record makes, checked against the files or the release."""
    rows = []

    version = record.get("version")
    tag = record.get("tag")
    rows.append(
        {
            "claim": "Tag names the version",
            "expected": f"v{version}",
            "found": str(tag),
            "ok": tag == f"v{version}",
            "detail": "" if tag == f"v{version}" else f"{tag} does not name {version}",
        }
    )

    commit = record.get("commit")
    rows.append(
        {
            "claim": "Source commit",
            "expected": "a git commit",
            "found": str(commit),
            "ok": isinstance(commit, str) and bool(re.fullmatch(r"[0-9a-f]{7,40}", commit)),
            "detail": "" if commit else "the record names no commit, so nothing explains the bytes",
        }
    )

    if tag_commit is not None:
        named = commit if isinstance(commit, str) else ""
        holds = bool(named) and (tag_commit.startswith(named) or named.startswith(tag_commit))
        rows.append(
            {
                "claim": "Source commit is the tag's commit",
                "expected": tag_commit[:12],
                "found": named[:12] or "none",
                "ok": holds,
                "detail": ""
                if holds
                else f"the record names {named or 'no commit'} while {tag} points at {tag_commit}",
            }
        )

    for entry in record.get("artifacts") or []:
        path = entry.get("path") or ""
        if is_published(path):
            try:
                published = published_digest(path, fetch)
            except RecordError as exc:
                rows.append(
                    {
                        "claim": f"Published digest of {entry['name']}",
                        "expected": entry["sha256"][:16],
                        "found": "not read",
                        "ok": False,
                        "detail": f"the digest published beside the asset was not read, so this "
                        f"claim is not checked, {exc}",
                    }
                )
                continue
            ok = published == entry["sha256"]
            rows.append(
                {
                    "claim": f"Published digest of {entry['name']}",
                    "expected": entry["sha256"][:16],
                    "found": published[:16],
                    "ok": ok,
                    "detail": ""
                    if ok
                    else f"the release publishes {published} for {entry['name']}, the record says "
                    f"{entry['sha256']}",
                }
            )
            continue

        local = root / path if not Path(path).is_absolute() else Path(path)
        if not local.is_file():
            rows.append(
                {
                    "claim": f"Digest of {entry['name']}",
                    "expected": entry["sha256"][:16],
                    "found": "no file",
                    "ok": False,
                    "detail": f"{local} is not there, so the record cannot be checked",
                }
            )
            continue
        found = sha256_of(local)
        size = local.stat().st_size
        ok = found == entry["sha256"] and size == entry.get("bytes", size)
        rows.append(
            {
                "claim": f"Digest of {entry['name']}",
                "expected": entry["sha256"][:16],
                "found": found[:16],
                "ok": ok,
                "detail": ""
                if ok
                else f"{entry['path']} is {size:,} bytes and hashes {found}, the record says "
                f"{entry.get('bytes', 'unknown')} bytes and {entry['sha256']}",
            }
        )
    return rows


def render(rows: list[dict], where: str) -> str:
    widths = {
        "claim": max([len("Claim")] + [len(r["claim"]) for r in rows]),
        "expected": max([len("Expected")] + [len(str(r["expected"])) for r in rows]),
        "found": max([len("Found")] + [len(str(r["found"])) for r in rows]),
    }
    lines = [
        f"record  {where}",
        "",
        f"{'Claim'.ljust(widths['claim'])}  {'Expected'.ljust(widths['expected'])}  "
        f"{'Found'.ljust(widths['found'])}  Verdict",
        "-" * (sum(widths.values()) + 16),
    ]
    for r in rows:
        lines.append(
            f"{r['claim'].ljust(widths['claim'])}  "
            f"{str(r['expected']).ljust(widths['expected'])}  "
            f"{str(r['found']).ljust(widths['found'])}  "
            f"{'ok' if r['ok'] else 'DRIFT'}"
        )
    drifted = [r for r in rows if not r["ok"]]
    lines.append("")
    if drifted:
        lines.append(f"{len(drifted)} of {len(rows)} claims do not hold against the bytes checked")
        for r in drifted:
            if r["detail"]:
                lines.append("  " + r["detail"])
    else:
        lines.append(
            f"All {len(rows)} claims hold, every artifact the record names agrees with the "
            "bytes it was checked against"
        )
    return "\n".join(lines)


def verify(args) -> int:
    path = Path(args.record)
    record = load(path)
    tag_commit = None
    if getattr(args, "repo", None):
        tag_commit = commit_for_tag(Path(args.repo).resolve(), str(record.get("tag") or ""))
    rows = judge(record, Path(args.root).resolve(), tag_commit=tag_commit)
    code = 0 if all(r["ok"] for r in rows) else 2
    if args.json:
        print(json.dumps({"ok": code == 0, "exit": code, "record": str(path), "checks": rows}, indent=2))
    else:
        print(render(rows, str(path)))
    return code


def resolve(args) -> int:
    out = Path(args.out)
    if args.tag:
        wanted = args.tag
    elif args.version:
        wanted = f"v{args.version}"
    else:
        raise RecordError("name a --version or a --tag, so a published version can be answered")

    path = record_path(out, wanted)
    if not path.is_file():
        raise RecordError(
            f"no record for {wanted} in {out}. A published version that resolves to no record "
            "has no source commit, no digest and no tag behind it"
        )
    record = load(path)
    tag_commit = None
    if getattr(args, "repo", None):
        tag_commit = commit_for_tag(Path(args.repo).resolve(), str(record.get("tag") or wanted))
    rows = judge(record, Path(args.root).resolve(), tag_commit=tag_commit)
    if args.json:
        print(json.dumps({"ok": all(r["ok"] for r in rows), "record": record, "checks": rows}, indent=2))
    else:
        print(f"{record['version']}  {record['tag']}  {record['commit']}  {path}")
        for entry in record["artifacts"]:
            print(f"  {entry['path']}  {entry['bytes']:,} bytes  {entry['sha256']}")
    return 0 if all(r["ok"] for r in rows) else 2


def selftest() -> int:
    """Write, verify, resolve and refuse, over real files in a temporary folder."""
    import contextlib
    import io
    import tempfile

    failures: list[str] = []
    with tempfile.TemporaryDirectory(prefix="release-record-") as folder:
        root = Path(folder)
        first = root / "dist" / "wc3-mcp.exe"
        second = root / "npm" / "wc3-mcp.tgz"
        first.parent.mkdir(parents=True)
        second.parent.mkdir(parents=True)
        first.write_bytes(b"the server, as published")
        second.write_bytes(b"the wrapper, as published")
        out = root / "releases"

        def run(argv: list[str]) -> tuple[int, str]:
            buffer = io.StringIO()
            with contextlib.redirect_stdout(buffer), contextlib.redirect_stderr(io.StringIO()):
                try:
                    code = main(argv)
                except RecordError:
                    code = 2
            return code, buffer.getvalue()

        code, text = run(
            [
                "write",
                "--version", "0.5.16",
                "--commit", "0123456789abcdef0123456789abcdef01234567",
                "--artifact", str(first),
                "--artifact", str(second),
                "--root", str(root),
                "--out", str(out),
            ]
        )
        record = out / "v0.5.16.json"
        if code != 0 or not record.is_file():
            failures.append(f"a record was not written, exit {code}")
        else:
            data = json.loads(record.read_text(encoding="utf-8"))
            if data.get("commit") != "0123456789abcdef0123456789abcdef01234567":
                failures.append("the record did not keep the commit it was given")
            if len(data.get("artifacts") or []) != 2:
                failures.append("the record did not carry one entry per artifact")
            if sha256_of(first) != (data["artifacts"][0]["sha256"]):
                failures.append("the recorded digest is not the digest of the file")
            if data["artifacts"][0]["path"] != "dist/wc3-mcp.exe":
                failures.append(f"the recorded path was {data['artifacts'][0]['path']!r}")

            code, text = run(["verify", str(record), "--root", str(root)])
            if code != 0:
                failures.append(f"a fresh record did not verify, exit {code}, {text.strip()}")

            first.write_bytes(b"a different build, same name")
            code, text = run(["verify", str(record), "--root", str(root)])
            if code != 2:
                failures.append(f"a changed artifact did not fail, exit {code}")
            if "DRIFT" not in text or "wc3-mcp.exe" not in text:
                failures.append("the changed artifact was not named")

            code, text = run(["resolve", "--version", "0.5.16", "--out", str(out), "--root", str(root)])
            if code != 2:
                failures.append("resolving a record whose artifact changed did not fail")

            code, text = run(["resolve", "--version", "0.5.19", "--out", str(out), "--root", str(root)])
            if code != 2:
                failures.append("a version with no record resolved to something")

            code, text = run(
                [
                    "write",
                    "--version", "0.5.16",
                    "--tag", "v0.5.19",
                    "--commit", "0123456789abcdef0123456789abcdef01234567",
                    "--artifact", str(first),
                    "--root", str(root),
                    "--out", str(out),
                ]
            )
            if code != 2:
                failures.append("a tag that does not name the version was accepted")

            code, text = run(
                [
                    "write",
                    "--version", "0.5.16",
                    "--commit", "0123456789abcdef0123456789abcdef01234567",
                    "--artifact", str(first),
                    "--root", str(root),
                    "--out", str(out),
                ]
            )
            if code != 2:
                failures.append("a second record for one tag was written over the first")

            # A record whose artifacts are the URLs the release served resolves through the
            # digest published beside each one, so a version needs no 55 MB download.
            tag_commit = "42a9e23c36b9bd1fbacbdd550511e399c542264c"
            published = {
                "record": SCHEMA,
                "version": "0.5.21",
                "tag": "v0.5.21",
                "commit": "42a9e23",
                "artifacts": [
                    {
                        "name": "wc3-mcp-v0.5.21-win-x64.zip",
                        "path": "https://example.invalid/v0.5.21/wc3-mcp-v0.5.21-win-x64.zip",
                        "bytes": 55191028,
                        "sha256": "e3" + "0" * 62,
                    }
                ],
            }
            served = published["artifacts"][0]["sha256"]

            def sidecar(digest: str):
                def read(url: str) -> str:
                    if not url.endswith(".sha256"):
                        raise RecordError(f"{url} is not a digest sidecar")
                    return f"{digest}  wc3-mcp-v0.5.21-win-x64.zip\n"

                return read

            def unreachable(url: str) -> str:
                raise RecordError(f"{url} could not be read, no answer")

            rows = judge(published, root, fetch=sidecar(served), tag_commit=tag_commit)
            if not all(row["ok"] for row in rows):
                failures.append(
                    "a published record did not resolve, "
                    + "; ".join(row["detail"] for row in rows if not row["ok"])
                )

            rows = judge(published, root, fetch=sidecar("f" * 64), tag_commit=tag_commit)
            if all(row["ok"] for row in rows) or not any(
                "the release publishes" in row["detail"] for row in rows
            ):
                failures.append("a published digest that differs from the record was not named")

            rows = judge(published, root, fetch=unreachable, tag_commit=tag_commit)
            if not any(
                row["found"] == "not read" and "not checked" in row["detail"] for row in rows
            ):
                failures.append("a digest that could not be read was not reported as unchecked")

            rows = judge(published, root, fetch=sidecar(served), tag_commit="f" * 40)
            if not any(
                row["claim"] == "Source commit is the tag's commit" and not row["ok"]
                for row in rows
            ):
                failures.append("a record that names no commit of the tag was accepted")

            # Two writers race on one tag. The create is exclusive, so exactly one record is
            # made, the other writer is told it is there, and the record on disk is whole.
            race = root / "race"
            race.mkdir()
            racer = race / "artifact.bin"
            racer.write_bytes(b"the artifact the race writes about\n")
            writer = [
                sys.executable,
                str(Path(__file__).resolve()),
                "write",
                "--version",
                "0.5.23",
                "--commit",
                "0123456789abcdef0123456789abcdef01234567",
                "--artifact",
                str(racer),
                "--root",
                str(race),
                "--out",
                str(race),
            ]
            writers = [
                subprocess.Popen(writer, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
                for _ in range(2)
            ]
            codes = sorted(process.wait(timeout=120) for process in writers)
            for process in writers:
                if process.stdout:
                    process.stdout.close()
                if process.stderr:
                    process.stderr.close()
            if codes != [0, 2]:
                failures.append(f"two writers on one tag exited {codes}, expected one 0 and one 2")
            contested = race / "v0.5.23.json"
            if not contested.is_file():
                failures.append("two writers on one tag left no record")
            else:
                try:
                    json.loads(contested.read_text(encoding="utf-8"))
                except ValueError as exc:
                    failures.append(f"two writers on one tag left a half written record, {exc}")

            code, text = run(
                [
                    "write",
                    "--version",
                    "0.5.23",
                    "--commit",
                    "0123456789abcdef0123456789abcdef01234567",
                    "--artifact",
                    str(racer),
                    "--root",
                    str(race),
                    "--out",
                    str(race),
                    "--force",
                ]
            )
            if code != 0:
                failures.append(
                    f"a record refused by a race was not replaceable with --force, exit {code}"
                )

    if failures:
        for failure in failures:
            print("selftest failed, " + failure, file=sys.stderr)
        return 1
    print(
        "selftest ok, a record carries the commit and one digest per artifact, a changed "
        "artifact fails verify and resolve, a wrong tag or a second record is refused, "
        "a published record resolves through the digest beside the asset, an unread digest "
        "is called unchecked, a commit that is not the tag's is refused, and two writers on "
        "one tag make one record while the other is refused and can be replaced with --force"
    )
    return 0


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(
        description="Write, verify and resolve the record that explains a published version."
    )
    parser.add_argument("--json", action="store_true", help="print the result as JSON")
    parser.add_argument("--selftest", action="store_true", help="run the built in selftest")
    commands = parser.add_subparsers(dest="command")

    writer = commands.add_parser("write", help="write the record for one release")
    writer.add_argument("--version", required=True, help="the version the release publishes")
    writer.add_argument("--tag", help="the tag, which defaults to v and the version")
    writer.add_argument("--commit", help="the source commit, read from git when absent")
    writer.add_argument("--artifact", action="append", default=[], help="one published file, repeatable")
    writer.add_argument("--root", default=".", help="the tree the artifact paths are relative to")
    writer.add_argument("--out", default=str(DEFAULT_OUT), help="the folder the record is written to")
    writer.add_argument("--force", action="store_true", help="replace a record that already exists")
    writer.add_argument("--json", action="store_true", help="print the record as JSON")

    checker = commands.add_parser("verify", help="recompute the digests of one record")
    checker.add_argument("record", help="the record to check")
    checker.add_argument("--root", default=".", help="the tree the artifact paths are relative to")
    checker.add_argument("--repo", help="the checkout whose tag the record's commit is compared with")
    checker.add_argument("--json", action="store_true", help="print the result as JSON")

    resolver = commands.add_parser("resolve", help="answer which record explains a version")
    resolver.add_argument("--version", help="the published version to explain")
    resolver.add_argument("--tag", help="the tag to explain, when the version is not at hand")
    resolver.add_argument("--out", default=str(DEFAULT_OUT), help="the folder the records live in")
    resolver.add_argument("--root", default=".", help="the tree the artifact paths are relative to")
    resolver.add_argument("--repo", help="the checkout whose tag the record's commit is compared with")
    resolver.add_argument("--json", action="store_true", help="print the result as JSON")

    args = parser.parse_args(argv)
    if args.selftest:
        return selftest()
    if not args.command:
        parser.print_help()
        return 1

    try:
        if args.command == "write":
            return write(args)
        if args.command == "verify":
            return verify(args)
        return resolve(args)
    except RecordError as exc:
        print(str(exc), file=sys.stderr)
        return 2


if __name__ == "__main__":
    sys.exit(main())
