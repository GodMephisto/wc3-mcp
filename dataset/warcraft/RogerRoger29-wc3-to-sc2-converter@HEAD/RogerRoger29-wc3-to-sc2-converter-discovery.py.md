---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "discovery.py"
ref: HEAD
---

"""Auto-discovery module: find textures, Blender, and estimate optimal scale.

All functions operate without user config — they scan the filesystem and the MDX data
to produce sensible defaults.  Used by both the CLI and GUI paths.
"""
from __future__ import annotations
import os, sys, glob, struct
from typing import List, Optional, Tuple, Dict


def find_texture_files(mdx_path: str, search_dirs: Optional[List[str]] = None,
                       texture_names: Optional[List[str]] = None) -> Dict[str, str]:
    """Search for texture files referenced by an MDX model.

    Tries (in order):
      1. Same directory as the .mdx
      2. Textures/ subdirectory
      3. Parent directory
      4. User-provided search_dirs
      5. Common WC3 texture paths

    Returns dict mapping basename → full path.
    """
    mdx_dir = os.path.dirname(os.path.abspath(mdx_path)) or "."
    found: Dict[str, str] = {}
    if texture_names is None:
        texture_names = []

    all_dirs = [mdx_dir, os.path.join(mdx_dir, "Textures"),
                os.path.dirname(mdx_dir)]
    if search_dirs:
        all_dirs.extend(search_dirs)

    # Windows WC3 install paths
    wc3_cands = [
        r"C:\Program Files (x86)\Warcraft III",
        r"C:\Program Files\Warcraft III",
        r"D:\Games\Warcraft III",
        r"E:\Games\Warcraft III",
    ]
    for wc3 in wc3_cands:
        if os.path.isdir(wc3):
            all_dirs.append(wc3)
            all_dirs.append(os.path.join(wc3, "Textures"))

    # Deduplicate while preserving order
    seen = set()
    unique_dirs = []
    for d in all_dirs:
        d = os.path.normpath(d)
        if d not in seen and os.path.isdir(d):
            seen.add(d)
            unique_dirs.append(d)

    for name in texture_names:
        basename = os.path.basename(name.replace("\\", "/"))
        if basename in found:
            continue
        for d in unique_dirs:
            candidate = os.path.join(d, basename)
            if os.path.exists(candidate):
                found[basename] = candidate
                break

    return found


def find_blender() -> Optional[str]:
    """Locate blender.exe on the system. Returns path or None."""
    if sys.platform == "win32":
        for base in (r"C:\Program Files\Blender Foundation",
                     r"C:\Program Files (x86)\Blender Foundation"):
            pattern = os.path.join(base, "Blender *", "blender.exe")
            cands = sorted(glob.glob(pattern), reverse=True)
            if cands:
                return cands[0]
    elif sys.platform == "darwin":
        for p in glob.glob("/Applications/Blender*.app/Contents/MacOS/Blender"):
            return p
    else:
        for p in glob.glob("/usr/local/bin/blender*"):
            return p
    return None


def estimate_scale(mdx_data: Dict) -> Tuple[float, str]:
    """Estimate the optimal SC2 scale factor from MDX model bounds.

    Returns (scale, confidence_level) where confidence is 'high', 'medium', or 'low'.

    Heuristics:
      - Hero/medium unit: bounds ~100-200 → 0.04-0.05
      - Large building: bounds ~300-600 → 0.06-0.08
      - Doodad/small: bounds ~20-80 → 0.03
      - Effect/particle: bounds ~5-30 → 0.02
    """
    model = mdx_data.get("model", {})
    bounds_r = model.get("boundsRadius", 100)
    bmin = model.get("min", [0, 0, 0])
    bmax = model.get("max", [100, 100, 100])
    extents = max(bmax[0] - bmin[0], bmax[1] - bmin[1], bmax[2] - bmin[2])

    # Check sequences for movement speed as additional signal
    seqs = mdx_data.get("sequences", [])
    avg_speed = 0.0
    if seqs:
        speeds = [s.get("moveSpeed", 0) for s in seqs if s.get("moveSpeed", 0) > 0]
        if speeds:
            avg_speed = sum(speeds) / len(speeds)

    if extents < 40:
        return (0.02, "medium")      # effect/doodad
    elif extents < 120:
        return (0.04, "high")        # standard unit
    elif extents < 250:
        return (0.05, "high")        # large unit / hero
    elif extents < 500:
        return (0.06, "medium")      # small building
    else:
        return (0.08, "low")         # large building


def discover_all(mdx_path: str, mdx_data: Dict,
                 extra_search_dirs: Optional[List[str]] = None
                 ) -> Dict:
    """Run all discovery and return a complete auto-config dict.

    Result has keys: textures_found, blender_path, estimated_scale, scale_confidence.
    """
    texture_paths = [t["path"] for t in mdx_data.get("textures", [])
                     if t.get("path") and t.get("replaceableId") not in (1, 2)]
    found = find_texture_files(mdx_path, extra_search_dirs, texture_paths)
    blender = find_blender()
    scale, confidence = estimate_scale(mdx_data)

    return {
        "textures_found": found,
        "textures_missing": [t for t in texture_paths
                             if os.path.basename(t.replace("\\", "/")) not in found],
        "blender_path": blender,
        "estimated_scale": scale,
        "scale_confidence": confidence,
    }
