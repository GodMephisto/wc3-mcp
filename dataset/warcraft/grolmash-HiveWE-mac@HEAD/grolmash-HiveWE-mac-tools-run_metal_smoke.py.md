---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "tools/run_metal_smoke.py"
ref: HEAD
---

#!/usr/bin/env python3
"""Run HiveWE's full Metal smoke gate and validate the PNG artifact set."""

from __future__ import annotations

import argparse
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import platform
import shlex
import shutil
import subprocess
import sys
import tempfile

sys.dont_write_bytecode = True

from compare_smoke_artifacts import (
    FULL_METAL_SMOKE_ARTIFACT_SIZES,
    MAP_RENDER_ARTIFACT_SIZES,
    METAL_PREVIEW_PROBE_ARTIFACT_SIZES,
    METAL_RENDERER_PROBE_ARTIFACT_SIZES,
    MIN_VARIED_PIXELS,
    PngError,
    SHARED_PREVIEW_ARTIFACT_SIZES,
    count_varied_pixels,
    encode_png_rgba,
    read_png_size,
    validate_png_artifact_manifest,
)


class SmokeError(RuntimeError):
    pass


FULL_SMOKE_PROVENANCE_NAME = "metal-smoke-provenance.json"
REQUIRED_FULL_SMOKE_COMMAND_FLAGS = (
    "--smoke-save-reload",
    "--smoke-mutated-save-reload",
    "--smoke-mpq-export-reload",
    "--smoke-render-modes",
    "--smoke-picking",
    "--smoke-model-previews",
)
REQUIRED_FULL_SMOKE_COMMAND_OPTIONS = (
    "--map",
    "--warcraft-dir",
    "--capture-frame",
    "--smoke-artifact-dir",
    "--capture-delay-ms",
)


def resolve_executable(path: Path) -> Path:
    if path.suffix == ".app":
        executable = path / "Contents" / "MacOS" / "HiveWE"
    else:
        executable = path
    if not executable.is_file():
        raise SmokeError(f"HiveWE executable not found: {executable}")
    return executable


