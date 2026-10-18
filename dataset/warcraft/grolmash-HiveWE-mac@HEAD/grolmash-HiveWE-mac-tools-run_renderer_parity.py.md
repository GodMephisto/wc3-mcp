---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "tools/run_renderer_parity.py"
ref: HEAD
---

#!/usr/bin/env python3
"""Capture and compare OpenGL/Metal renderer smoke artifacts."""

from __future__ import annotations

import argparse
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path, PurePosixPath
import platform
import shlex
import shutil
import subprocess
import sys
import tempfile
import zipfile
import zlib

sys.dont_write_bytecode = True

from compare_smoke_artifacts import (
    ARTIFACT_PRESETS,
    MAP_RENDER_ARTIFACT_SIZES,
    METAL_PREVIEW_PROBE_ARTIFACT_SIZES,
    METAL_RENDERER_PROBE_ARTIFACT_SIZES,
    PngError,
    SHARED_PREVIEW_ARTIFACT_SIZES,
    SYNTHETIC_SHARED_PREVIEW_ARTIFACT_SIZES,
    compare_artifact_directories,
    encode_png_rgba,
    read_png_size,
    validate_png_artifact_manifest,
)

CAPTURE_MANIFEST_NAME = "renderer-parity-capture.json"
VALID_RENDERER_BACKENDS = {"opengl", "metal"}


class CaptureManifestError(RuntimeError):
    pass


def safe_archive_parts(name: str) -> tuple[str, ...]:
    if "\\" in name:
        raise CaptureManifestError(f"capture archive contains unsafe path separator: {name}")
    path = PurePosixPath(name)
    if path.is_absolute():
        raise CaptureManifestError(f"capture archive contains absolute path: {name}")
    parts = path.parts
    if not parts or any(part in ("", ".", "..") for part in parts):
        raise CaptureManifestError(f"capture archive contains unsafe path: {name}")
    if any(":" in part for part in parts):
        raise CaptureManifestError(f"capture archive contains unsafe drive-qualified path: {name}")
    return parts


def extract_capture_archive(label: str, archive_path: Path, destination_dir: Path) -> Path:
    if not archive_path.is_file():
        raise FileNotFoundError(f"{label} capture archive not found: {archive_path}")
    if destination_dir.exists():
        shutil.rmtree(destination_dir)
    destination_dir.mkdir(parents=True, exist_ok=True)

    try:
        with zipfile.ZipFile(archive_path) as archive:
            file_infos = [info for info in archive.infolist() if not info.is_dir()]
            if not file_infos:
                raise CaptureManifestError(f"{label} capture archive is empty: {archive_path}")

            member_parts = [(info, safe_archive_parts(info.filename)) for info in file_infos]
            manifest_members = [
                parts for _, parts in member_parts
                if parts[-1] == CAPTURE_MANIFEST_NAME
            ]
            if len(manifest_members) != 1:
                raise CaptureManifestError(
                    f"{label} capture archive must contain exactly one {CAPTURE_MANIFEST_NAME}: {archive_path}"
                )

            prefix = manifest_members[0][:-1]
            if not prefix:
                raise CaptureManifestError(
                    f"{label} capture archive must keep artifacts under one top-level directory: {archive_path}"
                )
            for _, parts in member_parts:
                if parts[:len(prefix)] != prefix:
                    raise CaptureManifestError(
                        f"{label} capture archive contains files outside capture prefix {'/'.join(prefix)}: {archive_path}"
                    )

            for info, parts in member_parts:
                output_path = destination_dir.joinpath(*parts)
                output_path.parent.mkdir(parents=True, exist_ok=True)
                output_path.write_bytes(archive.read(info))
    except zipfile.BadZipFile as error:
        raise CaptureManifestError(f"{label} capture archive is not a valid zip: {archive_path}") from error

    return destination_dir.joinpath(*prefix)


def resolved_warcraft_dir(cli_warcraft_dir: Path | None) -> Path | None:
    if cli_warcraft_dir is not None:
        return cli_warcraft_dir
    env_warcraft_dir = os.environ.get("HIVEWE_WARCRAFT_DIR")
    if env_warcraft_dir:
        return Path(env_warcraft_dir)
    return None


