---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "tools/audit_renderer_pr_evidence.py"
ref: HEAD
---

#!/usr/bin/env python3
"""Audit renderer artifacts expected for a maintainer-facing Metal PR."""

from __future__ import annotations

import argparse
from dataclasses import dataclass
import hashlib
import json
import math
from pathlib import Path
import re
import sys
import tempfile
import zipfile

sys.dont_write_bytecode = True

from compare_smoke_artifacts import (
    FULL_METAL_SMOKE_ARTIFACT_SIZES,
    MAP_RENDER_ARTIFACT_SIZES,
    PngError,
    SHARED_PREVIEW_ARTIFACT_SIZES,
    compare_artifact_directories,
    encode_png_rgba,
    read_png_size,
)
from package_renderer_capture import manifest_mode, validate_capture
from run_metal_smoke import (
    build_full_smoke_command,
    SmokeError,
    read_smoke_provenance,
    sha256_file,
    validate_artifacts as validate_full_smoke_artifacts,
    write_smoke_provenance,
)
from run_renderer_parity import (
    CAPTURE_MANIFEST_NAME,
    CaptureManifestError,
    extract_capture_archive,
    source_revision_metadata,
    validate_capture_backends,
    validate_matching_capture_source_heads,
    write_capture_manifest,
)


class EvidenceAuditError(RuntimeError):
    pass


MANUAL_REVIEW_TOPICS = (
    ("terrain", ("terrain",)),
    ("cliffs", ("cliff", "cliffs")),
    ("water", ("water",)),
    ("SD materials", ("sd", "standard definition", "sd/hd")),
    ("HD materials", ("hd", "high definition", "sd/hd")),
    ("team colors", ("team color", "team-color")),
    ("transparency/alpha", ("transparent", "transparency", "alpha")),
    ("selection overlays", ("selection",)),
    ("picking", ("picking", "pick")),
    ("particles", ("particle", "pe1", "pe2", "ribbon")),
    ("model editor previews", ("model editor", "model-editor")),
    ("object grid previews", ("object grid", "object-grid")),
    ("save/reload parity", ("save/reload", "saved/reloaded", "post-save", "post-save/reload")),
    ("MPQ export/reload", ("mpq", "mpq-export", "export/reload")),
    ("diff metrics", ("diff", "rms", "changed percent", "changed_percent", "mean abs", "mean_abs")),
)

MANUAL_REVIEW_PLACEHOLDER_PATTERNS = (
    re.compile(r"\bTODO\b", re.IGNORECASE),
    re.compile(r"\bTBD\b", re.IGNORECASE),
    re.compile(r"\[ \]"),
    re.compile(r"<[^>\n]+>"),
)
MANUAL_REVIEW_EVIDENCE_PATTERNS = (
    (
        "parity report",
        re.compile(r"\b(parity report|renderer-parity\.(md|json)|renderer-parity-provenance\.json)\b", re.IGNORECASE),
    ),
    (
        "OpenGL capture",
        re.compile(r"\bopengl\b[^\n]*(capture|artifact|bundle|archive|zip|directory|dir)", re.IGNORECASE),
    ),
    (
        "Metal capture",
        re.compile(r"\bmetal\b[^\n]*(capture|artifact|bundle|archive|zip|directory|dir)", re.IGNORECASE),
    ),
)
MANUAL_REVIEW_METRIC_PATTERNS = (
    re.compile(r"\brms\b[^\n0-9-]*[-+]?\d+(?:\.\d+)?", re.IGNORECASE),
    re.compile(r"\bmean[ _-]?abs(?:olute)?\b[^\n0-9-]*[-+]?\d+(?:\.\d+)?", re.IGNORECASE),
    re.compile(r"\bchanged[ _-]?percent\b[^\n0-9-]*[-+]?\d+(?:\.\d+)?\s*%?", re.IGNORECASE),
)
BUILD_PROVENANCE_RELATIVE_PATH = Path("Contents/Resources/hivewe-build-provenance.json")
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


@dataclass
class AuditCheck:
    status: str
    name: str
    detail: str


@dataclass
class ParityReportEvidence:
    opengl_manifest: dict
    metal_manifest: dict


def current_source_head() -> str:
    source = source_revision_metadata()
    head = source.get("head") if isinstance(source, dict) else None
    if not isinstance(head, str) or not head:
        raise EvidenceAuditError("--require-current-source needs git metadata for the current source tree")
    return head


def resolve_capture_input(label: str, artifact_dir: Path | None, archive_path: Path | None, temp_root: Path) -> Path | None:
    if artifact_dir is not None and archive_path is not None:
        raise EvidenceAuditError(f"{label} evidence accepts either an artifact directory or archive, not both")
    if archive_path is not None:
        archive_dir_name = label.lower().replace(" ", "-")
        return extract_capture_archive(label, archive_path, temp_root / f"{archive_dir_name}-archive")
    if artifact_dir is not None:
        if not artifact_dir.is_dir():
            raise FileNotFoundError(f"{label} artifact directory not found: {artifact_dir}")
        return artifact_dir
    return None


def validate_default_pr_capture(
    label: str,
    artifact_dir: Path,
    *,
    expected_backend: str,
    expected_source_head: str | None,
    require_clean_source: bool,
) -> dict:
    manifest = validate_capture(
        artifact_dir,
        expected_backend=expected_backend,
        expected_source_head=expected_source_head,
        require_clean_source=require_clean_source,
    )
    mode = manifest_mode(manifest)
    if mode["preview_probes_only"] or mode["metal_preview_probes_only"] or mode["metal_renderer_probes_only"]:
        raise CaptureManifestError(
            f"{label} capture is a focused probe-only bundle; PR renderer evidence needs map render + previews"
        )
    if mode["map_render_only"]:
        raise CaptureManifestError(f"{label} capture is map-render-only; PR renderer evidence needs model previews too")
    if not mode["map_after_save_reload"]:
        raise CaptureManifestError(f"{label} capture was not taken after --smoke-save-reload")
    if mode["map_after_mpq_export_reload"]:
        raise CaptureManifestError(
            f"{label} capture is MPQ-export/reload mode; use the default saved/reloaded map + preview PR capture"
        )
    return manifest


def validate_mpq_map_render_capture(
    label: str,
    artifact_dir: Path,
    *,
    expected_backend: str,
    expected_source_head: str | None,
    require_clean_source: bool,
) -> dict:
    manifest = validate_capture(
        artifact_dir,
        expected_backend=expected_backend,
        expected_source_head=expected_source_head,
        require_clean_source=require_clean_source,
    )
    mode = manifest_mode(manifest)
    if mode["preview_probes_only"] or mode["metal_preview_probes_only"] or mode["metal_renderer_probes_only"]:
        raise CaptureManifestError(f"{label} MPQ capture is a focused probe-only bundle; expected map-render artifacts")
    if not mode["map_render_only"]:
        raise CaptureManifestError(f"{label} MPQ capture is not map-render-only")
    if mode["map_after_save_reload"]:
        raise CaptureManifestError(f"{label} MPQ capture also records folder save/reload mode")
    if not mode["map_after_mpq_export_reload"]:
        raise CaptureManifestError(f"{label} MPQ capture was not taken after --smoke-mpq-export-reload")
    if not bool(manifest.get("smoke_picking", False)):
        raise CaptureManifestError(f"{label} MPQ capture does not prove --smoke-picking ran")
    return manifest


def short_source(manifest: dict | None) -> str:
    if manifest is None:
        return "unknown"
    source = manifest.get("source_revision")
    head = source.get("head") if isinstance(source, dict) else None
    return head[:12] if isinstance(head, str) else "unknown"


