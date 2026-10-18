---
type: vendor-doc
source_repo: "https://github.com/DaniilVanin/pg2-jass-sector-toolkit"
source_path: "tools/normalize_jass_rawcodes.py"
ref: HEAD
---

from __future__ import annotations

import argparse
from pathlib import Path


def is_escaped_quote(data: bytes, pos: int) -> bool:
    slashes = 0
    i = pos - 1
    while i >= 0 and data[i] == 0x5C:
        slashes += 1
        i -= 1
    return slashes % 2 == 1


def int_literal_from_rawcode(raw: bytes) -> str:
    value = int.from_bytes(raw, "big", signed=False)
    if value > 0x7FFFFFFF:
        value -= 0x100000000
    return str(value)


def normalize(data: bytes) -> tuple[bytes, int, int]:
    out = bytearray()
    i = 0
    converted_rawcodes = 0
    converted_hex = 0
    in_string = False
    in_comment = False
    n = len(data)

    while i < n:
        c = data[i]

        if in_comment:
            out.append(c)
            if c == 0x0A:
                in_comment = False
            i += 1
            continue

        if in_string:
            out.append(c)
            if c == 0x22 and not is_escaped_quote(data, i):
                in_string = False
            i += 1
            continue

        if c == 0x2F and i + 1 < n and data[i + 1] == 0x2F:
            out.extend(data[i : i + 2])
            in_comment = True
            i += 2
            continue

        if c == 0x22:
            out.append(c)
            in_string = True
            i += 1
            continue

        if c == 0x27:
            end = data.find(b"'", i + 1)
            if end == -1:
                out.append(c)
                i += 1
                continue
            raw = data[i + 1 : end]
            if len(raw) == 4:
                out.extend(int_literal_from_rawcode(raw).encode("ascii"))
                converted_rawcodes += 1
            else:
                out.extend(data[i : end + 1])
            i = end + 1
            continue

        if c == 0x24:
            j = i + 1
            while j < n and (
                0x30 <= data[j] <= 0x39
                or 0x41 <= data[j] <= 0x46
                or 0x61 <= data[j] <= 0x66
            ):
                j += 1
            if j > i + 1:
                value = int(data[i + 1 : j].decode("ascii"), 16)
                if value > 0x7FFFFFFF:
                    value -= 0x100000000
                out.extend(str(value).encode("ascii"))
                converted_hex += 1
                i = j
                continue

        out.append(c)
        i += 1

    return bytes(out), converted_rawcodes, converted_hex


def main() -> int:
    parser = argparse.ArgumentParser(description="Convert JASS rawcode/hex literals to integers.")
    parser.add_argument("src", type=Path)
    parser.add_argument("dst", type=Path)
    args = parser.parse_args()

    data = args.src.read_bytes()
    normalized, rawcodes, hexes = normalize(data)
    args.dst.write_bytes(normalized)
    print(f"converted_rawcodes={rawcodes} converted_hex={hexes} size={len(normalized)}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
