---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "normal_map_gen.py"
ref: HEAD
---

"""Normal map generation from diffuse textures using Sobel edge detection.

Produces tangent-space normal maps suitable for SC2 PBR materials.
Pure NumPy — no scipy dependency.
"""
from __future__ import annotations
import numpy as np
from PIL import Image
from typing import Tuple


def _box_blur(arr: np.ndarray, radius: int = 1) -> np.ndarray:
    """Simple box blur via convolution — no scipy needed."""
    if radius <= 0:
        return arr
    kernel = np.ones((2 * radius + 1, 2 * radius + 1), dtype=np.float32)
    kernel /= kernel.sum()
    h, w = arr.shape
    kh, kw = kernel.shape
    pad_h, pad_w = kh // 2, kw // 2
    padded = np.pad(arr, ((pad_h, pad_h), (pad_w, pad_w)), mode="edge")
    result = np.zeros_like(arr)
    for i in range(h):
        for j in range(w):
            result[i, j] = (padded[i:i+kh, j:j+kw] * kernel).sum()
    return result


def sobel_normal_map(img: Image.Image, strength: float = 1.0,
                     blur_radius: float = 0.0) -> Image.Image:
    """Generate a tangent-space normal map from a diffuse/albedo texture.

    Uses Sobel operators to detect height changes, then constructs normal vectors.
    Higher strength = more pronounced normals.

    Returns a new RGBA Image with normals encoded as RGB (X,Y,Z) and alpha=255.
    """
    arr = np.array(img.convert("L")).astype(np.float32) / 255.0
    h, w = arr.shape

    # Optional pre-blur
    if blur_radius > 0:
        arr = _box_blur(arr, max(1, int(blur_radius)))

    # Sobel gradients
    gy = np.zeros_like(arr)
    gx = np.zeros_like(arr)
    gy[1:-1, :] = arr[2:, :] - arr[:-2, :]
    gx[:, 1:-1] = arr[:, 2:] - arr[:, :-2]

    # Scale by strength
    gx *= strength
    gy *= strength

    # Build normal vectors: (-gx, -gy, 1) normalized
    z = np.ones_like(arr)
    mag = np.sqrt(gx * gx + gy * gy + z * z)
    nx = -gx / mag
    ny = -gy / mag
    nz = z / mag

    # Map from [-1,1] to [0,255]
    r = ((nx + 1) * 127.5).clip(0, 255).astype(np.uint8)
    g = ((ny + 1) * 127.5).clip(0, 255).astype(np.uint8)
    b = ((nz + 1) * 127.5).clip(0, 255).astype(np.uint8)
    a = np.full((h, w), 255, dtype=np.uint8)

    return Image.fromarray(np.dstack([r, g, b, a]), "RGBA")


def generate_normal_from_diffuse(diffuse_path: str, output_path: str,
                                 strength: float = 1.0) -> Tuple[int, int]:
    """Load a diffuse texture, generate a normal map, and save as DDS.

    Returns (width, height) of the output.
    """
    import textures as tex
    img = tex.load_image(diffuse_path)
    normal = sobel_normal_map(img, strength)
    n = tex.write_dds_dxt5_mipped(output_path, normal)
    return normal.size
