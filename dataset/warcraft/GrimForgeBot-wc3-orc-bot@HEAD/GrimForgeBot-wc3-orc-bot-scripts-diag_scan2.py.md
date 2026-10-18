---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/diag_scan2.py"
ref: HEAD
---

#!/usr/bin/env python3
"""diag_scan2.py — follow WC3BOT_STATE key to find the value string.

The key 'WC3BOT_STATE' was found in the Lua VM at 0x15f4e7380.
This script:
  1. Searches for |G: |L: |FU: |FC: (always in sidecar string, any time/gold)
  2. Scans ±2KB around the WC3BOT_STATE key address for pointer-sized values
     that point into readable memory, then reads the target to find the string.
  3. Reports ALL findings.
"""
import ctypes, ctypes.util, struct, subprocess, sys, time

TARGET = "Warcraft III"
# Address where WC3BOT_STATE key was found in previous diag_scan run
WC3BOT_KEY_ADDR = 0x15f4e7380

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

def is_readable(task, addr):
    for base, size, prot in iter_regions(task):
        if base <= addr < base + size:
            return bool(prot & 1)
    return False

def read_region_chunk(task, base, size):
    CHUNK = 4 * 1024 * 1024
    data = b""
    off = 0
    while off < size:
        chunk = read_mem(task, base + off, min(CHUNK, size - off))
        data += chunk if chunk else b"\x00" * min(CHUNK, size - off)
        off += CHUNK
    return data


def main():
    result = subprocess.run(["pgrep", "-x", TARGET], capture_output=True, text=True)
    if result.returncode != 0:
        print(f"error: '{TARGET}' not running"); return 1
    pid  = int(result.stdout.strip().split("\n")[0])
    task = get_task(pid)
    print(f"Attached to {TARGET}  PID={pid}\n")

    # ── 1. Search for sidecar-specific substrings (time/gold independent) ────
    print("=== Step 1: Broad pattern search (|G: |FU: |FC:) ===")
    needles = [b"|G:", b"|FU:", b"|FC:", b"FU:", b"FC:", b"T:1", b"T:2", b"T:3", b"T:4", b"T:5"]
    found_any = False
    t0 = time.time()
    for base, size, prot in iter_regions(task):
        if not (prot & 1): continue
        if size > 256 * 1024 * 1024: continue
        data = read_region_chunk(task, base, size)
        for needle in needles:
            pos = 0
            while True:
                idx = data.find(needle, pos)
                if idx == -1: break
                abs_addr = base + idx
                ctx = data[max(0,idx-4):idx+60]
                safe = bytes(b if 32 <= b < 127 else ord('.') for b in ctx)
                # Only print if looks like sidecar (has digits after needle)
                after = data[idx+len(needle):idx+len(needle)+10]
                if any(48 <= b <= 57 for b in after):  # has a digit nearby
                    print(f"  {needle!r} @ 0x{abs_addr:016x}  {safe!r}")
                    found_any = True
                pos = idx + 1
    print(f"  Scan done in {time.time()-t0:.1f}s")
    if not found_any:
        print("  NONE of the sidecar substrings found in memory!")
        print("  → tick() is NOT running or producing output.")
        print("  → Are you IN a game (not lobby)?")
        print("  → Did WC3 show any error on map load?\n")
    else:
        print()

    # ── 2. Follow pointers near WC3BOT_STATE key address ────────────────────
    print(f"=== Step 2: Scan ±4KB around WC3BOT_STATE key @ 0x{WC3BOT_KEY_ADDR:x} ===")
    scan_start = WC3BOT_KEY_ADDR - 4096
    scan_size  = 8192
    near = read_mem(task, scan_start, scan_size)
    if not near:
        print("  Could not read memory near key address.")
    else:
        # Look for 8-byte aligned pointers pointing to readable memory with printable content
        ptr_hits = 0
        for i in range(0, len(near) - 7, 8):
            v = struct.unpack_from("<Q", near, i)[0]
            if v < 0x100000000 or v > 0x800000000000:
                continue
            # Try to read 80 bytes at that address
            target = read_mem(task, v, 80)
            if not target:
                continue
            # Check if it looks like a string starting with T: or contains |G: etc.
            printable = sum(32 <= b < 127 for b in target[:40])
            if printable < 20:
                continue
            safe = bytes(b if 32 <= b < 127 else ord('.') for b in target[:60])
            abs_ptr_addr = scan_start + i
            # Filter: must contain at least one of our markers
            if any(m in target for m in [b"T:", b"|G:", b"|L:", b"FU:", b"FC:"]):
                print(f"  PTR @ 0x{abs_ptr_addr:x} → 0x{v:x}  {safe!r}  *** SIDECAR? ***")
                ptr_hits += 1
            elif b"WC3BOT" in target or b"sidecar" in target.lower():
                print(f"  PTR @ 0x{abs_ptr_addr:x} → 0x{v:x}  {safe!r}  (related)")
                ptr_hits += 1
        if ptr_hits == 0:
            print("  No sidecar-related pointers found near key address.")

    # ── 3. Raw dump around WC3BOT_STATE key ─────────────────────────────────
    print(f"\n=== Step 3: Raw dump ±64 bytes around WC3BOT_STATE key ===")
    dump = read_mem(task, WC3BOT_KEY_ADDR - 32, 160)
    if dump:
        for row in range(0, len(dump), 16):
            chunk = dump[row:row+16]
            hex_part  = " ".join(f"{b:02x}" for b in chunk)
            safe_part = "".join(chr(b) if 32 <= b < 127 else '.' for b in chunk)
            addr = WC3BOT_KEY_ADDR - 32 + row
            print(f"  0x{addr:016x}  {hex_part:<48}  {safe_part}")


if __name__ == "__main__":
    sys.exit(main())