def audit_evidence(
    *,
    metal_artifact_dir: Path | None,
    metal_artifact_archive: Path | None,
    metal_mpq_artifact_dir: Path | None = None,
    metal_mpq_artifact_archive: Path | None = None,
    opengl_artifact_dir: Path | None,
    opengl_artifact_archive: Path | None,
    opengl_mpq_artifact_dir: Path | None = None,
    opengl_mpq_artifact_archive: Path | None = None,
    parity_report_dir: Path | None,
    mpq_parity_report_dir: Path | None = None,
    installed_smoke_artifact_dir: Path | None,
    installed_smoke_capture_frame: Path | None,
    installed_smoke_app: Path | None,
    manual_review_note: Path | None,
    require_current_source: bool,
    require_clean_source: bool,
    require_mpq_parity_report: bool,
    allow_missing_opengl: bool,
    allow_missing_manual_review: bool,
    allow_missing_installed_app_identity: bool,
    skip_installed_smoke: bool,
    debug_package_report: Path | None = None,
    release_package_report: Path | None = None,
    required_source_head: str | None = None,
) -> list[AuditCheck]:
    checks: list[AuditCheck] = []
    if required_source_head is not None and require_current_source:
        raise EvidenceAuditError("--require-source-head and --require-current-source are mutually exclusive")
    expected_source_head = required_source_head
    if expected_source_head is None and require_current_source:
        expected_source_head = current_source_head()

    def add(status: str, name: str, detail: str) -> None:
        checks.append(AuditCheck(status, name, detail))

    with tempfile.TemporaryDirectory(prefix="hivewe-renderer-pr-evidence-") as temp_dir:
        temp_root = Path(temp_dir)
        try:
            metal_dir = resolve_capture_input("Metal", metal_artifact_dir, metal_artifact_archive, temp_root)
        except (OSError, CaptureManifestError, EvidenceAuditError, zipfile.BadZipFile) as error:
            metal_dir = None
            add("FAIL", "Metal renderer capture", str(error))

        try:
            opengl_dir = resolve_capture_input("OpenGL", opengl_artifact_dir, opengl_artifact_archive, temp_root)
        except (OSError, CaptureManifestError, EvidenceAuditError, zipfile.BadZipFile) as error:
            opengl_dir = None
            add("FAIL", "OpenGL renderer capture", str(error))

        metal_manifest = None
        if metal_dir is None and not any(check.name == "Metal renderer capture" and check.status == "FAIL" for check in checks):
            add("FAIL", "Metal renderer capture", "missing Metal candidate artifact directory/archive")
        elif metal_dir is not None:
            try:
                metal_manifest = validate_default_pr_capture(
                    "Metal",
                    metal_dir,
                    expected_backend="metal",
                    expected_source_head=expected_source_head,
                    require_clean_source=require_clean_source,
                )
                add(
                    "PASS",
                    "Metal renderer capture",
                    f"{metal_dir} records backend metal, source {short_source(metal_manifest)}, saved/reloaded map + previews",
                )
            except (OSError, CaptureManifestError, zipfile.BadZipFile) as error:
                add("FAIL", "Metal renderer capture", str(error))

        opengl_manifest = None
        if opengl_dir is None and not any(check.name == "OpenGL renderer capture" and check.status == "FAIL" for check in checks):
            detail = (
                "missing OpenGL reference capture; capture it on Windows/Linux OpenGL 4.5 with "
                "`tools/run_renderer_parity.py --capture-only opengl --opengl-app <OpenGL HiveWE executable> "
                "--opengl-artifact-dir <opengl artifacts> --map \"data/test map/\" "
                "--warcraft-dir \"<Warcraft III>\" --pr-gate`"
            )
            add("WARN" if allow_missing_opengl else "FAIL", "OpenGL renderer capture", detail)
        elif opengl_dir is not None:
            try:
                opengl_manifest = validate_default_pr_capture(
                    "OpenGL",
                    opengl_dir,
                    expected_backend="opengl",
                    expected_source_head=expected_source_head,
                    require_clean_source=require_clean_source,
                )
                add(
                    "PASS",
                    "OpenGL renderer capture",
                    f"{opengl_dir} records backend opengl, source {short_source(opengl_manifest)}, saved/reloaded map + previews",
                )
            except (OSError, CaptureManifestError, zipfile.BadZipFile) as error:
                add("FAIL", "OpenGL renderer capture", str(error))

        if opengl_manifest is not None and metal_manifest is not None and opengl_dir is not None and metal_dir is not None:
            try:
                validate_matching_capture_source_heads(opengl_manifest, metal_manifest, opengl_dir, metal_dir)
                validate_capture_backends(
                    opengl_manifest,
                    metal_manifest,
                    opengl_dir,
                    metal_dir,
                    same_app_determinism=False,
                )
                add("PASS", "OpenGL/Metal pairing", "captures use matching source revisions and distinct reference/candidate backends")
            except CaptureManifestError as error:
                add("FAIL", "OpenGL/Metal pairing", str(error))

        if metal_manifest is not None:
            if debug_package_report is None:
                add(
                    "WARN" if allow_missing_opengl else "FAIL",
                    "Metal renderer capture package identity",
                    "missing Debug package report; cannot prove the Metal parity capture came from the packaged Debug app",
                )
            else:
                try:
                    add(
                        "PASS",
                        "Metal renderer capture package identity",
                        validate_metal_capture_package_report(
                            metal_manifest,
                            debug_package_report=debug_package_report,
                            expected_source_head=expected_source_head,
                            require_clean_source=require_clean_source,
                            label="Metal renderer capture",
                        ),
                    )
                except (OSError, EvidenceAuditError, json.JSONDecodeError) as error:
                    add("FAIL", "Metal renderer capture package identity", str(error))

        try:
            metal_mpq_dir = resolve_capture_input(
                "Metal MPQ",
                metal_mpq_artifact_dir,
                metal_mpq_artifact_archive,
                temp_root,
            )
        except (OSError, CaptureManifestError, EvidenceAuditError, zipfile.BadZipFile) as error:
            metal_mpq_dir = None
            add("FAIL", "Metal MPQ map-render capture", str(error))

        try:
            opengl_mpq_dir = resolve_capture_input(
                "OpenGL MPQ",
                opengl_mpq_artifact_dir,
                opengl_mpq_artifact_archive,
                temp_root,
            )
        except (OSError, CaptureManifestError, EvidenceAuditError, zipfile.BadZipFile) as error:
            opengl_mpq_dir = None
            add("FAIL", "OpenGL MPQ map-render capture", str(error))

        metal_mpq_manifest = None
        if metal_mpq_dir is None and require_mpq_parity_report and not any(
            check.name == "Metal MPQ map-render capture" and check.status == "FAIL" for check in checks
        ):
            add(
                "FAIL",
                "Metal MPQ map-render capture",
                "missing Metal MPQ export/reload map-render capture; pass "
                "`--metal-mpq-artifact-dir` or `--metal-mpq-artifact-archive` with the final MPQ parity evidence",
            )
        elif metal_mpq_dir is not None:
            try:
                metal_mpq_manifest = validate_mpq_map_render_capture(
                    "Metal",
                    metal_mpq_dir,
                    expected_backend="metal",
                    expected_source_head=expected_source_head,
                    require_clean_source=require_clean_source,
                )
                add(
                    "PASS",
                    "Metal MPQ map-render capture",
                    f"{metal_mpq_dir} records backend metal, source {short_source(metal_mpq_manifest)}, MPQ export/reload map render + picking",
                )
            except (OSError, CaptureManifestError, zipfile.BadZipFile) as error:
                add("FAIL", "Metal MPQ map-render capture", str(error))

        opengl_mpq_manifest = None
        if opengl_mpq_dir is None and require_mpq_parity_report and not any(
            check.name == "OpenGL MPQ map-render capture" and check.status == "FAIL" for check in checks
        ):
            add(
                "FAIL",
                "OpenGL MPQ map-render capture",
                "missing OpenGL MPQ export/reload map-render capture; pass "
                "`--opengl-mpq-artifact-dir` or `--opengl-mpq-artifact-archive` with the final MPQ parity evidence",
            )
        elif opengl_mpq_dir is not None:
            try:
                opengl_mpq_manifest = validate_mpq_map_render_capture(
                    "OpenGL",
                    opengl_mpq_dir,
                    expected_backend="opengl",
                    expected_source_head=expected_source_head,
                    require_clean_source=require_clean_source,
                )
                add(
                    "PASS",
                    "OpenGL MPQ map-render capture",
                    f"{opengl_mpq_dir} records backend opengl, source {short_source(opengl_mpq_manifest)}, MPQ export/reload map render + picking",
                )
            except (OSError, CaptureManifestError, zipfile.BadZipFile) as error:
                add("FAIL", "OpenGL MPQ map-render capture", str(error))

        if (
            opengl_mpq_manifest is not None
            and metal_mpq_manifest is not None
            and opengl_mpq_dir is not None
            and metal_mpq_dir is not None
        ):
            try:
                validate_matching_capture_source_heads(opengl_mpq_manifest, metal_mpq_manifest, opengl_mpq_dir, metal_mpq_dir)
                validate_capture_backends(
                    opengl_mpq_manifest,
                    metal_mpq_manifest,
                    opengl_mpq_dir,
                    metal_mpq_dir,
                    same_app_determinism=False,
                )
                add(
                    "PASS",
                    "OpenGL/Metal MPQ pairing",
                    "MPQ map-render captures use matching source revisions and distinct reference/candidate backends",
                )
            except CaptureManifestError as error:
                add("FAIL", "OpenGL/Metal MPQ pairing", str(error))

        if metal_mpq_manifest is not None:
            if debug_package_report is None:
                add(
                    "WARN" if allow_missing_opengl else "FAIL",
                    "Metal MPQ map-render capture package identity",
                    "missing Debug package report; cannot prove the Metal MPQ parity capture came from the packaged Debug app",
                )
            else:
                try:
                    add(
                        "PASS",
                        "Metal MPQ map-render capture package identity",
                        validate_metal_capture_package_report(
                            metal_mpq_manifest,
                            debug_package_report=debug_package_report,
                            expected_source_head=expected_source_head,
                            require_clean_source=require_clean_source,
                            label="Metal MPQ map-render capture",
                        ),
                    )
                except (OSError, EvidenceAuditError, json.JSONDecodeError) as error:
                    add("FAIL", "Metal MPQ map-render capture package identity", str(error))

        for default_manifest, mpq_manifest, backend_label in (
            (opengl_manifest, opengl_mpq_manifest, "OpenGL"),
            (metal_manifest, metal_mpq_manifest, "Metal"),
        ):
            if default_manifest is not None and mpq_manifest is not None:
                try:
                    add(
                        "PASS",
                        f"{backend_label} default/MPQ capture identity",
                        validate_capture_pair_executable_identity(
                            default_manifest,
                            mpq_manifest,
                            backend_label=backend_label,
                        ),
                    )
                except EvidenceAuditError as error:
                    add("FAIL", f"{backend_label} default/MPQ capture identity", str(error))

        if parity_report_dir is None:
            status = "WARN" if allow_missing_opengl and opengl_manifest is None else "FAIL"
            detail = (
                "missing final renderer parity report; run "
                "`tools/run_renderer_parity.py --skip-capture --pr-gate` with OpenGL and Metal artifacts"
            )
            add(status, "Renderer parity report", detail)
        else:
            try:
                parity_evidence = validate_parity_report(
                    parity_report_dir,
                    expected_source_head=expected_source_head,
                    require_clean_source=require_clean_source,
                    mpq_map_render=False,
                )
                if opengl_manifest is not None:
                    require_matching_report_capture_manifest(
                        "OpenGL",
                        submitted_manifest=opengl_manifest,
                        report_manifest=parity_evidence.opengl_manifest,
                    )
                if metal_manifest is not None:
                    require_matching_report_capture_manifest(
                        "Metal",
                        submitted_manifest=metal_manifest,
                        report_manifest=parity_evidence.metal_manifest,
                    )
                if opengl_dir is not None and metal_dir is not None:
                    validate_parity_report_matches_artifacts(
                        parity_report_dir,
                        opengl_dir,
                        metal_dir,
                        mpq_map_render=False,
                    )
                add(
                    "PASS",
                    "Renderer parity report",
                    f"{parity_report_dir} validates PR-gate provenance, submitted capture manifests, recomputed comparison metrics, and diff PNGs",
                )
            except (OSError, EvidenceAuditError, PngError, json.JSONDecodeError) as error:
                add("FAIL", "Renderer parity report", str(error))

        if mpq_parity_report_dir is None:
            status = "FAIL" if require_mpq_parity_report else "WARN"
            add(
                status,
                "MPQ map-render parity report",
                "missing MPQ export/reload map-render parity report; run "
                "`tools/run_renderer_parity.py --skip-capture --pr-gate --map-render-only "
                "--map-after-mpq-export-reload` with OpenGL and Metal artifacts",
            )
        else:
            try:
                mpq_parity_evidence = validate_parity_report(
                    mpq_parity_report_dir,
                    expected_source_head=expected_source_head,
                    require_clean_source=require_clean_source,
                    mpq_map_render=True,
                )
                if opengl_mpq_manifest is not None:
                    require_matching_report_capture_manifest(
                        "OpenGL MPQ",
                        submitted_manifest=opengl_mpq_manifest,
                        report_manifest=mpq_parity_evidence.opengl_manifest,
                    )
                if metal_mpq_manifest is not None:
                    require_matching_report_capture_manifest(
                        "Metal MPQ",
                        submitted_manifest=metal_mpq_manifest,
                        report_manifest=mpq_parity_evidence.metal_manifest,
                    )
                if opengl_mpq_dir is not None and metal_mpq_dir is not None:
                    validate_parity_report_matches_artifacts(
                        mpq_parity_report_dir,
                        opengl_mpq_dir,
                        metal_mpq_dir,
                        mpq_map_render=True,
                    )
                add(
                    "PASS",
                    "MPQ map-render parity report",
                    f"{mpq_parity_report_dir} validates PR-gate MPQ export/reload map-render provenance, submitted capture manifests, recomputed metrics, and diff PNGs",
                )
            except (OSError, EvidenceAuditError, PngError, json.JSONDecodeError) as error:
                add("FAIL", "MPQ map-render parity report", str(error))

        if skip_installed_smoke:
            add("WARN", "Installed full Metal smoke", "skipped by --skip-installed-smoke")
        elif installed_smoke_artifact_dir is None or installed_smoke_capture_frame is None:
            add(
                "FAIL",
                "Installed full Metal smoke",
                "missing --installed-smoke-artifact-dir and --installed-smoke-capture-frame evidence",
            )
        else:
            try:
                validate_full_smoke_artifacts(
                    installed_smoke_artifact_dir,
                    installed_smoke_capture_frame,
                    expected_source_head=expected_source_head,
                    require_clean_source=require_clean_source,
                )
                add(
                    "PASS",
                    "Installed full Metal smoke",
                    f"{installed_smoke_artifact_dir} validates full smoke provenance, capture frame, and required PNG artifacts",
                )
            except (OSError, SmokeError) as error:
                add("FAIL", "Installed full Metal smoke", str(error))

        if skip_installed_smoke:
            add("WARN", "Installed full Metal smoke app identity", "skipped by --skip-installed-smoke")
        elif installed_smoke_artifact_dir is None:
            add(
                "FAIL",
                "Installed full Metal smoke app identity",
                "missing --installed-smoke-artifact-dir evidence",
            )
        elif installed_smoke_app is None:
            if debug_package_report is not None:
                try:
                    add(
                        "PASS",
                        "Installed full Metal smoke app identity",
                        validate_installed_smoke_package_report(
                            installed_smoke_artifact_dir,
                            debug_package_report=debug_package_report,
                            expected_source_head=expected_source_head,
                            require_clean_source=require_clean_source,
                        ),
                    )
                except (OSError, SmokeError, EvidenceAuditError, json.JSONDecodeError) as error:
                    add("FAIL", "Installed full Metal smoke app identity", str(error))
            else:
                add(
                    "WARN" if allow_missing_opengl or allow_missing_installed_app_identity else "FAIL",
                    "Installed full Metal smoke app identity",
                    "missing --installed-smoke-app and Debug package report; cannot prove full-smoke provenance came from the installed bundle",
                )
        else:
            try:
                add(
                    "PASS",
                    "Installed full Metal smoke app identity",
                    validate_installed_smoke_app(
                        installed_smoke_artifact_dir,
                        expected_app=installed_smoke_app,
                        expected_source_head=expected_source_head,
                        require_clean_source=require_clean_source,
                    ),
                )
            except (OSError, SmokeError, EvidenceAuditError) as error:
                add("FAIL", "Installed full Metal smoke app identity", str(error))

        for label, package_report, build_type in (
            ("Debug package validation report", debug_package_report, "Debug"),
            ("Release package validation report", release_package_report, "Release"),
        ):
            if package_report is None:
                add(
                    "WARN" if allow_missing_opengl else "FAIL",
                    label,
                    f"missing {build_type} package validation report from `tools/check_macos_package.py --package-report-json`",
                )
                continue
            try:
                add(
                    "PASS",
                    label,
                    validate_package_report(
                        package_report,
                        expected_build_type=build_type,
                        expected_source_head=expected_source_head,
                        require_clean_source=require_clean_source,
                    ),
                )
            except (OSError, EvidenceAuditError, json.JSONDecodeError) as error:
                add("FAIL", label, str(error))

        if manual_review_note is None:
            add(
                "WARN" if allow_missing_opengl or allow_missing_manual_review else "FAIL",
                "Manual visual review note",
                "missing manual visual-review note for the final OpenGL/Metal parity diffs",
            )
        else:
            try:
                add(
                    "PASS",
                    "Manual visual review note",
                    validate_manual_review_note(manual_review_note, expected_source_head=expected_source_head),
                )
            except (OSError, EvidenceAuditError) as error:
                add("FAIL", "Manual visual review note", str(error))

    return checks


def read_json_object(path: Path) -> dict:
    value = json.loads(path.read_text(encoding="utf-8"))
    if not isinstance(value, dict):
        raise EvidenceAuditError(f"{path} must contain a JSON object")
    return value


def read_json_list(path: Path) -> list:
    value = json.loads(path.read_text(encoding="utf-8"))
    if not isinstance(value, list):
        raise EvidenceAuditError(f"{path} must contain a JSON list")
    return value


def require_report_flag(requested: dict, name: str, expected: bool) -> None:
    actual = bool(requested.get(name, False))
    if actual != expected:
        raise EvidenceAuditError(f"renderer parity report requested {name}={actual}, expected {expected}")


