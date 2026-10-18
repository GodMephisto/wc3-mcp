---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/scan_diag.py"
ref: HEAD
---

#!/usr/bin/env python3
"""scan_diag.py — diagnoses why gold filter drops to zero.

1. Scans for initial value (i32 only).
2. Waits for user to change gold.
3. Re-reads all candidates and prints a histogram of what values are there now.
   This tells us whether:
   a) the real gold address IS in the set but has a different value than expected
   b) nothing changed at all (read is returning stale/wrong data)
   c) all values are garbage (wrong region)
"""
import ctypes, ctypes.util, struct, subprocess, sys
from collections import Counter

TARGET = "Warcraft III"
libc = ctypes.CDLL(ctypes.util.find_library("c"))
libc.task_for_pid.restype = ctypes.c_int
libc.task_for_pid.argtypes = [ctypes.c_uint, ctypes.c_int, ctypes.POINTER(ctypes.c_uint)]
libc.mach_task_self.restype = ctypes.c_uint
libc.mach_task_self.argtypes = []
VM_REGION_BASIC_INFO_64 = 9
VM_REGION_BASIC_INFO_COUNT_64 = 9

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
    kr = libc.task_for_pid(libc.mach_task_self(), pid, ctypes.byref(t))
    assert kr == 0, f"task_for_pid failed kr={kr}"
    return t.value

def read_mem(task, addr, size):
    buf = (ctypes.c_char * size)()
    out = ctypes.c_uint64(0)
    kr = libc.mach_vm_read_overwrite(task, addr, size, ctypes.addressof(buf), ctypes.byref(out))
    return bytes(buf[:out.value]) if kr == 0 else None

def iter_rw(task):
    addr = ctypes.c_uint64(1)
    while True:
        size = ctypes.c_uint64(0)
        info = _Info(); cnt = ctypes.c_uint(VM_REGION_BASIC_INFO_COUNT_64); obj = ctypes.c_uint(0)
        kr = libc.mach_vm_region(task, ctypes.byref(addr), ctypes.byref(size),
                                  VM_REGION_BASIC_INFO_64, ctypes.byref(info), ctypes.byref(cnt), ctypes.byref(obj))
        if kr != 0: break
        if (info.protection & 3) == 3:
            yield addr.value, size.value
        addr.value += size.value

def scan(task, value):
    pat = struct.pack("<i", value)
    hits = []
    for base, size in iter_rw(task):
        off = 0
        while off < size:
            chunk = read_mem(task, base + off, min(4*1024*1024, size - off))
            if chunk:
                pos = 0
                while True:
                    idx = chunk.find(pat, pos)
                    if idx == -1: break
                    hits.append(base + off + idx)
                    pos = idx + 1
            off += min(4*1024*1024, size - off)
    return hits

def snapshot(task, addrs):
    PAGE = 4096
    snap = {}
    for addr in sorted(addrs):
        pg = addr & ~(PAGE-1)
        data = read_mem(task, pg, PAGE)
        if data:
            off = addr - pg
            if off + 4 <= len(data):
                snap[addr] = data[off:off+4]
    return snap

pid = int(subprocess.run(["pgrep","-x",TARGET],capture_output=True,text=True).stdout.strip().split("\n")[0])
task = get_task(pid)
print(f"PID={pid} task={task}")

v1 = int(input("current gold: "))
print("scanning i32 ...")
hits = scan(task, v1)
print(f"  {len(hits)} candidates")

input(f"change gold (buy something or cheat), then press Enter ...")
v2 = int(input("new gold value shown in HUD: "))

snap = snapshot(task, hits)
values = [struct.unpack("<i", b)[0] for b in snap.values()]

# Show distribution
counter = Counter(values)
print(f"\nTop 20 values now at the {len(values)} candidate addresses:")
for val, cnt in counter.most_common(20):
    marker = " <-- TARGET" if val == v2 else ""
    print(f"  {val:8d}  x{cnt}{marker}")

print(f"\nStill equal to v1={v1}: {sum(1 for v in values if v == v1)}")
print(f"Equal to v2={v2}: {sum(1 for v in values if v == v2)}")
print(f"Delta = v2-v1 = {v2-v1}")
print(f"Changed by delta: {sum(1 for addr,b in snap.items() if struct.unpack('<i',b)[0] - v1 == v2-v1)}")
