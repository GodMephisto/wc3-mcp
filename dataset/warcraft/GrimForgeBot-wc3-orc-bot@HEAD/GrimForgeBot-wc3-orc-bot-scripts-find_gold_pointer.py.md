---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/find_gold_pointer.py"
ref: HEAD
---

#!/usr/bin/env python3
"""find_gold_pointer.py — P0.5 spike: interactive Mach-VM-based scanner that
narrows down the in-memory address of the Player-0 gold counter inside the
Rosetta-emulated WC3 Reforged process.

Read-only by design (INV-001):
    * uses mach_vm_read_overwrite — pure reads
    * never writes target memory

Protocol (Cheat-Engine-style narrowing):

    1. Attach to the running "Warcraft III" process via task_for_pid.
    2. User reads the current gold from the UI (top-right of game HUD) and
       enters it at the prompt.
    3. Script scans every rw- region of the target for int32-LE matches of
       that value. (We also try u32 and f32 in parallel because we don't yet
       know how WC3 stores it.)
    4. User performs a small known-delta action (buy a Peon = -75 gold, wait
       for 10 gold of Peon mining = +10 gold, etc.) and enters the new
       displayed value.
    5. Script re-reads each surviving candidate address and keeps those whose
       current value equals the new entered value.
    6. Repeat steps 4-5 until the candidate set is small (1-3 addresses).
    7. For each surviving candidate, print:
         * absolute address
         * filename of mapped region (from vmmap) — module + offset if static
         * protection + size of the containing region

Backend: Mach VM APIs via ctypes (task_for_pid + mach_vm_region +
mach_vm_read_overwrite). Frida is not used.
Requires Python binary to be signed with com.apple.security.cs.debugger
(done via: codesign -f -s - --entitlements frida-entitlements.plist <python>)

Usage:
    source .venv/bin/activate
    python3 scripts/find_gold_pointer.py
        # then follow prompts; enter 'q' to quit, 'p' to print candidates

Status: Mach backend (replaces Frida scaffold after Darwin-25/Frida compat
issue documented in NVR).
"""
from __future__ import annotations

import argparse
import ctypes
import ctypes.util
import struct
import subprocess
import sys
from dataclasses import dataclass, field

TARGET = "Warcraft III"
MAX_INITIAL_CANDIDATES = 200_000
DONE_THRESHOLD = 3
READ_CHUNK = 4 * 1024 * 1024  # 4 MB per mach_vm_read call

# ---------------------------------------------------------------------------
# Mach VM ctypes bindings
# ---------------------------------------------------------------------------

libc = ctypes.CDLL(ctypes.util.find_library("c"))

# task_for_pid
libc.task_for_pid.restype = ctypes.c_int
libc.task_for_pid.argtypes = [
    ctypes.c_uint,                  # target_tport (mach_task_self())
    ctypes.c_int,                   # pid
    ctypes.POINTER(ctypes.c_uint),  # t (out)
]

# mach_task_self
libc.mach_task_self.restype = ctypes.c_uint
libc.mach_task_self.argtypes = []

# mach_vm_region — flavor VM_REGION_BASIC_INFO_64 = 9
VM_REGION_BASIC_INFO_64 = 9
VM_REGION_BASIC_INFO_COUNT_64 = 9

class _VMRegionBasicInfo64(ctypes.Structure):
    _fields_ = [
        ("protection",       ctypes.c_int),
        ("max_protection",   ctypes.c_int),
        ("inheritance",      ctypes.c_uint),
        ("shared",           ctypes.c_int),
        ("reserved",         ctypes.c_int),
        ("offset",           ctypes.c_uint64),
        ("behavior",         ctypes.c_int),
        ("user_wired_count", ctypes.c_ushort),
    ]

libc.mach_vm_region.restype = ctypes.c_int
libc.mach_vm_region.argtypes = [
    ctypes.c_uint,                          # target_task
    ctypes.POINTER(ctypes.c_uint64),        # address (in/out)
    ctypes.POINTER(ctypes.c_uint64),        # size (out)
    ctypes.c_int,                           # flavor
    ctypes.POINTER(_VMRegionBasicInfo64),   # info (out)
    ctypes.POINTER(ctypes.c_uint),          # infoCnt (in/out)
    ctypes.POINTER(ctypes.c_uint),          # object_name (out)
]

# mach_vm_read_overwrite
libc.mach_vm_read_overwrite.restype = ctypes.c_int
libc.mach_vm_read_overwrite.argtypes = [
    ctypes.c_uint,    # target_task
    ctypes.c_uint64,  # address
    ctypes.c_uint64,  # size
    ctypes.c_uint64,  # data (ptr to local buffer)
    ctypes.POINTER(ctypes.c_uint64),  # outsize
]

VM_PROT_READ  = 0x1
VM_PROT_WRITE = 0x2
KERN_SUCCESS  = 0


# ---------------------------------------------------------------------------
# Low-level helpers
# ---------------------------------------------------------------------------

def get_task(pid: int) -> int:
    task = ctypes.c_uint(0)
    kr = libc.task_for_pid(libc.mach_task_self(), pid, ctypes.byref(task))
    if kr != KERN_SUCCESS:
        raise RuntimeError(f"task_for_pid failed: kern_return={kr}")
    return task.value


