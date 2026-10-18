---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/watch_resources.py"
ref: HEAD
---

#!/usr/bin/env python3
"""watch_resources.py — live resource watcher using discovered session addresses.

Fill in the addresses from your scan-on-start scripts before running.
This is the P0.5b acceptance verification tool.

Usage:
    python3 scripts/watch_resources.py
"""
import ctypes, ctypes.util, struct, subprocess, sys, time, argparse

TARGET = "Warcraft III"

libc = ctypes.CDLL(ctypes.util.find_library("c"))
libc.task_for_pid.restype  = ctypes.c_int
libc.task_for_pid.argtypes = [ctypes.c_uint, ctypes.c_int, ctypes.POINTER(ctypes.c_uint)]
libc.mach_task_self.restype  = ctypes.c_uint
libc.mach_task_self.argtypes = []
libc.mach_vm_read_overwrite.restype  = ctypes.c_int
libc.mach_vm_read_overwrite.argtypes = [ctypes.c_uint, ctypes.c_uint64, ctypes.c_uint64,
    ctypes.c_uint64, ctypes.POINTER(ctypes.c_uint64)]


def get_task(pid: int) -> int:
    t = ctypes.c_uint(0)
    assert libc.task_for_pid(libc.mach_task_self(), pid, ctypes.byref(t)) == 0
    return t.value


def read_i32(task: int, addr: int, encode: int = 0) -> int | None:
    buf = (ctypes.c_char * 4)()
    out = ctypes.c_uint64(0)
    kr = libc.mach_vm_read_overwrite(task, addr, 4, ctypes.addressof(buf), ctypes.byref(out))
    if kr != 0 or out.value < 4:
        return None
    return struct.unpack("<i", buf)[0] - encode


ap = argparse.ArgumentParser(description=__doc__)
ap.add_argument("--gold-addr",  type=lambda x: int(x, 16), required=True)
ap.add_argument("--gold-enc",   type=int, required=True)
ap.add_argument("--lumber-addr", type=lambda x: int(x, 16), required=True)
ap.add_argument("--lumber-enc",  type=int, required=True)
ap.add_argument("--food-addr",  type=lambda x: int(x, 16), required=True)
ap.add_argument("--duration",   type=int, default=0,
                help="stop after N seconds (0=infinite)")
args = ap.parse_args()

result = subprocess.run(["pgrep", "-x", TARGET], capture_output=True, text=True)
if result.returncode != 0:
    print(f"error: '{TARGET}' not running", file=sys.stderr)
    sys.exit(1)
pid  = int(result.stdout.strip().split("\n")[0])
task = get_task(pid)
print(f"Attached to {TARGET} PID={pid}")
print(f"Watching gold/lumber/food (Ctrl+C to stop) ...\n")

start = time.time()
last = None
while True:
    g  = read_i32(task, args.gold_addr,   args.gold_enc)
    lv = read_i32(task, args.lumber_addr, args.lumber_enc)
    f  = read_i32(task, args.food_addr,   0)
    state = (g, lv, f)
    if state != last:
        elapsed = time.time() - start
        print(f"  [{elapsed:5.1f}s]  gold={g:>5}  lumber={lv:>4}  food_used={f}")
        last = state
    if args.duration and (time.time() - start) >= args.duration:
        print(f"\nDone ({args.duration}s sample complete).")
        break
    time.sleep(0.1)