def parity_capture_head(
    label: str,
    manifest: dict,
    *,
    expected_source_head: str | None,
    require_clean_source: bool,
    mpq_map_render: bool,
) -> str:
    backend = manifest.get("reported_backend")
    expected_backend = label.lower()
    if backend != expected_backend:
        raise EvidenceAuditError(f"{label} parity provenance reports backend {backend!r}, expected {expected_backend!r}")

    source = manifest.get("source_revision")
    head = source.get("head") if isinstance(source, dict) else None
    if not isinstance(head, str) or not head:
        raise EvidenceAuditError(f"{label} parity provenance does not record a source revision head")
    if expected_source_head is not None and head != expected_source_head:
        raise EvidenceAuditError(f"{label} parity provenance source {head} does not match expected {expected_source_head}")
    if require_clean_source:
        dirty = source.get("dirty") if isinstance(source, dict) else None
        if dirty is not False:
            raise EvidenceAuditError(f"{label} parity provenance does not record a clean source tree")

    required_false_flags = ("preview_probes_only", "metal_preview_probes_only", "metal_renderer_probes_only")
    for flag in required_false_flags:
        if bool(manifest.get(flag, False)):
            raise EvidenceAuditError(f"{label} parity provenance is not the default map + preview capture: {flag}=true")
    if mpq_map_render:
        if not bool(manifest.get("map_render_only", False)):
            raise EvidenceAuditError(f"{label} MPQ parity provenance is not map-render-only")
        if bool(manifest.get("map_after_save_reload", False)):
            raise EvidenceAuditError(f"{label} MPQ parity provenance also records folder save/reload mode")
        if not bool(manifest.get("map_after_mpq_export_reload", False)):
            raise EvidenceAuditError(f"{label} MPQ parity provenance was not captured after MPQ export/reload")
    else:
        if bool(manifest.get("map_render_only", False)):
            raise EvidenceAuditError(f"{label} parity provenance is map-render-only; default PR evidence needs previews")
        if not bool(manifest.get("map_after_save_reload", False)):
            raise EvidenceAuditError(f"{label} parity provenance was not captured after save/reload")
        if bool(manifest.get("map_after_mpq_export_reload", False)):
            raise EvidenceAuditError(f"{label} parity provenance uses MPQ-export/reload mode instead of default PR mode")
    if not bool(manifest.get("smoke_picking", False)):
        raise EvidenceAuditError(f"{label} parity provenance does not prove --smoke-picking ran")
    return head


def manifest_digest(manifest: dict) -> str:
    encoded = json.dumps(manifest, sort_keys=True, separators=(",", ":")).encode("utf-8")
    return hashlib.sha256(encoded).hexdigest()


def require_matching_report_capture_manifest(label: str, *, submitted_manifest: dict, report_manifest: dict) -> None:
    if submitted_manifest == report_manifest:
        return
    submitted_source = short_source(submitted_manifest)
    report_source = short_source(report_manifest)
    raise EvidenceAuditError(
        f"renderer parity report embedded {label} capture manifest does not match the submitted {label} evidence: "
        f"submitted source {submitted_source}, digest {manifest_digest(submitted_manifest)[:12]}; "
        f"report source {report_source}, digest {manifest_digest(report_manifest)[:12]}"
    )


def compare_metric_value(artifact_name: str, field: str, expected: object, actual: object) -> None:
    if isinstance(expected, bool) or isinstance(actual, bool):
        raise EvidenceAuditError(f"renderer parity artifact {artifact_name} has invalid boolean metric {field}")
    if isinstance(expected, int) and isinstance(actual, int):
        if expected != actual:
            raise EvidenceAuditError(
                f"renderer parity artifact {artifact_name} {field}={expected!r}, recomputed {actual!r}"
            )
        return
    if not isinstance(expected, (int, float)) or not isinstance(actual, (int, float)):
        raise EvidenceAuditError(f"renderer parity artifact {artifact_name} has invalid metric {field}")
    if not math.isclose(float(expected), float(actual), rel_tol=1e-9, abs_tol=1e-6):
        raise EvidenceAuditError(
            f"renderer parity artifact {artifact_name} {field}={float(expected):.9f}, "
            f"recomputed {float(actual):.9f}"
        )


def validate_parity_report_matches_artifacts(
    report_dir: Path,
    opengl_dir: Path,
    metal_dir: Path,
    *,
    mpq_map_render: bool,
) -> None:
    expected_names = set(MAP_RENDER_ARTIFACT_SIZES) if mpq_map_render else default_pr_parity_artifact_names()
    with tempfile.TemporaryDirectory(prefix="hivewe-renderer-pr-audit-diff-") as temp_dir:
        recomputed_diff_dir = Path(temp_dir) / "diffs"
        recomputed, failed = compare_artifact_directories(
            opengl_dir,
            metal_dir,
            patterns=[],
            exact_names=expected_names,
            common_only=False,
            diff_dir=recomputed_diff_dir,
            diff_scale=4.0,
            max_rms=None,
            max_mean_abs=None,
            max_changed_percent=None,
        )
        if failed:
            raise EvidenceAuditError("submitted capture artifacts no longer produce a passing parity comparison")
        recomputed_by_name = {result["name"]: result for result in recomputed if isinstance(result.get("name"), str)}
        results = read_json_list(report_dir / "renderer-parity.json")
        for result in results:
            if not isinstance(result, dict):
                continue
            name = result.get("name")
            if not isinstance(name, str):
                continue
            actual = recomputed_by_name.get(name)
            if actual is None:
                raise EvidenceAuditError(f"renderer parity artifact {name} is not produced by the submitted captures")
            for field in (
                "status",
                "left_size",
                "right_size",
            ):
                if result.get(field) != actual.get(field):
                    raise EvidenceAuditError(
                        f"renderer parity artifact {name} {field}={result.get(field)!r}, "
                        f"recomputed {actual.get(field)!r}"
                    )
            for field in (
                "changed_pixels",
                "changed_percent",
                "mean_abs_channel_delta",
                "rms_channel_delta",
                "max_channel_delta",
            ):
                compare_metric_value(name, field, result.get(field), actual.get(field))

            report_diff_path = resolve_report_diff_path(report_dir, str(result.get("diff_path", "")), name)
            recomputed_diff_path = actual.get("diff_path")
            if not isinstance(recomputed_diff_path, str) or not Path(recomputed_diff_path).is_file():
                raise EvidenceAuditError(f"recomputed renderer parity diff PNG is missing for {name}")
            if report_diff_path.read_bytes() != Path(recomputed_diff_path).read_bytes():
                raise EvidenceAuditError(f"renderer parity diff PNG for {name} does not match the submitted captures")


def topic_present(text: str, terms: tuple[str, ...]) -> bool:
    for term in terms:
        if re.search(rf"(?<![a-z0-9]){re.escape(term)}(?![a-z0-9])", text):
            return True
    return False


def validate_manual_review_note(path: Path, *, expected_source_head: str | None) -> str:
    if not path.is_file():
        raise FileNotFoundError(f"manual visual-review note not found: {path}")
    text = path.read_text(encoding="utf-8")
    stripped = text.strip()
    if len(stripped) < 200:
        raise EvidenceAuditError(f"manual visual-review note is too short to be review evidence: {path}")
    if any(pattern.search(stripped) for pattern in MANUAL_REVIEW_PLACEHOLDER_PATTERNS):
        raise EvidenceAuditError(f"manual visual-review note still contains unresolved template placeholder(s): {path}")
    lowered = stripped.lower()
    if expected_source_head is not None:
        short_head = expected_source_head[:12]
        if expected_source_head not in stripped and short_head not in stripped:
            raise EvidenceAuditError(
                f"manual visual-review note does not reference source revision {short_head}: {path}"
            )
    missing_evidence = [
        label for label, pattern in MANUAL_REVIEW_EVIDENCE_PATTERNS if pattern.search(stripped) is None
    ]
    if missing_evidence:
        raise EvidenceAuditError(
            "manual visual-review note does not reference required evidence: " + ", ".join(missing_evidence)
        )
    if not any(pattern.search(stripped) for pattern in MANUAL_REVIEW_METRIC_PATTERNS):
        raise EvidenceAuditError(
            "manual visual-review note does not record a numeric RMS, mean abs, or changed percent metric"
        )
    missing = [label for label, terms in MANUAL_REVIEW_TOPICS if not topic_present(lowered, terms)]
    if missing:
        raise EvidenceAuditError(
            "manual visual-review note does not mention required review topic(s): " + ", ".join(missing)
        )
    return f"{path} records source/diff review coverage for {len(MANUAL_REVIEW_TOPICS)} required topic(s)"


def validate_package_report(
    path: Path,
    *,
    expected_build_type: str,
    expected_source_head: str | None,
    require_clean_source: bool,
) -> str:
    if not path.is_file():
        raise FileNotFoundError(f"{expected_build_type} package validation report not found: {path}")
    report = read_json_object(path)
    if report.get("schema") != PACKAGE_REPORT_SCHEMA or report.get("label") != PACKAGE_REPORT_LABEL:
        raise EvidenceAuditError(f"{path} has an unsupported package report schema or label")
    if report.get("status") != "success":
        raise EvidenceAuditError(f"{path} does not record a successful package validation")

    source = report.get("source_revision")
    if not isinstance(source, dict):
        raise EvidenceAuditError(f"{path} is missing source revision metadata")
    head = source.get("head")
    if not isinstance(head, str) or not head:
        raise EvidenceAuditError(f"{path} does not record a source revision head")
    if expected_source_head is not None and head != expected_source_head:
        raise EvidenceAuditError(
            f"{expected_build_type} package report source {head} does not match expected {expected_source_head}"
        )
    dirty = source.get("dirty")
    if require_clean_source and dirty is not False:
        raise EvidenceAuditError(f"{expected_build_type} package report does not record a clean source tree")

    cmake = report.get("cmake")
    build_type = cmake.get("build_type") if isinstance(cmake, dict) else None
    if build_type != expected_build_type:
        raise EvidenceAuditError(
            f"{expected_build_type} package report records build_type={build_type!r}, expected {expected_build_type!r}"
        )

    expected = report.get("expected")
    if not isinstance(expected, dict):
        raise EvidenceAuditError(f"{expected_build_type} package report is missing expected gate metadata")
    if expected.get("build_type") != expected_build_type:
        raise EvidenceAuditError(
            f"{expected_build_type} package report expected build_type={expected.get('build_type')!r}"
        )
    if expected_source_head is not None and expected.get("source_head") != expected_source_head:
        raise EvidenceAuditError(
            f"{expected_build_type} package report expected source {expected.get('source_head')!r}, "
            f"expected {expected_source_head}"
        )

    checks = report.get("checks")
    if not isinstance(checks, dict):
        raise EvidenceAuditError(f"{expected_build_type} package report is missing check results")
    missing = [name for name in REQUIRED_PACKAGE_REPORT_CHECKS if checks.get(name) is not True]
    if missing:
        raise EvidenceAuditError(
            f"{expected_build_type} package report is missing required passing check(s): " + ", ".join(missing)
        )

    artifacts = report.get("artifacts")
    if not isinstance(artifacts, dict):
        raise EvidenceAuditError(f"{expected_build_type} package report is missing artifact metadata")
    for required_artifact in ("built_dmg", "verified_dmg"):
        if not isinstance(artifacts.get(required_artifact), str) or not artifacts[required_artifact]:
            raise EvidenceAuditError(
                f"{expected_build_type} package report is missing {required_artifact} artifact metadata"
            )
    executable = report.get("app_executable")
    if not isinstance(executable, dict):
        raise EvidenceAuditError(f"{expected_build_type} package report is missing app executable metadata")
    executable_path = executable.get("path")
    executable_size = executable.get("size_bytes")
    executable_hash = executable.get("sha256")
    if not isinstance(executable_path, str) or not executable_path:
        raise EvidenceAuditError(f"{expected_build_type} package report does not record app executable path")
    if not isinstance(executable_size, int) or executable_size <= 0:
        raise EvidenceAuditError(f"{expected_build_type} package report does not record a valid app executable size")
    if not isinstance(executable_hash, str) or len(executable_hash) != 64:
        raise EvidenceAuditError(f"{expected_build_type} package report does not record app executable SHA-256")
    return f"{path} records {expected_build_type} package/DMG validation from source {head[:12]}"


def parity_metric_summary(report_dir: Path | None, *, label: str) -> dict[str, object] | None:
    if report_dir is None:
        return None
    results_path = report_dir / "renderer-parity.json"
    markdown_path = report_dir / "renderer-parity.md"
    if not results_path.is_file():
        return {
            "label": label,
            "report": str(markdown_path),
            "missing": str(results_path),
        }
    results = read_json_list(results_path)
    worst_changed = max((float(result.get("changed_percent", 0.0)) for result in results if isinstance(result, dict)), default=0.0)
    worst_mean = max((float(result.get("mean_abs_channel_delta", 0.0)) for result in results if isinstance(result, dict)), default=0.0)
    worst_rms = max((float(result.get("rms_channel_delta", 0.0)) for result in results if isinstance(result, dict)), default=0.0)
    sortable_results = [result for result in results if isinstance(result, dict)]
    top_results = sorted(
        sortable_results,
        key=lambda result: (
            float(result.get("rms_channel_delta", 0.0)),
            float(result.get("changed_percent", 0.0)),
            float(result.get("mean_abs_channel_delta", 0.0)),
        ),
        reverse=True,
    )[:5]
    return {
        "label": label,
        "report": str(markdown_path),
        "compared": len(sortable_results),
        "worst_changed": worst_changed,
        "worst_mean": worst_mean,
        "worst_rms": worst_rms,
        "top_results": top_results,
    }


