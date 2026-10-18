---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/observe-hex.py"
ref: HEAD
---

"""Hex-dump targeted offsets in War3StatsObserverSharedMemory to
distinguish "player data is genuinely absent" from "we're parsing it
wrong". Three windows: section header, start of player[0], start of
player[1]."""

from __future__ import annotations

import mmap
import sys

SECTION_NAME = "War3StatsObserverSharedMemory"
PLAYER_STRIDE = 6416738
GAME_SIZE = 518  # current Rust layout
PLAYER_0_OFFSET = 4 + 4 + GAME_SIZE  # version + refresh_rate + game = 526

WINDOWS = [
    ("header (version + refresh_rate + game start)", 0, 64),
    ("player[0] start (name+race fields)", PLAYER_0_OFFSET, 96),
    ("player[1] start", PLAYER_0_OFFSET + PLAYER_STRIDE, 96),
    ("player[2] start", PLAYER_0_OFFSET + PLAYER_STRIDE * 2, 96),
    ("player[3] start", PLAYER_0_OFFSET + PLAYER_STRIDE * 3, 96),
]


def hexdump(data: bytes, base_offset: int) -> str:
    lines = []
    for i in range(0, len(data), 16):
        chunk = data[i : i + 16]
        hex_part = " ".join(f"{b:02x}" for b in chunk)
        ascii_part = "".join(chr(b) if 0x20 <= b < 0x7f else "." for b in chunk)
        lines.append(f"  {base_offset + i:>10}  {hex_part:<48}  {ascii_part}")
    return "\n".join(lines)


def dump_at(access_label: str, access: int) -> None:
    print(f"\n========== access = {access_label} ==========")
    for label, offset, size in WINDOWS:
        align = mmap.ALLOCATIONGRANULARITY
        seek = offset % align
        try:
            mm = mmap.mmap(
                -1,
                size + seek,
                SECTION_NAME,
                offset=offset - seek,
                access=access,
            )
        except OSError as exc:
            print(f"\n[{label}] open failed: {exc}")
            continue
        mm.seek(seek)
        data = mm.read(size)
        mm.close()
        nonzero = sum(1 for b in data if b != 0)
        print(f"\n[{label}] offset={offset} size={size}  nonzero={nonzero}/{size}")
        print(hexdump(data, offset))


def main() -> int:
    try:
        dump_at("READ", mmap.ACCESS_READ)
    except Exception as exc:
        print(f"READ pass failed: {exc!r}", file=sys.stderr)

    try:
        dump_at("WRITE", mmap.ACCESS_WRITE)
    except Exception as exc:
        print(f"WRITE pass failed: {exc!r}", file=sys.stderr)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
