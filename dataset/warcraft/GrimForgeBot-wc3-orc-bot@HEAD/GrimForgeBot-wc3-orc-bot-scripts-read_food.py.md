---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/read_food.py"
ref: HEAD
---

#!/usr/bin/env python3
"""read_food.py — P0.5b food_used finder.

Narrows food_used address by doing two consecutive peon-buy scans.
Only addresses that show +1 in BOTH scans survive.

Usage:
    python3 scripts/read_food.py
"""
import ctypes, ctypes.util, struct, subprocess, sys, time

TARGET     = "Warcraft III"
MAX_REGION = 64 * 1024 * 1024
FOOD_DELTA = 1

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


def find_delta(snap_a, snap_b, delta: int) -> set[int]:
    b_map = {base: data for base, data in snap_b}
    hits = set()
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
                addr = base + i
                if 0x200000000 <= addr <= 0x70000000000:
                    hits.add(addr)
            i += 4
    return hits


def read_val(task: int, addr: int) -> int | None:
    data = read_mem(task, addr, 4)
    if not data or len(data) < 4:
        return None
    return struct.unpack("<i", data)[0]


result = subprocess.run(["pgrep", "-x", TARGET], capture_output=True, text=True)
if result.returncode != 0:
    print(f"error: '{TARGET}' not running", file=sys.stderr)
    sys.exit(1)
pid  = int(result.stdout.strip().split("\n")[0])
task = get_task(pid)
print(f"Attached to {TARGET} PID={pid}")

print("""
Protocol: buy 1 Peon per round (no lumber harvesting).
Round 1 and Round 2 — only addresses with +1 in BOTH rounds survive.
""")

# Round 1
input("Round 1 — Press Enter for snapshot A ...")
snap_1a = snapshot(task)
print(f"  {len(snap_1a)} regions")

input("Buy 1 Peon. Press Enter for snapshot B ...")
snap_1b = snapshot(task)
print(f"  {len(snap_1b)} regions")

hits_1 = find_delta(snap_1a, snap_1b, FOOD_DELTA)
print(f"  Round 1: {len(hits_1)} candidates")

# Round 2
input("\nRound 2 — Press Enter for snapshot A ...")
snap_2a = snapshot(task)
print(f"  {len(snap_2a)} regions")

input("Buy 1 Peon. Press Enter for snapshot B ...")
snap_2b = snapshot(task)
print(f"  {len(snap_2b)} regions")

hits_2 = find_delta(snap_2a, snap_2b, FOOD_DELTA)
print(f"  Round 2: {len(hits_2)} candidates")

# Intersect
survivors = hits_1 & hits_2
print(f"\nSurvivors (both rounds): {len(survivors)}")

# Filter: food_used should be a small positive int (1..100 range)
plausible = []
for addr in sorted(survivors):
    v = read_val(task, addr)
    if v is not None and 0 < v < 200:
        plausible.append((addr, v))

print(f"Plausible food_used (value 1..199): {len(plausible)}")
for addr, v in plausible:
    print(f"  0x{addr:016x}  current={v}")

if len(plausible) == 0:
    print("\nNo plausible address found. Values out of 1..199 range.")
    print("All survivors:")
    for addr in sorted(survivors)[:10]:
        v = read_val(task, addr)
        print(f"  0x{addr:016x}  current={v}")
    sys.exit(1)

if len(plausible) > 1:
    # Round 3: buy 1 more peon, check which survivors increment
    candidates_3 = {addr for addr, _ in plausible}
    input(f"\nRound 3 — Press Enter for snapshot A ({len(candidates_3)} addresses to check) ...")
    snap_3a = snapshot(task)
    print(f"  {len(snap_3a)} regions")

    input("Buy 1 Peon. Press Enter for snapshot B ...")
    snap_3b = snapshot(task)
    print(f"  {len(snap_3b)} regions")

    hits_3 = find_delta(snap_3a, snap_3b, FOOD_DELTA)
    plausible = [(a, read_val(task, a)) for a in candidates_3 if a in hits_3]
    v_filter = [(a, v) for a, v in plausible if v is not None and 0 < v < 200]
    print(f"  After Round 3: {len(v_filter)} plausible")
    for addr, v in v_filter:
        print(f"  0x{addr:016x}  current={v}")
    plausible = v_filter

if len(plausible) == 1:
    addr, v = plausible[0]
    print(f"\nFOOD_USED: 0x{addr:016x}  current={v}")
    print("\nWatching food_used (Ctrl+C to stop)...")
    last = None
    while True:
        cur = read_val(task, addr)
        if cur != last:
            print(f"  food_used = {cur}")
            last = cur
        time.sleep(0.25)
else:
    print(f"\n{len(plausible)} addresses remain. Buy more peons or check manually which increments.")