def iter_rw_regions(task: int):
    """Yield (base_addr, size) for every readable+writable region."""
    addr = ctypes.c_uint64(1)
    while True:
        size = ctypes.c_uint64(0)
        info = _VMRegionBasicInfo64()
        cnt  = ctypes.c_uint(VM_REGION_BASIC_INFO_COUNT_64)
        obj  = ctypes.c_uint(0)
        kr = libc.mach_vm_region(
            task,
            ctypes.byref(addr),
            ctypes.byref(size),
            VM_REGION_BASIC_INFO_64,
            ctypes.byref(info),
            ctypes.byref(cnt),
            ctypes.byref(obj),
        )
        if kr != KERN_SUCCESS:
            break
        prot = info.protection
        if (prot & VM_PROT_READ) and (prot & VM_PROT_WRITE):
            yield addr.value, size.value
        addr.value += size.value


def read_memory(task: int, address: int, size: int) -> bytes | None:
    buf = (ctypes.c_char * size)()
    out = ctypes.c_uint64(0)
    kr = libc.mach_vm_read_overwrite(
        task, address, size,
        ctypes.addressof(buf),
        ctypes.byref(out),
    )
    if kr != KERN_SUCCESS:
        return None
    return bytes(buf[:out.value])


def read_value(task: int, addr: int, kind: str) -> int | float | None:
    data = read_memory(task, addr, 4)
    if data is None or len(data) < 4:
        return None
    fmt = {"i32": "<i", "u32": "<I", "f32": "<f"}[kind]
    return struct.unpack(fmt, data)[0]


# ---------------------------------------------------------------------------
# Scan / filter
# ---------------------------------------------------------------------------

def _pack(value: int, kind: str) -> bytes:
    if kind == "i32":
        return struct.pack("<i", int(value))
    if kind == "u32":
        return struct.pack("<I", int(value) & 0xFFFFFFFF)
    if kind == "f32":
        return struct.pack("<f", float(value))
    raise ValueError(kind)


def scan_initial(task: int, value: int, kind: str) -> list[int]:
    pattern = _pack(value, kind)
    hits: list[int] = []
    for base, size in iter_rw_regions(task):
        offset = 0
        while offset < size:
            chunk_size = min(READ_CHUNK, size - offset)
            chunk = read_memory(task, base + offset, chunk_size)
            if chunk:
                pos = 0
                while True:
                    idx = chunk.find(pattern, pos)
                    if idx == -1:
                        break
                    hits.append(base + offset + idx)
                    pos = idx + 1
            offset += chunk_size
    return hits


def snapshot_candidates(task: int, addrs: list[int]) -> dict[int, bytes]:
    """Read all candidate addresses in one fast pass and return addr→bytes map.

    Groups addresses by page to minimise mach_vm_read_overwrite calls, reducing
    the time window during which the game can change values between reads.
    """
    PAGE = 4096
    # Sort by address so we can coalesce nearby reads
    sorted_addrs = sorted(addrs)
    snapshot: dict[int, bytes] = {}
    i = 0
    while i < len(sorted_addrs):
        page_base = sorted_addrs[i] & ~(PAGE - 1)
        # Collect all addresses on this page
        j = i
        while j < len(sorted_addrs) and sorted_addrs[j] < page_base + PAGE:
            j += 1
        data = read_memory(task, page_base, PAGE)
        if data:
            for addr in sorted_addrs[i:j]:
                off = addr - page_base
                if off + 4 <= len(data):
                    snapshot[addr] = data[off:off + 4]
        i = j
    return snapshot


def filter_candidates(task: int, addrs: list[int], value: int, kind: str,
                      tolerance: int = 0) -> list[int]:
    """Snapshot all addresses at once, then compare — avoids timing drift."""
    snap = snapshot_candidates(task, addrs)
    fmt = {"i32": "<i", "u32": "<I", "f32": "<f"}[kind]
    survivors = []
    for addr, data in snap.items():
        if len(data) < 4:
            continue
        actual = struct.unpack(fmt, data)[0]
        if tolerance == 0:
            if data == _pack(value, kind):
                survivors.append(addr)
        else:
            if abs(actual - value) <= tolerance:
                survivors.append(addr)
    return survivors


# ---------------------------------------------------------------------------
# Classification (module + offset via vmmap subprocess)
# ---------------------------------------------------------------------------

@dataclass
class AddrInfo:
    addr: int
    region_base: int = 0
    region_size: int = 0
    prot: str = "???"
    mapped_file: str = ""


