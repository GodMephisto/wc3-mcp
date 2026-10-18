---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/read_lumber.py"
ref: HEAD
---

#!/usr/bin/env python3
"""read_lumber.py — P0.5b lumber address finder.

Protocol:
  1. Snapshot A (all peons idle, no mining).
  2. Assign exactly 1 peon to a lumber pile, wait for exactly 1 trip (+10 lumber).
  3. Snapshot B.
  4. Enter new lumber value.
  5. Script finds and validates lumber address.

Usage:
    python3 scripts/read_lumber.py
    python3 scripts/read_lumber.py --once
"""
import ctypes, ctypes.util, struct, subprocess, sys, time, argparse

TARGET        = "Warcraft III"
LUMBER_DELTA  = 10
MAX_REGION    = 64 * 1024 * 1024

libc = ctypes.CDLL(ctypes.util.find_library("c"))
libc.task_for_pid.restype  = ctypes.c_int
libc.task_for_pid.argtypes = [ctypes.c_uint, ctypes.c_int, ctypes.POINTER(ctypes.c_uint)]
libc.mach_task_self.restype  = ctypes.c_uint
libc.mach_task_self.argtypes = []

class _Info(ctypes.Structure):
    _fields_ = [("protection",ctypes.c_int),("max_protection",ctypes.c_int),
                ("inheritance",ctypes.c_uint),("shared",ctypes.c_int),
                ("reserved",ctypes.c_int),("offset",ctypes.c_uint64),
                ("behavior",ctypes.c_int),("user_wired_count",ctypes.c_ushort)]

libc.mach_vm_region.restype  = ctypes.c_int
libc.mach_vm_region.argtypes = [ctypes.c_uint,ctypes.POINTER(ctypes.c_uint64),
    ctypes.POINTER(ctypes.c_uint64),ctypes.c_int,ctypes.POINTER(_Info),
    ctypes.POINTER(ctypes.c_uint),ctypes.POINTER(ctypes.c_uint)]
libc.mach_vm_read_overwrite.restype  = ctypes.c_int
libc.mach_vm_read_overwrite.argtypes = [ctypes.c_uint,ctypes.c_uint64,ctypes.c_uint64,
    ctypes.c_uint64,ctypes.POINTER(ctypes.c_uint64)]


def get_task(pid: int) -> int:
    t = ctypes.c_uint(0)
    assert libc.task_for_pid(libc.mach_task_self(), pid, ctypes.byref(t)) == 0
    return t.value


def read_mem(task: int, addr: int, size: int) -> bytes | None:
    buf = (ctypes.c_char * size)()
    out = ctypes.c_uint64(0)
    kr = libc.mach_vm_read_overwrite(task, addr, size, ctypes.addressof(buf), ctypes.byref(out))
    return bytes(buf[:out.value]) if kr == 0 else None


def snapshot(task: int) -> list[tuple[int, bytes]]:
    regions = []
    addr = ctypes.c_uint64(1)
    while True:
        size = ctypes.c_uint64(0)
        info = _Info(); cnt = ctypes.c_uint(9); obj = ctypes.c_uint(0)
        kr = libc.mach_vm_region(task, ctypes.byref(addr), ctypes.byref(size), 9,
                                  ctypes.byref(info), ctypes.byref(cnt), ctypes.byref(obj))
        if kr != 0: break
        if (info.protection & 3) == 3 and size.value <= MAX_REGION:
            data = b""
            off = 0
            while off < size.value:
                chunk_sz = min(4*1024*1024, size.value - off)
                chunk = read_mem(task, addr.value + off, chunk_sz)
                data += chunk if chunk else b"\x00" * chunk_sz
                off += chunk_sz
            regions.append((addr.value, data))
        addr.value += size.value
    return regions


def find_lumber_addr(snap_a, snap_b, lumber_before: int) -> tuple[int, int] | None:
    b_map = {base: data for base, data in snap_b}
    candidates = []
    for base, data_a in snap_a:
        data_b = b_map.get(base)
        if data_b is None or len(data_b) != len(data_a):
            continue
        align = (4 - base % 4) % 4
        i = align
        while i + 4 <= len(data_a):
            va = struct.unpack_from("<i", data_a, i)[0]
            vb = struct.unpack_from("<i", data_b, i)[0]
            if vb - va == LUMBER_DELTA:
                addr = base + i
                if 0x200000000 <= addr <= 0x70000000000:
                    encode = va - lumber_before
                    candidates.append((addr, encode))
            i += 4
    return candidates


def read_val(task: int, addr: int, encode: int) -> int | None:
    data = read_mem(task, addr, 4)
    if not data or len(data) < 4:
        return None
    return struct.unpack("<i", data)[0] - encode


def main():
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--once", action="store_true", help="find address, print once, exit")
    args = ap.parse_args()

    result = subprocess.run(["pgrep", "-x", TARGET], capture_output=True, text=True)
    if result.returncode != 0:
        print(f"error: '{TARGET}' not running", file=sys.stderr)
        return 1
    pid  = int(result.stdout.strip().split("\n")[0])
    task = get_task(pid)
    print(f"Attached to {TARGET} PID={pid}")

    print("\nStep 1: ALL peons idle (no gold mining, no lumber harvesting).")
    lumber_a = int(input("Current lumber (from HUD): ").strip())
    input("Press Enter to take snapshot A ...")
    snap_a = snapshot(task)
    print(f"  snapshot A: {len(snap_a)} regions")

    print(f"\nStep 2: Assign exactly 1 peon to a lumber pile. Wait for exactly 1 trip (+10).")
    print("  Stop the peon immediately after 1 trip. No other actions.")
    input("Press Enter immediately after the trip completes → snapshot B ...")
    snap_b = snapshot(task)
    print(f"  snapshot B: {len(snap_b)} regions")

    raw = input("\nCurrent lumber AFTER the trip (from HUD): ").strip()
    lumber_b = int(raw)
    actual_delta = lumber_b - lumber_a
    if actual_delta != LUMBER_DELTA:
        print(f"WARNING: expected +{LUMBER_DELTA}, got {actual_delta:+d}. Continuing anyway.")

    print("\nLocating lumber address ...")
    candidates = find_lumber_addr(snap_a, snap_b, lumber_a)
    print(f"  {len(candidates)} candidate(s)")

    # Validate
    valid = [(a, e) for a, e in candidates if read_val(task, a, e) == lumber_b]
    print(f"  {len(valid)} validated")

    if not valid:
        print("ERROR: no validated lumber address found.", file=sys.stderr)
        if candidates:
            print("Candidates (unvalidated):")
            for a, e in candidates[:5]:
                v = read_val(task, a, e)
                print(f"  0x{a:016x}  encode={e}  current={v}")
        return 2

    lumber_addr, lumber_enc = valid[0]
    lv = read_val(task, lumber_addr, lumber_enc)
    print(f"Lumber address:   0x{lumber_addr:016x}")
    print(f"Encode constant:  stored - {lumber_enc} = lumber")
    print(f"Current lumber:   {lv}")

    if args.once:
        return 0

    print("\nWatching lumber (Ctrl+C to stop)...")
    last = None
    while True:
        lv = read_val(task, lumber_addr, lumber_enc)
        if lv != last:
            print(f"  lumber = {lv}")
            last = lv
        time.sleep(0.25)


if __name__ == "__main__":
    sys.exit(main())
