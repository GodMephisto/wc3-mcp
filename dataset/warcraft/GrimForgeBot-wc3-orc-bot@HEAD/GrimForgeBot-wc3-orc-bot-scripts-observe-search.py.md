---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/observe-search.py"
ref: HEAD
---

"""Brute-force search the War3StatsObserverSharedMemory section for the
player name string, to determine the true player-slot offset under
Reforged 2.0+ when war3structs's computed ObserverPlayer.sizeof() may
no longer match the real stride.

Usage:
    observe-search.py <needle>

Example:
    observe-search.py Seba
    observe-search.py Computer

Dumps every hit (byte offset, surrounding bytes, decoded preview).
"""

from __future__ import annotations

import mmap
import sys

SECTION_NAME = "War3StatsObserverSharedMemory"
TOTAL_SCAN_BYTES = 250 * 1024 * 1024  # 250 MB upper bound — section is ~180 MB
CHUNK_BYTES = 4 * 1024 * 1024  # 4 MB; multiple of 64 KB so it's mmap-aligned


def main(needle_str: str) -> int:
    needle = needle_str.encode("utf-8")
    if not needle:
        print("needle must not be empty", file=sys.stderr)
        return 2
    overlap = len(needle) - 1

    align = mmap.ALLOCATIONGRANULARITY
    if CHUNK_BYTES % align != 0:
        print(f"CHUNK_BYTES ({CHUNK_BYTES}) must be a multiple of ALLOCATIONGRANULARITY ({align})", file=sys.stderr)
        return 2

    print(f"Searching for {needle!r} in section '{SECTION_NAME}'")
    hits: list[int] = []
    tail = b""  # last `overlap` bytes from previous chunk, to catch boundary hits
    offset = 0

    while offset < TOTAL_SCAN_BYTES:
        try:
            mm = mmap.mmap(
                -1,
                CHUNK_BYTES,
                SECTION_NAME,
                offset=offset,
                access=mmap.ACCESS_READ,
            )
        except OSError as exc:
            print(f"  open at offset {offset} ({offset // (1024*1024)} MB) failed: {exc} — stopping scan.")
            break
        buf = mm.read()
        mm.close()
        if not buf:
            print(f"  empty buffer at offset {offset} — stopping scan.")
            break

        combined = tail + buf
        local_base = offset - len(tail)
        local = 0
        while True:
            i = combined.find(needle, local)
            if i < 0:
                break
            absolute = local_base + i
            context_start = max(0, i - 8)
            context_end = min(len(combined), i + len(needle) + 40)
            context = combined[context_start:context_end]
            preview = context.replace(b"\x00", b".").decode("utf-8", errors="replace").replace("\n", "?")
            hits.append(absolute)
            print(f"  hit @ 0x{absolute:08x} ({absolute:>11}): {preview!r}")
            local = i + 1

        tail = buf[-overlap:] if overlap > 0 else b""
        offset += CHUNK_BYTES

    print(f"\nFound {len(hits)} hits.")
    if len(hits) >= 2:
        for j in range(1, len(hits)):
            print(f"Distance hits[{j-1}]→hits[{j}]: {hits[j] - hits[j-1]} bytes")
    return 0


if __name__ == "__main__":
    if len(sys.argv) < 2:
        print(__doc__)
        raise SystemExit(2)
    raise SystemExit(main(sys.argv[1]))
