---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "tools/compare_smoke_artifacts.py"
ref: HEAD
---

#!/usr/bin/env python3
"""Compare HiveWE smoke artifact PNGs without external Python packages."""

from __future__ import annotations

import argparse
import fnmatch
import json
import math
from pathlib import Path
import struct
import sys
import tempfile
import zlib


PNG_SIGNATURE = b"\x89PNG\r\n\x1a\n"
MIN_VARIED_PIXELS = 32
FLAT_ALLOWED_ARTIFACTS = {
    "object-grid-empty-filter-preview.png",
}
REAL_PREVIEW_CLEAR_COLORS = {
    "model-editor-preview.png": (77, 77, 77),
    "object-grid-preview.png": (38, 38, 38),
}
SYNTHETIC_ARTIFACT_COLOR_MINIMUMS = {
    "metal-map-terrain-cliffs-particles.png": {
        "red": 512,
        "green": 2048,
        "blue": 128,
        "magenta": 256,
        "cyan": 512,
        "yellow": 128,
        "orange": 256,
    },
    "metal-alpha-picking.png": {
        "red": 2048,
        "green": 256,
        "blue": 256,
    },
    "metal-pe1-spawn-picking.png": {
        "green": 512,
    },
    "metal-meshless-bounds-picking.png": {
        "blue": 512,
    },
    "metal-pe1-spawn-render.png": {
        "green": 512,
        "blue": 512,
        "magenta": 64,
    },
    "metal-model-materials.png": {
        "red": 256,
        "green": 512,
        "blue": 512,
        "magenta": 256,
        "cyan": 256,
        "orange": 128,
    },
    "metal-particles.png": {
        "red": 2048,
        "yellow": 2048,
        "orange": 2048,
    },
    "model-editor-material-preview.png": {
        "red": 512,
        "green": 256,
        "magenta": 256,
        "cyan": 256,
        "orange": 256,
    },
    "object-grid-material-preview.png": {
        "red": 16,
        "green": 16,
        "blue": 16,
        "magenta": 16,
        "cyan": 16,
        "orange": 16,
        "white": 128,
    },
    "object-grid-multicell-material-preview.png": {
        "green": 64,
        "blue": 64,
        "white": 128,
    },
    "object-grid-scrolled-material-preview.png": {
        "green": 64,
        "blue": 64,
        "white": 256,
    },
    "model-editor-particle-preview.png": {
        "red": 512,
        "green": 2048,
        "magenta": 256,
        "orange": 512,
    },
    "object-grid-particle-preview.png": {
        "green": 32,
        "blue": 128,
        "white": 128,
    },
    "model-editor-ribbon-preview.png": {
        "cyan": 512,
    },
    "object-grid-ribbon-preview.png": {
        "cyan": 32,
        "white": 128,
    },
}

ArtifactSize = tuple[int, int] | None

MAP_RENDER_ARTIFACT_SIZES: dict[str, ArtifactSize] = {
    "map-render-baseline.png": None,
    "map-render-pathing-overlay.png": None,
    "map-render-brush-mask.png": None,
    "map-render-lighting-toggle.png": None,
    "map-render-water-toggle.png": None,
    "map-render-wireframe-toggle.png": None,
    "map-render-click-helpers-off.png": None,
    "map-render-click-helpers-on.png": None,
}
REAL_SHARED_PREVIEW_ARTIFACT_SIZES: dict[str, ArtifactSize] = {
    "model-editor-preview.png": (512, 512),
    "object-grid-preview.png": (512, 320),
}
SYNTHETIC_SHARED_PREVIEW_ARTIFACT_SIZES: dict[str, ArtifactSize] = {
    "model-editor-material-preview.png": (512, 512),
    "object-grid-material-preview.png": (512, 320),
    "model-editor-particle-preview.png": (512, 512),
    "object-grid-particle-preview.png": (512, 320),
}
SHARED_PREVIEW_ARTIFACT_SIZES: dict[str, ArtifactSize] = {
    **REAL_SHARED_PREVIEW_ARTIFACT_SIZES,
    **SYNTHETIC_SHARED_PREVIEW_ARTIFACT_SIZES,
}
METAL_RENDERER_PROBE_ARTIFACT_SIZES: dict[str, ArtifactSize] = {
    "metal-map-terrain-cliffs-particles.png": (256, 256),
    "metal-map-pathing-overlay.png": (256, 256),
    "metal-cliff-pathing-overlay.png": (256, 256),
    "metal-alpha-picking.png": (512, 512),
    "metal-pe1-spawn-picking.png": (512, 512),
    "metal-pe1-spawn-render.png": (256, 256),
    "metal-meshless-bounds-picking.png": (512, 512),
    "metal-model-materials.png": (256, 256),
    "metal-model-depth-isolation.png": (256, 256),
    "metal-selection-overlay.png": (256, 256),
    "metal-particles.png": (256, 256),
}
METAL_PREVIEW_PROBE_ARTIFACT_SIZES: dict[str, ArtifactSize] = {
    "model-editor-material-preview.png": (512, 512),
    "object-grid-material-preview.png": (512, 320),
    "object-grid-empty-filter-preview.png": (512, 320),
    "object-grid-load-failure-preview.png": (512, 320),
    "object-grid-multicell-material-preview.png": (512, 320),
    "object-grid-scrolled-material-preview.png": (512, 320),
    "model-editor-particle-preview.png": (512, 512),
    "object-grid-particle-preview.png": (512, 320),
    "model-editor-ribbon-preview.png": (512, 512),
    "object-grid-ribbon-preview.png": (512, 320),
}
METAL_MAP_CANVAS_ARTIFACT_SIZES: dict[str, ArtifactSize] = {
    **MAP_RENDER_ARTIFACT_SIZES,
    "metal-map-terrain-cliffs-particles.png": (256, 256),
    **METAL_PREVIEW_PROBE_ARTIFACT_SIZES,
}
FULL_METAL_SMOKE_ARTIFACT_SIZES: dict[str, ArtifactSize] = {
    **MAP_RENDER_ARTIFACT_SIZES,
    **METAL_RENDERER_PROBE_ARTIFACT_SIZES,
    **SHARED_PREVIEW_ARTIFACT_SIZES,
    **METAL_PREVIEW_PROBE_ARTIFACT_SIZES,
}

