---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/diag_bootstrap.py"
ref: HEAD
---

#!/usr/bin/env python3
"""diag_bootstrap.py — Diagnose why LuaStateReader bootstrap fails.

Shows:
1. Raw bytes around the WC3BOT_STATE key string
2. All possible TString addresses (offsets 12-40) and whether any pointer to them exists
3. The actual bytes following each candidate pointer in memory
"""
import ctypes, ctypes.util, struct, subprocess, sys
sys.path.insert(0, "src")

TARGET = "Warcraft III"

_libc = ctypes.CDLL(ctypes.util.find_library("c"))
_libc.task_for_pid.restype  = ctypes.c_int
_libc.task_for_pid.argtypes = [ctypes.c_uint, ctypes.c_int, ctypes.POINTER(ctypes.c_uint)]
_libc.mach_task_self.restype  = ctypes.c_uint
_libc.mach_task_self.argtypes = []
_libc.mach_vm_read_overwrite.restype  = ctypes.c_int
_libc.mach_vm_read_overwrite.argtypes = [
    ctypes.c_uint, ctypes.c_uint64, ctypes.c_uint64,
    ctypes.c_uint64, ctypes.POINTER(ctypes.c_uint64),
]

class _VMInfo(ctypes.Structure):
    _fields_ = [("protection",ctypes.c_int),("max_protection",ctypes.c_int),
                ("inheritance",ctypes.c_uint),("shared",ctypes.c_int),
                ("reserved",ctypes.c_int),("offset",ctypes.c_uint64),
                ("behavior",ctypes.c_int),("user_wired_count",ctypes.c_ushort)]

_libc.mach_vm_region.restype  = ctypes.c_int
_libc.mach_vm_region.argtypes = [ctypes.c_uint,ctypes.POINTER(ctypes.c_uint64),
    ctypes.POINTER(ctypes.c_uint64),ctypes.c_int,ctypes.POINTER(_VMInfo),
    ctypes.POINTER(ctypes.c_uint),ctypes.POINTER(ctypes.c_uint)]

def get_task(pid):
    t = ctypes.c_uint(0)
    assert _libc.task_for_pid(_libc.mach_task_self(), pid, ctypes.byref(t)) == 0
    return t.value

def read_mem(task, addr, size):
    if size <= 0: return b""
    buf = (ctypes.c_char * size)()
    out = ctypes.c_uint64(0)
    kr  = _libc.mach_vm_read_overwrite(task, addr, size, ctypes.addressof(buf), ctypes.byref(out))
    return bytes(buf[:out.value]) if kr == 0 else None

def iter_regions(task):
    addr = ctypes.c_uint64(1)
    while True:
        size = ctypes.c_uint64(0); info = _VMInfo(); cnt = ctypes.c_uint(9); obj = ctypes.c_uint(0)
        kr = _libc.mach_vm_region(task, ctypes.byref(addr), ctypes.byref(size), 9,
                                   ctypes.byref(info), ctypes.byref(cnt), ctypes.byref(obj))
        if kr != 0: break
        yield addr.value, size.value, info.protection
        addr.value += size.value

def read_region(task, base, size):
    CHUNK = 4*1024*1024; parts, off = [], 0
    while off < size:
        n = min(CHUNK, size-off); chunk = read_mem(task, base+off, n)
        parts.append(chunk if chunk else b"\x00"*n); off += n
    return b"".join(parts)

def hexdump(data, base=0):
    for r in range(0, min(len(data), 80), 16):
        c = data[r:r+16]
        h = " ".join(f"{b:02x}" for b in c)
        s = "".join(chr(b) if 32<=b<127 else "." for b in c)
        print(f"  0x{base+r:016x}  {h:<48}  {s}")

KEY_BYTES = b"WC3BOT_STATE\x00"
MAX_REGION = 512*1024*1024

result = subprocess.run(["pgrep", "-x", TARGET], capture_output=True, text=True)
if result.returncode != 0:
    print(f"error: {TARGET} not running"); sys.exit(1)
pid  = int(result.stdout.strip().split("\n")[0])
task = get_task(pid)
print(f"Attached to {TARGET} PID={pid}\n")

