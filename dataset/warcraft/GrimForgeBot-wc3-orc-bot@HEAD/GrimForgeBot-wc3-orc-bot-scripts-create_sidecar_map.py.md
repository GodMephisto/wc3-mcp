---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/create_sidecar_map.py"
ref: HEAD
---

#!/usr/bin/env python3
"""create_sidecar_map.py — build a minimal WC3 melee .w3x with embedded sidecar.

Creates a valid 64x64 Lordaeron Summer melee map from scratch using pure Python.
No World Editor, no external tools required.

Output: ~/Library/Application Support/Warcraft III/Maps/BotMaps/sidecar_bot.w3x

Usage:
    python3 scripts/create_sidecar_map.py
"""
import struct
import os
from pathlib import Path

# ── MPQ writer ──────────────────────────────────────────────────────────────
# Minimal MPQ v1: uncompressed, no encryption, single-sector files.

def _make_crypt_table():
    table = [0] * 0x500
    seed = 0x00100001
    for i in range(0x100):
        idx = i
        for _ in range(5):
            seed = (seed * 125 + 3) % 0x2AAAAB
            t1 = (seed & 0xFFFF) << 0x10
            seed = (seed * 125 + 3) % 0x2AAAAB
            t2 = seed & 0xFFFF
            table[idx] = t1 | t2
            idx += 0x100
    return table

_CRYPT = _make_crypt_table()

def _hash(s: str, t: int) -> int:
    s1, s2 = 0x7FED7FED, 0xEEEEEEEE
    for c in s.upper():
        ch = ord(c)
        s1 = (_CRYPT[(t << 8) + ch] ^ ((s1 + s2) & 0xFFFFFFFF)) & 0xFFFFFFFF
        s2 = (ch + s1 + s2 + (s2 << 5) + 3) & 0xFFFFFFFF
    return s1

