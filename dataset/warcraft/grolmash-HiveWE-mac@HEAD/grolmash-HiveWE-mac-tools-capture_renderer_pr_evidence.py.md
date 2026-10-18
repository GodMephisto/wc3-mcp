---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "tools/capture_renderer_pr_evidence.py"
ref: HEAD
---

#!/usr/bin/env python3
"""Capture and package one backend's renderer PR evidence bundles."""

from __future__ import annotations

import argparse
import json
from pathlib import Path
import shlex
import subprocess
import sys
import tempfile
from typing import Iterable

sys.dont_write_bytecode = True

CAPTURE_MANIFEST_NAME = "renderer-parity-capture.json"


def quote_command(command: Iterable[object]) -> str:
    return " ".join(shlex.quote(str(part)) for part in command)


def backend_app_flag(backend: str) -> str:
    return f"--{backend}-app"


def backend_artifact_flag(backend: str) -> str:
    return f"--{backend}-artifact-dir"


def source_gate_flags(args: argparse.Namespace) -> list[str]:
    if args.require_source_head:
        return ["--require-source-head", args.require_source_head]
    return ["--require-current-source"]


def capture_command(
    *,
    python: str,
    tool_dir: Path,
    backend: str,
    app: Path,
    artifact_dir: Path,
    map_path: Path,
    warcraft_dir: Path | None,
    timeout: float,
    capture_delay_ms: int,
    source_flags: list[str],
    mpq_map_render: bool,
) -> list[str]:
    command = [
        python,
        str(tool_dir / "run_renderer_parity.py"),
        "--capture-only",
        backend,
        backend_app_flag(backend),
        str(app),
        backend_artifact_flag(backend),
        str(artifact_dir),
        "--map",
        str(map_path),
        "--pr-gate",
        "--timeout",
        str(timeout),
        "--capture-delay-ms",
        str(capture_delay_ms),
        *source_flags,
    ]
    if warcraft_dir is not None:
        command.extend(["--warcraft-dir", str(warcraft_dir)])
    if mpq_map_render:
        command.extend(["--map-render-only", "--map-after-mpq-export-reload"])
    return command


def package_command(
    *,
    python: str,
    tool_dir: Path,
    backend: str,
    artifact_dir: Path,
    output: Path,
    source_flags: list[str],
) -> list[str]:
    return [
        python,
        str(tool_dir / "package_renderer_capture.py"),
        "--artifact-dir",
        str(artifact_dir),
        "--expected-backend",
        backend,
        "--require-clean-source",
        *source_flags,
        "--output",
        str(output),
    ]


def opposite_backend(backend: str) -> str:
    return "metal" if backend == "opengl" else "opengl"


def default_output_dir(backend: str) -> Path:
    return Path(tempfile.gettempdir()) / f"hivewe-{backend}-pr-evidence"


def default_counterpart_output_dir(backend: str) -> Path:
    return Path(f"hivewe-{opposite_backend(backend)}-pr-evidence")


def summary_counterpart_map(args: argparse.Namespace) -> Path:
    return args.counterpart_map or Path("data/test map/")


def command_plan(args: argparse.Namespace, tool_dir: Path) -> dict[str, object]:
    output_dir = args.output_dir
    default_artifacts = args.default_artifact_dir or output_dir / f"{args.backend}-artifacts"
    mpq_artifacts = args.mpq_artifact_dir or output_dir / f"{args.backend}-mpq-map-artifacts"
    default_zip = args.default_archive or output_dir / f"hivewe-{args.backend}-artifacts.zip"
    mpq_zip = args.mpq_archive or output_dir / f"hivewe-{args.backend}-mpq-map-artifacts.zip"
    source_flags = source_gate_flags(args)
    warcraft_dir = args.warcraft_dir

    default_capture = capture_command(
        python=args.python,
        tool_dir=tool_dir,
        backend=args.backend,
        app=args.app,
        artifact_dir=default_artifacts,
        map_path=args.map,
        warcraft_dir=warcraft_dir,
        timeout=args.timeout,
        capture_delay_ms=args.capture_delay_ms,
        source_flags=source_flags,
        mpq_map_render=False,
    )
    default_package = package_command(
        python=args.python,
        tool_dir=tool_dir,
        backend=args.backend,
        artifact_dir=default_artifacts,
        output=default_zip,
        source_flags=source_flags,
    )
    mpq_capture = capture_command(
        python=args.python,
        tool_dir=tool_dir,
        backend=args.backend,
        app=args.app,
        artifact_dir=mpq_artifacts,
        map_path=args.map,
        warcraft_dir=warcraft_dir,
        timeout=args.timeout,
        capture_delay_ms=args.capture_delay_ms,
        source_flags=source_flags,
        mpq_map_render=True,
    )
    mpq_package = package_command(
        python=args.python,
        tool_dir=tool_dir,
        backend=args.backend,
        artifact_dir=mpq_artifacts,
        output=mpq_zip,
        source_flags=source_flags,
    )

    return {
        "output_dir": output_dir,
        "default_artifacts": default_artifacts,
        "mpq_artifacts": mpq_artifacts,
        "default_zip": default_zip,
        "mpq_zip": mpq_zip,
        "commands": [default_capture, default_package, mpq_capture, mpq_package],
    }


