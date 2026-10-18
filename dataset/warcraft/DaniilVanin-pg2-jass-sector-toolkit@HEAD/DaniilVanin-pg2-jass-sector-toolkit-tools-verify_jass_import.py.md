---
type: vendor-doc
source_repo: "https://github.com/DaniilVanin/pg2-jass-sector-toolkit"
source_path: "tools/verify_jass_import.py"
ref: HEAD
---

"""Verify that a map contains the expected main JASS script."""

from __future__ import annotations

import argparse
from pathlib import Path
import sys

import pg2_jass_tool


def first_mismatch(left: bytes, right: bytes) -> int | None:
    for index, (a, b) in enumerate(zip(left, right)):
        if a != b:
            return index
    if len(left) != len(right):
        return min(len(left), len(right))
    return None


def format_byte(value: int | None) -> str:
    if value is None:
        return "<eof>"
    return f"0x{value:02x}"


def verify(map_path: Path, expected_script_path: Path, dump_path: Path | None = None) -> bool:
    expected = expected_script_path.read_bytes()
    data = map_path.read_bytes()
    block = pg2_jass_tool.pick_main_script_block(data)
    extracted = pg2_jass_tool.extract_block(data, block)

    if dump_path is not None:
        dump_path.parent.mkdir(parents=True, exist_ok=True)
        dump_path.write_bytes(extracted)

    print(
        f"block={block.block_start} sectors={block.sector_count} "
        f"fileSize={block.file_size} blockSize={block.block_size}"
    )
    print(f"expectedSize={len(expected)} extractedSize={len(extracted)}")

    if len(extracted) < len(expected):
        print(
            "ERROR: extracted script is shorter than expected "
            f"({len(extracted)} < {len(expected)})"
        )
        return False

    prefix = extracted[: len(expected)]
    if prefix != expected:
        offset = first_mismatch(prefix, expected)
        actual_byte = prefix[offset] if offset is not None and offset < len(prefix) else None
        expected_byte = expected[offset] if offset is not None and offset < len(expected) else None
        print(
            "ERROR: extracted script does not match expected script "
            f"at offset {offset}: actual={format_byte(actual_byte)} "
            f"expected={format_byte(expected_byte)}"
        )
        return False

    padding = extracted[len(expected) :]
    bad_padding = [index for index, byte in enumerate(padding) if byte != 0x20]
    if bad_padding:
        first_bad = bad_padding[0]
        print(
            "ERROR: extracted script has non-space padding "
            f"at extracted offset {len(expected) + first_bad}: "
            f"actual={format_byte(padding[first_bad])}"
        )
        print(f"paddingBytes={len(padding)} badPaddingBytes={len(bad_padding)}")
        return False

    print(f"OK: imported script matches expected bytes; paddingBytes={len(padding)}")
    if dump_path is not None:
        print(f"dumpedExtracted={dump_path}")
    return True


def main(argv: list[str]) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("map", type=Path, help="map to verify")
    parser.add_argument("expected_script", type=Path, help="script that should be in the map")
    parser.add_argument(
        "--dump-extracted",
        type=Path,
        default=None,
        help="optional path for the extracted script copy",
    )
    args = parser.parse_args(argv)

    return 0 if verify(args.map, args.expected_script, args.dump_extracted) else 1


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))
