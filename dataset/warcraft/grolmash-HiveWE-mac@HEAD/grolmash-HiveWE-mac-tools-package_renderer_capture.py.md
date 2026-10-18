---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "tools/package_renderer_capture.py"
ref: HEAD
---

#!/usr/bin/env python3
"""Validate and package one renderer parity capture directory."""

from __future__ import annotations

import argparse
from datetime import datetime, timezone
import json
from pathlib import Path
import sys
import tempfile
import zipfile

sys.dont_write_bytecode = True

from compare_smoke_artifacts import (
    MAP_RENDER_ARTIFACT_SIZES,
    METAL_PREVIEW_PROBE_ARTIFACT_SIZES,
    METAL_RENDERER_PROBE_ARTIFACT_SIZES,
    SHARED_PREVIEW_ARTIFACT_SIZES,
    SYNTHETIC_SHARED_PREVIEW_ARTIFACT_SIZES,
    encode_png_rgba,
)
from run_renderer_parity import (
    CAPTURE_MANIFEST_NAME,
    CaptureManifestError,
    capture_mode_requires_smoke_picking,
    capture_manifest_summary,
    read_capture_manifest,
    source_revision_metadata,
    validate_backend_artifacts,
    validate_capture_manifest,
    write_capture_manifest,
)


def manifest_mode(manifest: dict) -> dict:
    preview_probes_only = bool(manifest.get("preview_probes_only", False))
    metal_preview_probes_only = bool(manifest.get("metal_preview_probes_only", False))
    metal_renderer_probes_only = bool(manifest.get("metal_renderer_probes_only", False))
    return {
        "preview_probes_only": preview_probes_only,
        "map_render_only": False
        if preview_probes_only or metal_preview_probes_only or metal_renderer_probes_only
        else bool(manifest.get("map_render_only", False)),
        "metal_preview_probes_only": metal_preview_probes_only,
        "metal_renderer_probes_only": metal_renderer_probes_only,
        "map_after_save_reload": False
        if preview_probes_only or metal_preview_probes_only or metal_renderer_probes_only
        else bool(manifest.get("map_after_save_reload", False)),
        "map_after_mpq_export_reload": False
        if preview_probes_only or metal_preview_probes_only or metal_renderer_probes_only
        else bool(manifest.get("map_after_mpq_export_reload", False)),
    }


def source_head_from_current_tree() -> str:
    source = source_revision_metadata()
    head = source.get("head") if isinstance(source, dict) else None
    if not isinstance(head, str):
        raise CaptureManifestError("--require-current-source needs git metadata for the current source tree")
    return head


def validate_capture(
    artifact_dir: Path,
    *,
    expected_backend: str | None,
    expected_source_head: str | None,
    require_clean_source: bool,
) -> dict:
    manifest = read_capture_manifest(artifact_dir)
    if manifest is None:
        raise CaptureManifestError(f"artifact directory is missing {CAPTURE_MANIFEST_NAME}: {artifact_dir}")
    mode = manifest_mode(manifest)
    validate_backend_artifacts(
        "Renderer capture",
        artifact_dir,
        **{key: mode[key] for key in mode if key not in ("map_after_save_reload", "map_after_mpq_export_reload")},
    )
    return validate_capture_manifest(
        "Renderer capture",
        artifact_dir,
        expected_backend=expected_backend,
        require_backend_identity=True,
        preview_probes_only=mode["preview_probes_only"],
        map_render_only=mode["map_render_only"],
        metal_preview_probes_only=mode["metal_preview_probes_only"],
        metal_renderer_probes_only=mode["metal_renderer_probes_only"],
        map_after_save_reload=mode["map_after_save_reload"],
        map_after_mpq_export_reload=mode["map_after_mpq_export_reload"],
        require_manifest=True,
        expected_source_head=expected_source_head,
        require_clean_source=require_clean_source,
        require_smoke_picking=capture_mode_requires_smoke_picking(
            preview_probes_only=mode["preview_probes_only"],
            metal_preview_probes_only=mode["metal_preview_probes_only"],
            metal_renderer_probes_only=mode["metal_renderer_probes_only"],
        ),
        require_artifact_fingerprints=True,
        require_command_provenance=True,
    )


