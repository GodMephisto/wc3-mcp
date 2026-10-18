---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/find_pointer_chain.py"
ref: HEAD
---

#!/usr/bin/env python3
"""find_pointer_chain.py — P0.5: scan for pointers to the gold address.

Given a heap address (gold counter), scans all readable memory for 8-byte
values that point at it (or near it, within ±4096 bytes). For each pointer
found, checks if IT lives in a module-backed region (stable) or is also heap
(needs further chain). Prints results grouped by stability.

Usage:
    python3 scripts/find_pointer_chain.py <gold_addr_hex>
    e.g.:  python3 scripts/find_pointer_chain.py 0x000000030c7b2a38
"""
import ctypes, ctypes.util, struct, subprocess, sys

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

def get_task(pid):
    t = ctypes.c_uint(0)
    assert libc.task_for_pid(libc.mach_task_self(), pid, ctypes.byref(t)) == 0
    return t.value

def read_mem(task, addr, size):
    buf = (ctypes.c_char * size)()
    out = ctypes.c_uint64(0)
    kr = libc.mach_vm_read_overwrite(task, addr, size, ctypes.addressof(buf), ctypes.byref(out))
    return bytes(buf[:out.value]) if kr == 0 else None

def get_region(task, addr):
    a = ctypes.c_uint64(addr)
    size = ctypes.c_uint64(0)
    info = _Info(); cnt = ctypes.c_uint(9); obj = ctypes.c_uint(0)
    kr = libc.mach_vm_region(task, ctypes.byref(a), ctypes.byref(size), 9,
                              ctypes.byref(info), ctypes.byref(cnt), ctypes.byref(obj))
    if kr != 0:
        return None
    return a.value, size.value, info.protection

def get_vmmap(pid):
    """Returns list of (lo, hi, label) from vmmap --wide output."""
    try:
        out = subprocess.run(["vmmap", "--wide", str(pid)],
                             capture_output=True, text=True, timeout=20).stdout
    except Exception:
        return []
    entries = []
    for line in out.splitlines():
        parts = line.split()
        for part in parts:
            if "-" in part and len(part) > 10:
                try:
                    lo_s, hi_s = part.split("-", 1)
                    lo, hi = int(lo_s, 16), int(hi_s, 16)
                    # label = last token if it has a path
                    label = parts[-1] if "/" in parts[-1] else parts[0] if parts else ""
                    entries.append((lo, hi, label))
                    break
                except (ValueError, IndexError):
                    continue
    return entries

def vmmap_lookup(entries, addr):
    for lo, hi, label in entries:
        if lo <= addr < hi:
            return label, lo
    return None, None

TARGET = "Warcraft III"
if len(sys.argv) < 2:
    print("usage: find_pointer_chain.py <gold_addr_hex>")
    sys.exit(1)

gold_addr = int(sys.argv[1], 16)
NEAR = 4096  # scan for pointers within ±NEAR of gold_addr

pid = int(subprocess.run(["pgrep","-x",TARGET],capture_output=True,text=True).stdout.strip().split("\n")[0])
task = get_task(pid)
print(f"PID={pid}  gold_addr=0x{gold_addr:016x}")

print("Loading vmmap (may take a few seconds)...")
vmmap_entries = get_vmmap(pid)
print(f"  {len(vmmap_entries)} vmmap regions loaded")

# Build list of all readable regions
regions = []
addr = ctypes.c_uint64(1)
while True:
    size = ctypes.c_uint64(0)
    info = _Info(); cnt = ctypes.c_uint(9); obj = ctypes.c_uint(0)
    kr = libc.mach_vm_region(task, ctypes.byref(addr), ctypes.byref(size), 9,
                              ctypes.byref(info), ctypes.byref(cnt), ctypes.byref(obj))
    if kr != 0: break
    if (info.protection & 3) == 3:  # rw- or rwx only — pointer to heap always in writable mem
        regions.append((addr.value, size.value, info.protection))
    addr.value += size.value

print(f"Scanning {len(regions)} readable regions for pointers near 0x{gold_addr:016x} ...")
lo_bound = gold_addr - NEAR
hi_bound = gold_addr + NEAR

pointer_hits = []
total_mb = 0
for reg_base, reg_size, reg_prot in regions:
    off = 0
    while off < reg_size:
        chunk_sz = min(4*1024*1024, reg_size - off)
        chunk = read_mem(task, reg_base + off, chunk_sz)
        if chunk:
            # scan for 8-byte LE values in [lo_bound, hi_bound]
            for i in range(0, len(chunk) - 7, 8):
                val = struct.unpack_from("<Q", chunk, i)[0]
                if lo_bound <= val <= hi_bound:
                    ptr_addr = reg_base + off + i
                    offset_from_gold = val - gold_addr
                    pointer_hits.append((ptr_addr, val, offset_from_gold))
        off += chunk_sz
    total_mb += reg_size / 1024 / 1024

print(f"  scanned {total_mb:.0f} MB, found {len(pointer_hits)} pointer(s)")

# Classify each pointer location
print("\n--- Pointer locations ---")
stable = []
heap   = []
for ptr_addr, points_to, delta in pointer_hits:
    label, region_lo = vmmap_lookup(vmmap_entries, ptr_addr)
    reg = get_region(task, ptr_addr)
    prot_str = "???"
    if reg:
        p = reg[2]
        prot_str = ("r" if p&1 else "-") + ("w" if p&2 else "-") + ("x" if p&4 else "-")
    entry = (ptr_addr, points_to, delta, label or "", region_lo or 0, prot_str)
    if label and "/" in label:
        stable.append(entry)
    else:
        heap.append(entry)

print(f"\nSTABLE (module-backed): {len(stable)}")
for ptr_addr, points_to, delta, label, reg_lo, prot in stable[:20]:
    offset_in_module = ptr_addr - reg_lo if reg_lo else 0
    print(f"  ptr @ 0x{ptr_addr:016x}  ->  0x{points_to:016x} (gold{delta:+d})")
    print(f"       module={label}  module_offset=0x{offset_in_module:x}  prot={prot}")

print(f"\nHEAP (need further chain): {len(heap)}")
for ptr_addr, points_to, delta, label, reg_lo, prot in heap[:20]:
    print(f"  ptr @ 0x{ptr_addr:016x}  ->  0x{points_to:016x} (gold{delta:+d})  prot={prot}")