# ── Step 1: find all WC3BOT_STATE occurrences ──────────────────────────────────
print("=== Step 1: Finding WC3BOT_STATE key string in memory ===")
key_finds = []  # (content_addr, region_base, region_size)
for base, size, prot in iter_regions(task):
    if not (prot & 1): continue
    if size > MAX_REGION: continue
    data = read_region(task, base, size)
    off = 0
    while True:
        idx = data.find(KEY_BYTES, off)
        if idx == -1: break
        content_addr = base + idx
        key_finds.append((content_addr, base, size))
        off = idx + 1

print(f"Found {len(key_finds)} occurrence(s):")
for content_addr, rbase, rsize in key_finds:
    print(f"\n  content @ 0x{content_addr:016x}  (region 0x{rbase:x}+0x{rsize:x})")
    # Dump 48 bytes before the content to see TString header
    pre = read_mem(task, content_addr - 48, 80)
    if pre:
        print("  -48 to +32 bytes:")
        hexdump(pre, content_addr - 48)

# ── Step 2: for each found string, try all header offsets ─────────────────────
print("\n=== Step 2: Testing header offsets (12..40) — search for pointer in mem ===")

# Build candidate tstr_addrs for all reasonable header offsets
all_candidates = {}  # tstr_addr → (content_addr, offset)
for content_addr, _, _ in key_finds:
    for hdr_off in range(12, 44, 4):
        tstr_addr = content_addr - hdr_off
        if tstr_addr > 0:
            all_candidates[tstr_addr] = (content_addr, hdr_off)

print(f"Testing {len(all_candidates)} candidate TString addresses…")

# Scan all regions for pointers to any candidate
found_pointers = []  # (ptr_region_base, ptr_offset_in_region, ptr_val, ptr_addr, key_tt_byte)
for base, size, prot in iter_regions(task):
    if not (prot & 1): continue
    if size > MAX_REGION: continue
    data = read_region(task, base, size)
    for i in range(0, len(data) - 9, 4):
        ptr = struct.unpack_from("<Q", data, i)[0]
        if ptr in all_candidates:
            content_addr, hdr_off = all_candidates[ptr]
            key_tt_byte = data[i+8] if i+8 < len(data) else 0xff
            ptr_addr = base + i
            found_pointers.append((ptr_addr, ptr, hdr_off, content_addr, key_tt_byte,
                                   data[i:i+16] if i+16 <= len(data) else b""))

if found_pointers:
    print(f"\nFound {len(found_pointers)} pointer(s) to candidate TString addrs:")
    for ptr_addr, ptr_val, hdr_off, content_addr, key_tt_byte, ctx in found_pointers:
        print(f"\n  ptr @ 0x{ptr_addr:016x} → 0x{ptr_val:x}"
              f"  (hdr_off={hdr_off}, content=0x{content_addr:x})")
        print(f"  bytes at ptr: " + " ".join(f"{b:02x}" for b in ctx))
        print(f"  key_tt byte @ ptr+8: 0x{key_tt_byte:02x}")
        # Node base = ptr_addr - 16
        node_base = ptr_addr - 16
        node_hdr = read_mem(task, node_base, 32)
        if node_hdr:
            print(f"  Node candidate @ 0x{node_base:016x}:")
            hexdump(node_hdr, node_base)
            val_ptr = struct.unpack_from("<Q", node_hdr, 0)[0]
            val_tt  = node_hdr[8]
            print(f"  → i_val.ptr=0x{val_ptr:x}  i_val.tt_byte=0x{val_tt:02x}")
else:
    print("\nNo pointers found — the Node might be in a non-readable region.")
    print("Checking if any candidates have pointers in ANY region (including non-readable):")
    # Just dump the region map around key_finds
    for content_addr, rbase, rsize in key_finds[:1]:
        tstr_candidates = [(content_addr - h, h) for h in range(12, 44, 4)]
        print(f"\nRegion containing key: 0x{rbase:x} .. 0x{rbase+rsize:x}")
        print("Nearby region map:")
        check_range_lo = content_addr - 0x10000
        check_range_hi = content_addr + 0x10000
        for b2, s2, p2 in iter_regions(task):
            if b2 + s2 < check_range_lo: continue
            if b2 > check_range_hi: break
            print(f"  0x{b2:016x} .. 0x{b2+s2:016x}  prot=0x{p2:x}  size={s2//1024}K")