ARTIFACT_PRESETS = {
    "map-render": list(MAP_RENDER_ARTIFACT_SIZES),
    "shared-previews": list(SHARED_PREVIEW_ARTIFACT_SIZES),
    "shared-preview-probes": list(SYNTHETIC_SHARED_PREVIEW_ARTIFACT_SIZES),
    "metal-renderer-probes": list(METAL_RENDERER_PROBE_ARTIFACT_SIZES),
    "metal-preview-probes": list(METAL_PREVIEW_PROBE_ARTIFACT_SIZES),
}

MANIFEST_PRESETS = {
    "map-render": MAP_RENDER_ARTIFACT_SIZES,
    "shared-previews": SHARED_PREVIEW_ARTIFACT_SIZES,
    "shared-preview-probes": SYNTHETIC_SHARED_PREVIEW_ARTIFACT_SIZES,
    "metal-renderer-probes": METAL_RENDERER_PROBE_ARTIFACT_SIZES,
    "metal-preview-probes": METAL_PREVIEW_PROBE_ARTIFACT_SIZES,
    "metal-map-canvas": METAL_MAP_CANVAS_ARTIFACT_SIZES,
    "full-metal-smoke": FULL_METAL_SMOKE_ARTIFACT_SIZES,
}


class PngError(RuntimeError):
    pass


def paeth_predictor(left: int, above: int, upper_left: int) -> int:
    estimate = left + above - upper_left
    left_distance = abs(estimate - left)
    above_distance = abs(estimate - above)
    upper_left_distance = abs(estimate - upper_left)
    if left_distance <= above_distance and left_distance <= upper_left_distance:
        return left
    if above_distance <= upper_left_distance:
        return above
    return upper_left


def iter_png_chunks(data: bytes):
    if not data.startswith(PNG_SIGNATURE):
        raise PngError("missing PNG signature")

    offset = len(PNG_SIGNATURE)
    while offset + 8 <= len(data):
        length = struct.unpack(">I", data[offset:offset + 4])[0]
        chunk_type = data[offset + 4:offset + 8]
        offset += 8
        payload = data[offset:offset + length]
        offset += length + 4
        if len(payload) != length:
            raise PngError(f"truncated {chunk_type.decode('ascii', 'replace')} chunk")
        yield chunk_type, payload
        if chunk_type == b"IEND":
            return

    raise PngError("missing IEND chunk")


