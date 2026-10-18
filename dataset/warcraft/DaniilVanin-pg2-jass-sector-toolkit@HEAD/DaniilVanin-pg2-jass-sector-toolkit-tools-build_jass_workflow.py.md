---
type: vendor-doc
source_repo: "https://github.com/DaniilVanin/pg2-jass-sector-toolkit"
source_path: "tools/build_jass_workflow.py"
ref: HEAD
---

"""Reusable JASS build workflow for PG2-style Warcraft III maps.

This file is meant to be copied or edited for a specific map project. The
default workflow is intentionally generic:

1. prepare: extract and normalize the main JASS script;
2. edit: change the generated .edited.j file manually, or edit
   apply_custom_changes() below;
3. build: compact, import into a new map, and verify the result.
"""

from __future__ import annotations

import argparse
import difflib
from pathlib import Path
import sys

import compact_jass_comments
import normalize_jass_rawcodes
import pg2_jass_tool
import verify_jass_import


def apply_custom_changes(text: str) -> str:
    """Project-specific JASS patch hook.

    Keep this function as a no-op for fully manual editing. For a repeatable
    project build, add string replacements or parser-based edits here and run
    the build command again.
    """
    return text


def compact_bytes(data: bytes) -> bytes:
    kept: list[bytes] = []
    for line in compact_jass_comments.iter_logical_lines(data):
        stripped = line.lstrip()
        if stripped.startswith(b"//"):
            continue
        if stripped in (b"", b"\n", b"\r\n"):
            continue
        kept.append(line)
    return b"".join(kept)


def write_diff(before: bytes, after: bytes, out_path: Path) -> None:
    before_text = before.decode("utf-8").splitlines(keepends=True)
    after_text = after.decode("utf-8").splitlines(keepends=True)
    diff = difflib.unified_diff(
        before_text,
        after_text,
        fromfile="before.j",
        tofile="after.j",
    )
    out_path.parent.mkdir(parents=True, exist_ok=True)
    out_path.write_text("".join(diff), encoding="utf-8", newline="")


def project_name(map_path: Path, name: str | None) -> str:
    return name if name else map_path.stem


def paths_for(args: argparse.Namespace) -> dict[str, Path]:
    name = project_name(args.map, args.name)
    out_dir = args.out_dir
    build_dir = args.build_dir
    return {
        "raw": out_dir / f"{name}.war3map.j",
        "normalized": out_dir / f"{name}.normalized.j",
        "edited": out_dir / f"{name}.edited.j",
        "patched": out_dir / f"{name}.patched.j",
        "compact": out_dir / f"{name}.compact.j",
        "diff": out_dir / f"{name}.patch.diff",
        "check": out_dir / f"{name}.check.j",
        "map": args.output_map if args.output_map else build_dir / f"{name}.patched.w3x",
    }


def extract_main_jass(map_path: Path) -> tuple[bytes, pg2_jass_tool.SectorBlock]:
    data = map_path.read_bytes()
    block = pg2_jass_tool.pick_main_script_block(data)
    return pg2_jass_tool.extract_block(data, block), block


def cmd_prepare(args: argparse.Namespace) -> int:
    paths = paths_for(args)
    args.out_dir.mkdir(parents=True, exist_ok=True)
    args.build_dir.mkdir(parents=True, exist_ok=True)

    raw, block = extract_main_jass(args.map)
    normalized, rawcodes, hexes = normalize_jass_rawcodes.normalize(raw)

    paths["raw"].write_bytes(raw)
    paths["normalized"].write_bytes(normalized)

    if paths["edited"].exists() and not args.overwrite_edited:
        print(f"kept existing {paths['edited']} (use --overwrite-edited to replace it)")
    else:
        paths["edited"].write_bytes(normalized)
        print(f"wrote {paths['edited']}")

    print(
        f"block={block.block_start} sectors={block.sector_count} "
        f"fileSize={block.file_size} blockSize={block.block_size}"
    )
    print(f"normalized rawcodes={rawcodes} hexes={hexes}")
    print(f"wrote {paths['raw']} size={paths['raw'].stat().st_size}")
    print(f"wrote {paths['normalized']} size={paths['normalized'].stat().st_size}")
    print("next: edit the .edited.j file, then run the build command")
    return 0


def cmd_build(args: argparse.Namespace) -> int:
    paths = paths_for(args)
    args.out_dir.mkdir(parents=True, exist_ok=True)
    args.build_dir.mkdir(parents=True, exist_ok=True)

    if not paths["edited"].exists():
        raise SystemExit(f"Missing edited script: {paths['edited']}. Run prepare first.")

    edited = paths["edited"].read_bytes()
    patched_text = apply_custom_changes(edited.decode("utf-8"))
    patched = patched_text.encode("utf-8")
    compacted = compact_bytes(patched)

    paths["patched"].write_bytes(patched)
    paths["compact"].write_bytes(compacted)
    write_diff(edited, patched, paths["diff"])

    data = bytearray(args.map.read_bytes())
    block = pg2_jass_tool.pick_main_script_block(data)
    changed, optimized, slack = pg2_jass_tool.replace_block(data, block, compacted)
    paths["map"].parent.mkdir(parents=True, exist_ok=True)
    paths["map"].write_bytes(data)

    print(f"wrote {paths['patched']} size={paths['patched'].stat().st_size}")
    print(f"wrote {paths['compact']} size={paths['compact'].stat().st_size}")
    print(f"wrote {paths['diff']} size={paths['diff'].stat().st_size}")
    print(
        f"wrote {paths['map']}; changedSectors={changed}; "
        f"optimizedUnchangedSectors={optimized}; remainingBlockSlack={slack}"
    )

    verified = verify_jass_import.verify(paths["map"], paths["compact"], paths["check"])
    return 0 if verified else 2


def cmd_all(args: argparse.Namespace) -> int:
    prepare_status = cmd_prepare(args)
    if prepare_status != 0:
        return prepare_status
    return cmd_build(args)


def add_common_args(parser: argparse.ArgumentParser) -> None:
    parser.add_argument("--map", type=Path, required=True, help="input .w3x map")
    parser.add_argument("--name", default=None, help="project name used for output files")
    parser.add_argument("--out-dir", type=Path, default=Path("out"))
    parser.add_argument("--build-dir", type=Path, default=Path("build"))
    parser.add_argument("--output-map", type=Path, default=None)


def main(argv: list[str]) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    subparsers = parser.add_subparsers(dest="command", required=True)

    prepare = subparsers.add_parser("prepare", help="extract and normalize JASS for editing")
    add_common_args(prepare)
    prepare.add_argument(
        "--overwrite-edited",
        action="store_true",
        help="replace an existing .edited.j file with the normalized script",
    )
    prepare.set_defaults(func=cmd_prepare)

    build = subparsers.add_parser("build", help="compact, import, and verify edited JASS")
    add_common_args(build)
    build.set_defaults(func=cmd_build, overwrite_edited=False)

    all_cmd = subparsers.add_parser("all", help="prepare and build without a manual pause")
    add_common_args(all_cmd)
    all_cmd.add_argument(
        "--overwrite-edited",
        action="store_true",
        help="replace an existing .edited.j file with the normalized script",
    )
    all_cmd.set_defaults(func=cmd_all)

    args = parser.parse_args(argv)
    return args.func(args)


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))
