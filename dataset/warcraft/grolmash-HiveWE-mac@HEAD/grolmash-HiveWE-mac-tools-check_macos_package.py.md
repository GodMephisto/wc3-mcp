---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "tools/check_macos_package.py"
ref: HEAD
---

#!/usr/bin/env python3
"""Validate a macOS HiveWE.app install tree and optionally smoke-test it."""

from __future__ import annotations

import argparse
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import re
import shlex
import shutil
import subprocess
import sys
import tempfile

sys.dont_write_bytecode = True

from compare_smoke_artifacts import (
    METAL_MAP_CANVAS_ARTIFACT_SIZES,
    METAL_PREVIEW_PROBE_ARTIFACT_SIZES,
    METAL_RENDERER_PROBE_ARTIFACT_SIZES,
    PngError,
    SYNTHETIC_SHARED_PREVIEW_ARTIFACT_SIZES,
    encode_png_rgba,
    read_png_size,
    validate_png_artifact_manifest,
)

FORBIDDEN_DYLIB_NAME = re.compile(r"(libGL\.|libglapi\.|libX11|libXext|libXau|libxcb|XQuartz|/opt/X11|/usr/X11)")
METAL_SHADER_RELATIVE_PATH = Path("data/shaders/metal_map_renderer.metal")
BUILD_PROVENANCE_RELATIVE_PATH = Path("hivewe-build-provenance.json")
PACKAGE_REPORT_SCHEMA = 3
PACKAGE_REPORT_LABEL = "hivewe-macos-package-validation"
REQUIRED_BUNDLE_FILES = [
    Path("Contents/MacOS/HiveWE"),
    Path("Contents/Info.plist"),
    Path("Contents/Resources") / BUILD_PROVENANCE_RELATIVE_PATH,
    Path("Contents/Resources/data/test map/war3map.w3e"),
    Path("Contents/Resources/data/test map/war3map.w3i"),
    Path("Contents/Resources") / METAL_SHADER_RELATIVE_PATH,
    Path("Contents/Resources/data/warcraft/Cliffs.slk"),
]

class PackageError(RuntimeError):
    pass


def run(command: list[str], *, cwd: Path | None = None, timeout: float | None = None) -> None:
    command_text = shlex.join(command)
    print("Running:", command_text, flush=True)
    try:
        subprocess.run(command, cwd=cwd, check=True, timeout=timeout)
    except subprocess.TimeoutExpired as error:
        raise PackageError(f"command timed out after {error.timeout:.2f}s: {command_text}") from error
    except subprocess.CalledProcessError as error:
        raise PackageError(f"command failed with exit code {error.returncode}: {command_text}") from error


def validate_clean_git_tree(repo_root: Path) -> None:
    command = ["git", "-C", str(repo_root), "status", "--short"]
    try:
        result = subprocess.run(command, check=True, capture_output=True, text=True, timeout=5)
    except (FileNotFoundError, subprocess.CalledProcessError, subprocess.TimeoutExpired) as error:
        raise PackageError("--require-clean-source needs git status for the repository") from error
    status = result.stdout.strip()
    if status:
        raise PackageError("--require-clean-source needs a clean git working tree:\n" + status)


def current_git_head(repo_root: Path) -> str:
    command = ["git", "-C", str(repo_root), "rev-parse", "HEAD"]
    try:
        result = subprocess.run(command, check=True, capture_output=True, text=True, timeout=5)
    except (FileNotFoundError, subprocess.CalledProcessError, subprocess.TimeoutExpired) as error:
        raise PackageError("unable to determine current git HEAD for build provenance validation") from error
    head = result.stdout.strip()
    if not head:
        raise PackageError("current git HEAD is empty")
    return head


def installed_app(prefix: Path) -> Path:
    return prefix / "HiveWE.app"


def app_executable(app: Path) -> Path:
    return app / "Contents" / "MacOS" / "HiveWE"


