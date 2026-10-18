---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/diag_scan3.py"
ref: HEAD
---

#!/usr/bin/env python3
"""diag_scan3.py — Follow specific pointers from Step 3 raw dump.

From diag_scan2 Step 3 we know:
  WC3BOT_STATE key string @ 0x15f4e7380
  Pointer @ 0x15f4e73b0 → 0x15d4faca0 (possible VALUE TString)
  Pointer @ 0x15f4e7378 → 0x15cc57610 (hnext in string hash)
  Pointer @ 0x15f4e7390 → 0x1471adca0 (next GCobj?)

Plan:
  1. Find and print the VM region containing 0x15f4e7380 (base, size, prot).
  2. Search that ENTIRE region (ignoring size limit) for the sidecar pattern.
  3. Read 128 bytes at each pointer target, print as hex+ascii.
  4. Also read 128 bytes at target+32 (TString char data if 32-byte header).
"""
import ctypes, ctypes.util, re, struct, subprocess, sys

TARGET = "Warcraft III"
WC3BOT_KEY_STR   = 0x15f4e7380   # where "WC3BOT_STATE" chars are
POINTER_TARGETS  = {
    "ptr@73b0→VALUE?":   0x000000015d4faca0,
    "ptr@7378→hnext?":   0x000000015cc57610,
    "ptr@7360→nextgc?":  0x000000015f4e7330,
    "ptr@7390→nextobj?": 0x00000001471adca0,
}

SIDECAR_RE = re.compile(
    rb'T:\d+\.?\d*\|G:\d+\|L:\d+\|FU:\d+\|FC:\d+',
    re.ASCII,
)

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

def hexdump(data, base_addr=0):
    for row in range(0, len(data), 16):
        chunk = data[row:row+16]
        hex_part  = " ".join(f"{b:02x}" for b in chunk)
        safe_part = "".join(chr(b) if 32 <= b < 127 else '.' for b in chunk)
        print(f"  0x{base_addr+row:016x}  {hex_part:<48}  {safe_part}")