def mode_slug(mode: dict) -> str:
    if mode["preview_probes_only"]:
        return "shared-preview-probes"
    if mode["metal_preview_probes_only"]:
        return "metal-preview-probes"
    if mode["metal_renderer_probes_only"]:
        return "metal-renderer-probes"
    base = "map-render" if mode["map_render_only"] else "map-preview"
    if mode["map_after_save_reload"] and mode["map_after_mpq_export_reload"]:
        return f"{base}-save-mpq-reload"
    if mode["map_after_mpq_export_reload"]:
        return f"{base}-mpq-reload"
    if mode["map_after_save_reload"]:
        return f"{base}-save-reload"
    return base


def compare_mode_flags(mode: dict) -> list[str]:
    if mode["preview_probes_only"]:
        return ["--preview-probes-only"]
    if mode["metal_preview_probes_only"]:
        return ["--metal-preview-probes-only"]
    if mode["metal_renderer_probes_only"]:
        return ["--metal-renderer-probes-only"]
    flags = []
    if mode["map_render_only"]:
        flags.append("--map-render-only")
    if mode["map_after_save_reload"]:
        flags.append("--map-after-save-reload")
    if mode["map_after_mpq_export_reload"]:
        flags.append("--map-after-mpq-export-reload")
    return flags


def default_archive_prefix(manifest: dict, artifact_dir: Path) -> str:
    backend = manifest.get("reported_backend")
    source = manifest.get("source_revision")
    head = source.get("head") if isinstance(source, dict) else None
    parts = [
        "hivewe",
        str(backend) if isinstance(backend, str) else "renderer",
        mode_slug(manifest_mode(manifest)),
        "capture",
    ]
    if isinstance(head, str) and head:
        parts.append(head[:12])
    else:
        parts.append(artifact_dir.name)
    return "-".join(parts)


def expected_artifact_sizes_for_mode(mode: dict) -> dict[str, tuple[int, int] | None]:
    if mode["preview_probes_only"]:
        return SYNTHETIC_SHARED_PREVIEW_ARTIFACT_SIZES
    if mode["metal_preview_probes_only"]:
        return METAL_PREVIEW_PROBE_ARTIFACT_SIZES
    if mode["metal_renderer_probes_only"]:
        return METAL_RENDERER_PROBE_ARTIFACT_SIZES
    if mode["map_render_only"]:
        return MAP_RENDER_ARTIFACT_SIZES
    return {**MAP_RENDER_ARTIFACT_SIZES, **SHARED_PREVIEW_ARTIFACT_SIZES}


def artifact_size_label(size: tuple[int, int] | None) -> str:
    if size is None:
        return "map canvas size"
    width, height = size
    return f"{width}x{height}"


def captured_artifact_lookup(manifest: dict) -> dict[str, dict]:
    artifacts = manifest.get("artifacts", [])
    if not isinstance(artifacts, list):
        return {}
    return {
        artifact["name"]: artifact
        for artifact in artifacts
        if isinstance(artifact, dict) and isinstance(artifact.get("name"), str)
    }


def short_sha(value: object) -> str:
    if not isinstance(value, str) or not value:
        return "unknown"
    return value[:12]


def shell_flags(flags: list[str]) -> str:
    return "" if not flags else " ".join(flags) + " "