def sha256_file(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as file:
        for chunk in iter(lambda: file.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def validate_bundle_files(app: Path) -> None:
    missing = [relative for relative in REQUIRED_BUNDLE_FILES if not (app / relative).is_file()]
    if missing:
        raise PackageError("installed bundle is missing required files: " + ", ".join(str(path) for path in missing))
    print(f"Validated required bundle files in {app}.")


def relative_file_manifest(root: Path) -> list[Path]:
    if not root.is_dir():
        raise PackageError(f"required directory is missing: {root}")

    files = [path.relative_to(root) for path in root.rglob("*") if path.is_file()]
    files.sort(key=lambda path: path.as_posix())
    if not files:
        raise PackageError(f"required directory has no files: {root}")
    return files


def validate_bundled_data_directory(repo_root: Path, app: Path, relative_directory: Path, label: str) -> int:
    source_root = repo_root / relative_directory
    bundled_root = app / "Contents" / "Resources" / relative_directory
    source_files = relative_file_manifest(source_root)
    bundled_files = relative_file_manifest(bundled_root)
    source_file_set = set(source_files)
    bundled_file_set = set(bundled_files)

    missing = sorted(source_file_set - bundled_file_set, key=lambda path: path.as_posix())
    extra = sorted(bundled_file_set - source_file_set, key=lambda path: path.as_posix())
    empty = []
    changed = []
    for relative in source_files:
        if relative in missing:
            continue
        bundled_file = bundled_root / relative
        if bundled_file.stat().st_size == 0:
            empty.append(relative)
        if (source_root / relative).read_bytes() != bundled_file.read_bytes():
            changed.append(relative)

    if missing:
        raise PackageError(f"installed {label} is missing source files: " + ", ".join(path.as_posix() for path in missing))
    if extra:
        raise PackageError(f"installed {label} has stale extra files: " + ", ".join(path.as_posix() for path in extra))
    if empty:
        raise PackageError(f"installed {label} has empty source files: " + ", ".join(path.as_posix() for path in empty))
    if changed:
        raise PackageError(f"installed {label} differs from source files: " + ", ".join(path.as_posix() for path in changed))

    return len(source_files)


def validate_bundled_test_map(repo_root: Path, app: Path) -> None:
    file_count = validate_bundled_data_directory(repo_root, app, Path("data/test map"), "test map")
    print(f"Validated installed test map contains {file_count} byte-identical source files needed by save/reload smoke.")


def validate_bundled_warcraft_fixture(repo_root: Path, app: Path) -> None:
    file_count = validate_bundled_data_directory(repo_root, app, Path("data/warcraft"), "Warcraft-free smoke fixture")
    print(f"Validated installed Warcraft-free smoke fixture contains {file_count} byte-identical source files.")


def validate_bundled_file(repo_root: Path, app: Path, relative_file: Path, label: str) -> None:
    source_file = repo_root / relative_file
    bundled_file = app / "Contents" / "Resources" / relative_file
    if not source_file.is_file():
        raise PackageError(f"source {label} is missing: {source_file}")
    if not bundled_file.is_file():
        raise PackageError(f"installed {label} is missing: {bundled_file}")
    source_bytes = source_file.read_bytes()
    bundled_bytes = bundled_file.read_bytes()
    if not source_bytes:
        raise PackageError(f"source {label} is empty: {source_file}")
    if not bundled_bytes:
        raise PackageError(f"installed {label} is empty: {bundled_file}")
    if source_bytes != bundled_bytes:
        raise PackageError(f"installed {label} differs from source file: {bundled_file}")
    print(f"Validated installed {label} is byte-identical to {source_file}.")


def validate_bundled_metal_shader(repo_root: Path, app: Path) -> None:
    validate_bundled_file(repo_root, app, METAL_SHADER_RELATIVE_PATH, "Metal renderer shader")


def read_build_provenance(app: Path) -> dict:
    provenance_path = app / "Contents" / "Resources" / BUILD_PROVENANCE_RELATIVE_PATH
    if not provenance_path.is_file():
        raise PackageError(f"installed app build provenance is missing: {provenance_path}")
    try:
        provenance = json.loads(provenance_path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as error:
        raise PackageError(f"invalid installed app build provenance {provenance_path}: {error}") from error
    if not isinstance(provenance, dict):
        raise PackageError(f"invalid installed app build provenance {provenance_path}: expected object")
    return provenance


def validate_build_provenance(
    repo_root: Path,
    app: Path,
    *,
    expected_source_head: str | None = None,
    expected_build_type: str | None = None,
    require_current_source: bool,
    require_clean_source: bool,
) -> None:
    provenance = read_build_provenance(app)
    if provenance.get("schema") != 1 or provenance.get("label") != "hivewe-app-build-provenance":
        raise PackageError("installed app build provenance has an unsupported schema or label")
    source = provenance.get("source_revision")
    if not isinstance(source, dict):
        raise PackageError("installed app build provenance is missing source revision metadata")
    head = source.get("head")
    if not isinstance(head, str) or not head:
        raise PackageError("installed app build provenance does not record a source revision head")
    dirty = source.get("dirty")
    if not isinstance(dirty, bool):
        raise PackageError("installed app build provenance does not record source dirty state")
    if expected_source_head is not None and require_current_source:
        raise PackageError("--require-source-head and --require-current-source are mutually exclusive")
    if expected_source_head is not None:
        expected_head = expected_source_head
    elif require_current_source:
        expected_head = current_git_head(repo_root)
    else:
        expected_head = None
    if expected_head is not None and head != expected_head:
        raise PackageError(
            f"installed app was built from source {head}, current source is {expected_head}"
        )
    if require_clean_source and dirty:
        status = source.get("status_short")
        suffix = f":\n{status}" if isinstance(status, str) and status else ""
        raise PackageError("installed app build provenance records a dirty source tree" + suffix)
    cmake = provenance.get("cmake")
    if expected_build_type is not None:
        if not isinstance(cmake, dict):
            raise PackageError("installed app build provenance is missing CMake metadata")
        build_type = cmake.get("build_type")
        if build_type != expected_build_type:
            raise PackageError(
                f"installed app build type is {build_type!r}, expected {expected_build_type!r}"
            )
    build_type_detail = ""
    if isinstance(cmake, dict) and isinstance(cmake.get("build_type"), str):
        build_type_detail = f", build_type={cmake['build_type']}"
    print(f"Validated installed app build provenance: source {head[:12]}, dirty={dirty}{build_type_detail}.")


def validate_reported_backend(app: Path, expected_backend: str = "metal") -> None:
    executable = app_executable(app)
    if not executable.is_file():
        raise PackageError(f"installed app executable is missing: {executable}")

    command = [str(executable), "--print-render-backend"]
    command_text = shlex.join(command)
    try:
        result = subprocess.run(command, check=False, capture_output=True, text=True, timeout=10)
    except subprocess.TimeoutExpired as error:
        raise PackageError(f"backend fingerprint command timed out after {error.timeout:.2f}s: {command_text}") from error

    if result.returncode != 0:
        raise PackageError(
            f"backend fingerprint command failed with exit code {result.returncode}: {command_text}\n{result.stderr.strip()}"
        )

    reported_backend = result.stdout.strip().lower()
    if reported_backend != expected_backend:
        detail = result.stderr.strip()
        suffix = f"\nstderr:\n{detail}" if detail else ""
        raise PackageError(
            f"installed app reported backend '{reported_backend}', expected '{expected_backend}' from {command_text}{suffix}"
        )

    print(f"Validated installed app reports {expected_backend} renderer backend.")


def iter_bundle_linkables(app: Path):
    roots = [app / "Contents" / "MacOS", app / "Contents" / "Frameworks", app / "Contents" / "PlugIns"]
    for root in roots:
        if not root.exists():
            continue
        for path in root.rglob("*"):
            if path.is_file() and not path.is_symlink():
                yield path


def validate_no_forbidden_dylibs(app: Path) -> None:
    frameworks = app / "Contents" / "Frameworks"
    if frameworks.exists():
        bundled_matches = [
            path
            for path in frameworks.rglob("*")
            if path.is_file() and FORBIDDEN_DYLIB_NAME.search(path.name)
        ]
        if bundled_matches:
            raise PackageError(
                "Metal bundle contains forbidden OpenGL/XQuartz dylibs: "
                + ", ".join(str(path.relative_to(app)) for path in bundled_matches)
            )

    linked_matches: list[str] = []
    otool = shutil.which("otool")
    if otool is None:
        raise PackageError("otool is required for macOS package dependency validation")

    for linkable in iter_bundle_linkables(app):
        result = subprocess.run([otool, "-L", str(linkable)], text=True, capture_output=True, check=False)
        if result.returncode != 0:
            continue
        if FORBIDDEN_DYLIB_NAME.search(result.stdout):
            linked_matches.append(f"{linkable.relative_to(app)}:\n{result.stdout}")

    if linked_matches:
        raise PackageError("Metal bundle links forbidden OpenGL/XQuartz dylibs:\n" + "\n".join(linked_matches))
    print("Validated bundle has no forbidden XQuartz/OpenGL dylibs or links.")


def validate_codesign(app: Path) -> None:
    codesign = shutil.which("codesign")
    if codesign is None:
        raise PackageError("codesign is required for macOS package signature validation")

    command = [codesign, "--verify", "--deep", "--strict", "--verbose=2", str(app)]
    command_text = shlex.join(command)
    try:
        result = subprocess.run(command, text=True, capture_output=True, check=False, timeout=30)
    except subprocess.TimeoutExpired as error:
        raise PackageError(f"codesign verification timed out after {error.timeout:.2f}s: {command_text}") from error
    if result.returncode != 0:
        detail = "\n".join(part for part in (result.stdout.strip(), result.stderr.strip()) if part)
        suffix = f"\n{detail}" if detail else ""
        raise PackageError(
            f"codesign verification failed with exit code {result.returncode}: {command_text}{suffix}"
        )

    print(f"Validated code signature for {app}.")


def validate_renderer_probe_artifacts(artifact_dir: Path) -> None:
    required_count = validate_png_artifact_manifest(
        artifact_dir,
        METAL_RENDERER_PROBE_ARTIFACT_SIZES,
        label="installed-app renderer probe artifacts",
    )
    print(f"Validated {required_count} installed-app renderer probe artifacts in {artifact_dir}.")


def validate_map_canvas_artifacts(artifact_dir: Path) -> None:
    baseline = artifact_dir / "map-render-baseline.png"
    if not baseline.is_file():
        raise PackageError(f"missing installed-app map canvas baseline artifact: {baseline}")
    baseline_size = read_png_size(baseline)
    required_count = validate_png_artifact_manifest(
        artifact_dir,
        METAL_MAP_CANVAS_ARTIFACT_SIZES,
        variable_size=baseline_size,
        label="installed-app map canvas smoke artifacts",
    )
    print(f"Validated {required_count} installed-app map canvas smoke artifacts in {artifact_dir}.")


def validate_preview_probe_artifacts(artifact_dir: Path) -> None:
    required_count = validate_png_artifact_manifest(
        artifact_dir,
        METAL_PREVIEW_PROBE_ARTIFACT_SIZES,
        label="installed-app preview probe artifacts",
    )
    print(f"Validated {required_count} installed-app preview probe artifacts in {artifact_dir}.")


def validate_shared_preview_probe_artifacts(artifact_dir: Path) -> None:
    required_count = validate_png_artifact_manifest(
        artifact_dir,
        SYNTHETIC_SHARED_PREVIEW_ARTIFACT_SIZES,
        label="installed-app shared preview probe artifacts",
        allow_extra=True,
    )
    print(f"Validated {required_count} installed-app shared preview probe artifacts in {artifact_dir}.")


def run_installed_smoke(
    repo_root: Path,
    app: Path,
    warcraft_dir: Path | None,
    artifact_dir: Path,
    capture_frame: Path,
    timeout: float,
    expected_source_head: str | None,
    require_current_source: bool,
    require_clean_source: bool,
) -> None:
    bundled_test_map = app / "Contents" / "Resources" / "data" / "test map"
    command = [
        sys.executable,
        str(repo_root / "tools" / "run_metal_smoke.py"),
        str(app),
        "--map",
        str(bundled_test_map),
        "--capture-frame",
        str(capture_frame),
        "--artifact-dir",
        str(artifact_dir),
        "--timeout",
        str(timeout),
    ]
    if warcraft_dir is not None:
        command.extend(["--warcraft-dir", str(warcraft_dir)])
    if expected_source_head is not None:
        command.extend(["--require-source-head", expected_source_head])
    elif require_current_source:
        command.append("--require-current-source")
    if require_clean_source:
        command.append("--require-clean-source")
    run(command, cwd=repo_root, timeout=timeout + 10.0)


def run_installed_renderer_probes(repo_root: Path, app: Path, artifact_dir: Path, timeout: float) -> None:
    probe_artifacts = artifact_dir
    if probe_artifacts.exists():
        shutil.rmtree(probe_artifacts)
    command = [
        str(app / "Contents" / "MacOS" / "HiveWE"),
        "--smoke-metal-renderer-probes",
        "--smoke-artifact-dir",
        str(probe_artifacts),
    ]
    run(command, cwd=repo_root, timeout=timeout)
    validate_renderer_probe_artifacts(probe_artifacts)


def run_installed_map_canvas_smoke(repo_root: Path, app: Path, artifact_dir: Path, timeout: float) -> None:
    map_canvas_artifacts = artifact_dir
    if map_canvas_artifacts.exists():
        shutil.rmtree(map_canvas_artifacts)
    bundled_test_map = app / "Contents" / "Resources" / "data" / "test map"
    command = [
        str(app / "Contents" / "MacOS" / "HiveWE"),
        "--map",
        str(bundled_test_map),
        "--smoke-save-reload",
        "--smoke-mutated-save-reload",
        "--smoke-mpq-export-reload",
        "--smoke-render-modes",
        "--smoke-picking",
        "--smoke-synthetic-model-previews",
        "--smoke-artifact-dir",
        str(map_canvas_artifacts),
        "--capture-delay-ms",
        "1000",
    ]
    run(command, cwd=repo_root, timeout=timeout)
    validate_map_canvas_artifacts(map_canvas_artifacts)


def run_installed_preview_probes(repo_root: Path, app: Path, artifact_dir: Path, timeout: float) -> None:
    probe_artifacts = artifact_dir
    if probe_artifacts.exists():
        shutil.rmtree(probe_artifacts)
    command = [
        str(app / "Contents" / "MacOS" / "HiveWE"),
        "--smoke-metal-preview-probes",
        "--smoke-artifact-dir",
        str(probe_artifacts),
    ]
    run(command, cwd=repo_root, timeout=timeout)
    validate_preview_probe_artifacts(probe_artifacts)


def run_installed_shared_preview_probes(repo_root: Path, app: Path, artifact_dir: Path, timeout: float) -> None:
    probe_artifacts = artifact_dir
    if probe_artifacts.exists():
        shutil.rmtree(probe_artifacts)
    command = [
        str(app / "Contents" / "MacOS" / "HiveWE"),
        "--smoke-shared-preview-probes",
        "--smoke-artifact-dir",
        str(probe_artifacts),
    ]
    run(command, cwd=repo_root, timeout=timeout)
    validate_shared_preview_probe_artifacts(probe_artifacts)


def append_source_requirements(
    command: list[str],
    *,
    expected_source_head: str | None,
    require_current_source: bool,
    require_clean_source: bool,
) -> None:
    if expected_source_head is not None:
        command.extend(["--require-source-head", expected_source_head])
    elif require_current_source:
        command.append("--require-current-source")
    if require_clean_source:
        command.append("--require-clean-source")


def run_installed_shared_preview_parity(
    repo_root: Path,
    app: Path,
    artifact_dir: Path,
    timeout: float,
    expected_source_head: str | None,
    require_current_source: bool,
    require_clean_source: bool,
) -> None:
    if artifact_dir.exists():
        shutil.rmtree(artifact_dir)
    command = [
        sys.executable,
        str(repo_root / "tools" / "run_renderer_parity.py"),
        "--preview-probes-only",
        "--opengl-app",
        str(app),
        "--metal-app",
        str(app),
        "--opengl-artifact-dir",
        str(artifact_dir / "opengl"),
        "--metal-artifact-dir",
        str(artifact_dir / "metal"),
        "--report-dir",
        str(artifact_dir / "report"),
        "--same-app-determinism",
        "--require-capture-manifest",
        "--require-matching-source",
        "--max-rms",
        "0",
        "--max-mean-abs",
        "0",
        "--max-changed-percent",
        "0",
        "--timeout",
        str(timeout),
    ]
    append_source_requirements(
        command,
        expected_source_head=expected_source_head,
        require_current_source=require_current_source,
        require_clean_source=require_clean_source,
    )
    run(command, cwd=repo_root, timeout=(timeout * 2.0) + 20.0)


def run_installed_preview_probe_parity(
    repo_root: Path,
    app: Path,
    artifact_dir: Path,
    timeout: float,
    expected_source_head: str | None,
    require_current_source: bool,
    require_clean_source: bool,
) -> None:
    if artifact_dir.exists():
        shutil.rmtree(artifact_dir)
    command = [
        sys.executable,
        str(repo_root / "tools" / "run_renderer_parity.py"),
        "--metal-preview-probes-only",
        "--opengl-app",
        str(app),
        "--metal-app",
        str(app),
        "--opengl-artifact-dir",
        str(artifact_dir / "opengl"),
        "--metal-artifact-dir",
        str(artifact_dir / "metal"),
        "--report-dir",
        str(artifact_dir / "report"),
        "--same-app-determinism",
        "--require-capture-manifest",
        "--require-matching-source",
        "--max-rms",
        "0",
        "--max-mean-abs",
        "0",
        "--max-changed-percent",
        "0",
        "--timeout",
        str(timeout),
    ]
    append_source_requirements(
        command,
        expected_source_head=expected_source_head,
        require_current_source=require_current_source,
        require_clean_source=require_clean_source,
    )
    run(command, cwd=repo_root, timeout=(timeout * 2.0) + 20.0)


def run_installed_renderer_probe_parity(
    repo_root: Path,
    app: Path,
    artifact_dir: Path,
    timeout: float,
    expected_source_head: str | None,
    require_current_source: bool,
    require_clean_source: bool,
) -> None:
    if artifact_dir.exists():
        shutil.rmtree(artifact_dir)
    command = [
        sys.executable,
        str(repo_root / "tools" / "run_renderer_parity.py"),
        "--metal-renderer-probes-only",
        "--opengl-app",
        str(app),
        "--metal-app",
        str(app),
        "--opengl-artifact-dir",
        str(artifact_dir / "opengl"),
        "--metal-artifact-dir",
        str(artifact_dir / "metal"),
        "--report-dir",
        str(artifact_dir / "report"),
        "--same-app-determinism",
        "--require-capture-manifest",
        "--require-matching-source",
        "--max-rms",
        "0",
        "--max-mean-abs",
        "0",
        "--max-changed-percent",
        "0",
        "--timeout",
        str(timeout),
    ]
    append_source_requirements(
        command,
        expected_source_head=expected_source_head,
        require_current_source=require_current_source,
        require_clean_source=require_clean_source,
    )
    run(command, cwd=repo_root, timeout=(timeout * 2.0) + 20.0)


def run_installed_map_render_parity(
    repo_root: Path,
    app: Path,
    artifact_dir: Path,
    timeout: float,
    expected_source_head: str | None,
    require_current_source: bool,
    require_clean_source: bool,
    map_after_mpq_export_reload: bool = False,
) -> None:
    if artifact_dir.exists():
        shutil.rmtree(artifact_dir)
    bundled_test_map = app / "Contents" / "Resources" / "data" / "test map"
    command = [
        sys.executable,
        str(repo_root / "tools" / "run_renderer_parity.py"),
        "--map-render-only",
        "--map-after-mpq-export-reload" if map_after_mpq_export_reload else "--map-after-save-reload",
        "--map",
        str(bundled_test_map),
        "--opengl-app",
        str(app),
        "--metal-app",
        str(app),
        "--opengl-artifact-dir",
        str(artifact_dir / "opengl"),
        "--metal-artifact-dir",
        str(artifact_dir / "metal"),
        "--report-dir",
        str(artifact_dir / "report"),
        "--same-app-determinism",
        "--require-capture-manifest",
        "--require-matching-source",
        "--max-rms",
        "0",
        "--max-mean-abs",
        "0",
        "--max-changed-percent",
        "0",
        "--timeout",
        str(timeout),
    ]
    append_source_requirements(
        command,
        expected_source_head=expected_source_head,
        require_current_source=require_current_source,
        require_clean_source=require_clean_source,
    )
    run(command, cwd=repo_root, timeout=(timeout * 2.0) + 20.0)


def run_installed_save_reload_smoke(repo_root: Path, app: Path, timeout: float) -> None:
    bundled_test_map = app / "Contents" / "Resources" / "data" / "test map"
    command = [
        str(app / "Contents" / "MacOS" / "HiveWE"),
        "--map",
        str(bundled_test_map),
        "--smoke-save-reload",
        "--smoke-exit-ms",
        "3000",
    ]
    run(command, cwd=repo_root, timeout=timeout)


def run_installed_mpq_export_reload_smoke(repo_root: Path, app: Path, timeout: float) -> None:
    bundled_test_map = app / "Contents" / "Resources" / "data" / "test map"
    command = [
        str(app / "Contents" / "MacOS" / "HiveWE"),
        "--map",
        str(bundled_test_map),
        "--smoke-mpq-export-reload",
        "--smoke-exit-ms",
        "3000",
    ]
    run(command, cwd=repo_root, timeout=timeout)


def run_installed_mutated_save_reload_smoke(repo_root: Path, app: Path, timeout: float) -> None:
    bundled_test_map = app / "Contents" / "Resources" / "data" / "test map"
    command = [
        str(app / "Contents" / "MacOS" / "HiveWE"),
        "--map",
        str(bundled_test_map),
        "--smoke-mutated-save-reload",
        "--smoke-exit-ms",
        "3000",
    ]
    run(command, cwd=repo_root, timeout=timeout)


def expected_cpack_dmg(repo_root: Path, cpack_config: Path) -> Path:
    try:
        config_text = cpack_config.read_text(encoding="utf-8")
    except OSError as error:
        raise PackageError(f"unable to read CPack config {cpack_config}: {error}") from error

    match = re.search(r'set\(CPACK_PACKAGE_FILE_NAME "([^"]+)"\)', config_text)
    if match is None:
        raise PackageError(f"unable to determine CPACK_PACKAGE_FILE_NAME from {cpack_config}")
    return repo_root / f"{match.group(1)}.dmg"


def run_cpack(repo_root: Path, cpack_config: Path) -> Path:
    dmg = expected_cpack_dmg(repo_root, cpack_config)
    previous_mtime_ns = dmg.stat().st_mtime_ns if dmg.exists() else None
    run(["cpack", "-G", "DragNDrop", "--config", str(cpack_config)], cwd=repo_root)
    if not dmg.is_file():
        raise PackageError(f"cpack did not produce expected DMG: {dmg}")
    if previous_mtime_ns is not None and dmg.stat().st_mtime_ns <= previous_mtime_ns:
        raise PackageError(f"cpack did not refresh expected DMG: {dmg}")
    return dmg


def verify_dmg(dmg: Path) -> None:
    hdiutil = shutil.which("hdiutil")
    if hdiutil is None:
        raise PackageError("hdiutil is required for DMG verification")
    run([hdiutil, "verify", str(dmg)])


def validate_dmg_contents(
    repo_root: Path,
    dmg: Path,
    *,
    expected_source_head: str | None = None,
    expected_build_type: str | None = None,
    require_current_source: bool = False,
    require_clean_source: bool = False,
) -> None:
    hdiutil = shutil.which("hdiutil")
    if hdiutil is None:
        raise PackageError("hdiutil is required for DMG content validation")

    with tempfile.TemporaryDirectory(prefix="hivewe-dmg-") as temp_dir:
        mountpoint = Path(temp_dir) / "volume"
        mountpoint.mkdir()
        attached = False
        try:
            run([hdiutil, "attach", str(dmg), "-readonly", "-nobrowse", "-mountpoint", str(mountpoint)])
            attached = True
            app = mountpoint / "HiveWE.app"
            if not app.is_dir():
                raise PackageError(f"DMG does not contain HiveWE.app at its volume root: {dmg}")
            validate_bundle_files(app)
            validate_build_provenance(
                repo_root,
                app,
                expected_source_head=expected_source_head,
                expected_build_type=expected_build_type,
                require_current_source=require_current_source,
                require_clean_source=require_clean_source,
            )
            validate_bundled_test_map(repo_root, app)
            validate_bundled_warcraft_fixture(repo_root, app)
            validate_bundled_metal_shader(repo_root, app)
            validate_no_forbidden_dylibs(app)
            validate_codesign(app)
            validate_reported_backend(app)
        finally:
            if attached:
                run([hdiutil, "detach", str(mountpoint), "-force"])

    print(f"Validated DMG app bundle contents in {dmg}.")


def package_report_checks(args: argparse.Namespace, *, full_smoke_ran: bool, dmg_verified: bool) -> dict:
    return {
        "bundle_files": True,
        "build_provenance": True,
        "bundled_test_map": True,
        "bundled_warcraft_fixture": True,
        "bundled_metal_shader": True,
        "no_forbidden_dylibs": True,
        "codesign": True,
        "reported_backend": True,
        "save_reload_smoke": bool(args.save_reload_smoke),
        "mutated_save_reload_smoke": bool(args.mutated_save_reload_smoke),
        "mpq_export_reload_smoke": bool(args.mpq_export_reload_smoke),
        "renderer_probe_smoke": bool(args.renderer_probe_smoke),
        "renderer_probe_parity_smoke": bool(args.renderer_probe_parity_smoke),
        "map_canvas_smoke": bool(args.map_canvas_smoke),
        "map_render_parity_smoke": bool(args.map_render_parity_smoke),
        "mpq_map_render_parity_smoke": bool(args.mpq_map_render_parity_smoke),
        "preview_probe_smoke": bool(args.preview_probe_smoke),
        "preview_probe_parity_smoke": bool(args.preview_probe_parity_smoke),
        "shared_preview_probe_smoke": bool(args.shared_preview_probe_smoke),
        "shared_preview_parity_smoke": bool(args.shared_preview_parity_smoke),
        "full_installed_smoke": full_smoke_ran,
        "cpack": bool(args.run_cpack),
        "dmg_verified": dmg_verified,
        "dmg_contents": dmg_verified,
    }


def validate_full_smoke_policy(args: argparse.Namespace) -> None:
    if not getattr(args, "require_full_smoke", False):
        return
    if args.skip_smoke:
        raise PackageError("--require-full-smoke cannot be combined with --skip-smoke")
    if args.warcraft_dir is None:
        raise PackageError("--require-full-smoke requires --warcraft-dir or HIVEWE_WARCRAFT_DIR")


def package_report_artifacts(args: argparse.Namespace, *, built_dmg: Path | None, verified_dmg: Path | None) -> dict:
    artifacts: dict[str, str] = {}
    if args.renderer_probe_smoke:
        artifacts["renderer_probe_artifact_dir"] = str(args.renderer_probe_artifact_dir)
    if args.renderer_probe_parity_smoke:
        artifacts["renderer_probe_parity_artifact_dir"] = str(args.renderer_probe_parity_artifact_dir)
    if args.map_canvas_smoke:
        artifacts["map_canvas_artifact_dir"] = str(args.map_canvas_artifact_dir)
    if args.map_render_parity_smoke:
        artifacts["map_render_parity_artifact_dir"] = str(args.map_render_parity_artifact_dir)
    if args.mpq_map_render_parity_smoke:
        artifacts["mpq_map_render_parity_artifact_dir"] = str(args.mpq_map_render_parity_artifact_dir)
    if args.preview_probe_smoke:
        artifacts["preview_probe_artifact_dir"] = str(args.preview_probe_artifact_dir)
    if args.preview_probe_parity_smoke:
        artifacts["preview_probe_parity_artifact_dir"] = str(args.preview_probe_parity_artifact_dir)
    if args.shared_preview_probe_smoke:
        artifacts["shared_preview_probe_artifact_dir"] = str(args.shared_preview_probe_artifact_dir)
    if args.shared_preview_parity_smoke:
        artifacts["shared_preview_parity_artifact_dir"] = str(args.shared_preview_parity_artifact_dir)
    if not args.skip_smoke and args.warcraft_dir is not None:
        artifacts["installed_smoke_artifact_dir"] = str(args.installed_smoke_artifact_dir)
        artifacts["installed_smoke_capture_frame"] = str(args.installed_smoke_capture_frame)
    if built_dmg is not None:
        artifacts["built_dmg"] = str(built_dmg)
    if verified_dmg is not None:
        artifacts["verified_dmg"] = str(verified_dmg)
    return artifacts


def package_report_app_executable(app: Path) -> dict:
    executable = app_executable(app)
    return {
        "path": str(executable),
        "size_bytes": executable.stat().st_size,
        "sha256": sha256_file(executable),
    }


def build_package_report(
    repo_root: Path,
    args: argparse.Namespace,
    *,
    prefix: Path,
    app: Path,
    expected_source_head: str | None,
    require_current_source: bool,
    full_smoke_ran: bool,
    built_dmg: Path | None,
    verified_dmg: Path | None,
) -> dict:
    provenance = read_build_provenance(app)
    return {
        "schema": PACKAGE_REPORT_SCHEMA,
        "label": PACKAGE_REPORT_LABEL,
        "status": "success",
        "validated_at_utc": datetime.now(timezone.utc).isoformat(),
        "repo_root": str(repo_root),
        "build_dir": str(args.build_dir),
        "prefix": str(prefix),
        "app": str(app),
        "app_executable": package_report_app_executable(app),
        "source_revision": provenance.get("source_revision"),
        "cmake": provenance.get("cmake"),
        "expected": {
            "source_head": expected_source_head,
            "require_current_source": require_current_source,
            "require_clean_source": bool(args.require_clean_source),
            "require_full_smoke": bool(args.require_full_smoke),
            "build_type": args.require_build_type,
            "backend": "metal",
        },
        "checks": package_report_checks(
            args,
            full_smoke_ran=full_smoke_ran,
            dmg_verified=verified_dmg is not None,
        ),
        "artifacts": package_report_artifacts(args, built_dmg=built_dmg, verified_dmg=verified_dmg),
    }


def write_package_report_json(path: Path, report: dict) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(report, indent=2, sort_keys=True) + "\n", encoding="utf-8")
    print(f"Wrote package validation JSON report to {path}.")


def package_report_markdown(report: dict) -> str:
    source = report.get("source_revision")
    cmake = report.get("cmake")
    expected = report.get("expected")
    checks = report.get("checks")
    artifacts = report.get("artifacts")
    source_head = source.get("head") if isinstance(source, dict) else None
    source_dirty = source.get("dirty") if isinstance(source, dict) else None
    build_type = cmake.get("build_type") if isinstance(cmake, dict) else None
    lines = [
        "# HiveWE macOS Package Validation",
        "",
        f"- Status: `{report.get('status', 'unknown')}`",
        f"- App: `{report.get('app', 'unknown')}`",
        f"- Build dir: `{report.get('build_dir', 'unknown')}`",
        f"- Source: `{source_head if isinstance(source_head, str) else 'unknown'}`",
        f"- Source dirty: `{source_dirty}`",
        f"- Build type: `{build_type if isinstance(build_type, str) else 'unknown'}`",
        f"- Validated at UTC: `{report.get('validated_at_utc', 'unknown')}`",
        "",
        "## Expected Gates",
        "",
    ]
    if isinstance(expected, dict):
        lines.extend(f"- `{key}`: `{value}`" for key, value in sorted(expected.items()))
    else:
        lines.append("- Not recorded.")
    lines.extend(["", "## Checks", ""])
    if isinstance(checks, dict):
        lines.extend(f"- `{key}`: `{value}`" for key, value in sorted(checks.items()))
    else:
        lines.append("- Not recorded.")
    executable = report.get("app_executable")
    lines.extend(["", "## App Executable", ""])
    if isinstance(executable, dict):
        lines.extend(f"- `{key}`: `{value}`" for key, value in sorted(executable.items()))
    else:
        lines.append("- Not recorded.")
    lines.extend(["", "## Artifacts", ""])
    if isinstance(artifacts, dict) and artifacts:
        lines.extend(f"- `{key}`: `{value}`" for key, value in sorted(artifacts.items()))
    else:
        lines.append("- None.")
    return "\n".join(lines) + "\n"


def write_package_report_markdown(path: Path, report: dict) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(package_report_markdown(report), encoding="utf-8")
    print(f"Wrote package validation Markdown report to {path}.")


def write_self_test_shader_pair(root: Path, source_bytes: bytes, bundled_bytes: bytes | None = None) -> tuple[Path, Path]:
    repo_root = root / "repo"
    app = root / "HiveWE.app"
    source_shader = repo_root / METAL_SHADER_RELATIVE_PATH
    bundled_shader = app / "Contents" / "Resources" / METAL_SHADER_RELATIVE_PATH
    source_shader.parent.mkdir(parents=True, exist_ok=True)
    bundled_shader.parent.mkdir(parents=True, exist_ok=True)
    source_shader.write_bytes(source_bytes)
    bundled_shader.write_bytes(source_bytes if bundled_bytes is None else bundled_bytes)
    return repo_root, app


def write_self_test_file_pair(repo_root: Path, app: Path, relative: Path, source_bytes: bytes, bundled_bytes: bytes | None = None) -> None:
    source_file = repo_root / relative
    bundled_file = app / "Contents" / "Resources" / relative
    source_file.parent.mkdir(parents=True, exist_ok=True)
    bundled_file.parent.mkdir(parents=True, exist_ok=True)
    source_file.write_bytes(source_bytes)
    bundled_file.write_bytes(source_bytes if bundled_bytes is None else bundled_bytes)


def write_self_test_build_provenance(
    app: Path,
    *,
    head: str = "1" * 40,
    dirty: bool = False,
    build_type: str = "Debug",
) -> None:
    provenance_path = app / "Contents" / "Resources" / BUILD_PROVENANCE_RELATIVE_PATH
    provenance_path.parent.mkdir(parents=True, exist_ok=True)
    provenance_path.write_text(
        json.dumps(
            {
                "schema": 1,
                "label": "hivewe-app-build-provenance",
                "source_revision": {
                    "root": "/tmp/hivewe-self-test",
                    "head": head,
                    "branch": "self-test",
                    "dirty": dirty,
                    "status_short": " M dirty.cpp" if dirty else "",
                },
                "cmake": {
                    "build_type": build_type,
                    "enable_metal": "1",
                    "build_opengl_renderer": "0",
                },
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )


def write_self_test_bundle(root: Path) -> tuple[Path, Path]:
    repo_root = root / "repo"
    app = root / "HiveWE.app"
    executable = app / "Contents" / "MacOS" / "HiveWE"
    plist = app / "Contents" / "Info.plist"
    executable.parent.mkdir(parents=True, exist_ok=True)
    plist.parent.mkdir(parents=True, exist_ok=True)
    executable.write_text("#!/bin/sh\n", encoding="utf-8")
    plist.write_text("<plist><dict></dict></plist>\n", encoding="utf-8")
    write_self_test_build_provenance(app)
    write_self_test_file_pair(repo_root, app, Path("data/test map/war3map.w3e"), b"terrain\n")
    write_self_test_file_pair(repo_root, app, Path("data/test map/war3map.w3i"), b"info\n")
    write_self_test_file_pair(repo_root, app, Path("data/test map/imports/probe.txt"), b"payload\n")
    write_self_test_file_pair(repo_root, app, Path("data/warcraft/Cliffs.slk"), b"cliffs\n")
    write_self_test_file_pair(repo_root, app, Path("data/warcraft/scripts/common.j"), b"common\n")
    write_self_test_file_pair(repo_root, app, Path("data/warcraft/scripts/blizzard.j"), b"blizzard\n")
    write_self_test_file_pair(repo_root, app, METAL_SHADER_RELATIVE_PATH, b"kernel void hivewe_smoke() {}\n")
    return repo_root, app


def self_test_pixels(width: int, height: int, seed: int) -> bytes:
    pixels = bytearray()
    for index in range(width * height):
        pixels.extend(((seed + index) & 0xFF, (seed * 3 + index * 5) & 0xFF, (seed * 7 + index * 11) & 0xFF, 255))
    return bytes(pixels)


def write_self_test_png_artifacts(
    artifact_dir: Path,
    artifact_sizes: dict[str, tuple[int, int] | None],
    seed: int,
    *,
    skip_name: str | None = None,
) -> None:
    artifact_dir.mkdir(parents=True, exist_ok=True)
    for index, (name, size) in enumerate(artifact_sizes.items()):
        if name == skip_name:
            continue
        width, height = size if size is not None else (24, 16)
        encode_png_rgba(artifact_dir / name, width, height, self_test_pixels(width, height, seed + index))


def run_self_test() -> int:
    with tempfile.TemporaryDirectory(prefix="hivewe-macos-package-") as temp_dir:
        root = Path(temp_dir)
        bundle_repo_root, bundle_app = write_self_test_bundle(root / "bundle")
        validate_bundle_files(bundle_app)
        validate_build_provenance(bundle_repo_root, bundle_app, require_current_source=False, require_clean_source=False)
        validate_build_provenance(
            bundle_repo_root,
            bundle_app,
            expected_build_type="Debug",
            require_current_source=False,
            require_clean_source=False,
        )
        validate_build_provenance(bundle_repo_root, bundle_app, require_current_source=False, require_clean_source=True)
        validate_build_provenance(
            bundle_repo_root,
            bundle_app,
            expected_source_head="1" * 40,
            expected_build_type="Debug",
            require_current_source=False,
            require_clean_source=True,
        )
        wrong_build_type_failed = False
        try:
            validate_build_provenance(
                bundle_repo_root,
                bundle_app,
                expected_build_type="Release",
                require_current_source=False,
                require_clean_source=False,
            )
        except PackageError:
            wrong_build_type_failed = True
        if not wrong_build_type_failed:
            raise AssertionError("build provenance validation should reject build-type mismatches")
        release_repo_root, release_app = write_self_test_bundle(root / "release-build-provenance")
        write_self_test_build_provenance(release_app, build_type="Release")
        validate_build_provenance(
            release_repo_root,
            release_app,
            expected_build_type="Release",
            require_current_source=False,
            require_clean_source=False,
        )
        explicit_stale_build_failed = False
        try:
            validate_build_provenance(
                bundle_repo_root,
                bundle_app,
                expected_source_head="2" * 40,
                require_current_source=False,
                require_clean_source=False,
            )
        except PackageError:
            explicit_stale_build_failed = True
        if not explicit_stale_build_failed:
            raise AssertionError("build provenance validation should reject explicit source-head mismatches")
        original_current_git_head = current_git_head
        try:
            globals()["current_git_head"] = lambda _repo_root: "1" * 40
            validate_build_provenance(bundle_repo_root, bundle_app, require_current_source=True, require_clean_source=False)

            stale_build_repo_root, stale_build_app = write_self_test_bundle(root / "stale-build-provenance")
            write_self_test_build_provenance(stale_build_app, head="2" * 40)
            stale_build_failed = False
            try:
                validate_build_provenance(
                    stale_build_repo_root,
                    stale_build_app,
                    require_current_source=True,
                    require_clean_source=False,
                )
            except PackageError:
                stale_build_failed = True
            if not stale_build_failed:
                raise AssertionError("build provenance validation should reject stale source revisions")
        finally:
            globals()["current_git_head"] = original_current_git_head
        validate_bundled_test_map(bundle_repo_root, bundle_app)
        validate_bundled_warcraft_fixture(bundle_repo_root, bundle_app)
        validate_bundled_metal_shader(bundle_repo_root, bundle_app)

        codesign_repo_root, codesign_app = write_self_test_bundle(root / "codesign")
        del codesign_repo_root

        class FakeCodesignResult:
            def __init__(self, returncode: int, stdout: str = "", stderr: str = "") -> None:
                self.returncode = returncode
                self.stdout = stdout
                self.stderr = stderr

        original_which = shutil.which
        original_subprocess_run = subprocess.run
        captured_codesign_commands: list[list[str]] = []
        codesign_returncode = 0

        def fake_codesign_which(command: str) -> str | None:
            if command == "codesign":
                return "/usr/bin/codesign"
            return original_which(command)

        def fake_codesign_run(command, *args, **kwargs):
            if command and command[0] == "/usr/bin/codesign":
                captured_codesign_commands.append(list(command))
                if codesign_returncode == 0:
                    return FakeCodesignResult(0)
                return FakeCodesignResult(codesign_returncode, stderr="bundle format is ambiguous")
            return original_subprocess_run(command, *args, **kwargs)

        try:
            shutil.which = fake_codesign_which
            subprocess.run = fake_codesign_run
            validate_codesign(codesign_app)
            if captured_codesign_commands != [
                ["/usr/bin/codesign", "--verify", "--deep", "--strict", "--verbose=2", str(codesign_app)]
            ]:
                raise AssertionError("codesign validation should use strict deep bundle verification")
            codesign_returncode = 1
            invalid_signature_failed = False
            try:
                validate_codesign(codesign_app)
            except PackageError:
                invalid_signature_failed = True
            if not invalid_signature_failed:
                raise AssertionError("codesign validation should reject invalid bundle signatures")
        finally:
            shutil.which = original_which
            subprocess.run = original_subprocess_run

        original_which = shutil.which
        try:
            shutil.which = lambda command: None if command == "codesign" else original_which(command)
            missing_codesign_failed = False
            try:
                validate_codesign(codesign_app)
            except PackageError:
                missing_codesign_failed = True
            if not missing_codesign_failed:
                raise AssertionError("codesign validation should fail when codesign is unavailable")
        finally:
            shutil.which = original_which

        missing_repo_root, missing_app = write_self_test_bundle(root / "missing-bundle-file")
        del missing_repo_root
        (missing_app / "Contents" / "Info.plist").unlink()
        missing_bundle_failed = False
        try:
            validate_bundle_files(missing_app)
        except PackageError:
            missing_bundle_failed = True
        if not missing_bundle_failed:
            raise AssertionError("bundle file validation should reject missing required bundle files")

        dirty_build_repo_root, dirty_build_app = write_self_test_bundle(root / "dirty-build-provenance")
        write_self_test_build_provenance(dirty_build_app, dirty=True)
        dirty_build_failed = False
        try:
            validate_build_provenance(
                dirty_build_repo_root,
                dirty_build_app,
                require_current_source=False,
                require_clean_source=True,
            )
        except PackageError:
            dirty_build_failed = True
        if not dirty_build_failed:
            raise AssertionError("build provenance validation should reject dirty build source")

        stale_map_repo_root, stale_map_app = write_self_test_bundle(root / "stale-map")
        (stale_map_app / "Contents" / "Resources" / "data" / "test map" / "imports" / "probe.txt").write_bytes(b"stale\n")
        stale_map_failed = False
        try:
            validate_bundled_test_map(stale_map_repo_root, stale_map_app)
        except PackageError:
            stale_map_failed = True
        if not stale_map_failed:
            raise AssertionError("test-map validation should reject stale bundled map payloads")

        extra_fixture_repo_root, extra_fixture_app = write_self_test_bundle(root / "extra-fixture")
        extra_fixture = extra_fixture_app / "Contents" / "Resources" / "data" / "warcraft" / "extra.slk"
        extra_fixture.write_bytes(b"extra\n")
        extra_fixture_failed = False
        try:
            validate_bundled_warcraft_fixture(extra_fixture_repo_root, extra_fixture_app)
        except PackageError:
            extra_fixture_failed = True
        if not extra_fixture_failed:
            raise AssertionError("Warcraft fixture validation should reject stale extra files")

        forbidden_app = root / "forbidden-dylib" / "HiveWE.app"
        forbidden_framework = forbidden_app / "Contents" / "Frameworks" / "libGL.dylib"
        forbidden_framework.parent.mkdir(parents=True, exist_ok=True)
        forbidden_framework.write_bytes(b"forbidden\n")
        forbidden_dylib_failed = False
        try:
            validate_no_forbidden_dylibs(forbidden_app)
        except PackageError:
            forbidden_dylib_failed = True
        if not forbidden_dylib_failed:
            raise AssertionError("dylib validation should reject bundled OpenGL/XQuartz dylibs")

        linked_app = root / "forbidden-linked" / "HiveWE.app"
        linked_executable = linked_app / "Contents" / "MacOS" / "HiveWE"
        linked_executable.parent.mkdir(parents=True, exist_ok=True)
        linked_executable.write_bytes(b"binary\n")

        class FakeOtoolResult:
            returncode = 0
            stdout = "\t/opt/X11/lib/libX11.6.dylib (compatibility version 1.0.0, current version 1.0.0)\n"

        original_which = shutil.which
        original_subprocess_run = subprocess.run
        try:
            shutil.which = lambda command: "/usr/bin/otool" if command == "otool" else original_which(command)
            subprocess.run = lambda *args, **kwargs: FakeOtoolResult()
            linked_dylib_failed = False
            try:
                validate_no_forbidden_dylibs(linked_app)
            except PackageError:
                linked_dylib_failed = True
            if not linked_dylib_failed:
                raise AssertionError("dylib validation should reject linkable files that reference XQuartz/OpenGL")
        finally:
            shutil.which = original_which
            subprocess.run = original_subprocess_run

        repo_root, app = write_self_test_shader_pair(root / "matching", b"kernel void hivewe_smoke() {}\n")
        validate_bundled_metal_shader(repo_root, app)

        stale_repo_root, stale_app = write_self_test_shader_pair(
            root / "stale",
            b"kernel void hivewe_smoke() {}\n",
            b"kernel void stale_shader() {}\n",
        )
        stale_failed = False
        try:
            validate_bundled_metal_shader(stale_repo_root, stale_app)
        except PackageError:
            stale_failed = True
        if not stale_failed:
            raise AssertionError("Metal shader validation should reject stale bundled shader content")

        empty_repo_root, empty_app = write_self_test_shader_pair(root / "empty", b"kernel void hivewe_smoke() {}\n", b"")
        empty_failed = False
        try:
            validate_bundled_metal_shader(empty_repo_root, empty_app)
        except PackageError:
            empty_failed = True
        if not empty_failed:
            raise AssertionError("Metal shader validation should reject empty bundled shader content")

        renderer_probe_artifacts = root / "renderer-probes"
        write_self_test_png_artifacts(renderer_probe_artifacts, METAL_RENDERER_PROBE_ARTIFACT_SIZES, 12)
        validate_renderer_probe_artifacts(renderer_probe_artifacts)
        report_args = argparse.Namespace(
            build_dir=Path("build/macOS-Debug-WithTests"),
            require_build_type="Debug",
            require_clean_source=True,
            save_reload_smoke=True,
            mutated_save_reload_smoke=True,
            mpq_export_reload_smoke=True,
            renderer_probe_smoke=True,
            renderer_probe_parity_smoke=True,
            map_canvas_smoke=True,
            map_render_parity_smoke=True,
            mpq_map_render_parity_smoke=True,
            preview_probe_smoke=True,
            preview_probe_parity_smoke=True,
            shared_preview_probe_smoke=True,
            shared_preview_parity_smoke=True,
            skip_smoke=True,
            require_full_smoke=False,
            warcraft_dir=None,
            run_cpack=True,
            renderer_probe_artifact_dir=root / "report-renderer-probes",
            renderer_probe_parity_artifact_dir=root / "report-renderer-parity",
            map_canvas_artifact_dir=root / "report-map-canvas",
            map_render_parity_artifact_dir=root / "report-map-render-parity",
            mpq_map_render_parity_artifact_dir=root / "report-mpq-map-render-parity",
            preview_probe_artifact_dir=root / "report-preview-probes",
            preview_probe_parity_artifact_dir=root / "report-preview-parity",
            shared_preview_probe_artifact_dir=root / "report-shared-preview-probes",
            shared_preview_parity_artifact_dir=root / "report-shared-preview-parity",
            installed_smoke_artifact_dir=root / "report-full-smoke-artifacts",
            installed_smoke_capture_frame=root / "report-full-smoke.png",
        )
        report = build_package_report(
            bundle_repo_root,
            report_args,
            prefix=bundle_app.parent,
            app=bundle_app,
            expected_source_head="1" * 40,
            require_current_source=True,
            full_smoke_ran=False,
            built_dmg=root / "HiveWE-self-test.dmg",
            verified_dmg=root / "HiveWE-self-test.dmg",
        )
        if report.get("label") != PACKAGE_REPORT_LABEL or report.get("status") != "success":
            raise AssertionError("package report should record a successful package validation label")
        report_checks = report.get("checks")
        if not isinstance(report_checks, dict) or not all(
            report_checks.get(name) is True
            for name in (
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
                "codesign",
                "cpack",
                "dmg_verified",
                "dmg_contents",
            )
        ):
            raise AssertionError("package report should record all requested PR package checks")
        report_json = root / "package-report.json"
        report_markdown = root / "package-report.md"
        write_package_report_json(report_json, report)
        write_package_report_markdown(report_markdown, report)
        if json.loads(report_json.read_text(encoding="utf-8")).get("label") != PACKAGE_REPORT_LABEL:
            raise AssertionError("written package report JSON should round-trip")
        report_executable = report.get("app_executable")
        if not isinstance(report_executable, dict) or report_executable.get("sha256") != sha256_file(app_executable(bundle_app)):
            raise AssertionError("package report should fingerprint the installed app executable")
        if "HiveWE macOS Package Validation" not in report_markdown.read_text(encoding="utf-8"):
            raise AssertionError("written package report Markdown should include a title")
        missing_full_smoke_args = argparse.Namespace(skip_smoke=False, require_full_smoke=True, warcraft_dir=None)
        missing_full_smoke_failed = False
        try:
            validate_full_smoke_policy(missing_full_smoke_args)
        except PackageError:
            missing_full_smoke_failed = True
        if not missing_full_smoke_failed:
            raise AssertionError("full-smoke policy should require a Warcraft directory when strict full smoke is requested")

        skipped_full_smoke_args = argparse.Namespace(skip_smoke=True, require_full_smoke=True, warcraft_dir=root / "Warcraft III")
        skipped_full_smoke_failed = False
        try:
            validate_full_smoke_policy(skipped_full_smoke_args)
        except PackageError:
            skipped_full_smoke_failed = True
        if not skipped_full_smoke_failed:
            raise AssertionError("full-smoke policy should reject --skip-smoke with strict full smoke")

        validate_full_smoke_policy(argparse.Namespace(skip_smoke=False, require_full_smoke=True, warcraft_dir=root / "Warcraft III"))
        missing_meshless_probe_artifacts = root / "renderer-probes-missing-meshless"
        write_self_test_png_artifacts(
            missing_meshless_probe_artifacts,
            METAL_RENDERER_PROBE_ARTIFACT_SIZES,
            24,
            skip_name="metal-meshless-bounds-picking.png",
        )
        missing_meshless_failed = False
        try:
            validate_renderer_probe_artifacts(missing_meshless_probe_artifacts)
        except (PackageError, PngError):
            missing_meshless_failed = True
        if not missing_meshless_failed:
            raise AssertionError("renderer probe artifact validation should require meshless bounds picking output")
        missing_pe1_render_artifacts = root / "renderer-probes-missing-pe1-render"
        write_self_test_png_artifacts(
            missing_pe1_render_artifacts,
            METAL_RENDERER_PROBE_ARTIFACT_SIZES,
            24,
            skip_name="metal-pe1-spawn-render.png",
        )
        missing_pe1_render_failed = False
        try:
            validate_renderer_probe_artifacts(missing_pe1_render_artifacts)
        except (PackageError, PngError):
            missing_pe1_render_failed = True
        if not missing_pe1_render_failed:
            raise AssertionError("renderer probe artifact validation should require PE1 spawned-model render output")

        original_run = run
        captured_commands: list[list[str]] = []

        def capture_run(command: list[str], *, cwd: Path | None = None, timeout: float | None = None) -> None:
            del cwd, timeout
            captured_commands.append(command)

        try:
            globals()["run"] = capture_run
            parity_app = root / "parity" / "HiveWE.app"
            run_installed_map_render_parity(root, parity_app, root / "save-parity", 12.0, None, True, True)
            run_installed_map_render_parity(root, parity_app, root / "mpq-parity", 12.0, None, True, True, True)
            run_installed_smoke(
                root,
                parity_app,
                root / "Warcraft III",
                root / "installed-smoke-artifacts",
                root / "installed-smoke.png",
                12.0,
                None,
                True,
                True,
            )
            run_installed_smoke(
                root,
                parity_app,
                root / "Warcraft III",
                root / "installed-smoke-artifacts-by-head",
                root / "installed-smoke-by-head.png",
                12.0,
                "1" * 40,
                False,
                True,
            )
        finally:
            globals()["run"] = original_run

        if len(captured_commands) != 4:
            raise AssertionError("package validator self-test should capture four commands")
        save_command, mpq_command, full_smoke_command, full_smoke_by_head_command = captured_commands
        if "--map-after-save-reload" not in save_command or "--map-after-mpq-export-reload" in save_command:
            raise AssertionError("save/reload map-render parity command should request only save/reload capture")
        if "--map-after-mpq-export-reload" not in mpq_command or "--map-after-save-reload" in mpq_command:
            raise AssertionError("MPQ map-render parity command should request only MPQ export/reload capture")
        if "--require-clean-source" not in save_command or "--require-clean-source" not in mpq_command:
            raise AssertionError("installed-app parity commands should pass through clean-source requirements")
        if "--require-current-source" not in full_smoke_command or "--require-clean-source" not in full_smoke_command:
            raise AssertionError("installed-app full smoke command should pass through source provenance requirements")
        if "--require-source-head" not in full_smoke_by_head_command or "--require-current-source" in full_smoke_by_head_command:
            raise AssertionError("installed-app full smoke command should support explicit source-head requirements")
        if "--require-clean-source" not in full_smoke_by_head_command:
            raise AssertionError("installed-app full smoke command should combine explicit source-head and clean-source requirements")

        backend_app = root / "backend" / "HiveWE.app"
        backend_executable = backend_app / "Contents" / "MacOS" / "HiveWE"
        backend_executable.parent.mkdir(parents=True, exist_ok=True)
        backend_executable.write_text("#!/bin/sh\n", encoding="utf-8")

        class FakeBackendResult:
            def __init__(self, stdout: str) -> None:
                self.returncode = 0
                self.stdout = stdout
                self.stderr = ""

        original_subprocess_run = subprocess.run
        try:
            subprocess.run = lambda *args, **kwargs: FakeBackendResult("metal\n")
            validate_reported_backend(backend_app)

            subprocess.run = lambda *args, **kwargs: FakeBackendResult("opengl\n")
            backend_failed = False
            try:
                validate_reported_backend(backend_app)
            except PackageError:
                backend_failed = True
            if not backend_failed:
                raise AssertionError("backend validation should reject non-Metal installed apps")
        finally:
            subprocess.run = original_subprocess_run

    print("check_macos_package self-test passed.")
    return 0


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate a macOS HiveWE.app install tree.")
    parser.add_argument("--build-dir", type=Path, default=Path("build/macOS-Debug-WithTests"), help="CMake build tree to install.")
    parser.add_argument("--prefix", type=Path, default=None, help="Install prefix. Defaults to <build-dir>/install.")
    parser.add_argument("--skip-install", action="store_true", help="Validate an existing install prefix without running cmake --install.")
    parser.add_argument("--skip-smoke", action="store_true", help="Do not run the installed app Metal smoke helper.")
    parser.add_argument(
        "--require-full-smoke",
        action="store_true",
        help="Fail instead of skipping when the Warcraft-backed installed-app full Metal smoke cannot run.",
    )
    parser.add_argument(
        "--renderer-probe-smoke",
        action="store_true",
        help="Run the installed app's Warcraft-free Metal renderer probe smoke.",
    )
    parser.add_argument(
        "--renderer-probe-parity-smoke",
        action="store_true",
        help="Run a strict same-installed-app Metal renderer probe parity smoke with zero-difference thresholds.",
    )
    parser.add_argument(
        "--map-canvas-smoke",
        action="store_true",
        help="Run the installed app's Warcraft-free Metal save/reload, mutated save/reload, MPQ export/reload, map canvas render-mode, and picking smoke.",
    )
    parser.add_argument(
        "--preview-probe-smoke",
        action="store_true",
        help="Run the installed app's Warcraft-free Metal model editor/object-grid preview probe smoke.",
    )
    parser.add_argument(
        "--preview-probe-parity-smoke",
        action="store_true",
        help="Run a strict same-installed-app Metal preview probe parity smoke with zero-difference thresholds.",
    )
    parser.add_argument(
        "--shared-preview-probe-smoke",
        action="store_true",
        help="Run the installed app's Warcraft-free shared OpenGL/Metal model editor/object-grid preview probe smoke.",
    )
    parser.add_argument(
        "--shared-preview-parity-smoke",
        action="store_true",
        help="Run a strict same-installed-app shared preview parity smoke with zero-difference thresholds.",
    )
    parser.add_argument(
        "--map-render-parity-smoke",
        action="store_true",
        help="Run a strict same-installed-app save/reload map-render parity smoke with zero-difference thresholds.",
    )
    parser.add_argument(
        "--mpq-map-render-parity-smoke",
        action="store_true",
        help="Run a strict same-installed-app MPQ export/reload map-render parity smoke with zero-difference thresholds.",
    )
    parser.add_argument(
        "--require-source-head",
        metavar="SHA",
        help="Require installed app and DMG build provenance to match this exact source revision.",
    )
    parser.add_argument(
        "--require-current-source",
        action="store_true",
        help="Require installed app and DMG build provenance to match the current git HEAD.",
    )
    parser.add_argument(
        "--require-clean-source",
        action="store_true",
        help="Require installed-app parity smoke captures and build provenance to come from a clean git working tree. Implies --require-current-source.",
    )
    parser.add_argument(
        "--require-build-type",
        choices=("Debug", "Release", "RelWithDebInfo"),
        default=None,
        help="Require installed app build provenance to record this CMake build type.",
    )
    parser.add_argument(
        "--save-reload-smoke",
        action="store_true",
        help="Run the installed app's Warcraft-free save/reload smoke against the bundled test map.",
    )
    parser.add_argument(
        "--mutated-save-reload-smoke",
        action="store_true",
        help="Run the installed app's Warcraft-free mutated save/reload smoke against the bundled test map.",
    )
    parser.add_argument(
        "--mpq-export-reload-smoke",
        action="store_true",
        help="Run the installed app's Warcraft-free MPQ export/unpack/reload smoke against the bundled test map.",
    )
    parser.add_argument(
        "--renderer-probe-artifact-dir",
        type=Path,
        default=Path(tempfile.gettempdir()) / "hivewe-installed-metal-renderer-probes",
        help="Directory for installed-app renderer probe PNG artifacts.",
    )
    parser.add_argument(
        "--renderer-probe-parity-artifact-dir",
        type=Path,
        default=Path(tempfile.gettempdir()) / "hivewe-installed-metal-renderer-parity",
        help="Directory for installed-app renderer probe parity artifacts and report.",
    )
    parser.add_argument(
        "--map-canvas-artifact-dir",
        type=Path,
        default=Path(tempfile.gettempdir()) / "hivewe-installed-metal-map-canvas-smoke",
        help="Directory for installed-app map canvas smoke PNG artifacts.",
    )
    parser.add_argument(
        "--preview-probe-artifact-dir",
        type=Path,
        default=Path(tempfile.gettempdir()) / "hivewe-installed-metal-preview-probes",
        help="Directory for installed-app preview probe PNG artifacts.",
    )
    parser.add_argument(
        "--preview-probe-parity-artifact-dir",
        type=Path,
        default=Path(tempfile.gettempdir()) / "hivewe-installed-metal-preview-parity",
        help="Directory for installed-app Metal preview probe parity artifacts and report.",
    )
    parser.add_argument(
        "--shared-preview-probe-artifact-dir",
        type=Path,
        default=Path(tempfile.gettempdir()) / "hivewe-installed-shared-preview-probes",
        help="Directory for installed-app shared preview probe PNG artifacts.",
    )
    parser.add_argument(
        "--shared-preview-parity-artifact-dir",
        type=Path,
        default=Path(tempfile.gettempdir()) / "hivewe-installed-shared-preview-parity",
        help="Directory for installed-app shared preview parity artifacts and report.",
    )
    parser.add_argument(
        "--map-render-parity-artifact-dir",
        type=Path,
        default=Path(tempfile.gettempdir()) / "hivewe-installed-map-render-parity",
        help="Directory for installed-app map-render parity artifacts and report.",
    )
    parser.add_argument(
        "--mpq-map-render-parity-artifact-dir",
        type=Path,
        default=Path(tempfile.gettempdir()) / "hivewe-installed-mpq-map-render-parity",
        help="Directory for installed-app MPQ-backed map-render parity artifacts and report.",
    )
    parser.add_argument(
        "--warcraft-dir",
        type=Path,
        default=os.environ.get("HIVEWE_WARCRAFT_DIR") or None,
        help="Warcraft III installation for installed-app smoke. Defaults to HIVEWE_WARCRAFT_DIR.",
    )
    parser.add_argument(
        "--installed-smoke-artifact-dir",
        type=Path,
        default=Path(tempfile.gettempdir()) / "hivewe-installed-metal-smoke-artifacts",
        help="Directory for installed-app full smoke PNG artifacts.",
    )
    parser.add_argument(
        "--installed-smoke-capture-frame",
        type=Path,
        default=Path(tempfile.gettempdir()) / "hivewe-installed-metal-smoke.png",
        help="Main capture PNG path for installed-app full smoke.",
    )
    parser.add_argument("--smoke-timeout", type=float, default=180.0, help="Maximum seconds to wait for installed-app smoke.")
    parser.add_argument("--run-cpack", action="store_true", help="Build a DragNDrop DMG with cpack after install validation.")
    parser.add_argument("--cpack-config", type=Path, default=None, help="CPackConfig.cmake path. Defaults to <build-dir>/CPackConfig.cmake.")
    parser.add_argument("--verify-dmg", type=Path, default=None, help="Verify this DMG with hdiutil. With --run-cpack, defaults to the newest root DMG.")
    parser.add_argument("--package-report-json", type=Path, help="Write a machine-readable package validation report after success.")
    parser.add_argument("--package-report-markdown", type=Path, help="Write a Markdown package validation report after success.")
    parser.add_argument("--self-test", action="store_true", help="Run dependency-free package validator self-tests and exit.")
    args = parser.parse_args()
    if args.self_test:
        return run_self_test()

    repo_root = Path.cwd()
    if args.require_source_head and args.require_current_source:
        raise PackageError("--require-source-head and --require-current-source are mutually exclusive")
    expected_source_head = args.require_source_head
    require_current_source = args.require_current_source or args.require_clean_source
    if expected_source_head is not None:
        require_current_source = False
    if args.require_clean_source:
        validate_clean_git_tree(repo_root)
    validate_full_smoke_policy(args)

    prefix = args.prefix or args.build_dir / "install"
    if not args.skip_install:
        run(["cmake", "--install", str(args.build_dir), "--prefix", str(prefix)], cwd=repo_root)

    app = installed_app(prefix)
    if not app.is_dir():
        raise PackageError(f"installed app bundle not found: {app}")

    validate_bundle_files(app)
    validate_build_provenance(
        repo_root,
        app,
        expected_source_head=expected_source_head,
        expected_build_type=args.require_build_type,
        require_current_source=require_current_source,
        require_clean_source=args.require_clean_source,
    )
    validate_bundled_test_map(repo_root, app)
    validate_bundled_warcraft_fixture(repo_root, app)
    validate_bundled_metal_shader(repo_root, app)
    validate_no_forbidden_dylibs(app)
    validate_codesign(app)
    validate_reported_backend(app)

    if args.renderer_probe_smoke:
        run_installed_renderer_probes(repo_root, app, args.renderer_probe_artifact_dir, args.smoke_timeout)

    if args.renderer_probe_parity_smoke:
        run_installed_renderer_probe_parity(
            repo_root,
            app,
            args.renderer_probe_parity_artifact_dir,
            args.smoke_timeout,
            expected_source_head,
            require_current_source,
            args.require_clean_source,
        )

    if args.map_canvas_smoke:
        run_installed_map_canvas_smoke(repo_root, app, args.map_canvas_artifact_dir, args.smoke_timeout)

    if args.preview_probe_smoke:
        run_installed_preview_probes(repo_root, app, args.preview_probe_artifact_dir, args.smoke_timeout)

    if args.preview_probe_parity_smoke:
        run_installed_preview_probe_parity(
            repo_root,
            app,
            args.preview_probe_parity_artifact_dir,
            args.smoke_timeout,
            expected_source_head,
            require_current_source,
            args.require_clean_source,
        )

    if args.shared_preview_probe_smoke:
        run_installed_shared_preview_probes(repo_root, app, args.shared_preview_probe_artifact_dir, args.smoke_timeout)

    if args.shared_preview_parity_smoke:
        run_installed_shared_preview_parity(
            repo_root,
            app,
            args.shared_preview_parity_artifact_dir,
            args.smoke_timeout,
            expected_source_head,
            require_current_source,
            args.require_clean_source,
        )

    if args.map_render_parity_smoke:
        run_installed_map_render_parity(
            repo_root,
            app,
            args.map_render_parity_artifact_dir,
            args.smoke_timeout,
            expected_source_head,
            require_current_source,
            args.require_clean_source,
        )

    if args.mpq_map_render_parity_smoke:
        run_installed_map_render_parity(
            repo_root,
            app,
            args.mpq_map_render_parity_artifact_dir,
            args.smoke_timeout,
            expected_source_head,
            require_current_source,
            args.require_clean_source,
            True,
        )

    if args.save_reload_smoke:
        run_installed_save_reload_smoke(repo_root, app, args.smoke_timeout)

    if args.mutated_save_reload_smoke:
        run_installed_mutated_save_reload_smoke(repo_root, app, args.smoke_timeout)

    if args.mpq_export_reload_smoke:
        run_installed_mpq_export_reload_smoke(repo_root, app, args.smoke_timeout)

    full_smoke_ran = False
    if args.skip_smoke:
        print("Skipping installed-app smoke by request.")
    elif args.warcraft_dir is None:
        print("Skipping installed-app full Metal runtime smoke because no Warcraft dir was provided.")
    else:
        run_installed_smoke(
            repo_root,
            app,
            args.warcraft_dir,
            args.installed_smoke_artifact_dir,
            args.installed_smoke_capture_frame,
            args.smoke_timeout,
            expected_source_head,
            require_current_source,
            args.require_clean_source,
        )
        full_smoke_ran = True

    built_dmg = None
    verified_dmg = None
    if args.run_cpack:
        cpack_config = args.cpack_config or args.build_dir / "CPackConfig.cmake"
        built_dmg = run_cpack(repo_root, cpack_config)

    if args.verify_dmg is not None or args.run_cpack:
        dmg_to_verify = args.verify_dmg or built_dmg
        verify_dmg(dmg_to_verify)
        validate_dmg_contents(
            repo_root,
            dmg_to_verify,
            expected_source_head=expected_source_head,
            expected_build_type=args.require_build_type,
            require_current_source=require_current_source,
            require_clean_source=args.require_clean_source,
        )
        verified_dmg = dmg_to_verify

    if args.package_report_json is not None or args.package_report_markdown is not None:
        report_expected_source_head = expected_source_head
        if report_expected_source_head is None and require_current_source:
            report_expected_source_head = current_git_head(repo_root)
        report = build_package_report(
            repo_root,
            args,
            prefix=prefix,
            app=app,
            expected_source_head=report_expected_source_head,
            require_current_source=require_current_source,
            full_smoke_ran=full_smoke_ran,
            built_dmg=built_dmg,
            verified_dmg=verified_dmg,
        )
        if args.package_report_json is not None:
            write_package_report_json(args.package_report_json, report)
        if args.package_report_markdown is not None:
            write_package_report_markdown(args.package_report_markdown, report)

    print(f"macOS package validation succeeded for {app}.")
    return 0


if __name__ == "__main__":
    try:
        sys.exit(main())
    except (OSError, PngError, PackageError) as error:
        print(f"macOS package validation failed: {error}", file=sys.stderr)
        sys.exit(1)
