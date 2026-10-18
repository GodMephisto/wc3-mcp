---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/attach_sanity.py"
ref: HEAD
---

#!/usr/bin/env python3
"""attach_sanity.py — P0.4b attach sanity check (mac-setup runbook Step 4).

Read-only: attaches to the running Rosetta WC3 process via Mach task_for_pid,
enumerates the first 3 loaded modules via mach_vm_region, prints them.
No memory is modified.

Backend: Mach VM APIs via ctypes (replaces Frida after Darwin-25 compat issue).
Requires Python binary signed with com.apple.security.cs.debugger.

Usage:
    source .venv/bin/activate
    python3 scripts/attach_sanity.py

Exits 0 on success, non-zero if task_for_pid fails.
"""
from __future__ import annotations

import ctypes
import ctypes.util
import subprocess
import sys

TARGET = "Warcraft III"

libc = ctypes.CDLL(ctypes.util.find_library("c"))
libc.task_for_pid.restype = ctypes.c_int
libc.task_for_pid.argtypes = [ctypes.c_uint, ctypes.c_int, ctypes.POINTER(ctypes.c_uint)]
libc.mach_task_self.restype = ctypes.c_uint
libc.mach_task_self.argtypes = []

VM_REGION_BASIC_INFO_64 = 9
VM_REGION_BASIC_INFO_COUNT_64 = 9

class _VMRegionBasicInfo64(ctypes.Structure):
    _fields_ = [
        ("protection",       ctypes.c_int),
        ("max_protection",   ctypes.c_int),
        ("inheritance",      ctypes.c_uint),
        ("shared",           ctypes.c_int),
        ("reserved",         ctypes.c_int),
        ("offset",           ctypes.c_uint64),
        ("behavior",         ctypes.c_int),
        ("user_wired_count", ctypes.c_ushort),
    ]

libc.mach_vm_region.restype = ctypes.c_int
libc.mach_vm_region.argtypes = [
    ctypes.c_uint,
    ctypes.POINTER(ctypes.c_uint64),
    ctypes.POINTER(ctypes.c_uint64),
    ctypes.c_int,
    ctypes.POINTER(_VMRegionBasicInfo64),
    ctypes.POINTER(ctypes.c_uint),
    ctypes.POINTER(ctypes.c_uint),
]


def main() -> int:
    result = subprocess.run(["pgrep", "-x", TARGET], capture_output=True, text=True)
    if result.returncode != 0:
        print(f"error: process '{TARGET}' not found — start WC3 first", file=sys.stderr)
        return 2

    pid = int(result.stdout.strip().split("\n")[0])
    task = ctypes.c_uint(0)
    kr = libc.task_for_pid(libc.mach_task_self(), pid, ctypes.byref(task))
    if kr != 0:
        print(f"error: task_for_pid failed (kern_return={kr})", file=sys.stderr)
        print("Hint: ensure Python is signed — see runbook Step 4", file=sys.stderr)
        return 3

    # Enumerate first 3 executable regions as a proxy for "modules loaded"
    addr = ctypes.c_uint64(1)
    found = 0
    VM_PROT_EXECUTE = 0x4
    while found < 3:
        size = ctypes.c_uint64(0)
        info = _VMRegionBasicInfo64()
        cnt  = ctypes.c_uint(VM_REGION_BASIC_INFO_COUNT_64)
        obj  = ctypes.c_uint(0)
        kr2  = libc.mach_vm_region(
            task.value, ctypes.byref(addr), ctypes.byref(size),
            VM_REGION_BASIC_INFO_64, ctypes.byref(info),
            ctypes.byref(cnt), ctypes.byref(obj),
        )
        if kr2 != 0:
            break
        if info.protection & VM_PROT_EXECUTE:
            print(f"  region @ 0x{addr.value:016x}  size=0x{size.value:x}  prot={info.protection:#x}")
            found += 1
        addr.value += size.value

    if found == 0:
        print("warning: no executable regions found (unexpected)", file=sys.stderr)
        return 4

    print("attach OK")
    return 0


if __name__ == "__main__":
    sys.exit(main())