def read_source_head(artifact_dir: Path) -> str:
    manifest_path = artifact_dir / CAPTURE_MANIFEST_NAME
    try:
        manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    except (OSError, json.JSONDecodeError) as error:
        raise RuntimeError(f"could not read capture manifest {manifest_path}: {error}") from error
    source = manifest.get("source_revision")
    head = source.get("head") if isinstance(source, dict) else None
    if not isinstance(head, str) or not head:
        raise RuntimeError(f"capture manifest does not record a source revision: {manifest_path}")
    return head


def write_summary(args: argparse.Namespace, plan: dict[str, object]) -> Path:
    source_head = read_source_head(Path(plan["default_artifacts"]))
    mpq_source_head = read_source_head(Path(plan["mpq_artifacts"]))
    if mpq_source_head != source_head:
        raise RuntimeError(f"default capture source {source_head} differs from MPQ capture source {mpq_source_head}")
    summary_path = Path(plan["output_dir"]) / "renderer-pr-capture-summary.md"
    summary_path.write_text(summary_markdown(args, plan, source_head), encoding="utf-8")
    return summary_path


def summary_markdown(args: argparse.Namespace, plan: dict[str, object], source_head: str) -> str:
    backend = args.backend
    counterpart = opposite_backend(backend)
    default_zip = plan["default_zip"]
    mpq_zip = plan["mpq_zip"]
    output_dir = plan["output_dir"]
    counterpart_output = args.counterpart_output_dir or default_counterpart_output_dir(backend)
    counterpart_map = summary_counterpart_map(args)
    counterpart_default_zip = counterpart_output / f"hivewe-{counterpart}-artifacts.zip"
    counterpart_mpq_zip = counterpart_output / f"hivewe-{counterpart}-mpq-map-artifacts.zip"

    opengl_default_zip = default_zip if backend == "opengl" else counterpart_default_zip
    metal_default_zip = default_zip if backend == "metal" else counterpart_default_zip
    opengl_mpq_zip = mpq_zip if backend == "opengl" else counterpart_mpq_zip
    metal_mpq_zip = mpq_zip if backend == "metal" else counterpart_mpq_zip

    capture_counterpart = (
        f"python3 tools/capture_renderer_pr_evidence.py --backend {counterpart} "
        f"--app <{counterpart} HiveWE executable or app> --map {shlex.quote(str(counterpart_map))} "
        f"--warcraft-dir \"<Warcraft III>\" --output-dir {counterpart_output} "
        f"--require-source-head {source_head}"
    )
    default_compare = (
        "python3 tools/run_renderer_parity.py --skip-capture --pr-gate "
        f"--require-source-head {source_head} "
        f"--opengl-artifact-archive {opengl_default_zip} "
        f"--metal-artifact-archive {metal_default_zip} "
        "--report-dir /tmp/hivewe-renderer-parity"
    )
    mpq_compare = (
        "python3 tools/run_renderer_parity.py --skip-capture --pr-gate "
        "--map-render-only --map-after-mpq-export-reload "
        f"--require-source-head {source_head} "
        f"--opengl-artifact-archive {opengl_mpq_zip} "
        f"--metal-artifact-archive {metal_mpq_zip} "
        "--report-dir /tmp/hivewe-renderer-mpq-parity"
    )
    final_audit = (
        "python3 tools/audit_renderer_pr_evidence.py "
        f"--opengl-artifact-archive {opengl_default_zip} "
        f"--metal-artifact-archive {metal_default_zip} "
        f"--opengl-mpq-artifact-archive {opengl_mpq_zip} "
        f"--metal-mpq-artifact-archive {metal_mpq_zip} "
        "--parity-report-dir /tmp/hivewe-renderer-parity "
        "--mpq-parity-report-dir /tmp/hivewe-renderer-mpq-parity "
        "--installed-smoke-artifact-dir /tmp/hivewe-installed-full-metal-smoke-artifacts "
        "--installed-smoke-capture-frame /tmp/hivewe-installed-full-metal-smoke.png "
        "--installed-smoke-app build/macOS-Debug-WithTests/install/HiveWE.app "
        "--debug-package-report-json /tmp/hivewe-debug-package-report.json "
        "--release-package-report-json /tmp/hivewe-release-package-report.json "
        "--manual-review-note /tmp/hivewe-renderer-manual-review.md "
        f"--require-source-head {source_head} --require-clean-source --require-mpq-parity-report "
        "--markdown-output /tmp/hivewe-renderer-pr-evidence-audit.md"
    )
    manual_review_template = (
        "python3 tools/audit_renderer_pr_evidence.py "
        "--write-manual-review-template /tmp/hivewe-renderer-manual-review.md "
        f"--opengl-artifact-archive {opengl_default_zip} "
        f"--metal-artifact-archive {metal_default_zip} "
        f"--opengl-mpq-artifact-archive {opengl_mpq_zip} "
        f"--metal-mpq-artifact-archive {metal_mpq_zip} "
        "--parity-report-dir /tmp/hivewe-renderer-parity "
        "--mpq-parity-report-dir /tmp/hivewe-renderer-mpq-parity "
        f"--require-source-head {source_head}"
    )

    lines = [
        "# HiveWE Renderer PR Capture Evidence",
        "",
        f"- Backend: `{backend}`",
        f"- Source: `{source_head}`",
        f"- Output directory: `{output_dir}`",
        f"- Default capture archive: `{default_zip}`",
        f"- MPQ map-render capture archive: `{mpq_zip}`",
        "",
        "## Commands Run",
        "",
    ]
    lines.extend(f"- `{quote_command(command)}`" for command in plan["commands"])
    lines.extend(
        [
            "",
            "## Counterpart Capture",
            "",
            "Run the same helper on the opposite backend from the same clean source revision:",
            "",
            "```shell",
            capture_counterpart,
            "```",
            "",
            "## Final Comparisons",
            "",
            "```shell",
            default_compare,
            "",
            mpq_compare,
            "```",
            "",
            "If both backend bundles were produced by the GitHub Actions workflow, the `renderer-pr-evidence` job "
            "downloads them and uploads `final-renderer-pr-evidence` with the default parity report, MPQ parity "
            "report, automated pre-review audit, and a manual-review template prefilled with evidence paths and "
            "diff metrics. That artifact is not a strict final audit until the manual review note is completed.",
            "",
            "To start the required human visual-review note manually:",
            "",
            "```shell",
            manual_review_template,
            "```",
            "",
            "## Final Audit",
            "",
            "After the OpenGL reference, Metal candidate, parity reports, package reports, full Metal smoke artifacts, "
            "and manual visual review note are present, run:",
            "",
            "```shell",
            final_audit,
            "```",
            "",
            "When auditing downloaded CI artifacts on a host that does not have the installed macOS app bundle, omit "
            "`--installed-smoke-app`; the auditor will prove full-smoke identity by matching the smoke executable "
            "fingerprint against the Debug package report.",
            "",
        ]
    )
    return "\n".join(lines)