def format_prefilled_metrics(summary: dict[str, object] | None) -> list[str]:
    if summary is None:
        return ["- Not available yet."]
    if "missing" in summary:
        return [
            f"- {summary['label']}: parity JSON not found at `{summary['missing']}`.",
            f"- Report path: `{summary['report']}`",
        ]
    lines = [
        f"- {summary['label']} report: `{summary['report']}`",
        f"- Compared artifacts: `{summary['compared']}`",
        f"- Worst RMS: `{float(summary['worst_rms']):.3f}`",
        f"- Worst mean abs: `{float(summary['worst_mean']):.3f}`",
        f"- Worst changed percent: `{float(summary['worst_changed']):.3f}%`",
    ]
    top_results = summary.get("top_results")
    if isinstance(top_results, list) and top_results:
        lines.append("- Highest-diff artifacts:")
        for result in top_results:
            if not isinstance(result, dict):
                continue
            lines.append(
                "  - "
                f"`{result.get('name', 'unknown')}`: "
                f"RMS `{float(result.get('rms_channel_delta', 0.0)):.3f}`, "
                f"mean `{float(result.get('mean_abs_channel_delta', 0.0)):.3f}`, "
                f"changed `{float(result.get('changed_percent', 0.0)):.3f}%`, "
                f"diff `{result.get('diff_path', '')}`"
            )
    return lines


def first_path(*paths: Path | None) -> str:
    for path in paths:
        if path is not None:
            return str(path)
    return "<not provided>"


def manual_review_template_text(
    source_head: str | None,
    *,
    parity_report_dir: Path | None = None,
    mpq_parity_report_dir: Path | None = None,
    opengl_capture: Path | None = None,
    metal_capture: Path | None = None,
    opengl_mpq_capture: Path | None = None,
    metal_mpq_capture: Path | None = None,
) -> str:
    source_line = source_head if source_head else "<source revision>"
    topic_lines = "\n".join(
        f"- [ ] {label}: TODO compare OpenGL reference and Metal candidate artifacts."
        for label, _ in MANUAL_REVIEW_TOPICS
    )
    default_metrics = "\n".join(format_prefilled_metrics(parity_metric_summary(parity_report_dir, label="Default parity")))
    mpq_metrics = "\n".join(format_prefilled_metrics(parity_metric_summary(mpq_parity_report_dir, label="MPQ export/reload parity")))
    return (
        "# HiveWE Renderer Manual Visual Review\n\n"
        f"Source revision: {source_line}\n\n"
        f"Parity report: `{parity_report_dir / 'renderer-parity.md' if parity_report_dir is not None else '<renderer-parity.md path or link>'}`\n"
        f"MPQ parity report: `{mpq_parity_report_dir / 'renderer-parity.md' if mpq_parity_report_dir is not None else '<MPQ renderer-parity.md path or link>'}`\n"
        f"OpenGL capture: `{first_path(opengl_capture)}`\n"
        f"Metal capture: `{first_path(metal_capture)}`\n"
        f"OpenGL MPQ capture: `{first_path(opengl_mpq_capture)}`\n"
        f"Metal MPQ capture: `{first_path(metal_mpq_capture)}`\n\n"
        "Replace every TODO and placeholder before submitting this note to "
        "`tools/audit_renderer_pr_evidence.py --manual-review-note`.\n\n"
        "## Diff Metrics Summary\n\n"
        "### Default Parity\n\n"
        f"{default_metrics}\n\n"
        "### MPQ Export/Reload Parity\n\n"
        f"{mpq_metrics}\n\n"
        "TODO summarize the final renderer-parity diff metrics. Record whether any nonzero diffs are expected and why.\n\n"
        "## Review Topics\n\n"
        f"{topic_lines}\n\n"
        "## Decision\n\n"
        "TODO state whether the OpenGL reference and Metal candidate are visually acceptable for maintainer review.\n"
    )


def write_manual_review_template(
    path: Path,
    *,
    source_head: str | None,
    parity_report_dir: Path | None = None,
    mpq_parity_report_dir: Path | None = None,
    opengl_capture: Path | None = None,
    metal_capture: Path | None = None,
    opengl_mpq_capture: Path | None = None,
    metal_mpq_capture: Path | None = None,
) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(
        manual_review_template_text(
            source_head,
            parity_report_dir=parity_report_dir,
            mpq_parity_report_dir=mpq_parity_report_dir,
            opengl_capture=opengl_capture,
            metal_capture=metal_capture,
            opengl_mpq_capture=opengl_mpq_capture,
            metal_mpq_capture=metal_mpq_capture,
        ),
        encoding="utf-8",
    )


def resolve_hivewe_executable(app: Path) -> Path:
    executable = app / "Contents" / "MacOS" / "HiveWE" if app.is_dir() and app.suffix == ".app" else app
    if not executable.is_file():
        raise FileNotFoundError(f"installed HiveWE executable not found: {executable}")
    return executable


def recorded_path_matches(recorded: str, expected: Path) -> bool:
    recorded_path = Path(recorded)
    if not recorded_path.is_absolute():
        recorded_path = Path.cwd() / recorded_path
    return recorded_path.resolve(strict=False) == expected.resolve(strict=False)


def read_app_build_provenance(app: Path) -> dict:
    provenance_path = app / BUILD_PROVENANCE_RELATIVE_PATH
    if not provenance_path.is_file():
        raise FileNotFoundError(f"installed app build provenance not found: {provenance_path}")
    value = json.loads(provenance_path.read_text(encoding="utf-8"))
    if not isinstance(value, dict):
        raise EvidenceAuditError(f"installed app build provenance must be a JSON object: {provenance_path}")
    return value


def validate_app_build_provenance(
    app: Path,
    *,
    expected_source_head: str | None,
    require_clean_source: bool,
) -> str:
    provenance = read_app_build_provenance(app)
    if provenance.get("schema") != 1 or provenance.get("label") != "hivewe-app-build-provenance":
        raise EvidenceAuditError("installed app build provenance has an unsupported schema or label")
    source = provenance.get("source_revision")
    if not isinstance(source, dict):
        raise EvidenceAuditError("installed app build provenance is missing source revision metadata")
    head = source.get("head")
    if not isinstance(head, str) or not head:
        raise EvidenceAuditError("installed app build provenance does not record a source revision head")
    if expected_source_head is not None and head != expected_source_head:
        raise EvidenceAuditError(
            f"installed app build provenance source {head} does not match expected {expected_source_head}"
        )
    dirty = source.get("dirty")
    if require_clean_source and dirty is not False:
        raise EvidenceAuditError("installed app build provenance does not record a clean source tree")
    return head


def validate_installed_smoke_app(
    artifact_dir: Path,
    *,
    expected_app: Path,
    expected_source_head: str | None,
    require_clean_source: bool,
) -> str:
    provenance = read_smoke_provenance(artifact_dir)
    recorded_app = provenance.get("app")
    recorded_executable = provenance.get("executable")
    if not isinstance(recorded_app, str) or not isinstance(recorded_executable, str):
        raise EvidenceAuditError("full Metal smoke provenance is missing app or executable path data")

    expected_executable = resolve_hivewe_executable(expected_app)
    if not recorded_path_matches(recorded_app, expected_app):
        raise EvidenceAuditError(
            f"full Metal smoke provenance app {recorded_app!r} does not match installed app {expected_app}"
        )
    if not recorded_path_matches(recorded_executable, expected_executable):
        raise EvidenceAuditError(
            "full Metal smoke provenance executable "
            f"{recorded_executable!r} does not match installed executable {expected_executable}"
        )

    expected_size = expected_executable.stat().st_size
    expected_sha = sha256_file(expected_executable)
    if provenance.get("executable_size_bytes") != expected_size:
        raise EvidenceAuditError(
            "full Metal smoke provenance executable size does not match installed executable: "
            f"{provenance.get('executable_size_bytes')}, expected {expected_size}"
        )
    if provenance.get("executable_sha256") != expected_sha:
        raise EvidenceAuditError("full Metal smoke provenance executable SHA-256 does not match installed executable")

    build_head = validate_app_build_provenance(
        expected_app,
        expected_source_head=expected_source_head,
        require_clean_source=require_clean_source,
    )
    smoke_source = provenance.get("source_revision")
    smoke_head = smoke_source.get("head") if isinstance(smoke_source, dict) else None
    if isinstance(smoke_head, str) and smoke_head and smoke_head != build_head:
        raise EvidenceAuditError(
            f"full Metal smoke source {smoke_head} does not match installed app build source {build_head}"
        )

    return f"{artifact_dir} provenance matches installed app {expected_app} built from {build_head[:12]}"


def validate_installed_smoke_package_report(
    artifact_dir: Path,
    *,
    debug_package_report: Path,
    expected_source_head: str | None,
    require_clean_source: bool,
) -> str:
    validate_package_report(
        debug_package_report,
        expected_build_type="Debug",
        expected_source_head=expected_source_head,
        require_clean_source=require_clean_source,
    )
    report = read_json_object(debug_package_report)
    provenance = read_smoke_provenance(artifact_dir)

    report_app = report.get("app")
    executable = report.get("app_executable")
    if not isinstance(report_app, str) or not report_app:
        raise EvidenceAuditError("Debug package report does not record installed app path")
    if not isinstance(executable, dict):
        raise EvidenceAuditError("Debug package report is missing app executable metadata")

    report_executable = executable.get("path")
    report_size = executable.get("size_bytes")
    report_hash = executable.get("sha256")
    recorded_app = provenance.get("app")
    recorded_executable = provenance.get("executable")
    if not isinstance(recorded_app, str) or not isinstance(recorded_executable, str):
        raise EvidenceAuditError("full Metal smoke provenance is missing app or executable path data")
    if not recorded_path_matches(recorded_app, Path(report_app)):
        raise EvidenceAuditError(
            f"full Metal smoke provenance app {recorded_app!r} does not match Debug package report app {report_app!r}"
        )
    if not isinstance(report_executable, str) or not recorded_path_matches(recorded_executable, Path(report_executable)):
        raise EvidenceAuditError(
            "full Metal smoke provenance executable "
            f"{recorded_executable!r} does not match Debug package report executable {report_executable!r}"
        )
    if provenance.get("executable_size_bytes") != report_size:
        raise EvidenceAuditError(
            "full Metal smoke provenance executable size does not match Debug package report: "
            f"{provenance.get('executable_size_bytes')}, expected {report_size}"
        )
    if provenance.get("executable_sha256") != report_hash:
        raise EvidenceAuditError("full Metal smoke provenance executable SHA-256 does not match Debug package report")

    report_source = report.get("source_revision")
    report_head = report_source.get("head") if isinstance(report_source, dict) else None
    smoke_source = provenance.get("source_revision")
    smoke_head = smoke_source.get("head") if isinstance(smoke_source, dict) else None
    if not isinstance(report_head, str) or not report_head:
        raise EvidenceAuditError("Debug package report does not record a source revision head")
    if isinstance(smoke_head, str) and smoke_head and smoke_head != report_head:
        raise EvidenceAuditError(
            f"full Metal smoke source {smoke_head} does not match Debug package report source {report_head}"
        )
    return f"{artifact_dir} provenance matches Debug package report executable {str(report_hash)[:12]} from {report_head[:12]}"


def validate_metal_capture_package_report(
    capture_manifest: dict,
    *,
    debug_package_report: Path,
    expected_source_head: str | None,
    require_clean_source: bool,
    label: str,
) -> str:
    validate_package_report(
        debug_package_report,
        expected_build_type="Debug",
        expected_source_head=expected_source_head,
        require_clean_source=require_clean_source,
    )
    report = read_json_object(debug_package_report)

    report_app = report.get("app")
    executable = report.get("app_executable")
    if not isinstance(report_app, str) or not report_app:
        raise EvidenceAuditError("Debug package report does not record installed app path")
    if not isinstance(executable, dict):
        raise EvidenceAuditError("Debug package report is missing app executable metadata")

    report_executable = executable.get("path")
    report_size = executable.get("size_bytes")
    report_hash = executable.get("sha256")
    captured_app = capture_manifest.get("app")
    captured_executable = capture_manifest.get("executable")
    if not isinstance(captured_app, str) or not isinstance(captured_executable, str):
        raise EvidenceAuditError(f"{label} manifest is missing app or executable path data")
    if not recorded_path_matches(captured_app, Path(report_app)):
        raise EvidenceAuditError(
            f"{label} app {captured_app!r} does not match Debug package report app {report_app!r}"
        )
    if not isinstance(report_executable, str) or not recorded_path_matches(
        captured_executable,
        Path(report_executable),
    ):
        raise EvidenceAuditError(
            f"{label} executable {captured_executable!r} does not match Debug package report executable "
            f"{report_executable!r}"
        )
    if capture_manifest.get("executable_size_bytes") != report_size:
        raise EvidenceAuditError(
            f"{label} executable size does not match Debug package report: "
            f"{capture_manifest.get('executable_size_bytes')}, expected {report_size}"
        )
    if capture_manifest.get("executable_sha256") != report_hash:
        raise EvidenceAuditError(f"{label} executable SHA-256 does not match Debug package report")

    report_source = report.get("source_revision")
    report_head = report_source.get("head") if isinstance(report_source, dict) else None
    capture_source = capture_manifest.get("source_revision")
    capture_head = capture_source.get("head") if isinstance(capture_source, dict) else None
    if not isinstance(report_head, str) or not report_head:
        raise EvidenceAuditError("Debug package report does not record a source revision head")
    if not isinstance(capture_head, str) or not capture_head:
        raise EvidenceAuditError(f"{label} manifest does not record a source revision head")
    if capture_head != report_head:
        raise EvidenceAuditError(f"{label} source {capture_head} does not match Debug package report source {report_head}")
    return f"{label} manifest matches Debug package report executable {str(report_hash)[:12]} from {report_head[:12]}"


