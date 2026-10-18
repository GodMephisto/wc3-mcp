---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "team_color_mask.py"
ref: HEAD
---

"""Team color mask generator — properly isolates team-color geometry regions.

Instead of masking the TEAMEMIS emissive with the full diffuse texture (which makes
the entire model glow), this generates a proper UV-space mask where only the
team-color geoset regions are white and everything else is black.
"""
from __future__ import annotations
import numpy as np
from PIL import Image, ImageDraw
from typing import Dict, List, Tuple, Optional


def generate_team_color_mask(mdx_data: Dict, mask_size: int = 512) -> Optional[Image.Image]:
    """Generate a white-on-black mask image for team-color emissive regions.

    Analyzes which geosets use team-color materials (replaceableId 1 or 2) and
    renders their UV islands as white on a black background at the given resolution.

    Returns a PIL RGBA Image (white where team color, black elsewhere, alpha=255)
    or None if the model has no team-color geosets.
    """
    textures = mdx_data.get("textures", [])
    materials = mdx_data.get("materials", [])
    geosets = mdx_data.get("geosets", [])

    # Find which material indices are team-color
    team_mat_ids: set[int] = set()
    for mi, mat in enumerate(materials):
        for layer in mat.get("layers", []):
            tid = layer.get("textureId", -1)
            if 0 <= tid < len(textures):
                if textures[tid].get("replaceableId") in (1, 2):
                    team_mat_ids.add(mi)

    if not team_mat_ids:
        return None

    # Collect UV triangles from team-color geosets
    mask = Image.new("L", (mask_size, mask_size), 0)
    draw = ImageDraw.Draw(mask)

    for g in geosets:
        if g.get("materialId", -1) not in team_mat_ids:
            continue

        uvs = g.get("uvs", [])
        faces = g.get("faces", [])
        if not uvs or not faces:
            continue

        for face in faces:
            if len(face) < 3:
                continue
            pts = []
            for vi in face:
                if vi < len(uvs):
                    u, v = uvs[vi]
                    # Clamp UV to [0,1] and scale to mask resolution
                    px = int(max(0, min(1, u)) * (mask_size - 1))
                    py = int(max(0, min(1, v)) * (mask_size - 1))
                    pts.append((px, py))
            if len(pts) >= 3:
                draw.polygon(pts, fill=255)

    # Slight dilation to avoid seams (1px)
    arr = np.array(mask)
    dilated = np.zeros_like(arr)
    for dy in (-1, 0, 1):
        for dx in (-1, 0, 1):
            shifted = np.roll(np.roll(arr, dx, axis=1), dy, axis=0)
            dilated = np.maximum(dilated, shifted)
    mask = Image.fromarray(dilated, "L")

    # Convert to RGBA (white mask, full alpha)
    rgba = Image.new("RGBA", (mask_size, mask_size), (0, 0, 0, 0))
    rgba.paste((255, 255, 255, 255), (0, 0), mask)
    return rgba


def save_team_color_mask(mdx_data: Dict, output_path: str, mask_size: int = 512) -> bool:
    """Generate and save a team color mask DDS. Returns True if mask was created."""
    mask = generate_team_color_mask(mdx_data, mask_size)
    if mask is None:
        return False
    # Save as PNG first, then convert to DDS
    import textures as tex
    tex.convert_texture(
        output_path.replace(".dds", ".png") if output_path.endswith(".dds") else output_path,
        output_path if output_path.endswith(".dds") else output_path + ".dds",
        alpha_invert=False, glow=False)
    return True