def markdown_summary(manifest: dict, artifact_dir: Path, archive_prefix: str) -> str:
    mode = manifest_mode(manifest)
    backend = manifest.get("reported_backend", "unknown")
    source = manifest.get("source_revision")
    head = source.get("head") if isinstance(source, dict) else None
    dirty = source.get("dirty") if isinstance(source, dict) else None
    source_branch = source.get("branch") if isinstance(source, dict) else None
    source_status = source.get("status_short") if isinstance(source, dict) else None
    artifacts = manifest.get("artifacts", [])
    commands = manifest.get("commands", [])
    artifact_count = len(artifacts) if isinstance(artifacts, list) else 0
    command_lines = []
    if isinstance(commands, list):
        for command in commands:
            if isinstance(command, list):
                command_lines.append(" ".join(str(part) for part in command))
    expected_artifacts = expected_artifact_sizes_for_mode(mode)
    captured_artifacts = captured_artifact_lookup(manifest)
    mode_flags = shell_flags(compare_mode_flags(mode))
    source_gate = f"--require-source-head {head} " if isinstance(head, str) and head else ""

    compare_note = ""
    if backend == "metal":
        compare_note = (
            "Capture the OpenGL reference on a Windows/Linux OpenGL 4.5 runtime with the same commit, then run:\n\n"
            "```shell\n"
            f"python3 tools/run_renderer_parity.py --skip-capture --pr-gate {source_gate}{mode_flags}"
            "--opengl-artifact-dir <opengl artifacts> --metal-artifact-archive <this zip> "
            "--report-dir <report dir>\n"
            "```\n"
        )
    elif backend == "opengl":
        compare_note = (
            "Capture the Metal candidate on macOS with the same commit, then run:\n\n"
            "```shell\n"
            f"python3 tools/run_renderer_parity.py --skip-capture --pr-gate {source_gate}{mode_flags}"
            "--opengl-artifact-archive <this zip> --metal-artifact-dir <metal artifacts> "
            "--report-dir <report dir>\n"
            "```\n"
        )

    lines = [
        "# HiveWE Renderer Capture Bundle",
        "",
        f"- Archive prefix: `{archive_prefix}`",
        f"- Artifact directory: `{artifact_dir}`",
        f"- Backend: `{backend}`",
        f"- Source: `{head if isinstance(head, str) else 'unknown'}`",
        f"- Source branch: `{source_branch if isinstance(source_branch, str) else 'unknown'}`",
        f"- Source dirty at capture: `{dirty}`",
        f"- Host: `{manifest.get('host', 'unknown')}`",
        f"- App: `{manifest.get('app', 'unknown')}`",
        f"- Executable: `{manifest.get('executable', 'unknown')}`",
        f"- Executable size: `{manifest.get('executable_size_bytes', 'unknown')}` bytes",
        f"- Executable SHA-256: `{short_sha(manifest.get('executable_sha256'))}`",
        f"- Captured at UTC: `{manifest.get('captured_at_utc', 'unknown')}`",
        f"- Packaged at UTC: `{datetime.now(timezone.utc).isoformat()}`",
        f"- Artifacts: `{artifact_count}` PNG file(s)",
        f"- Mode: {capture_manifest_summary(manifest, **mode)}",
        "",
        "## Capture Commands",
        "",
    ]
    if command_lines:
        lines.extend(f"- `{command}`" for command in command_lines)
    else:
        lines.append("- Not recorded.")
    if isinstance(source_status, list) and source_status:
        lines.extend(["", "## Dirty Source Files", ""])
        lines.extend(f"- `{entry}`" for entry in source_status if isinstance(entry, str))
    lines.extend(
        [
            "",
            "## Required Artifacts",
            "",
            "| Artifact | Expected size | Captured size | Bytes | SHA-256 |",
            "| --- | --- | --- | ---: | --- |",
        ]
    )
    for name, expected_size in expected_artifacts.items():
        captured = captured_artifacts.get(name, {})
        width = captured.get("width")
        height = captured.get("height")
        captured_size = f"{width}x{height}" if isinstance(width, int) and isinstance(height, int) else "missing"
        size_bytes = captured.get("size_bytes")
        size_bytes_label = str(size_bytes) if isinstance(size_bytes, int) else "missing"
        sha256 = captured.get("sha256")
        sha256_label = f"`{sha256[:12]}`" if isinstance(sha256, str) else "missing"
        lines.append(
            f"| `{name}` | {artifact_size_label(expected_size)} | {captured_size} | {size_bytes_label} | {sha256_label} |"
        )
    extra_artifact_names = sorted(name for name in captured_artifacts if name not in expected_artifacts)
    if extra_artifact_names:
        lines.extend(
            [
                "",
                "## Additional Captured Artifacts",
                "",
                "| Artifact | Captured size | Bytes | SHA-256 |",
                "| --- | --- | ---: | --- |",
            ]
        )
        for name in extra_artifact_names:
            captured = captured_artifacts[name]
            width = captured.get("width")
            height = captured.get("height")
            captured_size = f"{width}x{height}" if isinstance(width, int) and isinstance(height, int) else "unknown"
            size_bytes = captured.get("size_bytes")
            size_bytes_label = str(size_bytes) if isinstance(size_bytes, int) else "unknown"
            sha256 = captured.get("sha256")
            sha256_label = f"`{sha256[:12]}`" if isinstance(sha256, str) else "unknown"
            lines.append(f"| `{name}` | {captured_size} | {size_bytes_label} | {sha256_label} |")
    lines.extend(
        [
            "",
            "## PR Gate Review Checklist",
            "",
            "- Compare this bundle with the counterpart backend using `tools/run_renderer_parity.py --skip-capture --pr-gate`.",
            "- The counterpart capture must use the same clean source revision recorded in `renderer-parity-capture.json`; "
            "use the pinned `--require-source-head` flag below when auditing this archived bundle after the branch advances.",
            "- Map parity bundles must include recorded folder save/reload or MPQ export/reload capture state plus "
            "recorded `--smoke-picking` coverage.",
            "- Review terrain, cliffs, water, pathing, brush masks, selection overlays, SD/HD materials, team colors, "
            "transparency, particles, model editor, and object-grid artifacts before treating the evidence as PR-ready.",
        ]
    )
    if compare_note:
        lines.extend(["", "## Next Step", "", compare_note.rstrip()])
    return "\n".join(lines) + "\n"