def sha256_file(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for block in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(block)
    return digest.hexdigest()


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


def artifact_fingerprints(artifact_dir: Path) -> list[dict]:
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


def validate_reported_backend(executable: Path, expected_backend: str = "metal") -> str:
    command = [str(executable), "--print-render-backend"]
    command_text = shlex.join(command)
    try:
        result = subprocess.run(command, check=False, capture_output=True, text=True, timeout=10)
    except subprocess.TimeoutExpired as error:
        raise SmokeError(f"backend fingerprint command timed out after {error.timeout:.2f}s: {command_text}") from error

    if result.returncode != 0:
        detail = result.stderr.strip()
        suffix = f"\n{detail}" if detail else ""
        raise SmokeError(f"backend fingerprint command failed with exit code {result.returncode}: {command_text}{suffix}")

    reported_backend = result.stdout.strip().lower()
    if reported_backend != expected_backend:
        detail = result.stderr.strip()
        suffix = f"\nstderr:\n{detail}" if detail else ""
        raise SmokeError(
            f"full Metal runtime smoke requires backend '{expected_backend}', got '{reported_backend}' from {command_text}{suffix}"
        )

    print(f"Validated app reports {expected_backend} renderer backend.")
    return reported_backend


def write_smoke_provenance(
    *,
    app: Path,
    executable: Path,
    reported_backend: str,
    artifact_dir: Path,
    capture_frame: Path,
    map_path: Path,
    warcraft_dir: Path,
    capture_delay_ms: int,
    command: list[str],
) -> Path:
    capture_width, capture_height = read_png_size(capture_frame)
    provenance = {
        "schema": 1,
        "label": "full-metal-runtime-smoke",
        "expected_backend": "metal",
        "reported_backend": reported_backend,
        "captured_at_utc": datetime.now(timezone.utc).isoformat(),
        "host": platform.platform(),
        "app": str(app),
        "executable": str(executable),
        "executable_size_bytes": executable.stat().st_size,
        "executable_sha256": sha256_file(executable),
        "source_revision": source_revision_metadata(),
        "artifact_dir": str(artifact_dir),
        "map": str(map_path),
        "warcraft_dir": str(warcraft_dir),
        "capture_delay_ms": capture_delay_ms,
        "command": command,
        "capture_frame": {
            "path": str(capture_frame),
            "name": capture_frame.name,
            "width": capture_width,
            "height": capture_height,
            "size_bytes": capture_frame.stat().st_size,
            "sha256": sha256_file(capture_frame),
        },
        "artifacts": artifact_fingerprints(artifact_dir),
    }
    provenance_path = artifact_dir / FULL_SMOKE_PROVENANCE_NAME
    provenance_path.write_text(json.dumps(provenance, indent=2) + "\n", encoding="utf-8")
    print(f"Wrote {provenance_path}")
    return provenance_path


def read_smoke_provenance(artifact_dir: Path) -> dict:
    provenance_path = artifact_dir / FULL_SMOKE_PROVENANCE_NAME
    if not provenance_path.is_file():
        raise SmokeError(f"missing full Metal smoke provenance: {provenance_path}")
    try:
        provenance = json.loads(provenance_path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as error:
        raise SmokeError(f"invalid full Metal smoke provenance {provenance_path}: {error}") from error
    if not isinstance(provenance, dict):
        raise SmokeError(f"invalid full Metal smoke provenance {provenance_path}: expected object")
    return provenance


def validate_command_option(command: list[str], option: str) -> str:
    try:
        index = command.index(option)
    except ValueError as error:
        raise SmokeError(f"full Metal smoke provenance command is missing {option}") from error
    if index + 1 >= len(command):
        raise SmokeError(f"full Metal smoke provenance command is missing a value for {option}")
    value = command[index + 1]
    if not value or value.startswith("--"):
        raise SmokeError(f"full Metal smoke provenance command has an invalid value for {option}: {value!r}")
    return value


def require_command_option_value(command: list[str], option: str, expected: object) -> None:
    if not isinstance(expected, (str, int)):
        raise SmokeError(f"full Metal smoke provenance cannot validate {option}; recorded value is missing")
    actual = validate_command_option(command, option)
    expected_text = str(expected)
    if actual != expected_text:
        raise SmokeError(
            f"full Metal smoke provenance command {option} value {actual!r} does not match recorded {expected_text!r}"
        )


def validate_smoke_command(provenance: dict) -> None:
    command = provenance.get("command")
    if not isinstance(command, list) or not command or not all(isinstance(part, str) for part in command):
        raise SmokeError("full Metal smoke provenance is missing the executed command")
    missing_flags = [flag for flag in REQUIRED_FULL_SMOKE_COMMAND_FLAGS if flag not in command]
    if missing_flags:
        raise SmokeError("full Metal smoke provenance command is missing required flag(s): " + ", ".join(missing_flags))

    for earlier, later in (
        ("--smoke-save-reload", "--smoke-render-modes"),
        ("--smoke-mutated-save-reload", "--smoke-render-modes"),
        ("--smoke-mpq-export-reload", "--smoke-render-modes"),
    ):
        if command.index(earlier) > command.index(later):
            raise SmokeError(f"full Metal smoke provenance command records {earlier} after {later}")

    warcraft_dir = provenance.get("warcraft_dir")
    if not isinstance(warcraft_dir, str) or not warcraft_dir:
        raise SmokeError("full Metal smoke provenance does not record a Warcraft III directory")
    capture_frame = provenance.get("capture_frame")
    capture_frame_path = capture_frame.get("path") if isinstance(capture_frame, dict) else None
    for option, expected in (
        ("--map", provenance.get("map")),
        ("--warcraft-dir", warcraft_dir),
        ("--capture-frame", capture_frame_path),
        ("--smoke-artifact-dir", provenance.get("artifact_dir")),
        ("--capture-delay-ms", provenance.get("capture_delay_ms")),
    ):
        require_command_option_value(command, option, expected)


def validate_smoke_provenance(
    artifact_dir: Path,
    capture_frame: Path,
    *,
    expected_source_head: str | None = None,
    require_current_source: bool = False,
    require_clean_source: bool = False,
) -> None:
    provenance = read_smoke_provenance(artifact_dir)
    if provenance.get("schema") != 1:
        raise SmokeError("full Metal smoke provenance has unsupported schema")
    if provenance.get("label") != "full-metal-runtime-smoke":
        raise SmokeError("full Metal smoke provenance has the wrong label")
    if provenance.get("expected_backend") != "metal" or provenance.get("reported_backend") != "metal":
        raise SmokeError("full Metal smoke provenance does not prove the Metal backend")
    validate_smoke_command(provenance)

    executable = provenance.get("executable")
    executable_size = provenance.get("executable_size_bytes")
    executable_hash = provenance.get("executable_sha256")
    if not isinstance(executable, str) or not isinstance(executable_size, int) or not isinstance(executable_hash, str):
        raise SmokeError("full Metal smoke provenance is missing executable fingerprint data")

    source = provenance.get("source_revision")
    if expected_source_head is not None or require_current_source or require_clean_source:
        if not isinstance(source, dict):
            raise SmokeError("full Metal smoke provenance does not record source revision metadata")
        source_head = source.get("head")
        if not isinstance(source_head, str) or not source_head:
            raise SmokeError("full Metal smoke provenance does not record a source revision head")
    if expected_source_head is not None and source.get("head") != expected_source_head:
        raise SmokeError(
            "full Metal smoke provenance was captured from source "
            f"{source.get('head')}, expected {expected_source_head}"
        )
    if require_current_source:
        current_source = source_revision_metadata()
        current_head = current_source.get("head") if isinstance(current_source, dict) else None
        if not isinstance(current_head, str) or not current_head:
            raise SmokeError("--require-current-source needs git metadata for the current source tree")
        if source.get("head") != current_head:
            raise SmokeError(
                "full Metal smoke provenance was captured from source "
                f"{source.get('head')}, current source is {current_head}"
            )
    if require_clean_source:
        source_dirty = source.get("dirty")
        if not isinstance(source_dirty, bool):
            raise SmokeError("full Metal smoke provenance does not record source dirty state")
        if source_dirty:
            status = source.get("status_short")
            suffix = ""
            if isinstance(status, list) and status:
                suffix = ":\n" + "\n".join(str(line) for line in status)
            raise SmokeError("full Metal smoke provenance was captured from a dirty source tree" + suffix)

    capture = provenance.get("capture_frame")
    if not isinstance(capture, dict):
        raise SmokeError("full Metal smoke provenance is missing capture-frame fingerprint data")
    capture_width, capture_height = read_png_size(capture_frame)
    expected_capture = {
        "name": capture_frame.name,
        "width": capture_width,
        "height": capture_height,
        "size_bytes": capture_frame.stat().st_size,
        "sha256": sha256_file(capture_frame),
    }
    for key, expected_value in expected_capture.items():
        if capture.get(key) != expected_value:
            raise SmokeError(
                f"full Metal smoke provenance capture-frame {key} mismatch: "
                f"recorded {capture.get(key)!r}, current {expected_value!r}"
            )

    recorded_artifacts = provenance.get("artifacts")
    if not isinstance(recorded_artifacts, list):
        raise SmokeError("full Metal smoke provenance is missing artifact fingerprints")
    current_artifacts = artifact_fingerprints(artifact_dir)
    if recorded_artifacts != current_artifacts:
        raise SmokeError("full Metal smoke provenance artifact fingerprints do not match the current PNG files")

    print(f"Validated full Metal smoke provenance in {artifact_dir / FULL_SMOKE_PROVENANCE_NAME}.")


def build_full_smoke_command(
    executable: Path,
    map_path: Path,
    warcraft_dir: Path | None,
    capture_frame: Path,
    artifact_dir: Path,
    capture_delay_ms: int,
) -> list[str]:
    command = [
        str(executable),
        "--map",
        str(map_path),
        "--smoke-save-reload",
        "--smoke-mutated-save-reload",
        "--smoke-mpq-export-reload",
        "--smoke-render-modes",
        "--smoke-picking",
        "--smoke-model-previews",
        "--capture-frame",
        str(capture_frame),
        "--smoke-artifact-dir",
        str(artifact_dir),
        "--capture-delay-ms",
        str(capture_delay_ms),
    ]
    if warcraft_dir is not None:
        command[3:3] = ["--warcraft-dir", str(warcraft_dir)]
    return command


def validate_artifacts(
    artifact_dir: Path,
    capture_frame: Path,
    *,
    expected_source_head: str | None = None,
    require_current_source: bool = False,
    require_clean_source: bool = False,
) -> None:
    if not capture_frame.is_file():
        raise SmokeError(f"missing capture frame: {capture_frame}")
    capture_size = read_png_size(capture_frame)
    _capture_width, _capture_height, capture_varied = count_varied_pixels(capture_frame)
    if capture_varied < MIN_VARIED_PIXELS:
        raise SmokeError(f"capture frame is too flat: {capture_frame} has {capture_varied} varied pixels")
    map_render_baseline = artifact_dir / "map-render-baseline.png"
    if not map_render_baseline.is_file():
        raise SmokeError(f"missing smoke artifacts: {map_render_baseline.name}")
    map_render_size = read_png_size(map_render_baseline)

    required_names = set(FULL_METAL_SMOKE_ARTIFACT_SIZES)
    if capture_frame.name in required_names and (artifact_dir / capture_frame.name).resolve() == capture_frame.resolve():
        raise SmokeError(f"capture frame path uses reserved smoke artifact name: {capture_frame.name}")

    required_count = validate_png_artifact_manifest(
        artifact_dir,
        FULL_METAL_SMOKE_ARTIFACT_SIZES,
        variable_size=map_render_size,
        label="smoke artifacts",
        allow_extra=True,
    )

    capture_path = capture_frame.resolve()
    extra = sorted(
        path.name
        for path in artifact_dir.glob("*.png")
        if path.name not in required_names and path.resolve() != capture_path
    )
    if extra:
        raise SmokeError("unexpected smoke artifacts: " + ", ".join(extra))

    validate_smoke_provenance(
        artifact_dir,
        capture_frame,
        expected_source_head=expected_source_head,
        require_current_source=require_current_source,
        require_clean_source=require_clean_source,
    )

    png_count = len(list(artifact_dir.glob("*.png")))
    print(f"Validated {required_count} required artifacts in {artifact_dir} ({png_count} PNGs total).")
    print(f"Capture frame: {capture_frame} {capture_size[0]}x{capture_size[1]}, {capture_varied} varied pixels")
    print(f"Map render artifacts: {map_render_size[0]}x{map_render_size[1]}")


def self_test_pixels(width: int, height: int, seed: int) -> bytes:
    pixels = bytearray()
    for index in range(width * height):
        pixels.extend(((seed + index) & 0xFF, (seed * 3 + index * 5) & 0xFF, (seed * 7 + index * 11) & 0xFF, 255))
    return bytes(pixels)


def require_self_test(condition: bool, message: str) -> None:
    if not condition:
        raise AssertionError(message)


def run_self_test() -> int:
    with tempfile.TemporaryDirectory(prefix="hivewe-metal-smoke-wrapper-") as temp_dir:
        root = Path(temp_dir)
        executable = root / "HiveWE"
        executable.write_text("fake\n", encoding="utf-8")
        command = build_full_smoke_command(
            executable=executable,
            map_path=Path("data/test map"),
            warcraft_dir=Path("/Applications/Warcraft III"),
            capture_frame=root / "capture.png",
            artifact_dir=root / "artifacts",
            capture_delay_ms=123,
        )
        required_flags = [
            "--warcraft-dir",
            "--smoke-save-reload",
            "--smoke-mutated-save-reload",
            "--smoke-mpq-export-reload",
            "--smoke-render-modes",
            "--smoke-picking",
            "--smoke-model-previews",
            "--capture-frame",
            "--smoke-artifact-dir",
            "--capture-delay-ms",
        ]
        for flag in required_flags:
            require_self_test(flag in command, f"full smoke command should include {flag}")
        require_self_test(
            command.index("--warcraft-dir") < command.index("--smoke-save-reload"),
            "Warcraft directory should be passed before smoke mode flags",
        )
        require_self_test(
            command.index("--smoke-save-reload") < command.index("--smoke-render-modes"),
            "save/reload smoke should run before render-mode smoke",
        )

        expected_names = (
            set(MAP_RENDER_ARTIFACT_SIZES)
            | set(METAL_RENDERER_PROBE_ARTIFACT_SIZES)
            | set(SHARED_PREVIEW_ARTIFACT_SIZES)
            | set(METAL_PREVIEW_PROBE_ARTIFACT_SIZES)
        )
        require_self_test(
            set(FULL_METAL_SMOKE_ARTIFACT_SIZES) == expected_names,
            "full Metal smoke manifest should be the exact union of map, renderer, shared-preview, and Metal-preview manifests",
        )
        require_self_test(
            "metal-model-depth-isolation.png" in FULL_METAL_SMOKE_ARTIFACT_SIZES,
            "full Metal smoke manifest should include model depth-isolation renderer probe",
        )
        require_self_test(
            "metal-meshless-bounds-picking.png" in FULL_METAL_SMOKE_ARTIFACT_SIZES,
            "full Metal smoke manifest should include meshless bounds picking renderer probe",
        )
        require_self_test(
            "metal-pe1-spawn-render.png" in FULL_METAL_SMOKE_ARTIFACT_SIZES,
            "full Metal smoke manifest should include visible PE1 spawned-model renderer probe",
        )

        class FakeBackendResult:
            def __init__(self, stdout: str) -> None:
                self.returncode = 0
                self.stdout = stdout
                self.stderr = ""

        original_subprocess_run = subprocess.run
        try:
            subprocess.run = lambda *args, **kwargs: FakeBackendResult("metal\n")
            reported_backend = validate_reported_backend(executable)
            require_self_test(reported_backend == "metal", "backend validation should return reported Metal backend")

            subprocess.run = lambda *args, **kwargs: FakeBackendResult("opengl\n")
            backend_failed = False
            try:
                validate_reported_backend(executable)
            except SmokeError:
                backend_failed = True
            require_self_test(backend_failed, "full Metal smoke should reject non-Metal backend identity")
        finally:
            subprocess.run = original_subprocess_run

        artifact_dir = root / "artifacts"
        artifact_dir.mkdir()
        capture_frame = root / "capture.png"
        encode_png_rgba(capture_frame, 64, 48, self_test_pixels(64, 48, 7))
        for index, (name, size) in enumerate(FULL_METAL_SMOKE_ARTIFACT_SIZES.items()):
            width, height = size if size is not None else (32, 24)
            encode_png_rgba(artifact_dir / name, width, height, self_test_pixels(width, height, 19 + index))
        provenance_path = write_smoke_provenance(
            app=executable,
            executable=executable,
            reported_backend="metal",
            artifact_dir=artifact_dir,
            capture_frame=capture_frame,
            map_path=Path("data/test map"),
            warcraft_dir=Path("/Applications/Warcraft III"),
            capture_delay_ms=123,
            command=command,
        )
        provenance = json.loads(provenance_path.read_text(encoding="utf-8"))
        require_self_test(
            provenance.get("reported_backend") == "metal",
            "full Metal smoke provenance should record backend identity",
        )
        require_self_test(
            provenance.get("executable_sha256") == sha256_file(executable),
            "full Metal smoke provenance should fingerprint the executable",
        )
        require_self_test(
            len(provenance.get("artifacts", [])) == len(FULL_METAL_SMOKE_ARTIFACT_SIZES),
            "full Metal smoke provenance should fingerprint every required PNG artifact",
        )
        provenance_artifact_names = {artifact.get("name") for artifact in provenance.get("artifacts", []) if isinstance(artifact, dict)}
        require_self_test(
            "metal-meshless-bounds-picking.png" in provenance_artifact_names,
            "full Metal smoke provenance should fingerprint meshless bounds picking artifact",
        )
        require_self_test(
            "metal-pe1-spawn-render.png" in provenance_artifact_names,
            "full Metal smoke provenance should fingerprint PE1 spawned-model render artifact",
        )
        validate_artifacts(artifact_dir, capture_frame)

        incomplete_command_provenance = dict(provenance)
        incomplete_command_provenance["command"] = [
            str(executable),
            "--map",
            "data/test map",
            "--warcraft-dir",
            "/Applications/Warcraft III",
            "--smoke-render-modes",
            "--smoke-model-previews",
            "--capture-frame",
            str(capture_frame),
            "--smoke-artifact-dir",
            str(artifact_dir),
            "--capture-delay-ms",
            "123",
        ]
        provenance_path.write_text(json.dumps(incomplete_command_provenance, indent=2) + "\n", encoding="utf-8")
        incomplete_command_failed = False
        try:
            validate_artifacts(artifact_dir, capture_frame)
        except SmokeError:
            incomplete_command_failed = True
        require_self_test(
            incomplete_command_failed,
            "full Metal smoke validation should reject provenance missing save/reload command flags",
        )
        provenance_path.write_text(json.dumps(provenance, indent=2) + "\n", encoding="utf-8")

        mismatched_command_provenance = dict(provenance)
        mismatched_command_provenance["command"] = list(provenance["command"])
        mismatched_command_provenance["command"][mismatched_command_provenance["command"].index("--map") + 1] = "data/other map"
        provenance_path.write_text(json.dumps(mismatched_command_provenance, indent=2) + "\n", encoding="utf-8")
        mismatched_command_failed = False
        try:
            validate_artifacts(artifact_dir, capture_frame)
        except SmokeError:
            mismatched_command_failed = True
        require_self_test(
            mismatched_command_failed,
            "full Metal smoke validation should reject provenance command values that do not match recorded inputs",
        )
        provenance_path.write_text(json.dumps(provenance, indent=2) + "\n", encoding="utf-8")

        provenance_path.unlink()
        missing_provenance_failed = False
        try:
            validate_artifacts(artifact_dir, capture_frame)
        except SmokeError:
            missing_provenance_failed = True
        require_self_test(missing_provenance_failed, "full Metal smoke validation should require provenance")
        provenance_path.write_text(json.dumps(provenance, indent=2) + "\n", encoding="utf-8")

        stale_provenance = dict(provenance)
        stale_provenance["artifacts"] = list(provenance["artifacts"])
        stale_provenance["artifacts"][0] = dict(stale_provenance["artifacts"][0])
        stale_provenance["artifacts"][0]["sha256"] = "0" * 64
        provenance_path.write_text(json.dumps(stale_provenance, indent=2) + "\n", encoding="utf-8")
        stale_provenance_failed = False
        try:
            validate_artifacts(artifact_dir, capture_frame)
        except SmokeError:
            stale_provenance_failed = True
        require_self_test(stale_provenance_failed, "full Metal smoke validation should reject stale provenance")

        current_source = source_revision_metadata()
        if isinstance(current_source, dict) and isinstance(current_source.get("head"), str):
            clean_source_provenance = dict(provenance)
            clean_source_provenance["source_revision"] = {
                "head": current_source["head"],
                "dirty": False,
                "status_short": [],
            }
            provenance_path.write_text(json.dumps(clean_source_provenance, indent=2) + "\n", encoding="utf-8")
            validate_artifacts(
                artifact_dir,
                capture_frame,
                require_current_source=True,
                require_clean_source=True,
            )
            validate_artifacts(
                artifact_dir,
                capture_frame,
                expected_source_head=current_source["head"],
                require_clean_source=True,
            )

            dirty_source_provenance = dict(clean_source_provenance)
            dirty_source_provenance["source_revision"] = {
                "head": current_source["head"],
                "dirty": True,
                "status_short": [" M tools/run_metal_smoke.py"],
            }
            provenance_path.write_text(json.dumps(dirty_source_provenance, indent=2) + "\n", encoding="utf-8")
            dirty_source_failed = False
            try:
                validate_artifacts(artifact_dir, capture_frame, require_clean_source=True)
            except SmokeError:
                dirty_source_failed = True
            require_self_test(dirty_source_failed, "full Metal smoke validation should reject dirty source provenance")

            explicit_wrong_source_failed = False
            try:
                validate_artifacts(artifact_dir, capture_frame, expected_source_head="f" * 40)
            except SmokeError:
                explicit_wrong_source_failed = True
            require_self_test(
                explicit_wrong_source_failed,
                "full Metal smoke validation should reject explicit source-head mismatches",
            )

            wrong_source_provenance = dict(clean_source_provenance)
            wrong_source_provenance["source_revision"] = {
                "head": "0" * 40,
                "dirty": False,
                "status_short": [],
            }
            provenance_path.write_text(json.dumps(wrong_source_provenance, indent=2) + "\n", encoding="utf-8")
            wrong_source_failed = False
            try:
                validate_artifacts(artifact_dir, capture_frame, require_current_source=True)
            except SmokeError:
                wrong_source_failed = True
            require_self_test(wrong_source_failed, "full Metal smoke validation should reject stale source provenance")

    print("run_metal_smoke self-test passed.")
    return 0


def main() -> int:
    parser = argparse.ArgumentParser(description="Run HiveWE's full Metal smoke gate and validate artifacts.")
    parser.add_argument(
        "app",
        nargs="?",
        type=Path,
        default=Path("build/macOS-Debug-WithTests/HiveWE.app"),
        help="HiveWE.app bundle or HiveWE executable. Defaults to build/macOS-Debug-WithTests/HiveWE.app.",
    )
    parser.add_argument("--map", type=Path, default=Path("data/test map/"), help="Map directory to load.")
    parser.add_argument(
        "--warcraft-dir",
        type=Path,
        default=os.environ.get("HIVEWE_WARCRAFT_DIR") or None,
        help="Warcraft III installation directory to pass to HiveWE. Defaults to HIVEWE_WARCRAFT_DIR when set.",
    )
    parser.add_argument("--artifact-dir", type=Path, default=None, help="Directory for smoke PNG artifacts.")
    parser.add_argument("--capture-frame", type=Path, default=None, help="Main map capture PNG path.")
    parser.add_argument("--capture-delay-ms", type=int, default=1000, help="Delay before render smoke capture.")
    parser.add_argument("--timeout", type=float, default=60.0, help="Maximum seconds to wait for HiveWE.")
    parser.add_argument("--keep-existing-artifacts", action="store_true", help="Do not clear the artifact directory first.")
    parser.add_argument("--skip-run", action="store_true", help="Only validate an existing artifact directory and capture frame.")
    parser.add_argument(
        "--require-source-head",
        metavar="SHA",
        help="Require full-smoke provenance to match this exact source revision.",
    )
    parser.add_argument(
        "--require-current-source",
        action="store_true",
        help="Require full-smoke provenance to match the current git HEAD.",
    )
    parser.add_argument(
        "--require-clean-source",
        action="store_true",
        help="Require full-smoke provenance to record a clean source tree.",
    )
    parser.add_argument(
        "--skip-if-missing-warcraft-dir",
        action="store_true",
        help="Return 77 without running when neither --warcraft-dir nor HIVEWE_WARCRAFT_DIR is set.",
    )
    parser.add_argument("--self-test", action="store_true", help="Run dependency-free wrapper and manifest self-tests and exit.")
    args = parser.parse_args()

    if args.self_test:
        return run_self_test()
    if args.require_source_head and args.require_current_source:
        raise SmokeError("--require-source-head and --require-current-source are mutually exclusive")

    artifact_dir = args.artifact_dir or Path(tempfile.gettempdir()) / "hivewe-metal-smoke-artifacts"
    capture_frame = args.capture_frame or Path(tempfile.gettempdir()) / "hivewe-metal-smoke.png"

    if not args.skip_run:
        if args.skip_if_missing_warcraft_dir and args.warcraft_dir is None:
            print("Skipping full Metal runtime smoke: HIVEWE_WARCRAFT_DIR is not set.")
            return 77
        if args.warcraft_dir is None:
            raise SmokeError("full Metal runtime smoke requires --warcraft-dir or HIVEWE_WARCRAFT_DIR")
        if args.warcraft_dir is not None and not args.warcraft_dir.is_dir():
            raise SmokeError(f"Warcraft III directory not found: {args.warcraft_dir}")

        executable = resolve_executable(args.app)
        reported_backend = validate_reported_backend(executable)
        if not args.keep_existing_artifacts and artifact_dir.exists():
            shutil.rmtree(artifact_dir)
        artifact_dir.mkdir(parents=True, exist_ok=True)
        if capture_frame.exists():
            capture_frame.unlink()

        command = build_full_smoke_command(
            executable=executable,
            map_path=args.map,
            warcraft_dir=args.warcraft_dir,
            capture_frame=capture_frame,
            artifact_dir=artifact_dir,
            capture_delay_ms=args.capture_delay_ms,
        )
        print("Running:", shlex.join(command), flush=True)
        subprocess.run(command, check=True, timeout=args.timeout)
        write_smoke_provenance(
            app=args.app,
            executable=executable,
            reported_backend=reported_backend,
            artifact_dir=artifact_dir,
            capture_frame=capture_frame,
            map_path=args.map,
            warcraft_dir=args.warcraft_dir,
            capture_delay_ms=args.capture_delay_ms,
            command=command,
        )

    validate_artifacts(
        artifact_dir,
        capture_frame,
        expected_source_head=args.require_source_head,
        require_current_source=args.require_current_source,
        require_clean_source=args.require_clean_source,
    )
    return 0


if __name__ == "__main__":
    try:
        sys.exit(main())
    except (OSError, PngError, SmokeError, subprocess.CalledProcessError, subprocess.TimeoutExpired) as error:
        print(f"Metal smoke failed: {error}", file=sys.stderr)
        sys.exit(1)
