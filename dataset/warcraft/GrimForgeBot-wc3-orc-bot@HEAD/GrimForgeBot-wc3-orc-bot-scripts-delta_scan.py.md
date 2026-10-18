---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/delta_scan.py"
ref: HEAD
---

#!/usr/bin/env python3
"""delta_scan.py — fast delta scan: two raw-byte snapshots, find i32 addresses
that changed by exactly the expected delta.

Optimisations vs the slow version:
  - Stores raw bytes per region (not per-address dicts)
  - Skips regions larger than MAX_REGION_MB (huge mappings are never game state)
  - Alignment-correct i32 comparison in Python loop (still fast for small regions)

Usage:
    python3 scripts/delta_scan.py           # default delta = -75 (1 Peon)
    python3 scripts/delta_scan.py -d -200   # custom delta
"""
import ctypes, ctypes.util, struct, subprocess, sys, argparse, time

libc = ctypes.CDLL(ctypes.util.find_library("c"))
libc.task_for_pid.restype = ctypes.c_int
libc.task_for_pid.argtypes = [ctypes.c_uint, ctypes.c_int, ctypes.POINTER(ctypes.c_uint)]
libc.mach_task_self.restype = ctypes.c_uint

class _Info(ctypes.Structure):
    _fields_ = [("protection",ctypes.c_int),("max_protection",ctypes.c_int),
                ("inheritance",ctypes.c_uint),("shared",ctypes.c_int),
                ("reserved",ctypes.c_int),("offset",ctypes.c_uint64),
                ("behavior",ctypes.c_int),("user_wired_count",ctypes.c_ushort)]

libc.mach_vm_region.restype = ctypes.c_int
libc.mach_vm_region.argtypes = [ctypes.c_uint,ctypes.POINTER(ctypes.c_uint64),
    ctypes.POINTER(ctypes.c_uint64),ctypes.c_int,ctypes.POINTER(_Info),
    ctypes.POINTER(ctypes.c_uint),ctypes.POINTER(ctypes.c_uint)]
libc.mach_vm_read_overwrite.restype = ctypes.c_int
libc.mach_vm_read_overwrite.argtypes = [ctypes.c_uint,ctypes.c_uint64,ctypes.c_uint64,
    ctypes.c_uint64,ctypes.POINTER(ctypes.c_uint64)]

MAX_REGION_MB = 64  # skip huge regions (shared libs, asset caches)

def get_task(pid):
    t = ctypes.c_uint(0)
    assert libc.task_for_pid(libc.mach_task_self(), pid, ctypes.byref(t)) == 0
    return t.value

def read_mem(task, addr, size):
    buf = (ctypes.c_char * size)()
    out = ctypes.c_uint64(0)
    kr = libc.mach_vm_read_overwrite(task, addr, size, ctypes.addressof(buf), ctypes.byref(out))
    return bytes(buf[:out.value]) if kr == 0 else None

def snapshot(task) -> list[tuple[int, bytes]]:
    """Returns list of (region_base, raw_bytes) for small rw+rwx regions."""
    regions = []
    addr = ctypes.c_uint64(1)
    CHUNK = 4 * 1024 * 1024
    total_mb = 0
    while True:
        size = ctypes.c_uint64(0)
        info = _Info(); cnt = ctypes.c_uint(9); obj = ctypes.c_uint(0)
        kr = libc.mach_vm_region(task, ctypes.byref(addr), ctypes.byref(size), 9,
                                  ctypes.byref(info), ctypes.byref(cnt), ctypes.byref(obj))
        if kr != 0: break
        if (info.protection & 3) == 3 and size.value <= MAX_REGION_MB * 1024 * 1024:
            data = b""
            off = 0
            while off < size.value:
                chunk_sz = min(CHUNK, size.value - off)
                chunk = read_mem(task, addr.value + off, chunk_sz)
                data += chunk if chunk else b"\x00" * chunk_sz
                off += chunk_sz
            regions.append((addr.value, data))
            total_mb += size.value / 1024 / 1024
        addr.value += size.value
    print(f"  {len(regions)} regions, {total_mb:.0f} MB", file=sys.stderr)
    return regions

def find_delta(snap_a, snap_b, delta):
    hits = []
    b_map = {base: data for base, data in snap_b}
    for base, data_a in snap_a:
        data_b = b_map.get(base)
        if data_b is None or len(data_b) != len(data_a):
            continue
        # align base to 4
        align = (4 - base % 4) % 4
        i = align
        while i + 4 <= len(data_a):
            va = struct.unpack_from("<i", data_a, i)[0]
            vb = struct.unpack_from("<i", data_b, i)[0]
            if vb - va == delta:
                hits.append((base + i, va, vb))
            i += 4
    return hits

ap = argparse.ArgumentParser()
ap.add_argument("-d", "--delta", type=int, default=-75)
args = ap.parse_args()

TARGET = "Warcraft III"
pid = int(subprocess.run(["pgrep","-x",TARGET],capture_output=True,text=True).stdout.strip().split("\n")[0])
task = get_task(pid)
print(f"PID={pid}  delta={args.delta:+d}  max_region={MAX_REGION_MB}MB")
print()
print("Stop peon mining. No gold events yet.")
input("Enter → Snapshot A ...")
t0 = time.time()
snap_a = snapshot(task)
print(f"  done in {time.time()-t0:.1f}s")

print(f"\nBuy exactly 1 Peon (cost 75). No other actions.")
input("Enter when done → Snapshot B ...")
t0 = time.time()
snap_b = snapshot(task)
print(f"  done in {time.time()-t0:.1f}s")

print(f"\nSearching for delta={args.delta:+d} ...")
hits = find_delta(snap_a, snap_b, args.delta)
print(f"Found {len(hits)} address(es):")
for addr, va, vb in hits[:20]:
    print(f"  0x{addr:016x}  {va} -> {vb}")
