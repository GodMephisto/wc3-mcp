---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "tools/prepare_renderer_pr_evidence.py"
ref: HEAD
---

#!/usr/bin/env python3
"""Prepare downloaded renderer PR evidence artifacts for final CI comparison."""

from __future__ import annotations

import argparse
import fnmatch
import hashlib
import json
from pathlib import Path
import sys
import tempfile
import zipfile

sys.dont_write_bytecode = True


REQUIRED_INPUTS: tuple[tuple[str, Path], ...] = (
    ("OPENGL_DEFAULT_ARCHIVE", Path("windows-opengl-pr-renderer-evidence/hivewe-opengl-pr-artifacts.zip")),
    ("OPENGL_MPQ_ARCHIVE", Path("windows-opengl-pr-renderer-evidence/hivewe-opengl-pr-mpq-map-artifacts.zip")),
    ("METAL_DEFAULT_ARCHIVE", Path("macos-metal-pr-renderer-evidence-final/hivewe-metal-pr-artifacts.zip")),
    ("METAL_MPQ_ARCHIVE", Path("macos-metal-pr-renderer-evidence-final/hivewe-metal-pr-mpq-map-artifacts.zip")),
    ("INSTALLED_SMOKE_PNG", Path("macos-installed-metal-smoke/hivewe-installed-metal-smoke.png")),
    ("INSTALLED_SMOKE_DIR", Path("macos-installed-metal-smoke/hivewe-installed-metal-smoke-artifacts")),
    ("DEBUG_PACKAGE_REPORT", Path("macos-metal-pr-renderer-evidence-final/hivewe-debug-package-report.json")),
    ("RELEASE_PACKAGE_REPORT", Path("macos-metal-pr-renderer-evidence-final/hivewe-release-package-report.json")),
)

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


RENDERER_EVIDENCE_PATH_PATTERNS: tuple[str, ...] = (
    ".github/workflows/compile.yml",
    "CMakeLists.txt",
    "CMakePresets.json",
    "cmake/**",
    "docs/metal-renderer-status.md",
    "data/shaders/**",
    "data/test map/**",
    "data/warcraft/**",
    "src/base/doodad*.ixx",
    "src/base/pathing_map.ixx",
    "src/base/render_manager.ixx",
    "src/base/resource_manager.ixx",
    "src/base/shadow_map.ixx",
    "src/base/terrain*.ixx",
    "src/base/units.ixx",
    "src/brush/**",
    "src/file_formats/blp.ixx",
    "src/file_formats/mdx/**",
    "src/main.cpp",
    "src/main_window/glwidget*",
    "src/main_window/hivewe.cpp",
    "src/main_window/hivewe.h",
    "src/main_window/map_canvas_widget*",
    "src/main_window/playtest_launcher.cpp",
    "src/model_editor/**",
    "src/object_editor/model_grid*",
    "src/qt_imgui/**",
    "src/rendering/**",
    "src/resources/**",
    "src/runtime_config.h",
    "src/smoke/**",
    "tests/CMakeLists.txt",
    "tests/metal_*",
    "tools/audit_renderer_pr_evidence.py",
    "tools/capture_renderer_pr_evidence.py",
    "tools/check_macos_package.py",
    "tools/compare_smoke_artifacts.py",
    "tools/create_renderer_pr_handoff.py",
    "tools/package_renderer_capture.py",
    "tools/prepare_renderer_pr_evidence.py",
    "tools/run_metal_smoke.py",
    "tools/run_renderer_parity.py",
)


def normalize_changed_path(path: str) -> str:
    normalized = path.replace("\\", "/")
    if normalized.endswith("\r"):
        normalized = normalized[:-1]
    while normalized.startswith("./"):
        normalized = normalized[2:]
    return normalized


def path_requires_renderer_pr_evidence(path: str) -> bool:
    normalized = normalize_changed_path(path)
    return bool(normalized) and any(fnmatch.fnmatchcase(normalized, pattern) for pattern in RENDERER_EVIDENCE_PATH_PATTERNS)


def renderer_evidence_required_for_paths(paths: list[str]) -> bool:
    return any(path_requires_renderer_pr_evidence(path) for path in paths)


def read_changed_paths(path: Path) -> list[str]:
    if not path.exists():
        raise FileNotFoundError(f"changed-file list not found: {path}")
    data = path.read_bytes()
    if not data:
        return []
    raw_paths = data.split(b"\0") if b"\0" in data else data.splitlines()
    return [
        normalized
        for raw_path in raw_paths
        if raw_path
        for normalized in (normalize_changed_path(raw_path.decode("utf-8", errors="surrogateescape")),)
        if normalized
    ]


def required_input_exists(name: str, path: Path) -> bool:
    if name == "INSTALLED_SMOKE_DIR":
        return path.is_dir()
    return path.is_file()


def read_json_object(path: Path) -> dict:
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except (OSError, json.JSONDecodeError) as error:
        raise ValueError(f"could not read JSON object {path}: {error}") from error
    if not isinstance(data, dict):
        raise ValueError(f"expected JSON object in {path}")
    return data