def capture_executable_identity(manifest: dict, *, label: str) -> dict[str, object]:
    app = manifest.get("app")
    executable = manifest.get("executable")
    executable_size = manifest.get("executable_size_bytes")
    executable_hash = manifest.get("executable_sha256")
    if not isinstance(app, str) or not app:
        raise EvidenceAuditError(f"{label} manifest does not record app path")
    if not isinstance(executable, str) or not executable:
        raise EvidenceAuditError(f"{label} manifest does not record executable path")
    if not isinstance(executable_size, int) or executable_size <= 0:
        raise EvidenceAuditError(f"{label} manifest does not record a valid executable size")
    if not isinstance(executable_hash, str) or len(executable_hash) != 64:
        raise EvidenceAuditError(f"{label} manifest does not record executable SHA-256")
    return {
        "app": app,
        "executable": executable,
        "size_bytes": executable_size,
        "sha256": executable_hash,
    }


def validate_capture_pair_executable_identity(default_manifest: dict, mpq_manifest: dict, *, backend_label: str) -> str:
    default_identity = capture_executable_identity(default_manifest, label=f"{backend_label} default capture")
    mpq_identity = capture_executable_identity(mpq_manifest, label=f"{backend_label} MPQ capture")
    if not recorded_path_matches(str(mpq_identity["app"]), Path(str(default_identity["app"]))):
        raise EvidenceAuditError(
            f"{backend_label} MPQ capture app {mpq_identity['app']!r} does not match "
            f"{backend_label} default capture app {default_identity['app']!r}"
        )
    if not recorded_path_matches(str(mpq_identity["executable"]), Path(str(default_identity["executable"]))):
        raise EvidenceAuditError(
            f"{backend_label} MPQ capture executable {mpq_identity['executable']!r} does not match "
            f"{backend_label} default capture executable {default_identity['executable']!r}"
        )
    if mpq_identity["size_bytes"] != default_identity["size_bytes"]:
        raise EvidenceAuditError(
            f"{backend_label} MPQ capture executable size {mpq_identity['size_bytes']!r} does not match "
            f"{backend_label} default capture {default_identity['size_bytes']!r}"
        )
    if mpq_identity["sha256"] != default_identity["sha256"]:
        raise EvidenceAuditError(
            f"{backend_label} MPQ capture executable SHA-256 does not match {backend_label} default capture"
        )
    return (
        f"{backend_label} default and MPQ captures use the same executable "
        f"{str(default_identity['sha256'])[:12]}"
    )


def default_pr_parity_artifact_names() -> set[str]:
    return set(MAP_RENDER_ARTIFACT_SIZES) | set(SHARED_PREVIEW_ARTIFACT_SIZES)


def resolve_report_diff_path(report_dir: Path, diff_path: str, artifact_name: str) -> Path:
    raw_path = Path(diff_path)
    candidates = [raw_path] if raw_path.is_absolute() else [report_dir / raw_path, report_dir / "diffs" / raw_path.name]
    for candidate in candidates:
        if candidate.is_file():
            resolved_report_dir = report_dir.resolve()
            resolved_candidate = candidate.resolve()
            if resolved_candidate != resolved_report_dir and resolved_report_dir not in resolved_candidate.parents:
                raise EvidenceAuditError(
                    f"renderer parity diff PNG for {artifact_name} is outside the report directory: {diff_path}"
                )
            return resolved_candidate
    raise FileNotFoundError(f"renderer parity diff PNG not found for {artifact_name}: {diff_path}")


def capture_artifact_size_map(label: str, manifest: dict) -> dict[str, tuple[int, int]]:
    artifacts = manifest.get("artifacts")
    if not isinstance(artifacts, list):
        raise EvidenceAuditError(f"renderer parity {label} provenance is missing PNG artifact fingerprints")
    sizes: dict[str, tuple[int, int]] = {}
    for index, artifact in enumerate(artifacts):
        if not isinstance(artifact, dict):
            raise EvidenceAuditError(f"renderer parity {label} provenance artifact entry {index} is not an object")
        name = artifact.get("name")
        width = artifact.get("width")
        height = artifact.get("height")
        if not isinstance(name, str) or not isinstance(width, int) or not isinstance(height, int):
            raise EvidenceAuditError(f"renderer parity {label} provenance artifact entry {index} has incomplete dimensions")
        if name in sizes:
            raise EvidenceAuditError(f"renderer parity {label} provenance contains duplicate artifact fingerprint for {name}")
        sizes[name] = (width, height)
    return sizes


def require_result_size(result: dict, artifact_name: str, field: str) -> tuple[int, int]:
    value = result.get(field)
    if (
        not isinstance(value, list)
        or len(value) != 2
        or not isinstance(value[0], int)
        or not isinstance(value[1], int)
        or value[0] <= 0
        or value[1] <= 0
    ):
        raise EvidenceAuditError(f"renderer parity artifact {artifact_name} has invalid {field}: {value!r}")
    return (value[0], value[1])


def require_numeric_metric(
    result: dict,
    artifact_name: str,
    field: str,
    *,
    integer: bool = False,
    max_value: float,
) -> float:
    value = result.get(field)
    if isinstance(value, bool) or not isinstance(value, (int, float)):
        raise EvidenceAuditError(f"renderer parity artifact {artifact_name} has invalid {field}: {value!r}")
    numeric = float(value)
    if not math.isfinite(numeric) or numeric < 0.0 or numeric > max_value:
        raise EvidenceAuditError(f"renderer parity artifact {artifact_name} has out-of-range {field}: {value!r}")
    if integer and not isinstance(value, int):
        raise EvidenceAuditError(f"renderer parity artifact {artifact_name} has non-integer {field}: {value!r}")
    return numeric


def validate_parity_report(
    report_dir: Path,
    *,
    expected_source_head: str | None,
    require_clean_source: bool,
    mpq_map_render: bool,
) -> ParityReportEvidence:
    if not report_dir.is_dir():
        raise FileNotFoundError(f"renderer parity report directory not found: {report_dir}")

    results_path = report_dir / "renderer-parity.json"
    markdown_path = report_dir / "renderer-parity.md"
    provenance_path = report_dir / "renderer-parity-provenance.json"
    for path in (results_path, markdown_path, provenance_path):
        if not path.is_file():
            raise FileNotFoundError(f"missing renderer parity report file: {path}")

    results = read_json_list(results_path)
    if not results:
        raise EvidenceAuditError("renderer parity report has no comparison results")

    provenance = read_json_object(provenance_path)
    requested = provenance.get("requested")
    if not isinstance(requested, dict):
        raise EvidenceAuditError("renderer parity provenance is missing requested settings")
    require_report_flag(requested, "pr_gate", True)
    require_report_flag(requested, "same_app_determinism", False)
    require_report_flag(requested, "preview_probes_only", False)
    require_report_flag(requested, "map_render_only", mpq_map_render)
    require_report_flag(requested, "metal_preview_probes_only", False)
    require_report_flag(requested, "metal_renderer_probes_only", False)
    require_report_flag(requested, "map_after_save_reload", not mpq_map_render)
    require_report_flag(requested, "map_after_mpq_export_reload", mpq_map_render)
    require_report_flag(requested, "require_matching_source", True)
    if require_clean_source:
        require_report_flag(requested, "require_clean_source", True)
    recorded_required_head = requested.get("required_source_head")
    if expected_source_head is not None and recorded_required_head != expected_source_head:
        raise EvidenceAuditError(
            f"renderer parity report required source {recorded_required_head!r}, expected {expected_source_head!r}"
        )

    opengl_manifest = provenance.get("opengl")
    metal_manifest = provenance.get("metal")
    if not isinstance(opengl_manifest, dict) or not isinstance(metal_manifest, dict):
        raise EvidenceAuditError("renderer parity provenance is missing OpenGL or Metal capture manifests")
    opengl_head = parity_capture_head(
        "OpenGL",
        opengl_manifest,
        expected_source_head=expected_source_head,
        require_clean_source=require_clean_source,
        mpq_map_render=mpq_map_render,
    )
    metal_head = parity_capture_head(
        "Metal",
        metal_manifest,
        expected_source_head=expected_source_head,
        require_clean_source=require_clean_source,
        mpq_map_render=mpq_map_render,
    )
    if opengl_head != metal_head:
        raise EvidenceAuditError(
            f"renderer parity report compares different source revisions: OpenGL {opengl_head}, Metal {metal_head}"
        )
    opengl_sizes = capture_artifact_size_map("OpenGL", opengl_manifest)
    metal_sizes = capture_artifact_size_map("Metal", metal_manifest)

    expected_result_names = set(MAP_RENDER_ARTIFACT_SIZES) if mpq_map_render else default_pr_parity_artifact_names()
    report_label = "MPQ map-render parity report" if mpq_map_render else "renderer parity report"
    seen_result_names: set[str] = set()
    for index, result in enumerate(results):
        if not isinstance(result, dict):
            raise EvidenceAuditError(f"renderer parity result {index} is not an object")
        name = result.get("name")
        if not isinstance(name, str) or not name:
            raise EvidenceAuditError(f"renderer parity result {index} does not record an artifact name")
        if name in seen_result_names:
            raise EvidenceAuditError(f"renderer parity report contains duplicate result for {name}")
        seen_result_names.add(name)
        status = result.get("status")
        if status != "ok":
            raise EvidenceAuditError(f"renderer parity artifact {name} has status {status!r}, expected 'ok'")
        left_size = require_result_size(result, name, "left_size")
        right_size = require_result_size(result, name, "right_size")
        expected_left_size = opengl_sizes.get(name)
        expected_right_size = metal_sizes.get(name)
        if expected_left_size is None:
            raise EvidenceAuditError(f"renderer parity OpenGL provenance is missing artifact fingerprint for {name}")
        if expected_right_size is None:
            raise EvidenceAuditError(f"renderer parity Metal provenance is missing artifact fingerprint for {name}")
        if left_size != expected_left_size:
            raise EvidenceAuditError(
                f"renderer parity artifact {name} left_size {left_size[0]}x{left_size[1]} "
                f"does not match OpenGL capture manifest {expected_left_size[0]}x{expected_left_size[1]}"
            )
        if right_size != expected_right_size:
            raise EvidenceAuditError(
                f"renderer parity artifact {name} right_size {right_size[0]}x{right_size[1]} "
                f"does not match Metal capture manifest {expected_right_size[0]}x{expected_right_size[1]}"
            )
        if left_size != right_size:
            raise EvidenceAuditError(f"renderer parity artifact {name} reports mismatched left/right sizes")
        require_numeric_metric(result, name, "changed_percent", max_value=100.0)
        require_numeric_metric(result, name, "mean_abs_channel_delta", max_value=255.0)
        require_numeric_metric(result, name, "rms_channel_delta", max_value=255.0)
        require_numeric_metric(result, name, "max_channel_delta", integer=True, max_value=255.0)
        require_numeric_metric(result, name, "changed_pixels", integer=True, max_value=float(left_size[0] * left_size[1]))
        diff_path = result.get("diff_path")
        if not isinstance(diff_path, str) or not diff_path:
            raise EvidenceAuditError(f"renderer parity artifact {name} does not record a diff PNG path")
        diff_size = read_png_size(resolve_report_diff_path(report_dir, diff_path, name))
        if diff_size != left_size:
            raise EvidenceAuditError(
                f"renderer parity diff PNG for {name} is {diff_size[0]}x{diff_size[1]}, "
                f"expected {left_size[0]}x{left_size[1]}"
            )

    missing_names = sorted(expected_result_names - seen_result_names)
    extra_names = sorted(seen_result_names - expected_result_names)
    if missing_names:
        raise EvidenceAuditError(
            f"{report_label} is missing required artifact result(s): " + ", ".join(missing_names)
        )
    if extra_names:
        raise EvidenceAuditError(
            f"{report_label} contains unexpected artifact result(s): " + ", ".join(extra_names)
        )

    markdown = markdown_path.read_text(encoding="utf-8")
    if "- Result: PASS" not in markdown:
        raise EvidenceAuditError("renderer parity Markdown report does not record a PASS result")
    if "PR Gate Review Checklist" not in markdown:
        raise EvidenceAuditError("renderer parity Markdown report is missing the PR gate review checklist")

    return ParityReportEvidence(opengl_manifest=opengl_manifest, metal_manifest=metal_manifest)