def read_region_chunks(task, base, size):
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

    # ── 1. Find region containing WC3BOT_STATE key ───────────────────────────
    print(f"=== Step 1: Region containing 0x{WC3BOT_KEY_STR:x} ===")
    key_region_base = key_region_size = key_region_prot = None
    for base, size, prot in iter_regions(task):
        if base <= WC3BOT_KEY_STR < base + size:
            key_region_base = base
            key_region_size = size
            key_region_prot = prot
            break
    if key_region_base is None:
        print("  ERROR: region not found")
    else:
        print(f"  base=0x{key_region_base:016x}  size={key_region_size//1024//1024}MB  prot={key_region_prot:#x}")

    # ── 2. Search that entire region for sidecar pattern ─────────────────────
    if key_region_base is not None:
        print(f"\n=== Step 2: Search entire region ({key_region_size//1024//1024}MB) for sidecar ===")
        import time
        t0 = time.time()
        region_data = read_region_chunks(task, key_region_base, key_region_size)
        m = SIDECAR_RE.search(region_data)
        if m:
            abs_addr = key_region_base + m.start()
            print(f"  FOUND @ 0x{abs_addr:016x}: {m.group(0)[:120]!r}")
        else:
            print(f"  Not found in this region ({time.time()-t0:.1f}s, {key_region_size//1024//1024}MB)")
            # Try partial: look for just "T:" followed by digits and "|G:"
            partial_re = re.compile(rb'T:\d+\.?\d*\|G:\d+', re.ASCII)
            m2 = partial_re.search(region_data)
            if m2:
                abs_addr = key_region_base + m2.start()
                ctx = region_data[m2.start():m2.start()+150]
                safe = bytes(b if 32 <= b < 127 else ord('.') for b in ctx)
                print(f"  Partial 'T:...|G:' FOUND @ 0x{abs_addr:016x}: {safe!r}")
            else:
                print(f"  Even partial 'T:...|G:' not found in this region!")

    # ── 3. Read pointer targets from Step 3 dump ─────────────────────────────
    print(f"\n=== Step 3: Dereference pointers from raw dump ===")
    for label, addr in POINTER_TARGETS.items():
        print(f"\n  [{label}]  @ 0x{addr:016x}")
        data = read_mem(task, addr, 128)
        if data is None or len(data) < 4:
            print("    → unreadable")
            continue
        hexdump(data, addr)
        # Check at offset 16 and 32 for string content (TString char data)
        for hdr_size in [16, 20, 24, 32]:
            if hdr_size < len(data):
                substr = data[hdr_size:hdr_size+80]
                safe = bytes(b if 32 <= b < 127 else ord('.') for b in substr)
                if b'T:' in substr and b'|' in substr:
                    print(f"    *** SIDECAR STRING at offset +{hdr_size}: {safe!r}")
                elif sum(32 <= b < 127 for b in substr[:20]) >= 15:
                    print(f"    Printable at offset +{hdr_size}: {safe[:60]!r}")

    # ── 4. Also scan the 15d4faca0 region directly ───────────────────────────
    val_ptr = 0x000000015d4faca0
    print(f"\n=== Step 4: Read 256 bytes at VALUE pointer + wider context ===")
    # Read 256 bytes starting 32 bytes before (to see header + data)
    data = read_mem(task, val_ptr - 32, 256)
    if data:
        print(f"  Reading 256 bytes @ 0x{val_ptr-32:x} (=ptr-32):")
        hexdump(data, val_ptr - 32)
    else:
        print(f"  Unreadable at 0x{val_ptr-32:x}")

    # ── 5. Check if WC3BOT_STATE value is nil (never assigned) ───────────────
    print(f"\n=== Step 5: Look for 'nil' indicator near WC3BOT_STATE in table ===")
    # In Lua tables, a nil value for a key means the key doesn't exist.
    # If the global _G.WC3BOT_STATE is nil, the "WC3BOT_STATE" string still
    # exists in the string pool (because source code references it),
    # but there's no table node pointing to a value.
    # The Node for WC3BOT_STATE (if the global IS set) would be:
    #   TValue val: {ptr to string, type=4}
    #   TKey key:   {ptr to "WC3BOT_STATE", type=4, next_offset}
    # We need to find a node where key.ptr == WC3BOT_KEY_STR - 32 (TString base)
    tstr_base = WC3BOT_KEY_STR - 32  # TString object start (before 32-byte header)
    print(f"  TString base addr of key = 0x{tstr_base:x}")
    print(f"  Searching ALL RW regions for this address as a 64-bit pointer ...")
    found_nodes = []
    for base, size, prot in iter_regions(task):
        if (prot & 3) != 3: continue
        if size > 256 * 1024 * 1024: continue
        data = read_region_chunks(task, base, size)
        align = (8 - base % 8) % 8
        i = align
        while i + 24 <= len(data):
            v = struct.unpack_from("<Q", data, i)[0]
            if v == tstr_base:
                abs_ptr = base + i
                # Read 48 bytes before (might be TValue=value) + 48 after
                ctx_before = data[max(0,i-24):i]
                ctx_after  = data[i:i+48]
                hex_b = " ".join(f"{x:02x}" for x in ctx_before)
                hex_a = " ".join(f"{x:02x}" for x in ctx_after)
                safe_b = "".join(chr(x) if 32<=x<127 else '.' for x in ctx_before)
                safe_a = "".join(chr(x) if 32<=x<127 else '.' for x in ctx_after)
                print(f"    Key ptr @ 0x{abs_ptr:016x}")
                print(f"      BEFORE (value?): {hex_b}  |{safe_b}|")
                print(f"      AT+AFTER (key):  {hex_a}  |{safe_a}|")
                found_nodes.append(abs_ptr)
            i += 8
    if not found_nodes:
        print(f"  No pointer to TString base found — key might never have been assigned!")
        print(f"  (The string literal 'WC3BOT_STATE' appears in source but global never set)")


if __name__ == "__main__":
    sys.exit(main())
