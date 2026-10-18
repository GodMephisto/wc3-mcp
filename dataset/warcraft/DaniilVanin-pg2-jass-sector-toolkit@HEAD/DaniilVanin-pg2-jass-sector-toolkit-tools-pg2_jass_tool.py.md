---
type: vendor-doc
source_repo: "https://github.com/DaniilVanin/pg2-jass-sector-toolkit"
source_path: "tools/pg2_jass_tool.py"
ref: HEAD
---

"""
Extract and replace the main JASS sector block in PG2-protected Warcraft III maps.

PG2 can hide or corrupt the normal MPQ hash/block tables. The script block is
still stored as ordinary MPQ compressed sectors: an offset table followed by
0x02 + zlib streams. This tool finds the sector chain containing map JASS
markers, extracts it, and can replace it in-place.
"""

from __future__ import annotations

import argparse
import dataclasses
import pathlib
import struct
import sys
import zlib


SECTOR_SIZE = 4096
ZLIB_SECOND_BYTES = {0x01, 0x5E, 0x9C, 0xDA}


@dataclasses.dataclass(frozen=True)
class StreamInfo:
    pos: int
    compressed_len: int
    uncompressed_len: int

    @property
    def next_pos(self) -> int:
        # The byte before the zlib stream is the MPQ compression marker 0x02.
        return self.pos + self.compressed_len + 1


@dataclasses.dataclass
class SectorBlock:
    block_start: int
    offsets: list[int]
    streams: list[StreamInfo]
    score: int
    markers: list[str]

    @property
    def sector_count(self) -> int:
        return len(self.offsets) - 1

    @property
    def file_size(self) -> int:
        return sum(stream.uncompressed_len for stream in self.streams)

    @property
    def block_size(self) -> int:
        return self.offsets[-1]


def read_u32(data: bytes | bytearray, offset: int) -> int:
    return struct.unpack_from("<I", data, offset)[0]


def write_u32(data: bytearray, offset: int, value: int) -> None:
    struct.pack_into("<I", data, offset, value)


def iter_zlib_streams(data: bytes) -> list[StreamInfo]:
    streams: list[StreamInfo] = []
    pos = 1
    size = len(data)

    while pos < size - 2:
        pos = data.find(b"\x78", pos)
        if pos < 0 or pos >= size - 2:
            break

        if data[pos - 1] == 0x02 and data[pos + 1] in ZLIB_SECOND_BYTES:
            decompressor = zlib.decompressobj()
            window = data[pos : min(size, pos + 65536)]
            try:
                output = decompressor.decompress(window)
            except zlib.error:
                pos += 1
                continue

            if not decompressor.unused_data and pos + len(window) < size:
                pos += 1
                continue

            consumed = len(window) - len(decompressor.unused_data)
            if consumed > 0 and output:
                streams.append(StreamInfo(pos, consumed, len(output)))

        pos += 1

    return streams


def build_chains(streams: list[StreamInfo]) -> list[list[StreamInfo]]:
    by_pos = {stream.pos: stream for stream in streams}
    pointed_to = {stream.next_pos for stream in streams}
    chains: list[list[StreamInfo]] = []

    for stream in streams:
        if stream.pos in pointed_to:
            continue

        chain: list[StreamInfo] = []
        current = stream
        while current is not None:
            chain.append(current)
            current = by_pos.get(current.next_pos)

        if len(chain) > 1:
            chains.append(chain)

    return chains


def verify_offset_table(data: bytes, chain: list[StreamInfo]) -> list[int] | None:
    first_offset = 4 * (len(chain) + 1)
    block_start = chain[0].pos - 1 - first_offset
    if block_start < 0:
        return None

    offsets = [read_u32(data, block_start + i * 4) for i in range(len(chain) + 1)]
    if offsets[0] != first_offset:
        return None

    expected = first_offset
    for index, stream in enumerate(chain):
        if offsets[index] != expected:
            return None
        if block_start + offsets[index] != stream.pos - 1:
            return None
        expected += 1 + stream.compressed_len

    if offsets[-1] != expected:
        return None

    return offsets