def sha256_file(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as file:
        for chunk in iter(lambda: file.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def sha256_bytes(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def recorded_path_matches(recorded: str, expected: str) -> bool:
    recorded_path = Path(recorded)
    expected_path = Path(expected)
    if not recorded_path.is_absolute():
        recorded_path = Path.cwd() / recorded_path
    if not expected_path.is_absolute():
        expected_path = Path.cwd() / expected_path
    return recorded_path.resolve(strict=False) == expected_path.resolve(strict=False)


def read_capture_manifest_from_archive(path: Path) -> dict:
    try:
        with zipfile.ZipFile(path) as archive:
            manifest_names = [name for name in archive.namelist() if name.endswith("/" + CAPTURE_MANIFEST_NAME)]
            if len(manifest_names) != 1:
                raise ValueError(
                    f"expected exactly one {CAPTURE_MANIFEST_NAME} in {path}, found {len(manifest_names)}"
                )
            manifest = json.loads(archive.read(manifest_names[0]).decode("utf-8"))
    except (OSError, zipfile.BadZipFile, json.JSONDecodeError) as error:
        raise ValueError(f"could not read capture archive {path}: {error}") from error
    if not isinstance(manifest, dict):
        raise ValueError(f"capture archive manifest is not a JSON object: {path}")
    return manifest


def validate_capture_archive_artifacts(path: Path, manifest: dict) -> None:
    artifacts = manifest.get("artifacts")
    if not isinstance(artifacts, list) or not artifacts:
        raise ValueError(f"capture archive manifest is missing PNG fingerprints: {path}")
    try:
        with zipfile.ZipFile(path) as archive:
            manifest_names = [name for name in archive.namelist() if name.endswith("/" + CAPTURE_MANIFEST_NAME)]
            if len(manifest_names) != 1:
                raise ValueError(
                    f"expected exactly one {CAPTURE_MANIFEST_NAME} in {path}, found {len(manifest_names)}"
                )
            prefix = manifest_names[0][: -len(CAPTURE_MANIFEST_NAME)]
            archive_names = set(archive.namelist())
            recorded_names: set[str] = set()
            for index, artifact in enumerate(artifacts):
                if not isinstance(artifact, dict):
                    raise ValueError(f"capture archive manifest has invalid PNG fingerprint {index}: {path}")
                name = artifact.get("name")
                if not isinstance(name, str) or not name or Path(name).name != name or Path(name).is_absolute():
                    raise ValueError(f"capture archive manifest has unsafe PNG fingerprint name {index}: {path}")
                if name in recorded_names:
                    raise ValueError(f"capture archive manifest has duplicate PNG fingerprint for {name}: {path}")
                recorded_names.add(name)
                archive_name = prefix + name
                if archive_name not in archive_names:
                    raise ValueError(f"capture archive is missing fingerprinted PNG {name}: {path}")
                data = archive.read(archive_name)
                if artifact.get("size_bytes") != len(data):
                    raise ValueError(
                        f"capture archive PNG {name} size_bytes mismatch: "
                        f"recorded {artifact.get('size_bytes')!r}, current {len(data)}: {path}"
                    )
                if artifact.get("sha256") != sha256_bytes(data):
                    raise ValueError(f"capture archive PNG {name} SHA-256 mismatch: {path}")
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
                raise ValueError(f"capture archive PNG set mismatch: {'; '.join(details)}: {path}")
    except zipfile.BadZipFile as error:
        raise ValueError(f"could not read capture archive {path}: {error}") from error


def executable_identity_from_capture_manifest(manifest: dict, *, label: str) -> dict[str, object]:
    app = manifest.get("app")
    executable = manifest.get("executable")
    executable_size = manifest.get("executable_size_bytes")
    executable_hash = manifest.get("executable_sha256")
    if not isinstance(app, str) or not app:
        raise ValueError(f"{label} manifest does not record app path")
    if not isinstance(executable, str) or not executable:
        raise ValueError(f"{label} manifest does not record executable path")
    if not isinstance(executable_size, int) or executable_size <= 0:
        raise ValueError(f"{label} manifest does not record a valid executable size")
    if not isinstance(executable_hash, str) or len(executable_hash) != 64:
        raise ValueError(f"{label} manifest does not record executable SHA-256")
    return {
        "app": app,
        "executable": executable,
        "size_bytes": executable_size,
        "sha256": executable_hash,
    }


def executable_identity_from_package_report(path: Path, *, expected_build_type: str) -> dict[str, object]:
    report = read_json_object(path)
    app = report.get("app")
    if not isinstance(app, str) or not app:
        raise ValueError(f"{expected_build_type} package report does not record installed app path: {path}")
    executable = report.get("app_executable")
    if not isinstance(executable, dict):
        raise ValueError(f"{expected_build_type} package report is missing app executable metadata: {path}")
    executable_path = executable.get("path")
    executable_size = executable.get("size_bytes")
    executable_hash = executable.get("sha256")
    if not isinstance(executable_path, str) or not executable_path:
        raise ValueError(f"{expected_build_type} package report does not record app executable path: {path}")
    if not isinstance(executable_size, int) or executable_size <= 0:
        raise ValueError(f"{expected_build_type} package report does not record a valid app executable size: {path}")
    if not isinstance(executable_hash, str) or len(executable_hash) != 64:
        raise ValueError(f"{expected_build_type} package report does not record app executable SHA-256: {path}")
    return {
        "app": app,
        "executable": executable_path,
        "size_bytes": executable_size,
        "sha256": executable_hash,
    }


def validate_identity_matches_package(identity: dict[str, object], package_identity: dict[str, object], *, label: str) -> None:
    if not recorded_path_matches(str(identity["app"]), str(package_identity["app"])):
        raise ValueError(f"{label} app {identity['app']!r} does not match Debug package app {package_identity['app']!r}")
    if not recorded_path_matches(str(identity["executable"]), str(package_identity["executable"])):
        raise ValueError(
            f"{label} executable {identity['executable']!r} does not match Debug package executable "
            f"{package_identity['executable']!r}"
        )
    if identity["size_bytes"] != package_identity["size_bytes"]:
        raise ValueError(
            f"{label} executable size {identity['size_bytes']!r} does not match Debug package "
            f"{package_identity['size_bytes']!r}"
        )
    if identity["sha256"] != package_identity["sha256"]:
        raise ValueError(f"{label} executable SHA-256 does not match Debug package executable")


def validate_identity_matches_identity(
    identity: dict[str, object],
    expected_identity: dict[str, object],
    *,
    label: str,
    expected_label: str,
) -> None:
    if not recorded_path_matches(str(identity["app"]), str(expected_identity["app"])):
        raise ValueError(f"{label} app {identity['app']!r} does not match {expected_label} app {expected_identity['app']!r}")
    if not recorded_path_matches(str(identity["executable"]), str(expected_identity["executable"])):
        raise ValueError(
            f"{label} executable {identity['executable']!r} does not match {expected_label} executable "
            f"{expected_identity['executable']!r}"
        )
    if identity["size_bytes"] != expected_identity["size_bytes"]:
        raise ValueError(
            f"{label} executable size {identity['size_bytes']!r} does not match {expected_label} "
            f"{expected_identity['size_bytes']!r}"
        )
    if identity["sha256"] != expected_identity["sha256"]:
        raise ValueError(f"{label} executable SHA-256 does not match {expected_label}")


def validate_capture_archive(path: Path, *, expected_backend: str, expected_mode: str) -> str:
    manifest = read_capture_manifest_from_archive(path)
    if manifest.get("schema") != CAPTURE_MANIFEST_SCHEMA:
        raise ValueError(f"capture archive manifest must use schema {CAPTURE_MANIFEST_SCHEMA}: {path}")
    if manifest.get("reported_backend") != expected_backend:
        raise ValueError(f"capture archive must report backend {expected_backend!r}: {path}")
    source = manifest.get("source_revision")
    if not isinstance(source, dict) or not isinstance(source.get("head"), str) or not source["head"]:
        raise ValueError(f"capture archive does not record source_revision.head: {path}")
    if source.get("dirty") is not False:
        raise ValueError(f"capture archive does not record clean source provenance: {path}")
    executable_identity_from_capture_manifest(manifest, label=str(path))
    commands = manifest.get("commands")
    if not isinstance(commands, list) or not commands:
        raise ValueError(f"capture archive is missing command provenance: {path}")
    artifacts = manifest.get("artifacts")
    if not isinstance(artifacts, list) or not artifacts:
        raise ValueError(f"capture archive is missing PNG fingerprints: {path}")
    for index, artifact in enumerate(artifacts):
        if not isinstance(artifact, dict) or not isinstance(artifact.get("name"), str) or not isinstance(artifact.get("sha256"), str):
            raise ValueError(f"capture archive has incomplete PNG fingerprint {index}: {path}")
    validate_capture_archive_artifacts(path, manifest)

    probe_only = bool(manifest.get("preview_probes_only")) or bool(manifest.get("metal_preview_probes_only")) or bool(
        manifest.get("metal_renderer_probes_only")
    )
    map_render_only = bool(manifest.get("map_render_only"))
    map_after_save_reload = bool(manifest.get("map_after_save_reload"))
    map_after_mpq_export_reload = bool(manifest.get("map_after_mpq_export_reload"))
    if expected_mode == "default":
        if probe_only or map_render_only or not map_after_save_reload or map_after_mpq_export_reload:
            raise ValueError(f"capture archive is not a saved/reloaded map+preview PR capture: {path}")
    elif expected_mode == "mpq":
        if probe_only or not map_render_only or map_after_save_reload or not map_after_mpq_export_reload:
            raise ValueError(f"capture archive is not an MPQ export/reload map-render PR capture: {path}")
    else:
        raise AssertionError(f"unknown capture mode {expected_mode!r}")
    if manifest.get("smoke_picking") is not True:
        raise ValueError(f"capture archive does not record picking smoke coverage: {path}")
    return source["head"]


def validate_package_report(path: Path, *, expected_build_type: str) -> str:
    report = read_json_object(path)
    if report.get("schema") != PACKAGE_REPORT_SCHEMA or report.get("label") != PACKAGE_REPORT_LABEL:
        raise ValueError(f"package report has unsupported schema or label: {path}")
    if report.get("status") != "success":
        raise ValueError(f"package report does not record successful validation: {path}")
    source = report.get("source_revision")
    if not isinstance(source, dict) or not isinstance(source.get("head"), str) or not source["head"]:
        raise ValueError(f"package report does not record source_revision.head: {path}")
    if source.get("dirty") is not False:
        raise ValueError(f"package report does not record clean source provenance: {path}")
    expected = report.get("expected")
    if not isinstance(expected, dict) or expected.get("build_type") != expected_build_type:
        raise ValueError(f"package report does not record expected build_type={expected_build_type!r}: {path}")
    cmake = report.get("cmake")
    if not isinstance(cmake, dict) or cmake.get("build_type") != expected_build_type:
        raise ValueError(f"package report does not record CMake build_type={expected_build_type!r}: {path}")
    checks = report.get("checks")
    if not isinstance(checks, dict):
        raise ValueError(f"package report is missing check results: {path}")
    missing = [name for name in REQUIRED_PACKAGE_REPORT_CHECKS if checks.get(name) is not True]
    if missing:
        raise ValueError(f"package report is missing required passing check(s) {', '.join(missing)}: {path}")
    executable_identity_from_package_report(path, expected_build_type=expected_build_type)
    return source["head"]


def validate_smoke_provenance(artifact_dir: Path, capture_frame: Path) -> str:
    if not capture_frame.is_file():
        raise ValueError(f"full Metal smoke capture frame is missing: {capture_frame}")
    provenance_path = artifact_dir / SMOKE_PROVENANCE_NAME
    provenance = read_json_object(provenance_path)
    if provenance.get("schema") != 1 or provenance.get("label") != "full-metal-runtime-smoke":
        raise ValueError(f"full Metal smoke provenance has unsupported schema or label: {provenance_path}")
    if provenance.get("expected_backend") != "metal" or provenance.get("reported_backend") != "metal":
        raise ValueError(f"full Metal smoke provenance does not prove the Metal backend: {provenance_path}")
    executable_identity_from_capture_manifest(
        {
            "app": provenance.get("app"),
            "executable": provenance.get("executable"),
            "executable_size_bytes": provenance.get("executable_size_bytes"),
            "executable_sha256": provenance.get("executable_sha256"),
        },
        label=str(provenance_path),
    )

    source = provenance.get("source_revision")
    if not isinstance(source, dict) or not isinstance(source.get("head"), str) or not source["head"]:
        raise ValueError(f"full Metal smoke provenance does not record source_revision.head: {provenance_path}")
    if source.get("dirty") is not False:
        raise ValueError(f"full Metal smoke provenance does not record clean source provenance: {provenance_path}")

    frame = provenance.get("capture_frame")
    if not isinstance(frame, dict):
        raise ValueError(f"full Metal smoke provenance does not record capture-frame metadata: {provenance_path}")
    actual_frame = {
        "name": capture_frame.name,
        "size_bytes": capture_frame.stat().st_size,
        "sha256": sha256_file(capture_frame),
    }
    for key, actual_value in actual_frame.items():
        if frame.get(key) != actual_value:
            raise ValueError(
                f"full Metal smoke provenance capture-frame {key} mismatch: "
                f"recorded {frame.get(key)!r}, current {actual_value!r}: {provenance_path}"
            )

    artifacts = provenance.get("artifacts")
    if not isinstance(artifacts, list) or not artifacts:
        raise ValueError(f"full Metal smoke provenance does not record PNG artifact fingerprints: {provenance_path}")
    recorded_names: set[str] = set()
    for index, artifact in enumerate(artifacts):
        if not isinstance(artifact, dict):
            raise ValueError(f"full Metal smoke provenance artifact fingerprint {index} is not an object: {provenance_path}")
        name = artifact.get("name")
        if not isinstance(name, str) or not name or Path(name).name != name:
            raise ValueError(f"full Metal smoke provenance artifact fingerprint {index} has invalid name: {provenance_path}")
        artifact_path = artifact_dir / name
        if not artifact_path.is_file():
            raise ValueError(f"full Metal smoke provenance artifact is missing: {artifact_path}")
        recorded_names.add(name)
        if artifact.get("size_bytes") != artifact_path.stat().st_size or artifact.get("sha256") != sha256_file(artifact_path):
            raise ValueError(f"full Metal smoke provenance artifact fingerprint mismatch: {artifact_path}")
    actual_names = {path.name for path in artifact_dir.glob("*.png")}
    if actual_names != recorded_names:
        extra = sorted(actual_names - recorded_names)
        missing = sorted(recorded_names - actual_names)
        details = []
        if extra:
            details.append("extra PNG(s): " + ", ".join(extra))
        if missing:
            details.append("missing PNG(s): " + ", ".join(missing))
        raise ValueError(f"full Metal smoke provenance artifact set mismatch: {'; '.join(details)}: {provenance_path}")
    return source["head"]


def validate_ready_inputs(paths: dict[str, Path]) -> list[str]:
    errors: list[str] = []
    source_heads: list[tuple[str, str]] = []
    validators = (
        ("OPENGL_DEFAULT_ARCHIVE", lambda path: validate_capture_archive(path, expected_backend="opengl", expected_mode="default")),
        ("OPENGL_MPQ_ARCHIVE", lambda path: validate_capture_archive(path, expected_backend="opengl", expected_mode="mpq")),
        ("METAL_DEFAULT_ARCHIVE", lambda path: validate_capture_archive(path, expected_backend="metal", expected_mode="default")),
        ("METAL_MPQ_ARCHIVE", lambda path: validate_capture_archive(path, expected_backend="metal", expected_mode="mpq")),
        ("DEBUG_PACKAGE_REPORT", lambda path: validate_package_report(path, expected_build_type="Debug")),
        ("RELEASE_PACKAGE_REPORT", lambda path: validate_package_report(path, expected_build_type="Release")),
    )
    for name, validator in validators:
        try:
            source_heads.append((name, validator(paths[name])))
        except ValueError as error:
            errors.append(f"{paths[name]}: {error}")
    try:
        source_heads.append(("INSTALLED_SMOKE", validate_smoke_provenance(paths["INSTALLED_SMOKE_DIR"], paths["INSTALLED_SMOKE_PNG"])))
    except ValueError as error:
        errors.append(f"{paths['INSTALLED_SMOKE_DIR']}: {error}")

    try:
        for default_name, mpq_name, backend_label in (
            ("OPENGL_DEFAULT_ARCHIVE", "OPENGL_MPQ_ARCHIVE", "OpenGL"),
            ("METAL_DEFAULT_ARCHIVE", "METAL_MPQ_ARCHIVE", "Metal"),
        ):
            default_identity = executable_identity_from_capture_manifest(
                read_capture_manifest_from_archive(paths[default_name]),
                label=f"{backend_label} default capture",
            )
            mpq_identity = executable_identity_from_capture_manifest(
                read_capture_manifest_from_archive(paths[mpq_name]),
                label=f"{backend_label} MPQ capture",
            )
            validate_identity_matches_identity(
                mpq_identity,
                default_identity,
                label=f"{backend_label} MPQ capture",
                expected_label=f"{backend_label} default capture",
            )

        debug_identity = executable_identity_from_package_report(paths["DEBUG_PACKAGE_REPORT"], expected_build_type="Debug")
        for name, label in (("METAL_DEFAULT_ARCHIVE", "default Metal capture"), ("METAL_MPQ_ARCHIVE", "MPQ Metal capture")):
            identity = executable_identity_from_capture_manifest(read_capture_manifest_from_archive(paths[name]), label=label)
            validate_identity_matches_package(identity, debug_identity, label=label)
        smoke_provenance = read_json_object(paths["INSTALLED_SMOKE_DIR"] / SMOKE_PROVENANCE_NAME)
        smoke_identity = executable_identity_from_capture_manifest(
            {
                "app": smoke_provenance.get("app"),
                "executable": smoke_provenance.get("executable"),
                "executable_size_bytes": smoke_provenance.get("executable_size_bytes"),
                "executable_sha256": smoke_provenance.get("executable_sha256"),
            },
            label="installed full Metal smoke",
        )
        validate_identity_matches_package(smoke_identity, debug_identity, label="installed full Metal smoke")
    except ValueError as error:
        errors.append(str(error))
    source_values = {head for _, head in source_heads}
    if len(source_values) > 1:
        errors.append(
            "renderer PR evidence inputs are not from one source revision: "
            + ", ".join(f"{name}={head}" for name, head in source_heads)
        )
    return errors


def append_github_env(path: Path, values: dict[str, str]) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("a", encoding="utf-8") as file:
        for key, value in values.items():
            file.write(f"{key}={value}\n")


def prepare_evidence(artifact_root: Path, output_dir: Path, github_env: Path, skip_note: Path, *, require_evidence: bool = False) -> bool:
    paths = {name: artifact_root / relative for name, relative in REQUIRED_INPUTS}
    missing = [path for name, path in paths.items() if not required_input_exists(name, path)]

    validation_errors = [] if missing else validate_ready_inputs(paths)
    if missing or validation_errors:
        skip_note.parent.mkdir(parents=True, exist_ok=True)
        status = "failed" if require_evidence else "skipped"
        details = []
        if missing:
            details.append("missing required artifact(s):\n" + "".join(f"- {path}\n" for path in missing))
        if validation_errors:
            details.append("invalid required artifact(s):\n" + "".join(f"- {error}\n" for error in validation_errors))
        skip_note.write_text(f"Renderer PR evidence comparison {status}; " + "\n".join(details), encoding="utf-8")
        append_github_env(github_env, {"HIVEWE_FINAL_PR_EVIDENCE_READY": "false"})
        print(skip_note.read_text(encoding="utf-8"), end="")
        return False

    if skip_note.exists():
        skip_note.unlink()
    output_dir.mkdir(parents=True, exist_ok=True)
    append_github_env(
        github_env,
        {
            "HIVEWE_FINAL_PR_EVIDENCE_READY": "true",
            **{name: str(path) for name, path in paths.items()},
            "DEFAULT_PARITY_REPORT_DIR": str(output_dir / "hivewe-final-renderer-parity"),
            "MPQ_PARITY_REPORT_DIR": str(output_dir / "hivewe-final-renderer-mpq-parity"),
            "MANUAL_REVIEW_TEMPLATE": str(output_dir / "hivewe-renderer-manual-review-template.md"),
            "AUTOMATED_AUDIT_REPORT": str(output_dir / "hivewe-final-renderer-pr-evidence-audit.md"),
        },
    )
    print(f"Renderer PR evidence inputs are complete under {artifact_root}")
    return True


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Prepare downloaded renderer PR evidence artifacts for CI comparison.")
    parser.add_argument("--artifact-root", type=Path, help="Root directory populated by actions/download-artifact.")
    parser.add_argument("--output-dir", type=Path, help="Directory where final reports/templates should be written.")
    parser.add_argument("--github-env", type=Path, help="GitHub Actions environment file to append.")
    parser.add_argument("--skip-note", type=Path, help="Path for the skipped-evidence note.")
    parser.add_argument("--require-evidence", action="store_true", help="Exit nonzero when any required renderer PR evidence input is missing.")
    parser.add_argument(
        "--require-evidence-for-renderer-changes",
        action="store_true",
        help="Exit nonzero when --changed-files contains renderer/runtime/evidence-sensitive paths and required inputs are missing.",
    )
    parser.add_argument("--changed-files", type=Path, help="NUL- or newline-delimited changed-file list for --require-evidence-for-renderer-changes.")
    parser.add_argument("--self-test", action="store_true", help="Run dependency-free self-tests and exit.")
    return parser.parse_args()


def require_self_test(condition: bool, message: str) -> None:
    if not condition:
        raise AssertionError(message)


def write_self_test_capture_archive(
    path: Path,
    *,
    backend: str,
    mode: str,
    source_head: str,
    app: Path,
    executable: Path,
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
        raise AssertionError(f"unknown self-test capture mode {mode!r}")
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
                "height": 4,
            }
        ],
    }
    path.parent.mkdir(parents=True, exist_ok=True)
    with zipfile.ZipFile(path, "w", compression=zipfile.ZIP_DEFLATED) as archive:
        archive_root = f"hivewe-{backend}-{mode}"
        archive.writestr(f"{archive_root}/{CAPTURE_MANIFEST_NAME}", json.dumps(manifest))
        archive.writestr(f"{archive_root}/{artifact_name}", artifact_bytes)


def write_self_test_package_report(path: Path, *, build_type: str, source_head: str, app: Path, executable: Path) -> None:
    report = {
        "schema": PACKAGE_REPORT_SCHEMA,
        "label": PACKAGE_REPORT_LABEL,
        "status": "success",
        "app": str(app),
        "app_executable": {
            "path": str(executable),
            "size_bytes": executable.stat().st_size,
            "sha256": sha256_file(executable),
        },
        "source_revision": {"head": source_head, "dirty": False, "status_short": ""},
        "expected": {"build_type": build_type, "backend": "metal", "require_clean_source": True},
        "cmake": {"build_type": build_type, "enable_metal": "1"},
        "checks": {name: True for name in REQUIRED_PACKAGE_REPORT_CHECKS},
    }
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(report), encoding="utf-8")


