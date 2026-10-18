---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "tools/check_renderer_evidence_current.py"
ref: HEAD
---

#!/usr/bin/env python3
"""Check whether local renderer PR evidence files match the current source HEAD."""

from __future__ import annotations

import argparse
import json
from pathlib import Path
import subprocess
import sys
import tempfile

sys.dont_write_bytecode = True


DEFAULT_EVIDENCE_PATHS = (
    {
        "name": "debug-package",
        "path": Path("build/macOS-Debug-WithTests/package-validation/debug-package-report.json"),
    },
    {
        "name": "release-package",
        "path": Path("build/macOS-Release/package-validation/release-package-report.json"),
    },
    {
        "name": "metal-capture",
        "path": Path("build/macOS-Debug-WithTests/pr-evidence/metal/metal-artifacts/renderer-parity-capture.json"),
    },
    {
        "name": "metal-mpq-capture",
        "path": Path("build/macOS-Debug-WithTests/pr-evidence/metal/metal-mpq-map-artifacts/renderer-parity-capture.json"),
    },
    {
        "name": "full-metal-smoke",
        "path": Path("build/macOS-Debug-WithTests/package-validation/full-metal-smoke-artifacts/metal-smoke-provenance.json"),
    },
    {
        "name": "opengl-fallback",
        "path": Path("build/macOS-Debug-OpenGL/HiveWE.app/Contents/Resources/hivewe-build-provenance.json"),
        "cmake": {
            "enable_metal": "0",
            "build_opengl_renderer": "1",
        },
    },
)


class EvidenceCurrentError(RuntimeError):
    pass


def git_head(repo_root: Path) -> str:
    try:
        result = subprocess.run(
            ["git", "-C", str(repo_root), "rev-parse", "HEAD"],
            check=True,
            capture_output=True,
            text=True,
            timeout=5,
        )
    except (FileNotFoundError, subprocess.CalledProcessError, subprocess.TimeoutExpired) as error:
        raise EvidenceCurrentError(f"unable to read current git HEAD for {repo_root}") from error

    head = result.stdout.strip()
    if not head:
        raise EvidenceCurrentError(f"git HEAD is empty for {repo_root}")
    return head


def git_status_short(repo_root: Path) -> str:
    try:
        result = subprocess.run(
            ["git", "-C", str(repo_root), "status", "--short"],
            check=True,
            capture_output=True,
            text=True,
            timeout=5,
        )
    except (FileNotFoundError, subprocess.CalledProcessError, subprocess.TimeoutExpired) as error:
        raise EvidenceCurrentError(f"unable to read current git status for {repo_root}") from error
    return result.stdout.strip()


def read_json_object(path: Path) -> dict:
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as error:
        raise EvidenceCurrentError(f"invalid JSON in {path}: {error}") from error
    if not isinstance(data, dict):
        raise EvidenceCurrentError(f"invalid JSON in {path}: expected object")
    return data


def source_revision(data: dict) -> tuple[str | None, bool | None]:
    source = data.get("source_revision")
    if not isinstance(source, dict):
        return None, None

    head = source.get("head")
    dirty = source.get("dirty")
    return head if isinstance(head, str) else None, dirty if isinstance(dirty, bool) else None


def matching_cmake_fields(data: dict, expected_fields: dict[str, str] | None) -> tuple[bool, str | None]:
    if not expected_fields:
        return True, None
    cmake = data.get("cmake")
    if not isinstance(cmake, dict):
        return False, "cmake metadata is missing"
    for key, expected in expected_fields.items():
        actual = cmake.get(key)
        if actual != expected:
            return False, f"cmake.{key} records {actual!r}, expected {expected!r}"
    return True, None


