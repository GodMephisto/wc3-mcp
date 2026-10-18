---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/read_gold.py"
ref: HEAD
---

#!/usr/bin/env python3
"""read_gold.py — P0.5 production observer: scan-on-start gold reader.

Since WC3 Reforged allocates the player struct fresh each game session
(and the pointer in the binary's writable data section is per-session),
a static pointer chain is not reliable. Instead we do a fast delta scan
at game start to locate gold, then read it in a tight loop.

Protocol:
  1. Attach to WC3 via task_for_pid.
  2. Take snapshot A.
  3. Wait for user to cause a known gold delta (buy 1 Peon = -75).
  4. Take snapshot B.
  5. Find address(es) where B-A == -75 AND A value follows the +32260 encoding
     (i.e. A - 32260 is a plausible starting gold: 250-750 range).
  6. Watch that address in a loop.

Total scan time: ~0.6s per snapshot, ~1.5s total setup.

Usage:
    python3 scripts/read_gold.py          # guided scan + live watch
    python3 scripts/read_gold.py --once   # scan + print once, exit 0
"""
import ctypes, ctypes.util, struct, subprocess, sys, time, argparse

TARGET       = "Warcraft III"
GOLD_ENCODE  = 32260          # stored_value - GOLD_ENCODE = actual gold
GOLD_DELTA   = -75            # cost of 1 Peon
GOLD_MIN     = 50             # sanity: gold won't be below this at game start
GOLD_MAX     = 2000           # sanity: starting gold never this high
MAX_REGION   = 64 * 1024 * 1024

libc    = ctypes.CDLL(ctypes.util.find_library("c"))
libproc = ctypes.CDLL(ctypes.util.find_library("proc"))

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


def find_gold_addr(snap_a: list, snap_b: list,
                   gold_before: int | None = None) -> tuple[int, int] | None:
    """Returns (gold_addr, encode_const) or None.

    If gold_before is given, uses it to filter and derive encode_const.
    Otherwise accepts any address that changed by GOLD_DELTA in heap range.
    """
    b_map = {base: data for base, data in snap_b}
    candidates = []
    for base, data_a in snap_a:
        data_b = b_map.get(base)
        if data_b is None or len(data_b) != len(data_a):
            continue
        align = (4 - base % 4) % 4
        i = align
        while i + 4 <= len(data_a):
            va = struct.unpack_from("<i", data_a, i)[0]
            vb = struct.unpack_from("<i", data_b, i)[0]
            if vb - va == GOLD_DELTA:
                addr = base + i
                if gold_before is not None:
                    # exact match: use provided gold to derive encode constant
                    encode = va - gold_before
                    candidates.append((addr, encode))
                else:
                    # no gold hint: accept heap addresses, derive encode later
                    if addr > 0x100000000:
                        candidates.append((addr, None))
            i += 4
    if not candidates:
        return None
    # Filter: WC3 heap is typically in 0x200000000..0x700000000 range.
    # Exclude stack (0x7f00000000000+), binary data (< 0x200000000), etc.
    heap = [(a, e) for a, e in candidates if 0x300000000 <= a <= 0x70000000000]
    best = heap if heap else candidates
    return best[0] if best else None


def read_gold(task: int, gold_addr: int, encode_const: int = GOLD_ENCODE) -> int | None:
    data = read_mem(task, gold_addr, 4)
    if not data or len(data) < 4:
        return None
    return struct.unpack("<i", data)[0] - encode_const


def scan_for_value(task: int, target: int) -> list[int]:
    """Scan all RW memory for a specific 32-bit value. Returns list of addresses."""
    hits = []
    addr = ctypes.c_uint64(1)
    while True:
        size = ctypes.c_uint64(0)
        info = _Info(); cnt = ctypes.c_uint(9); obj = ctypes.c_uint(0)
        kr = libc.mach_vm_region(task, ctypes.byref(addr), ctypes.byref(size), 9,
                                  ctypes.byref(info), ctypes.byref(cnt), ctypes.byref(obj))
        if kr != 0:
            break
        if (info.protection & 3) == 3 and 4096 < size.value <= MAX_REGION:
            off = 0
            while off < size.value:
                chunk_sz = min(4 * 1024 * 1024, size.value - off)
                data = read_mem(task, addr.value + off, chunk_sz)
                if data:
                    align = (4 - (addr.value + off) % 4) % 4
                    i = align
                    while i + 4 <= len(data):
                        if struct.unpack_from("<i", data, i)[0] == target:
                            hits.append(addr.value + off + i)
                        i += 4
                off += chunk_sz
        addr.value += size.value
    return hits


def main():
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--once", action="store_true", help="find address, print gold once, exit")
    args = ap.parse_args()

    result = subprocess.run(["pgrep", "-x", TARGET], capture_output=True, text=True)
    if result.returncode != 0:
        print(f"error: '{TARGET}' not running", file=sys.stderr)
        return 1
    pid = int(result.stdout.strip().split("\n")[0])
    task = get_task(pid)
    print(f"Attached to {TARGET} PID={pid}")

    print("\nMake sure you are IN a game (HUD visible, gold shown).")
    print("IMPORTANT: Stop all Peon miners first (select them → Hold position)")
    print("           so gold stays stable during the scan.")
    input("Press Enter when miners are stopped and gold is not changing ...")
    raw = input("Enter your current gold (read from HUD exactly): ").strip()
    gold_now = int(raw)

    # WC3 stores gold as: raw_int32 = gold + GOLD_ENCODE
    stored = gold_now + GOLD_ENCODE
    print(f"\nScanning memory for stored value {stored} (gold {gold_now} + encode {GOLD_ENCODE}) ...")
    hits = scan_for_value(task, stored)
    print(f"  {len(hits)} candidate(s) found")

    if not hits:
        print("ERROR: no candidates found. Is WC3 in-game with gold visible?", file=sys.stderr)
        return 2

    if len(hits) > 5:
        print(f"Too many hits ({len(hits)}). Spend or gain a known amount of gold, then re-run.")
        return 2

    # Validate: read each candidate and check it gives expected gold
    gold_addr, encode_const = None, GOLD_ENCODE
    for addr in hits:
        g = read_gold(task, addr, GOLD_ENCODE)
        print(f"  0x{addr:016x} → gold = {g}")
        if g == gold_now:
            gold_addr = addr
            break

    if gold_addr is None:
        print("ERROR: no candidate matched current gold. Try again.", file=sys.stderr)
        return 2

    print(f"\nGold address:    0x{gold_addr:016x}")
    print(f"Encode constant: {GOLD_ENCODE}")
    print(f"Current gold:    {gold_now}")

    if args.once:
        return 0

    print("\nWatching gold (Ctrl+C to stop)...")
    last = None
    while True:
        g = read_gold(task, gold_addr, GOLD_ENCODE)
        if g != last:
            print(f"  gold = {g}")
            last = g
        time.sleep(0.25)


if __name__ == "__main__":
    sys.exit(main())
