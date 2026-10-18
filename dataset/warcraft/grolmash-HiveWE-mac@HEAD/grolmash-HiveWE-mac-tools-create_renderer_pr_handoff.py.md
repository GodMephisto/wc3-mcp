---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "tools/create_renderer_pr_handoff.py"
ref: HEAD
---

#!/usr/bin/env python3
"""Assemble deterministic renderer PR evidence handoff bundles."""

from __future__ import annotations

import argparse
from datetime import datetime, timezone
import hashlib
import json
from pathlib import Path
import shutil
import sys
import tempfile
from typing import Iterable
import zipfile

sys.dont_write_bytecode = True

CAPTURE_MANIFEST_NAME = "renderer-parity-capture.json"
CAPTURE_MANIFEST_SCHEMA = 8
SMOKE_PROVENANCE_NAME = "metal-smoke-provenance.json"
PACKAGE_REPORT_SCHEMA = 3
PACKAGE_REPORT_LABEL = "hivewe-macos-package-validation"
REQUIRED_PACKAGE_REPORT_CHECKS = (
    "bundle_files",
    "build_provenance",
    "bundled_test_map",
    "bundled_warcraft_fixture",
    "bundled_metal_shader",
    "no_forbidden_dylibs",
    "codesign",
    "reported_backend",
    "save_reload_smoke",
    "mutated_save_reload_smoke",
    "mpq_export_reload_smoke",
    "renderer_probe_smoke",
    "renderer_probe_parity_smoke",
    "map_canvas_smoke",
    "map_render_parity_smoke",
    "mpq_map_render_parity_smoke",
    "preview_probe_smoke",
    "preview_probe_parity_smoke",
    "shared_preview_probe_smoke",
    "shared_preview_parity_smoke",
    "cpack",
    "dmg_verified",
    "dmg_contents",
)


class HandoffError(RuntimeError):
    pass


