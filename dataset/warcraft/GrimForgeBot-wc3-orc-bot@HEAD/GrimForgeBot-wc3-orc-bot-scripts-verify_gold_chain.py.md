---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/verify_gold_chain.py"
ref: HEAD
---

#!/usr/bin/env python3
"""verify_gold_chain.py — P0.5 stability verification.

Reads gold via the discovered pointer chain:
  image_base + 0x363eef98  ->  player_ptr
  player_ptr + 0x178       ->  gold (i32)

Prints gold value live. Run after each game restart to verify the chain
is stable. P0.5 acceptance = works across ≥3 restarts.

Usage:
    python3 scripts/verify_gold_chain.py          # live watch
    python3 scripts/verify_gold_chain.py --once   # print once and exit
"""
import ctypes, ctypes.util, struct, subprocess, sys, time, argparse

TARGET      = "Warcraft III"
BINARY_PATH = b"/Applications/Warcraft III/_retail_/x86_64/Warcraft III.app/Contents/MacOS/Warcraft III"
PTR_OFFSET   = 0x58f329d0  # image_base + this = pointer to player struct
GOLD_OFFSET  = 0x24c       # player_ptr + this = gold (i32, stored as gold+32260)
GOLD_ENCODE  = 32260       # stored_value - GOLD_ENCODE = actual gold

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


def find_image_base(pid: int, task: int) -> int | None:
    addr = ctypes.c_uint64(1)
    while True:
        size = ctypes.c_uint64(0)
        info = _Info(); cnt = ctypes.c_uint(9); obj = ctypes.c_uint(0)
        kr = libc.mach_vm_region(task, ctypes.byref(addr), ctypes.byref(size), 9,
                                  ctypes.byref(info), ctypes.byref(cnt), ctypes.byref(obj))
        if kr != 0:
            break
        if info.protection & 1:
            buf = ctypes.create_string_buffer(4096)
            ret = libproc.proc_regionfilename(pid, addr.value, buf, 4096)
            if ret > 0 and buf.value == BINARY_PATH:
                return addr.value
        addr.value += size.value
    return None


def read_mem(task: int, address: int, size: int) -> bytes | None:
    buf = (ctypes.c_char * size)()
    out = ctypes.c_uint64(0)
    kr = libc.mach_vm_read_overwrite(task, address, size, ctypes.addressof(buf), ctypes.byref(out))
    return bytes(buf[:out.value]) if kr == 0 else None


def read_gold(task: int, image_base: int) -> int | None:
    ptr_addr = image_base + PTR_OFFSET
    data = read_mem(task, ptr_addr, 8)
    if not data or len(data) < 8:
        return None
    player_ptr = struct.unpack("<Q", data)[0]
    if player_ptr == 0:
        return None
    gold_data = read_mem(task, player_ptr + GOLD_OFFSET, 4)
    if not gold_data or len(gold_data) < 4:
        return None
    stored = struct.unpack("<i", gold_data)[0]
    return stored - GOLD_ENCODE


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--once", action="store_true", help="print once and exit")
    args = ap.parse_args()

    result = subprocess.run(["pgrep", "-x", TARGET], capture_output=True, text=True)
    if result.returncode != 0:
        print(f"error: '{TARGET}' not running", file=sys.stderr)
        return 1
    pid = int(result.stdout.strip().split("\n")[0])
    task = get_task(pid)

    image_base = find_image_base(pid, task)
    if image_base is None:
        print("error: could not find WC3 image base", file=sys.stderr)
        return 1

    print(f"PID={pid}  image_base=0x{image_base:016x}")
    print(f"Chain: [image_base + 0x{PTR_OFFSET:x}] -> player_ptr -> [player_ptr + 0x{GOLD_OFFSET:x}] - {GOLD_ENCODE} = gold")

    if args.once:
        gold = read_gold(task, image_base)
        print(f"gold = {gold}")
        return 0

    print("Watching gold (Ctrl+C to stop)...")
    last = None
    while True:
        gold = read_gold(task, image_base)
        if gold != last:
            print(f"  gold = {gold}")
            last = gold
        time.sleep(0.25)


if __name__ == "__main__":
    sys.exit(main())
