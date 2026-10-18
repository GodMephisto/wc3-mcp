---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "lod_gen.py"
ref: HEAD
---

"""LOD (Level of Detail) generator — creates simplified mesh versions for SC2.

Uses edge collapse decimation on the parsed MDX geometry to produce LOD1 (50% tris)
and LOD2 (25% tris) meshes suitable for SC2 multi-LOD models.
"""
from __future__ import annotations
from typing import List, Dict, Tuple
import math


def _edge_length(v0: List[float], v1: List[float]) -> float:
    return math.sqrt((v1[0]-v0[0])**2 + (v1[1]-v0[1])**2 + (v1[2]-v0[2])**2)


def decimate_geoset(verts: List[List[float]], faces: List[List[int]],
                    target_ratio: float = 0.5) -> Tuple[List[List[float]], List[List[int]]]:
    """Simplify a geoset by collapsing shortest edges until target_ratio of
    original triangle count remains.

    Args:
        verts: list of [x,y,z] vertex positions
        faces: list of [v0,v1,v2] triangle indices
        target_ratio: fraction of original tri count to keep (0.5 = half)

    Returns (new_verts, new_faces).
    """
    if target_ratio >= 1.0 or len(faces) <= 4:
        return verts, faces

    target_tris = max(4, int(len(faces) * target_ratio))
    current_faces = [list(f) for f in faces]
    current_verts = [list(v) for v in verts]
    removed = [False] * len(current_verts)

    while len(current_faces) > target_tris:
        # Find shortest edge
        best_len = float("inf")
        best_edge = None
        edge_map: Dict[Tuple[int, int], int] = {}

        for fi, f in enumerate(current_faces):
            for i in range(3):
                a, b = f[i], f[(i+1) % 3]
                if a > b: a, b = b, a
                key = (a, b)
                edge_map[key] = edge_map.get(key, 0) + 1

        for (a, b), count in edge_map.items():
            if removed[a] or removed[b]:
                continue
            length = _edge_length(current_verts[a], current_verts[b])
            if length < best_len:
                best_len = length
                best_edge = (a, b)

        if best_edge is None:
            break

        # Collapse: merge vertex b into a
        a, b = best_edge
        mid = [(current_verts[a][i] + current_verts[b][i]) / 2.0 for i in range(3)]
        current_verts[a] = mid
        removed[b] = True

        # Remap faces
        new_faces = []
        for f in current_faces:
            nf = [(a if v == b else v) for v in f]
            if len(set(nf)) >= 3:
                new_faces.append(nf)
        current_faces = new_faces

        if best_len > 1e6:
            break

    return current_verts, current_faces


def generate_lods(mdx_data: Dict, levels: List[float] = None
                  ) -> Dict[int, Dict]:
    """Generate LOD levels for all geosets in an MDX model.

    Args:
        mdx_data: parsed MDX dict
        levels: list of triangle ratios per LOD (default: [0.5, 0.25])

    Returns dict: {lod_index: {"geosets": [...], "ratio": float}}
    """
    if levels is None:
        levels = [0.5, 0.25]

    geosets = mdx_data.get("geosets", [])
    lods = {}

    for lod_idx, ratio in enumerate(levels):
        lod_geosets = []
        for g in geosets:
            verts = g.get("verts", [])
            faces = g.get("faces", [])
            if not verts or not faces:
                lod_geosets.append(g)
                continue

            new_verts, new_faces = decimate_geoset(verts, faces, ratio)
            lod_geosets.append({
                "index": g["index"],
                "nverts": len(new_verts),
                "ntris": len(new_faces),
                "verts": new_verts,
                "faces": new_faces,
                "uvs": g.get("uvs", []),
                "materialId": g.get("materialId", 0),
                "vertexBones": g.get("vertexBones", []),
            })

        lods[lod_idx] = {
            "geosets": lod_geosets,
            "ratio": ratio,
            "total_tris": sum(g["ntris"] for g in lod_geosets),
        }

    return lods