def run_self_test() -> int:
    with tempfile.TemporaryDirectory(prefix="hivewe-pr-evidence-prepare-") as temp_dir:
        root = Path(temp_dir)
        artifact_root = root / "artifacts"
        output_dir = root / "out"
        github_env = root / "github-env"
        skip_note = output_dir / "skipped.txt"

        ready = prepare_evidence(artifact_root, output_dir, github_env, skip_note)
        require_self_test(not ready, "missing artifacts should not be ready")
        require_self_test("HIVEWE_FINAL_PR_EVIDENCE_READY=false" in github_env.read_text(encoding="utf-8"), "missing artifacts should write false env")
        require_self_test("hivewe-opengl-pr-artifacts.zip" in skip_note.read_text(encoding="utf-8"), "skip note should list missing OpenGL archive")

        required_note = output_dir / "required.txt"
        github_env.write_text("", encoding="utf-8")
        ready = prepare_evidence(artifact_root, output_dir, github_env, required_note, require_evidence=True)
        require_self_test(not ready, "required missing artifacts should not be ready")
        require_self_test("comparison failed" in required_note.read_text(encoding="utf-8"), "required mode should write failed evidence note")
        require_self_test(
            path_requires_renderer_pr_evidence(r".\src\rendering\metal_map_renderer.h"),
            "renderer source changes should require PR evidence",
        )
        require_self_test(
            path_requires_renderer_pr_evidence("src/main_window/map_canvas_widget.cpp"),
            "map canvas widget changes should require PR evidence",
        )
        require_self_test(
            path_requires_renderer_pr_evidence("src/main_window/glwidget.cpp"),
            "legacy map canvas widget rename paths should require PR evidence",
        )
        require_self_test(
            path_requires_renderer_pr_evidence("tools/run_renderer_parity.py"),
            "renderer evidence tooling changes should require PR evidence",
        )
        require_self_test(
            path_requires_renderer_pr_evidence("tools/create_renderer_pr_handoff.py"),
            "renderer handoff tooling changes should require PR evidence",
        )
        require_self_test(
            path_requires_renderer_pr_evidence("data/test map/war3map.w3e"),
            "test-map changes should require PR evidence",
        )
        require_self_test(
            not path_requires_renderer_pr_evidence("README.md"),
            "general README changes should not require proprietary/GPU renderer PR evidence",
        )
        require_self_test(
            path_requires_renderer_pr_evidence("docs/metal-renderer-status.md"),
            "renderer evidence status doc changes should require PR evidence",
        )
        require_self_test(
            path_requires_renderer_pr_evidence("src/resources/gpu_texture.ixx"),
            "renderer resource changes should require PR evidence",
        )
        require_self_test(
            path_requires_renderer_pr_evidence("src/file_formats/mdx/mdx_reader.cpp"),
            "MDX parser changes should require PR evidence",
        )
        require_self_test(
            path_requires_renderer_pr_evidence("src/model_editor/model_editor_metal_widget.cpp"),
            "model-editor preview changes should require PR evidence",
        )
        require_self_test(
            not path_requires_renderer_pr_evidence("src/trigger_editor/jass_editor.cpp"),
            "unrelated editor changes should not require renderer PR evidence",
        )

        github_env.write_text("", encoding="utf-8")
        source_head = "0123456789abcdef0123456789abcdef01234567"
        required_relatives = dict(REQUIRED_INPUTS)
        fake_app = root / "install" / "HiveWE.app"
        fake_executable = fake_app / "Contents" / "MacOS" / "HiveWE"
        fake_executable.parent.mkdir(parents=True)
        fake_executable.write_bytes(b"fake executable\n")
        other_app = root / "other-install" / "HiveWE.app"
        other_executable = other_app / "Contents" / "MacOS" / "HiveWE"
        other_executable.parent.mkdir(parents=True)
        other_executable.write_bytes(b"other executable\n")
        for name, relative in REQUIRED_INPUTS:
            path = artifact_root / relative
            path.parent.mkdir(parents=True, exist_ok=True)
            if name == "OPENGL_DEFAULT_ARCHIVE":
                write_self_test_capture_archive(
                    path, backend="opengl", mode="default", source_head=source_head, app=fake_app, executable=fake_executable
                )
            elif name == "OPENGL_MPQ_ARCHIVE":
                write_self_test_capture_archive(
                    path, backend="opengl", mode="mpq", source_head=source_head, app=fake_app, executable=fake_executable
                )
            elif name == "METAL_DEFAULT_ARCHIVE":
                write_self_test_capture_archive(
                    path, backend="metal", mode="default", source_head=source_head, app=fake_app, executable=fake_executable
                )
            elif name == "METAL_MPQ_ARCHIVE":
                write_self_test_capture_archive(
                    path, backend="metal", mode="mpq", source_head=source_head, app=fake_app, executable=fake_executable
                )
            elif name == "DEBUG_PACKAGE_REPORT":
                write_self_test_package_report(path, build_type="Debug", source_head=source_head, app=fake_app, executable=fake_executable)
            elif name == "RELEASE_PACKAGE_REPORT":
                write_self_test_package_report(path, build_type="Release", source_head=source_head, app=fake_app, executable=fake_executable)
            elif name == "INSTALLED_SMOKE_PNG":
                path.write_bytes(b"fake installed smoke frame\n")
            elif name == "INSTALLED_SMOKE_DIR":
                path.mkdir(exist_ok=True)
                capture_frame = artifact_root / required_relatives["INSTALLED_SMOKE_PNG"]
                smoke_artifact = path / "map-render-baseline.png"
                smoke_artifact.write_bytes(b"fake smoke artifact\n")
                (path / SMOKE_PROVENANCE_NAME).write_text(
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
                                "name": capture_frame.name,
                                "size_bytes": capture_frame.stat().st_size,
                                "sha256": sha256_file(capture_frame),
                            },
                            "artifacts": [
                                {
                                    "name": smoke_artifact.name,
                                    "size_bytes": smoke_artifact.stat().st_size,
                                    "sha256": sha256_file(smoke_artifact),
                                }
                            ],
                        }
                    ),
                    encoding="utf-8",
                )
            else:
                path.write_text("self-test\n", encoding="utf-8")

        ready = prepare_evidence(artifact_root, output_dir, github_env, skip_note)
        env_text = github_env.read_text(encoding="utf-8")
        require_self_test(ready, "complete artifacts should be ready")
        require_self_test("HIVEWE_FINAL_PR_EVIDENCE_READY=true" in env_text, "complete artifacts should write true env")
        require_self_test("OPENGL_DEFAULT_ARCHIVE=" in env_text, "complete artifacts should write archive env")
        require_self_test("AUTOMATED_AUDIT_REPORT=" in env_text, "complete artifacts should write audit report env")
        require_self_test(not skip_note.exists(), "complete artifacts should remove stale skip note")

        invalid_report_root = root / "invalid-report-artifacts"
        shutil_ready_root = artifact_root
        for path in shutil_ready_root.rglob("*"):
            if path.is_file():
                target = invalid_report_root / path.relative_to(shutil_ready_root)
                target.parent.mkdir(parents=True, exist_ok=True)
                target.write_bytes(path.read_bytes())
            elif path.is_dir():
                (invalid_report_root / path.relative_to(shutil_ready_root)).mkdir(parents=True, exist_ok=True)
        invalid_debug_report = invalid_report_root / Path("macos-metal-pr-renderer-evidence-final/hivewe-debug-package-report.json")
        invalid_debug_data = read_json_object(invalid_debug_report)
        invalid_debug_data["checks"].pop("codesign")
        invalid_debug_report.write_text(json.dumps(invalid_debug_data), encoding="utf-8")
        ready = prepare_evidence(invalid_report_root, output_dir, github_env, skip_note, require_evidence=True)
        require_self_test(not ready, "package reports missing codesign should not be ready")
        require_self_test("codesign" in skip_note.read_text(encoding="utf-8"), "invalid package report should explain missing codesign")

        invalid_archive_root = root / "invalid-archive-artifacts"
        for path in shutil_ready_root.rglob("*"):
            if path.is_file():
                target = invalid_archive_root / path.relative_to(shutil_ready_root)
                target.parent.mkdir(parents=True, exist_ok=True)
                target.write_bytes(path.read_bytes())
            elif path.is_dir():
                (invalid_archive_root / path.relative_to(shutil_ready_root)).mkdir(parents=True, exist_ok=True)
        write_self_test_capture_archive(
            invalid_archive_root / Path("macos-metal-pr-renderer-evidence-final/hivewe-metal-pr-mpq-map-artifacts.zip"),
            backend="metal",
            mode="default",
            source_head=source_head,
            app=fake_app,
            executable=fake_executable,
        )
        ready = prepare_evidence(invalid_archive_root, output_dir, github_env, skip_note, require_evidence=True)
        require_self_test(not ready, "wrong-mode MPQ archive should not be ready")
        require_self_test("MPQ export/reload" in skip_note.read_text(encoding="utf-8"), "invalid archive should explain wrong MPQ mode")

        wrong_opengl_pair_root = root / "wrong-opengl-pair-artifacts"
        for path in shutil_ready_root.rglob("*"):
            if path.is_file():
                target = wrong_opengl_pair_root / path.relative_to(shutil_ready_root)
                target.parent.mkdir(parents=True, exist_ok=True)
                target.write_bytes(path.read_bytes())
            elif path.is_dir():
                (wrong_opengl_pair_root / path.relative_to(shutil_ready_root)).mkdir(parents=True, exist_ok=True)
        write_self_test_capture_archive(
            wrong_opengl_pair_root / Path("windows-opengl-pr-renderer-evidence/hivewe-opengl-pr-mpq-map-artifacts.zip"),
            backend="opengl",
            mode="mpq",
            source_head=source_head,
            app=other_app,
            executable=other_executable,
        )
        ready = prepare_evidence(wrong_opengl_pair_root, output_dir, github_env, skip_note, require_evidence=True)
        require_self_test(not ready, "OpenGL captures from different executables should not be ready")
        require_self_test(
            "OpenGL default capture" in skip_note.read_text(encoding="utf-8"),
            "wrong OpenGL capture pair note should explain the default/MPQ executable mismatch",
        )

        stale_capture_payload_root = root / "stale-capture-payload-artifacts"
        for path in shutil_ready_root.rglob("*"):
            if path.is_file():
                target = stale_capture_payload_root / path.relative_to(shutil_ready_root)
                target.parent.mkdir(parents=True, exist_ok=True)
                target.write_bytes(path.read_bytes())
            elif path.is_dir():
                (stale_capture_payload_root / path.relative_to(shutil_ready_root)).mkdir(parents=True, exist_ok=True)
        stale_capture_archive = stale_capture_payload_root / Path("windows-opengl-pr-renderer-evidence/hivewe-opengl-pr-artifacts.zip")
        stale_capture_manifest = read_capture_manifest_from_archive(stale_capture_archive)
        with zipfile.ZipFile(stale_capture_archive, "w", compression=zipfile.ZIP_DEFLATED) as archive:
            archive.writestr(f"hivewe-opengl-default/{CAPTURE_MANIFEST_NAME}", json.dumps(stale_capture_manifest))
            archive.writestr("hivewe-opengl-default/map-render-baseline.png", b"changed capture payload\n")
        ready = prepare_evidence(stale_capture_payload_root, output_dir, github_env, skip_note, require_evidence=True)
        require_self_test(not ready, "stale packaged capture PNG should not be ready")
        require_self_test(
            "capture archive PNG" in skip_note.read_text(encoding="utf-8"),
            "stale packaged capture PNG note should explain archive payload fingerprint mismatch",
        )

        wrong_metal_executable_root = root / "wrong-metal-executable-artifacts"
        for path in shutil_ready_root.rglob("*"):
            if path.is_file():
                target = wrong_metal_executable_root / path.relative_to(shutil_ready_root)
                target.parent.mkdir(parents=True, exist_ok=True)
                target.write_bytes(path.read_bytes())
            elif path.is_dir():
                (wrong_metal_executable_root / path.relative_to(shutil_ready_root)).mkdir(parents=True, exist_ok=True)
        write_self_test_capture_archive(
            wrong_metal_executable_root / Path("macos-metal-pr-renderer-evidence-final/hivewe-metal-pr-artifacts.zip"),
            backend="metal",
            mode="default",
            source_head=source_head,
            app=other_app,
            executable=other_executable,
        )
        write_self_test_capture_archive(
            wrong_metal_executable_root / Path("macos-metal-pr-renderer-evidence-final/hivewe-metal-pr-mpq-map-artifacts.zip"),
            backend="metal",
            mode="mpq",
            source_head=source_head,
            app=other_app,
            executable=other_executable,
        )
        ready = prepare_evidence(wrong_metal_executable_root, output_dir, github_env, skip_note, require_evidence=True)
        require_self_test(not ready, "Metal capture from a different executable should not be ready")
        require_self_test(
            "does not match Debug package" in skip_note.read_text(encoding="utf-8"),
            "wrong Metal executable note should explain Debug package mismatch",
        )

        wrong_smoke_executable_root = root / "wrong-smoke-executable-artifacts"
        for path in shutil_ready_root.rglob("*"):
            if path.is_file():
                target = wrong_smoke_executable_root / path.relative_to(shutil_ready_root)
                target.parent.mkdir(parents=True, exist_ok=True)
                target.write_bytes(path.read_bytes())
            elif path.is_dir():
                (wrong_smoke_executable_root / path.relative_to(shutil_ready_root)).mkdir(parents=True, exist_ok=True)
        wrong_smoke_provenance = (
            wrong_smoke_executable_root
            / Path("macos-installed-metal-smoke/hivewe-installed-metal-smoke-artifacts")
            / SMOKE_PROVENANCE_NAME
        )
        wrong_smoke_provenance_data = read_json_object(wrong_smoke_provenance)
        wrong_smoke_provenance_data["app"] = str(other_app)
        wrong_smoke_provenance_data["executable"] = str(other_executable)
        wrong_smoke_provenance_data["executable_size_bytes"] = other_executable.stat().st_size
        wrong_smoke_provenance_data["executable_sha256"] = sha256_file(other_executable)
        wrong_smoke_provenance.write_text(json.dumps(wrong_smoke_provenance_data), encoding="utf-8")
        ready = prepare_evidence(wrong_smoke_executable_root, output_dir, github_env, skip_note, require_evidence=True)
        require_self_test(not ready, "full smoke from a different executable should not be ready")
        require_self_test(
            "does not match Debug package" in skip_note.read_text(encoding="utf-8"),
            "wrong smoke executable note should explain Debug package mismatch",
        )

        stale_smoke_png_root = root / "stale-smoke-png-artifacts"
        for path in shutil_ready_root.rglob("*"):
            if path.is_file():
                target = stale_smoke_png_root / path.relative_to(shutil_ready_root)
                target.parent.mkdir(parents=True, exist_ok=True)
                target.write_bytes(path.read_bytes())
            elif path.is_dir():
                (stale_smoke_png_root / path.relative_to(shutil_ready_root)).mkdir(parents=True, exist_ok=True)
        stale_smoke_png = stale_smoke_png_root / Path("macos-installed-metal-smoke/hivewe-installed-metal-smoke.png")
        stale_smoke_png.write_bytes(b"changed installed smoke frame\n")
        ready = prepare_evidence(stale_smoke_png_root, output_dir, github_env, skip_note, require_evidence=True)
        require_self_test(not ready, "stale installed smoke frame should not be ready")
        require_self_test(
            "capture-frame" in skip_note.read_text(encoding="utf-8"),
            "stale installed smoke frame note should explain capture-frame provenance",
        )

        stale_smoke_artifact_root = root / "stale-smoke-artifact-root"
        for path in shutil_ready_root.rglob("*"):
            if path.is_file():
                target = stale_smoke_artifact_root / path.relative_to(shutil_ready_root)
                target.parent.mkdir(parents=True, exist_ok=True)
                target.write_bytes(path.read_bytes())
            elif path.is_dir():
                (stale_smoke_artifact_root / path.relative_to(shutil_ready_root)).mkdir(parents=True, exist_ok=True)
        stale_smoke_artifact = stale_smoke_artifact_root / Path(
            "macos-installed-metal-smoke/hivewe-installed-metal-smoke-artifacts/map-render-baseline.png"
        )
        stale_smoke_artifact.write_bytes(b"changed smoke artifact\n")
        ready = prepare_evidence(stale_smoke_artifact_root, output_dir, github_env, skip_note, require_evidence=True)
        require_self_test(not ready, "stale installed smoke artifact should not be ready")
        require_self_test(
            "artifact fingerprint mismatch" in skip_note.read_text(encoding="utf-8"),
            "stale installed smoke artifact note should explain artifact fingerprint mismatch",
        )

        mismatched_source_root = root / "mismatched-source-artifacts"
        for path in shutil_ready_root.rglob("*"):
            if path.is_file():
                target = mismatched_source_root / path.relative_to(shutil_ready_root)
                target.parent.mkdir(parents=True, exist_ok=True)
                target.write_bytes(path.read_bytes())
            elif path.is_dir():
                (mismatched_source_root / path.relative_to(shutil_ready_root)).mkdir(parents=True, exist_ok=True)
        smoke_provenance = mismatched_source_root / Path("macos-installed-metal-smoke/hivewe-installed-metal-smoke-artifacts") / SMOKE_PROVENANCE_NAME
        smoke_provenance_data = read_json_object(smoke_provenance)
        smoke_provenance_data["source_revision"] = {"head": "f" * 40, "dirty": False, "status_short": ""}
        smoke_provenance.write_text(json.dumps(smoke_provenance_data), encoding="utf-8")
        ready = prepare_evidence(mismatched_source_root, output_dir, github_env, skip_note, require_evidence=True)
        require_self_test(not ready, "mismatched installed smoke source should not be ready")
        require_self_test(
            "not from one source revision" in skip_note.read_text(encoding="utf-8"),
            "mismatched source note should list source revisions",
        )

        dirty_smoke_root = root / "dirty-smoke-artifacts"
        for path in shutil_ready_root.rglob("*"):
            if path.is_file():
                target = dirty_smoke_root / path.relative_to(shutil_ready_root)
                target.parent.mkdir(parents=True, exist_ok=True)
                target.write_bytes(path.read_bytes())
            elif path.is_dir():
                (dirty_smoke_root / path.relative_to(shutil_ready_root)).mkdir(parents=True, exist_ok=True)
        dirty_smoke_provenance = dirty_smoke_root / Path("macos-installed-metal-smoke/hivewe-installed-metal-smoke-artifacts") / SMOKE_PROVENANCE_NAME
        dirty_smoke_provenance_data = read_json_object(dirty_smoke_provenance)
        dirty_smoke_provenance_data["source_revision"] = {"head": source_head, "dirty": True, "status_short": [" M src/rendering/metal_map_renderer.mm"]}
        dirty_smoke_provenance.write_text(json.dumps(dirty_smoke_provenance_data), encoding="utf-8")
        ready = prepare_evidence(dirty_smoke_root, output_dir, github_env, skip_note, require_evidence=True)
        require_self_test(not ready, "dirty installed smoke source should not be ready")
        require_self_test(
            "clean source provenance" in skip_note.read_text(encoding="utf-8"),
            "dirty installed smoke note should explain clean source provenance",
        )

        changed_files = root / "changed-files.txt"
        changed_files.write_text("README.md\nsrc/main_window/map_canvas_widget.h\n", encoding="utf-8")
        require_self_test(renderer_evidence_required_for_paths(read_changed_paths(changed_files)), "changed renderer source should require evidence")

        nul_changed_files = root / "changed-files-z.txt"
        nul_changed_files.write_bytes(b"src/model_editor/model_editor_metal_widget.cpp\0README.md\0")
        require_self_test(renderer_evidence_required_for_paths(read_changed_paths(nul_changed_files)), "NUL-delimited changed renderer paths should require evidence")

        wrong_type_root = root / "wrong-type-artifacts"
        for name, relative in REQUIRED_INPUTS:
            path = wrong_type_root / relative
            path.parent.mkdir(parents=True, exist_ok=True)
            if name == "OPENGL_DEFAULT_ARCHIVE":
                path.mkdir(exist_ok=True)
            elif name == "INSTALLED_SMOKE_DIR":
                path.mkdir(exist_ok=True)
            else:
                path.write_text("self-test\n", encoding="utf-8")
        ready = prepare_evidence(wrong_type_root, output_dir, github_env, skip_note)
        require_self_test(not ready, "wrong-typed archive path should not be ready")
        require_self_test("hivewe-opengl-pr-artifacts.zip" in skip_note.read_text(encoding="utf-8"), "wrong-typed archive path should be reported")

    print("prepare_renderer_pr_evidence self-test passed.")
    return 0