def write_archive(artifact_dir: Path, output: Path, manifest: dict, archive_prefix: str) -> None:
    output.parent.mkdir(parents=True, exist_ok=True)
    summary = markdown_summary(manifest, artifact_dir, archive_prefix)
    with zipfile.ZipFile(output, "w", compression=zipfile.ZIP_DEFLATED) as archive:
        archive.writestr(f"{archive_prefix}/renderer-capture-summary.md", summary)
        for path in sorted(artifact_dir.iterdir()):
            if path.is_file():
                archive.write(path, f"{archive_prefix}/{path.name}")


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Validate and zip one HiveWE renderer parity capture directory.")
    parser.add_argument("--artifact-dir", type=Path, help="Renderer artifact directory containing renderer-parity-capture.json.")
    parser.add_argument("--output", type=Path, help="Output .zip path.")
    parser.add_argument("--expected-backend", choices=("opengl", "metal"), help="Require this captured backend.")
    parser.add_argument("--require-source-head", metavar="SHA", help="Require this exact source revision in the capture manifest.")
    parser.add_argument("--require-current-source", action="store_true", help="Require the capture source revision to match current git HEAD.")
    parser.add_argument("--require-clean-source", action="store_true", help="Require the capture manifest to record a clean source tree.")
    parser.add_argument("--prefix", help="Archive directory prefix. Defaults to backend and source revision.")
    parser.add_argument("--self-test", action="store_true", help="Run dependency-free package self-test and exit.")
    return parser.parse_args()


def self_test_pixels(width: int, height: int, seed: int) -> bytes:
    pixels = bytearray()
    for index in range(width * height):
        pixels.extend(((seed + index) & 0xFF, (seed * 3 + index * 5) & 0xFF, (seed * 7 + index * 11) & 0xFF, 255))
    return bytes(pixels)


def require_self_test(condition: bool, message: str) -> None:
    if not condition:
        raise AssertionError(message)


def write_self_test_artifacts(artifact_dir: Path, artifact_sizes: dict[str, tuple[int, int] | None], seed: int) -> None:
    artifact_dir.mkdir()
    for index, (name, size) in enumerate(artifact_sizes.items()):
        width, height = size if size is not None else (24, 16)
        encode_png_rgba(artifact_dir / name, width, height, self_test_pixels(width, height, seed + index))