def classify_addresses(pid: int, task: int, addrs: list[int]) -> list[AddrInfo]:
    results = []
    for addr in addrs:
        a = ctypes.c_uint64(addr)
        size = ctypes.c_uint64(0)
        info = _VMRegionBasicInfo64()
        cnt  = ctypes.c_uint(VM_REGION_BASIC_INFO_COUNT_64)
        obj  = ctypes.c_uint(0)
        kr = libc.mach_vm_region(
            task, ctypes.byref(a), ctypes.byref(size),
            VM_REGION_BASIC_INFO_64, ctypes.byref(info),
            ctypes.byref(cnt), ctypes.byref(obj),
        )
        prot_str = "???"
        if kr == KERN_SUCCESS:
            p = info.protection
            prot_str = (
                ("r" if p & VM_PROT_READ  else "-") +
                ("w" if p & VM_PROT_WRITE else "-") +
                "-"
            )
        results.append(AddrInfo(
            addr=addr,
            region_base=a.value,
            region_size=size.value,
            prot=prot_str,
        ))

    # Enrich with mapped filename via vmmap (best-effort)
    try:
        vmmap_out = subprocess.run(
            ["vmmap", "--wide", str(pid)],
            capture_output=True, text=True, timeout=10,
        ).stdout
        for ai in results:
            for line in vmmap_out.splitlines():
                parts = line.split()
                if len(parts) < 3:
                    continue
                try:
                    rng = parts[1] if "-" in parts[1] else None
                    if rng:
                        lo_s, hi_s = rng.split("-")
                        lo, hi = int(lo_s, 16), int(hi_s, 16)
                        if lo <= ai.addr < hi:
                            # last field is often the mapped file
                            ai.mapped_file = parts[-1] if "/" in parts[-1] else ""
                            break
                except (ValueError, IndexError):
                    continue
    except Exception:
        pass

    return results


# ---------------------------------------------------------------------------
# Interactive loop
# ---------------------------------------------------------------------------

def prompt_int(label: str) -> int | None:
    raw = input(f"{label}: ").strip()
    if raw.lower() in {"q", "quit", "exit"}:
        return None
    try:
        return int(raw)
    except ValueError:
        print(f"  not a valid integer: {raw!r}")
        return prompt_int(label)


def main(argv: list[str]) -> int:
    ap = argparse.ArgumentParser(description="WC3 gold pointer scanner (Mach VM backend)")
    ap.add_argument("--kinds", default="i32,u32,f32",
                    help="comma-separated value kinds (default: i32,u32,f32)")
    ap.add_argument("--tolerance", type=int, default=0,
                    help="allow ±N gold when filtering (use 10-20 to handle mining drift)")
    args = ap.parse_args(argv)
    kinds = [k.strip() for k in args.kinds.split(",") if k.strip()]

    result = subprocess.run(["pgrep", "-x", TARGET], capture_output=True, text=True)
    if result.returncode != 0:
        print(f"error: process '{TARGET}' not found — start WC3 first", file=sys.stderr)
        return 2
    pid = int(result.stdout.strip().split("\n")[0])
    print(f"Found {TARGET!r} PID={pid}")

    try:
        task = get_task(pid)
    except RuntimeError as e:
        print(f"error: {e}", file=sys.stderr)
        print("Hint: ensure Python is signed with com.apple.security.cs.debugger", file=sys.stderr)
        return 3
    print(f"task_for_pid OK (task={task})")

    # --- iteration 1: full-process scan
    initial = prompt_int("current gold (read from HUD top-right)")
    if initial is None:
        return 0

    print(f"scanning rw- regions for value {initial} across kinds={kinds} ...")
    by_kind: dict[str, list[int]] = {}
    for kind in kinds:
        hits = scan_initial(task, initial, kind)
        print(f"  {kind:>3}: {len(hits)} candidate(s)")
        if 0 < len(hits) <= MAX_INITIAL_CANDIDATES:
            by_kind[kind] = hits

    if not by_kind:
        print("no usable candidates — try a more distinctive starting value (>500)")
        return 1

    # --- iterations 2+: narrow
    while True:
        print()
        print("perform an action that changes gold (build a Peon = -75, wait")
        print("for mining = +10, etc.), then enter the new HUD value.")
        new_val = prompt_int("new gold")
        if new_val is None:
            break

        next_round: dict[str, list[int]] = {}
        for kind, addrs in by_kind.items():
            survivors = filter_candidates(task, addrs, new_val, kind, args.tolerance)
            print(f"  {kind:>3}: {len(addrs)} -> {len(survivors)}")
            if survivors:
                next_round[kind] = survivors
        if not next_round:
            print("all kinds dropped to zero — start over with a fresh initial value")
            break
        by_kind = next_round

        total = sum(len(a) for a in by_kind.values())
        if total <= DONE_THRESHOLD:
            print(f"\nnarrowed to {total} candidate(s) — classifying...")
            all_addrs = [a for addrs in by_kind.values() for a in addrs]
            infos = classify_addresses(pid, task, all_addrs)
            for kind, addrs in by_kind.items():
                addr_set = set(addrs)
                for ai in infos:
                    if ai.addr not in addr_set:
                        continue
                    print(f"  [{kind}] 0x{ai.addr:016x}")
                    print(f"        region 0x{ai.region_base:016x}  size=0x{ai.region_size:x}  prot={ai.prot}")
                    if ai.mapped_file:
                        offset = ai.addr - ai.region_base
                        print(f"        file={ai.mapped_file}  offset=0x{offset:x}")
                    else:
                        print("        file=<heap/anonymous>")
            break

    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