def decode_png_rgba(path: Path) -> tuple[int, int, bytes]:
    width = 0
    height = 0
    bit_depth = 0
    color_type = 0
    interlace = 0
    idat_parts: list[bytes] = []

    for chunk_type, payload in iter_png_chunks(path.read_bytes()):
        if chunk_type == b"IHDR":
            width, height, bit_depth, color_type, _compression, _filter, interlace = struct.unpack(">IIBBBBB", payload)
        elif chunk_type == b"IDAT":
            idat_parts.append(payload)

    if width <= 0 or height <= 0:
        raise PngError("invalid or missing IHDR dimensions")
    if bit_depth != 8 or color_type != 6 or interlace != 0:
        raise PngError("only 8-bit non-interlaced RGBA PNGs are supported")

    bytes_per_pixel = 4
    stride = width * bytes_per_pixel
    raw = zlib.decompress(b"".join(idat_parts))
    expected_size = height * (stride + 1)
    if len(raw) != expected_size:
        raise PngError(f"unexpected decompressed size {len(raw)}, expected {expected_size}")

    rows: list[bytearray] = []
    previous = bytearray(stride)
    offset = 0
    for _row in range(height):
        filter_type = raw[offset]
        offset += 1
        row = bytearray(raw[offset:offset + stride])
        offset += stride

        if filter_type == 1:
            for i in range(stride):
                left = row[i - bytes_per_pixel] if i >= bytes_per_pixel else 0
                row[i] = (row[i] + left) & 0xFF
        elif filter_type == 2:
            for i in range(stride):
                row[i] = (row[i] + previous[i]) & 0xFF
        elif filter_type == 3:
            for i in range(stride):
                left = row[i - bytes_per_pixel] if i >= bytes_per_pixel else 0
                row[i] = (row[i] + ((left + previous[i]) // 2)) & 0xFF
        elif filter_type == 4:
            for i in range(stride):
                left = row[i - bytes_per_pixel] if i >= bytes_per_pixel else 0
                upper_left = previous[i - bytes_per_pixel] if i >= bytes_per_pixel else 0
                row[i] = (row[i] + paeth_predictor(left, previous[i], upper_left)) & 0xFF
        elif filter_type != 0:
            raise PngError(f"unsupported PNG filter type {filter_type}")

        rows.append(row)
        previous = row

    return width, height, b"".join(rows)


def encode_png_rgba(path: Path, width: int, height: int, pixels: bytes) -> None:
    if width <= 0 or height <= 0 or len(pixels) != width * height * 4:
        raise PngError(f"invalid RGBA image for {path}: {width}x{height}, {len(pixels)} bytes")

    def chunk(chunk_type: bytes, payload: bytes) -> bytes:
        checksum = zlib.crc32(chunk_type)
        checksum = zlib.crc32(payload, checksum) & 0xFFFFFFFF
        return struct.pack(">I", len(payload)) + chunk_type + payload + struct.pack(">I", checksum)

    stride = width * 4
    raw = bytearray()
    for y in range(height):
        raw.append(0)
        row_offset = y * stride
        raw.extend(pixels[row_offset:row_offset + stride])

    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_bytes(
        PNG_SIGNATURE
        + chunk(b"IHDR", struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0))
        + chunk(b"IDAT", zlib.compress(bytes(raw), level=9))
        + chunk(b"IEND", b"")
    )


def read_png_size(path: Path) -> tuple[int, int]:
    with path.open("rb") as handle:
        header = handle.read(24)
    if len(header) < 24 or not header.startswith(PNG_SIGNATURE) or header[12:16] != b"IHDR":
        raise PngError(f"{path} is not a valid PNG")
    width, height = struct.unpack(">II", header[16:24])
    if width <= 0 or height <= 0:
        raise PngError(f"{path} has invalid PNG dimensions: {width}x{height}")
    return width, height


def count_varied_pixels(path: Path) -> tuple[int, int, int]:
    width, height, pixels = decode_png_rgba(path)
    if not pixels:
        return width, height, 0

    color_counts: dict[bytes, int] = {}
    for offset in range(0, len(pixels), 4):
        color = pixels[offset:offset + 4]
        color_counts[color] = color_counts.get(color, 0) + 1

    pixel_count = len(pixels) // 4
    dominant_count = max(color_counts.values(), default=0)
    varied = pixel_count - dominant_count
    return width, height, varied


def count_preview_pixel_diversity(path: Path, clear_color: tuple[int, int, int]) -> tuple[int, int, int]:
    width, height, pixels = decode_png_rgba(path)
    pixel_count = width * height
    if pixel_count <= 0:
        return 0, 0, 0

    clear_r, clear_g, clear_b = clear_color
    color_bins: dict[tuple[int, int, int], int] = {}
    changed_pixels = 0
    for offset in range(0, len(pixels), 4):
        r = pixels[offset]
        g = pixels[offset + 1]
        b = pixels[offset + 2]
        delta = abs(r - clear_r) + abs(g - clear_g) + abs(b - clear_b)
        if delta <= 18:
            continue

        changed_pixels += 1
        color_bin = (r // 16, g // 16, b // 16)
        color_bins[color_bin] = color_bins.get(color_bin, 0) + 1

    significant_bins = sum(1 for count in color_bins.values() if count >= 4)
    dominant_bin_pixels = max(color_bins.values(), default=0)
    return changed_pixels, significant_bins, dominant_bin_pixels


def synthetic_reference_pixel_count(name: str, width: int, height: int) -> int:
    for manifests in (
        METAL_RENDERER_PROBE_ARTIFACT_SIZES,
        METAL_PREVIEW_PROBE_ARTIFACT_SIZES,
        SYNTHETIC_SHARED_PREVIEW_ARTIFACT_SIZES,
    ):
        expected_size = manifests.get(name)
        if expected_size is not None:
            return expected_size[0] * expected_size[1]
    return width * height


def count_synthetic_color_buckets(width: int, height: int, pixels: bytes) -> dict[str, int]:
    counts = {name: 0 for name in ("red", "green", "blue", "magenta", "cyan", "yellow", "orange", "white")}
    for offset in range(0, len(pixels), 4):
        r = pixels[offset]
        g = pixels[offset + 1]
        b = pixels[offset + 2]

        if r > 150 and g < 120 and b < 120:
            counts["red"] += 1
        if g > 130 and r < 140 and b < 140:
            counts["green"] += 1
        if b > 130 and r < 140 and g < 160:
            counts["blue"] += 1
        if r > 130 and b > 130 and g < 130:
            counts["magenta"] += 1
        if g > 130 and b > 130 and r < 130:
            counts["cyan"] += 1
        if r > 150 and g > 130 and b < 130:
            counts["yellow"] += 1
        if r > 150 and 50 < g < 180 and b < 110:
            counts["orange"] += 1
        if r > 200 and g > 200 and b > 200:
            counts["white"] += 1
    return counts


def validate_synthetic_artifact_semantics(path: Path) -> None:
    minimums = SYNTHETIC_ARTIFACT_COLOR_MINIMUMS.get(path.name)
    if minimums is None:
        return

    width, height, pixels = decode_png_rgba(path)
    reference_pixels = synthetic_reference_pixel_count(path.name, width, height)
    actual_pixels = width * height
    counts = count_synthetic_color_buckets(width, height, pixels)
    missing = []
    for bucket, minimum in minimums.items():
        scaled_minimum = max(1, math.ceil(minimum * actual_pixels / reference_pixels))
        if counts.get(bucket, 0) < scaled_minimum:
            missing.append(f"{bucket}={counts.get(bucket, 0)} < {scaled_minimum}")

    if missing:
        details = ", ".join(missing)
        observed = ", ".join(f"{name}={count}" for name, count in sorted(counts.items()))
        raise PngError(f"{path.name} is missing expected synthetic render colors: {details}; observed {observed}")


def validate_real_preview_diversity(path: Path) -> None:
    clear_color = REAL_PREVIEW_CLEAR_COLORS.get(path.name)
    if clear_color is None:
        return

    width, height = read_png_size(path)
    pixel_count = width * height
    changed_pixels, significant_bins, dominant_bin_pixels = count_preview_pixel_diversity(path, clear_color)
    min_changed_pixels = max(64, pixel_count // 5000)
    min_color_bins = max(16, min(40, changed_pixels // 80))
    max_dominant_pixels = max(32, changed_pixels * 7 // 10)
    if (
        changed_pixels < min_changed_pixels
        or significant_bins < min_color_bins
        or dominant_bin_pixels > max_dominant_pixels
    ):
        raise PngError(
            f"{path.name} does not look like a textured real model preview: "
            f"{changed_pixels} changed pixels, {significant_bins} significant color bins, "
            f"{dominant_bin_pixels} dominant-bin pixels; expected at least {min_changed_pixels} changed pixels, "
            f"at least {min_color_bins} color bins, and at most {max_dominant_pixels} dominant-bin pixels"
        )


def validate_png_artifact_manifest(
    artifact_dir: Path,
    required_artifacts: dict[str, ArtifactSize],
    *,
    variable_size: tuple[int, int] | None = None,
    label: str = "PNG artifacts",
    min_varied_pixels: int = MIN_VARIED_PIXELS,
    allow_extra: bool = False,
) -> int:
    missing = []
    wrong_size = []
    flat = []
    for name, expected_size in required_artifacts.items():
        path = artifact_dir / name
        if not path.is_file():
            missing.append(name)
            continue

        actual_size = read_png_size(path)
        if expected_size is None:
            if variable_size is None:
                raise PngError(f"{name} has a variable expected size but no reference size was provided")
            expected_size = variable_size
        if actual_size != expected_size:
            wrong_size.append((name, actual_size, expected_size))
            continue

        _width, _height, varied = count_varied_pixels(path)
        if varied < min_varied_pixels and name not in FLAT_ALLOWED_ARTIFACTS:
            flat.append((name, varied))
            continue
        validate_real_preview_diversity(path)
        validate_synthetic_artifact_semantics(path)

    if missing:
        raise PngError(f"missing {label}: " + ", ".join(sorted(missing)))
    if wrong_size:
        details = ", ".join(f"{name} was {actual}, expected {expected}" for name, actual, expected in wrong_size)
        raise PngError(f"unexpected {label} dimensions: " + details)
    if flat:
        details = ", ".join(f"{name} had {varied} varied pixels" for name, varied in flat)
        raise PngError(f"{label} were too flat: " + details)
    if not allow_extra:
        required_names = set(required_artifacts)
        extra = sorted(path.name for path in artifact_dir.glob("*.png") if path.name not in required_names)
        if extra:
            raise PngError(f"unexpected {label}: " + ", ".join(extra))

    return len(required_artifacts)


def build_diff_rgba(left_pixels: bytes, right_pixels: bytes, scale: float) -> bytes:
    out = bytearray(len(left_pixels))
    for pixel_offset in range(0, len(left_pixels), 4):
        alpha_delta = abs(left_pixels[pixel_offset + 3] - right_pixels[pixel_offset + 3])
        for channel_offset in range(3):
            delta = abs(left_pixels[pixel_offset + channel_offset] - right_pixels[pixel_offset + channel_offset])
            out[pixel_offset + channel_offset] = min(255, int((delta + alpha_delta * 0.5) * scale))
        out[pixel_offset + 3] = 255
    return bytes(out)


def matching_png_names(directory: Path, patterns: list[str], exact_names: set[str]) -> set[str]:
    names = {path.name for path in directory.iterdir() if path.is_file() and path.suffix.lower() == ".png"}
    return {
        name
        for name in names
        if name in exact_names or any(fnmatch.fnmatch(name, pattern) for pattern in patterns)
    }


def compare_image(left_path: Path, right_path: Path, diff_path: Path | None = None, diff_scale: float = 4.0) -> dict:
    left_width, left_height, left_pixels = decode_png_rgba(left_path)
    right_width, right_height, right_pixels = decode_png_rgba(right_path)
    result = {
        "name": left_path.name,
        "left_size": [left_width, left_height],
        "right_size": [right_width, right_height],
        "status": "ok",
    }
    if (left_width, left_height) != (right_width, right_height):
        result["status"] = "dimension-mismatch"
        return result

    if diff_path is not None:
        encode_png_rgba(diff_path, left_width, left_height, build_diff_rgba(left_pixels, right_pixels, diff_scale))

    changed_pixels = 0
    sum_abs = 0
    sum_square = 0
    max_delta = 0
    for pixel_offset in range(0, len(left_pixels), 4):
        pixel_changed = False
        for channel_offset in range(4):
            delta = abs(left_pixels[pixel_offset + channel_offset] - right_pixels[pixel_offset + channel_offset])
            sum_abs += delta
            sum_square += delta * delta
            max_delta = max(max_delta, delta)
            pixel_changed = pixel_changed or delta != 0
        if pixel_changed:
            changed_pixels += 1

    channel_count = len(left_pixels)
    pixel_count = left_width * left_height
    result.update(
        {
            "changed_pixels": changed_pixels,
            "changed_percent": (changed_pixels / pixel_count) * 100.0 if pixel_count else 0.0,
            "mean_abs_channel_delta": sum_abs / channel_count if channel_count else 0.0,
            "rms_channel_delta": math.sqrt(sum_square / channel_count) if channel_count else 0.0,
            "max_channel_delta": max_delta,
        }
    )
    return result


def markdown_escape(value: object) -> str:
    return str(value).replace("|", "\\|")


def write_markdown_report(
    path: Path,
    left_dir: Path,
    right_dir: Path,
    results: list[dict],
    ok_count: int,
    worst_changed: float,
    worst_mean: float,
    worst_rms: float,
    failed: bool,
) -> None:
    lines = [
        "# HiveWE Smoke Artifact Comparison",
        "",
        f"- Reference: `{left_dir}`",
        f"- Candidate: `{right_dir}`",
        f"- Result: {'FAIL' if failed else 'PASS'}",
        f"- Compared: {len(results)} artifacts ({ok_count} ok, {len(results) - ok_count} failed)",
        f"- Worst changed pixels: {worst_changed:.3f}%",
        f"- Worst mean channel delta: {worst_mean:.3f}",
        f"- Worst RMS channel delta: {worst_rms:.3f}",
        "",
        "| Artifact | Status | Changed % | Mean | RMS | Max | Diff |",
        "| --- | --- | ---: | ---: | ---: | ---: | --- |",
    ]
    for result in results:
        lines.append(
            "| "
            + " | ".join(
                [
                    markdown_escape(result["name"]),
                    markdown_escape(result["status"]),
                    f"{result.get('changed_percent', 0.0):.3f}",
                    f"{result.get('mean_abs_channel_delta', 0.0):.3f}",
                    f"{result.get('rms_channel_delta', 0.0):.3f}",
                    str(result.get("max_channel_delta", 0)),
                    markdown_escape(result.get("diff_path", "")),
                ]
            )
            + " |"
        )

    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text("\n".join(lines) + "\n", encoding="utf-8")


def compare_artifact_directories(
    left_dir: Path,
    right_dir: Path,
    *,
    patterns: list[str],
    exact_names: set[str],
    common_only: bool,
    diff_dir: Path | None,
    diff_scale: float,
    max_rms: float | None,
    max_mean_abs: float | None,
    max_changed_percent: float | None,
) -> tuple[list[dict], bool]:
    left_names = matching_png_names(left_dir, patterns, exact_names)
    right_names = matching_png_names(right_dir, patterns, exact_names)
    names = sorted((left_names & right_names) if common_only else (left_names | right_names | exact_names))
    if not names:
        raise PngError("no PNG artifacts matched the requested patterns")

    results = []
    failed = False
    for name in names:
        if name not in left_names and name not in right_names:
            results.append({"name": name, "status": "missing-both"})
            failed = True
            continue
        if name not in left_names:
            results.append({"name": name, "status": "missing-left"})
            failed = True
            continue
        if name not in right_names:
            results.append({"name": name, "status": "missing-right"})
            failed = True
            continue

        try:
            diff_path = diff_dir / f"{Path(name).stem}.diff.png" if diff_dir is not None else None
            result = compare_image(left_dir / name, right_dir / name, diff_path, diff_scale)
            if diff_path is not None and result["status"] == "ok":
                result["diff_path"] = str(diff_path)
        except (OSError, PngError, zlib.error) as error:
            result = {"name": name, "status": "decode-error", "error": str(error)}

        if result["status"] != "ok":
            failed = True
        threshold_failures = []
        if max_rms is not None and result.get("rms_channel_delta", 0.0) > max_rms:
            threshold_failures.append("rms")
            failed = True
        if max_mean_abs is not None and result.get("mean_abs_channel_delta", 0.0) > max_mean_abs:
            threshold_failures.append("mean-abs")
            failed = True
        if max_changed_percent is not None and result.get("changed_percent", 0.0) > max_changed_percent:
            threshold_failures.append("changed-percent")
            failed = True
        if threshold_failures:
            result["status"] = "threshold-failed"
            result["threshold_failures"] = threshold_failures
        results.append(result)

    return results, failed


def self_test_pixels(width: int, height: int, rgba: tuple[int, int, int, int]) -> bytes:
    return bytes(rgba) * width * height


def varied_self_test_pixels(width: int, height: int) -> bytes:
    pixels = bytearray()
    for index in range(width * height):
        pixels.extend((index & 0xFF, (index * 3) & 0xFF, (255 - index) & 0xFF, 255))
    return bytes(pixels)


def diverse_real_preview_self_test_pixels(width: int, height: int) -> bytes:
    pixels = bytearray()
    for y in range(height):
        for x in range(width):
            block = (x // 2 + (y // 2) * max(1, width // 2)) & 0x3F
            pixels.extend(
                (
                    (32 + (block & 0x03) * 48) & 0xFF,
                    (48 + ((block >> 2) & 0x03) * 44) & 0xFF,
                    (64 + ((block >> 4) & 0x03) * 40) & 0xFF,
                    255,
                )
            )
    return bytes(pixels)


def low_diversity_real_preview_self_test_pixels(width: int, height: int) -> bytes:
    pixels = bytearray()
    for index in range(width * height):
        if index % 7 == 0:
            pixels.extend((188, 48, 48, 255))
        else:
            pixels.extend((180, 40, 40, 255))
    return bytes(pixels)


def one_pixel_outlier_self_test_pixels(width: int, height: int) -> bytes:
    pixels = bytearray(self_test_pixels(width, height, (10, 20, 30, 255)))
    pixels[0:4] = bytes((200, 210, 220, 255))
    return bytes(pixels)


def semantic_synthetic_self_test_pixels(width: int, height: int) -> bytes:
    pixels = bytearray()
    semantic_colors = [
        (32, 180, 32, 255),
        (32, 64, 220, 255),
        (240, 240, 240, 255),
    ]
    for index in range(width * height):
        if index < len(semantic_colors):
            pixels.extend(semantic_colors[index])
        else:
            value = 48 + (index % 80)
            pixels.extend((value, 42 + (index % 40), 36 + (index % 32), 255))
    return bytes(pixels)


def nonsemantic_synthetic_self_test_pixels(width: int, height: int) -> bytes:
    pixels = bytearray()
    for index in range(width * height):
        value = 48 + (index % 80)
        pixels.extend((value, 42 + (index % 40), 36 + (index % 32), 255))
    return bytes(pixels)


def require_self_test(condition: bool, message: str) -> None:
    if not condition:
        raise AssertionError(message)


def run_self_test() -> int:
    with tempfile.TemporaryDirectory(prefix="hivewe-artifact-compare-") as temp_dir:
        root = Path(temp_dir)
        left_dir = root / "left"
        right_dir = root / "right"
        diff_dir = root / "diff"
        left_dir.mkdir()
        right_dir.mkdir()

        encode_png_rgba(left_dir / "same.png", 2, 2, self_test_pixels(2, 2, (10, 20, 30, 255)))
        encode_png_rgba(right_dir / "same.png", 2, 2, self_test_pixels(2, 2, (10, 20, 30, 255)))
        encode_png_rgba(left_dir / "changed.png", 2, 2, self_test_pixels(2, 2, (10, 20, 30, 255)))
        encode_png_rgba(right_dir / "changed.png", 2, 2, self_test_pixels(2, 2, (40, 20, 30, 255)))
        encode_png_rgba(left_dir / "dimension.png", 1, 1, self_test_pixels(1, 1, (0, 0, 0, 255)))
        encode_png_rgba(right_dir / "dimension.png", 2, 1, self_test_pixels(2, 1, (0, 0, 0, 255)))
        encode_png_rgba(left_dir / "left-only.png", 1, 1, self_test_pixels(1, 1, (0, 0, 0, 255)))

        results, failed = compare_artifact_directories(
            left_dir,
            right_dir,
            patterns=[],
            exact_names={"same.png"},
            common_only=False,
            diff_dir=diff_dir,
            diff_scale=4.0,
            max_rms=None,
            max_mean_abs=None,
            max_changed_percent=None,
        )
        require_self_test(not failed, "identical PNGs should pass")
        require_self_test(results[0]["status"] == "ok", "identical PNG status should be ok")
        require_self_test((diff_dir / "same.diff.png").is_file(), "diff PNG should be written for comparable images")

        results, failed = compare_artifact_directories(
            left_dir,
            right_dir,
            patterns=[],
            exact_names={"changed.png"},
            common_only=False,
            diff_dir=None,
            diff_scale=4.0,
            max_rms=0.0,
            max_mean_abs=None,
            max_changed_percent=None,
        )
        require_self_test(failed, "threshold failure should fail comparison")
        require_self_test(results[0]["status"] == "threshold-failed", "threshold failure should update result status")
        require_self_test("rms" in results[0].get("threshold_failures", []), "RMS threshold should be reported")

        results, failed = compare_artifact_directories(
            left_dir,
            right_dir,
            patterns=[],
            exact_names={"dimension.png", "left-only.png", "missing.png"},
            common_only=False,
            diff_dir=None,
            diff_scale=4.0,
            max_rms=None,
            max_mean_abs=None,
            max_changed_percent=None,
        )
        by_name = {result["name"]: result for result in results}
        require_self_test(failed, "missing and dimension mismatches should fail comparison")
        require_self_test(by_name["dimension.png"]["status"] == "dimension-mismatch", "dimension mismatch should be reported")
        require_self_test(by_name["left-only.png"]["status"] == "missing-right", "missing candidate should be reported")
        require_self_test(by_name["missing.png"]["status"] == "missing-both", "missing required artifact should be reported")

        manifest_dir = root / "manifest"
        manifest_dir.mkdir()
        encode_png_rgba(manifest_dir / "fixed.png", 8, 8, varied_self_test_pixels(8, 8))
        encode_png_rgba(manifest_dir / "variable.png", 8, 8, varied_self_test_pixels(8, 8))
        required_count = validate_png_artifact_manifest(
            manifest_dir,
            {"fixed.png": (8, 8), "variable.png": None},
            variable_size=(8, 8),
        )
        require_self_test(required_count == 2, "manifest validation should report the required artifact count")

        encode_png_rgba(manifest_dir / "extra.png", 8, 8, varied_self_test_pixels(8, 8))
        extra_failed = False
        try:
            validate_png_artifact_manifest(manifest_dir, {"fixed.png": (8, 8)}, allow_extra=False)
        except PngError as error:
            extra_failed = "unexpected" in str(error) and "extra.png" in str(error)
        require_self_test(extra_failed, "manifest validation should reject unexpected PNG artifacts by default")
        allow_extra_count = validate_png_artifact_manifest(manifest_dir, {"fixed.png": (8, 8)}, allow_extra=True)
        require_self_test(allow_extra_count == 1, "manifest validation should optionally allow extra PNG artifacts")

        encode_png_rgba(manifest_dir / "flat.png", 8, 8, self_test_pixels(8, 8, (10, 20, 30, 255)))
        flat_failed = False
        try:
            validate_png_artifact_manifest(manifest_dir, {"flat.png": (8, 8)})
        except PngError as error:
            flat_failed = "too flat" in str(error)
        require_self_test(flat_failed, "manifest validation should reject flat PNGs")

        encode_png_rgba(manifest_dir / "one-pixel-outlier.png", 8, 8, one_pixel_outlier_self_test_pixels(8, 8))
        _width, _height, outlier_varied = count_varied_pixels(manifest_dir / "one-pixel-outlier.png")
        require_self_test(outlier_varied == 1, "one-pixel outlier should count as one varied pixel")
        outlier_failed = False
        try:
            validate_png_artifact_manifest(manifest_dir, {"one-pixel-outlier.png": (8, 8)})
        except PngError as error:
            outlier_failed = "too flat" in str(error)
        require_self_test(outlier_failed, "manifest validation should reject nearly flat PNGs with one outlier")
        encode_png_rgba(manifest_dir / "object-grid-empty-filter-preview.png", 8, 8, self_test_pixels(8, 8, (5, 10, 15, 255)))
        validate_png_artifact_manifest(manifest_dir, {"object-grid-empty-filter-preview.png": (8, 8)}, allow_extra=True)
        encode_png_rgba(
            manifest_dir / "model-editor-preview.png",
            16,
            16,
            diverse_real_preview_self_test_pixels(16, 16),
        )
        validate_png_artifact_manifest(manifest_dir, {"model-editor-preview.png": (16, 16)}, allow_extra=True)

        encode_png_rgba(
            manifest_dir / "object-grid-preview.png",
            16,
            16,
            low_diversity_real_preview_self_test_pixels(16, 16),
        )
        non_textured_preview_failed = False
        try:
            validate_png_artifact_manifest(manifest_dir, {"object-grid-preview.png": (16, 16)}, allow_extra=True)
        except PngError as error:
            non_textured_preview_failed = "textured real model preview" in str(error)
        require_self_test(
            non_textured_preview_failed,
            "manifest validation should reject non-textured real model preview captures",
        )

        encode_png_rgba(
            manifest_dir / "object-grid-scrolled-material-preview.png",
            16,
            16,
            semantic_synthetic_self_test_pixels(16, 16),
        )
        validate_png_artifact_manifest(
            manifest_dir,
            {"object-grid-scrolled-material-preview.png": (16, 16)},
            allow_extra=True,
        )

        encode_png_rgba(
            manifest_dir / "object-grid-scrolled-material-preview.png",
            16,
            16,
            nonsemantic_synthetic_self_test_pixels(16, 16),
        )
        semantic_preview_failed = False
        try:
            validate_png_artifact_manifest(
                manifest_dir,
                {"object-grid-scrolled-material-preview.png": (16, 16)},
                allow_extra=True,
            )
        except PngError as error:
            semantic_preview_failed = "synthetic render colors" in str(error)
        require_self_test(
            semantic_preview_failed,
            "manifest validation should reject synthetic preview artifacts without expected semantic colors",
        )

    print("compare_smoke_artifacts self-test passed.")
    return 0


def main() -> int:
    parser = argparse.ArgumentParser(description="Compare HiveWE smoke artifact PNG directories.")
    parser.add_argument("left_dir", type=Path, nargs="?", help="Reference artifact directory, usually OpenGL.")
    parser.add_argument("right_dir", type=Path, nargs="?", help="Candidate artifact directory, usually Metal.")
    parser.add_argument(
        "--include",
        action="append",
        default=[],
        metavar="GLOB",
        help="PNG filename glob to compare. Can be repeated. Defaults to *.png.",
    )
    parser.add_argument(
        "--preset",
        action="append",
        choices=sorted(ARTIFACT_PRESETS),
        default=[],
        help="Compare a named HiveWE artifact set. Can be repeated.",
    )
    parser.add_argument(
        "--require",
        action="append",
        default=[],
        metavar="PNG",
        help="Exact PNG filename that must be present in both directories. Can be repeated.",
    )
    parser.add_argument("--common-only", action="store_true", help="Compare only filenames present in both directories.")
    parser.add_argument("--max-rms", type=float, default=None, help="Fail if any image RMS channel delta exceeds this value.")
    parser.add_argument("--max-mean-abs", type=float, default=None, help="Fail if any image mean absolute channel delta exceeds this value.")
    parser.add_argument("--max-changed-percent", type=float, default=None, help="Fail if any image changed-pixel percentage exceeds this value.")
    parser.add_argument("--diff-dir", type=Path, default=None, help="Write amplified per-pixel diff PNGs to this directory.")
    parser.add_argument("--diff-scale", type=float, default=4.0, help="Multiplier for diff PNG channel deltas.")
    parser.add_argument("--json-output", type=Path, default=None, help="Write detailed comparison results to this JSON file.")
    parser.add_argument("--markdown-output", type=Path, default=None, help="Write a maintainer-facing Markdown summary report.")
    parser.add_argument(
        "--validate-manifest",
        type=Path,
        default=None,
        metavar="DIR",
        help="Validate one artifact directory against a named required PNG manifest and exit.",
    )
    parser.add_argument(
        "--manifest-preset",
        choices=sorted(MANIFEST_PRESETS),
        default=None,
        help="Required PNG manifest to use with --validate-manifest.",
    )
    parser.add_argument(
        "--variable-size-from",
        type=Path,
        default=None,
        metavar="PNG",
        help="PNG whose dimensions provide the expected size for variable-size manifest entries.",
    )
    parser.add_argument(
        "--allow-extra",
        action="store_true",
        help="Allow extra PNG files when using --validate-manifest.",
    )
    parser.add_argument("--self-test", action="store_true", help="Run dependency-free PNG comparison self-tests and exit.")
    args = parser.parse_args()

    if args.self_test:
        return run_self_test()

    if args.validate_manifest is not None:
        if args.manifest_preset is None:
            parser.error("--validate-manifest requires --manifest-preset")
        if not args.validate_manifest.is_dir():
            parser.error(f"{args.validate_manifest} is not a directory")
        try:
            variable_size = read_png_size(args.variable_size_from) if args.variable_size_from is not None else None
            count = validate_png_artifact_manifest(
                args.validate_manifest,
                MANIFEST_PRESETS[args.manifest_preset],
                variable_size=variable_size,
                label=f"{args.manifest_preset} PNG artifacts",
                allow_extra=args.allow_extra,
            )
        except (OSError, PngError, zlib.error) as error:
            parser.error(str(error))
        print(f"Validated {count} {args.manifest_preset} PNG artifacts in {args.validate_manifest}.")
        return 0

    if args.manifest_preset is not None or args.variable_size_from is not None or args.allow_extra:
        parser.error("--manifest-preset, --variable-size-from, and --allow-extra require --validate-manifest")

    if args.left_dir is None or args.right_dir is None:
        parser.error("left_dir and right_dir are required unless --self-test is used")

    exact_names = set(args.require)
    for preset in args.preset:
        exact_names.update(ARTIFACT_PRESETS[preset])

    patterns = args.include or ([] if exact_names else ["*.png"])
    if not args.left_dir.is_dir():
        parser.error(f"{args.left_dir} is not a directory")
    if not args.right_dir.is_dir():
        parser.error(f"{args.right_dir} is not a directory")

    try:
        results, failed = compare_artifact_directories(
            args.left_dir,
            args.right_dir,
            patterns=patterns,
            exact_names=exact_names,
            common_only=args.common_only,
            diff_dir=args.diff_dir,
            diff_scale=args.diff_scale,
            max_rms=args.max_rms,
            max_mean_abs=args.max_mean_abs,
            max_changed_percent=args.max_changed_percent,
        )
    except PngError as error:
        parser.error(str(error))

    print(f"{'artifact':45} {'status':18} {'changed%':>10} {'mean':>10} {'rms':>10} {'max':>5}")
    for result in results:
        print(
            f"{result['name'][:45]:45} "
            f"{result['status']:18} "
            f"{result.get('changed_percent', 0.0):10.3f} "
            f"{result.get('mean_abs_channel_delta', 0.0):10.3f} "
            f"{result.get('rms_channel_delta', 0.0):10.3f} "
            f"{result.get('max_channel_delta', 0):5}"
        )

    ok_count = sum(1 for result in results if result["status"] == "ok")
    worst_rms = max((result.get("rms_channel_delta", 0.0) for result in results), default=0.0)
    worst_mean = max((result.get("mean_abs_channel_delta", 0.0) for result in results), default=0.0)
    worst_changed = max((result.get("changed_percent", 0.0) for result in results), default=0.0)
    print(
        f"Compared {len(results)} artifacts: {ok_count} ok, {len(results) - ok_count} failed; "
        f"worst changed {worst_changed:.3f}%, mean {worst_mean:.3f}, rms {worst_rms:.3f}."
    )

    if args.json_output is not None:
        args.json_output.parent.mkdir(parents=True, exist_ok=True)
        args.json_output.write_text(json.dumps(results, indent=2) + "\n", encoding="utf-8")
    if args.markdown_output is not None:
        write_markdown_report(
            args.markdown_output,
            args.left_dir,
            args.right_dir,
            results,
            ok_count,
            worst_changed,
            worst_mean,
            worst_rms,
            failed,
        )

    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())
