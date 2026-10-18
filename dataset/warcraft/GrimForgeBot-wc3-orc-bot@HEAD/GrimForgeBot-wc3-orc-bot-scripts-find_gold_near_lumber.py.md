---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/find_gold_near_lumber.py"
ref: HEAD
---

#!/usr/bin/env python3
"""find_gold_near_lumber.py — targeted gold finder.

We already know lumber address. Gold is in the same player struct nearby.
Scans ±SCAN_RADIUS bytes around lumber_addr for an i32 that changes by -75
when you buy exactly 1 Peon.

Usage:
    python3 scripts/find_gold_near_lumber.py
"""
import ctypes, ctypes.util, struct, subprocess, sys, time

TARGET       = "Warcraft III"
LUMBER_ADDR  = 0x000000030fe1562c  # found in P0.5b run 2 (consistent across 2 sessions)
LUMBER_ENC   = -109                # stored - encode = lumber
SCAN_RADIUS  = 131072              # 128 KB each side (used as fallback)
GOLD_DELTA   = -75

libc = ctypes.CDLL(ctypes.util.find_library("c"))
libc.task_for_pid.restype  = ctypes.c_int
libc.task_for_pid.argtypes = [ctypes.c_uint, ctypes.c_int, ctypes.POINTER(ctypes.c_uint)]
libc.mach_task_self.restype  = ctypes.c_uint
libc.mach_task_self.argtypes = []
libc.mach_vm_read_overwrite.restype  = ctypes.c_int
libc.mach_vm_read_overwrite.argtypes = [ctypes.c_uint, ctypes.c_uint64, ctypes.c_uint64,
    ctypes.c_uint64, ctypes.POINTER(ctypes.c_uint64)]


def get_task(pid: int) -> int:
    t = ctypes.c_uint(0)
    assert libc.task_for_pid(libc.mach_task_self(), pid, ctypes.byref(t)) == 0
    return t.value


def read_mem(task: int, addr: int, size: int) -> bytes | None:
    buf = (ctypes.c_char * size)()
    out = ctypes.c_uint64(0)
    kr = libc.mach_vm_read_overwrite(task, addr, size, ctypes.addressof(buf), ctypes.byref(out))
    return bytes(buf[:out.value]) if kr == 0 else None


def read_val(task: int, addr: int, encode: int) -> int | None:
    data = read_mem(task, addr, 4)
    if not data or len(data) < 4:
        return None
    return struct.unpack("<i", data)[0] - encode


pid_result = subprocess.run(["pgrep", "-x", TARGET], capture_output=True, text=True)
if pid_result.returncode != 0:
    print(f"error: '{TARGET}' not running", file=sys.stderr)
    sys.exit(1)
pid  = int(pid_result.stdout.strip().split("\n")[0])
task = get_task(pid)
print(f"Attached to {TARGET} PID={pid}")

# Verify lumber still readable
lumber_now = read_val(task, LUMBER_ADDR, LUMBER_ENC)
if lumber_now is None:
    print("ERROR: lumber address no longer readable. Session may have restarted.")
    sys.exit(1)
print(f"Lumber address check OK — current lumber = {lumber_now}")

print(f"\nScanning ±{SCAN_RADIUS} bytes around lumber address for gold offset.")
print("Protocol: Stop ALL peons. Buy exactly 1 Peon. Nothing else.")

PAGE = 4096

def snap_region(task: int, base: int, radius: int) -> dict[int, bytes]:
    """Read pages around base±radius. Returns {page_addr: bytes} for readable pages."""
    pages = {}
    start = (base - radius) & ~(PAGE - 1)
    end   = (base + radius + PAGE) & ~(PAGE - 1)
    for p in range(start, end, PAGE):
        data = read_mem(task, p, PAGE)
        if data and len(data) == PAGE:
            pages[p] = data
    return pages

gold_a = int(input("\nCurrent gold (from HUD): ").strip())
input("Press Enter to snapshot A ...")
pages_a = snap_region(task, LUMBER_ADDR, SCAN_RADIUS)
print(f"  snapshot A: {len(pages_a)} readable pages ({len(pages_a)*PAGE//1024} KB)")

input("Buy exactly 1 Peon (-75 gold), NO peon mining. Press Enter for snapshot B ...")
pages_b = snap_region(task, LUMBER_ADDR, SCAN_RADIUS)
print(f"  snapshot B: {len(pages_b)} readable pages")

gold_b         = int(input("New gold (from HUD after peon purchase): ").strip())
expected_delta = gold_b - gold_a
if expected_delta != GOLD_DELTA:
    print(f"WARNING: expected delta {GOLD_DELTA}, got {expected_delta}. Proceeding anyway.")

print(f"\nSearching for delta={expected_delta:+d} across {len(pages_a)} pages ...")
candidates = []
for page_addr, data_a in pages_a.items():
    data_b = pages_b.get(page_addr)
    if data_b is None or len(data_b) != PAGE:
        continue
    align = (4 - page_addr % 4) % 4
    i = align
    while i + 4 <= PAGE:
        va = struct.unpack_from("<i", data_a, i)[0]
        vb = struct.unpack_from("<i", data_b, i)[0]
        if vb - va == expected_delta:
            addr = page_addr + i
            encode = va - gold_a
            candidates.append((addr, encode))
        i += 4

print(f"  {len(candidates)} candidate(s):")
for addr, enc in candidates:
    print(f"  0x{addr:016x}  encode={enc}  offset_from_lumber={addr - LUMBER_ADDR:+d}")

if not candidates:
    print("\nNot found in ±128 KB around lumber.")
    sys.exit(1)

# Validate and watch best candidate
gold_addr, gold_enc = candidates[0]
g = read_val(task, gold_addr, gold_enc)
if g != gold_b:
    print(f"WARNING: validation mismatch (read {g}, expected {gold_b}). Trying all candidates ...")
    for addr, enc in candidates:
        g = read_val(task, addr, enc)
        if g == gold_b:
            gold_addr, gold_enc = addr, enc
            break
    else:
        print("ERROR: no candidate validated. Exiting.")
        sys.exit(1)

lumber_now = read_val(task, LUMBER_ADDR, LUMBER_ENC)
print(f"\nGOLD address:  0x{gold_addr:016x}  encode={gold_enc}")
print(f"Offset from lumber: {gold_addr - LUMBER_ADDR:+d} bytes")
print(f"\nWatching gold + lumber (Ctrl+C to stop)...")
last = None
while True:
    g  = read_val(task, gold_addr, gold_enc)
    lv = read_val(task, LUMBER_ADDR, LUMBER_ENC)
    state = (g, lv)
    if state != last:
        print(f"  gold={g}  lumber={lv}")
        last = state
    time.sleep(0.25)