def _encrypt_block(data: bytes, key: int) -> bytes:
    """MPQ block encryption (used for hash table and block table)."""
    seed = 0xEEEEEEEE
    key  = key & 0xFFFFFFFF
    result = bytearray(data)
    for i in range(0, (len(data) // 4) * 4, 4):
        seed = (seed + _CRYPT[0x400 + (key & 0xFF)]) & 0xFFFFFFFF
        orig = struct.unpack_from('<I', data, i)[0]
        enc  = (orig ^ ((key + seed) & 0xFFFFFFFF)) & 0xFFFFFFFF
        struct.pack_into('<I', result, i, enc)
        key  = ((~key << 0x15) + 0x11111111) | (key >> 0x0B)
        key  = key & 0xFFFFFFFF
        seed = (orig + seed + (seed << 5) + 3) & 0xFFFFFFFF
    return bytes(result)


def _next_pow2(n: int) -> int:
    p = 16
    while p < n * 2:
        p <<= 1
    return p

def build_hm3w_header(map_name: str, flags: int = 0, max_players: int = 12) -> bytes:
    """512-byte HM3W header that WC3 prepends to every .w3x file."""
    name_bytes = map_name.encode("utf-8") + b"\x00"
    payload = (
        b"HM3W"
        + struct.pack("<I", 0)           # unknown
        + name_bytes
        + struct.pack("<II", flags, max_players)
    )
    return payload.ljust(512, b"\x00")


def build_mpq(files: dict[str, bytes]) -> bytes:
    """files = {archive_name: data}. Returns MPQ bytes."""
    HEADER_SIZE = 32
    ht_size = _next_pow2(len(files))

    # Sort files for deterministic output
    file_list = list(files.items())
    if "(listfile)" not in files:
        listfile = "\r\n".join(files.keys()).encode("utf-8")
        file_list.append(("(listfile)", listfile))
        ht_size = _next_pow2(len(file_list))

    # Build block table and collect offsets
    block_entries = []  # (offset, comp_size, raw_size, flags)
    raw_data = b""

    for name, data in file_list:
        offset = HEADER_SIZE + len(raw_data)
        flags  = 0x81000000  # EXISTS | SINGLE_UNIT (uncompressed, unencrypted)
        block_entries.append((offset, len(data), len(data), flags))
        raw_data += data

    ht_offset = HEADER_SIZE + len(raw_data)
    bt_offset = ht_offset + ht_size * 16

    # Build hash table (initialised to 0xFFFFFFFF)
    ht = [0xFFFFFFFF] * (ht_size * 4)
    for idx, (name, _) in enumerate(file_list):
        h  = _hash(name, 0) % ht_size
        ha = _hash(name, 1)
        hb = _hash(name, 2)
        # Linear probe to find empty slot
        slot = h
        while ht[slot * 4 + 3] != 0xFFFFFFFF:
            slot = (slot + 1) % ht_size
        ht[slot * 4 + 0] = ha
        ht[slot * 4 + 1] = hb
        ht[slot * 4 + 2] = 0  # locale/platform = 0
        ht[slot * 4 + 3] = idx

    ht_bytes_plain = struct.pack(f"<{len(ht)}I", *ht)
    ht_bytes = _encrypt_block(ht_bytes_plain, _hash("(hash table)", 3))

    # Build block table
    bt_bytes_plain = b""
    for off, cs, rs, fl in block_entries:
        bt_bytes_plain += struct.pack("<IIII", off, cs, rs, fl)
    bt_bytes = _encrypt_block(bt_bytes_plain, _hash("(block table)", 3))

    archive_size = bt_offset + len(bt_bytes)
    header = struct.pack(
        "<4sIIHHIIII",
        b"MPQ\x1a",   # magic
        HEADER_SIZE,  # header size
        archive_size, # archive size
        0,            # format version (v1)
        3,            # sector size shift (512 << 3 = 4096)
        ht_offset,    # hash table offset
        bt_offset,    # block table offset
        ht_size,      # hash table entries
        len(file_list)# block table entries
    )

    return header + raw_data + ht_bytes + bt_bytes


# ── WC3 map file builders ────────────────────────────────────────────────────

MAP_W = 96   # total tiles wide (incl. 16-tile border each side)
MAP_H = 96   # total tiles tall
PLAY_W = 64  # playable tiles
PLAY_H = 64

def cstr(s: str) -> bytes:
    return s.encode("utf-8") + b"\x00"

def build_w3i() -> bytes:
    """war3map.w3i — format version 31 (patch 1.32, Reforged-compatible).

    Field order follows ChiefOfGxBxL/WC3MapSpecification (Info/0-33.md).
    v31 is the minimum version that includes all fields Reforged reads.
    """
    out = b""
    # ── header ──────────────────────────────────────────────────────────────
    out += struct.pack("<i", 31)              # formatVersion
    out += struct.pack("<i", 1)              # numberOfSaves
    out += struct.pack("<i", 6116)           # editorVersion
    out += struct.pack("<4I", 2, 0, 3, 0)   # gameVersion A.B.C.D (v27+)

    # ── strings ─────────────────────────────────────────────────────────────
    out += cstr("Sidecar Bot Map")
    out += cstr("wc3-orc-bot")
    out += cstr("Orc bot sidecar map")
    out += cstr("2")                         # recommendedPlayers

    # ── camera ──────────────────────────────────────────────────────────────
    half_w = (PLAY_W / 2) * 128.0           # 4096.0
    half_h = (PLAY_H / 2) * 128.0
    out += struct.pack("<8f",
        -half_w, -half_h,  half_w,  half_h,
        -half_w,  half_h,  half_w, -half_h,
    )
    out += struct.pack("<4i", 0, 0, 0, 0)   # cameraBoundsComplements
    out += struct.pack("<ii", PLAY_W, PLAY_H)

    # ── flags + ground ───────────────────────────────────────────────────────
    out += struct.pack("<i", 0x0004)         # mapFlags: melee only
    out += b"L"                              # mainGroundType: Lordaeron Summer

    # ── loading screen ───────────────────────────────────────────────────────
    out += struct.pack("<i", 0)              # campaignBackground (0=none)
    out += cstr("")                          # loadingScreenPath
    out += cstr("")                          # loadingScreenText
    out += cstr("")                          # loadingScreenTitle
    out += cstr("")                          # loadingScreenSubtitle

    # ── prologue ─────────────────────────────────────────────────────────────
    out += struct.pack("<i", 0)              # usedGameDataSet (0=Default)
    out += cstr("")                          # prologuePath
    out += cstr("")                          # prologueText
    out += cstr("")                          # prologueTitle
    out += cstr("")                          # prologueSubtitle

    # ── fog & environment ────────────────────────────────────────────────────
    out += struct.pack("<i", 0)              # fogType (0=linear)
    out += struct.pack("<fff", 0.0, 0.0, 0.0)  # fogStart, fogEnd, fogDensity
    out += struct.pack("<4B", 0, 0, 0, 0)   # fogColor RGBA
    out += struct.pack("<i", 0)              # globalWeatherId (0=none)
    out += cstr("")                          # customSoundEnvironment
    out += b"\x00"                           # customLightEnvironment
    out += struct.pack("<4B", 255, 255, 255, 255)  # waterTintColor

    # ── Reforged additions ───────────────────────────────────────────────────
    out += struct.pack("<i", 0)              # scriptingLanguage (v28: 0=JASS)
    out += struct.pack("<i", 3)              # supportedGraphicsModes (v29: 3=SD+HD)
    out += struct.pack("<i", 1)              # gameDataVersion (v30: 1=TFT)

    # ── players (v31: 11 fields each — adds enemyLow/HighPriorityFlags) ──────
    out += struct.pack("<i", 2)
    # Player 0: human, Orc
    out += struct.pack("<iiii", 0, 1, 2, 1)   # num, type=human, race=orc, fixedPos
    out += cstr("Player 1")
    out += struct.pack("<ff", -2048.0, -2048.0)
    out += struct.pack("<4i", 0, 0, 0, 0)     # allyLow, allyHigh, enemyLow, enemyHigh
    # Player 1: computer, any race
    out += struct.pack("<iiii", 1, 2, 0, 1)   # num, type=computer, race=random, fixedPos
    out += cstr("Insane AI")
    out += struct.pack("<ff", 2048.0, 2048.0)
    out += struct.pack("<4i", 0, 0, 0, 0)

    # ── forces ───────────────────────────────────────────────────────────────
    out += struct.pack("<i", 2)
    out += struct.pack("<ii", 0, 0b01)       # forceFlags, playerMask (player 0)
    out += cstr("Player 1")
    out += struct.pack("<ii", 0, 0b10)       # forceFlags, playerMask (player 1)
    out += cstr("Insane AI")

    # ── trailing counts (all empty) ──────────────────────────────────────────
    out += struct.pack("<4i", 0, 0, 0, 0)   # upgrades, tech, randUnit, randItem

    return out


def build_w3e() -> bytes:
    """war3map.w3e — flat Lordaeron Summer terrain."""
    W = MAP_W + 1  # vertices = tiles + 1
    H = MAP_H + 1

    # Standard Lordaeron Summer tilesets
    ground_tiles = [
        b"Ldrt", b"Lgrs", b"Lrck", b"Lrok", b"Lbdi", b"Ldro",
        b"Lgrd", b"Lgrr", b"Lgrv", b"Ldtr", b"Lvrg", b"Ldgr",
        b"Lblb", b"Ldbl", b"Lbs2", b"Lgr2",
    ]
    cliff_tiles = [
        b"CLcr", b"CLdi", b"CLve", b"CLch", b"CLcs", b"CLhd",
        b"CLta", b"CLtb", b"CLtc", b"CLtd", b"CLte", b"CLtf",
        b"CLtg", b"CLth", b"CLti", b"CLtj",
    ]

    out  = b"W3E!"                     # magic
    out += struct.pack("<i", 11)        # formatVersion
    out += b"L"                         # tileset = Lordaeron Summer
    out += struct.pack("<i", 0)         # customTileset = no
    out += struct.pack("<i", 16)        # numGroundTilesets
    for t in ground_tiles:
        out += t
    out += struct.pack("<i", 16)        # numCliffTilesets
    for t in cliff_tiles:
        out += t
    out += struct.pack("<ii", W, H)     # mapWidth, mapHeight (vertices)
    out += struct.pack("<2f", -(MAP_W / 2.0) * 128, -(MAP_H / 2.0) * 128)  # centerX, centerY

    # Terrain data: each vertex = groundHeight(2) + waterHeight(2) + flags(1) + groundTex(1) + layerHeight(1)
    BASE_HEIGHT = 0x2000
    vertex = struct.pack("<HHBBBxx",
        BASE_HEIGHT,  # groundHeight
        BASE_HEIGHT,  # waterHeight
        0x00,         # flags
        0x00,         # groundTexture index 0 (Ldrt) | variation 0
        0x01,         # cliffTexture | layerHeight=1 (ground level)
    )
    # Actually correct struct: HH BB B (7 bytes total, no padding in file)
    vertex = struct.pack("<HH", BASE_HEIGHT, BASE_HEIGHT) + bytes([0x00, 0x00, 0x01])
    out += vertex * (W * H)

    return out


def build_wpm() -> bytes:
    """war3map.wpm — pathing map (fully walkable/buildable/flyable)."""
    W = MAP_W * 4  # 4 cells per tile
    H = MAP_H * 4
    out  = b"MP3W"
    out += struct.pack("<iii", 0, W, H)  # version, width, height
    out += bytes(W * H)                  # all zeros = fully passable
    return out


def build_doo() -> bytes:
    """war3map.doo — terrain doodads (empty)."""
    out  = b"W3do"
    out += struct.pack("<iii", 8, 11, 0)  # version, subversion, numDoodads
    out += struct.pack("<i", 0)           # numSpecialDoodads
    return out


def build_units_doo() -> bytes:
    """war3mapUnits.doo — unit/item placement (empty, exactly 16 bytes)."""
    return struct.pack("<4sIII", b"W3do", 8, 11, 0)  # magic, version, subversion, count=0


def build_shd() -> bytes:
    """war3map.shd — shadow map (no shadows)."""
    return bytes(MAP_W * MAP_H)


def build_w3r() -> bytes:
    """war3map.w3r — regions (empty)."""
    return b"W3R " + struct.pack("<ii", 5, 0)


def build_w3c() -> bytes:
    """war3map.w3c — cameras (empty)."""
    return b"W3C " + struct.pack("<ii", 0, 0)


def build_w3s() -> bytes:
    """war3map.w3s — sounds (empty)."""
    return b"W3S " + struct.pack("<ii", 3, 0)


def build_w3j(sidecar_j: str) -> bytes:
    """war3map.j — JASS script: melee config + sidecar."""
    script = f"""\
// war3map.j — generated by create_sidecar_map.py
// Melee configuration for 2-player bot test map

function config takes nothing returns nothing
    local trigger t = null
    call SetMapName("Sidecar Bot Map")
    call SetGameDescriptionText("")
    call SetMapFlag(MAP_FLAGS_USE_HANDICAPS, false)
    call SetMapFlag(MAP_FLAGS_MASKED_PARTIAL_FOG, false)
    call DefineStartLocation(0, -2048.0, -2048.0)
    call DefineStartLocation(1, 2048.0, 2048.0)
    call SetPlayerStartLocation(Player(0), 0)
    call SetPlayerStartLocation(Player(1), 1)
    call SetPlayerColor(Player(0), PLAYER_COLOR_RED)
    call SetPlayerColor(Player(1), PLAYER_COLOR_BLUE)
    call SetPlayerRaceSelectable(Player(0), false)
    call SetPlayerRaceSelectable(Player(1), false)
endfunction

function main takes nothing returns nothing
    call InitBlizzard()
    call InitSidecar()
endfunction

{sidecar_j}
"""
    return script.encode("utf-8")


# ── entry point ──────────────────────────────────────────────────────────────

def main():
    sidecar_j_path = Path(__file__).parent.parent / "maps" / "sidecar" / "sidecar.j"
    if not sidecar_j_path.exists():
        print(f"ERROR: sidecar.j not found at {sidecar_j_path}")
        print("Run: python3 scripts/create_sidecar_j.py  (or check maps/sidecar/sidecar.j)")
        raise SystemExit(1)

    sidecar_j = sidecar_j_path.read_text(encoding="utf-8")

    print("Building map files...")
    files = {
        "war3map.w3i": build_w3i(),
        "war3map.w3e": build_w3e(),
        "war3map.wpm": build_wpm(),
        "war3map.doo": build_doo(),
        "war3map.shd": build_shd(),
        "war3map.w3r": build_w3r(),
        "war3map.w3c": build_w3c(),
        "war3map.w3s": build_w3s(),
        "war3map.j":        build_w3j(sidecar_j),
        "war3mapUnits.doo": build_units_doo(),
    }

    for name, data in files.items():
        print(f"  {name}: {len(data):,} bytes")

    print("\nPacking MPQ archive...")
    mpq_data = build_mpq(files)

    # macOS: ~/Library/Application Support/Blizzard/Warcraft III/Maps/BotMaps
    out_dir = Path.home() / "Library/Application Support/Blizzard/Warcraft III/Maps/BotMaps"
    out_dir.mkdir(parents=True, exist_ok=True)
    out_path = out_dir / "sidecar_bot.w3x"
    out_path.write_bytes(mpq_data)

    print(f"\nDone! Map written to:")
    print(f"  {out_path}")
    print(f"  Size: {len(mpq_data):,} bytes")
    print("\nIn WC3: Single Player → Custom Game → BotMaps → sidecar_bot")
    print("Then: python3 src/wc3bot/observe/sidecar_reader.py")


if __name__ == "__main__":
    main()
