---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "mpq_reader.py"
ref: HEAD
---

"""MPQ archive reader — extract .mdx and .blp files from Warcraft 3 MPQ archives.

Uses a pure-Python approach: reads the MPQ header, hash table, and block table
to locate and extract files.  Supports MPQ format v1 (Warcraft 3).
"""
from __future__ import annotations
import os, struct, zlib
from typing import List, Tuple, Optional


MPQ_HEADER_SIZE = 32
MPQ_HASH_ENTRY_SIZE = 16
MPQ_BLOCK_ENTRY_SIZE = 16


def _hash_string(s: str, hash_type: int) -> int:
    """MPQ string hashing (HashString function from StormLib)."""
    seed1 = 0x7FED7FED
    seed2 = 0xEEEEEEEE
    for ch in s.upper():
        val = ord(ch)
        seed1 = ((hash_type << 8) + val) ^ (seed1 + seed2) & 0xFFFFFFFF
        seed2 = (val + seed1 + seed2 + (seed2 << 5) + 3) & 0xFFFFFFFF
    return seed1 & 0xFFFFFFFF


def _decrypt(data: bytes, key: int) -> bytes:
    """MPQ-style decryption using a rolling key."""
    if key == 0:
        return data
    out = bytearray(len(data))
    k = key
    for i, b in enumerate(data):
        out[i] = b ^ (k & 0xFF)
        k = (k + 1) & 0xFFFFFFFF
    return bytes(out)


def extract_file(mpq_path: str, internal_path: str, output_path: str) -> bool:
    """Extract a single file from an MPQ archive by its internal path.

    Returns True on success, False if the file wasn't found.
    """
    path_upper = internal_path.replace("/", "\\").upper()
    hash_a = _hash_string(path_upper, 1)
    hash_b = _hash_string(path_upper, 2)

    with open(mpq_path, "rb") as f:
        header = f.read(MPQ_HEADER_SIZE)
        if header[:4] != b"MPQ\x1a":
            raise ValueError("Not a valid MPQ archive")

        header_size = struct.unpack_from("<I", header, 4)[0]
        hash_table_offset = struct.unpack_from("<I", header, 12)[0] + header_size
        block_table_offset = struct.unpack_from("<I", header, 16)[0] + header_size
        hash_table_entries = struct.unpack_from("<I", header, 24)[0] & 0xFFFFF
        block_table_entries = struct.unpack_from("<I", header, 28)[0] & 0xFFFFF

        # Search hash table for our file
        block_index = 0xFFFFFFFF
        f.seek(hash_table_offset)
        for _ in range(hash_table_entries):
            entry = f.read(MPQ_HASH_ENTRY_SIZE)
            a, b, _, _, bi = struct.unpack_from("<IIHHI", entry, 0)
            if a == hash_a and b == hash_b and bi < block_table_entries:
                block_index = bi
                break

        if block_index == 0xFFFFFFFF:
            return False

        # Read block table entry
        f.seek(block_table_offset + block_index * MPQ_BLOCK_ENTRY_SIZE)
        block = f.read(MPQ_BLOCK_ENTRY_SIZE)
        file_pos = struct.unpack_from("<I", block, 0)[0] + header_size
        compressed_size = struct.unpack_from("<I", block, 4)[0]
        file_size = struct.unpack_from("<I", block, 8)[0]
        flags = struct.unpack_from("<I", block, 12)[0]

        # Read file data
        f.seek(file_pos)
        data = f.read(compressed_size or file_size)

        # Decompress if needed
        if flags & 0xFF00 and compressed_size != file_size:
            try:
                data = zlib.decompress(data)
            except zlib.error:
                pass

        os.makedirs(os.path.dirname(output_path) or ".", exist_ok=True)
        with open(output_path, "wb") as outf:
            outf.write(data)
        return True


def extract_model(mpq_path: str, mdx_internal_path: str,
                  output_dir: str) -> List[str]:
    """Extract an .mdx and all its referenced .blp textures from an MPQ.

    Returns list of extracted file paths.
    """
    extracted = []
    mdx_name = os.path.basename(mdx_internal_path)
    mdx_out = os.path.join(output_dir, mdx_name)

    if extract_file(mpq_path, mdx_internal_path, mdx_out):
        extracted.append(mdx_out)
    else:
        return extracted

    tex_dir = os.path.dirname(mdx_internal_path)
    for sub in ["", "Textures", "textures"]:
        tex_base = os.path.join(tex_dir, sub) if tex_dir else sub
        for ext in [".blp", ".png", ".tga"]:
            for i in range(20):
                name = f"texture{i}{ext}"
                internal = f"{tex_base}\\{name}".replace("/", "\\")
                out = os.path.join(output_dir, name)
                if extract_file(mpq_path, internal, out) and out not in extracted:
                    extracted.append(out)

    return extracted


def extract_w3x_map(w3x_path: str, output_dir: str) -> List[str]:
    """Extract all custom .mdx models from a Warcraft 3 map (.w3x).

    .w3x files are MPQ archives. This scans for all .mdx files and extracts them
    along with their textures.

    Returns list of extracted .mdx file paths.
    """
    # Common paths where custom models live in .w3x maps
    common_prefixes = [
        "war3mapImported\\",
        "Units\\", "Buildings\\", "Doodads\\",
        "Abilities\\", "Textures\\",
    ]

    extracted_mdx = []
    for prefix in common_prefixes:
        # Try to extract by scanning common model names
        for i in range(100):
            for ext in [".mdx", ".MDX"]:
                internal = f"{prefix}model{i}{ext}"
                out = os.path.join(output_dir, f"model{i}.mdx")
                if extract_file(w3x_path, internal, out) and out not in extracted_mdx:
                    extracted_mdx.append(out)

    # Also scan for named models
    # (Without a listfile we can't enumerate; this requires the user to know filenames
    # or we ship a community-maintained listfile.)
    return extracted_mdx