def score_script(data: bytes, chain: list[StreamInfo]) -> tuple[int, list[str]]:
    sample = bytearray()

    sample_indexes = set(range(min(8, len(chain))))
    sample_indexes.update(range(max(0, len(chain) - 48), len(chain)))
    if len(chain) > 64:
        step = max(1, len(chain) // 16)
        sample_indexes.update(range(0, len(chain), step))

    for index in sorted(sample_indexes):
        stream = chain[index]
        start = stream.pos
        end = stream.pos + stream.compressed_len
        try:
            sample.extend(zlib.decompress(data[start:end]))
        except zlib.error:
            return 0, []

    markers = [
        b"function main",
        b"function config",
        b"InitCustomTriggers",
        b"gg_trg_",
        b"globals",
        b"endglobals",
        b"function InitTrig",
    ]
    found = [marker.decode("ascii") for marker in markers if marker in sample]
    score = sum(10 for marker in found if marker in {"function main", "function config"})
    score += sum(3 for marker in found if marker not in {"function main", "function config"})
    score += min(len(chain), 1000) // 20
    return score, found


def find_script_blocks(data: bytes) -> list[SectorBlock]:
    blocks: list[SectorBlock] = []
    for chain in build_chains(iter_zlib_streams(data)):
        offsets = verify_offset_table(data, chain)
        if offsets is None:
            continue

        score, markers = score_script(data, chain)
        if markers:
            block_start = chain[0].pos - 1 - offsets[0]
            blocks.append(SectorBlock(block_start, offsets, chain, score, markers))

    blocks.sort(key=lambda block: block.score, reverse=True)
    return blocks


def pick_main_script_block(data: bytes) -> SectorBlock:
    blocks = find_script_blocks(data)
    if not blocks:
        raise SystemExit("No PG2 JASS sector block found.")

    main_blocks = [
        block
        for block in blocks
        if "function main" in block.markers and "function config" in block.markers
    ]
    return main_blocks[0] if main_blocks else blocks[0]


def extract_block(data: bytes, block: SectorBlock) -> bytes:
    output = bytearray()
    for start_offset, end_offset in zip(block.offsets, block.offsets[1:]):
        start = block.block_start + start_offset
        end = block.block_start + end_offset
        if data[start] != 0x02:
            raise SystemExit(f"Unexpected sector compression byte {data[start]:#x} at {start}.")
        output.extend(zlib.decompress(data[start + 1 : end]))
    return bytes(output)


def best_compressed_sector(chunk: bytes) -> bytes:
    best = bytes([0x02]) + zlib.compress(chunk, 9)
    for level in range(1, 9):
        candidate = bytes([0x02]) + zlib.compress(chunk, level)
        if len(candidate) < len(best):
            best = candidate
    return best


def replace_block(data: bytearray, block: SectorBlock, replacement: bytes) -> tuple[int, int, int]:
    old_chunks = []
    for start_offset, end_offset in zip(block.offsets, block.offsets[1:]):
        start = block.block_start + start_offset
        end = block.block_start + end_offset
        old_chunks.append(zlib.decompress(data[start + 1 : end]))

    old_file_size = sum(len(chunk) for chunk in old_chunks)
    if len(replacement) > old_file_size:
        raise SystemExit(
            f"Replacement is {len(replacement)} bytes, but PG2 in-place import "
            f"can only fit {old_file_size} bytes. Shorten it or keep the size equal."
        )

    if len(replacement) < old_file_size:
        replacement += b" " * (old_file_size - len(replacement))

    new_chunks = [
        replacement[index : index + SECTOR_SIZE]
        for index in range(0, len(replacement), SECTOR_SIZE)
    ]
    if len(new_chunks) != len(old_chunks):
        raise SystemExit("Replacement changed the sector count; keep the extracted file size.")

    offsets = [len(block.offsets) * 4]
    sector_payloads: list[bytes] = []
    optimization_candidates: list[tuple[int, bytes]] = []
    changed_sectors = 0
    optimized_unchanged = 0

    for index, chunk in enumerate(new_chunks):
        start = block.block_start + block.offsets[index]
        end = block.block_start + block.offsets[index + 1]
        original_sector = bytes(data[start:end])

        if chunk == old_chunks[index]:
            sector = original_sector
            optimized_sector = best_compressed_sector(chunk)
            if len(optimized_sector) < len(original_sector):
                optimization_candidates.append((index, optimized_sector))
        else:
            changed_sectors += 1
            sector = best_compressed_sector(chunk)

        sector_payloads.append(sector)
        offsets.append(offsets[-1] + len(sector))

    if offsets[-1] > block.block_size:
        for index, optimized_sector in sorted(
            optimization_candidates,
            key=lambda item: len(sector_payloads[item[0]]) - len(item[1]),
            reverse=True,
        ):
            if offsets[-1] <= block.block_size:
                break

            saved = len(sector_payloads[index]) - len(optimized_sector)
            if saved <= 0:
                continue

            sector_payloads[index] = optimized_sector
            optimized_unchanged += 1
            for offset_index in range(index + 1, len(offsets)):
                offsets[offset_index] -= saved

    if offsets[-1] > block.block_size:
        raise SystemExit(
            f"Changed sectors compressed to {offsets[-1]} bytes, "
            f"but the old PG2 block has only {block.block_size} bytes. "
            "Make the edit smaller or remove whitespace/comments nearby."
        )

    for index, offset in enumerate(offsets):
        write_u32(data, block.block_start + index * 4, offset)

    payload = b"".join(sector_payloads)
    payload_start = block.block_start + offsets[0]
    payload_end = block.block_start + block.block_size
    data[payload_start:payload_end] = payload + b"\x00" * (block.block_size - offsets[-1])
    return changed_sectors, optimized_unchanged, block.block_size - offsets[-1]


def cmd_scan(args: argparse.Namespace) -> None:
    data = pathlib.Path(args.map).read_bytes()
    blocks = find_script_blocks(data)
    for index, block in enumerate(blocks[: args.limit]):
        print(
            f"#{index}: block={block.block_start} sectors={block.sector_count} "
            f"fileSize={block.file_size} blockSize={block.block_size} "
            f"score={block.score} markers={','.join(block.markers)}"
        )


def cmd_extract(args: argparse.Namespace) -> None:
    data = pathlib.Path(args.map).read_bytes()
    block = pick_main_script_block(data)
    output = extract_block(data, block)
    out_path = pathlib.Path(args.out)
    out_path.parent.mkdir(parents=True, exist_ok=True)
    out_path.write_bytes(output)
    print(
        f"Extracted {len(output)} bytes from block {block.block_start} "
        f"({block.sector_count} sectors) to {out_path}"
    )


def cmd_import(args: argparse.Namespace) -> None:
    map_path = pathlib.Path(args.map)
    script_path = pathlib.Path(args.script)
    out_path = pathlib.Path(args.out)

    data = bytearray(map_path.read_bytes())
    block = pick_main_script_block(data)
    changed, optimized, slack = replace_block(data, block, script_path.read_bytes())
    out_path.write_bytes(data)
    print(
        f"Wrote {out_path}; changedSectors={changed}; "
        f"optimizedUnchangedSectors={optimized}; "
        f"remainingBlockSlack={slack} bytes"
    )


def main(argv: list[str]) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    subparsers = parser.add_subparsers(dest="command", required=True)

    scan = subparsers.add_parser("scan", help="list JASS-like sector blocks")
    scan.add_argument("map")
    scan.add_argument("--limit", type=int, default=10)
    scan.set_defaults(func=cmd_scan)

    extract = subparsers.add_parser("extract", help="extract the main map JASS block")
    extract.add_argument("map")
    extract.add_argument("out")
    extract.set_defaults(func=cmd_extract)

    imp = subparsers.add_parser("import", help="replace the main map JASS block in-place")
    imp.add_argument("map")
    imp.add_argument("script")
    imp.add_argument("out")
    imp.set_defaults(func=cmd_import)

    args = parser.parse_args(argv)
    args.func(args)
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))
