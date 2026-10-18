---
type: vendor-doc
source_repo: "https://github.com/DaniilVanin/pg2-jass-sector-toolkit"
source_path: "tools/compact_jass_comments.py"
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


def iter_logical_lines(data: bytes):
    start = 0
    i = 0
    n = len(data)
    in_string = False
    in_comment = False

    while i < n:
        c = data[i]

        if in_comment:
            if c == 0x0A:
                yield data[start : i + 1]
                start = i + 1
                in_comment = False
            i += 1
            continue

        if in_string:
            if c == 0x22 and not is_escaped_quote(data, i):
                in_string = False
            i += 1
            continue

        if c == 0x22:
            in_string = True
            i += 1
            continue

        if c == 0x27:
            end = data.find(b"'", i + 1)
            if end != -1:
                i = end + 1
                continue

        if c == 0x2F and i + 1 < n and data[i + 1] == 0x2F:
            in_comment = True
            i += 2
            continue

        if c == 0x0A:
            yield data[start : i + 1]
            start = i + 1

        i += 1

    if start < n:
        yield data[start:]


def main() -> int:
    parser = argparse.ArgumentParser(description="Remove full-line JASS comments and blank lines.")
    parser.add_argument("path", type=Path)
    args = parser.parse_args()

    data = args.path.read_bytes()
    kept: list[bytes] = []
    removed_bytes = 0
    removed_comments = 0
    removed_blank = 0

    for line in iter_logical_lines(data):
        stripped = line.lstrip()
        if stripped.startswith(b"//"):
            removed_bytes += len(line)
            removed_comments += 1
            continue
        if stripped in (b"", b"\n", b"\r\n"):
            removed_bytes += len(line)
            removed_blank += 1
            continue
        kept.append(line)

    args.path.write_bytes(b"".join(kept))
    print(
        f"removed_bytes={removed_bytes} "
        f"removed_comments={removed_comments} "
        f"removed_blank={removed_blank} "
        f"new_size={args.path.stat().st_size}"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
