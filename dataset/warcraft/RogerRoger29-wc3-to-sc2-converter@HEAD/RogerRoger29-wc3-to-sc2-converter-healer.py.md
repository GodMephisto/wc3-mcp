---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "healer.py"
ref: HEAD
---

"""Self-healing engine: automatically fix common WC3 model issues.

Each fix is a pure function that takes MDX data (or relevant subset) and returns
the corrected version along with a description of what was changed.
"""
from __future__ import annotations
import os
from typing import Dict, List, Any, Tuple, Optional
import numpy as np
from PIL import Image


def fix_degenerate_faces(geosets: List[Dict]) -> Tuple[List[Dict], int]:
    """Remove degenerate (zero-area) faces from all geosets.

    Returns (fixed_geosets, removed_count).
    """
    total_removed = 0
    for g in geosets:
        faces = g.get("faces", [])
        clean = []
        for f in faces:
            if len(f) >= 3 and len(set(f)) >= 3:
                clean.append(f)
            else:
                total_removed += 1
        g["faces"] = clean
        g["ntris"] = len(clean)
    return geosets, total_removed


def fix_bone_hierarchy(bones: List[Dict], helpers: List[Dict],
                       geosets: List[Dict]) -> Tuple[List[Dict], List[str]]:
    """Repair bone hierarchy issues.

    Fixes applied:
      - Create a dummy root if none exists
      - Re-parent orphaned bones to root
      - Break circular parent references

    Returns (fixed_bones, log_messages).
    """
    log: List[str] = []
    all_nodes = {n["objectId"]: n for n in bones + helpers}

    # Find root(s)
    roots = [b for b in bones if b.get("parentId") is None]
    if not roots:
        # Create dummy root
        max_id = max((b["objectId"] for b in bones), default=-1)
        dummy_id = max_id + 1000
        dummy = {
            "name": "_root_auto", "objectId": dummy_id, "parentId": None,
            "flags": 0, "tracks": {},
        }
        bones.insert(0, dummy)
        log.append(f"Created dummy root bone '_root_auto' (id={dummy_id})")
        root_id = dummy_id
    else:
        root_id = roots[0]["objectId"]

    # Fix orphans: bones whose parent doesn't exist
    for b in bones:
        pid = b.get("parentId")
        if pid is not None and pid not in all_nodes:
            b["parentId"] = root_id
            log.append(f"Re-parented orphan bone '{b['name']}' (parent {pid} not found) → root")

    # Detect and break cycles (simple Floyd-like: follow parent chain, max 100 steps)
    for b in bones:
        visited = set()
        current = b
        steps = 0
        while current.get("parentId") is not None and steps < 100:
            steps += 1
            pid = current["parentId"]
            if pid in visited:
                current["parentId"] = root_id
                log.append(f"Broke parent cycle at bone '{b['name']}' → re-parented to root")
                break
            visited.add(pid)
            current = all_nodes.get(pid, {})
            if not current:
                break

    return bones, log


def fix_alpha_invert(img: Image.Image) -> Tuple[Image.Image, bool]:
    """Auto-detect and fix inverted alpha in a texture.

    Heuristic: sample border pixels. If >80% of border pixels have alpha>200,
    the alpha is likely inverted (WC3 stores alpha inverted in BLP).

    Returns (fixed_image, was_fixed).
    """
    arr = np.array(img)
    if arr.shape[2] < 4:
        return img, False
    alpha = arr[..., 3]
    h, w = alpha.shape

    # Sample border (1px perimeter)
    border = np.concatenate([
        alpha[0, :], alpha[-1, :],
        alpha[1:-1, 0], alpha[1:-1, -1],
    ])
    if len(border) == 0:
        return img, False

    opaque_ratio = (border > 200).mean()
    if opaque_ratio > 0.8:
        # Border is mostly opaque → alpha is likely inverted
        arr[..., 3] = 255 - arr[..., 3]
        return Image.fromarray(arr, "RGBA"), True
    return img, False


def fix_texture_paths(mdx_data: Dict, found_textures: Dict[str, str]) -> Tuple[Dict, List[str]]:
    """Update TEXS paths to point to found texture files.

    Returns (fixed_mdx_data, log_messages).
    """
    log: List[str] = []
    for t in mdx_data.get("textures", []):
        if t.get("replaceableId", 0) in (1, 2):
            continue
        path = t.get("path", "")
        if not path:
            continue
        basename = os.path.basename(path.replace("\\", "/"))
        if basename in found_textures:
            t["_resolved_path"] = found_textures[basename]
    return mdx_data, log


def fix_particle_lifespan(particles: List[Dict]) -> Tuple[List[Dict], int]:
    """Clamp extreme particle lifespans to sane range [0.3, 8.0].

    Returns (fixed_particles, clamped_count).
    """
    clamped = 0
    for p in particles:
        life = p.get("lifespan", 1.0)
        if life < 0.3:
            p["lifespan"] = 0.3
            clamped += 1
        elif life > 8.0:
            p["lifespan"] = 8.0
            clamped += 1
    return particles, clamped


def apply_all_fixes(mdx_data: Dict, geosets: List[Dict],
                    bones: List[Dict], helpers: List[Dict],
                    particles: List[Dict],
                    texture_image: Optional[Image.Image] = None,
                    found_textures: Optional[Dict[str, str]] = None) -> Dict:
    """Run all self-healing fixes and return a summary of what was changed.

    Returns a dict: {'fixes_applied': [...], 'data': fixed_mdx_data}
    """
    fixes: List[str] = []

    # Degenerate faces
    g2, removed = fix_degenerate_faces(geosets)
    if removed:
        fixes.append(f"Removed {removed} degenerate face(s)")
    mdx_data["geosets"] = g2

    # Bone hierarchy
    b2, bone_log = fix_bone_hierarchy(bones, helpers, geosets)
    mdx_data["bones"] = b2
    fixes.extend(bone_log)

    # Particle lifespans
    p2, clamped = fix_particle_lifespan(particles)
    if clamped:
        fixes.append(f"Clamped {clamped} extreme particle lifespan(s) to [0.3, 8.0]")
    mdx_data["particles"] = p2

    # Alpha invert (must be applied externally to the image)
    if texture_image is not None:
        _, was_fixed = fix_alpha_invert(texture_image)
        if was_fixed:
            fixes.append("Auto-inverted alpha channel")

    # Texture paths
    if found_textures:
        _, tex_log = fix_texture_paths(mdx_data, found_textures)
        fixes.extend(tex_log)

    return {"fixes_applied": fixes or ["No fixes needed"], "data": mdx_data}
