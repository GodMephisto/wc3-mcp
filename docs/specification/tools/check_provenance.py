"""Assert a published artifact names its commit, its digest and its tag.

PKG-13 says every published artifact names its source commit, its artifact
digest and the tag it was built from, and that a version number alone is not
provenance. This reads a release asset folder, the tag it belongs to, and the
`.sha256` file beside each artifact, recomputes every digest from the bytes, and
prints one row per artifact.

Provenance can come from either form of record the release writes.

  * a `release.json` in the folder, the record `tools/release_record.py` writes
    at release time, carrying the tag, the full commit and each artifact digest
  * a sidecar per artifact, `<name>.provenance.json`, or `<name>.commit`, for a
    tree that publishes one file at a time

The record must name the same tag as the one being checked, and its commit must
be the commit the tag points at in the repository. A record that names only a
version is refused, because a version number alone cannot be resolved to source.

    python tools/check_provenance.py
    python tools/check_provenance.py --tag v0.5.16
    python tools/check_provenance.py --assets dist/release
    python tools/check_provenance.py --git "C:\\Program Files\\Git\\bin\\git.exe"
    python tools/check_provenance.py --selftest

Exit 0 when every artifact carries a resolved commit, a matching digest and the
tag, 2 when one does not, or when the tag names no artifact at all, and 1 when
the repository or the tag cannot be read.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import os
import re
import subprocess
import sys
import tempfile
from pathlib import Path

HERE = Path(__file__).resolve().parent
REPO = HERE.parent.parent.parent

DEFAULT_GIT = r"C:\Program Files\Git\bin\git.exe"

TAG_RE = re.compile(r"^v?(\d+)\.(\d+)\.(\d+)$")
COMMIT_RE = re.compile(r"^[0-9a-f]{40}$")
SHA_RE = re.compile(r"\b([0-9a-fA-F]{64})\b")

RECORD_NAME = "release.json"
# A folder is a release folder when it holds a record or a digest sidecar.
RELEASE_MARKERS = (RECORD_NAME, "*.sha256")
# Usual places a release folder lands, checked only when none is named.
CANDIDATES = ("dist/release", "release", "dist", "artifacts", "out")


class ProvenanceError(RuntimeError):
    """An input that cannot be read at all."""


def sort_key(text: str) -> tuple[int, int, int]:
    match = TAG_RE.match(text.strip())
    return tuple(int(part) for part in match.groups()) if match else (-1, -1, -1)


def git_binary(explicit: str | None) -> str:
    if explicit:
        return explicit
    if os.name == "nt" and Path(DEFAULT_GIT).is_file():
        return DEFAULT_GIT
    return "git"


def git(repo: Path, git_path: str, *args: str) -> str:
    try:
        result = subprocess.run(
            [git_path, "-C", str(repo).replace("\\", "/"), *args],
            capture_output=True,
            text=True,
            timeout=60,
        )
    except (OSError, subprocess.SubprocessError) as exc:
        raise ProvenanceError(f"git could not be run at {git_path}, {exc}") from exc
    if result.returncode != 0:
        raise ProvenanceError(
            "git " + " ".join(args) + " failed, " + result.stderr.strip()
        )
    return result.stdout.strip()


def newest_tag(repo: Path, git_path: str) -> str:
    tags = [line.strip() for line in git(repo, git_path, "tag", "--list").splitlines()]
    versioned = [tag for tag in tags if TAG_RE.match(tag)]
    if not versioned:
        raise ProvenanceError(f"no vX.Y.Z tag in {repo}")
    return max(versioned, key=sort_key)


def tag_commit(repo: Path, git_path: str, tag: str) -> str:
    """The commit the tag points at, following an annotated tag to its target."""
    commit = git(repo, git_path, "rev-list", "-n", "1", tag)
    if not COMMIT_RE.match(commit):
        raise ProvenanceError(f"tag {tag} resolves to {commit!r}, which is no commit")
    return commit


def digest_of(path: Path) -> str:
    sha = hashlib.sha256()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(1 << 20), b""):
            sha.update(chunk)
    return sha.hexdigest()


def read_digest(path: Path) -> str:
    try:
        text = path.read_text(encoding="utf-8-sig", errors="replace")
    except OSError as exc:
        raise ProvenanceError(f"{path} could not be read, {exc}") from exc
    match = SHA_RE.search(text)
    if not match:
        raise ProvenanceError(f"{path} holds no sha256 digest")
    return match.group(1).lower()


def find_assets(repo: Path, explicit: str | None) -> Path | None:
    """The folder holding release assets, or None when the tree holds none."""
    if explicit:
        folder = Path(explicit)
        if not folder.is_absolute():
            folder = repo / folder
        if not folder.is_dir():
            raise ProvenanceError(f"no asset folder at {folder}")
        return folder
    for name in CANDIDATES:
        folder = repo / name
        if not folder.is_dir():
            continue
        if (folder / RECORD_NAME).is_file():
            return folder
        if any(folder.glob("*.sha256")):
            return folder
    return None


def artifacts_in(folder: Path) -> list[Path]:
    """Every file that looks like an artifact, not a record and not a digest."""
    found = []
    for path in sorted(folder.iterdir()):
        if not path.is_file():
            continue
        name = path.name
        if name.startswith(".") or name == RECORD_NAME:
            continue
        if name.endswith(".sha256") or name.endswith(".provenance.json"):
            continue
        if name.endswith(".commit"):
            continue
        found.append(path)
    return found


def record_provenance(folder: Path, tag: str) -> tuple[str | None, str | None]:
    """The commit and tag a release record names, and the reason when it cannot."""
    record = folder / RECORD_NAME
    if not record.is_file():
        return None, None
    try:
        document = json.loads(record.read_text(encoding="utf-8-sig"))
    except OSError as exc:
        raise ProvenanceError(f"{record} could not be read, {exc}") from exc
    except ValueError as exc:
        raise ProvenanceError(f"{record} is not valid JSON, {exc}") from exc
    if not isinstance(document, dict):
        return None, "release.json is not a JSON object"
    named_tag = document.get("tag")
    commit = document.get("commit")
    if not isinstance(commit, str) or not COMMIT_RE.match(commit):
        if "version" in document or isinstance(named_tag, str):
            return None, "release.json names a version and no full commit, and a version alone is not provenance"
        return None, "release.json names no full 40 character commit"
    if not isinstance(named_tag, str):
        return None, "release.json names a commit and no tag"
    if named_tag.strip() != tag.strip():
        return None, f"release.json names tag {named_tag} while the checked tag is {tag}"
    if not document.get("artifacts"):
        return None, "release.json lists no artifact digest"
    return commit, None


def sidecar_provenance(path: Path, tag: str) -> tuple[str | None, str | None]:
    """The commit a per artifact sidecar names, and the reason when it cannot."""
    for name, reader in (
        (path.name + ".provenance.json", "json"),
        (path.name + ".commit", "text"),
    ):
        sidecar = path.with_name(name)
        if not sidecar.is_file():
            continue
        try:
            text = sidecar.read_text(encoding="utf-8-sig", errors="replace")
        except OSError as exc:
            raise ProvenanceError(f"{sidecar} could not be read, {exc}") from exc
        if reader == "json":
            try:
                document = json.loads(text)
            except ValueError as exc:
                raise ProvenanceError(f"{sidecar} is not valid JSON, {exc}") from exc
            commit = document.get("commit") if isinstance(document, dict) else None
            named_tag = document.get("tag") if isinstance(document, dict) else None
            if isinstance(named_tag, str) and named_tag.strip() != tag.strip():
                return None, f"{sidecar.name} names tag {named_tag} while the checked tag is {tag}"
        else:
            commit = text.strip().split()[0] if text.strip() else None
        if not isinstance(commit, str) or not COMMIT_RE.match(commit.strip().lower()):
            return None, f"{sidecar.name} names no full 40 character commit"
        return commit.strip().lower(), None
    return None, None


def assess(folder: Path, tag: str, expected_commit: str) -> tuple[list[dict], list[str]]:
    """One row per artifact, and every reason an artifact is refused."""
    rows: list[dict] = []
    problems: list[str] = []
    record_commit, record_problem = record_provenance(folder, tag)

    artifacts = artifacts_in(folder)
    if not artifacts:
        return rows, [
            f"{tag} names no artifact in {folder}, so the tag is a version number and not provenance"
        ]

    for path in artifacts:
        row = {
            "artifact": path.name,
            "digest": "",
            "commit": "",
            "tag": "",
            "verdict": "ok",
            "detail": "",
        }
        commit = record_commit
        detail = record_problem or ""

        if commit is None:
            commit, sidecar_problem = sidecar_provenance(path, tag)
            detail = sidecar_problem or detail

        sidecar = path.with_name(path.name + ".sha256")
        if not sidecar.is_file():
            row["verdict"] = "refused"
            row["detail"] = detail or "no .sha256 beside the artifact"
            problems.append(f"{path.name} carries no .sha256, so its digest is unstated")
            rows.append(row)
            continue

        stated = read_digest(sidecar)
        actual = digest_of(path)
        row["digest"] = actual[:16]
        if stated != actual:
            row["verdict"] = "refused"
            row["detail"] = "the .sha256 does not match the bytes"
            problems.append(
                f"{path.name} sha256 is {actual} and its .sha256 says {stated}"
            )
            rows.append(row)
            continue

        if commit is None:
            row["verdict"] = "refused"
            row["detail"] = detail or "no record names its source commit"
            problems.append(
                f"{path.name} carries a digest and no source commit, so the tag alone is a version number"
            )
            rows.append(row)
            continue

        row["commit"] = commit[:12]
        row["tag"] = tag
        if commit != expected_commit:
            row["verdict"] = "refused"
            row["detail"] = f"commit {commit} is not the commit {tag} points at"
            problems.append(
                f"{path.name} names commit {commit} while {tag} points at {expected_commit}"
            )
        rows.append(row)

    return rows, problems


def render(rows: list[dict], tag: str, commit: str, folder: Path) -> str:
    lines = [
        f"tag {tag}",
        f"commit {commit}",
        f"assets {folder}",
        "",
    ]
    if not rows:
        return "\n".join(lines)
    widths = {
        "artifact": max([len("Artifact")] + [len(r["artifact"]) for r in rows]),
        "digest": max([len("Digest")] + [len(r["digest"] or "-") for r in rows]),
        "commit": max([len("Commit")] + [len(r["commit"] or "-") for r in rows]),
        "tag": max([len("Tag")] + [len(r["tag"] or "-") for r in rows]),
    }
    head = (
        f"{'Artifact'.ljust(widths['artifact'])}  "
        f"{'Digest'.ljust(widths['digest'])}  "
        f"{'Commit'.ljust(widths['commit'])}  "
        f"{'Tag'.ljust(widths['tag'])}  Verdict"
    )
    lines += [head, "-" * len(head)]
    for row in rows:
        detail = f"  {row['detail']}" if row["detail"] else ""
        lines.append(
            f"{row['artifact'].ljust(widths['artifact'])}  "
            f"{(row['digest'] or '-').ljust(widths['digest'])}  "
            f"{(row['commit'] or '-').ljust(widths['commit'])}  "
            f"{(row['tag'] or '-').ljust(widths['tag'])}  {row['verdict']}{detail}"
        )
    return "\n".join(lines)


def selftest() -> int:
    """Prove a record passes, a tampered byte fails, and a version alone fails."""
    failures: list[str] = []
    commit = "0123456789abcdef0123456789abcdef01234567"
    tag = "v0.5.20"

    with tempfile.TemporaryDirectory(prefix="check-provenance-") as folder:
        root = Path(folder)

        def release(name: str, payload: bytes = b"artifact payload\n") -> Path:
            assets = root / name
            assets.mkdir()
            artifact = assets / "wc3-mcp-win-x64.zip"
            artifact.write_bytes(payload)
            digest = hashlib.sha256(payload).hexdigest()
            (assets / "wc3-mcp-win-x64.zip.sha256").write_text(
                f"{digest}  wc3-mcp-win-x64.zip\n", encoding="utf-8"
            )
            (assets / RECORD_NAME).write_text(
                json.dumps(
                    {
                        "tag": tag,
                        "commit": commit,
                        "artifacts": [
                            {"name": artifact.name, "sha256": digest}
                        ],
                    }
                ),
                encoding="utf-8",
            )
            return assets

        good = release("good")
        rows, problems = assess(good, tag, commit)
        if problems or not rows or rows[0]["verdict"] != "ok":
            failures.append("a complete release was refused, " + "; ".join(problems))

        tampered = release("tampered")
        with (tampered / "wc3-mcp-win-x64.zip").open("ab") as handle:
            handle.write(b"x")
        rows, problems = assess(tampered, tag, commit)
        if not problems or not any("sha256" in problem for problem in problems):
            failures.append("a changed byte was not caught by the digest")

        nosum = release("nosum")
        (nosum / "wc3-mcp-win-x64.zip.sha256").unlink()
        rows, problems = assess(nosum, tag, commit)
        if not any(".sha256" in problem for problem in problems):
            failures.append("a missing digest sidecar was not reported")

        versiononly = release("versiononly")
        (versiononly / RECORD_NAME).write_text(
            json.dumps({"version": "0.5.20", "tag": tag}), encoding="utf-8"
        )
        rows, problems = assess(versiononly, tag, commit)
        if not any("version" in problem for problem in problems):
            failures.append("a version only record was not refused as provenance")

        wrongtag = release("wrongtag")
        (wrongtag / RECORD_NAME).write_text(
            json.dumps({"tag": "v0.5.19", "commit": commit, "artifacts": [{"name": "x"}]}),
            encoding="utf-8",
        )
        rows, problems = assess(wrongtag, "v0.5.20", commit)
        if not problems:
            failures.append("a record for another tag was accepted")

        empty = root / "empty"
        empty.mkdir()
        rows, problems = assess(empty, tag, commit)
        if not any("names no artifact" in problem for problem in problems):
            failures.append("an empty release folder was accepted")

    if failures:
        for failure in failures:
            print("selftest failed, " + failure, file=sys.stderr)
        return 1
    print("selftest ok, a record passes, a changed byte, a missing digest, a version only record and an empty folder all fail")
    return 0


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description="Assert each artifact names its commit, digest and tag.")
    parser.add_argument("--repo", default=str(REPO), help="the checkout whose tags are read")
    parser.add_argument("--tag", default=None, help="the published tag, vX.Y.Z, default the newest")
    parser.add_argument("--assets", default=None, help="the release asset folder")
    parser.add_argument("--git", default=None, help="the git executable, the full path when PATH is broken")
    parser.add_argument("--json", action="store_true", help="print the result as JSON")
    parser.add_argument("--selftest", action="store_true", help="run the built in selftest")
    args = parser.parse_args(argv)

    if args.selftest:
        return selftest()

    repo = Path(args.repo).resolve()
    if not repo.is_dir():
        print(f"no repository at {repo}", file=sys.stderr)
        return 1

    git_path = git_binary(args.git)
    try:
        tag = args.tag or newest_tag(repo, git_path)
        if not TAG_RE.match(tag):
            raise ProvenanceError(f"tag {tag!r} is not vX.Y.Z")
        commit = tag_commit(repo, git_path, tag)
        folder = find_assets(repo, args.assets)
    except ProvenanceError as exc:
        print(str(exc), file=sys.stderr)
        return 1

    if folder is None:
        reason = (
            f"{tag} names no release asset folder in {repo}, so no artifact records "
            "its digest and source commit, and the tag name alone is a version number"
        )
        if args.json:
            print(json.dumps({"ok": False, "tag": tag, "commit": commit, "assets": None, "problems": [reason]}, indent=2))
        else:
            print(f"tag {tag}")
            print(f"commit {commit}")
            print("assets none")
            print()
            print("refused, " + reason)
        return 2

    try:
        rows, problems = assess(folder, tag, commit)
    except ProvenanceError as exc:
        print(str(exc), file=sys.stderr)
        return 1

    if args.json:
        print(
            json.dumps(
                {
                    "ok": not problems,
                    "tag": tag,
                    "commit": commit,
                    "assets": str(folder),
                    "rows": rows,
                    "problems": problems,
                },
                indent=2,
                ensure_ascii=False,
            )
        )
    else:
        print(render(rows, tag, commit, folder))
        print()
        if problems:
            print(f"{len(problems)} problem(s)")
            for problem in problems:
                print("  " + problem)
        else:
            print(f"all {len(rows)} artifacts carry a source commit, a matching digest and the tag")

    return 0 if not problems else 2


if __name__ == "__main__":
    sys.exit(main())