def format_report(checks: list[AuditCheck]) -> str:
    fail_count = sum(1 for check in checks if check.status == "FAIL")
    warn_count = sum(1 for check in checks if check.status == "WARN")
    pass_count = sum(1 for check in checks if check.status == "PASS")
    lines = [
        "# HiveWE Metal Renderer PR Evidence Audit",
        "",
        "| Status | Check | Detail |",
        "| --- | --- | --- |",
    ]
    for check in checks:
        detail = check.detail.replace("|", "\\|").replace("\n", "<br>")
        lines.append(f"| {check.status} | {check.name} | {detail} |")
    lines.extend(
        [
            "",
            f"Result: {pass_count} passed, {warn_count} warning(s), {fail_count} failure(s).",
        ]
    )
    if fail_count:
        lines.append("PR evidence is incomplete.")
    elif warn_count:
        lines.append("PR evidence is locally valid but still has warnings to resolve before maintainer review.")
    else:
        lines.append("PR evidence has the required automated artifacts and manual visual-review note.")
    return "\n".join(lines) + "\n"


def self_test_pixels(width: int, height: int, seed: int) -> bytes:
    pixels = bytearray()
    for index in range(width * height):
        pixels.extend(((seed + index) & 0xFF, (seed * 3 + index * 5) & 0xFF, (seed * 7 + index * 11) & 0xFF, 255))
    return bytes(pixels)


def write_fake_png_artifacts(artifact_dir: Path, artifact_sizes: dict[str, tuple[int, int] | None], seed: int) -> None:
    artifact_dir.mkdir(parents=True, exist_ok=True)
    for index, (name, size) in enumerate(artifact_sizes.items()):
        width, height = size if size is not None else (24, 16)
        encode_png_rgba(artifact_dir / name, width, height, self_test_pixels(width, height, seed + index))


def write_fake_metal_app(root: Path) -> tuple[Path, Path]:
    fake_app = root / "Metal-HiveWE.app"
    fake_executable = fake_app / "Contents" / "MacOS" / "HiveWE"
    fake_executable.parent.mkdir(parents=True, exist_ok=True)
    if not fake_executable.is_file():
        fake_executable.write_text("fake\n", encoding="utf-8")
    return fake_app, fake_executable


def write_fake_capture(
    root: Path,
    *,
    backend: str,
    seed: int,
    map_render_only: bool = False,
    map_after_save_reload: bool = True,
    map_after_mpq_export_reload: bool = False,
    app: Path | None = None,
    executable: Path | None = None,
) -> Path:
    artifact_dir = root / f"{backend}-capture-{seed}"
    fake_app = app if app is not None else root / f"{backend}-HiveWE-{seed}"
    fake_executable = executable if executable is not None else fake_app
    if not fake_executable.exists():
        fake_executable.parent.mkdir(parents=True, exist_ok=True)
        fake_executable.write_text("fake\n", encoding="utf-8")
    artifact_sizes = MAP_RENDER_ARTIFACT_SIZES if map_render_only else {**MAP_RENDER_ARTIFACT_SIZES, **SHARED_PREVIEW_ARTIFACT_SIZES}
    write_fake_png_artifacts(artifact_dir, artifact_sizes, seed)
    commands = [["fake", "--smoke-render-modes", "--smoke-picking"]]
    if map_after_save_reload:
        commands[0].insert(1, "--smoke-save-reload")
    if map_after_mpq_export_reload:
        commands[0].insert(1, "--smoke-mpq-export-reload")
    if not map_render_only:
        commands.append(["fake", "--smoke-model-previews"])
    write_capture_manifest(
        label=backend.title(),
        expected_backend=backend,
        reported_backend=backend,
        app=fake_app,
        executable=fake_executable,
        artifact_dir=artifact_dir,
        map_path=Path("data/test map"),
        warcraft_dir=Path("/tmp/Warcraft III"),
        capture_delay_ms=1000,
        preview_probes_only=False,
        map_render_only=map_render_only,
        metal_preview_probes_only=False,
        metal_renderer_probes_only=False,
        map_after_save_reload=map_after_save_reload,
        map_after_mpq_export_reload=map_after_mpq_export_reload,
        commands=commands,
    )
    manifest_path = artifact_dir / CAPTURE_MANIFEST_NAME
    manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    manifest["source_revision"] = {
        "root": str(root),
        "head": "1" * 40,
        "branch": "self-test",
        "dirty": False,
        "status_short": [],
    }
    manifest_path.write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
    return artifact_dir