def sha256_file(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for block in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(block)
    return digest.hexdigest()


def capture_artifact_fingerprints(artifact_dir: Path) -> list[dict]:
    artifacts = []
    for path in sorted(artifact_dir.glob("*.png")):
        width, height = read_png_size(path)
        artifacts.append(
            {
                "name": path.name,
                "width": width,
                "height": height,
                "size_bytes": path.stat().st_size,
                "sha256": sha256_file(path),
            }
        )
    return artifacts


def git_output(root: Path, *args: str) -> str | None:
    try:
        result = subprocess.run(
            ["git", "-C", str(root), *args],
            check=True,
            capture_output=True,
            text=True,
            timeout=5,
        )
    except (FileNotFoundError, subprocess.CalledProcessError, subprocess.TimeoutExpired):
        return None
    return result.stdout.strip()


def source_revision_metadata() -> dict | None:
    for candidate in (Path.cwd(), Path(__file__).resolve().parents[1]):
        toplevel = git_output(candidate, "rev-parse", "--show-toplevel")
        if not toplevel:
            continue
        root = Path(toplevel)
        head = git_output(root, "rev-parse", "HEAD")
        if not head:
            continue
        status = git_output(root, "status", "--short")
        branch = git_output(root, "rev-parse", "--abbrev-ref", "HEAD")
        return {
            "root": str(root),
            "head": head,
            "branch": branch,
            "dirty": bool(status),
            "status_short": [] if not status else status.splitlines(),
        }
    return None


def resolve_executable(app: Path) -> Path:
    if app.is_dir() and app.suffix == ".app":
        executable = app / "Contents" / "MacOS" / "HiveWE"
    else:
        executable = app
    if not executable.is_file():
        raise FileNotFoundError(f"HiveWE executable not found: {executable}")
    return executable


def run_command(command: list[str], timeout: float) -> None:
    print("Running:", shlex.join(command), flush=True)
    subprocess.run(command, check=True, timeout=timeout)


def query_renderer_backend(executable: Path, timeout: float) -> str:
    command = [str(executable), "--print-render-backend"]
    print("Running:", shlex.join(command), flush=True)
    result = subprocess.run(command, check=True, capture_output=True, text=True, timeout=timeout)
    lines = [line.strip().lower() for line in result.stdout.splitlines() if line.strip()]
    backend = lines[-1] if lines else ""
    if backend not in VALID_RENDERER_BACKENDS:
        stderr = result.stderr.strip()
        suffix = f" stderr: {stderr}" if stderr else ""
        raise CaptureManifestError(
            f"{executable} reported invalid renderer backend {backend!r}; expected one of "
            f"{sorted(VALID_RENDERER_BACKENDS)}.{suffix}"
        )
    return backend


def write_capture_manifest(
    *,
    label: str,
    expected_backend: str | None,
    reported_backend: str | None,
    app: Path,
    executable: Path,
    artifact_dir: Path,
    map_path: Path,
    warcraft_dir: Path | None,
    capture_delay_ms: int,
    preview_probes_only: bool,
    map_render_only: bool,
    metal_preview_probes_only: bool,
    metal_renderer_probes_only: bool,
    map_after_save_reload: bool,
    map_after_mpq_export_reload: bool,
    commands: list[list[str]],
) -> dict:
    probe_only = preview_probes_only or metal_preview_probes_only or metal_renderer_probes_only
    smoke_picking = False if probe_only else any("--smoke-picking" in command for command in commands)
    manifest = {
        "schema": 8,
        "label": label,
        "expected_backend": expected_backend,
        "reported_backend": reported_backend,
        "captured_at_utc": datetime.now(timezone.utc).isoformat(),
        "host": platform.platform(),
        "app": str(app),
        "executable": str(executable),
        "executable_size_bytes": executable.stat().st_size,
        "executable_sha256": sha256_file(executable),
        "source_revision": source_revision_metadata(),
        "artifact_dir": str(artifact_dir),
        "map": None if probe_only else str(map_path),
        "warcraft_dir": None if warcraft_dir is None else str(warcraft_dir),
        "capture_delay_ms": capture_delay_ms,
        "preview_probes_only": preview_probes_only,
        "map_render_only": False if probe_only else map_render_only,
        "metal_preview_probes_only": metal_preview_probes_only,
        "metal_renderer_probes_only": metal_renderer_probes_only,
        "map_after_save_reload": False if probe_only else map_after_save_reload,
        "map_after_mpq_export_reload": False if probe_only else map_after_mpq_export_reload,
        "smoke_picking": smoke_picking,
        "commands": commands,
        "artifacts": capture_artifact_fingerprints(artifact_dir),
    }
    (artifact_dir / CAPTURE_MANIFEST_NAME).write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
    return manifest


def read_capture_manifest(artifact_dir: Path) -> dict | None:
    path = artifact_dir / CAPTURE_MANIFEST_NAME
    if not path.is_file():
        return None
    try:
        manifest = json.loads(path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as error:
        raise CaptureManifestError(f"invalid capture manifest {path}: {error}") from error
    if not isinstance(manifest, dict):
        raise CaptureManifestError(f"invalid capture manifest {path}: expected object")
    return manifest


def expected_capture_artifact_names(
    *,
    preview_probes_only: bool,
    map_render_only: bool,
    metal_preview_probes_only: bool,
    metal_renderer_probes_only: bool,
) -> set[str]:
    if preview_probes_only:
        return set(SYNTHETIC_SHARED_PREVIEW_ARTIFACT_SIZES)
    if metal_preview_probes_only:
        return set(METAL_PREVIEW_PROBE_ARTIFACT_SIZES)
    if metal_renderer_probes_only:
        return set(METAL_RENDERER_PROBE_ARTIFACT_SIZES)
    names = set(MAP_RENDER_ARTIFACT_SIZES)
    if not map_render_only:
        names.update(SHARED_PREVIEW_ARTIFACT_SIZES)
    return names


def validate_capture_artifact_fingerprints(
    label: str,
    artifact_dir: Path,
    manifest: dict,
    *,
    require_artifact_fingerprints: bool = False,
    required_artifact_names: set[str] | None = None,
) -> None:
    if require_artifact_fingerprints and manifest.get("schema") != 8:
        raise CaptureManifestError(f"{label} artifact manifest must use schema 8 to prove PNG fingerprints")
    artifacts = manifest.get("artifacts")
    if artifacts is None:
        if require_artifact_fingerprints:
            raise CaptureManifestError(f"{label} artifact manifest is missing PNG fingerprints")
        return
    if not isinstance(artifacts, list):
        raise CaptureManifestError(f"{label} artifact manifest has invalid artifacts field: expected list")
    if require_artifact_fingerprints and not artifacts:
        raise CaptureManifestError(f"{label} artifact manifest has no PNG fingerprints")

    manifest_names: set[str] = set()
    for index, artifact in enumerate(artifacts):
        if not isinstance(artifact, dict):
            raise CaptureManifestError(f"{label} artifact manifest has invalid artifact entry {index}: expected object")
        name = artifact.get("name")
        expected_width = artifact.get("width")
        expected_height = artifact.get("height")
        expected_size = artifact.get("size_bytes")
        expected_sha256 = artifact.get("sha256")
        if (
            not isinstance(name, str)
            or not isinstance(expected_width, int)
            or not isinstance(expected_height, int)
            or not isinstance(expected_size, int)
            or not isinstance(expected_sha256, str)
        ):
            raise CaptureManifestError(f"{label} artifact manifest has incomplete fingerprint entry {index}")
        if Path(name).name != name or Path(name).is_absolute():
            raise CaptureManifestError(f"{label} artifact manifest has unsafe artifact name in entry {index}: {name}")
        if name in manifest_names:
            raise CaptureManifestError(f"{label} artifact manifest has duplicate fingerprint entry for {name}")
        manifest_names.add(name)

        path = artifact_dir / name
        if not path.is_file():
            raise CaptureManifestError(f"{label} artifact fingerprint references missing file: {path}")

        actual_width, actual_height = read_png_size(path)
        actual_size = path.stat().st_size
        actual_sha256 = sha256_file(path)
        if (actual_width, actual_height) != (expected_width, expected_height):
            raise CaptureManifestError(
                f"{label} artifact {name} dimensions changed after capture: "
                f"{actual_width}x{actual_height}, expected {expected_width}x{expected_height}"
            )
        if actual_size != expected_size:
            raise CaptureManifestError(
                f"{label} artifact {name} size changed after capture: {actual_size}, expected {expected_size}"
            )
        if actual_sha256 != expected_sha256:
            raise CaptureManifestError(f"{label} artifact {name} SHA-256 changed after capture")

    extra_pngs = sorted(path.name for path in artifact_dir.glob("*.png") if path.name not in manifest_names)
    if extra_pngs:
        raise CaptureManifestError(
            f"{label} artifact directory contains PNGs not fingerprinted by {CAPTURE_MANIFEST_NAME}: "
            + ", ".join(extra_pngs)
        )
    if require_artifact_fingerprints and required_artifact_names is not None:
        missing = sorted(required_artifact_names - manifest_names)
        if missing:
            raise CaptureManifestError(
                f"{label} artifact manifest is missing required PNG fingerprint(s): " + ", ".join(missing)
            )


def manifest_command_lists(label: str, manifest: dict, *, require_command_provenance: bool) -> list[list[str]]:
    commands = manifest.get("commands")
    if commands is None:
        if require_command_provenance:
            raise CaptureManifestError(f"{label} artifact manifest is missing capture commands")
        return []
    if not isinstance(commands, list) or not commands:
        raise CaptureManifestError(f"{label} artifact manifest has invalid commands field: expected non-empty list")
    parsed: list[list[str]] = []
    for index, command in enumerate(commands):
        if not isinstance(command, list) or not command or not all(isinstance(part, str) for part in command):
            raise CaptureManifestError(f"{label} artifact manifest has invalid command entry {index}")
        parsed.append(command)
    return parsed


def command_has(command: list[str], flag: str) -> bool:
    return flag in command


def require_command_with_flag(label: str, commands: list[list[str]], flag: str) -> list[str]:
    for command in commands:
        if command_has(command, flag):
            return command
    raise CaptureManifestError(f"{label} artifact manifest commands do not prove {flag} ran")


def reject_command_with_flag(label: str, commands: list[list[str]], flag: str) -> None:
    if any(command_has(command, flag) for command in commands):
        raise CaptureManifestError(f"{label} artifact manifest commands unexpectedly include {flag}")


def require_flag_before(label: str, command: list[str], earlier: str, later: str) -> None:
    if not command_has(command, earlier):
        raise CaptureManifestError(f"{label} artifact manifest render command does not prove {earlier} ran")
    if not command_has(command, later):
        raise CaptureManifestError(f"{label} artifact manifest render command does not prove {later} ran")
    if command.index(earlier) > command.index(later):
        raise CaptureManifestError(f"{label} artifact manifest command records {earlier} after {later}")


def validate_capture_command_provenance(
    label: str,
    manifest: dict,
    *,
    preview_probes_only: bool,
    map_render_only: bool,
    metal_preview_probes_only: bool,
    metal_renderer_probes_only: bool,
    map_after_save_reload: bool,
    map_after_mpq_export_reload: bool,
    require_smoke_picking: bool,
    require_command_provenance: bool,
) -> None:
    commands = manifest_command_lists(label, manifest, require_command_provenance=require_command_provenance)
    if not commands or not require_command_provenance:
        return
    if preview_probes_only:
        require_command_with_flag(label, commands, "--smoke-shared-preview-probes")
        return
    if metal_preview_probes_only:
        require_command_with_flag(label, commands, "--smoke-metal-preview-probes")
        return
    if metal_renderer_probes_only:
        require_command_with_flag(label, commands, "--smoke-metal-renderer-probes")
        return

    render_command = require_command_with_flag(label, commands, "--smoke-render-modes")
    if require_smoke_picking or bool(manifest.get("smoke_picking", False)):
        if not command_has(render_command, "--smoke-picking"):
            raise CaptureManifestError(f"{label} artifact manifest render command does not prove --smoke-picking ran")
    if map_after_save_reload:
        require_flag_before(label, render_command, "--smoke-save-reload", "--smoke-render-modes")
    else:
        reject_command_with_flag(label, [render_command], "--smoke-save-reload")
    if map_after_mpq_export_reload:
        require_flag_before(label, render_command, "--smoke-mpq-export-reload", "--smoke-render-modes")
    else:
        reject_command_with_flag(label, [render_command], "--smoke-mpq-export-reload")

    if map_render_only:
        reject_command_with_flag(label, commands, "--smoke-model-previews")
    else:
        require_command_with_flag(label, commands, "--smoke-model-previews")


def validate_capture_manifest(
    label: str,
    artifact_dir: Path,
    *,
    expected_backend: str | None = None,
    require_backend_identity: bool = False,
    same_app_determinism: bool = False,
    preview_probes_only: bool,
    map_render_only: bool,
    metal_preview_probes_only: bool,
    metal_renderer_probes_only: bool,
    map_after_save_reload: bool,
    map_after_mpq_export_reload: bool,
    require_manifest: bool = False,
    expected_source_head: str | None = None,
    require_clean_source: bool = False,
    require_smoke_picking: bool = False,
    require_artifact_fingerprints: bool = False,
    require_command_provenance: bool = False,
) -> dict | None:
    manifest = read_capture_manifest(artifact_dir)
    if manifest is None:
        if require_manifest or expected_source_head is not None or require_clean_source or require_backend_identity:
            raise CaptureManifestError(f"{label} artifact directory is missing {CAPTURE_MANIFEST_NAME}: {artifact_dir}")
        return None

    reported_backend = manifest.get("reported_backend")
    if reported_backend is not None and reported_backend not in VALID_RENDERER_BACKENDS:
        raise CaptureManifestError(
            f"{label} artifact manifest records invalid renderer backend {reported_backend!r}: "
            f"{artifact_dir / CAPTURE_MANIFEST_NAME}"
        )
    if require_backend_identity and not isinstance(reported_backend, str):
        raise CaptureManifestError(
            f"{label} artifact manifest does not record the renderer backend: "
            f"{artifact_dir / CAPTURE_MANIFEST_NAME}"
        )
    if expected_backend is not None and not same_app_determinism and reported_backend is not None and reported_backend != expected_backend:
        raise CaptureManifestError(
            f"{label} artifact manifest was captured with renderer backend {reported_backend!r}, "
            f"expected {expected_backend!r}: {artifact_dir / CAPTURE_MANIFEST_NAME}"
        )

    actual_preview_only = bool(manifest.get("preview_probes_only", False))
    if actual_preview_only != preview_probes_only:
        raise CaptureManifestError(
            f"{label} artifact manifest was captured with preview_probes_only={actual_preview_only}, "
            f"but this comparison requested preview_probes_only={preview_probes_only}: {artifact_dir / CAPTURE_MANIFEST_NAME}"
        )

    actual_metal_preview_probes_only = bool(manifest.get("metal_preview_probes_only", False))
    if actual_metal_preview_probes_only != metal_preview_probes_only:
        raise CaptureManifestError(
            f"{label} artifact manifest was captured with metal_preview_probes_only={actual_metal_preview_probes_only}, "
            f"but this comparison requested metal_preview_probes_only={metal_preview_probes_only}: "
            f"{artifact_dir / CAPTURE_MANIFEST_NAME}"
        )

    actual_renderer_probes_only = bool(manifest.get("metal_renderer_probes_only", False))
    if actual_renderer_probes_only != metal_renderer_probes_only:
        raise CaptureManifestError(
            f"{label} artifact manifest was captured with metal_renderer_probes_only={actual_renderer_probes_only}, "
            f"but this comparison requested metal_renderer_probes_only={metal_renderer_probes_only}: "
            f"{artifact_dir / CAPTURE_MANIFEST_NAME}"
        )

    if not preview_probes_only and not metal_preview_probes_only and not metal_renderer_probes_only:
        actual_map_render_only = bool(manifest.get("map_render_only", False))
        if actual_map_render_only != map_render_only:
            raise CaptureManifestError(
                f"{label} artifact manifest was captured with map_render_only={actual_map_render_only}, "
                f"but this comparison requested map_render_only={map_render_only}: "
                f"{artifact_dir / CAPTURE_MANIFEST_NAME}"
            )
        actual_save_reload = bool(manifest.get("map_after_save_reload", False))
        if actual_save_reload != map_after_save_reload:
            raise CaptureManifestError(
                f"{label} artifact manifest was captured with map_after_save_reload={actual_save_reload}, "
                f"but this comparison requested map_after_save_reload={map_after_save_reload}: "
                f"{artifact_dir / CAPTURE_MANIFEST_NAME}"
            )
        actual_mpq_export_reload = bool(manifest.get("map_after_mpq_export_reload", False))
        if actual_mpq_export_reload != map_after_mpq_export_reload:
            raise CaptureManifestError(
                f"{label} artifact manifest was captured with map_after_mpq_export_reload={actual_mpq_export_reload}, "
                f"but this comparison requested map_after_mpq_export_reload={map_after_mpq_export_reload}: "
                f"{artifact_dir / CAPTURE_MANIFEST_NAME}"
            )
        if require_smoke_picking and not bool(manifest.get("smoke_picking", False)):
            raise CaptureManifestError(
                f"{label} artifact manifest does not prove --smoke-picking ran for this map capture: "
                f"{artifact_dir / CAPTURE_MANIFEST_NAME}"
            )
    validate_capture_command_provenance(
        label,
        manifest,
        preview_probes_only=preview_probes_only,
        map_render_only=map_render_only,
        metal_preview_probes_only=metal_preview_probes_only,
        metal_renderer_probes_only=metal_renderer_probes_only,
        map_after_save_reload=map_after_save_reload,
        map_after_mpq_export_reload=map_after_mpq_export_reload,
        require_smoke_picking=require_smoke_picking,
        require_command_provenance=require_command_provenance,
    )
    validate_capture_artifact_fingerprints(
        label,
        artifact_dir,
        manifest,
        require_artifact_fingerprints=require_artifact_fingerprints,
        required_artifact_names=expected_capture_artifact_names(
            preview_probes_only=preview_probes_only,
            map_render_only=map_render_only,
            metal_preview_probes_only=metal_preview_probes_only,
            metal_renderer_probes_only=metal_renderer_probes_only,
        ),
    )
    if expected_source_head is not None:
        source = manifest.get("source_revision")
        actual_head = source.get("head") if isinstance(source, dict) else None
        if not isinstance(actual_head, str):
            raise CaptureManifestError(
                f"{label} artifact manifest does not record a source revision head: "
                f"{artifact_dir / CAPTURE_MANIFEST_NAME}"
            )
        if actual_head != expected_source_head:
            raise CaptureManifestError(
                f"{label} artifact manifest was captured from source {actual_head}, "
                f"expected {expected_source_head}: {artifact_dir / CAPTURE_MANIFEST_NAME}"
            )
    if require_clean_source:
        source = manifest.get("source_revision")
        dirty = source.get("dirty") if isinstance(source, dict) else None
        if not isinstance(dirty, bool):
            raise CaptureManifestError(
                f"{label} artifact manifest does not record source dirty state: "
                f"{artifact_dir / CAPTURE_MANIFEST_NAME}"
            )
        if dirty:
            status = source.get("status_short") if isinstance(source, dict) else None
            status_lines = status if isinstance(status, list) else []
            suffix = ""
            if status_lines:
                suffix = " Dirty files: " + "; ".join(str(line) for line in status_lines[:5])
            raise CaptureManifestError(
                f"{label} artifact manifest was captured from a dirty source tree: "
                f"{artifact_dir / CAPTURE_MANIFEST_NAME}.{suffix}"
            )
    return manifest


def capture_manifest_source_head(label: str, artifact_dir: Path, manifest: dict | None) -> str:
    if manifest is None:
        raise CaptureManifestError(f"{label} artifact directory is missing {CAPTURE_MANIFEST_NAME}: {artifact_dir}")
    source = manifest.get("source_revision")
    head = source.get("head") if isinstance(source, dict) else None
    if not isinstance(head, str):
        raise CaptureManifestError(
            f"{label} artifact manifest does not record a source revision head: "
            f"{artifact_dir / CAPTURE_MANIFEST_NAME}"
        )
    return head


def validate_matching_capture_source_heads(
    opengl_manifest: dict | None,
    metal_manifest: dict | None,
    opengl_artifact_dir: Path,
    metal_artifact_dir: Path,
) -> None:
    opengl_head = capture_manifest_source_head("OpenGL", opengl_artifact_dir, opengl_manifest)
    metal_head = capture_manifest_source_head("Metal", metal_artifact_dir, metal_manifest)
    if opengl_head != metal_head:
        raise CaptureManifestError(
            "OpenGL and Metal artifact manifests were captured from different source revisions: "
            f"OpenGL {opengl_head}, Metal {metal_head}"
        )


def capture_manifest_backend(label: str, artifact_dir: Path, manifest: dict | None) -> str:
    if manifest is None:
        raise CaptureManifestError(f"{label} artifact directory is missing {CAPTURE_MANIFEST_NAME}: {artifact_dir}")
    backend = manifest.get("reported_backend")
    if not isinstance(backend, str):
        raise CaptureManifestError(
            f"{label} artifact manifest does not record the renderer backend: "
            f"{artifact_dir / CAPTURE_MANIFEST_NAME}"
        )
    if backend not in VALID_RENDERER_BACKENDS:
        raise CaptureManifestError(
            f"{label} artifact manifest records invalid renderer backend {backend!r}: "
            f"{artifact_dir / CAPTURE_MANIFEST_NAME}"
        )
    return backend


def validate_capture_backends(
    opengl_manifest: dict | None,
    metal_manifest: dict | None,
    opengl_artifact_dir: Path,
    metal_artifact_dir: Path,
    *,
    same_app_determinism: bool,
) -> None:
    opengl_backend = capture_manifest_backend("OpenGL", opengl_artifact_dir, opengl_manifest)
    metal_backend = capture_manifest_backend("Metal", metal_artifact_dir, metal_manifest)
    if same_app_determinism:
        if opengl_backend != metal_backend:
            raise CaptureManifestError(
                "Same-app determinism comparison captured different renderer backends: "
                f"reference {opengl_backend!r}, candidate {metal_backend!r}"
            )
        return

    if opengl_backend != "opengl" or metal_backend != "metal":
        raise CaptureManifestError(
            "Renderer parity requires an OpenGL reference and Metal candidate. "
            f"Captured backends were reference {opengl_backend!r}, candidate {metal_backend!r}. "
            "Use --same-app-determinism only for intentional same-backend smoke gates."
        )


def capture_mode_requires_smoke_picking(
    *,
    preview_probes_only: bool,
    metal_preview_probes_only: bool,
    metal_renderer_probes_only: bool,
) -> bool:
    return not (preview_probes_only or metal_preview_probes_only or metal_renderer_probes_only)


def map_capture_mode_label(*, map_render_only: bool, map_after_save_reload: bool, map_after_mpq_export_reload: bool) -> str:
    if map_after_save_reload and map_after_mpq_export_reload:
        prefix = "saved/reloaded + MPQ-exported/reloaded"
    elif map_after_mpq_export_reload:
        prefix = "MPQ-exported/reloaded"
    elif map_after_save_reload:
        prefix = "saved/reloaded"
    else:
        prefix = "initial"
    return f"{prefix} map render + picking only" if map_render_only else f"{prefix} map + picking + previews"


def capture_manifest_summary(
    manifest: dict | None,
    *,
    preview_probes_only: bool,
    map_render_only: bool,
    metal_preview_probes_only: bool,
    metal_renderer_probes_only: bool,
    map_after_save_reload: bool,
    map_after_mpq_export_reload: bool,
) -> str:
    if manifest is None:
        return "not found"
    if preview_probes_only:
        mode = "preview probes only"
    elif metal_preview_probes_only:
        mode = "Metal preview probes only"
    elif metal_renderer_probes_only:
        mode = "Metal renderer probes only"
    else:
        mode = map_capture_mode_label(
            map_render_only=map_render_only,
            map_after_save_reload=map_after_save_reload,
            map_after_mpq_export_reload=map_after_mpq_export_reload,
        )
    commands = manifest.get("commands", [])
    command_count = len(commands) if isinstance(commands, list) else 0
    artifacts = manifest.get("artifacts", [])
    artifact_count = len(artifacts) if isinstance(artifacts, list) else 0
    app = manifest.get("app", "<unknown app>")
    backend = manifest.get("reported_backend")
    backend_summary = f", backend `{backend}`" if isinstance(backend, str) else ""
    source = manifest.get("source_revision")
    source_summary = ""
    if isinstance(source, dict) and isinstance(source.get("head"), str):
        source_summary = f", source `{source['head'][:12]}`"
    return f"{mode}, {command_count} command(s), {artifact_count} PNG artifact(s){source_summary}{backend_summary}, app `{app}`"


def capture_backend(
    *,
    label: str,
    expected_backend: str | None,
    app: Path,
    artifact_dir: Path,
    map_path: Path,
    warcraft_dir: Path | None,
    capture_delay_ms: int,
    timeout: float,
    preview_probes_only: bool,
    map_render_only: bool,
    metal_preview_probes_only: bool,
    metal_renderer_probes_only: bool,
    map_after_save_reload: bool,
    map_after_mpq_export_reload: bool,
) -> None:
    executable = resolve_executable(app)
    reported_backend = query_renderer_backend(executable, min(timeout, 10.0))
    if expected_backend is not None and reported_backend != expected_backend:
        raise CaptureManifestError(
            f"{label} app reported renderer backend {reported_backend!r}, expected {expected_backend!r}: {app}"
        )
    artifact_dir.mkdir(parents=True, exist_ok=True)

    if preview_probes_only:
        preview_probes = [
            str(executable),
            "--smoke-shared-preview-probes",
            "--smoke-artifact-dir",
            str(artifact_dir),
        ]
        run_command(preview_probes, timeout)
        write_capture_manifest(
            label=label,
            expected_backend=expected_backend,
            reported_backend=reported_backend,
            app=app,
            executable=executable,
            artifact_dir=artifact_dir,
            map_path=map_path,
            warcraft_dir=warcraft_dir,
            capture_delay_ms=capture_delay_ms,
            preview_probes_only=preview_probes_only,
            map_render_only=False,
            metal_preview_probes_only=False,
            metal_renderer_probes_only=False,
            map_after_save_reload=map_after_save_reload,
            map_after_mpq_export_reload=map_after_mpq_export_reload,
            commands=[preview_probes],
        )
        print(f"Captured {label} shared preview probe artifacts in {artifact_dir}.")
        return

    if metal_preview_probes_only:
        metal_preview_probes = [
            str(executable),
            "--smoke-metal-preview-probes",
            "--smoke-artifact-dir",
            str(artifact_dir),
        ]
        run_command(metal_preview_probes, timeout)
        write_capture_manifest(
            label=label,
            expected_backend=expected_backend,
            reported_backend=reported_backend,
            app=app,
            executable=executable,
            artifact_dir=artifact_dir,
            map_path=map_path,
            warcraft_dir=warcraft_dir,
            capture_delay_ms=capture_delay_ms,
            preview_probes_only=False,
            map_render_only=False,
            metal_preview_probes_only=True,
            metal_renderer_probes_only=False,
            map_after_save_reload=False,
            map_after_mpq_export_reload=False,
            commands=[metal_preview_probes],
        )
        print(f"Captured {label} preview probe artifacts in {artifact_dir}.")
        return

    if metal_renderer_probes_only:
        renderer_probes = [
            str(executable),
            "--smoke-metal-renderer-probes",
            "--smoke-artifact-dir",
            str(artifact_dir),
        ]
        run_command(renderer_probes, timeout)
        write_capture_manifest(
            label=label,
            expected_backend=expected_backend,
            reported_backend=reported_backend,
            app=app,
            executable=executable,
            artifact_dir=artifact_dir,
            map_path=map_path,
            warcraft_dir=warcraft_dir,
            capture_delay_ms=capture_delay_ms,
            preview_probes_only=False,
            map_render_only=False,
            metal_preview_probes_only=False,
            metal_renderer_probes_only=True,
            map_after_save_reload=False,
            map_after_mpq_export_reload=False,
            commands=[renderer_probes],
        )
        print(f"Captured {label} renderer probe artifacts in {artifact_dir}.")
        return

    common = [str(executable), "--map", str(map_path)]
    if warcraft_dir is not None:
        common.extend(["--warcraft-dir", str(warcraft_dir)])

    pre_render_smokes = []
    if map_after_save_reload:
        pre_render_smokes.append("--smoke-save-reload")
    if map_after_mpq_export_reload:
        pre_render_smokes.append("--smoke-mpq-export-reload")

    render_modes = [
        *common,
        *pre_render_smokes,
        "--smoke-render-modes",
        "--smoke-picking",
        "--smoke-artifact-dir",
        str(artifact_dir),
        "--capture-delay-ms",
        str(capture_delay_ms),
    ]
    run_command(render_modes, timeout)
    commands = [render_modes]

    if not map_render_only:
        model_previews = [
            *common,
            "--smoke-model-previews",
            "--smoke-artifact-dir",
            str(artifact_dir),
            "--capture-delay-ms",
            str(capture_delay_ms),
        ]
        run_command(model_previews, timeout)
        commands.append(model_previews)

    write_capture_manifest(
        label=label,
        expected_backend=expected_backend,
        reported_backend=reported_backend,
        app=app,
        executable=executable,
        artifact_dir=artifact_dir,
        map_path=map_path,
        warcraft_dir=warcraft_dir,
        capture_delay_ms=capture_delay_ms,
        preview_probes_only=preview_probes_only,
        map_render_only=map_render_only,
        metal_preview_probes_only=False,
        metal_renderer_probes_only=False,
        map_after_save_reload=map_after_save_reload,
        map_after_mpq_export_reload=map_after_mpq_export_reload,
        commands=commands,
    )
    mode = "map-render artifacts" if map_render_only else "artifacts"
    print(f"Captured {label} {mode} in {artifact_dir}.")


def validate_backend_artifacts(
    label: str,
    artifact_dir: Path,
    *,
    preview_probes_only: bool = False,
    map_render_only: bool = False,
    metal_preview_probes_only: bool = False,
    metal_renderer_probes_only: bool = False,
) -> None:
    if not artifact_dir.is_dir():
        raise FileNotFoundError(f"{label} artifact directory not found: {artifact_dir}")
    if preview_probes_only:
        validate_png_artifact_manifest(
            artifact_dir,
            SYNTHETIC_SHARED_PREVIEW_ARTIFACT_SIZES,
            label=f"{label} shared preview probe parity artifacts",
            allow_extra=True,
        )
        return

    if metal_preview_probes_only:
        validate_png_artifact_manifest(
            artifact_dir,
            METAL_PREVIEW_PROBE_ARTIFACT_SIZES,
            label=f"{label} Metal preview probe parity artifacts",
            allow_extra=True,
        )
        return

    if metal_renderer_probes_only:
        validate_png_artifact_manifest(
            artifact_dir,
            METAL_RENDERER_PROBE_ARTIFACT_SIZES,
            label=f"{label} Metal renderer probe parity artifacts",
            allow_extra=True,
        )
        return

    baseline = artifact_dir / "map-render-baseline.png"
    variable_size = read_png_size(baseline)
    validate_png_artifact_manifest(
        artifact_dir,
        MAP_RENDER_ARTIFACT_SIZES,
        variable_size=variable_size,
        label=f"{label} map-render parity artifacts",
        allow_extra=True,
    )
    if map_render_only:
        return
    validate_png_artifact_manifest(
        artifact_dir,
        SHARED_PREVIEW_ARTIFACT_SIZES,
        label=f"{label} shared-preview parity artifacts",
        allow_extra=True,
    )


def write_markdown_report(
    path: Path,
    *,
    opengl_app: Path,
    metal_app: Path,
    map_path: Path,
    opengl_artifact_dir: Path,
    metal_artifact_dir: Path,
    diff_dir: Path,
    results: list[dict],
    failed: bool,
    max_rms: float | None,
    max_mean_abs: float | None,
    max_changed_percent: float | None,
    preview_probes_only: bool = False,
    map_render_only: bool = False,
    metal_preview_probes_only: bool = False,
    metal_renderer_probes_only: bool = False,
    map_after_save_reload: bool = False,
    map_after_mpq_export_reload: bool = False,
    opengl_capture_manifest: dict | None = None,
    metal_capture_manifest: dict | None = None,
    pr_gate: bool = False,
) -> None:
    def threshold_label(value: float | None) -> str:
        return "not set" if value is None else str(value)

    ok_count = sum(1 for result in results if result["status"] == "ok")
    worst_changed = max((result.get("changed_percent", 0.0) for result in results), default=0.0)
    worst_mean = max((result.get("mean_abs_channel_delta", 0.0) for result in results), default=0.0)
    worst_rms = max((result.get("rms_channel_delta", 0.0) for result in results), default=0.0)
    map_source = "not used"
    if metal_preview_probes_only:
        map_source = "not used (Metal preview probes only)"
    elif metal_renderer_probes_only:
        map_source = "not used (Metal renderer probes only)"
    elif not preview_probes_only:
        map_source = map_capture_mode_label(
            map_render_only=map_render_only,
            map_after_save_reload=map_after_save_reload,
            map_after_mpq_export_reload=map_after_mpq_export_reload,
        )

    lines = [
        "# HiveWE Renderer Parity Report",
        "",
        f"- Reference app: `{opengl_app}`",
        f"- Candidate app: `{metal_app}`",
        f"- Map: `{'not used' if preview_probes_only or metal_preview_probes_only or metal_renderer_probes_only else str(map_path)}`",
        f"- Reference artifacts: `{opengl_artifact_dir}`",
        f"- Candidate artifacts: `{metal_artifact_dir}`",
        f"- Diff artifacts: `{diff_dir}`",
        f"- Result: {'FAIL' if failed else 'PASS'}",
        f"- PR gate: {'enabled' if pr_gate else 'not enabled'}",
        f"- Map render source: `{map_source}`",
        f"- Reference capture: {capture_manifest_summary(opengl_capture_manifest, preview_probes_only=preview_probes_only, map_render_only=map_render_only, metal_preview_probes_only=metal_preview_probes_only, metal_renderer_probes_only=metal_renderer_probes_only, map_after_save_reload=map_after_save_reload, map_after_mpq_export_reload=map_after_mpq_export_reload)}",
        f"- Candidate capture: {capture_manifest_summary(metal_capture_manifest, preview_probes_only=preview_probes_only, map_render_only=map_render_only, metal_preview_probes_only=metal_preview_probes_only, metal_renderer_probes_only=metal_renderer_probes_only, map_after_save_reload=map_after_save_reload, map_after_mpq_export_reload=map_after_mpq_export_reload)}",
        (
            f"- Thresholds: RMS `{threshold_label(max_rms)}`, mean absolute `{threshold_label(max_mean_abs)}`, "
            f"changed pixels `{threshold_label(max_changed_percent)}`"
        ),
        "- Note: without thresholds, PASS means the required artifacts were present, valid, non-flat, and comparable; review the metrics and diff PNGs manually.",
        f"- Compared: {len(results)} artifacts ({ok_count} ok, {len(results) - ok_count} failed)",
        f"- Worst changed pixels: {worst_changed:.3f}%",
        f"- Worst mean channel delta: {worst_mean:.3f}",
        f"- Worst RMS channel delta: {worst_rms:.3f}",
    ]
    if pr_gate:
        lines.extend(
            [
                "",
                "## PR Gate Review Checklist",
                "",
                "- Reference capture must report backend `opengl`; candidate capture must report backend `metal`.",
                "- Both captures must come from the same clean source revision recorded in `renderer-parity-capture.json`.",
                "- Map parity captures must include `--smoke-picking` and use the saved/reloaded map state.",
                "- Review nonzero diffs for terrain, cliffs, water, pathing, brush overlays, and selection overlays.",
                "- Review SD/HD model materials, team colors, alpha rejection, transparency sorting, and particles including PE1 model spawns.",
                "- Review model editor and object-grid preview artifacts before treating the report as PR-ready evidence.",
            ]
        )
    lines.extend(
        [
        "",
        "| Artifact | Status | Changed % | Mean | RMS | Max | Diff |",
        "| --- | --- | ---: | ---: | ---: | ---: | --- |",
        ]
    )
    for result in results:
        lines.append(
            "| "
            + " | ".join(
                [
                    str(result["name"]).replace("|", "\\|"),
                    str(result["status"]).replace("|", "\\|"),
                    f"{result.get('changed_percent', 0.0):.3f}",
                    f"{result.get('mean_abs_channel_delta', 0.0):.3f}",
                    f"{result.get('rms_channel_delta', 0.0):.3f}",
                    str(result.get("max_channel_delta", 0)),
                    str(result.get("diff_path", "")).replace("|", "\\|"),
                ]
            )
            + " |"
        )

    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text("\n".join(lines) + "\n", encoding="utf-8")


def prepare_report_output_dir(report_dir: Path) -> None:
    report_dir.mkdir(parents=True, exist_ok=True)
    for name in (
        "renderer-parity.json",
        "renderer-parity.md",
        "renderer-parity-provenance.json",
    ):
        path = report_dir / name
        if path.exists():
            path.unlink()

    diff_path = report_dir / "diffs"
    if diff_path.exists():
        if diff_path.is_dir():
            shutil.rmtree(diff_path)
        else:
            diff_path.unlink()


def parse_args() -> argparse.Namespace:
    default_root = Path(tempfile.gettempdir()) / "hivewe-renderer-parity"
    parser = argparse.ArgumentParser(
        description=(
            "Capture OpenGL and Metal smoke artifacts with matching filenames, validate them, "
            "and write a visual parity report."
        )
    )
    parser.add_argument("--opengl-app", type=Path, help="Reference OpenGL HiveWE executable or .app bundle.")
    parser.add_argument("--metal-app", type=Path, help="Candidate Metal HiveWE executable or .app bundle.")
    parser.add_argument("--map", type=Path, default=Path("data/test map/"), help="Map directory to load.")
    parser.add_argument(
        "--warcraft-dir",
        type=Path,
        default=None,
        help="Warcraft III installation directory to pass to both apps, when needed. Defaults to HIVEWE_WARCRAFT_DIR when set.",
    )
    parser.add_argument(
        "--opengl-artifact-dir",
        type=Path,
        default=default_root / "opengl-artifacts",
        help="Reference artifact directory.",
    )
    parser.add_argument(
        "--opengl-artifact-archive",
        type=Path,
        help="Reference artifact zip produced by tools/package_renderer_capture.py; requires --skip-capture.",
    )
    parser.add_argument(
        "--metal-artifact-dir",
        type=Path,
        default=default_root / "metal-artifacts",
        help="Candidate artifact directory.",
    )
    parser.add_argument(
        "--metal-artifact-archive",
        type=Path,
        help="Candidate artifact zip produced by tools/package_renderer_capture.py; requires --skip-capture.",
    )
    parser.add_argument(
        "--report-dir",
        type=Path,
        default=default_root / "report",
        help="Directory for parity JSON, Markdown, and diff PNGs.",
    )
    parser.add_argument("--capture-delay-ms", type=int, default=1000, help="Delay before render smoke captures.")
    parser.add_argument("--timeout", type=float, default=60.0, help="Maximum seconds for each HiveWE smoke command.")
    parser.add_argument("--skip-capture", action="store_true", help="Only validate and compare existing artifact directories.")
    parser.add_argument(
        "--capture-only",
        choices=("opengl", "metal"),
        default=None,
        help="Capture and validate one backend artifact directory, then exit without comparing.",
    )
    parser.add_argument("--keep-existing-artifacts", action="store_true", help="Do not clear artifact directories before capture.")
    parser.add_argument(
        "--require-capture-manifest",
        action="store_true",
        help="Fail if either artifact directory lacks renderer-parity-capture.json.",
    )
    parser.add_argument(
        "--require-source-head",
        metavar="SHA",
        default=None,
        help="Fail if either capture manifest was not produced from this exact git commit SHA.",
    )
    parser.add_argument(
        "--require-current-source",
        action="store_true",
        help="Fail if either capture manifest was not produced from the current git HEAD.",
    )
    parser.add_argument(
        "--require-clean-source",
        action="store_true",
        help="Fail if either capture manifest was produced from a dirty git working tree.",
    )
    parser.add_argument(
        "--require-matching-source",
        action="store_true",
        help="Fail if OpenGL and Metal capture manifests were not produced from the same git commit SHA.",
    )
    parser.add_argument(
        "--same-app-determinism",
        action="store_true",
        help=(
            "Allow both artifact sets to report the same renderer backend. Use only for same-app determinism smoke gates; "
            "real OpenGL-vs-Metal parity rejects same-backend captures."
        ),
    )
    parser.add_argument(
        "--pr-gate",
        action="store_true",
        help=(
            "Apply maintainer-facing parity defaults: require capture manifests, current clean source, "
            "matching OpenGL/Metal source revisions when comparing, and post-save/reload map captures for map modes "
            "unless --map-after-mpq-export-reload is explicitly requested."
        ),
    )
    parser.add_argument(
        "--preview-probes-only",
        action="store_true",
        help="Capture and compare only Warcraft-free shared synthetic preview probe artifacts.",
    )
    parser.add_argument(
        "--map-render-only",
        action="store_true",
        help="Capture and compare only map-render artifacts while still validating picking, skipping model editor/object-grid previews.",
    )
    parser.add_argument(
        "--metal-renderer-probes-only",
        action="store_true",
        help="Capture and compare only Metal renderer probe artifacts.",
    )
    parser.add_argument(
        "--metal-preview-probes-only",
        action="store_true",
        help="Capture and compare only Metal-specific preview probe artifacts.",
    )
    parser.add_argument(
        "--map-after-save-reload",
        action="store_true",
        help="Run --smoke-save-reload before capturing map-render parity artifacts.",
    )
    parser.add_argument(
        "--map-after-mpq-export-reload",
        action="store_true",
        help="Run --smoke-mpq-export-reload before capturing map-render parity artifacts.",
    )
    parser.add_argument("--max-rms", type=float, default=None, help="Fail if any image RMS channel delta exceeds this value.")
    parser.add_argument("--max-mean-abs", type=float, default=None, help="Fail if any image mean absolute channel delta exceeds this value.")
    parser.add_argument("--max-changed-percent", type=float, default=None, help="Fail if any changed-pixel percentage exceeds this value.")
    parser.add_argument("--diff-scale", type=float, default=4.0, help="Multiplier for diff PNG channel deltas.")
    parser.add_argument("--self-test", action="store_true", help="Run dependency-free renderer parity self-tests and exit.")
    return parser.parse_args()


def apply_pr_gate_defaults(args: argparse.Namespace) -> None:
    if not args.pr_gate:
        return

    args.require_capture_manifest = True
    args.require_clean_source = True
    if args.require_source_head is None:
        args.require_current_source = True
    if args.capture_only is None:
        args.require_matching_source = True
    if not (
        args.preview_probes_only
        or args.metal_preview_probes_only
        or args.metal_renderer_probes_only
        or args.map_after_mpq_export_reload
    ):
        args.map_after_save_reload = True


def expected_backend_for_label(label: str, *, same_app_determinism: bool) -> str | None:
    if same_app_determinism:
        return None
    normalized = label.lower()
    return normalized if normalized in VALID_RENDERER_BACKENDS else None


def self_test_pixels(width: int, height: int, seed: int) -> bytes:
    pixels = bytearray()
    for index in range(width * height):
        pixels.extend(((seed + index) & 0xFF, (seed * 3 + index * 5) & 0xFF, (seed * 7 + index * 11) & 0xFF, 255))
    return bytes(pixels)


def write_self_test_artifacts(left_dir: Path, right_dir: Path) -> None:
    left_dir.mkdir(parents=True)
    right_dir.mkdir(parents=True)
    for index, name in enumerate(MAP_RENDER_ARTIFACT_SIZES):
        encode_png_rgba(left_dir / name, 24, 16, self_test_pixels(24, 16, 10 + index))
        encode_png_rgba(right_dir / name, 24, 16, self_test_pixels(24, 16, 12 + index))
    for index, (name, size) in enumerate(SHARED_PREVIEW_ARTIFACT_SIZES.items()):
        width, height = size
        encode_png_rgba(left_dir / name, width, height, self_test_pixels(width, height, 80 + index))
        encode_png_rgba(right_dir / name, width, height, self_test_pixels(width, height, 82 + index))
    for index, (name, size) in enumerate(METAL_RENDERER_PROBE_ARTIFACT_SIZES.items()):
        width, height = size
        encode_png_rgba(left_dir / name, width, height, self_test_pixels(width, height, 120 + index))
        encode_png_rgba(right_dir / name, width, height, self_test_pixels(width, height, 122 + index))
    for index, (name, size) in enumerate(METAL_PREVIEW_PROBE_ARTIFACT_SIZES.items()):
        width, height = size
        encode_png_rgba(left_dir / name, width, height, self_test_pixels(width, height, 150 + index))
        encode_png_rgba(right_dir / name, width, height, self_test_pixels(width, height, 152 + index))
    encode_png_rgba(right_dir / "metal-extra-probe.png", 8, 8, self_test_pixels(8, 8, 120))


def require_self_test(condition: bool, message: str) -> None:
    if not condition:
        raise AssertionError(message)


def run_self_test() -> int:
    previous_warcraft_dir = os.environ.get("HIVEWE_WARCRAFT_DIR")
    os.environ["HIVEWE_WARCRAFT_DIR"] = "/tmp/hivewe-warcraft-env"
    try:
        require_self_test(
            resolved_warcraft_dir(None) == Path("/tmp/hivewe-warcraft-env"),
            "renderer parity should default to HIVEWE_WARCRAFT_DIR",
        )
        require_self_test(
            resolved_warcraft_dir(Path("/tmp/hivewe-warcraft-cli")) == Path("/tmp/hivewe-warcraft-cli"),
            "explicit --warcraft-dir should override HIVEWE_WARCRAFT_DIR",
        )
    finally:
        if previous_warcraft_dir is None:
            os.environ.pop("HIVEWE_WARCRAFT_DIR", None)
        else:
            os.environ["HIVEWE_WARCRAFT_DIR"] = previous_warcraft_dir

    def pr_gate_namespace(**overrides) -> argparse.Namespace:
        defaults = {
            "pr_gate": True,
            "require_capture_manifest": False,
            "require_clean_source": False,
            "require_source_head": None,
            "require_current_source": False,
            "require_matching_source": False,
            "same_app_determinism": False,
            "capture_only": None,
            "preview_probes_only": False,
            "map_render_only": False,
            "metal_preview_probes_only": False,
            "metal_renderer_probes_only": False,
            "map_after_save_reload": False,
            "map_after_mpq_export_reload": False,
        }
        defaults.update(overrides)
        return argparse.Namespace(**defaults)

    pr_compare_args = pr_gate_namespace()
    apply_pr_gate_defaults(pr_compare_args)
    require_self_test(pr_compare_args.require_capture_manifest, "PR gate should require capture manifests")
    require_self_test(pr_compare_args.require_clean_source, "PR gate should require a clean source tree")
    require_self_test(pr_compare_args.require_current_source, "PR gate should default to the current source revision")
    require_self_test(pr_compare_args.require_matching_source, "PR gate should require matching backend source revisions when comparing")
    require_self_test(pr_compare_args.map_after_save_reload, "PR gate should capture map parity after save/reload")

    pr_capture_args = pr_gate_namespace(capture_only="metal")
    apply_pr_gate_defaults(pr_capture_args)
    require_self_test(pr_capture_args.require_capture_manifest, "capture-only PR gate should still require capture manifests")
    require_self_test(pr_capture_args.require_clean_source, "capture-only PR gate should require a clean source tree")
    require_self_test(pr_capture_args.require_current_source, "capture-only PR gate should default to the current source revision")
    require_self_test(not pr_capture_args.require_matching_source, "capture-only PR gate should not require paired source revisions")
    require_self_test(pr_capture_args.map_after_save_reload, "capture-only PR gate should capture map parity after save/reload")

    pr_probe_args = pr_gate_namespace(preview_probes_only=True)
    apply_pr_gate_defaults(pr_probe_args)
    require_self_test(not pr_probe_args.map_after_save_reload, "PR gate should not add save/reload to map-free probe captures")

    pr_pinned_args = pr_gate_namespace(require_source_head="1" * 40)
    apply_pr_gate_defaults(pr_pinned_args)
    require_self_test(not pr_pinned_args.require_current_source, "PR gate should not override an explicit source revision")

    pr_mpq_args = pr_gate_namespace(map_render_only=True, map_after_mpq_export_reload=True)
    apply_pr_gate_defaults(pr_mpq_args)
    require_self_test(pr_mpq_args.map_after_mpq_export_reload, "PR gate should preserve explicit MPQ export/reload mode")
    require_self_test(
        not pr_mpq_args.map_after_save_reload,
        "PR gate should not add folder save/reload when MPQ export/reload mode is explicit",
    )

    with tempfile.TemporaryDirectory(prefix="hivewe-renderer-parity-") as temp_dir:
        root = Path(temp_dir)
        opengl_dir = root / "opengl"
        metal_dir = root / "metal"
        report_dir = root / "report"
        write_self_test_artifacts(opengl_dir, metal_dir)

        stale_report_file = report_dir / "renderer-parity.json"
        stale_diff_file = report_dir / "diffs" / "stale.diff.png"
        preserved_extracted_dir = report_dir / "extracted-opengl-artifacts"
        preserved_extracted_dir.mkdir(parents=True)
        (preserved_extracted_dir / "keep.txt").write_text("keep\n", encoding="utf-8")
        stale_report_file.write_text("stale\n", encoding="utf-8")
        stale_diff_file.parent.mkdir(parents=True, exist_ok=True)
        stale_diff_file.write_text("stale\n", encoding="utf-8")
        prepare_report_output_dir(report_dir)
        require_self_test(not stale_report_file.exists(), "report preparation should remove stale JSON output")
        require_self_test(not stale_diff_file.exists(), "report preparation should remove stale diff PNGs")
        require_self_test(
            (preserved_extracted_dir / "keep.txt").is_file(),
            "report preparation should preserve extracted capture archives",
        )

        fake_app = root / "fake_hivewe.py"
        fake_app.write_text(
            "#!/usr/bin/env python3\n"
            "import json\n"
            "import sys\n"
            "from pathlib import Path\n"
            "if '--print-render-backend' in sys.argv[1:]:\n"
            "    print('metal')\n"
            "    raise SystemExit(0)\n"
            "with Path(__file__).with_suffix('.jsonl').open('a', encoding='utf-8') as output:\n"
            "    output.write(json.dumps(sys.argv[1:]) + '\\n')\n",
            encoding="utf-8",
        )
        fake_app.chmod(0o755)
        capture_backend(
            label="fake",
            expected_backend=None,
            app=fake_app,
            artifact_dir=root / "fake-artifacts",
            map_path=Path("data/test map"),
            warcraft_dir=None,
            capture_delay_ms=123,
            timeout=10.0,
            preview_probes_only=False,
            map_render_only=False,
            metal_preview_probes_only=False,
            metal_renderer_probes_only=False,
            map_after_save_reload=True,
            map_after_mpq_export_reload=False,
        )
        fake_commands = [
            json.loads(line)
            for line in fake_app.with_suffix(".jsonl").read_text(encoding="utf-8").splitlines()
        ]
        fake_manifest = read_capture_manifest(root / "fake-artifacts")
        require_self_test(fake_manifest is not None, "capture should write a provenance manifest")
        require_self_test(fake_manifest["map_after_save_reload"], "capture manifest should record save/reload mode")
        require_self_test(fake_manifest["smoke_picking"], "capture manifest should record picking smoke coverage")
        require_self_test(fake_manifest["reported_backend"] == "metal", "capture manifest should record the reported renderer backend")
        require_self_test(len(fake_manifest["commands"]) == 2, "capture manifest should record both capture commands")
        require_self_test(isinstance(fake_manifest.get("artifacts"), list), "capture manifest should record artifact fingerprints")
        require_self_test(fake_manifest.get("executable_sha256") == sha256_file(fake_app), "capture manifest should fingerprint the executable")
        current_source = source_revision_metadata()
        if current_source is not None:
            require_self_test(
                fake_manifest.get("source_revision", {}).get("head") == current_source["head"],
                "capture manifest should record the source revision when git metadata is available",
            )
            validate_capture_manifest(
                "fake",
                root / "fake-artifacts",
                require_backend_identity=True,
                same_app_determinism=True,
                preview_probes_only=False,
                map_render_only=False,
                metal_preview_probes_only=False,
                metal_renderer_probes_only=False,
                map_after_save_reload=True,
                map_after_mpq_export_reload=False,
                expected_source_head=current_source["head"],
                require_smoke_picking=True,
            )
        fake_capture_zip = root / "fake-capture.zip"
        with zipfile.ZipFile(fake_capture_zip, "w", compression=zipfile.ZIP_DEFLATED) as archive:
            for path in sorted((root / "fake-artifacts").iterdir()):
                if path.is_file():
                    archive.write(path, f"fake-prefix/{path.name}")
        extracted_fake_dir = extract_capture_archive("fake", fake_capture_zip, root / "extracted-fake")
        require_self_test(
            (extracted_fake_dir / CAPTURE_MANIFEST_NAME).is_file(),
            "capture archive extraction should return the artifact directory containing the manifest",
        )
        require_self_test(
            read_capture_manifest(extracted_fake_dir)["reported_backend"] == "metal",
            "capture archive extraction should preserve the capture manifest",
        )

        unsafe_capture_zip = root / "unsafe-capture.zip"
        with zipfile.ZipFile(unsafe_capture_zip, "w", compression=zipfile.ZIP_DEFLATED) as archive:
            archive.writestr(f"unsafe-prefix/{CAPTURE_MANIFEST_NAME}", "{}\n")
            archive.writestr("../escape.png", "not safe")
            archive.writestr("unsafe-prefix\\windows-escape.png", "not safe")
        unsafe_archive_failed = False
        try:
            extract_capture_archive("unsafe", unsafe_capture_zip, root / "extracted-unsafe")
        except CaptureManifestError:
            unsafe_archive_failed = True
        require_self_test(unsafe_archive_failed, "capture archive extraction should reject unsafe paths")

        outside_prefix_zip = root / "outside-prefix-capture.zip"
        with zipfile.ZipFile(outside_prefix_zip, "w", compression=zipfile.ZIP_DEFLATED) as archive:
            archive.writestr(f"prefix/{CAPTURE_MANIFEST_NAME}", "{}\n")
            archive.writestr("other/file.png", "outside")
        outside_prefix_failed = False
        try:
            extract_capture_archive("outside-prefix", outside_prefix_zip, root / "extracted-outside-prefix")
        except CaptureManifestError:
            outside_prefix_failed = True
        require_self_test(outside_prefix_failed, "capture archive extraction should reject files outside the manifest prefix")
        backend_mismatch_failed = False
        try:
            validate_capture_manifest(
                "fake",
                root / "fake-artifacts",
                expected_backend="opengl",
                require_backend_identity=True,
                preview_probes_only=False,
                map_render_only=False,
                metal_preview_probes_only=False,
                metal_renderer_probes_only=False,
                map_after_save_reload=True,
                map_after_mpq_export_reload=False,
            )
        except CaptureManifestError:
            backend_mismatch_failed = True
        require_self_test(backend_mismatch_failed, "manifest validation should reject mismatched renderer backends")
        require_self_test(len(fake_commands) == 2, "capture should run map render and model preview commands")
        require_self_test("--smoke-save-reload" in fake_commands[0], "map capture should include save/reload when requested")
        require_self_test("--smoke-render-modes" in fake_commands[0], "first capture command should render map modes")
        require_self_test("--smoke-picking" in fake_commands[0], "first capture command should validate picking")
        require_self_test(
            fake_commands[0].index("--smoke-save-reload") < fake_commands[0].index("--smoke-render-modes"),
            "save/reload should run before map render modes",
        )
        require_self_test("--smoke-save-reload" not in fake_commands[1], "model preview capture should not save/reload")
        require_self_test("--smoke-model-previews" in fake_commands[1], "second capture command should render previews")

        fake_map_only_app = root / "fake_map_only_hivewe.py"
        fake_map_only_app.write_text(fake_app.read_text(encoding="utf-8"), encoding="utf-8")
        fake_map_only_app.chmod(0o755)
        capture_backend(
            label="fake-map-render-only",
            expected_backend=None,
            app=fake_map_only_app,
            artifact_dir=root / "fake-map-only-artifacts",
            map_path=Path("data/test map"),
            warcraft_dir=None,
            capture_delay_ms=123,
            timeout=10.0,
            preview_probes_only=False,
            map_render_only=True,
            metal_preview_probes_only=False,
            metal_renderer_probes_only=False,
            map_after_save_reload=True,
            map_after_mpq_export_reload=False,
        )
        fake_map_only_commands = [
            json.loads(line)
            for line in fake_map_only_app.with_suffix(".jsonl").read_text(encoding="utf-8").splitlines()
        ]
        fake_map_only_manifest = read_capture_manifest(root / "fake-map-only-artifacts")
        require_self_test(fake_map_only_manifest is not None, "map-render-only capture should write a provenance manifest")
        require_self_test(fake_map_only_manifest["map_render_only"], "capture manifest should record map-render-only mode")
        require_self_test(fake_map_only_manifest["map_after_save_reload"], "map-render-only manifest should record save/reload mode")
        require_self_test(fake_map_only_manifest["smoke_picking"], "map-render-only manifest should record picking smoke coverage")
        require_self_test(len(fake_map_only_manifest["commands"]) == 1, "map-render-only manifest should record one capture command")
        require_self_test(len(fake_map_only_commands) == 1, "map-render-only capture should skip model preview commands")
        require_self_test("--smoke-render-modes" in fake_map_only_commands[0], "map-render-only capture should render map modes")
        require_self_test("--smoke-picking" in fake_map_only_commands[0], "map-render-only capture should validate picking")
        require_self_test("--smoke-save-reload" in fake_map_only_commands[0], "map-render-only capture should include save/reload when requested")
        require_self_test(
            "--smoke-model-previews" not in fake_map_only_commands[0],
            "map-render-only capture should not render previews",
        )

        fake_mpq_map_only_app = root / "fake_mpq_map_only_hivewe.py"
        fake_mpq_map_only_app.write_text(fake_app.read_text(encoding="utf-8"), encoding="utf-8")
        fake_mpq_map_only_app.chmod(0o755)
        capture_backend(
            label="fake-mpq-map-render-only",
            expected_backend=None,
            app=fake_mpq_map_only_app,
            artifact_dir=root / "fake-mpq-map-only-artifacts",
            map_path=Path("data/test map"),
            warcraft_dir=None,
            capture_delay_ms=123,
            timeout=10.0,
            preview_probes_only=False,
            map_render_only=True,
            metal_preview_probes_only=False,
            metal_renderer_probes_only=False,
            map_after_save_reload=False,
            map_after_mpq_export_reload=True,
        )
        fake_mpq_map_only_commands = [
            json.loads(line)
            for line in fake_mpq_map_only_app.with_suffix(".jsonl").read_text(encoding="utf-8").splitlines()
        ]
        fake_mpq_map_only_manifest = read_capture_manifest(root / "fake-mpq-map-only-artifacts")
        require_self_test(fake_mpq_map_only_manifest is not None, "MPQ map-render-only capture should write a provenance manifest")
        require_self_test(
            fake_mpq_map_only_manifest["map_after_mpq_export_reload"],
            "MPQ map-render-only manifest should record MPQ export/reload mode",
        )
        require_self_test(len(fake_mpq_map_only_commands) == 1, "MPQ map-render-only capture should skip model preview commands")
        require_self_test(
            "--smoke-mpq-export-reload" in fake_mpq_map_only_commands[0],
            "MPQ map-render-only capture should include MPQ export/reload when requested",
        )
        require_self_test(
            "--smoke-save-reload" not in fake_mpq_map_only_commands[0],
            "MPQ map-render-only capture should not imply folder save/reload",
        )
        require_self_test(
            fake_mpq_map_only_commands[0].index("--smoke-mpq-export-reload")
            < fake_mpq_map_only_commands[0].index("--smoke-render-modes"),
            "MPQ export/reload should run before map render modes",
        )

        fake_renderer_probe_app = root / "fake_renderer_probe_hivewe.py"
        fake_renderer_probe_app.write_text(fake_app.read_text(encoding="utf-8"), encoding="utf-8")
        fake_renderer_probe_app.chmod(0o755)
        capture_backend(
            label="fake-renderer-probes-only",
            expected_backend=None,
            app=fake_renderer_probe_app,
            artifact_dir=root / "fake-renderer-probe-artifacts",
            map_path=Path("data/test map"),
            warcraft_dir=None,
            capture_delay_ms=123,
            timeout=10.0,
            preview_probes_only=False,
            map_render_only=False,
            metal_preview_probes_only=False,
            metal_renderer_probes_only=True,
            map_after_save_reload=False,
            map_after_mpq_export_reload=False,
        )
        fake_renderer_probe_commands = [
            json.loads(line)
            for line in fake_renderer_probe_app.with_suffix(".jsonl").read_text(encoding="utf-8").splitlines()
        ]
        fake_renderer_probe_manifest = read_capture_manifest(root / "fake-renderer-probe-artifacts")
        require_self_test(fake_renderer_probe_manifest is not None, "renderer-probes-only capture should write a provenance manifest")
        require_self_test(
            fake_renderer_probe_manifest["metal_renderer_probes_only"],
            "capture manifest should record renderer-probes-only mode",
        )
        require_self_test(
            fake_renderer_probe_manifest["map"] is None,
            "renderer-probes-only manifest should not record a map path",
        )
        require_self_test(len(fake_renderer_probe_manifest["commands"]) == 1, "renderer-probes-only manifest should record one capture command")
        require_self_test(len(fake_renderer_probe_commands) == 1, "renderer-probes-only capture should run one command")
        require_self_test(
            "--smoke-metal-renderer-probes" in fake_renderer_probe_commands[0],
            "renderer-probes-only capture should run Metal renderer probes",
        )
        require_self_test("--map" not in fake_renderer_probe_commands[0], "renderer-probes-only capture should not load a map")

        fake_metal_preview_probe_app = root / "fake_metal_preview_probe_hivewe.py"
        fake_metal_preview_probe_app.write_text(fake_app.read_text(encoding="utf-8"), encoding="utf-8")
        fake_metal_preview_probe_app.chmod(0o755)
        capture_backend(
            label="fake-metal-preview-probes-only",
            expected_backend=None,
            app=fake_metal_preview_probe_app,
            artifact_dir=root / "fake-metal-preview-probe-artifacts",
            map_path=Path("data/test map"),
            warcraft_dir=None,
            capture_delay_ms=123,
            timeout=10.0,
            preview_probes_only=False,
            map_render_only=False,
            metal_preview_probes_only=True,
            metal_renderer_probes_only=False,
            map_after_save_reload=False,
            map_after_mpq_export_reload=False,
        )
        fake_metal_preview_probe_commands = [
            json.loads(line)
            for line in fake_metal_preview_probe_app.with_suffix(".jsonl").read_text(encoding="utf-8").splitlines()
        ]
        fake_metal_preview_probe_manifest = read_capture_manifest(root / "fake-metal-preview-probe-artifacts")
        require_self_test(fake_metal_preview_probe_manifest is not None, "metal-preview-probes-only capture should write a provenance manifest")
        require_self_test(
            fake_metal_preview_probe_manifest["metal_preview_probes_only"],
            "capture manifest should record metal-preview-probes-only mode",
        )
        require_self_test(
            fake_metal_preview_probe_manifest["map"] is None,
            "metal-preview-probes-only manifest should not record a map path",
        )
        require_self_test(
            len(fake_metal_preview_probe_manifest["commands"]) == 1,
            "metal-preview-probes-only manifest should record one capture command",
        )
        require_self_test(len(fake_metal_preview_probe_commands) == 1, "metal-preview-probes-only capture should run one command")
        require_self_test(
            "--smoke-metal-preview-probes" in fake_metal_preview_probe_commands[0],
            "metal-preview-probes-only capture should run Metal preview probes",
        )
        require_self_test("--map" not in fake_metal_preview_probe_commands[0], "metal-preview-probes-only capture should not load a map")

        validate_backend_artifacts("OpenGL", opengl_dir)
        validate_backend_artifacts("Metal", metal_dir)
        validate_backend_artifacts("OpenGL map only", opengl_dir, map_render_only=True)
        validate_backend_artifacts("Metal map only", metal_dir, map_render_only=True)
        validate_backend_artifacts("OpenGL preview probes", opengl_dir, preview_probes_only=True)
        validate_backend_artifacts("Metal preview probes", metal_dir, preview_probes_only=True)
        validate_backend_artifacts("OpenGL Metal preview probes", opengl_dir, metal_preview_probes_only=True)
        validate_backend_artifacts("Metal Metal preview probes", metal_dir, metal_preview_probes_only=True)
        validate_backend_artifacts("OpenGL renderer probes", opengl_dir, metal_renderer_probes_only=True)
        validate_backend_artifacts("Metal renderer probes", metal_dir, metal_renderer_probes_only=True)

        exact_names = set(ARTIFACT_PRESETS["map-render"]) | set(ARTIFACT_PRESETS["shared-previews"])
        results, failed = compare_artifact_directories(
            opengl_dir,
            metal_dir,
            patterns=[],
            exact_names=exact_names,
            common_only=False,
            diff_dir=report_dir / "diffs",
            diff_scale=4.0,
            max_rms=None,
            max_mean_abs=None,
            max_changed_percent=None,
        )
        require_self_test(not failed, "comparison without thresholds should pass for comparable artifacts")
        require_self_test(len(results) == len(exact_names), "comparison should cover all shared parity artifacts")
        require_self_test(
            len(list((report_dir / "diffs").glob("*.diff.png"))) == len(exact_names),
            "comparison should write one diff PNG per shared parity artifact",
        )

        preview_probe_names = set(ARTIFACT_PRESETS["shared-preview-probes"])
        preview_probe_results, preview_probe_failed = compare_artifact_directories(
            opengl_dir,
            metal_dir,
            patterns=[],
            exact_names=preview_probe_names,
            common_only=False,
            diff_dir=None,
            diff_scale=4.0,
            max_rms=None,
            max_mean_abs=None,
            max_changed_percent=None,
        )
        require_self_test(not preview_probe_failed, "preview probe comparison without thresholds should pass")
        require_self_test(
            len(preview_probe_results) == len(preview_probe_names),
            "preview probe comparison should cover all synthetic shared preview artifacts",
        )

        metal_preview_probe_names = set(ARTIFACT_PRESETS["metal-preview-probes"])
        metal_preview_probe_results, metal_preview_probe_failed = compare_artifact_directories(
            opengl_dir,
            metal_dir,
            patterns=[],
            exact_names=metal_preview_probe_names,
            common_only=False,
            diff_dir=None,
            diff_scale=4.0,
            max_rms=None,
            max_mean_abs=None,
            max_changed_percent=None,
        )
        require_self_test(not metal_preview_probe_failed, "Metal preview probe comparison without thresholds should pass")
        require_self_test(
            len(metal_preview_probe_results) == len(metal_preview_probe_names),
            "Metal preview probe comparison should cover all Metal preview probe artifacts",
        )

        renderer_probe_names = set(ARTIFACT_PRESETS["metal-renderer-probes"])
        renderer_probe_results, renderer_probe_failed = compare_artifact_directories(
            opengl_dir,
            metal_dir,
            patterns=[],
            exact_names=renderer_probe_names,
            common_only=False,
            diff_dir=None,
            diff_scale=4.0,
            max_rms=None,
            max_mean_abs=None,
            max_changed_percent=None,
        )
        require_self_test(not renderer_probe_failed, "renderer probe comparison without thresholds should pass")
        require_self_test(
            len(renderer_probe_results) == len(renderer_probe_names),
            "renderer probe comparison should cover all Metal renderer probe artifacts",
        )

        threshold_results, threshold_failed = compare_artifact_directories(
            opengl_dir,
            metal_dir,
            patterns=[],
            exact_names=exact_names,
            common_only=False,
            diff_dir=None,
            diff_scale=4.0,
            max_rms=0.0,
            max_mean_abs=None,
            max_changed_percent=None,
        )
        require_self_test(threshold_failed, "strict RMS threshold should fail changed parity artifacts")
        require_self_test(
            all(result["status"] == "threshold-failed" for result in threshold_results),
            "threshold failures should be reported per artifact",
        )

        write_markdown_report(
            report_dir / "renderer-parity.md",
            opengl_app=Path("<self-test-opengl>"),
            metal_app=Path("<self-test-metal>"),
            map_path=Path("data/test map"),
            opengl_artifact_dir=opengl_dir,
            metal_artifact_dir=metal_dir,
            diff_dir=report_dir / "diffs",
            results=results,
            failed=failed,
            max_rms=None,
            max_mean_abs=None,
            max_changed_percent=None,
            preview_probes_only=False,
            map_render_only=False,
            metal_preview_probes_only=False,
            metal_renderer_probes_only=False,
            map_after_save_reload=True,
            map_after_mpq_export_reload=False,
            opengl_capture_manifest={
                "app": "<self-test-opengl>",
                "commands": [["opengl"]],
                "source_revision": {"head": "0123456789abcdef"},
            },
            metal_capture_manifest={
                "app": "<self-test-metal>",
                "commands": [["metal"]],
                "source_revision": {"head": "fedcba9876543210"},
            },
            pr_gate=True,
        )
        markdown = (report_dir / "renderer-parity.md").read_text(encoding="utf-8")
        require_self_test("without thresholds" in markdown, "report should clarify threshold-free PASS meaning")
        require_self_test("saved/reloaded map" in markdown, "report should record save/reload-backed map capture mode")
        require_self_test("Reference capture" in markdown, "report should include reference capture provenance")
        require_self_test("0123456789ab" in markdown, "report should include source revision summaries")
        require_self_test("PR Gate Review Checklist" in markdown, "PR gate report should include maintainer review checklist")
        require_self_test("PE1 model spawns" in markdown, "PR gate checklist should call out particle model-spawn review")

        write_markdown_report(
            report_dir / "renderer-parity-mpq.md",
            opengl_app=Path("<self-test-opengl>"),
            metal_app=Path("<self-test-metal>"),
            map_path=Path("data/test map"),
            opengl_artifact_dir=opengl_dir,
            metal_artifact_dir=metal_dir,
            diff_dir=report_dir / "diffs",
            results=results,
            failed=failed,
            max_rms=None,
            max_mean_abs=None,
            max_changed_percent=None,
            preview_probes_only=False,
            map_render_only=True,
            metal_preview_probes_only=False,
            metal_renderer_probes_only=False,
            map_after_save_reload=False,
            map_after_mpq_export_reload=True,
            opengl_capture_manifest={
                "app": "<self-test-opengl>",
                "commands": [["opengl"]],
                "source_revision": {"head": "0123456789abcdef"},
            },
            metal_capture_manifest={
                "app": "<self-test-metal>",
                "commands": [["metal"]],
                "source_revision": {"head": "fedcba9876543210"},
            },
        )
        mpq_markdown = (report_dir / "renderer-parity-mpq.md").read_text(encoding="utf-8")
        require_self_test("MPQ-exported/reloaded map render" in mpq_markdown, "report should record MPQ-backed map capture mode")

        mismatch_dir = root / "mismatch"
        mismatch_dir.mkdir()
        (mismatch_dir / CAPTURE_MANIFEST_NAME).write_text(
            json.dumps({"preview_probes_only": False, "map_after_save_reload": False}) + "\n",
            encoding="utf-8",
        )
        mismatch_failed = False
        try:
            validate_capture_manifest(
                "mismatch",
                mismatch_dir,
                preview_probes_only=False,
                map_render_only=False,
                metal_preview_probes_only=False,
                metal_renderer_probes_only=False,
                map_after_save_reload=True,
                map_after_mpq_export_reload=False,
            )
        except CaptureManifestError:
            mismatch_failed = True
        require_self_test(mismatch_failed, "manifest validation should reject mismatched save/reload mode")

        mpq_mismatch_dir = root / "mpq-mismatch"
        mpq_mismatch_dir.mkdir()
        (mpq_mismatch_dir / CAPTURE_MANIFEST_NAME).write_text(
            json.dumps({"preview_probes_only": False, "map_after_mpq_export_reload": False}) + "\n",
            encoding="utf-8",
        )
        mpq_mismatch_failed = False
        try:
            validate_capture_manifest(
                "mpq-mismatch",
                mpq_mismatch_dir,
                preview_probes_only=False,
                map_render_only=False,
                metal_preview_probes_only=False,
                metal_renderer_probes_only=False,
                map_after_save_reload=False,
                map_after_mpq_export_reload=True,
            )
        except CaptureManifestError:
            mpq_mismatch_failed = True
        require_self_test(mpq_mismatch_failed, "manifest validation should reject mismatched MPQ export/reload mode")

        map_only_mismatch_dir = root / "map-only-mismatch"
        map_only_mismatch_dir.mkdir()
        (map_only_mismatch_dir / CAPTURE_MANIFEST_NAME).write_text(
            json.dumps({"preview_probes_only": False, "map_render_only": False, "map_after_save_reload": True}) + "\n",
            encoding="utf-8",
        )
        map_only_mismatch_failed = False
        try:
            validate_capture_manifest(
                "map-only-mismatch",
                map_only_mismatch_dir,
                preview_probes_only=False,
                map_render_only=True,
                metal_preview_probes_only=False,
                metal_renderer_probes_only=False,
                map_after_save_reload=True,
                map_after_mpq_export_reload=False,
            )
        except CaptureManifestError:
            map_only_mismatch_failed = True
        require_self_test(map_only_mismatch_failed, "manifest validation should reject mismatched map-render-only mode")

        missing_picking_dir = root / "missing-picking"
        missing_picking_dir.mkdir()
        (missing_picking_dir / CAPTURE_MANIFEST_NAME).write_text(
            json.dumps(
                {
                    "preview_probes_only": False,
                    "map_render_only": False,
                    "map_after_save_reload": True,
                    "smoke_picking": False,
                }
            )
            + "\n",
            encoding="utf-8",
        )
        missing_picking_failed = False
        try:
            validate_capture_manifest(
                "missing-picking",
                missing_picking_dir,
                preview_probes_only=False,
                map_render_only=False,
                metal_preview_probes_only=False,
                metal_renderer_probes_only=False,
                map_after_save_reload=True,
                map_after_mpq_export_reload=False,
                require_smoke_picking=True,
            )
        except CaptureManifestError:
            missing_picking_failed = True
        require_self_test(missing_picking_failed, "manifest validation should require picking coverage for PR map captures")

        missing_command_dir = root / "missing-command-provenance"
        missing_command_dir.mkdir()
        (missing_command_dir / CAPTURE_MANIFEST_NAME).write_text(
            json.dumps(
                {
                    "preview_probes_only": False,
                    "map_render_only": False,
                    "map_after_save_reload": True,
                    "map_after_mpq_export_reload": False,
                    "smoke_picking": True,
                }
            )
            + "\n",
            encoding="utf-8",
        )
        missing_command_failed = False
        try:
            validate_capture_manifest(
                "missing-command-provenance",
                missing_command_dir,
                preview_probes_only=False,
                map_render_only=False,
                metal_preview_probes_only=False,
                metal_renderer_probes_only=False,
                map_after_save_reload=True,
                map_after_mpq_export_reload=False,
                require_smoke_picking=True,
                require_command_provenance=True,
            )
        except CaptureManifestError:
            missing_command_failed = True
        require_self_test(missing_command_failed, "PR manifest validation should require command provenance")

        wrong_command_dir = root / "wrong-command-provenance"
        wrong_command_dir.mkdir()
        (wrong_command_dir / CAPTURE_MANIFEST_NAME).write_text(
            json.dumps(
                {
                    "preview_probes_only": False,
                    "map_render_only": False,
                    "map_after_save_reload": True,
                    "map_after_mpq_export_reload": False,
                    "smoke_picking": True,
                    "commands": [["fake", "--smoke-render-modes", "--smoke-picking"], ["fake", "--smoke-model-previews"]],
                }
            )
            + "\n",
            encoding="utf-8",
        )
        wrong_command_failed = False
        try:
            validate_capture_manifest(
                "wrong-command-provenance",
                wrong_command_dir,
                preview_probes_only=False,
                map_render_only=False,
                metal_preview_probes_only=False,
                metal_renderer_probes_only=False,
                map_after_save_reload=True,
                map_after_mpq_export_reload=False,
                require_smoke_picking=True,
                require_command_provenance=True,
            )
        except CaptureManifestError:
            wrong_command_failed = True
        require_self_test(wrong_command_failed, "PR manifest validation should reject commands that do not prove the requested mode")

        renderer_probe_mismatch_dir = root / "renderer-probe-mismatch"
        renderer_probe_mismatch_dir.mkdir()
        (renderer_probe_mismatch_dir / CAPTURE_MANIFEST_NAME).write_text(
            json.dumps({"preview_probes_only": False, "metal_renderer_probes_only": False, "map_after_save_reload": False}) + "\n",
            encoding="utf-8",
        )
        renderer_probe_mismatch_failed = False
        try:
            validate_capture_manifest(
                "renderer-probe-mismatch",
                renderer_probe_mismatch_dir,
                preview_probes_only=False,
                map_render_only=False,
                metal_preview_probes_only=False,
                metal_renderer_probes_only=True,
                map_after_save_reload=False,
                map_after_mpq_export_reload=False,
            )
        except CaptureManifestError:
            renderer_probe_mismatch_failed = True
        require_self_test(
            renderer_probe_mismatch_failed,
            "manifest validation should reject mismatched renderer-probes-only mode",
        )

        metal_preview_probe_mismatch_dir = root / "metal-preview-probe-mismatch"
        metal_preview_probe_mismatch_dir.mkdir()
        (metal_preview_probe_mismatch_dir / CAPTURE_MANIFEST_NAME).write_text(
            json.dumps({"preview_probes_only": False, "metal_preview_probes_only": False, "map_after_save_reload": False}) + "\n",
            encoding="utf-8",
        )
        metal_preview_probe_mismatch_failed = False
        try:
            validate_capture_manifest(
                "metal-preview-probe-mismatch",
                metal_preview_probe_mismatch_dir,
                preview_probes_only=False,
                map_render_only=False,
                metal_preview_probes_only=True,
                metal_renderer_probes_only=False,
                map_after_save_reload=False,
                map_after_mpq_export_reload=False,
            )
        except CaptureManifestError:
            metal_preview_probe_mismatch_failed = True
        require_self_test(
            metal_preview_probe_mismatch_failed,
            "manifest validation should reject mismatched metal-preview-probes-only mode",
        )

        missing_manifest_failed = False
        try:
            validate_capture_manifest(
                "missing",
                root,
                preview_probes_only=False,
                map_render_only=False,
                metal_preview_probes_only=False,
                metal_renderer_probes_only=False,
                map_after_save_reload=False,
                map_after_mpq_export_reload=False,
                require_manifest=True,
            )
        except CaptureManifestError:
            missing_manifest_failed = True
        require_self_test(missing_manifest_failed, "manifest validation should optionally require provenance")

        missing_source_manifest_failed = False
        try:
            validate_capture_manifest(
                "missing-source",
                root,
                preview_probes_only=False,
                map_render_only=False,
                metal_preview_probes_only=False,
                metal_renderer_probes_only=False,
                map_after_save_reload=False,
                map_after_mpq_export_reload=False,
                expected_source_head="1" * 40,
            )
        except CaptureManifestError:
            missing_source_manifest_failed = True
        require_self_test(missing_source_manifest_failed, "source validation should require provenance")

        stale_source_dir = root / "stale-source"
        stale_source_dir.mkdir()
        (stale_source_dir / CAPTURE_MANIFEST_NAME).write_text(
            json.dumps(
                {
                    "preview_probes_only": True,
                    "map_after_save_reload": False,
                    "source_revision": {"head": "0" * 40},
                }
            )
            + "\n",
            encoding="utf-8",
        )
        stale_source_failed = False
        try:
            validate_capture_manifest(
                "stale-source",
                stale_source_dir,
                preview_probes_only=True,
                map_render_only=False,
                metal_preview_probes_only=False,
                metal_renderer_probes_only=False,
                map_after_save_reload=False,
                map_after_mpq_export_reload=False,
                expected_source_head="1" * 40,
            )
        except CaptureManifestError:
            stale_source_failed = True
        require_self_test(stale_source_failed, "manifest validation should reject stale source revisions when requested")

        dirty_source_dir = root / "dirty-source"
        dirty_source_dir.mkdir()
        (dirty_source_dir / CAPTURE_MANIFEST_NAME).write_text(
            json.dumps(
                {
                    "preview_probes_only": True,
                    "metal_preview_probes_only": False,
                    "metal_renderer_probes_only": False,
                    "source_revision": {
                        "head": "2" * 40,
                        "dirty": True,
                        "status_short": [" M src/rendering/example.cpp"],
                    },
                }
            )
            + "\n",
            encoding="utf-8",
        )
        dirty_source_failed = False
        try:
            validate_capture_manifest(
                "dirty-source",
                dirty_source_dir,
                preview_probes_only=True,
                map_render_only=False,
                metal_preview_probes_only=False,
                metal_renderer_probes_only=False,
                map_after_save_reload=False,
                map_after_mpq_export_reload=False,
                require_clean_source=True,
            )
        except CaptureManifestError:
            dirty_source_failed = True
        require_self_test(dirty_source_failed, "manifest validation should reject dirty source revisions when requested")

        clean_source_dir = root / "clean-source"
        clean_source_dir.mkdir()
        (clean_source_dir / CAPTURE_MANIFEST_NAME).write_text(
            json.dumps(
                {
                    "preview_probes_only": True,
                    "metal_preview_probes_only": False,
                    "metal_renderer_probes_only": False,
                    "source_revision": {
                        "head": "3" * 40,
                        "dirty": False,
                        "status_short": [],
                    },
                }
            )
            + "\n",
            encoding="utf-8",
        )
        validate_capture_manifest(
            "clean-source",
            clean_source_dir,
            preview_probes_only=True,
            map_render_only=False,
            metal_preview_probes_only=False,
            metal_renderer_probes_only=False,
            map_after_save_reload=False,
            map_after_mpq_export_reload=False,
            require_clean_source=True,
        )

        matching_source_opengl_dir = root / "matching-source-opengl"
        matching_source_metal_dir = root / "matching-source-metal"
        matching_source_opengl_dir.mkdir()
        matching_source_metal_dir.mkdir()
        for artifact_dir in (matching_source_opengl_dir, matching_source_metal_dir):
            (artifact_dir / CAPTURE_MANIFEST_NAME).write_text(
                json.dumps({"source_revision": {"head": "4" * 40}}) + "\n",
                encoding="utf-8",
            )
        validate_matching_capture_source_heads(
            read_capture_manifest(matching_source_opengl_dir),
            read_capture_manifest(matching_source_metal_dir),
            matching_source_opengl_dir,
            matching_source_metal_dir,
        )

        mismatched_source_opengl_dir = root / "mismatched-source-opengl"
        mismatched_source_metal_dir = root / "mismatched-source-metal"
        mismatched_source_opengl_dir.mkdir()
        mismatched_source_metal_dir.mkdir()
        (mismatched_source_opengl_dir / CAPTURE_MANIFEST_NAME).write_text(
            json.dumps({"source_revision": {"head": "5" * 40}}) + "\n",
            encoding="utf-8",
        )
        (mismatched_source_metal_dir / CAPTURE_MANIFEST_NAME).write_text(
            json.dumps({"source_revision": {"head": "6" * 40}}) + "\n",
            encoding="utf-8",
        )
        mismatched_source_failed = False
        try:
            validate_matching_capture_source_heads(
                read_capture_manifest(mismatched_source_opengl_dir),
                read_capture_manifest(mismatched_source_metal_dir),
                mismatched_source_opengl_dir,
                mismatched_source_metal_dir,
            )
        except CaptureManifestError:
            mismatched_source_failed = True
        require_self_test(mismatched_source_failed, "manifest validation should reject mismatched source revisions when requested")

        backend_opengl_dir = root / "backend-opengl"
        backend_metal_dir = root / "backend-metal"
        backend_opengl_dir.mkdir()
        backend_metal_dir.mkdir()
        (backend_opengl_dir / CAPTURE_MANIFEST_NAME).write_text(
            json.dumps({"reported_backend": "opengl"}) + "\n",
            encoding="utf-8",
        )
        (backend_metal_dir / CAPTURE_MANIFEST_NAME).write_text(
            json.dumps({"reported_backend": "metal"}) + "\n",
            encoding="utf-8",
        )
        validate_capture_backends(
            read_capture_manifest(backend_opengl_dir),
            read_capture_manifest(backend_metal_dir),
            backend_opengl_dir,
            backend_metal_dir,
            same_app_determinism=False,
        )

        same_backend_left_dir = root / "same-backend-left"
        same_backend_right_dir = root / "same-backend-right"
        same_backend_left_dir.mkdir()
        same_backend_right_dir.mkdir()
        for artifact_dir in (same_backend_left_dir, same_backend_right_dir):
            (artifact_dir / CAPTURE_MANIFEST_NAME).write_text(
                json.dumps({"reported_backend": "metal"}) + "\n",
                encoding="utf-8",
            )
        same_backend_parity_failed = False
        try:
            validate_capture_backends(
                read_capture_manifest(same_backend_left_dir),
                read_capture_manifest(same_backend_right_dir),
                same_backend_left_dir,
                same_backend_right_dir,
                same_app_determinism=False,
            )
        except CaptureManifestError:
            same_backend_parity_failed = True
        require_self_test(same_backend_parity_failed, "real parity validation should reject same-backend captures")
        validate_capture_backends(
            read_capture_manifest(same_backend_left_dir),
            read_capture_manifest(same_backend_right_dir),
            same_backend_left_dir,
            same_backend_right_dir,
            same_app_determinism=True,
        )

        fingerprint_dir = root / "fingerprints"
        fingerprint_dir.mkdir()
        encode_png_rgba(fingerprint_dir / "artifact.png", 2, 2, self_test_pixels(2, 2, 5))
        write_capture_manifest(
            label="fingerprints",
            expected_backend=None,
            reported_backend="metal",
            app=fake_app,
            executable=fake_app,
            artifact_dir=fingerprint_dir,
            map_path=Path("data/test map"),
            warcraft_dir=None,
            capture_delay_ms=123,
            preview_probes_only=True,
            map_render_only=False,
            metal_preview_probes_only=False,
            metal_renderer_probes_only=False,
            map_after_save_reload=False,
            map_after_mpq_export_reload=False,
            commands=[["fake"]],
        )
        validate_capture_manifest(
            "fingerprints",
            fingerprint_dir,
            preview_probes_only=True,
            map_render_only=False,
            metal_preview_probes_only=False,
            metal_renderer_probes_only=False,
            map_after_save_reload=False,
            map_after_mpq_export_reload=False,
            require_manifest=True,
        )
        missing_required_fingerprint_dir = root / "missing-required-fingerprints"
        missing_required_fingerprint_dir.mkdir()
        encode_png_rgba(
            missing_required_fingerprint_dir / "model-editor-material-preview.png",
            512,
            512,
            self_test_pixels(512, 512, 11),
        )
        write_capture_manifest(
            label="missing-required-fingerprints",
            expected_backend=None,
            reported_backend="metal",
            app=fake_app,
            executable=fake_app,
            artifact_dir=missing_required_fingerprint_dir,
            map_path=Path("data/test map"),
            warcraft_dir=None,
            capture_delay_ms=123,
            preview_probes_only=True,
            map_render_only=False,
            metal_preview_probes_only=False,
            metal_renderer_probes_only=False,
            map_after_save_reload=False,
            map_after_mpq_export_reload=False,
            commands=[["fake"]],
        )
        missing_required_fingerprint_failed = False
        try:
            validate_capture_manifest(
                "missing-required-fingerprints",
                missing_required_fingerprint_dir,
                preview_probes_only=True,
                map_render_only=False,
                metal_preview_probes_only=False,
                metal_renderer_probes_only=False,
                map_after_save_reload=False,
                map_after_mpq_export_reload=False,
                require_artifact_fingerprints=True,
            )
        except CaptureManifestError:
            missing_required_fingerprint_failed = True
        require_self_test(
            missing_required_fingerprint_failed,
            "PR manifest validation should require every required PNG fingerprint",
        )
        legacy_fingerprint_dir = root / "legacy-fingerprints"
        legacy_fingerprint_dir.mkdir()
        (legacy_fingerprint_dir / CAPTURE_MANIFEST_NAME).write_text(
            json.dumps(
                {
                    "schema": 8,
                    "preview_probes_only": True,
                    "metal_preview_probes_only": False,
                    "metal_renderer_probes_only": False,
                }
            )
            + "\n",
            encoding="utf-8",
        )
        legacy_fingerprint_failed = False
        try:
            validate_capture_manifest(
                "legacy-fingerprints",
                legacy_fingerprint_dir,
                preview_probes_only=True,
                map_render_only=False,
                metal_preview_probes_only=False,
                metal_renderer_probes_only=False,
                map_after_save_reload=False,
                map_after_mpq_export_reload=False,
                require_artifact_fingerprints=True,
            )
        except CaptureManifestError:
            legacy_fingerprint_failed = True
        require_self_test(legacy_fingerprint_failed, "PR manifest validation should require PNG fingerprints")
        encode_png_rgba(fingerprint_dir / "stale-extra.png", 2, 2, self_test_pixels(2, 2, 7))
        extra_fingerprint_failed = False
        try:
            validate_capture_manifest(
                "fingerprints",
                fingerprint_dir,
                preview_probes_only=True,
                map_render_only=False,
                metal_preview_probes_only=False,
                metal_renderer_probes_only=False,
                map_after_save_reload=False,
                map_after_mpq_export_reload=False,
                require_manifest=True,
            )
        except CaptureManifestError:
            extra_fingerprint_failed = True
        require_self_test(extra_fingerprint_failed, "manifest validation should reject unfingerprinted PNG artifacts")
        (fingerprint_dir / "stale-extra.png").unlink()
        encode_png_rgba(fingerprint_dir / "artifact.png", 2, 2, self_test_pixels(2, 2, 6))
        fingerprint_failed = False
        try:
            validate_capture_manifest(
                "fingerprints",
                fingerprint_dir,
                preview_probes_only=True,
                map_render_only=False,
                metal_preview_probes_only=False,
                metal_renderer_probes_only=False,
                map_after_save_reload=False,
                map_after_mpq_export_reload=False,
                require_manifest=True,
            )
        except CaptureManifestError:
            fingerprint_failed = True
        require_self_test(fingerprint_failed, "manifest validation should reject artifacts changed after capture")

    print("run_renderer_parity self-test passed.")
    return 0


def main() -> int:
    args = parse_args()
    if args.self_test:
        return run_self_test()
    apply_pr_gate_defaults(args)
    if args.pr_gate and args.same_app_determinism:
        raise SystemExit("--pr-gate cannot be used with --same-app-determinism")
    if args.pr_gate and args.keep_existing_artifacts:
        raise SystemExit("--keep-existing-artifacts cannot be used with --pr-gate")
    exclusive_modes = [args.preview_probes_only, args.map_render_only, args.metal_preview_probes_only, args.metal_renderer_probes_only]
    if sum(1 for enabled in exclusive_modes if enabled) > 1:
        raise SystemExit("--preview-probes-only, --map-render-only, --metal-preview-probes-only, and --metal-renderer-probes-only are mutually exclusive")
    if args.preview_probes_only and args.map_after_save_reload:
        raise SystemExit("--map-after-save-reload cannot be used with --preview-probes-only")
    if args.preview_probes_only and args.map_after_mpq_export_reload:
        raise SystemExit("--map-after-mpq-export-reload cannot be used with --preview-probes-only")
    if args.metal_preview_probes_only and args.map_after_save_reload:
        raise SystemExit("--map-after-save-reload cannot be used with --metal-preview-probes-only")
    if args.metal_preview_probes_only and args.map_after_mpq_export_reload:
        raise SystemExit("--map-after-mpq-export-reload cannot be used with --metal-preview-probes-only")
    if args.metal_renderer_probes_only and args.map_after_save_reload:
        raise SystemExit("--map-after-save-reload cannot be used with --metal-renderer-probes-only")
    if args.metal_renderer_probes_only and args.map_after_mpq_export_reload:
        raise SystemExit("--map-after-mpq-export-reload cannot be used with --metal-renderer-probes-only")
    if args.require_source_head is not None and args.require_current_source:
        raise SystemExit("--require-source-head and --require-current-source are mutually exclusive")
    if args.capture_only is not None and args.skip_capture:
        raise SystemExit("--capture-only cannot be used with --skip-capture")
    if args.capture_only is not None and args.require_matching_source:
        raise SystemExit("--require-matching-source cannot be used with --capture-only")
    uses_capture_archives = args.opengl_artifact_archive is not None or args.metal_artifact_archive is not None
    if uses_capture_archives and not args.skip_capture:
        raise SystemExit("--opengl-artifact-archive and --metal-artifact-archive require --skip-capture")
    if args.capture_only is not None and uses_capture_archives:
        raise SystemExit("--capture-only cannot be used with artifact archives")

    if args.capture_only == "opengl" and args.opengl_app is None:
        raise SystemExit("--capture-only opengl requires --opengl-app")
    if args.capture_only == "metal" and args.metal_app is None:
        raise SystemExit("--capture-only metal requires --metal-app")
    if args.capture_only is None and not args.skip_capture and (args.opengl_app is None or args.metal_app is None):
        raise SystemExit("--opengl-app and --metal-app are required unless --skip-capture is used")
    warcraft_dir = resolved_warcraft_dir(args.warcraft_dir)
    captures_model_previews = not (
        args.skip_capture
        or args.preview_probes_only
        or args.map_render_only
        or args.metal_preview_probes_only
        or args.metal_renderer_probes_only
    )
    if captures_model_previews and warcraft_dir is None:
        raise SystemExit(
            "Default renderer parity capture runs --smoke-model-previews and needs Warcraft III content. "
            "Pass --warcraft-dir, set HIVEWE_WARCRAFT_DIR, or use --map-render-only/--preview-probes-only."
        )

    expected_source_head = args.require_source_head
    if args.require_current_source:
        current_source = source_revision_metadata()
        if current_source is None or not isinstance(current_source.get("head"), str):
            raise SystemExit("--require-current-source needs git metadata for the current source tree")
        expected_source_head = current_source["head"]

    require_backend_identity = (
        not args.skip_capture
        or args.require_capture_manifest
        or args.require_matching_source
        or expected_source_head is not None
        or args.require_clean_source
        or args.same_app_determinism
    )
    require_smoke_picking = args.pr_gate and capture_mode_requires_smoke_picking(
        preview_probes_only=args.preview_probes_only,
        metal_preview_probes_only=args.metal_preview_probes_only,
        metal_renderer_probes_only=args.metal_renderer_probes_only,
    )

    if args.capture_only == "opengl":
        capture_artifact_dirs = (args.opengl_artifact_dir,)
    elif args.capture_only == "metal":
        capture_artifact_dirs = (args.metal_artifact_dir,)
    else:
        capture_artifact_dirs = (args.opengl_artifact_dir, args.metal_artifact_dir)

    if not args.keep_existing_artifacts and not args.skip_capture:
        for artifact_dir in capture_artifact_dirs:
            if artifact_dir.exists():
                shutil.rmtree(artifact_dir)

    try:
        if args.opengl_artifact_archive is not None:
            args.opengl_artifact_dir = extract_capture_archive(
                "OpenGL",
                args.opengl_artifact_archive,
                args.report_dir / "extracted-opengl-artifacts",
            )
            print(f"Extracted OpenGL capture archive to {args.opengl_artifact_dir}")
        if args.metal_artifact_archive is not None:
            args.metal_artifact_dir = extract_capture_archive(
                "Metal",
                args.metal_artifact_archive,
                args.report_dir / "extracted-metal-artifacts",
            )
            print(f"Extracted Metal capture archive to {args.metal_artifact_dir}")

        if args.capture_only is not None:
            if args.capture_only == "opengl":
                label = "OpenGL"
                app = args.opengl_app
                artifact_dir = args.opengl_artifact_dir
            else:
                label = "Metal"
                app = args.metal_app
                artifact_dir = args.metal_artifact_dir
            capture_backend(
                label=label,
                expected_backend=expected_backend_for_label(label, same_app_determinism=args.same_app_determinism),
                app=app,
                artifact_dir=artifact_dir,
                map_path=args.map,
                warcraft_dir=warcraft_dir,
                capture_delay_ms=args.capture_delay_ms,
                timeout=args.timeout,
                preview_probes_only=args.preview_probes_only,
                map_render_only=args.map_render_only,
                metal_preview_probes_only=args.metal_preview_probes_only,
                metal_renderer_probes_only=args.metal_renderer_probes_only,
                map_after_save_reload=args.map_after_save_reload,
                map_after_mpq_export_reload=args.map_after_mpq_export_reload,
            )
            validate_backend_artifacts(
                label,
                artifact_dir,
                preview_probes_only=args.preview_probes_only,
                map_render_only=args.map_render_only,
                metal_preview_probes_only=args.metal_preview_probes_only,
                metal_renderer_probes_only=args.metal_renderer_probes_only,
            )
            capture_manifest = validate_capture_manifest(
                label,
                artifact_dir,
                expected_backend=expected_backend_for_label(label, same_app_determinism=args.same_app_determinism),
                require_backend_identity=require_backend_identity,
                same_app_determinism=args.same_app_determinism,
                preview_probes_only=args.preview_probes_only,
                map_render_only=args.map_render_only,
                metal_preview_probes_only=args.metal_preview_probes_only,
                metal_renderer_probes_only=args.metal_renderer_probes_only,
                map_after_save_reload=args.map_after_save_reload,
                map_after_mpq_export_reload=args.map_after_mpq_export_reload,
                require_manifest=True,
                expected_source_head=expected_source_head,
                require_clean_source=args.require_clean_source,
                require_smoke_picking=require_smoke_picking,
                require_artifact_fingerprints=args.pr_gate,
                require_command_provenance=args.pr_gate,
            )
            print(
                f"Captured and validated {label} artifacts in {artifact_dir}: "
                f"{capture_manifest_summary(capture_manifest, preview_probes_only=args.preview_probes_only, map_render_only=args.map_render_only, metal_preview_probes_only=args.metal_preview_probes_only, metal_renderer_probes_only=args.metal_renderer_probes_only, map_after_save_reload=args.map_after_save_reload, map_after_mpq_export_reload=args.map_after_mpq_export_reload)}"
            )
            return 0

        if not args.skip_capture:
            capture_backend(
                label="OpenGL",
                expected_backend=expected_backend_for_label("OpenGL", same_app_determinism=args.same_app_determinism),
                app=args.opengl_app,
                artifact_dir=args.opengl_artifact_dir,
                map_path=args.map,
                warcraft_dir=warcraft_dir,
                capture_delay_ms=args.capture_delay_ms,
                timeout=args.timeout,
                preview_probes_only=args.preview_probes_only,
                map_render_only=args.map_render_only,
                metal_preview_probes_only=args.metal_preview_probes_only,
                metal_renderer_probes_only=args.metal_renderer_probes_only,
                map_after_save_reload=args.map_after_save_reload,
                map_after_mpq_export_reload=args.map_after_mpq_export_reload,
            )
            capture_backend(
                label="Metal",
                expected_backend=expected_backend_for_label("Metal", same_app_determinism=args.same_app_determinism),
                app=args.metal_app,
                artifact_dir=args.metal_artifact_dir,
                map_path=args.map,
                warcraft_dir=warcraft_dir,
                capture_delay_ms=args.capture_delay_ms,
                timeout=args.timeout,
                preview_probes_only=args.preview_probes_only,
                map_render_only=args.map_render_only,
                metal_preview_probes_only=args.metal_preview_probes_only,
                metal_renderer_probes_only=args.metal_renderer_probes_only,
                map_after_save_reload=args.map_after_save_reload,
                map_after_mpq_export_reload=args.map_after_mpq_export_reload,
            )

        validate_backend_artifacts(
            "OpenGL",
            args.opengl_artifact_dir,
            preview_probes_only=args.preview_probes_only,
            map_render_only=args.map_render_only,
            metal_preview_probes_only=args.metal_preview_probes_only,
            metal_renderer_probes_only=args.metal_renderer_probes_only,
        )
        validate_backend_artifacts(
            "Metal",
            args.metal_artifact_dir,
            preview_probes_only=args.preview_probes_only,
            map_render_only=args.map_render_only,
            metal_preview_probes_only=args.metal_preview_probes_only,
            metal_renderer_probes_only=args.metal_renderer_probes_only,
        )
        opengl_capture_manifest = validate_capture_manifest(
            "OpenGL",
            args.opengl_artifact_dir,
            expected_backend=expected_backend_for_label("OpenGL", same_app_determinism=args.same_app_determinism),
            require_backend_identity=require_backend_identity,
            same_app_determinism=args.same_app_determinism,
            preview_probes_only=args.preview_probes_only,
            map_render_only=args.map_render_only,
            metal_preview_probes_only=args.metal_preview_probes_only,
            metal_renderer_probes_only=args.metal_renderer_probes_only,
            map_after_save_reload=args.map_after_save_reload,
            map_after_mpq_export_reload=args.map_after_mpq_export_reload,
            require_manifest=args.require_capture_manifest or args.require_matching_source,
            expected_source_head=expected_source_head,
            require_clean_source=args.require_clean_source,
            require_smoke_picking=require_smoke_picking,
            require_artifact_fingerprints=args.pr_gate,
            require_command_provenance=args.pr_gate,
        )
        metal_capture_manifest = validate_capture_manifest(
            "Metal",
            args.metal_artifact_dir,
            expected_backend=expected_backend_for_label("Metal", same_app_determinism=args.same_app_determinism),
            require_backend_identity=require_backend_identity,
            same_app_determinism=args.same_app_determinism,
            preview_probes_only=args.preview_probes_only,
            map_render_only=args.map_render_only,
            metal_preview_probes_only=args.metal_preview_probes_only,
            metal_renderer_probes_only=args.metal_renderer_probes_only,
            map_after_save_reload=args.map_after_save_reload,
            map_after_mpq_export_reload=args.map_after_mpq_export_reload,
            require_manifest=args.require_capture_manifest or args.require_matching_source,
            expected_source_head=expected_source_head,
            require_clean_source=args.require_clean_source,
            require_smoke_picking=require_smoke_picking,
            require_artifact_fingerprints=args.pr_gate,
            require_command_provenance=args.pr_gate,
        )
        if require_backend_identity:
            validate_capture_backends(
                opengl_capture_manifest,
                metal_capture_manifest,
                args.opengl_artifact_dir,
                args.metal_artifact_dir,
                same_app_determinism=args.same_app_determinism,
            )
        if args.require_matching_source:
            validate_matching_capture_source_heads(
                opengl_capture_manifest,
                metal_capture_manifest,
                args.opengl_artifact_dir,
                args.metal_artifact_dir,
            )

        if args.preview_probes_only:
            exact_names = set(ARTIFACT_PRESETS["shared-preview-probes"])
        elif args.metal_preview_probes_only:
            exact_names = set(ARTIFACT_PRESETS["metal-preview-probes"])
        elif args.metal_renderer_probes_only:
            exact_names = set(ARTIFACT_PRESETS["metal-renderer-probes"])
        elif args.map_render_only:
            exact_names = set(ARTIFACT_PRESETS["map-render"])
        else:
            exact_names = set(ARTIFACT_PRESETS["map-render"]) | set(ARTIFACT_PRESETS["shared-previews"])
        prepare_report_output_dir(args.report_dir)
        diff_dir = args.report_dir / "diffs"
        results, failed = compare_artifact_directories(
            args.opengl_artifact_dir,
            args.metal_artifact_dir,
            patterns=[],
            exact_names=exact_names,
            common_only=False,
            diff_dir=diff_dir,
            diff_scale=args.diff_scale,
            max_rms=args.max_rms,
            max_mean_abs=args.max_mean_abs,
            max_changed_percent=args.max_changed_percent,
        )
    except (
        CaptureManifestError,
        FileNotFoundError,
        OSError,
        PngError,
        subprocess.CalledProcessError,
        subprocess.TimeoutExpired,
        zlib.error,
    ) as error:
        print(f"Renderer parity failed: {error}", file=sys.stderr)
        return 1

    json_path = args.report_dir / "renderer-parity.json"
    markdown_path = args.report_dir / "renderer-parity.md"
    provenance_path = args.report_dir / "renderer-parity-provenance.json"
    json_path.write_text(json.dumps(results, indent=2) + "\n", encoding="utf-8")
    provenance_path.write_text(
        json.dumps(
            {
                "requested": {
                    "pr_gate": args.pr_gate,
                    "preview_probes_only": args.preview_probes_only,
                    "map_render_only": args.map_render_only,
                    "metal_preview_probes_only": args.metal_preview_probes_only,
                    "metal_renderer_probes_only": args.metal_renderer_probes_only,
                    "map_after_save_reload": args.map_after_save_reload,
                    "map_after_mpq_export_reload": args.map_after_mpq_export_reload,
                    "same_app_determinism": args.same_app_determinism,
                    "map": None if args.preview_probes_only or args.metal_preview_probes_only or args.metal_renderer_probes_only else str(args.map),
                    "required_source_head": expected_source_head,
                    "require_matching_source": args.require_matching_source,
                    "require_clean_source": args.require_clean_source,
                    "opengl_artifact_archive": None if args.opengl_artifact_archive is None else str(args.opengl_artifact_archive),
                    "metal_artifact_archive": None if args.metal_artifact_archive is None else str(args.metal_artifact_archive),
                },
                "opengl": opengl_capture_manifest,
                "metal": metal_capture_manifest,
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )
    write_markdown_report(
        markdown_path,
        opengl_app=args.opengl_app or Path("<existing artifacts>"),
        metal_app=args.metal_app or Path("<existing artifacts>"),
        map_path=args.map,
        opengl_artifact_dir=args.opengl_artifact_dir,
        metal_artifact_dir=args.metal_artifact_dir,
        diff_dir=diff_dir,
        results=results,
        failed=failed,
        max_rms=args.max_rms,
        max_mean_abs=args.max_mean_abs,
        max_changed_percent=args.max_changed_percent,
        preview_probes_only=args.preview_probes_only,
        map_render_only=args.map_render_only,
        metal_preview_probes_only=args.metal_preview_probes_only,
        metal_renderer_probes_only=args.metal_renderer_probes_only,
        map_after_save_reload=args.map_after_save_reload,
        map_after_mpq_export_reload=args.map_after_mpq_export_reload,
        opengl_capture_manifest=opengl_capture_manifest,
        metal_capture_manifest=metal_capture_manifest,
        pr_gate=args.pr_gate,
    )

    ok_count = sum(1 for result in results if result["status"] == "ok")
    print(f"Compared {len(results)} renderer parity artifacts: {ok_count} ok, {len(results) - ok_count} failed.")
    print(f"Wrote {json_path}")
    print(f"Wrote {provenance_path}")
    print(f"Wrote {markdown_path}")
    print(f"Wrote diff PNGs in {diff_dir}")
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())