def check_evidence(repo_root: Path, expected_head: str, allow_missing: bool) -> tuple[list[dict], bool]:
    rows: list[dict] = []
    ok = True

    for evidence in DEFAULT_EVIDENCE_PATHS:
        name = evidence["name"]
        relative_path = evidence["path"]
        path = repo_root / relative_path
        if not path.is_file():
            status = "missing"
            detail = f"{relative_path} is missing"
            current = False
        else:
            data = read_json_object(path)
            head, dirty = source_revision(data)
            cmake_matches, cmake_detail = matching_cmake_fields(data, evidence.get("cmake"))
            if head is None:
                status = "invalid"
                detail = "source_revision.head is missing"
                current = False
            elif dirty is not False:
                status = "dirty"
                detail = f"source_revision records dirty={dirty!r} at {head[:12]}"
                current = False
            elif head != expected_head:
                status = "stale"
                detail = f"recorded {head[:12]}, expected {expected_head[:12]}"
                current = False
            elif not cmake_matches:
                status = "invalid"
                detail = cmake_detail or "cmake metadata does not match"
                current = False
            else:
                status = "current"
                detail = f"recorded {head[:12]}"
                current = True

        if not current and not (allow_missing and status == "missing"):
            ok = False
        rows.append({
            "name": name,
            "path": str(relative_path),
            "status": status,
            "detail": detail,
        })

    return rows, ok


def print_rows(rows: list[dict], expected_head: str, current_dirty: bool) -> None:
    print(f"Expected source HEAD: {expected_head}")
    print(f"Current source dirty: {str(current_dirty).lower()}")
    for row in rows:
        print(f"{row['status']:>7}  {row['name']:<18} {row['path']} ({row['detail']})")


def run_self_test() -> None:
    with tempfile.TemporaryDirectory(prefix="hivewe-evidence-current-") as temp:
        root = Path(temp)
        head = "a" * 40
        for evidence in DEFAULT_EVIDENCE_PATHS:
            relative_path = evidence["path"]
            path = root / relative_path
            path.parent.mkdir(parents=True, exist_ok=True)
            data: dict[str, object] = {"source_revision": {"head": head, "dirty": False}}
            if "cmake" in evidence:
                data["cmake"] = dict(evidence["cmake"])
            path.write_text(json.dumps(data), encoding="utf-8")

        rows, ok = check_evidence(root, head, allow_missing=False)
        if not ok or any(row["status"] != "current" for row in rows):
            raise AssertionError("current evidence should pass")

        stale_path = root / DEFAULT_EVIDENCE_PATHS[0]["path"]
        stale_path.write_text(json.dumps({"source_revision": {"head": "b" * 40, "dirty": False}}), encoding="utf-8")
        rows, ok = check_evidence(root, head, allow_missing=False)
        if ok or rows[0]["status"] != "stale":
            raise AssertionError("stale evidence should fail")

        stale_path.unlink()
        rows, ok = check_evidence(root, head, allow_missing=True)
        if not ok or rows[0]["status"] != "missing":
            raise AssertionError("--allow-missing should tolerate missing evidence")

        opengl_path = root / DEFAULT_EVIDENCE_PATHS[-1]["path"]
        opengl_path.write_text(
            json.dumps({
                "source_revision": {"head": head, "dirty": False},
                "cmake": {"enable_metal": "1", "build_opengl_renderer": "1"},
            }),
            encoding="utf-8",
        )
        rows, ok = check_evidence(root, head, allow_missing=False)
        if ok or rows[-1]["status"] != "invalid":
            raise AssertionError("OpenGL fallback evidence should reject Metal-enabled provenance")

    print("check_renderer_evidence_current self-test passed.")


def parse_args(argv: list[str]) -> argparse.Namespace:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--repo-root", type=Path, default=Path.cwd(), help="Repository root to inspect. Defaults to cwd.")
    parser.add_argument("--require-head", help="Expected source HEAD. Defaults to `git rev-parse HEAD`.")
    parser.add_argument("--allow-missing", action="store_true", help="Do not fail only because a default evidence file is missing.")
    parser.add_argument("--self-test", action="store_true", help="Run the tool self-test and exit.")
    return parser.parse_args(argv)


def main(argv: list[str]) -> int:
    args = parse_args(argv)
    if args.self_test:
        run_self_test()
        return 0

    repo_root = args.repo_root.resolve()
    expected_head = args.require_head or git_head(repo_root)
    current_dirty = bool(git_status_short(repo_root))
    rows, ok = check_evidence(repo_root, expected_head, args.allow_missing)
    print_rows(rows, expected_head, current_dirty)

    if current_dirty:
        print("Current source tree is dirty; regenerate maintainer-facing evidence from a clean tree.")
        ok = False

    if not ok:
        print("Renderer evidence is not current for the requested source revision.")
        return 1

    print("Renderer evidence is current for the requested source revision.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))
