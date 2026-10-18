---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/diag_scan.py"
ref: HEAD
---

#!/usr/bin/env python3
"""diag_scan.py — diagnostic: scan ALL WC3 memory for known sidecar strings.

Searches for:
  - b'INIT:sidecar'   → confirms sidecar Lua code is loaded
  - b'WC3BOT_STATE'   → confirms _G.WC3BOT_STATE global exists
  - b'T:'             → any occurrence of 'T:' (broad sweep)
  - b'G:' + b'L:'     → resource strings

Reports every hit with 48 bytes of context so we can see what surrounds it.
No region filtering — scans everything readable.
"""
import ctypes, ctypes.util, subprocess, sys, time

TARGET = "Warcraft III"

libc = ctypes.CDLL(ctypes.util.find_library("c"))
libc.task_for_pid.restype  = ctypes.c_int
libc.task_for_pid.argtypes = [ctypes.c_uint, ctypes.c_int, ctypes.POINTER(ctypes.c_uint)]
libc.mach_task_self.restype  = ctypes.c_uint
libc.mach_task_self.argtypes = []
libc.mach_vm_read_overwrite.restype  = ctypes.c_int
libc.mach_vm_read_overwrite.argtypes = [ctypes.c_uint, ctypes.c_uint64, ctypes.c_uint64,
    ctypes.c_uint64, ctypes.POINTER(ctypes.c_uint64)]

class _VMInfo(ctypes.Structure):
    _fields_ = [("protection", ctypes.c_int), ("max_protection", ctypes.c_int),
                ("inheritance", ctypes.c_uint), ("shared", ctypes.c_int),
                ("reserved", ctypes.c_int), ("offset", ctypes.c_uint64),
                ("behavior", ctypes.c_int), ("user_wired_count", ctypes.c_ushort)]

libc.mach_vm_region.restype  = ctypes.c_int
libc.mach_vm_region.argtypes = [ctypes.c_uint, ctypes.POINTER(ctypes.c_uint64),
    ctypes.POINTER(ctypes.c_uint64), ctypes.c_int, ctypes.POINTER(_VMInfo),
    ctypes.POINTER(ctypes.c_uint), ctypes.POINTER(ctypes.c_uint)]


def get_task(pid):
    t = ctypes.c_uint(0)
    assert libc.task_for_pid(libc.mach_task_self(), pid, ctypes.byref(t)) == 0
    return t.value

def read_mem(task, addr, size):
    if size <= 0: return b""
    buf = (ctypes.c_char * size)()
    out = ctypes.c_uint64(0)
    kr  = libc.mach_vm_read_overwrite(task, addr, size, ctypes.addressof(buf), ctypes.byref(out))
    return bytes(buf[:out.value]) if kr == 0 else None

def iter_regions(task):
    addr = ctypes.c_uint64(1)
    while True:
        size = ctypes.c_uint64(0)
        info = _VMInfo(); cnt = ctypes.c_uint(9); obj = ctypes.c_uint(0)
        kr = libc.mach_vm_region(task, ctypes.byref(addr), ctypes.byref(size), 9,
                                  ctypes.byref(info), ctypes.byref(cnt), ctypes.byref(obj))
        if kr != 0: break
        yield addr.value, size.value, info.protection
        addr.value += size.value


def scan_all(task, needles):
    """Scan every readable region for each needle. Returns dict needle→list of (addr, ctx)."""
    results = {n: [] for n in needles}
    CHUNK = 4 * 1024 * 1024
    regions_scanned = 0
    bytes_scanned   = 0

    for base, size, prot in iter_regions(task):
        if not (prot & 1):   # not readable
            continue
        if size > 512 * 1024 * 1024:  # skip truly huge (>512MB)
            continue
        regions_scanned += 1
        off = 0
        region_data = b""
        while off < size:
            chunk_sz = min(CHUNK, size - off)
            chunk = read_mem(task, base + off, chunk_sz)
            if chunk:
                region_data += chunk
            else:
                region_data += b"\x00" * chunk_sz
            off += chunk_sz
        bytes_scanned += len(region_data)

        for needle in needles:
            pos = 0
            while True:
                idx = region_data.find(needle, pos)
                if idx == -1:
                    break
                abs_addr = base + idx
                ctx_start = max(0, idx - 8)
                ctx_end   = min(len(region_data), idx + len(needle) + 40)
                ctx = region_data[ctx_start:ctx_end]
                results[needle].append((abs_addr, ctx))
                pos = idx + 1

    print(f"  Scanned {regions_scanned} regions, {bytes_scanned//1024//1024} MB total")
    return results


def main():
    result = subprocess.run(["pgrep", "-x", TARGET], capture_output=True, text=True)
    if result.returncode != 0:
        print(f"error: '{TARGET}' not running"); return 1
    pid  = int(result.stdout.strip().split("\n")[0])
    task = get_task(pid)
    print(f"Attached to {TARGET}  PID={pid}\n")

    needles = [
        b"INIT:sidecar",
        b"WC3BOT_STATE",
        b"WC3BOT",
        b"T:0.",          # early-game time like T:0.1
        b"G:500\x7c",     # common starting gold G:500|
        b"G:500|",
    ]

    print("Scanning ALL readable memory (no filters) ...")
    t0 = time.time()
    results = scan_all(task, needles)
    print(f"  Done in {time.time()-t0:.2f}s\n")

    any_found = False
    for needle, hits in results.items():
        if not hits:
            print(f"  {needle!r:30s}  → NOT FOUND")
        else:
            any_found = True
            print(f"  {needle!r:30s}  → {len(hits)} hit(s):")
            for addr, ctx in hits[:5]:
                safe = bytes(b if 32 <= b < 127 else ord('.') for b in ctx)
                print(f"      0x{addr:016x}  {safe!r}")

    print()
    if not any_found:
        print("DIAGNOSIS: Nothing found.")
        print("  → Sidecar is NOT running. Map did not load correctly,")
        print("    or Lua code has a syntax error preventing execution.")
        print("  → Check: did WC3 show any error when loading sidecar_bot?")
        print("  → Try: load the map, wait 10s in lobby, then re-run this script.")
    elif b"INIT:sidecar" in results and results[b"INIT:sidecar"]:
        print("DIAGNOSIS: Sidecar source code found (Lua loaded).")
        if results[b"WC3BOT_STATE"]:
            print("  _G.WC3BOT_STATE key found — global exists in Lua VM.")
            print("  → Look for the VALUE string near those addresses.")
        else:
            print("  WC3BOT_STATE NOT found — global not set yet?")
            print("  → Make sure you are IN a game (not just lobby).")


if __name__ == "__main__":
    sys.exit(main())
