---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/inject_sidecar.py"
ref: HEAD
---

#!/usr/bin/env python3
"""inject_sidecar.py — inject sidecar.lua into an existing .w3x using StormLib.

Uses StormLib (the same MPQ library WC3 uses internally) via ctypes.
Takes a W3Champions map as base, appends sidecar.lua to war3map.lua.

Usage:
    python3 scripts/inject_sidecar.py
"""
import ctypes
import shutil
import tempfile
import os
from pathlib import Path

# ── StormLib setup ────────────────────────────────────────────────────────────

STORM = ctypes.CDLL("/opt/homebrew/Cellar/stormlib/9.31/lib/libstorm.dylib")

# SFileOpenArchive(szMpqName, dwPriority, dwFlags, phMpq)
STORM.SFileOpenArchive.argtypes = [ctypes.c_char_p, ctypes.c_uint32,
                                   ctypes.c_uint32, ctypes.POINTER(ctypes.c_void_p)]
STORM.SFileOpenArchive.restype  = ctypes.c_bool

# SFileCloseArchive(hMpq)
STORM.SFileCloseArchive.argtypes = [ctypes.c_void_p]
STORM.SFileCloseArchive.restype  = ctypes.c_bool

# SFileHasFile(hMpq, szFileName)
STORM.SFileHasFile.argtypes = [ctypes.c_void_p, ctypes.c_char_p]
STORM.SFileHasFile.restype  = ctypes.c_bool

# SFileRemoveFile(hMpq, szFileName, dwSearchScope)
STORM.SFileRemoveFile.argtypes = [ctypes.c_void_p, ctypes.c_char_p, ctypes.c_uint32]
STORM.SFileRemoveFile.restype  = ctypes.c_bool

# SFileAddFileEx(hMpq, szFileName, szArchivedName, dwFlags, dwCompression, dwCompressionNext)
STORM.SFileAddFileEx.argtypes = [ctypes.c_void_p, ctypes.c_char_p, ctypes.c_char_p,
                                  ctypes.c_uint32, ctypes.c_uint32, ctypes.c_uint32]
STORM.SFileAddFileEx.restype  = ctypes.c_bool

# SFileExtractFile(hMpq, szToExtract, szExtracted, dwSearchScope)
STORM.SFileExtractFile.argtypes = [ctypes.c_void_p, ctypes.c_char_p,
                                    ctypes.c_char_p, ctypes.c_uint32]
STORM.SFileExtractFile.restype  = ctypes.c_bool

# SFileCompactArchive(hMpq, szListFile, bReserved)
STORM.SFileCompactArchive.argtypes = [ctypes.c_void_p, ctypes.c_char_p, ctypes.c_bool]
STORM.SFileCompactArchive.restype  = ctypes.c_bool

# MPQ flags
MPQ_OPEN_READ_ONLY   = 0x00000100
MPQ_FILE_COMPRESS    = 0x00000200
MPQ_FILE_REPLACEEXISTING = 0x80000000
SFILE_OPEN_FROM_MPQ  = 0

# Compression: ZLIB (same as WC3 maps)
MPQ_COMPRESSION_ZLIB = 0x02


def open_mpq(path: str, readonly: bool = False) -> ctypes.c_void_p:
    handle = ctypes.c_void_p()
    flags = MPQ_OPEN_READ_ONLY if readonly else 0
    ok = STORM.SFileOpenArchive(path.encode(), 0, flags, ctypes.byref(handle))
    if not ok:
        err = ctypes.get_last_error() if hasattr(ctypes, 'get_last_error') else '?'
        raise RuntimeError(f"SFileOpenArchive failed on {path} (err={err})")
    return handle


def extract_file(handle: ctypes.c_void_p, archive_name: str, dest_path: str) -> bool:
    return STORM.SFileExtractFile(handle, archive_name.encode(),
                                   dest_path.encode(), SFILE_OPEN_FROM_MPQ)


def remove_file(handle: ctypes.c_void_p, archive_name: str) -> bool:
    if STORM.SFileHasFile(handle, archive_name.encode()):
        return STORM.SFileRemoveFile(handle, archive_name.encode(), SFILE_OPEN_FROM_MPQ)
    return True


def add_file(handle: ctypes.c_void_p, src_path: str, archive_name: str):
    ok = STORM.SFileAddFileEx(
        handle, src_path.encode(), archive_name.encode(),
        MPQ_FILE_REPLACEEXISTING | MPQ_FILE_COMPRESS,
        MPQ_COMPRESSION_ZLIB, MPQ_COMPRESSION_ZLIB,
    )
    if not ok:
        raise RuntimeError(f"SFileAddFileEx failed for {archive_name}")


def close_mpq(handle: ctypes.c_void_p):
    STORM.SFileCloseArchive(handle)


# ── main ─────────────────────────────────────────────────────────────────────

def main():
    base_map  = Path.home() / "Library/Application Support/Blizzard/Warcraft III/Maps/Download/Season8" \
                / "(2)LastRefuge_S2_v1.5.w3x"
    sidecar   = Path("maps/sidecar/sidecar.lua")
    out_dir   = Path.home() / "Library/Application Support/Blizzard/Warcraft III/Maps/BotMaps"
    out_map   = out_dir / "sidecar_bot.w3x"

    out_dir.mkdir(parents=True, exist_ok=True)

    print(f"Base map : {base_map.name}")
    print(f"Sidecar  : {sidecar}")
    print(f"Output   : {out_map}")

    # Work on a copy so we don't damage the W3Champions map
    shutil.copy2(base_map, out_map)

    with tempfile.TemporaryDirectory() as tmp:
        tmp = Path(tmp)
        lua_orig  = tmp / "war3map_orig.lua"
        lua_patch = tmp / "war3map.lua"

        print("\nOpening archive...")
        h = open_mpq(str(out_map))

        print("Extracting war3map.lua...")
        if not extract_file(h, "war3map.lua", str(lua_orig)):
            close_mpq(h)
            raise RuntimeError("Could not extract war3map.lua")

        # Append sidecar AFTER the map's function definitions.
        # Standard WC3 melee maps define main() which WC3 calls after init.
        # We hook main() so our timer starts only after full game initialization.
        orig_text = lua_orig.read_bytes()
        sidecar_block = b"\n\n-- wc3-orc-bot sidecar (injected at bottom)\n" + sidecar.read_bytes()
        patch = orig_text + sidecar_block
        lua_patch.write_bytes(patch)
        print(f"war3map.lua: {len(orig_text):,} -> {len(patch):,} bytes (injected at bottom)")

        # Do NOT remove (attributes) — StormLib regenerates it with correct CRC32s on SFileCloseArchive

        # Replace war3map.lua
        print("Inserting patched war3map.lua...")
        add_file(h, str(lua_patch), "war3map.lua")

        # Compact: removes dead space from old war3map.lua block + rebuilds (attributes) cleanly
        print("Compacting archive...")
        STORM.SFileCompactArchive(h, None, False)

        close_mpq(h)

    print(f"\nDone! {out_map.stat().st_size:,} bytes")
    print("Restart WC3 → BotMaps → sidecar_bot")
    print("Then: python3 src/wc3bot/observe/sidecar_reader.py")


if __name__ == "__main__":
    main()