def main() -> int:
    args = parse_args()
    if args.self_test:
        return run_self_test()
    missing_args = [
        name
        for name, value in (
            ("--artifact-root", args.artifact_root),
            ("--output-dir", args.output_dir),
            ("--github-env", args.github_env),
            ("--skip-note", args.skip_note),
        )
        if value is None
    ]
    if missing_args:
        print("Missing required argument(s): " + ", ".join(missing_args), file=sys.stderr)
        return 2
    require_evidence = args.require_evidence
    if args.require_evidence_for_renderer_changes:
        if args.changed_files is None:
            print("--require-evidence-for-renderer-changes requires --changed-files", file=sys.stderr)
            return 2
        changed_paths = read_changed_paths(args.changed_files)
        if renderer_evidence_required_for_paths(changed_paths):
            require_evidence = True
            print("Renderer PR evidence is required because this pull request changes renderer/runtime/evidence-sensitive paths.")
        else:
            print("Renderer PR evidence is optional because no renderer/runtime/evidence-sensitive paths changed.")
    ready = prepare_evidence(
        args.artifact_root,
        args.output_dir,
        args.github_env,
        args.skip_note,
        require_evidence=require_evidence,
    )
    return 0 if ready or not require_evidence else 1


if __name__ == "__main__":
    sys.exit(main())
