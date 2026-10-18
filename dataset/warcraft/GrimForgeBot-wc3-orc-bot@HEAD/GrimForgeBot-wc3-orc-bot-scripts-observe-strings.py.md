---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/observe-strings.py"
ref: HEAD
---

"""Dump every printable-ASCII / UTF-8 run of length >= MIN_LEN in the
War3StatsObserverSharedMemory section, with absolute byte offsets.

Used to locate player names, race strings, and any other text-bearing
fields when the war3structs layout doesn't match Reforged 2.0+'s actual
binary format.
"""

from __future__ import annotations

import mmap
import re
import sys

SECTION_NAME = "War3StatsObserverSharedMemory"
TOTAL_SCAN_BYTES = 250 * 1024 * 1024
CHUNK_BYTES = 4 * 1024 * 1024
MIN_LEN = 5  # minimum length of a "string" run

# Printable ASCII + common UTF-8 lead bytes; excludes NUL and control chars.
PRINTABLE = re.compile(rb"[\x20-\x7e]{%d,}" % MIN_LEN)


def main() -> int:
    align = mmap.ALLOCATIONGRANULARITY
    if CHUNK_BYTES % align != 0:
        print(f"CHUNK_BYTES ({CHUNK_BYTES}) must be multiple of {align}", file=sys.stderr)
        return 2

    print(f"Scanning '{SECTION_NAME}' for printable runs of length >= {MIN_LEN}")
    overlap = 128  # safety: bridge string runs that cross chunk boundaries
    tail = b""
    offset = 0
    total_hits = 0

    while offset < TOTAL_SCAN_BYTES:
        try:
            mm = mmap.mmap(-1, CHUNK_BYTES, SECTION_NAME, offset=offset, access=mmap.ACCESS_READ)
        except OSError as exc:
            print(f"  open at {offset} ({offset // (1024*1024)} MB) failed: {exc} — stopping.")
            break
        buf = mm.read()
        mm.close()
        if not buf:
            print(f"  empty buffer at {offset} — stopping.")
            break

        combined = tail + buf
        local_base = offset - len(tail)
        for m in PRINTABLE.finditer(combined):
            s = m.group(0)
            absolute = local_base + m.start()
            try:
                decoded = s.decode("utf-8")
            except UnicodeDecodeError:
                decoded = s.decode("latin-1", errors="replace")
            # Filter out obviously-noise binary-looking floats etc.
            if len(decoded) < MIN_LEN:
                continue
            print(f"  0x{absolute:08x} ({absolute:>11}) len={len(s):>4}  {decoded!r}")
            total_hits += 1

        tail = buf[-overlap:] if overlap > 0 else b""
        offset += CHUNK_BYTES

    print(f"\nTotal: {total_hits} runs found.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