def run_command(command: list[str]) -> None:
    print(f"Running: {quote_command(command)}")
    subprocess.run(command, check=True)


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description=(
            "Capture and package the default plus MPQ renderer PR evidence bundles for one HiveWE backend. "
            "Use this on the Windows/Linux OpenGL reference host or the macOS Metal candidate host."
        )
    )
    parser.add_argument("--backend", choices=("opengl", "metal"), required=False, help="Backend to capture.")
    parser.add_argument("--app", type=Path, help="HiveWE executable or .app bundle for the selected backend.")
    parser.add_argument("--map", type=Path, default=Path("data/test map/"), help="Map directory to load.")
    parser.add_argument(
        "--warcraft-dir",
        type=Path,
        help="Warcraft III installation directory. Defaults to HIVEWE_WARCRAFT_DIR inside run_renderer_parity.py.",
    )
    parser.add_argument("--output-dir", type=Path, help="Directory for artifact folders, zips, and capture summary.")
    parser.add_argument("--default-artifact-dir", type=Path, help="Override the default PR capture artifact directory.")
    parser.add_argument("--default-archive", type=Path, help="Override the default PR capture zip path.")
    parser.add_argument("--mpq-artifact-dir", type=Path, help="Override the MPQ export/reload map-render capture artifact directory.")
    parser.add_argument("--mpq-archive", type=Path, help="Override the MPQ export/reload map-render capture zip path.")
    parser.add_argument("--timeout", type=float, default=180.0, help="Maximum seconds for each HiveWE smoke command.")
    parser.add_argument("--capture-delay-ms", type=int, default=1000, help="Delay before render smoke captures.")
    parser.add_argument("--require-source-head", metavar="SHA", help="Require this source revision instead of current HEAD.")
    parser.add_argument(
        "--counterpart-map",
        type=Path,
        help=(
            "Map path to show in the generated opposite-backend capture command. "
            "Defaults to the repository test map so summaries created from installed app bundles remain portable."
        ),
    )
    parser.add_argument(
        "--counterpart-output-dir",
        type=Path,
        help="Output directory to show for the generated opposite-backend capture command. Defaults to ./hivewe-<backend>-pr-evidence.",
    )
    parser.add_argument("--python", default=sys.executable, help="Python executable used to run helper tools.")
    parser.add_argument("--dry-run", action="store_true", help="Print the commands that would run, but do not execute them.")
    parser.add_argument("--summary-only", action="store_true", help="Rewrite renderer-pr-capture-summary.md from existing capture manifests without rerunning captures.")
    parser.add_argument("--self-test", action="store_true", help="Run command-construction self-tests and exit.")
    return parser.parse_args()