def run_self_test() -> int:
    with tempfile.TemporaryDirectory(prefix="hivewe-renderer-capture-package-") as temp_dir:
        root = Path(temp_dir)
        artifact_dir = root / "metal-artifacts"
        fake_app = root / "fake_hivewe"
        fake_app.write_text("fake\n", encoding="utf-8")
        write_self_test_artifacts(artifact_dir, {**MAP_RENDER_ARTIFACT_SIZES, **SHARED_PREVIEW_ARTIFACT_SIZES}, 10)
        write_capture_manifest(
            label="Metal",
            expected_backend="metal",
            reported_backend="metal",
            app=fake_app,
            executable=fake_app,
            artifact_dir=artifact_dir,
            map_path=Path("data/test map"),
            warcraft_dir=Path("/tmp/Warcraft III"),
            capture_delay_ms=1000,
            preview_probes_only=False,
            map_render_only=False,
            metal_preview_probes_only=False,
            metal_renderer_probes_only=False,
            map_after_save_reload=True,
            map_after_mpq_export_reload=False,
            commands=[["fake", "--smoke-save-reload", "--smoke-render-modes", "--smoke-picking"], ["fake", "--smoke-model-previews"]],
        )
        manifest = validate_capture(
            artifact_dir,
            expected_backend="metal",
            expected_source_head=None,
            require_clean_source=False,
        )
        require_self_test(
            "map-preview-save-reload" in default_archive_prefix(manifest, artifact_dir),
            "default archive prefix should include the map capture mode",
        )
        output = root / "capture.zip"
        write_archive(artifact_dir, output, manifest, "test-prefix")
        require_self_test(output.is_file(), "package should create a zip archive")
        with zipfile.ZipFile(output) as archive:
            names = set(archive.namelist())
            require_self_test("test-prefix/renderer-capture-summary.md" in names, "archive should include summary markdown")
            require_self_test(f"test-prefix/{CAPTURE_MANIFEST_NAME}" in names, "archive should include capture manifest")
            summary = archive.read("test-prefix/renderer-capture-summary.md").decode("utf-8")
            require_self_test("Backend: `metal`" in summary, "summary should record backend")
            require_self_test("--smoke-picking" in summary, "summary should record picking smoke coverage")
            require_self_test("Required Artifacts" in summary, "summary should list required artifacts")
            require_self_test("map-render-baseline.png" in summary, "summary should include map-render artifact names")
            require_self_test("PR Gate Review Checklist" in summary, "summary should include PR gate checklist")
            require_self_test("same clean source revision" in summary, "summary should call out matching clean source requirement")
            require_self_test("--require-source-head " in summary, "summary should pin the source revision for archived compare commands")
            require_self_test("--map-after-save-reload" in summary, "summary should include map save/reload comparison flag")
            require_self_test("--metal-artifact-archive <this zip>" in summary, "summary should include next comparison step")

        wrong_backend_failed = False
        try:
            validate_capture(
                artifact_dir,
                expected_backend="opengl",
                expected_source_head=None,
                require_clean_source=False,
            )
        except CaptureManifestError:
            wrong_backend_failed = True
        require_self_test(wrong_backend_failed, "package validation should reject the wrong expected backend")

        shared_preview_probe_dir = root / "shared-preview-probes"
        write_self_test_artifacts(shared_preview_probe_dir, SYNTHETIC_SHARED_PREVIEW_ARTIFACT_SIZES, 80)
        write_capture_manifest(
            label="Shared preview probes",
            expected_backend="metal",
            reported_backend="metal",
            app=fake_app,
            executable=fake_app,
            artifact_dir=shared_preview_probe_dir,
            map_path=Path("data/test map"),
            warcraft_dir=None,
            capture_delay_ms=1000,
            preview_probes_only=True,
            map_render_only=False,
            metal_preview_probes_only=False,
            metal_renderer_probes_only=False,
            map_after_save_reload=False,
            map_after_mpq_export_reload=False,
            commands=[["fake", "--smoke-shared-preview-probes"]],
        )
        shared_preview_probe_manifest = validate_capture(
            shared_preview_probe_dir,
            expected_backend="metal",
            expected_source_head=None,
            require_clean_source=False,
        )
        require_self_test(
            "shared-preview-probes" in default_archive_prefix(shared_preview_probe_manifest, shared_preview_probe_dir),
            "default archive prefix should include the shared preview probe mode",
        )
        shared_preview_probe_output = root / "shared-preview-probes.zip"
        write_archive(shared_preview_probe_dir, shared_preview_probe_output, shared_preview_probe_manifest, "shared-preview-probes")
        with zipfile.ZipFile(shared_preview_probe_output) as archive:
            names = set(archive.namelist())
            summary = archive.read("shared-preview-probes/renderer-capture-summary.md").decode("utf-8")
            require_self_test("preview probes only" in summary, "summary should record shared-preview-probes-only mode")
            require_self_test(
                "object-grid-particle-preview.png" in summary,
                "shared preview probe summary should list required artifact names",
            )
            require_self_test("--preview-probes-only" in summary, "shared preview probe summary should include compare mode flag")
            require_self_test(
                "shared-preview-probes/model-editor-material-preview.png" in names,
                "shared preview probe package should include the model-editor material artifact",
            )
            require_self_test(
                "shared-preview-probes/object-grid-particle-preview.png" in names,
                "shared preview probe package should include the object-grid particle artifact",
            )

        mpq_map_render_dir = root / "mpq-map-render"
        write_self_test_artifacts(mpq_map_render_dir, MAP_RENDER_ARTIFACT_SIZES, 100)
        write_capture_manifest(
            label="MPQ map render",
            expected_backend="metal",
            reported_backend="metal",
            app=fake_app,
            executable=fake_app,
            artifact_dir=mpq_map_render_dir,
            map_path=Path("data/test map"),
            warcraft_dir=None,
            capture_delay_ms=1000,
            preview_probes_only=False,
            map_render_only=True,
            metal_preview_probes_only=False,
            metal_renderer_probes_only=False,
            map_after_save_reload=False,
            map_after_mpq_export_reload=True,
            commands=[["fake", "--smoke-mpq-export-reload", "--smoke-render-modes", "--smoke-picking"]],
        )
        mpq_map_render_manifest = validate_capture(
            mpq_map_render_dir,
            expected_backend="metal",
            expected_source_head=None,
            require_clean_source=False,
        )
        mpq_map_render_output = root / "mpq-map-render.zip"
        write_archive(mpq_map_render_dir, mpq_map_render_output, mpq_map_render_manifest, "mpq-map-render")
        with zipfile.ZipFile(mpq_map_render_output) as archive:
            summary = archive.read("mpq-map-render/renderer-capture-summary.md").decode("utf-8")
            require_self_test(
                "--map-render-only --map-after-mpq-export-reload" in summary,
                "MPQ map-render summary should include map-render and MPQ comparison flags",
            )
            require_self_test(
                "--map-after-save-reload" not in summary,
                "MPQ map-render summary should not add folder save/reload comparison flags",
            )

        renderer_probe_dir = root / "metal-renderer-probes"
        write_self_test_artifacts(renderer_probe_dir, METAL_RENDERER_PROBE_ARTIFACT_SIZES, 120)
        write_capture_manifest(
            label="Metal renderer probes",
            expected_backend="metal",
            reported_backend="metal",
            app=fake_app,
            executable=fake_app,
            artifact_dir=renderer_probe_dir,
            map_path=Path("data/test map"),
            warcraft_dir=None,
            capture_delay_ms=1000,
            preview_probes_only=False,
            map_render_only=False,
            metal_preview_probes_only=False,
            metal_renderer_probes_only=True,
            map_after_save_reload=False,
            map_after_mpq_export_reload=False,
            commands=[["fake", "--smoke-metal-renderer-probes"]],
        )
        renderer_probe_manifest = validate_capture(
            renderer_probe_dir,
            expected_backend="metal",
            expected_source_head=None,
            require_clean_source=False,
        )
        require_self_test(
            "metal-renderer-probes" in default_archive_prefix(renderer_probe_manifest, renderer_probe_dir),
            "default archive prefix should include the Metal renderer probe mode",
        )
        renderer_probe_output = root / "renderer-probes.zip"
        write_archive(renderer_probe_dir, renderer_probe_output, renderer_probe_manifest, "renderer-probes")
        with zipfile.ZipFile(renderer_probe_output) as archive:
            names = set(archive.namelist())
            summary = archive.read("renderer-probes/renderer-capture-summary.md").decode("utf-8")
            require_self_test("Metal renderer probes only" in summary, "summary should record renderer-probes-only mode")
            require_self_test(
                "metal-meshless-bounds-picking.png" in summary,
                "renderer probe summary should list the meshless bounds picking artifact",
            )
            require_self_test(
                "metal-pe1-spawn-render.png" in summary,
                "renderer probe summary should list the PE1 spawned-model render artifact",
            )
            require_self_test("--metal-renderer-probes-only" in summary, "renderer probe summary should include compare mode flag")
            require_self_test(
                "renderer-probes/metal-model-depth-isolation.png" in names,
                "renderer probe package should include the depth-isolation artifact",
            )
            require_self_test(
                "renderer-probes/metal-meshless-bounds-picking.png" in names,
                "renderer probe package should include the meshless bounds picking artifact",
            )
            require_self_test(
                "renderer-probes/metal-pe1-spawn-render.png" in names,
                "renderer probe package should include the PE1 spawned-model render artifact",
            )

        metal_preview_probe_dir = root / "metal-preview-probes"
        write_self_test_artifacts(metal_preview_probe_dir, METAL_PREVIEW_PROBE_ARTIFACT_SIZES, 180)
        write_capture_manifest(
            label="Metal preview probes",
            expected_backend="metal",
            reported_backend="metal",
            app=fake_app,
            executable=fake_app,
            artifact_dir=metal_preview_probe_dir,
            map_path=Path("data/test map"),
            warcraft_dir=None,
            capture_delay_ms=1000,
            preview_probes_only=False,
            map_render_only=False,
            metal_preview_probes_only=True,
            metal_renderer_probes_only=False,
            map_after_save_reload=False,
            map_after_mpq_export_reload=False,
            commands=[["fake", "--smoke-metal-preview-probes"]],
        )
        metal_preview_probe_manifest = validate_capture(
            metal_preview_probe_dir,
            expected_backend="metal",
            expected_source_head=None,
            require_clean_source=False,
        )
        require_self_test(
            "metal-preview-probes" in default_archive_prefix(metal_preview_probe_manifest, metal_preview_probe_dir),
            "default archive prefix should include the Metal preview probe mode",
        )
        metal_preview_probe_output = root / "metal-preview-probes.zip"
        write_archive(metal_preview_probe_dir, metal_preview_probe_output, metal_preview_probe_manifest, "metal-preview-probes")
        with zipfile.ZipFile(metal_preview_probe_output) as archive:
            names = set(archive.namelist())
            summary = archive.read("metal-preview-probes/renderer-capture-summary.md").decode("utf-8")
            require_self_test("Metal preview probes only" in summary, "summary should record Metal-preview-probes-only mode")
            require_self_test(
                "model-editor-ribbon-preview.png" in summary,
                "Metal preview probe summary should list ribbon preview artifacts",
            )
            require_self_test("--metal-preview-probes-only" in summary, "Metal preview probe summary should include compare mode flag")
            require_self_test(
                "metal-preview-probes/object-grid-multicell-material-preview.png" in names,
                "Metal preview probe package should include the multicell object-grid artifact",
            )

    print("package_renderer_capture self-test passed.")
    return 0


def main() -> int:
    args = parse_args()
    if args.self_test:
        return run_self_test()
    if args.artifact_dir is None or args.output is None:
        raise SystemExit("--artifact-dir and --output are required unless --self-test is used")
    if args.require_source_head and args.require_current_source:
        raise SystemExit("--require-source-head and --require-current-source are mutually exclusive")

    expected_source_head = args.require_source_head
    if args.require_current_source:
        expected_source_head = source_head_from_current_tree()

    try:
        manifest = validate_capture(
            args.artifact_dir,
            expected_backend=args.expected_backend,
            expected_source_head=expected_source_head,
            require_clean_source=args.require_clean_source,
        )
        archive_prefix = args.prefix or default_archive_prefix(manifest, args.artifact_dir)
        write_archive(args.artifact_dir, args.output, manifest, archive_prefix)
    except (CaptureManifestError, OSError, zipfile.BadZipFile) as error:
        print(f"Renderer capture package failed: {error}", file=sys.stderr)
        return 1

    print(f"Packaged renderer capture in {args.output}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
