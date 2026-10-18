---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "blp.py"
ref: HEAD
---

"""Minimal Warcraft 3 BLP (BLP1) texture decoder -> PIL RGBA Image.

WC3 ships textures as BLP1, in two flavours:
  - JPEG content   (compression==0): a shared JPEG header + per-mip JPEG body, stored BGRA.
  - Palettized      (compression==1): a 256-entry BGRA palette + indexed pixels (+ optional alpha plane).

Only mip level 0 (full resolution) is decoded; SC2's DDS will get a fresh mip chain built by textures.py.
BLP2 (the World of Warcraft format) is detected and rejected — this tool targets WC3 models.

No external deps beyond Pillow + numpy.
"""
from __future__ import annotations
import struct, io
from typing import Dict
import numpy as np
from PIL import Image


def _read_header(d: bytes) -> Dict[str, int | bytes]:
    magic = d[:4]
    if magic == b"BLP2":
        raise ValueError("BLP2 (WoW) is not supported; this tool handles WC3 BLP1 textures")
    if magic != b"BLP1":
        raise ValueError("not a BLP1 file (magic=%r)" % magic)
    compression, alpha_bits, width, height, pic_type, pic_subtype = struct.unpack_from("<6I", d, 4)
    mip_offsets = struct.unpack_from("<16I", d, 28)
    mip_sizes = struct.unpack_from("<16I", d, 92)
    return dict(compression=compression, alpha_bits=alpha_bits, width=width, height=height,
                pic_type=pic_type, mip_offsets=mip_offsets, mip_sizes=mip_sizes)


def _decode_jpeg(d: bytes, h: Dict[str, int | bytes]) -> np.ndarray:
    """JPEG-content BLP1: shared header at 156 (len prefix) + mip0 body; pixels are BGRA."""
    jpeg_header_size = struct.unpack_from("<I", d, 156)[0]
    header = d[160:160 + jpeg_header_size]
    body = d[h["mip_offsets"][0]:h["mip_offsets"][0] + h["mip_sizes"][0]]
    im = Image.open(io.BytesIO(header + body))
    arr = np.array(im)
    if arr.ndim == 2:  # grayscale
        rgb = np.dstack([arr, arr, arr]); a = np.full(arr.shape, 255, np.uint8)
    elif arr.shape[2] == 4:  # BGRA -> RGBA  (WC3 stores BGRA; the 4th channel is inverted alpha)
        b, g, r, a = arr[..., 0], arr[..., 1], arr[..., 2], arr[..., 3]
        rgb = np.dstack([r, g, b]); a = 255 - a
    else:  # BGR -> RGB
        rgb = arr[..., ::-1]; a = np.full(arr.shape[:2], 255, np.uint8)
    return np.dstack([rgb, a]).astype(np.uint8)


def _decode_paletted(d: bytes, h: Dict[str, int | bytes]) -> np.ndarray:
    """Palettized BLP1: 256 BGRA palette at offset 156, then indexed pixels at mip0, then alpha plane."""
    w, ht = h["width"], h["height"]
    pal = np.frombuffer(d, np.uint8, 256 * 4, 156).reshape(256, 4)  # BGRA
    off = h["mip_offsets"][0]
    n = w * ht
    idx = np.frombuffer(d, np.uint8, n, off).reshape(ht, w)
    rgb = pal[idx][..., :3][..., ::-1]  # BGR -> RGB
    if h["alpha_bits"] >= 8:
        a = np.frombuffer(d, np.uint8, n, off + n).reshape(ht, w)
    elif h["alpha_bits"] == 1:
        bits = np.unpackbits(np.frombuffer(d, np.uint8, (n + 7) // 8, off + n))[:n]
        a = (bits.reshape(ht, w) * 255).astype(np.uint8)
    else:
        a = np.full((ht, w), 255, np.uint8)
    return np.dstack([rgb, a]).astype(np.uint8)


def decode_blp(path: str) -> Image.Image:
    """Decode a .blp file to a PIL RGBA Image (mip 0)."""
    d = open(path, "rb").read()
    h = _read_header(d)
    rgba = _decode_jpeg(d, h) if h["compression"] == 0 else _decode_paletted(d, h)
    return Image.fromarray(rgba, "RGBA")


if __name__ == "__main__":
    import sys
    img = decode_blp(sys.argv[1])
    print("decoded", sys.argv[1], img.size, img.mode)
    if len(sys.argv) > 2:
        img.save(sys.argv[2]); print("wrote", sys.argv[2])