def sha256_file(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as file:
        for chunk in iter(lambda: file.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def sha256_bytes(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def file_entry(root: Path, path: Path) -> dict[str, object]:
    return {
        "path": path.relative_to(root).as_posix(),
        "size_bytes": path.stat().st_size,
        "sha256": sha256_file(path),
    }


def read_json(path: Path) -> dict:
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except (OSError, json.JSONDecodeError) as error:
        raise HandoffError(f"could not read JSON file {path}: {error}") from error
    if not isinstance(data, dict):
        raise HandoffError(f"expected JSON object in {path}")
    return data


def copy_file(source: Path, destination: Path) -> Path:
    if not source.is_file():
        raise HandoffError(f"required input file is missing: {source}")
    destination.parent.mkdir(parents=True, exist_ok=True)
    shutil.copy2(source, destination)
    return destination


def copy_tree(source: Path, destination: Path) -> Path:
    if not source.is_dir():
        raise HandoffError(f"required input directory is missing: {source}")
    if destination.exists():
        shutil.rmtree(destination)
    shutil.copytree(source, destination)
    return destination


def recorded_path_matches(recorded: str, expected: str) -> bool:
    recorded_path = Path(recorded)
    expected_path = Path(expected)
    if not recorded_path.is_absolute():
        recorded_path = Path.cwd() / recorded_path
    if not expected_path.is_absolute():
        expected_path = Path.cwd() / expected_path
    return recorded_path.resolve(strict=False) == expected_path.resolve(strict=False)


def executable_identity_from_package_report(path: Path, expected_build_type: str) -> dict[str, object]:
    report = read_json(path)
    app = report.get("app")
    if not isinstance(app, str) or not app:
        raise HandoffError(f"{expected_build_type} package report does not record installed app path: {path}")
    executable = report.get("app_executable")
    if not isinstance(executable, dict):
        raise HandoffError(f"{expected_build_type} package report is missing app executable metadata: {path}")
    executable_path = executable.get("path")
    executable_size = executable.get("size_bytes")
    executable_hash = executable.get("sha256")
    if not isinstance(executable_path, str) or not executable_path:
        raise HandoffError(f"{expected_build_type} package report does not record app executable path: {path}")
    if not isinstance(executable_size, int) or executable_size <= 0:
        raise HandoffError(f"{expected_build_type} package report does not record a valid app executable size: {path}")
    if not isinstance(executable_hash, str) or len(executable_hash) != 64:
        raise HandoffError(f"{expected_build_type} package report does not record app executable SHA-256: {path}")
    return {
        "app": app,
        "executable": executable_path,
        "size_bytes": executable_size,
        "sha256": executable_hash,
    }


def source_head_from_package_report(path: Path, expected_build_type: str) -> str:
    report = read_json(path)
    if report.get("schema") != PACKAGE_REPORT_SCHEMA or report.get("label") != PACKAGE_REPORT_LABEL:
        raise HandoffError(f"package report has unsupported schema or label: {path}")
    if report.get("status") != "success":
        raise HandoffError(f"package report does not record a successful validation: {path}")

    source_revision = report.get("source_revision")
    head = source_revision.get("head") if isinstance(source_revision, dict) else None
    if not isinstance(head, str) or not head:
        raise HandoffError(f"package report does not record source_revision.head: {path}")
    if source_revision.get("dirty") is not False:
        raise HandoffError(f"package report does not record a clean source revision: {path}")

    expected = report.get("expected")
    if not isinstance(expected, dict) or expected.get("build_type") != expected_build_type:
        raise HandoffError(f"package report does not record expected build_type={expected_build_type!r}: {path}")
    cmake = report.get("cmake")
    if not isinstance(cmake, dict) or cmake.get("build_type") != expected_build_type:
        raise HandoffError(f"package report does not record CMake build_type={expected_build_type!r}: {path}")

    checks = report.get("checks")
    if not isinstance(checks, dict):
        raise HandoffError(f"package report is missing check results: {path}")
    missing_checks = [name for name in REQUIRED_PACKAGE_REPORT_CHECKS if checks.get(name) is not True]
    if missing_checks:
        raise HandoffError(
            f"package report is missing required passing check(s): {', '.join(missing_checks)}: {path}"
        )

    artifacts = report.get("artifacts")
    if not isinstance(artifacts, dict):
        raise HandoffError(f"package report is missing artifact metadata: {path}")
    for name in ("built_dmg", "verified_dmg"):
        if not isinstance(artifacts.get(name), str) or not artifacts[name]:
            raise HandoffError(f"package report is missing {name} artifact metadata: {path}")
    executable_identity_from_package_report(path, expected_build_type)
    return head


def capture_manifest_from_archive(path: Path) -> dict:
    if not path.is_file():
        raise HandoffError(f"required capture archive is missing: {path}")
    try:
        with zipfile.ZipFile(path) as archive:
            manifest_names = [name for name in archive.namelist() if name.endswith("/" + CAPTURE_MANIFEST_NAME)]
            if len(manifest_names) != 1:
                raise HandoffError(
                    f"expected exactly one {CAPTURE_MANIFEST_NAME} in {path}, found {len(manifest_names)}"
                )
            manifest = json.loads(archive.read(manifest_names[0]).decode("utf-8"))
    except (OSError, zipfile.BadZipFile, json.JSONDecodeError) as error:
        raise HandoffError(f"could not read capture archive {path}: {error}") from error
    if not isinstance(manifest, dict):
        raise HandoffError(f"capture archive manifest is not a JSON object: {path}")
    return manifest


def validate_capture_archive_artifacts(path: Path, manifest: dict) -> None:
    artifacts = manifest.get("artifacts")
    if not isinstance(artifacts, list) or not artifacts:
        raise HandoffError(f"capture archive manifest is missing PNG artifact fingerprints: {path}")
    try:
        with zipfile.ZipFile(path) as archive:
            manifest_names = [name for name in archive.namelist() if name.endswith("/" + CAPTURE_MANIFEST_NAME)]
            if len(manifest_names) != 1:
                raise HandoffError(
                    f"expected exactly one {CAPTURE_MANIFEST_NAME} in {path}, found {len(manifest_names)}"
                )
            prefix = manifest_names[0][: -len(CAPTURE_MANIFEST_NAME)]
            archive_names = set(archive.namelist())
            recorded_names: set[str] = set()
            for index, artifact in enumerate(artifacts):
                if not isinstance(artifact, dict):
                    raise HandoffError(f"capture archive manifest has invalid artifact fingerprint {index}: {path}")
                name = artifact.get("name")
                if not isinstance(name, str) or not name or Path(name).name != name or Path(name).is_absolute():
                    raise HandoffError(f"capture archive manifest has unsafe artifact name in fingerprint {index}: {path}")
                if name in recorded_names:
                    raise HandoffError(f"capture archive manifest has duplicate artifact fingerprint for {name}: {path}")
                recorded_names.add(name)
                archive_name = prefix + name
                if archive_name not in archive_names:
                    raise HandoffError(f"capture archive is missing fingerprinted PNG {name}: {path}")
                data = archive.read(archive_name)
                if artifact.get("size_bytes") != len(data):
                    raise HandoffError(
                        f"capture archive PNG {name} size_bytes mismatch: "
                        f"recorded {artifact.get('size_bytes')!r}, current {len(data)}: {path}"
                    )
                if artifact.get("sha256") != sha256_bytes(data):
                    raise HandoffError(f"capture archive PNG {name} SHA-256 mismatch: {path}")
            actual_names = {
                relative
                for entry_name in archive_names
                if entry_name.startswith(prefix)
                for relative in (entry_name[len(prefix) :],)
                if relative and "/" not in relative and relative.lower().endswith(".png")
            }
            if actual_names != recorded_names:
                extra = sorted(actual_names - recorded_names)
                missing = sorted(recorded_names - actual_names)
                details = []
                if extra:
                    details.append("extra PNG(s): " + ", ".join(extra))
                if missing:
                    details.append("missing PNG(s): " + ", ".join(missing))
                raise HandoffError(f"capture archive PNG set mismatch: {'; '.join(details)}: {path}")
    except zipfile.BadZipFile as error:
        raise HandoffError(f"could not read capture archive {path}: {error}") from error


def executable_identity_from_capture_manifest(manifest: dict, *, label: str) -> dict[str, object]:
    app = manifest.get("app")
    executable = manifest.get("executable")
    executable_size = manifest.get("executable_size_bytes")
    executable_hash = manifest.get("executable_sha256")
    if not isinstance(app, str) or not app:
        raise HandoffError(f"{label} manifest does not record app path")
    if not isinstance(executable, str) or not executable:
        raise HandoffError(f"{label} manifest does not record executable path")
    if not isinstance(executable_size, int) or executable_size <= 0:
        raise HandoffError(f"{label} manifest does not record a valid executable size")
    if not isinstance(executable_hash, str) or len(executable_hash) != 64:
        raise HandoffError(f"{label} manifest does not record executable SHA-256")
    return {
        "app": app,
        "executable": executable,
        "size_bytes": executable_size,
        "sha256": executable_hash,
    }


def validate_capture_matches_package(path: Path, package_identity: dict[str, object], *, label: str) -> None:
    identity = executable_identity_from_capture_manifest(capture_manifest_from_archive(path), label=label)
    if not recorded_path_matches(str(identity["app"]), str(package_identity["app"])):
        raise HandoffError(f"{label} app {identity['app']!r} does not match Debug package app {package_identity['app']!r}")
    if not recorded_path_matches(str(identity["executable"]), str(package_identity["executable"])):
        raise HandoffError(
            f"{label} executable {identity['executable']!r} does not match Debug package executable "
            f"{package_identity['executable']!r}"
        )
    if identity["size_bytes"] != package_identity["size_bytes"]:
        raise HandoffError(
            f"{label} executable size {identity['size_bytes']!r} does not match Debug package "
            f"{package_identity['size_bytes']!r}"
        )
    if identity["sha256"] != package_identity["sha256"]:
        raise HandoffError(f"{label} executable SHA-256 does not match Debug package executable")


def source_head_from_capture_archive(path: Path, *, expected_mode: str) -> str:
    manifest = capture_manifest_from_archive(path)
    if manifest.get("schema") != CAPTURE_MANIFEST_SCHEMA:
        raise HandoffError(f"capture archive manifest must use schema {CAPTURE_MANIFEST_SCHEMA}: {path}")
    if manifest.get("reported_backend") != "metal":
        raise HandoffError(f"capture archive must record reported_backend='metal': {path}")

    source_revision = manifest.get("source_revision") if isinstance(manifest, dict) else None
    head = source_revision.get("head") if isinstance(source_revision, dict) else None
    if not isinstance(head, str) or not head:
        raise HandoffError(f"capture archive does not record source_revision.head: {path}")
    if source_revision.get("dirty") is not False:
        raise HandoffError(f"capture archive does not record a clean source revision: {path}")
    executable_identity_from_capture_manifest(manifest, label=str(path))

    artifacts = manifest.get("artifacts")
    if not isinstance(artifacts, list) or not artifacts:
        raise HandoffError(f"capture archive manifest is missing PNG artifact fingerprints: {path}")
    for index, artifact in enumerate(artifacts):
        if not isinstance(artifact, dict) or not isinstance(artifact.get("name"), str) or not isinstance(artifact.get("sha256"), str):
            raise HandoffError(f"capture archive manifest has incomplete artifact fingerprint {index}: {path}")
    validate_capture_archive_artifacts(path, manifest)
    commands = manifest.get("commands")
    if not isinstance(commands, list) or not commands:
        raise HandoffError(f"capture archive manifest is missing capture command provenance: {path}")

    probe_only = bool(manifest.get("preview_probes_only")) or bool(manifest.get("metal_preview_probes_only")) or bool(
        manifest.get("metal_renderer_probes_only")
    )
    map_render_only = bool(manifest.get("map_render_only"))
    map_after_save_reload = bool(manifest.get("map_after_save_reload"))
    map_after_mpq_export_reload = bool(manifest.get("map_after_mpq_export_reload"))
    if expected_mode == "default":
        if probe_only or map_render_only or not map_after_save_reload or map_after_mpq_export_reload:
            raise HandoffError(f"capture archive is not a default saved/reloaded map+preview Metal PR capture: {path}")
        if manifest.get("smoke_picking") is not True:
            raise HandoffError(f"default capture archive does not record picking smoke coverage: {path}")
    elif expected_mode == "mpq":
        if probe_only or not map_render_only or map_after_save_reload or not map_after_mpq_export_reload:
            raise HandoffError(f"capture archive is not an MPQ export/reload map-render Metal PR capture: {path}")
        if manifest.get("smoke_picking") is not True:
            raise HandoffError(f"MPQ capture archive does not record picking smoke coverage: {path}")
    else:
        raise AssertionError(f"unknown capture mode {expected_mode!r}")
    return head


def validate_matching_source_heads(named_heads: Iterable[tuple[str, str]]) -> str:
    heads = list(named_heads)
    if not heads:
        raise HandoffError("no source heads were provided")
    expected = heads[0][1]
    mismatches = [f"{name}={head}" for name, head in heads if head != expected]
    if mismatches:
        all_heads = ", ".join(f"{name}={head}" for name, head in heads)
        raise HandoffError(f"renderer handoff inputs are not from one source revision: {all_heads}")
    return expected


def validate_smoke_matches_package(artifact_dir: Path, package_identity: dict[str, object]) -> None:
    provenance = read_json(artifact_dir / SMOKE_PROVENANCE_NAME)
    app = provenance.get("app")
    executable = provenance.get("executable")
    executable_size = provenance.get("executable_size_bytes")
    executable_hash = provenance.get("executable_sha256")
    if not isinstance(app, str) or not app:
        raise HandoffError(f"{SMOKE_PROVENANCE_NAME} does not record app path: {artifact_dir}")
    if not isinstance(executable, str) or not executable:
        raise HandoffError(f"{SMOKE_PROVENANCE_NAME} does not record executable path: {artifact_dir}")
    if not recorded_path_matches(app, str(package_identity["app"])):
        raise HandoffError(f"{SMOKE_PROVENANCE_NAME} app {app!r} does not match Debug package app {package_identity['app']!r}")
    if not recorded_path_matches(executable, str(package_identity["executable"])):
        raise HandoffError(
            f"{SMOKE_PROVENANCE_NAME} executable {executable!r} does not match Debug package executable "
            f"{package_identity['executable']!r}"
        )
    if executable_size != package_identity["size_bytes"]:
        raise HandoffError(
            f"{SMOKE_PROVENANCE_NAME} executable size {executable_size!r} does not match Debug package "
            f"{package_identity['size_bytes']!r}"
        )
    if executable_hash != package_identity["sha256"]:
        raise HandoffError(f"{SMOKE_PROVENANCE_NAME} executable SHA-256 does not match Debug package executable")


def validate_smoke_provenance(artifact_dir: Path, capture_frame: Path) -> str:
    if not capture_frame.is_file():
        raise HandoffError(f"installed smoke capture frame is missing: {capture_frame}")
    provenance = read_json(artifact_dir / SMOKE_PROVENANCE_NAME)
    if provenance.get("schema") != 1 or provenance.get("label") != "full-metal-runtime-smoke":
        raise HandoffError(f"{SMOKE_PROVENANCE_NAME} has unsupported schema or label: {artifact_dir}")
    if provenance.get("expected_backend") != "metal" or provenance.get("reported_backend") != "metal":
        raise HandoffError(f"{SMOKE_PROVENANCE_NAME} does not prove the Metal backend: {artifact_dir}")

    source_revision = provenance.get("source_revision")
    head = source_revision.get("head") if isinstance(source_revision, dict) else None
    if not isinstance(head, str) or not head:
        raise HandoffError(f"{SMOKE_PROVENANCE_NAME} does not record source_revision.head: {artifact_dir}")
    if source_revision.get("dirty") is not False:
        raise HandoffError(f"{SMOKE_PROVENANCE_NAME} does not record a clean source revision: {artifact_dir}")

    frame = provenance.get("capture_frame")
    if not isinstance(frame, dict):
        raise HandoffError(f"{SMOKE_PROVENANCE_NAME} does not record capture_frame metadata")
    recorded_name = frame.get("name")
    recorded_size = frame.get("size_bytes")
    recorded_sha = frame.get("sha256")
    actual_sha = sha256_file(capture_frame)
    actual_size = capture_frame.stat().st_size
    if recorded_name != capture_frame.name or recorded_size != actual_size or recorded_sha != actual_sha:
        raise HandoffError(
            "installed smoke capture frame does not match provenance: "
            f"recorded name={recorded_name!r} size={recorded_size!r} sha={recorded_sha!r}; "
            f"actual name={capture_frame.name!r} size={actual_size!r} sha={actual_sha!r}"
        )

    artifacts = provenance.get("artifacts")
    if not isinstance(artifacts, list) or not artifacts:
        raise HandoffError(f"{SMOKE_PROVENANCE_NAME} does not record PNG artifact fingerprints: {artifact_dir}")
    recorded_names: set[str] = set()
    for index, artifact in enumerate(artifacts):
        if not isinstance(artifact, dict):
            raise HandoffError(f"{SMOKE_PROVENANCE_NAME} artifact fingerprint {index} is not an object: {artifact_dir}")
        name = artifact.get("name")
        if not isinstance(name, str) or not name or Path(name).name != name:
            raise HandoffError(f"{SMOKE_PROVENANCE_NAME} artifact fingerprint {index} has invalid name: {artifact_dir}")
        if name in recorded_names:
            raise HandoffError(f"{SMOKE_PROVENANCE_NAME} records duplicate artifact fingerprint for {name}: {artifact_dir}")
        artifact_path = artifact_dir / name
        if not artifact_path.is_file():
            raise HandoffError(f"{SMOKE_PROVENANCE_NAME} artifact is missing: {artifact_path}")
        recorded_names.add(name)
        if artifact.get("size_bytes") != artifact_path.stat().st_size or artifact.get("sha256") != sha256_file(artifact_path):
            raise HandoffError(f"{SMOKE_PROVENANCE_NAME} artifact fingerprint mismatch: {artifact_path}")
    actual_names = {path.name for path in artifact_dir.glob("*.png")}
    if actual_names != recorded_names:
        extra = sorted(actual_names - recorded_names)
        missing = sorted(recorded_names - actual_names)
        details = []
        if extra:
            details.append("extra PNG(s): " + ", ".join(extra))
        if missing:
            details.append("missing PNG(s): " + ", ".join(missing))
        raise HandoffError(f"{SMOKE_PROVENANCE_NAME} artifact set mismatch: {'; '.join(details)}: {artifact_dir}")
    return head


def readme_markdown(source_head: str, capture_frame_name: str) -> str:
    short_head = source_head[:12]
    return f"""# HiveWE Renderer PR Handoff

This bundle contains Metal-side renderer evidence for source revision `{source_head}`.

## Contents

- `metal-evidence/hivewe-metal-artifacts.zip`: default Metal PR capture archive.
- `metal-evidence/hivewe-metal-mpq-map-artifacts.zip`: MPQ export/reload Metal map-render capture archive.
- `metal-evidence/renderer-pr-capture-summary.md`: source-pinned capture and comparison commands.
- `full-smoke/{capture_frame_name}`: full installed Metal smoke frame, kept under the provenance-recorded basename.
- `full-smoke/hivewe-installed-metal-smoke-artifacts/`: full installed Metal smoke artifacts and provenance.
- `package-reports/`: Debug and Release schema-{PACKAGE_REPORT_SCHEMA} package validation reports with required codesign checks.
- `audits/`: local Metal-only audits, when supplied.

## Remaining External Evidence

This handoff is intentionally Metal-side only. Final maintainer-facing evidence still needs OpenGL reference captures
from a Windows/Linux OpenGL 4.5 runtime, default and MPQ OpenGL-vs-Metal parity reports, and a completed manual
visual-review note.

Capture the OpenGL side from the same source revision:

```shell
python3 tools/capture_renderer_pr_evidence.py --backend opengl --app <OpenGL HiveWE executable> --map "data/test map/" --warcraft-dir "<Warcraft III>" --output-dir hivewe-opengl-pr-evidence --require-source-head {source_head}
```

Generate the final parity reports:

```shell
python3 tools/run_renderer_parity.py --skip-capture --pr-gate --require-source-head {source_head} --opengl-artifact-archive hivewe-opengl-pr-evidence/hivewe-opengl-artifacts.zip --metal-artifact-archive metal-evidence/hivewe-metal-artifacts.zip --report-dir hivewe-final-renderer-parity
python3 tools/run_renderer_parity.py --skip-capture --pr-gate --map-render-only --map-after-mpq-export-reload --require-source-head {source_head} --opengl-artifact-archive hivewe-opengl-pr-evidence/hivewe-opengl-mpq-map-artifacts.zip --metal-artifact-archive metal-evidence/hivewe-metal-mpq-map-artifacts.zip --report-dir hivewe-final-renderer-mpq-parity
```

Create the manual-review starter:

```shell
python3 tools/audit_renderer_pr_evidence.py --write-manual-review-template hivewe-renderer-manual-review.md --require-source-head {source_head} --opengl-artifact-archive hivewe-opengl-pr-evidence/hivewe-opengl-artifacts.zip --metal-artifact-archive metal-evidence/hivewe-metal-artifacts.zip --opengl-mpq-artifact-archive hivewe-opengl-pr-evidence/hivewe-opengl-mpq-map-artifacts.zip --metal-mpq-artifact-archive metal-evidence/hivewe-metal-mpq-map-artifacts.zip --parity-report-dir hivewe-final-renderer-parity --mpq-parity-report-dir hivewe-final-renderer-mpq-parity
```

Run the strict final audit after replacing every manual-review TODO:

```shell
python3 tools/audit_renderer_pr_evidence.py --opengl-artifact-archive hivewe-opengl-pr-evidence/hivewe-opengl-artifacts.zip --metal-artifact-archive metal-evidence/hivewe-metal-artifacts.zip --opengl-mpq-artifact-archive hivewe-opengl-pr-evidence/hivewe-opengl-mpq-map-artifacts.zip --metal-mpq-artifact-archive metal-evidence/hivewe-metal-mpq-map-artifacts.zip --parity-report-dir hivewe-final-renderer-parity --mpq-parity-report-dir hivewe-final-renderer-mpq-parity --installed-smoke-artifact-dir full-smoke/hivewe-installed-metal-smoke-artifacts --installed-smoke-capture-frame full-smoke/{capture_frame_name} --debug-package-report-json package-reports/hivewe-debug-package-report.json --release-package-report-json package-reports/hivewe-release-package-report.json --manual-review-note hivewe-renderer-manual-review.md --require-source-head {source_head} --require-clean-source --require-mpq-parity-report --markdown-output hivewe-renderer-pr-evidence-audit.md
```

Bundle source shorthand: `{short_head}`.
"""


def write_manifest(root: Path, source_head: str) -> Path:
    files = [file_entry(root, path) for path in sorted(root.rglob("*")) if path.is_file() and path.name != "handoff-manifest.json"]
    manifest = {
        "schema": 1,
        "created_at_utc": datetime.now(timezone.utc).isoformat(),
        "source_head": source_head,
        "source_head_short": source_head[:12],
        "files": files,
    }
    path = root / "handoff-manifest.json"
    path.write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
    return path


def validate_manifest(root: Path) -> None:
    manifest = read_json(root / "handoff-manifest.json")
    entries = manifest.get("files")
    if not isinstance(entries, list):
        raise HandoffError("handoff manifest does not contain a files list")
    for entry in entries:
        if not isinstance(entry, dict):
            raise HandoffError("handoff manifest contains a non-object file entry")
        relative = entry.get("path")
        if not isinstance(relative, str):
            raise HandoffError("handoff manifest file entry is missing a path")
        path = root / relative
        if not path.is_file():
            raise HandoffError(f"manifest file is missing: {relative}")
        size = path.stat().st_size
        digest = sha256_file(path)
        if entry.get("size_bytes") != size or entry.get("sha256") != digest:
            raise HandoffError(f"manifest file fingerprint mismatch: {relative}")


def write_zip(root: Path, output_zip: Path) -> None:
    output_zip.parent.mkdir(parents=True, exist_ok=True)
    if output_zip.exists():
        output_zip.unlink()
    with zipfile.ZipFile(output_zip, "w", compression=zipfile.ZIP_DEFLATED, compresslevel=6) as archive:
        for path in sorted(root.rglob("*")):
            if not path.is_file():
                continue
            relative = Path(root.name) / path.relative_to(root)
            if any(part == "__MACOSX" or part.startswith("._") for part in relative.parts):
                raise HandoffError(f"refusing to zip AppleDouble path: {relative.as_posix()}")
            archive.write(path, relative.as_posix())


def validate_zip(output_zip: Path) -> None:
    with zipfile.ZipFile(output_zip) as archive:
        bad_names = [
            name
            for name in archive.namelist()
            if "__MACOSX" in Path(name).parts or any(part.startswith("._") for part in Path(name).parts)
        ]
    if bad_names:
        raise HandoffError("handoff zip contains AppleDouble entries: " + ", ".join(bad_names))


def build_handoff(args: argparse.Namespace) -> tuple[Path, Path | None, str]:
    output_dir = args.output_dir
    if output_dir.exists():
        if not args.force:
            raise HandoffError(f"output directory already exists; pass --force to replace it: {output_dir}")
        shutil.rmtree(output_dir)
    output_dir.mkdir(parents=True)

    source_head = validate_matching_source_heads(
        [
            ("metal default capture", source_head_from_capture_archive(args.metal_artifact_archive, expected_mode="default")),
            ("metal MPQ capture", source_head_from_capture_archive(args.metal_mpq_artifact_archive, expected_mode="mpq")),
            ("Debug package report", source_head_from_package_report(args.debug_package_report_json, "Debug")),
            ("Release package report", source_head_from_package_report(args.release_package_report_json, "Release")),
            ("installed full Metal smoke", validate_smoke_provenance(args.installed_smoke_artifact_dir, args.installed_smoke_capture_frame)),
        ]
    )
    debug_package_identity = executable_identity_from_package_report(args.debug_package_report_json, "Debug")
    validate_capture_matches_package(args.metal_artifact_archive, debug_package_identity, label="default Metal capture")
    validate_capture_matches_package(args.metal_mpq_artifact_archive, debug_package_identity, label="MPQ Metal capture")
    validate_smoke_matches_package(args.installed_smoke_artifact_dir, debug_package_identity)

    copy_file(args.metal_artifact_archive, output_dir / "metal-evidence" / "hivewe-metal-artifacts.zip")
    copy_file(args.metal_mpq_artifact_archive, output_dir / "metal-evidence" / "hivewe-metal-mpq-map-artifacts.zip")
    if args.renderer_pr_capture_summary is not None:
        copy_file(args.renderer_pr_capture_summary, output_dir / "metal-evidence" / "renderer-pr-capture-summary.md")

    copy_tree(args.installed_smoke_artifact_dir, output_dir / "full-smoke" / "hivewe-installed-metal-smoke-artifacts")
    copy_file(args.installed_smoke_capture_frame, output_dir / "full-smoke" / args.installed_smoke_capture_frame.name)

    copy_file(args.debug_package_report_json, output_dir / "package-reports" / "hivewe-debug-package-report.json")
    copy_file(args.release_package_report_json, output_dir / "package-reports" / "hivewe-release-package-report.json")
    if args.debug_package_report_markdown is not None:
        copy_file(args.debug_package_report_markdown, output_dir / "package-reports" / "hivewe-debug-package-report.md")
    if args.release_package_report_markdown is not None:
        copy_file(args.release_package_report_markdown, output_dir / "package-reports" / "hivewe-release-package-report.md")

    for audit in args.audit:
        copy_file(audit, output_dir / "audits" / audit.name)

    (output_dir / "README.md").write_text(
        readme_markdown(source_head, args.installed_smoke_capture_frame.name),
        encoding="utf-8",
    )
    write_manifest(output_dir, source_head)
    validate_manifest(output_dir)

    if args.output_zip is not None:
        write_zip(output_dir, args.output_zip)
        validate_zip(args.output_zip)

    return output_dir, args.output_zip, source_head


def write_fake_capture_archive(
    path: Path,
    source_head: str,
    *,
    app: Path,
    executable: Path,
    backend: str = "metal",
    mode: str = "default",
) -> None:
    if mode == "default":
        map_render_only = False
        map_after_save_reload = True
        map_after_mpq_export_reload = False
    elif mode == "mpq":
        map_render_only = True
        map_after_save_reload = False
        map_after_mpq_export_reload = True
    else:
        raise AssertionError(f"unknown fake capture mode {mode!r}")
    artifact_name = "map-render-baseline.png"
    artifact_bytes = f"{backend}-{mode}-artifact\n".encode("utf-8")
    manifest = {
        "schema": CAPTURE_MANIFEST_SCHEMA,
        "reported_backend": backend,
        "app": str(app),
        "executable": str(executable),
        "executable_size_bytes": executable.stat().st_size,
        "executable_sha256": sha256_file(executable),
        "source_revision": {"head": source_head, "dirty": False, "status_short": ""},
        "preview_probes_only": False,
        "map_render_only": map_render_only,
        "metal_preview_probes_only": False,
        "metal_renderer_probes_only": False,
        "map_after_save_reload": map_after_save_reload,
        "map_after_mpq_export_reload": map_after_mpq_export_reload,
        "smoke_picking": True,
        "commands": [["HiveWE", "--smoke-render-modes", "--smoke-picking"]],
        "artifacts": [
            {
                "name": artifact_name,
                "sha256": sha256_bytes(artifact_bytes),
                "size_bytes": len(artifact_bytes),
                "width": 4,
                "height": 3,
            }
        ],
    }
    with zipfile.ZipFile(path, "w", compression=zipfile.ZIP_DEFLATED) as archive:
        archive_root = f"hivewe-{backend}-capture"
        archive.writestr(f"{archive_root}/{CAPTURE_MANIFEST_NAME}", json.dumps(manifest))
        archive.writestr(f"{archive_root}/{artifact_name}", artifact_bytes)


def write_fake_package_report(path: Path, source_head: str, build_type: str, *, app: Path, executable: Path) -> None:
    path.write_text(
        json.dumps(
            {
                "schema": PACKAGE_REPORT_SCHEMA,
                "label": PACKAGE_REPORT_LABEL,
                "status": "success",
                "expected": {
                    "source_head": source_head,
                    "require_current_source": True,
                    "require_clean_source": True,
                    "build_type": build_type,
                    "backend": "metal",
                },
                "app": str(app),
                "app_executable": {
                    "path": str(executable),
                    "size_bytes": executable.stat().st_size,
                    "sha256": sha256_file(executable),
                },
                "source_revision": {"head": source_head, "dirty": False, "status_short": ""},
                "cmake": {"build_type": build_type, "enable_metal": "1"},
                "checks": {name: True for name in REQUIRED_PACKAGE_REPORT_CHECKS},
                "artifacts": {
                    "built_dmg": f"/tmp/HiveWE-{build_type}.dmg",
                    "verified_dmg": f"/tmp/HiveWE-{build_type}.dmg",
                },
            }
        ),
        encoding="utf-8",
    )


def run_self_test() -> None:
    source_head = "0123456789abcdef0123456789abcdef01234567"
    with tempfile.TemporaryDirectory(prefix="hivewe-renderer-handoff-self-test-") as temp:
        root = Path(temp)
        fake_app = root / "install" / "HiveWE.app"
        fake_executable = fake_app / "Contents" / "MacOS" / "HiveWE"
        fake_executable.parent.mkdir(parents=True)
        fake_executable.write_bytes(b"fake executable\n")

        capture = root / "capture.png"
        capture.write_bytes(b"fake png bytes")
        capture_sha = sha256_file(capture)

        smoke_dir = root / "smoke-artifacts"
        smoke_dir.mkdir()
        smoke_artifact = smoke_dir / "artifact.png"
        smoke_artifact.write_bytes(b"artifact")
        smoke_artifact_sha = sha256_file(smoke_artifact)
        (smoke_dir / SMOKE_PROVENANCE_NAME).write_text(
            json.dumps(
                {
                    "schema": 1,
                    "label": "full-metal-runtime-smoke",
                    "expected_backend": "metal",
                    "reported_backend": "metal",
                    "app": str(fake_app),
                    "executable": str(fake_executable),
                    "executable_size_bytes": fake_executable.stat().st_size,
                    "executable_sha256": sha256_file(fake_executable),
                    "source_revision": {"head": source_head, "dirty": False, "status_short": ""},
                    "capture_frame": {
                        "name": capture.name,
                        "size_bytes": capture.stat().st_size,
                        "sha256": capture_sha,
                    },
                    "artifacts": [
                        {
                            "name": smoke_artifact.name,
                            "size_bytes": smoke_artifact.stat().st_size,
                            "sha256": smoke_artifact_sha,
                        }
                    ],
                }
            ),
            encoding="utf-8",
        )
        assert validate_smoke_provenance(smoke_dir, capture) == source_head

        default_archive = root / "metal-default.zip"
        mpq_archive = root / "metal-mpq.zip"
        write_fake_capture_archive(default_archive, source_head, app=fake_app, executable=fake_executable)
        write_fake_capture_archive(mpq_archive, source_head, app=fake_app, executable=fake_executable, mode="mpq")
        assert source_head_from_capture_archive(default_archive, expected_mode="default") == source_head
        assert source_head_from_capture_archive(mpq_archive, expected_mode="mpq") == source_head

        wrong_backend_archive = root / "wrong-backend.zip"
        write_fake_capture_archive(wrong_backend_archive, source_head, app=fake_app, executable=fake_executable, backend="opengl")
        failed_wrong_backend = False
        try:
            source_head_from_capture_archive(wrong_backend_archive, expected_mode="default")
        except HandoffError:
            failed_wrong_backend = True
        assert failed_wrong_backend, "handoff packaging should reject non-Metal capture archives"

        wrong_mode_archive = root / "wrong-mode.zip"
        write_fake_capture_archive(wrong_mode_archive, source_head, app=fake_app, executable=fake_executable, mode="default")
        failed_wrong_mode = False
        try:
            source_head_from_capture_archive(wrong_mode_archive, expected_mode="mpq")
        except HandoffError:
            failed_wrong_mode = True
        assert failed_wrong_mode, "handoff packaging should reject default captures in the MPQ slot"

        no_fingerprint_archive = root / "no-fingerprints.zip"
        manifest = capture_manifest_from_archive(default_archive)
        manifest["artifacts"] = []
        with zipfile.ZipFile(no_fingerprint_archive, "w", compression=zipfile.ZIP_DEFLATED) as archive:
            archive.writestr(f"hivewe-metal-capture/{CAPTURE_MANIFEST_NAME}", json.dumps(manifest))
        failed_no_fingerprints = False
        try:
            source_head_from_capture_archive(no_fingerprint_archive, expected_mode="default")
        except HandoffError:
            failed_no_fingerprints = True
        assert failed_no_fingerprints, "handoff packaging should reject captures without PNG fingerprints"

        stale_capture_payload_archive = root / "stale-capture-payload.zip"
        manifest = capture_manifest_from_archive(default_archive)
        with zipfile.ZipFile(stale_capture_payload_archive, "w", compression=zipfile.ZIP_DEFLATED) as archive:
            archive.writestr(f"hivewe-metal-capture/{CAPTURE_MANIFEST_NAME}", json.dumps(manifest))
            archive.writestr("hivewe-metal-capture/map-render-baseline.png", b"changed capture payload\n")
        failed_stale_capture_payload = False
        try:
            source_head_from_capture_archive(stale_capture_payload_archive, expected_mode="default")
        except HandoffError:
            failed_stale_capture_payload = True
        assert failed_stale_capture_payload, "handoff packaging should reject stale capture PNG payloads"

        debug_report = root / "debug.json"
        release_report = root / "release.json"
        write_fake_package_report(debug_report, source_head, "Debug", app=fake_app, executable=fake_executable)
        write_fake_package_report(release_report, source_head, "Release", app=fake_app, executable=fake_executable)
        assert source_head_from_package_report(debug_report, "Debug") == source_head

        stale_schema_report = root / "stale-schema-debug.json"
        write_fake_package_report(stale_schema_report, source_head, "Debug", app=fake_app, executable=fake_executable)
        stale_schema_data = read_json(stale_schema_report)
        stale_schema_data["schema"] = PACKAGE_REPORT_SCHEMA - 1
        stale_schema_report.write_text(json.dumps(stale_schema_data), encoding="utf-8")
        failed_stale_schema = False
        try:
            source_head_from_package_report(stale_schema_report, "Debug")
        except HandoffError:
            failed_stale_schema = True
        assert failed_stale_schema, "handoff packaging should reject stale package report schemas"

        missing_codesign_report = root / "missing-codesign-debug.json"
        write_fake_package_report(missing_codesign_report, source_head, "Debug", app=fake_app, executable=fake_executable)
        missing_codesign_data = read_json(missing_codesign_report)
        missing_codesign_data["checks"].pop("codesign")
        missing_codesign_report.write_text(json.dumps(missing_codesign_data), encoding="utf-8")
        failed_missing_codesign = False
        try:
            source_head_from_package_report(missing_codesign_report, "Debug")
        except HandoffError:
            failed_missing_codesign = True
        assert failed_missing_codesign, "handoff packaging should reject package reports without codesign evidence"

        wrong_build_type_report = root / "wrong-build-type-debug.json"
        write_fake_package_report(wrong_build_type_report, source_head, "Release", app=fake_app, executable=fake_executable)
        failed_wrong_build_type = False
        try:
            source_head_from_package_report(wrong_build_type_report, "Debug")
        except HandoffError:
            failed_wrong_build_type = True
        assert failed_wrong_build_type, "handoff packaging should reject mismatched package report build types"

        summary = root / "summary.md"
        summary.write_text("summary\n", encoding="utf-8")
        audit = root / "audit.md"
        audit.write_text("audit\n", encoding="utf-8")

        output_dir = root / "handoff"
        output_zip = root / "handoff.zip"
        args = argparse.Namespace(
            output_dir=output_dir,
            output_zip=output_zip,
            force=False,
            metal_artifact_archive=default_archive,
            metal_mpq_artifact_archive=mpq_archive,
            renderer_pr_capture_summary=summary,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture,
            debug_package_report_json=debug_report,
            debug_package_report_markdown=None,
            release_package_report_json=release_report,
            release_package_report_markdown=None,
            audit=[audit],
        )
        built_dir, built_zip, built_head = build_handoff(args)
        assert built_dir == output_dir
        assert built_zip == output_zip
        assert built_head == source_head
        validate_manifest(output_dir)
        validate_zip(output_zip)
        readme = (output_dir / "README.md").read_text(encoding="utf-8")
        assert f"full-smoke/{capture.name}" in readme
        assert "--metal-artifact-archive metal-evidence/hivewe-metal-artifacts.zip" in readme
        assert "--opengl-mpq-artifact-archive hivewe-opengl-pr-evidence/hivewe-opengl-mpq-map-artifacts.zip" in readme
        with zipfile.ZipFile(output_zip) as archive:
            names = archive.namelist()
            assert not any("__MACOSX" in Path(name).parts or any(part.startswith("._") for part in Path(name).parts) for name in names)
            assert f"{output_dir.name}/full-smoke/{capture.name}" in names

        other_app = root / "other-install" / "HiveWE.app"
        other_executable = other_app / "Contents" / "MacOS" / "HiveWE"
        other_executable.parent.mkdir(parents=True)
        other_executable.write_bytes(b"other fake executable\n")
        wrong_executable_archive = root / "wrong-executable-metal-default.zip"
        write_fake_capture_archive(wrong_executable_archive, source_head, app=other_app, executable=other_executable)
        wrong_executable_args = argparse.Namespace(
            **{**vars(args), "output_dir": root / "wrong-executable-handoff", "output_zip": None, "metal_artifact_archive": wrong_executable_archive}
        )
        failed_wrong_executable_capture = False
        try:
            build_handoff(wrong_executable_args)
        except HandoffError:
            failed_wrong_executable_capture = True
        assert failed_wrong_executable_capture, "handoff packaging should reject Metal captures from a different executable"

        wrong_smoke_executable_dir = root / "wrong-smoke-executable-artifacts"
        shutil.copytree(smoke_dir, wrong_smoke_executable_dir)
        wrong_smoke_executable_provenance = read_json(wrong_smoke_executable_dir / SMOKE_PROVENANCE_NAME)
        wrong_smoke_executable_provenance["executable"] = str(other_executable)
        wrong_smoke_executable_provenance["executable_size_bytes"] = other_executable.stat().st_size
        wrong_smoke_executable_provenance["executable_sha256"] = sha256_file(other_executable)
        (wrong_smoke_executable_dir / SMOKE_PROVENANCE_NAME).write_text(json.dumps(wrong_smoke_executable_provenance), encoding="utf-8")
        wrong_smoke_executable_args = argparse.Namespace(
            **{
                **vars(args),
                "output_dir": root / "wrong-smoke-executable-handoff",
                "output_zip": None,
                "installed_smoke_artifact_dir": wrong_smoke_executable_dir,
            }
        )
        failed_wrong_smoke_executable = False
        try:
            build_handoff(wrong_smoke_executable_args)
        except HandoffError:
            failed_wrong_smoke_executable = True
        assert failed_wrong_smoke_executable, "handoff packaging should reject full smoke from a different executable"

        stale_smoke_dir = root / "stale-smoke-artifacts"
        shutil.copytree(smoke_dir, stale_smoke_dir)
        stale_smoke_provenance = read_json(stale_smoke_dir / SMOKE_PROVENANCE_NAME)
        stale_smoke_provenance["source_revision"] = {"head": "f" * 40, "dirty": False}
        (stale_smoke_dir / SMOKE_PROVENANCE_NAME).write_text(json.dumps(stale_smoke_provenance), encoding="utf-8")
        stale_args = argparse.Namespace(**{**vars(args), "output_dir": root / "stale-handoff", "output_zip": None, "installed_smoke_artifact_dir": stale_smoke_dir})
        failed_stale_smoke = False
        try:
            build_handoff(stale_args)
        except HandoffError:
            failed_stale_smoke = True
        assert failed_stale_smoke, "handoff packaging should reject installed smoke evidence from another source revision"

        dirty_smoke_dir = root / "dirty-smoke-artifacts"
        shutil.copytree(smoke_dir, dirty_smoke_dir)
        dirty_smoke_provenance = read_json(dirty_smoke_dir / SMOKE_PROVENANCE_NAME)
        dirty_smoke_provenance["source_revision"] = {"head": source_head, "dirty": True, "status_short": [" M src/rendering/metal_map_renderer.mm"]}
        (dirty_smoke_dir / SMOKE_PROVENANCE_NAME).write_text(json.dumps(dirty_smoke_provenance), encoding="utf-8")
        failed_dirty_smoke = False
        try:
            validate_smoke_provenance(dirty_smoke_dir, capture)
        except HandoffError:
            failed_dirty_smoke = True
        assert failed_dirty_smoke, "handoff packaging should reject dirty installed smoke provenance"

        stale_smoke_artifact_dir = root / "stale-smoke-png-artifacts"
        shutil.copytree(smoke_dir, stale_smoke_artifact_dir)
        (stale_smoke_artifact_dir / smoke_artifact.name).write_bytes(b"changed artifact")
        failed_stale_smoke_artifact = False
        try:
            validate_smoke_provenance(stale_smoke_artifact_dir, capture)
        except HandoffError:
            failed_stale_smoke_artifact = True
        assert failed_stale_smoke_artifact, "handoff packaging should reject stale installed smoke artifact fingerprints"

        extra_smoke_artifact_dir = root / "extra-smoke-png-artifacts"
        shutil.copytree(smoke_dir, extra_smoke_artifact_dir)
        (extra_smoke_artifact_dir / "extra.png").write_bytes(b"extra artifact")
        failed_extra_smoke_artifact = False
        try:
            validate_smoke_provenance(extra_smoke_artifact_dir, capture)
        except HandoffError:
            failed_extra_smoke_artifact = True
        assert failed_extra_smoke_artifact, "handoff packaging should reject unrecorded installed smoke PNG artifacts"

        renamed = root / "renamed.png"
        shutil.copy2(capture, renamed)
        failed = False
        try:
            validate_smoke_provenance(smoke_dir, renamed)
        except HandoffError:
            failed = True
        assert failed, "renamed capture frame should fail provenance validation"

    print("create_renderer_pr_handoff self-test passed.")


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output-dir", type=Path, help="Directory to create for the handoff bundle.")
    parser.add_argument("--output-zip", type=Path, help="Optional deterministic zip archive to write.")
    parser.add_argument("--force", action="store_true", help="Replace --output-dir if it already exists.")
    parser.add_argument("--metal-artifact-archive", type=Path, help="Default Metal renderer PR capture archive.")
    parser.add_argument("--metal-mpq-artifact-archive", type=Path, help="Metal MPQ export/reload map-render capture archive.")
    parser.add_argument("--renderer-pr-capture-summary", type=Path, help="Optional capture summary Markdown to include.")
    parser.add_argument("--installed-smoke-artifact-dir", type=Path, help="Full installed Metal smoke artifact directory.")
    parser.add_argument("--installed-smoke-capture-frame", type=Path, help="Full installed Metal smoke frame.")
    parser.add_argument("--debug-package-report-json", type=Path, help="Debug package report JSON.")
    parser.add_argument("--debug-package-report-markdown", type=Path, help="Optional Debug package report Markdown.")
    parser.add_argument("--release-package-report-json", type=Path, help="Release package report JSON.")
    parser.add_argument("--release-package-report-markdown", type=Path, help="Optional Release package report Markdown.")
    parser.add_argument("--audit", type=Path, action="append", default=[], help="Optional local audit Markdown to copy. Repeatable.")
    parser.add_argument("--self-test", action="store_true", help="Run dependency-free self-tests and exit.")
    args = parser.parse_args()
    if args.self_test:
        return args

    required = [
        "output_dir",
        "metal_artifact_archive",
        "metal_mpq_artifact_archive",
        "installed_smoke_artifact_dir",
        "installed_smoke_capture_frame",
        "debug_package_report_json",
        "release_package_report_json",
    ]
    missing = [name for name in required if getattr(args, name) is None]
    if missing:
        parser.error("missing required argument(s): " + ", ".join("--" + name.replace("_", "-") for name in missing))
    return args


def main() -> None:
    args = parse_args()
    if args.self_test:
        run_self_test()
        return
    try:
        output_dir, output_zip, source_head = build_handoff(args)
    except HandoffError as error:
        print(f"Renderer PR handoff failed: {error}", file=sys.stderr)
        raise SystemExit(1) from error
    print(f"Wrote renderer PR handoff directory: {output_dir}")
    if output_zip is not None:
        print(f"Wrote renderer PR handoff zip: {output_zip}")
    print(f"Source revision: {source_head}")


if __name__ == "__main__":
    main()