def write_fake_full_smoke(root: Path, seed: int) -> tuple[Path, Path]:
    artifact_dir = root / "full-metal-smoke"
    capture_frame = root / "full-metal-smoke.png"
    fake_app, fake_executable = write_fake_metal_app(root)
    build_provenance_path = fake_app / BUILD_PROVENANCE_RELATIVE_PATH
    build_provenance_path.parent.mkdir(parents=True, exist_ok=True)
    build_provenance_path.write_text(
        json.dumps(
            {
                "schema": 1,
                "label": "hivewe-app-build-provenance",
                "source_revision": {
                    "root": str(root),
                    "head": "1" * 40,
                    "branch": "self-test",
                    "dirty": False,
                    "status_short": "",
                },
                "cmake": {"build_type": "Debug", "enable_metal": "1", "build_opengl_renderer": "0"},
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )
    encode_png_rgba(capture_frame, 24, 16, self_test_pixels(24, 16, seed))
    write_fake_png_artifacts(artifact_dir, FULL_METAL_SMOKE_ARTIFACT_SIZES, seed + 20)
    provenance_path = write_smoke_provenance(
        app=fake_app,
        executable=fake_executable,
        reported_backend="metal",
        artifact_dir=artifact_dir,
        capture_frame=capture_frame,
        map_path=Path("data/test map"),
        warcraft_dir=Path("/tmp/Warcraft III"),
        capture_delay_ms=1000,
        command=build_full_smoke_command(
            executable=fake_executable,
            map_path=Path("data/test map"),
            warcraft_dir=Path("/tmp/Warcraft III"),
            capture_frame=capture_frame,
            artifact_dir=artifact_dir,
            capture_delay_ms=1000,
        ),
    )
    provenance = json.loads(provenance_path.read_text(encoding="utf-8"))
    provenance["source_revision"] = {
        "root": str(root),
        "head": "1" * 40,
        "branch": "self-test",
        "dirty": False,
        "status_short": [],
    }
    provenance_path.write_text(json.dumps(provenance, indent=2) + "\n", encoding="utf-8")
    return artifact_dir, capture_frame


def write_fake_manual_review_note(root: Path) -> Path:
    note = root / "manual-visual-review.md"
    note.write_text(
        "# Manual Visual Review\n\n"
        "Source revision: 1111111111111111111111111111111111111111\n\n"
        "Parity report: /tmp/hivewe-renderer-parity/renderer-parity.md\n"
        "OpenGL capture: /tmp/hivewe-opengl-artifacts.zip\n"
        "Metal capture: /tmp/hivewe-metal-artifacts.zip\n\n"
        "Reviewed renderer-parity diff metrics: RMS 0.00, changed percent 0.00%, and mean abs 0.00. "
        "Terrain, cliffs, water, SD materials, HD materials, team color behavior, transparency and alpha, "
        "selection overlays, picking results, particles including PE1, PE2, and ribbon content, model editor previews, "
        "object grid previews, save/reload parity, post-save/reload map frames, and MPQ export/reload map frames "
        "were inspected against the OpenGL reference and Metal candidate artifacts.\n",
        encoding="utf-8",
    )
    return note


def write_fake_package_report(root: Path, build_type: str) -> Path:
    report = root / f"{build_type.lower()}-package-report.json"
    fake_app, fake_executable = write_fake_metal_app(root)
    report.write_text(
        json.dumps(
            {
                "schema": PACKAGE_REPORT_SCHEMA,
                "label": PACKAGE_REPORT_LABEL,
                "status": "success",
                "validated_at_utc": "2026-05-31T00:00:00+00:00",
                "repo_root": str(root),
                "build_dir": f"build/macOS-{build_type}",
                "prefix": f"build/macOS-{build_type}/install",
                "app": str(fake_app),
                "app_executable": {
                    "path": str(fake_executable),
                    "size_bytes": fake_executable.stat().st_size,
                    "sha256": sha256_file(fake_executable),
                },
                "source_revision": {
                    "root": str(root),
                    "head": "1" * 40,
                    "branch": "self-test",
                    "dirty": False,
                    "status_short": "",
                },
                "cmake": {
                    "build_type": build_type,
                    "enable_metal": "1",
                    "build_opengl_renderer": "0",
                },
                "expected": {
                    "source_head": "1" * 40,
                    "require_current_source": True,
                    "require_clean_source": True,
                    "build_type": build_type,
                    "backend": "metal",
                },
                "checks": {name: True for name in REQUIRED_PACKAGE_REPORT_CHECKS},
                "artifacts": {
                    "built_dmg": str(root / f"HiveWE-{build_type}.dmg"),
                    "verified_dmg": str(root / f"HiveWE-{build_type}.dmg"),
                },
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )
    return report


def write_fake_parity_report(
    root: Path,
    opengl_dir: Path,
    metal_dir: Path,
    seed: int,
    *,
    result_names: list[str] | None = None,
    mpq_map_render: bool = False,
) -> Path:
    report_dir = root / f"renderer-parity-report-{seed}"
    diff_dir = report_dir / "diffs"
    diff_dir.mkdir(parents=True)
    names = result_names if result_names is not None else sorted(
        set(MAP_RENDER_ARTIFACT_SIZES) if mpq_map_render else default_pr_parity_artifact_names()
    )
    results, failed = compare_artifact_directories(
        opengl_dir,
        metal_dir,
        patterns=[],
        exact_names=set(names),
        common_only=False,
        diff_dir=diff_dir,
        diff_scale=4.0,
        max_rms=None,
        max_mean_abs=None,
        max_changed_percent=None,
    )
    if failed:
        raise AssertionError("fake parity report generation should produce passing comparison rows")

    opengl_manifest = json.loads((opengl_dir / CAPTURE_MANIFEST_NAME).read_text(encoding="utf-8"))
    metal_manifest = json.loads((metal_dir / CAPTURE_MANIFEST_NAME).read_text(encoding="utf-8"))
    opengl_source = opengl_manifest.get("source_revision")
    required_source_head = opengl_source.get("head") if isinstance(opengl_source, dict) else None
    (report_dir / "renderer-parity.json").write_text(json.dumps(results, indent=2) + "\n", encoding="utf-8")
    (report_dir / "renderer-parity-provenance.json").write_text(
        json.dumps(
            {
                "requested": {
                    "pr_gate": True,
                    "preview_probes_only": False,
                    "map_render_only": mpq_map_render,
                    "metal_preview_probes_only": False,
                    "metal_renderer_probes_only": False,
                    "map_after_save_reload": not mpq_map_render,
                    "map_after_mpq_export_reload": mpq_map_render,
                    "same_app_determinism": False,
                    "map": "data/test map",
                    "required_source_head": required_source_head,
                    "require_matching_source": True,
                    "require_clean_source": False,
                    "opengl_artifact_archive": None,
                    "metal_artifact_archive": None,
                },
                "opengl": opengl_manifest,
                "metal": metal_manifest,
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )
    (report_dir / "renderer-parity.md").write_text(
        "# HiveWE Renderer Parity Report\n\n"
        "- Result: PASS\n"
        "- PR gate: enabled\n\n"
        "## PR Gate Review Checklist\n\n"
        "- Review nonzero diffs before treating the report as PR-ready evidence.\n",
        encoding="utf-8",
    )
    return report_dir


def require_self_test(condition: bool, message: str) -> None:
    if not condition:
        raise AssertionError(message)


def has_status(checks: list[AuditCheck], status: str, name: str) -> bool:
    return any(check.status == status and check.name == name for check in checks)


def run_self_test() -> int:
    with tempfile.TemporaryDirectory(prefix="hivewe-renderer-pr-evidence-self-test-") as temp_dir:
        root = Path(temp_dir)
        packaged_metal_app, packaged_metal_executable = write_fake_metal_app(root)
        metal_dir = write_fake_capture(
            root,
            backend="metal",
            seed=10,
            app=packaged_metal_app,
            executable=packaged_metal_executable,
        )
        opengl_app = root / "OpenGL-HiveWE"
        opengl_app.write_text("fake opengl\n", encoding="utf-8")
        opengl_dir = write_fake_capture(root, backend="opengl", seed=40, app=opengl_app, executable=opengl_app)
        parity_report_dir = write_fake_parity_report(root, opengl_dir, metal_dir, seed=60)
        mpq_metal_dir = write_fake_capture(
            root,
            backend="metal",
            seed=65,
            map_render_only=True,
            map_after_save_reload=False,
            map_after_mpq_export_reload=True,
            app=packaged_metal_app,
            executable=packaged_metal_executable,
        )
        mpq_opengl_dir = write_fake_capture(
            root,
            backend="opengl",
            seed=66,
            map_render_only=True,
            map_after_save_reload=False,
            map_after_mpq_export_reload=True,
            app=opengl_app,
            executable=opengl_app,
        )
        mpq_parity_report_dir = write_fake_parity_report(
            root,
            mpq_opengl_dir,
            mpq_metal_dir,
            seed=67,
            mpq_map_render=True,
        )
        smoke_dir, capture_frame = write_fake_full_smoke(root, seed=80)
        installed_smoke_app = root / "Metal-HiveWE.app"
        manual_review_note = write_fake_manual_review_note(root)
        debug_package_report = write_fake_package_report(root, "Debug")
        release_package_report = write_fake_package_report(root, "Release")

        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            metal_mpq_artifact_dir=mpq_metal_dir,
            metal_mpq_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            opengl_mpq_artifact_dir=mpq_opengl_dir,
            opengl_mpq_artifact_archive=None,
            parity_report_dir=parity_report_dir,
            mpq_parity_report_dir=mpq_parity_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=True,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
            debug_package_report=debug_package_report,
            release_package_report=release_package_report,
        )
        require_self_test(not any(check.status == "FAIL" for check in checks), "complete fake evidence should pass")
        require_self_test(has_status(checks, "PASS", "OpenGL/Metal pairing"), "complete evidence should pair captures")
        require_self_test(has_status(checks, "PASS", "Renderer parity report"), "complete evidence should include parity report")
        require_self_test(
            has_status(checks, "PASS", "MPQ map-render parity report"),
            "complete evidence should include MPQ map-render parity when required and provided",
        )
        require_self_test(
            has_status(checks, "PASS", "Metal MPQ map-render capture"),
            "complete evidence should validate Metal MPQ map-render capture",
        )
        require_self_test(
            has_status(checks, "PASS", "OpenGL/Metal MPQ pairing"),
            "complete evidence should pair MPQ map-render captures",
        )
        require_self_test(
            has_status(checks, "PASS", "Metal renderer capture package identity"),
            "complete evidence should prove the Metal parity capture came from the packaged Debug app",
        )
        require_self_test(
            has_status(checks, "PASS", "Metal MPQ map-render capture package identity"),
            "complete evidence should prove the Metal MPQ capture came from the packaged Debug app",
        )
        require_self_test(
            has_status(checks, "PASS", "OpenGL default/MPQ capture identity"),
            "complete evidence should prove OpenGL default and MPQ captures came from one executable",
        )
        require_self_test(
            has_status(checks, "PASS", "Metal default/MPQ capture identity"),
            "complete evidence should prove Metal default and MPQ captures came from one executable",
        )

        other_opengl_app = root / "Other-OpenGL-HiveWE"
        other_opengl_app.write_text("other fake opengl\n", encoding="utf-8")
        wrong_opengl_mpq_dir = write_fake_capture(
            root,
            backend="opengl",
            seed=68,
            map_render_only=True,
            map_after_save_reload=False,
            map_after_mpq_export_reload=True,
            app=other_opengl_app,
            executable=other_opengl_app,
        )
        wrong_opengl_mpq_parity_report_dir = write_fake_parity_report(
            root,
            wrong_opengl_mpq_dir,
            mpq_metal_dir,
            seed=69,
            mpq_map_render=True,
        )
        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            metal_mpq_artifact_dir=mpq_metal_dir,
            metal_mpq_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            opengl_mpq_artifact_dir=wrong_opengl_mpq_dir,
            opengl_mpq_artifact_archive=None,
            parity_report_dir=parity_report_dir,
            mpq_parity_report_dir=wrong_opengl_mpq_parity_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=True,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
            debug_package_report=debug_package_report,
            release_package_report=release_package_report,
        )
        require_self_test(
            has_status(checks, "FAIL", "OpenGL default/MPQ capture identity"),
            "final audit should reject OpenGL default and MPQ captures from different executables",
        )

        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            metal_mpq_artifact_dir=mpq_metal_dir,
            metal_mpq_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            opengl_mpq_artifact_dir=mpq_opengl_dir,
            opengl_mpq_artifact_archive=None,
            parity_report_dir=parity_report_dir,
            mpq_parity_report_dir=None,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=True,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
            debug_package_report=debug_package_report,
            release_package_report=release_package_report,
        )
        require_self_test(
            has_status(checks, "FAIL", "MPQ map-render parity report"),
            "required MPQ map-render parity should fail when the MPQ report is missing",
        )

        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=parity_report_dir,
            mpq_parity_report_dir=mpq_parity_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=True,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
            debug_package_report=debug_package_report,
            release_package_report=release_package_report,
        )
        require_self_test(
            has_status(checks, "FAIL", "Metal MPQ map-render capture"),
            "required MPQ parity should fail when the submitted Metal MPQ capture is missing",
        )
        require_self_test(
            has_status(checks, "FAIL", "OpenGL MPQ map-render capture"),
            "required MPQ parity should fail when the submitted OpenGL MPQ capture is missing",
        )

        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=parity_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
            required_source_head="1" * 40,
            debug_package_report=debug_package_report,
            release_package_report=release_package_report,
        )
        require_self_test(
            not any(check.status == "FAIL" for check in checks),
            "complete fake evidence should pass explicit source-head validation",
        )

        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=parity_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
            required_source_head="2" * 40,
        )
        require_self_test(
            has_status(checks, "FAIL", "Metal renderer capture"),
            "explicit source-head validation should reject stale renderer captures",
        )

        try:
            audit_evidence(
                metal_artifact_dir=metal_dir,
                metal_artifact_archive=None,
                opengl_artifact_dir=opengl_dir,
                opengl_artifact_archive=None,
                parity_report_dir=parity_report_dir,
                installed_smoke_artifact_dir=smoke_dir,
                installed_smoke_capture_frame=capture_frame,
                installed_smoke_app=installed_smoke_app,
                manual_review_note=manual_review_note,
                require_current_source=True,
                require_clean_source=False,
                require_mpq_parity_report=False,
                allow_missing_opengl=False,
                allow_missing_manual_review=False,
                allow_missing_installed_app_identity=False,
                skip_installed_smoke=False,
                required_source_head="1" * 40,
            )
            raise AssertionError("explicit source-head and current-source validation should be mutually exclusive")
        except EvidenceAuditError:
            pass

        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=None,
            opengl_artifact_archive=None,
            parity_report_dir=None,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=None,
            manual_review_note=None,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=True,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
        )
        require_self_test(has_status(checks, "WARN", "OpenGL renderer capture"), "missing OpenGL can be a local warning")
        require_self_test(has_status(checks, "WARN", "Renderer parity report"), "missing OpenGL can warn for missing final report")
        require_self_test(has_status(checks, "WARN", "Manual visual review note"), "local audits can warn for missing manual review")

        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=None,
            opengl_artifact_archive=None,
            parity_report_dir=None,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
        )
        require_self_test(has_status(checks, "FAIL", "OpenGL renderer capture"), "missing OpenGL should fail final audit")

        mismatched_opengl_dir = write_fake_capture(root, backend="opengl", seed=120)
        manifest_path = mismatched_opengl_dir / CAPTURE_MANIFEST_NAME
        manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
        manifest["source_revision"]["head"] = "0" * 40
        manifest_path.write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=mismatched_opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=parity_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
        )
        require_self_test(has_status(checks, "FAIL", "OpenGL/Metal pairing"), "mismatched source heads should fail")

        initial_metal_dir = write_fake_capture(root, backend="metal", seed=160, map_after_save_reload=False)
        checks = audit_evidence(
            metal_artifact_dir=initial_metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=parity_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
        )
        require_self_test(has_status(checks, "FAIL", "Metal renderer capture"), "initial map captures should fail PR audit")

        broken_report_dir = write_fake_parity_report(root, opengl_dir, metal_dir, seed=200)
        broken_results = json.loads((broken_report_dir / "renderer-parity.json").read_text(encoding="utf-8"))
        broken_results[0]["status"] = "threshold-failed"
        (broken_report_dir / "renderer-parity.json").write_text(json.dumps(broken_results, indent=2) + "\n", encoding="utf-8")
        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=broken_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
        )
        require_self_test(has_status(checks, "FAIL", "Renderer parity report"), "failed parity result should fail audit")

        truncated_report_dir = write_fake_parity_report(
            root,
            opengl_dir,
            metal_dir,
            seed=220,
            result_names=["map-render-baseline.png"],
        )
        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=truncated_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
        )
        require_self_test(
            has_status(checks, "FAIL", "Renderer parity report"),
            "truncated parity report should fail audit",
        )

        metricless_report_dir = write_fake_parity_report(root, opengl_dir, metal_dir, seed=225)
        metricless_results_path = metricless_report_dir / "renderer-parity.json"
        metricless_results = json.loads(metricless_results_path.read_text(encoding="utf-8"))
        del metricless_results[0]["rms_channel_delta"]
        metricless_results_path.write_text(json.dumps(metricless_results, indent=2) + "\n", encoding="utf-8")
        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=metricless_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
        )
        require_self_test(
            has_status(checks, "FAIL", "Renderer parity report"),
            "parity reports without numeric metrics should fail audit",
        )

        stale_metric_report_dir = write_fake_parity_report(root, opengl_dir, metal_dir, seed=226)
        stale_metric_results_path = stale_metric_report_dir / "renderer-parity.json"
        stale_metric_results = json.loads(stale_metric_results_path.read_text(encoding="utf-8"))
        stale_metric_results[0]["changed_pixels"] = int(stale_metric_results[0]["changed_pixels"]) + 1
        stale_metric_results_path.write_text(json.dumps(stale_metric_results, indent=2) + "\n", encoding="utf-8")
        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=stale_metric_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
        )
        require_self_test(
            has_status(checks, "FAIL", "Renderer parity report"),
            "parity reports with metrics that do not match submitted PNGs should fail audit",
        )

        stale_diff_report_dir = write_fake_parity_report(root, opengl_dir, metal_dir, seed=227)
        stale_diff_results = json.loads((stale_diff_report_dir / "renderer-parity.json").read_text(encoding="utf-8"))
        stale_diff_path = Path(stale_diff_results[0]["diff_path"])
        stale_width, stale_height = read_png_size(stale_diff_path)
        encode_png_rgba(stale_diff_path, stale_width, stale_height, self_test_pixels(stale_width, stale_height, 228))
        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=stale_diff_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
        )
        require_self_test(
            has_status(checks, "FAIL", "Renderer parity report"),
            "parity reports with stale diff PNGs should fail audit",
        )

        wrong_diff_size_report_dir = write_fake_parity_report(root, opengl_dir, metal_dir, seed=229)
        wrong_diff_results = json.loads((wrong_diff_size_report_dir / "renderer-parity.json").read_text(encoding="utf-8"))
        wrong_diff_path = Path(wrong_diff_results[0]["diff_path"])
        encode_png_rgba(wrong_diff_path, 8, 8, self_test_pixels(8, 8, 230))
        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=wrong_diff_size_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
        )
        require_self_test(
            has_status(checks, "FAIL", "Renderer parity report"),
            "parity reports with wrong-sized diff PNGs should fail audit",
        )

        outside_diff_report_dir = write_fake_parity_report(root, opengl_dir, metal_dir, seed=230)
        outside_diff = root / "outside.diff.png"
        encode_png_rgba(outside_diff, 8, 8, self_test_pixels(8, 8, 231))
        outside_diff_results_path = outside_diff_report_dir / "renderer-parity.json"
        outside_diff_results = json.loads(outside_diff_results_path.read_text(encoding="utf-8"))
        outside_diff_results[0]["diff_path"] = str(outside_diff)
        outside_diff_results_path.write_text(json.dumps(outside_diff_results, indent=2) + "\n", encoding="utf-8")
        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=outside_diff_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
        )
        require_self_test(
            has_status(checks, "FAIL", "Renderer parity report"),
            "diff PNGs outside the parity report directory should fail audit",
        )

        stale_metal_dir = write_fake_capture(root, backend="metal", seed=240)
        stale_report_dir = write_fake_parity_report(root, opengl_dir, stale_metal_dir, seed=260)
        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=stale_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
        )
        require_self_test(
            has_status(checks, "FAIL", "Renderer parity report"),
            "parity report generated from different capture manifests should fail audit",
        )

        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=parity_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=None,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
        )
        require_self_test(
            has_status(checks, "FAIL", "Manual visual review note"),
            "final audit should require a manual visual-review note",
        )

        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=parity_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=None,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=True,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
            debug_package_report=debug_package_report,
            release_package_report=release_package_report,
        )
        require_self_test(
            has_status(checks, "WARN", "Manual visual review note"),
            "automated audits can warn for a missing human manual visual-review note",
        )
        require_self_test(
            not any(check.status == "FAIL" for check in checks),
            "manual-review warning mode should not fail otherwise complete evidence",
        )

        template_note = root / "manual-review-template.md"
        write_manual_review_template(
            template_note,
            source_head="1" * 40,
            parity_report_dir=parity_report_dir,
            mpq_parity_report_dir=mpq_parity_report_dir,
            opengl_capture=root / "opengl-artifacts.zip",
            metal_capture=root / "metal-artifacts.zip",
            opengl_mpq_capture=root / "opengl-mpq-artifacts.zip",
            metal_mpq_capture=root / "metal-mpq-artifacts.zip",
        )
        template_text = template_note.read_text(encoding="utf-8")
        require_self_test("Default parity report:" in template_text, "manual review template should include default parity metrics")
        require_self_test("MPQ export/reload parity report:" in template_text, "manual review template should include MPQ parity metrics")
        require_self_test("opengl-artifacts.zip" in template_text, "manual review template should include OpenGL evidence path")
        require_self_test("Worst RMS:" in template_text, "manual review template should prefill numeric diff metrics")
        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=parity_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=template_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
        )
        require_self_test(
            has_status(checks, "FAIL", "Manual visual review note"),
            "unfilled manual review template should fail audit",
        )

        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=parity_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
            debug_package_report=None,
            release_package_report=release_package_report,
        )
        require_self_test(
            has_status(checks, "FAIL", "Debug package validation report"),
            "final audit should require Debug package validation evidence",
        )

        stale_release_report = write_fake_package_report(root, "Release")
        stale_release_value = read_json_object(stale_release_report)
        stale_release_value["cmake"]["build_type"] = "Debug"
        stale_release_report.write_text(json.dumps(stale_release_value, indent=2) + "\n", encoding="utf-8")
        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=parity_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
            debug_package_report=debug_package_report,
            release_package_report=stale_release_report,
        )
        require_self_test(
            has_status(checks, "FAIL", "Release package validation report"),
            "final audit should reject package reports with the wrong build type",
        )

        generic_note = root / "generic-manual-review.md"
        generic_note.write_text(
            "# Manual Visual Review\n\n"
            "Source revision: 1111111111111111111111111111111111111111\n\n"
            "Reviewed renderer-parity diff metrics across terrain, cliffs, water, SD materials, HD materials, "
            "team color behavior, transparency and alpha, selection overlays, picking results, particles including "
            "PE1, PE2, and ribbon content, model editor previews, object grid previews, save/reload parity, "
            "post-save/reload map frames, and MPQ export/reload map frames. The OpenGL reference and Metal "
            "candidate artifacts were visually acceptable.\n",
            encoding="utf-8",
        )
        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=parity_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=generic_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
        )
        require_self_test(
            has_status(checks, "FAIL", "Manual visual review note"),
            "generic manual review prose without concrete evidence and numeric metrics should fail audit",
        )

        stale_build_provenance = read_app_build_provenance(installed_smoke_app)
        stale_build_source = stale_build_provenance["source_revision"]
        stale_build_source["head"] = "2" * 40
        (installed_smoke_app / BUILD_PROVENANCE_RELATIVE_PATH).write_text(
            json.dumps(stale_build_provenance, indent=2) + "\n",
            encoding="utf-8",
        )
        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=parity_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=installed_smoke_app,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
        )
        require_self_test(
            has_status(checks, "FAIL", "Installed full Metal smoke app identity"),
            "final audit should reject app build provenance from a different source",
        )

        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=parity_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=None,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
        )
        require_self_test(
            has_status(checks, "FAIL", "Installed full Metal smoke app identity"),
            "final audit should require installed app identity for full-smoke provenance",
        )

        release_package_report = write_fake_package_report(root, "Release")
        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=parity_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=None,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
            debug_package_report=debug_package_report,
            release_package_report=release_package_report,
        )
        require_self_test(
            has_status(checks, "PASS", "Installed full Metal smoke app identity"),
            "cross-job audits should prove installed app identity from the Debug package report",
        )
        require_self_test(
            not any(check.status == "FAIL" for check in checks),
            "Debug package-report identity mode should not fail otherwise complete evidence",
        )

        unpackaged_metal_dir = write_fake_capture(root, backend="metal", seed=125)
        unpackaged_parity_report_dir = write_fake_parity_report(root, opengl_dir, unpackaged_metal_dir, seed=126)
        checks = audit_evidence(
            metal_artifact_dir=unpackaged_metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=unpackaged_parity_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=None,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
            debug_package_report=debug_package_report,
            release_package_report=release_package_report,
        )
        require_self_test(
            has_status(checks, "FAIL", "Metal renderer capture package identity"),
            "final audit should reject Metal parity captures that were not made from the packaged Debug app",
        )

        unpackaged_mpq_metal_dir = write_fake_capture(
            root,
            backend="metal",
            seed=127,
            map_render_only=True,
            map_after_save_reload=False,
            map_after_mpq_export_reload=True,
        )
        unpackaged_mpq_parity_report_dir = write_fake_parity_report(
            root,
            mpq_opengl_dir,
            unpackaged_mpq_metal_dir,
            seed=128,
            mpq_map_render=True,
        )
        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            metal_mpq_artifact_dir=unpackaged_mpq_metal_dir,
            metal_mpq_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            opengl_mpq_artifact_dir=mpq_opengl_dir,
            opengl_mpq_artifact_archive=None,
            parity_report_dir=parity_report_dir,
            mpq_parity_report_dir=unpackaged_mpq_parity_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=None,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=True,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
            debug_package_report=debug_package_report,
            release_package_report=release_package_report,
        )
        require_self_test(
            has_status(checks, "FAIL", "Metal MPQ map-render capture package identity"),
            "final audit should reject Metal MPQ captures that were not made from the packaged Debug app",
        )

        mismatched_identity_report_root = root / "mismatched-identity-package-reports"
        mismatched_identity_report_root.mkdir()
        mismatched_identity_debug_report = write_fake_package_report(mismatched_identity_report_root, "Debug")
        checks = audit_evidence(
            metal_artifact_dir=metal_dir,
            metal_artifact_archive=None,
            opengl_artifact_dir=opengl_dir,
            opengl_artifact_archive=None,
            parity_report_dir=parity_report_dir,
            installed_smoke_artifact_dir=smoke_dir,
            installed_smoke_capture_frame=capture_frame,
            installed_smoke_app=None,
            manual_review_note=manual_review_note,
            require_current_source=False,
            require_clean_source=False,
            require_mpq_parity_report=False,
            allow_missing_opengl=False,
            allow_missing_manual_review=False,
            allow_missing_installed_app_identity=False,
            skip_installed_smoke=False,
            debug_package_report=mismatched_identity_debug_report,
            release_package_report=release_package_report,
        )
        require_self_test(
            has_status(checks, "FAIL", "Installed full Metal smoke app identity"),
            "cross-job audits should reject a Debug package report for a different executable",
        )

    print("audit_renderer_pr_evidence self-test passed.")
    return 0


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Audit HiveWE Metal renderer PR evidence artifacts.")
    parser.add_argument("--metal-artifact-dir", type=Path, help="Metal renderer capture directory.")
    parser.add_argument("--metal-artifact-archive", type=Path, help="Packaged Metal renderer capture zip.")
    parser.add_argument("--metal-mpq-artifact-dir", type=Path, help="Metal MPQ export/reload map-render capture directory.")
    parser.add_argument("--metal-mpq-artifact-archive", type=Path, help="Packaged Metal MPQ export/reload map-render capture zip.")
    parser.add_argument("--opengl-artifact-dir", type=Path, help="OpenGL renderer reference capture directory.")
    parser.add_argument("--opengl-artifact-archive", type=Path, help="Packaged OpenGL renderer reference capture zip.")
    parser.add_argument("--opengl-mpq-artifact-dir", type=Path, help="OpenGL MPQ export/reload map-render capture directory.")
    parser.add_argument("--opengl-mpq-artifact-archive", type=Path, help="Packaged OpenGL MPQ export/reload map-render capture zip.")
    parser.add_argument("--parity-report-dir", type=Path, help="Final renderer parity report directory.")
    parser.add_argument(
        "--mpq-parity-report-dir",
        type=Path,
        help="Optional MPQ export/reload map-render parity report directory.",
    )
    parser.add_argument("--installed-smoke-artifact-dir", type=Path, help="Installed full Metal smoke artifact directory.")
    parser.add_argument("--installed-smoke-capture-frame", type=Path, help="Installed full Metal smoke map-frame PNG.")
    parser.add_argument(
        "--installed-smoke-app",
        type=Path,
        help="Installed HiveWE.app used for the full Metal smoke provenance.",
    )
    parser.add_argument(
        "--debug-package-report-json",
        type=Path,
        help="Debug package validation JSON report from tools/check_macos_package.py.",
    )
    parser.add_argument(
        "--release-package-report-json",
        type=Path,
        help="Release package validation JSON report from tools/check_macos_package.py.",
    )
    parser.add_argument(
        "--manual-review-note",
        type=Path,
        help="Markdown/text note recording manual review of the final OpenGL/Metal parity diffs.",
    )
    parser.add_argument(
        "--write-manual-review-template",
        type=Path,
        help="Write a manual visual-review note template and exit.",
    )
    parser.add_argument("--require-current-source", action="store_true", help="Require evidence source revisions to match git HEAD.")
    parser.add_argument(
        "--require-source-head",
        metavar="SHA",
        help="Require evidence source revisions to match this exact source revision.",
    )
    parser.add_argument("--require-clean-source", action="store_true", help="Require evidence manifests to record clean source trees.")
    parser.add_argument(
        "--require-mpq-parity-report",
        action="store_true",
        help=(
            "Fail when --mpq-parity-report-dir or submitted MPQ capture bundles are missing instead of treating "
            "MPQ parity as a warning."
        ),
    )
    parser.add_argument(
        "--allow-missing-opengl",
        action="store_true",
        help="Treat the missing OpenGL reference as a warning for local Metal-only audits.",
    )
    parser.add_argument(
        "--allow-missing-manual-review",
        action="store_true",
        help="Treat a missing manual visual-review note as a warning for automated pre-review audits.",
    )
    parser.add_argument(
        "--allow-missing-installed-app-identity",
        action="store_true",
        help="Treat missing installed app bundle identity evidence as a warning when no Debug package report is available.",
    )
    parser.add_argument("--skip-installed-smoke", action="store_true", help="Do not require installed full Metal smoke evidence.")
    parser.add_argument("--markdown-output", type=Path, help="Write the audit report to this Markdown file.")
    parser.add_argument("--self-test", action="store_true", help="Run dependency-free auditor self-tests and exit.")
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    if args.self_test:
        return run_self_test()
    if args.require_source_head and args.require_current_source:
        print(
            "Renderer PR evidence audit failed: --require-source-head and --require-current-source are mutually exclusive",
            file=sys.stderr,
        )
        return 1
    if args.write_manual_review_template is not None:
        source = source_revision_metadata()
        source_head = source.get("head") if isinstance(source, dict) else None
        if args.require_source_head:
            source_head = args.require_source_head
        elif args.require_current_source:
            source_head = current_source_head()
        write_manual_review_template(
            args.write_manual_review_template,
            source_head=source_head,
            parity_report_dir=args.parity_report_dir,
            mpq_parity_report_dir=args.mpq_parity_report_dir,
            opengl_capture=args.opengl_artifact_archive or args.opengl_artifact_dir,
            metal_capture=args.metal_artifact_archive or args.metal_artifact_dir,
            opengl_mpq_capture=args.opengl_mpq_artifact_archive or args.opengl_mpq_artifact_dir,
            metal_mpq_capture=args.metal_mpq_artifact_archive or args.metal_mpq_artifact_dir,
        )
        print(f"Wrote manual visual-review template to {args.write_manual_review_template}")
        return 0

    try:
        checks = audit_evidence(
            metal_artifact_dir=args.metal_artifact_dir,
            metal_artifact_archive=args.metal_artifact_archive,
            metal_mpq_artifact_dir=args.metal_mpq_artifact_dir,
            metal_mpq_artifact_archive=args.metal_mpq_artifact_archive,
            opengl_artifact_dir=args.opengl_artifact_dir,
            opengl_artifact_archive=args.opengl_artifact_archive,
            opengl_mpq_artifact_dir=args.opengl_mpq_artifact_dir,
            opengl_mpq_artifact_archive=args.opengl_mpq_artifact_archive,
            parity_report_dir=args.parity_report_dir,
            mpq_parity_report_dir=args.mpq_parity_report_dir,
            installed_smoke_artifact_dir=args.installed_smoke_artifact_dir,
            installed_smoke_capture_frame=args.installed_smoke_capture_frame,
            installed_smoke_app=args.installed_smoke_app,
            manual_review_note=args.manual_review_note,
            require_current_source=args.require_current_source,
            require_clean_source=args.require_clean_source,
            require_mpq_parity_report=args.require_mpq_parity_report,
            allow_missing_opengl=args.allow_missing_opengl,
            allow_missing_manual_review=args.allow_missing_manual_review,
            allow_missing_installed_app_identity=args.allow_missing_installed_app_identity,
            skip_installed_smoke=args.skip_installed_smoke,
            debug_package_report=args.debug_package_report_json,
            release_package_report=args.release_package_report_json,
            required_source_head=args.require_source_head,
        )
    except (OSError, EvidenceAuditError, CaptureManifestError, SmokeError, zipfile.BadZipFile) as error:
        print(f"Renderer PR evidence audit failed: {error}", file=sys.stderr)
        return 1

    report = format_report(checks)
    print(report, end="")
    if args.markdown_output is not None:
        args.markdown_output.parent.mkdir(parents=True, exist_ok=True)
        args.markdown_output.write_text(report, encoding="utf-8")

    return 1 if any(check.status == "FAIL" for check in checks) else 0


if __name__ == "__main__":
    sys.exit(main())