def validate_args(args: argparse.Namespace) -> None:
    if args.self_test:
        return
    if args.backend is None:
        raise SystemExit("--backend is required unless --self-test is used")
    if args.app is None:
        raise SystemExit("--app is required unless --self-test is used")
    if args.output_dir is None:
        args.output_dir = default_output_dir(args.backend)


def run_self_test() -> int:
    with tempfile.TemporaryDirectory(prefix="hivewe-pr-capture-helper-") as temp_dir:
        root = Path(temp_dir)
        args = argparse.Namespace(
            backend="opengl",
            app=Path("/tmp/HiveWE"),
            map=Path("data/test map/"),
            warcraft_dir=Path("/Applications/Warcraft III"),
            output_dir=root / "opengl-out",
            default_artifact_dir=None,
            default_archive=None,
            mpq_artifact_dir=None,
            mpq_archive=None,
            timeout=180.0,
            capture_delay_ms=1000,
            require_source_head="abc123def456",
            counterpart_map=None,
            counterpart_output_dir=None,
            python="python3",
        )
        plan = command_plan(args, Path("/repo/tools"))
        commands = [quote_command(command) for command in plan["commands"]]
        assert len(commands) == 4
        assert "--capture-only opengl" in commands[0]
        assert "--opengl-app /tmp/HiveWE" in commands[0]
        assert "--map-after-mpq-export-reload" not in commands[0]
        assert "--expected-backend opengl" in commands[1]
        assert "--map-render-only --map-after-mpq-export-reload" in commands[2]
        assert "--require-source-head abc123def456" in commands[2]
        assert "--require-clean-source" in commands[3]

        summary = summary_markdown(args, plan, "abc123def456")
        assert "capture_renderer_pr_evidence.py --backend metal" in summary
        assert "--opengl-artifact-archive" in summary
        assert "--metal-artifact-archive" in summary
        assert "--require-mpq-parity-report" in summary
        assert "hivewe-opengl-mpq-map-artifacts.zip" in summary
        assert "--write-manual-review-template /tmp/hivewe-renderer-manual-review.md" in summary
        assert "--parity-report-dir /tmp/hivewe-renderer-parity" in summary
        assert "--mpq-parity-report-dir /tmp/hivewe-renderer-mpq-parity" in summary
        assert "--output-dir hivewe-metal-pr-evidence" in summary
        assert "--map 'data/test map'" in summary

        installed_map_args = argparse.Namespace(
            backend="metal",
            app=Path("/tmp/HiveWE.app"),
            map=Path("/tmp/HiveWE.app/Contents/Resources/data/test map"),
            warcraft_dir=None,
            output_dir=root / "installed-metal-out",
            default_artifact_dir=None,
            default_archive=None,
            mpq_artifact_dir=None,
            mpq_archive=None,
            timeout=30.0,
            capture_delay_ms=250,
            require_source_head=None,
            counterpart_map=None,
            counterpart_output_dir=None,
            python="python3",
        )
        installed_plan = command_plan(installed_map_args, Path("/repo/tools"))
        installed_summary = summary_markdown(installed_map_args, installed_plan, "abc123def456")
        assert "--map 'data/test map'" in installed_summary
        assert "Contents/Resources/data/test map" not in installed_summary.split("## Counterpart Capture", 1)[1]
        assert "--output-dir hivewe-opengl-pr-evidence" in installed_summary

        summary_existing_args = argparse.Namespace(
            backend="metal",
            app=Path("/tmp/HiveWE.app"),
            map=Path("/tmp/HiveWE.app/Contents/Resources/data/test map"),
            warcraft_dir=None,
            output_dir=root / "summary-existing",
            default_artifact_dir=None,
            default_archive=None,
            mpq_artifact_dir=None,
            mpq_archive=None,
            timeout=30.0,
            capture_delay_ms=250,
            require_source_head=None,
            counterpart_map=None,
            counterpart_output_dir=None,
            python="python3",
        )
        summary_existing_plan = command_plan(summary_existing_args, Path("/repo/tools"))
        for artifact_key in ("default_artifacts", "mpq_artifacts"):
            artifact_dir = Path(summary_existing_plan[artifact_key])
            artifact_dir.mkdir(parents=True)
            (artifact_dir / CAPTURE_MANIFEST_NAME).write_text(
                json.dumps({"source_revision": {"head": "abc123def456"}}),
                encoding="utf-8",
            )
        summary_path = write_summary(summary_existing_args, summary_existing_plan)
        summary_text = summary_path.read_text(encoding="utf-8")
        assert "abc123def456" in summary_text
        assert "--map 'data/test map'" in summary_text

        (Path(summary_existing_plan["mpq_artifacts"]) / CAPTURE_MANIFEST_NAME).write_text(
            json.dumps({"source_revision": {"head": "different"}}),
            encoding="utf-8",
        )
        try:
            write_summary(summary_existing_args, summary_existing_plan)
        except RuntimeError as error:
            assert "differs from MPQ capture source" in str(error)
        else:
            raise AssertionError("write_summary should reject mismatched capture sources")

        metal_args = argparse.Namespace(
            backend="metal",
            app=Path("/tmp/HiveWE.app"),
            map=Path("data/test map/"),
            warcraft_dir=None,
            output_dir=root / "metal-out",
            default_artifact_dir=root / "custom-metal-artifacts",
            default_archive=root / "custom-metal.zip",
            mpq_artifact_dir=root / "custom-metal-mpq-artifacts",
            mpq_archive=root / "custom-metal-mpq.zip",
            timeout=30.0,
            capture_delay_ms=250,
            require_source_head=None,
            counterpart_map=Path("/shared/maps/test map"),
            counterpart_output_dir=root / "opengl-counterpart",
            python="python3",
        )
        metal_plan = command_plan(metal_args, Path("/repo/tools"))
        metal_commands = [quote_command(command) for command in metal_plan["commands"]]
        assert "--capture-only metal" in metal_commands[0]
        assert "--metal-app /tmp/HiveWE.app" in metal_commands[0]
        assert str(root / "custom-metal-artifacts") in metal_commands[0]
        assert str(root / "custom-metal.zip") in metal_commands[1]
        assert str(root / "custom-metal-mpq-artifacts") in metal_commands[2]
        assert str(root / "custom-metal-mpq.zip") in metal_commands[3]
        assert "--require-current-source" in metal_commands[0]
        assert "--warcraft-dir" not in metal_commands[0]
        metal_summary = summary_markdown(metal_args, metal_plan, "feedface1234")
        assert "capture_renderer_pr_evidence.py --backend opengl" in metal_summary
        assert "custom-metal.zip" in metal_summary
        assert "--map '/shared/maps/test map'" in metal_summary
        assert str(root / "opengl-counterpart") in metal_summary

    print("capture_renderer_pr_evidence self-test passed.")
    return 0


def main() -> int:
    args = parse_args()
    if args.self_test:
        return run_self_test()
    validate_args(args)

    tool_dir = Path(__file__).resolve().parent
    plan = command_plan(args, tool_dir)
    if args.dry_run:
        for command in plan["commands"]:
            print(quote_command(command))
        return 0

    try:
        Path(plan["output_dir"]).mkdir(parents=True, exist_ok=True)
        if args.summary_only:
            summary_path = write_summary(args, plan)
            print(f"Wrote renderer PR capture summary to {summary_path}")
            return 0
        for command in plan["commands"]:
            run_command(command)
        summary_path = write_summary(args, plan)
    except (OSError, RuntimeError, subprocess.CalledProcessError) as error:
        print(f"Renderer PR evidence capture failed: {error}", file=sys.stderr)
        return 1

    print(f"Wrote renderer PR capture summary to {summary_path}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
