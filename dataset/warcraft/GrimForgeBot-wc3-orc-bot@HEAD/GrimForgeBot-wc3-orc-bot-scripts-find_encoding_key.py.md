---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/find_encoding_key.py"
ref: HEAD
---

#!/usr/bin/env python3
"""find_encoding_key.py — find WC3's dynamic encoding key.

WC3 stores resources as: stored = actual_value + encoding_key
The encoding_key changes periodically (anti-memory-scan).

Strategy:
  1. Read gold_addr continuously until a jump occurs (no player action).
  2. At the moment of the jump, do a full snapshot.
  3. Do another snapshot immediately after.
  4. Find addresses where B-A == same_delta_as_gold_jump.
     The encoding key (or a value derived from it) should be among those.

Usage:
    python3 scripts/find_encoding_key.py --gold-addr 1459ee988 --gold-enc -1190
"""
import ctypes, ctypes.util, struct, subprocess, sys, time, argparse

TARGET     = "Warcraft III"
MAX_REGION = 64 * 1024 * 1024

libc = ctypes.CDLL(ctypes.util.find_library("c"))
libc.task_for_pid.restype  = ctypes.c_int
libc.task_for_pid.argtypes = [ctypes.c_uint, ctypes.c_int, ctypes.POINTER(ctypes.c_uint)]
libc.mach_task_self.restype  = ctypes.c_uint
libc.mach_task_self.argtypes = []

class _Info(ctypes.Structure):
    _fields_ = [("protection",ctypes.c_int),("max_protection",ctypes.c_int),
                ("inheritance",ctypes.c_uint),("shared",ctypes.c_int),
                ("reserved",ctypes.c_int),("offset",ctypes.c_uint64),
                ("behavior",ctypes.c_int),("user_wired_count",ctypes.c_ushort)]

libc.mach_vm_region.restype  = ctypes.c_int
libc.mach_vm_region.argtypes = [ctypes.c_uint,ctypes.POINTER(ctypes.c_uint64),
    ctypes.POINTER(ctypes.c_uint64),ctypes.c_int,ctypes.POINTER(_Info),
    ctypes.POINTER(ctypes.c_uint),ctypes.POINTER(ctypes.c_uint)]
libc.mach_vm_read_overwrite.restype  = ctypes.c_int
libc.mach_vm_read_overwrite.argtypes = [ctypes.c_uint,ctypes.c_uint64,ctypes.c_uint64,
    ctypes.c_uint64,ctypes.POINTER(ctypes.c_uint64)]


def get_task(pid: int) -> int:
    t = ctypes.c_uint(0)
    assert libc.task_for_pid(libc.mach_task_self(), pid, ctypes.byref(t)) == 0
    return t.value


def read_mem(task: int, addr: int, size: int) -> bytes | None:
    buf = (ctypes.c_char * size)()
    out = ctypes.c_uint64(0)
    kr = libc.mach_vm_read_overwrite(task, addr, size, ctypes.addressof(buf), ctypes.byref(out))
    return bytes(buf[:out.value]) if kr == 0 else None


def read_i32(task: int, addr: int) -> int | None:
    data = read_mem(task, addr, 4)
    if not data or len(data) < 4:
        return None
    return struct.unpack("<i", data)[0]


def snapshot(task: int) -> list[tuple[int, bytes]]:
    regions = []
    addr = ctypes.c_uint64(1)
    while True:
        size = ctypes.c_uint64(0)
        info = _Info(); cnt = ctypes.c_uint(9); obj = ctypes.c_uint(0)
        kr = libc.mach_vm_region(task, ctypes.byref(addr), ctypes.byref(size), 9,
                                  ctypes.byref(info), ctypes.byref(cnt), ctypes.byref(obj))
        if kr != 0: break
        if (info.protection & 3) == 3 and size.value <= MAX_REGION:
            data = b""
            off = 0
            while off < size.value:
                chunk_sz = min(4*1024*1024, size.value - off)
                chunk = read_mem(task, addr.value + off, chunk_sz)
                data += chunk if chunk else b"\x00" * chunk_sz
                off += chunk_sz
            regions.append((addr.value, data))
        addr.value += size.value
    return regions


def find_delta(snap_a, snap_b, delta: int) -> list[tuple[int, int, int]]:
    b_map = {base: data for base, data in snap_b}
    hits = []
    for base, data_a in snap_a:
        data_b = b_map.get(base)
        if data_b is None or len(data_b) != len(data_a):
            continue
        align = (4 - base % 4) % 4
        i = align
        while i + 4 <= len(data_a):
            va = struct.unpack_from("<i", data_a, i)[0]
            vb = struct.unpack_from("<i", data_b, i)[0]
            if vb - va == delta:
                hits.append((base + i, va, vb))
            i += 4
    return hits


ap = argparse.ArgumentParser(description=__doc__)
ap.add_argument("--gold-addr", type=lambda x: int(x, 16), required=True)
ap.add_argument("--gold-enc",  type=int, required=True)
args = ap.parse_args()

result = subprocess.run(["pgrep", "-x", TARGET], capture_output=True, text=True)
if result.returncode != 0:
    print(f"error: '{TARGET}' not running", file=sys.stderr); sys.exit(1)
pid  = int(result.stdout.strip().split("\n")[0])
task = get_task(pid)
print(f"Attached to {TARGET} PID={pid}")
print(f"Gold addr: 0x{args.gold_addr:016x}  encode={args.gold_enc}")
print("\nWaiting for encoding key jump (do NOT buy units or mine)...")
print("This may take up to 30s. Keep the game running.\n")

last_stored = read_i32(task, args.gold_addr)
print(f"Initial stored value at gold_addr: {last_stored}")

# Poll until stored value changes without player action
while True:
    time.sleep(0.05)
    cur = read_i32(task, args.gold_addr)
    if cur is None:
        continue
    if cur != last_stored:
        jump = cur - last_stored
        print(f"\nJump detected! stored: {last_stored} -> {cur}  delta={jump:+d}")
        print("Taking snapshot BEFORE (using last snapshot)...")
        # Take two rapid snapshots around the jump
        snap_a = snapshot(task)
        snap_b = snapshot(task)
        print(f"  {len(snap_a)} / {len(snap_b)} regions")
        print(f"Searching for addresses that changed by {jump:+d} ...")
        hits = find_delta(snap_a, snap_b, jump)
        heap = [(a, va, vb) for a, va, vb in hits if 0x300000000 <= a <= 0x70000000000]
        print(f"  {len(hits)} total, {len(heap)} in heap range (0x300000000+)")
        print(f"\nTop heap candidates (sorted by proximity to gold_addr):")
        heap.sort(key=lambda x: abs(x[0] - args.gold_addr))
        for addr, va, vb in heap[:20]:
            offset = addr - args.gold_addr
            print(f"  0x{addr:016x}  {va} -> {vb}  offset_from_gold={offset:+d}")
        if not heap:
            print("  (none in heap range — checking all)")
            hits.sort(key=lambda x: abs(x[0] - args.gold_addr))
            for addr, va, vb in hits[:20]:
                print(f"  0x{addr:016x}  {va} -> {vb}  offset={addr - args.gold_addr:+d}")
        break
    last_stored = cur
