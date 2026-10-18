---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "m3_validator.py"
ref: HEAD
---

"""SC2 M3 model validator — checks exported models against SC2 engine limits."""
from __future__ import annotations
import os, struct
from typing import List, Dict


SC2_LIMITS = {
    "max_triangles": 65536,
    "max_bones": 256,
    "max_texture_dimension": 2048,
    "max_material_layers": 4,
    "max_file_size_mb": 50,
}


def validate_m3(m3_path: str) -> Dict[str, List[str]]:
    """Validate an exported .m3 against SC2 engine limits.

    Returns {"errors": [...], "warnings": [...]}.
    """
    errors: List[str] = []
    warnings: List[str] = []

    if not os.path.exists(m3_path):
        errors.append(f"M3 file not found: {m3_path}")
        return {"errors": errors, "warnings": warnings}

    size_mb = os.path.getsize(m3_path) / (1024 * 1024)
    if size_mb > SC2_LIMITS["max_file_size_mb"]:
        warnings.append(f"M3 file is {size_mb:.1f}MB (recommended < {SC2_LIMITS['max_file_size_mb']}MB)")

    # Parse M3 header to extract basic stats
    try:
        with open(m3_path, "rb") as f:
            header = f.read(16)
            if header[:4] != b"MD34":
                errors.append("Not a valid M3 file (bad magic)")
                return {"errors": errors, "warnings": warnings}

            tag, ofs, entries, _ = struct.unpack_from("<4sIII", header, 4)
            # M3 tag is "MD34" reversed in the reference table
            ref_offset = struct.unpack_from("<I", header, 12)[0]

            # Read reference table for model stats
            f.seek(ref_offset)
            model_data = f.read(1024)  # first KB of refs
            # Count MD34 entries (model sections)
            section_count = model_data.count(b"MD34")

            if section_count == 0:
                errors.append("M3 file appears empty (no model sections)")
    except Exception as e:
        errors.append(f"Failed to parse M3 header: {e}")

    return {"errors": errors, "warnings": warnings}


def validate_texture_dims(dds_path: str, max_dim: int = 2048) -> List[str]:
    """Check DDS texture dimensions against SC2 limits."""
    warnings = []
    if not os.path.exists(dds_path):
        return [f"Texture not found: {dds_path}"]
    try:
        with open(dds_path, "rb") as f:
            if f.read(4) != b"DDS ":
                return [f"Not a valid DDS: {dds_path}"]
            f.seek(12)
            h, w = struct.unpack_from("<II", f.read(8))
            if w > max_dim or h > max_dim:
                warnings.append(f"Texture {os.path.basename(dds_path)} is {w}x{h} (max recommended: {max_dim})")
    except Exception:
        pass
    return warnings
