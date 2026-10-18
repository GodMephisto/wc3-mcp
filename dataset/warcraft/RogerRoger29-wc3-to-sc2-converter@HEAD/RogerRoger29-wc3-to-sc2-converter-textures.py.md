---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "textures.py"
ref: HEAD
---

"""Convert WC3 textures (BLP) or images (PNG/TGA) to SC2 DDS (DXT5 + full mip chain).

Pillow can DXT5-encode one image but not write a mip chain, so we encode each level and assemble the
DDS (header + concatenated blocks) ourselves.

Per-texture options:
  alpha_invert : flip the alpha channel (some WC3 textures store alpha inverted for a given use).
  glow         : particle/glow treatment — premultiply RGB by luminance and apply a radial fade with a
                 hard black border, and set alpha = luminance. This stops an additive sprite from
                 accumulating into a bright square; it fades to a round soft glow instead.
"""
from __future__ import annotations
import struct, io, os
from typing import Tuple
import numpy as np
from PIL import Image

try:
    from blp import decode_blp
except ImportError:
    from .blp import decode_blp


def _dxt5_blocks(img_rgba: Image.Image) -> bytes:
    buf = io.BytesIO()
    img_rgba.save(buf, format="DDS", pixel_format="DXT5")
    data = buf.getvalue()
    assert data[:4] == b"DDS ", "Pillow did not emit DDS (need Pillow with DDS write support)"
    return data[128:]


def write_dds_dxt5_mipped(path: str, base_rgba: Image.Image) -> int:
    """Write a DXT5 DDS with a full mip chain from a PIL RGBA image. Returns mip level count."""
    w, h = base_rgba.size
    levels, lw, lh = [], w, h
    while True:
        cur = base_rgba if (lw, lh) == (w, h) else base_rgba.resize((lw, lh), Image.LANCZOS)
        levels.append(_dxt5_blocks(cur))
        if lw == 1 and lh == 1:
            break
        lw, lh = max(1, lw // 2), max(1, lh // 2)
    blob = b"".join(levels)
    DDSD = 0x1 | 0x2 | 0x4 | 0x1000 | 0x20000 | 0x80000  # caps|h|w|pixfmt|mipcount|linearsize
    linsz = max(1, w // 4) * max(1, h // 4) * 16
    caps = 0x1000 | 0x400000 | 0x8  # texture|mipmap|complex
    hdr = bytearray(128)
    struct.pack_into("<4sIIIIIII", hdr, 0, b"DDS ", 124, DDSD, h, w, linsz, 0, len(levels))
    struct.pack_into("<II4s", hdr, 76, 32, 0x4, b"DXT5")
    struct.pack_into("<I", hdr, 108, caps)
    with open(path, "wb") as f:
        f.write(hdr); f.write(blob)
    return len(levels)


def _radial_glow(h: int, w: int, sigma: float = 0.30, cutoff: float = 0.80) -> np.ndarray:
    yy, xx = np.mgrid[0:h, 0:w].astype(np.float32)
    cy, cx = (h - 1) / 2.0, (w - 1) / 2.0
    r = np.sqrt(((xx - cx) / (w / 2.0)) ** 2 + ((yy - cy) / (h / 2.0)) ** 2)
    g = np.exp(-(r / sigma) ** 2 / 2.0)
    g[r > cutoff] = 0.0  # hard zero border -> no additive-accumulation square
    return g[..., None]


def load_image(src: str) -> Image.Image:
    """Load a BLP/PNG/TGA/etc. as a PIL RGBA image."""
    if src.lower().endswith(".blp"):
        return decode_blp(src)
    return Image.open(src).convert("RGBA")


def convert_texture(src: str, out_path: str, alpha_invert: bool = False,
                    glow: bool = False, glow_dim: float = 0.7) -> Tuple[Tuple[int, int], int]:
    """Convert a source texture to DDS DXT5. Returns ((width, height), mip_levels)."""
    img = load_image(src)
    arr = np.array(img).astype(np.float32)
    rgb, a = arr[..., :3], arr[..., 3]
    if glow:
        lum = (0.299 * rgb[..., 0] + 0.587 * rgb[..., 1] + 0.114 * rgb[..., 2]).clip(0, 255)
        f = (lum / 255.0)[..., None]
        gl = _radial_glow(*lum.shape)
        rgb = (rgb * f * gl * glow_dim).clip(0, 255)
        a = (lum * gl[..., 0]).clip(0, 255)
    if alpha_invert:
        a = 255.0 - a
    out = np.dstack([rgb, a]).clip(0, 255).astype(np.uint8)
    n = write_dds_dxt5_mipped(out_path, Image.fromarray(out, "RGBA"))
    return img.size, n


if __name__ == "__main__":
    import sys
    src, dst = sys.argv[1], sys.argv[2]
    size, mips = convert_texture(src, dst, alpha_invert="--invert" in sys.argv, glow="--glow" in sys.argv)
    print("wrote %s  %s mips=%d" % (dst, size, mips))
