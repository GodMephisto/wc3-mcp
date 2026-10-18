---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/scan_rwx.py"
ref: HEAD
---

#!/usr/bin/env python3
"""scan_rwx.py — scan rwx regions + f64 type to find gold counter."""
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

def scan_all(task, patterns_by_label):
    """Scan all readable regions for multiple byte patterns.
    patterns_by_label: dict[label -> bytes]
    Returns dict[label -> list[int]]
    """
    hits = {label: [] for label in patterns_by_label}
    addr = ctypes.c_uint64(1)
    total_mb = 0
    while True:
        size = ctypes.c_uint64(0)
        info = _Info(); cnt = ctypes.c_uint(9); obj = ctypes.c_uint(0)
        kr = libc.mach_vm_region(task, ctypes.byref(addr), ctypes.byref(size), 9,
                                  ctypes.byref(info), ctypes.byref(cnt), ctypes.byref(obj))
        if kr != 0: break
        p = info.protection
        is_readable  = bool(p & 1)
        is_writable  = bool(p & 2)
        if is_readable and is_writable:
            off = 0
            while off < size.value:
                chunk_sz = min(4*1024*1024, size.value - off)
                chunk = read_mem(task, addr.value + off, chunk_sz)
                if chunk:
                    for label, pat in patterns_by_label.items():
                        pos = 0
                        while True:
                            idx = chunk.find(pat, pos)
                            if idx == -1: break
                            abs_addr = addr.value + off + idx
                            if abs_addr % 4 == 0:  # 4-byte aligned only
                                hits[label].append(abs_addr)
                            pos = idx + 1
                off += chunk_sz
            total_mb += size.value / 1024 / 1024
        addr.value += size.value
    print(f"  (scanned {total_mb:.0f} MB of rw+rwx)", file=sys.stderr)
    return hits

def snapshot(task, addrs):
    PAGE = 4096
    snap = {}
    for addr in sorted(set(addrs)):
        pg = addr & ~(PAGE - 1)
        data = read_mem(task, pg, PAGE)
        if data:
            off = addr - pg
            if off + 8 <= len(data):
                snap[addr] = data[off:off+8]
    return snap

pid = int(subprocess.run(["pgrep","-x","Warcraft III"],capture_output=True,text=True).stdout.strip().split("\n")[0])
task = get_task(pid)
print(f"PID={pid} task={task}")

v1 = int(input("current gold: "))

patterns = {
    "i32":  struct.pack("<i", v1),
    "u32":  struct.pack("<I", v1 & 0xFFFFFFFF),
    "f32":  struct.pack("<f", float(v1)),
    "f64":  struct.pack("<d", float(v1)),
    "i64":  struct.pack("<q", v1),
}
print("scanning rw + rwx regions for all types ...")
by_type = scan_all(task, patterns)
for label, addrs in by_type.items():
    print(f"  {label:>4}: {len(addrs)}")

input("\nchange gold, then press Enter ...")
v2 = int(input("new gold: "))

print("\nfiltering ...")
for label, addrs in by_type.items():
    if not addrs:
        continue
    if label in ("i32","u32"):
        width, fmt = 4, {"i32":"<i","u32":"<I"}[label]
    elif label == "f32":
        width, fmt = 4, "<f"
    elif label == "f64":
        width, fmt = 8, "<d"
    elif label == "i64":
        width, fmt = 8, "<q"
    snap = snapshot(task, addrs)
    survivors = []
    for addr, data in snap.items():
        if len(data) < width: continue
        val = struct.unpack(fmt, data[:width])[0]
        if abs(val - v2) == 0:
            survivors.append((addr, val))
    print(f"  {label:>4}: {len(addrs)} -> {len(survivors)} survivors")
    for addr, val in survivors[:10]:
        print(f"          0x{addr:016x}  value={val}")
