---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/read_resources.py"
ref: HEAD
---

#!/usr/bin/env python3
"""read_resources.py — P0.5b multi-resource scanner.

Finds Gold, Lumber, Food in one delta-scan session.
Extends the scan-on-start approach from read_gold.py.

Protocol:
  1. Snapshot A (game running, peons idle).
  2. User causes known deltas simultaneously:
       - Buy 1 Peon  → gold  -75
       - 1 Peon trip → lumber +10  (assign 1 peon to trees, wait one trip)
     Food used changes by +1 when buying the peon (handled automatically).
  3. Snapshot B.
  4. User enters new gold + new lumber values.
  5. Script finds addresses for each resource.

Usage:
    python3 scripts/read_resources.py
"""
import ctypes, ctypes.util, struct, subprocess, sys, time

TARGET      = "Warcraft III"
MAX_REGION  = 64 * 1024 * 1024

libc    = ctypes.CDLL(ctypes.util.find_library("c"))
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


def find_candidates(snap_a, snap_b, delta: int, val_before: int) -> list[tuple[int, int]]:
    """Return (addr, encode_const) for all aligned i32 addresses where B-A == delta
    and stored_before - encode_const == val_before."""
    b_map = {base: data for base, data in snap_b}
    results = []
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
                    encode = va - val_before
                    results.append((addr, encode))
            i += 4
    return results


def validate(task: int, addr: int, encode: int, expected: int) -> bool:
    data = read_mem(task, addr, 4)
    if not data or len(data) < 4:
        return False
    return struct.unpack("<i", data)[0] - encode == expected


def read_val(task: int, addr: int, encode: int) -> int | None:
    data = read_mem(task, addr, 4)
    if not data or len(data) < 4:
        return None
    return struct.unpack("<i", data)[0] - encode


# ── main ────────────────────────────────────────────────────────────────────

pid = int(subprocess.run(["pgrep","-x",TARGET],capture_output=True,text=True).stdout.strip().split("\n")[0])
task = get_task(pid)
print(f"Attached to {TARGET} PID={pid}")

print("""
Protocol:
  A) Note current gold and lumber from HUD.
  B) Press Enter → snapshot A.
  C) Do BOTH actions:
       1. Buy 1 Peon  (-75 gold, +1 food used)
       2. Assign 1 peon to lumber, wait for exactly 1 trip (+10 lumber)
  D) Press Enter → snapshot B.
  E) Enter new gold and lumber values.
""")

gold_a  = int(input("Current gold  (snapshot A): ").strip())
lumber_a = int(input("Current lumber (snapshot A): ").strip())

input("\nPress Enter to take snapshot A ...")
t0 = time.time()
snap_a = snapshot(task)
print(f"  {len(snap_a)} regions in {time.time()-t0:.1f}s")

print("\nNow: buy 1 Peon AND wait for 1 lumber trip (+10).")
input("Press Enter AFTER both actions are done → snapshot B ...")
t0 = time.time()
snap_b = snapshot(task)
print(f"  {len(snap_b)} regions in {time.time()-t0:.1f}s")

gold_b   = int(input("\nNew gold   (from HUD): ").strip())
lumber_b = int(input("New lumber (from HUD): ").strip())

gold_delta   = gold_b   - gold_a
lumber_delta = lumber_b - lumber_a
food_delta   = 1  # buying 1 peon always costs 1 food slot

print(f"\nDeltas — gold:{gold_delta:+d}  lumber:{lumber_delta:+d}  food_used:+1")

print("\nScanning for gold ...")
gold_candidates = find_candidates(snap_a, snap_b, gold_delta, gold_a)
gold_valid = [(a, e) for a, e in gold_candidates if validate(task, a, e, gold_b)]
print(f"  {len(gold_candidates)} candidates → {len(gold_valid)} validated")

print("Scanning for lumber ...")
lumber_candidates = find_candidates(snap_a, snap_b, lumber_delta, lumber_a)
lumber_valid = [(a, e) for a, e in lumber_candidates if validate(task, a, e, lumber_b)]
print(f"  {len(lumber_candidates)} candidates → {len(lumber_valid)} validated")

print("Scanning for food_used (+1) ...")
# food_before unknown — just find delta +1 candidates and validate plausible range
food_raw = []
b_map = {base: data for base, data in snap_b}
for base, data_a in snap_a:
    data_b = b_map.get(base)
    if data_b is None or len(data_b) != len(data_a): continue
    align = (4 - base % 4) % 4
    i = align
    while i + 4 <= len(data_a):
        va = struct.unpack_from("<i", data_a, i)[0]
        vb = struct.unpack_from("<i", data_b, i)[0]
        if vb - va == food_delta:
            addr = base + i
            if 0x200000000 <= addr <= 0x70000000000:
                food_raw.append((addr, va, vb))
        i += 4
print(f"  {len(food_raw)} candidates (need further narrowing)")

print("\n=== RESULTS ===")
if gold_valid:
    ga, ge = gold_valid[0]
    g = read_val(task, ga, ge)
    print(f"GOLD:   0x{ga:016x}  encode={ge}  current={g}")
else:
    print("GOLD:   NOT FOUND")

if lumber_valid:
    la, le = lumber_valid[0]
    lv = read_val(task, la, le)
    print(f"LUMBER: 0x{la:016x}  encode={le}  current={lv}")
else:
    print("LUMBER: NOT FOUND")

print(f"FOOD_USED candidates: {len(food_raw)} (need restart + second delta to narrow)")
for addr, va, vb in food_raw[:5]:
    print(f"  0x{addr:016x}  {va} -> {vb}")

# live watch if gold found
if gold_valid and lumber_valid:
    ga, ge = gold_valid[0]
    la, le = lumber_valid[0]
    print("\nWatching gold + lumber (Ctrl+C to stop)...")
    last = None
    while True:
        g  = read_val(task, ga, ge)
        lv = read_val(task, la, le)
        state = (g, lv)
        if state != last:
            print(f"  gold={g}  lumber={lv}")
            last = state
        time.sleep(0.25)
