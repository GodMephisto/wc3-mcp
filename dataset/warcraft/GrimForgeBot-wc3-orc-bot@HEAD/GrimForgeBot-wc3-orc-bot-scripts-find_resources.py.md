---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/find_resources.py"
ref: HEAD
---

#!/usr/bin/env python3
"""find_resources.py — robust resource address finder.

WC3 encodes resources as: stored = actual + key
The key rotates every ~30s (anti-memory-scan). To survive this we:
  1. Find gold address by delta scan (-75 for 1 peon, done in <5s)
  2. Find lumber near gold (same player struct, ±512 bytes)
  3. Monitor both: key rotation = both change by same delta simultaneously
                   player action = only gold or only lumber changes

Usage:
    python3 scripts/find_resources.py          # interactive scan
    python3 scripts/find_resources.py --watch  # watch after finding addresses
"""
import ctypes, ctypes.util, struct, subprocess, sys, time, argparse

TARGET = "Warcraft III"
MAX_REGION = 32 * 1024 * 1024
GOLD_DELTA = -75       # cost of 1 Peon
ENCODE_MIN = 10000     # plausible encode range
ENCODE_MAX = 60000
STRUCT_SCAN = 512      # bytes to scan around gold for lumber

libc = ctypes.CDLL(ctypes.util.find_library("c"))
libc.task_for_pid.restype = ctypes.c_int
libc.task_for_pid.argtypes = [ctypes.c_uint, ctypes.c_int, ctypes.POINTER(ctypes.c_uint)]
libc.mach_task_self.restype = ctypes.c_uint
libc.mach_task_self.argtypes = []
libc.mach_vm_read_overwrite.restype = ctypes.c_int
libc.mach_vm_read_overwrite.argtypes = [ctypes.c_uint, ctypes.c_uint64, ctypes.c_uint64,
    ctypes.c_uint64, ctypes.POINTER(ctypes.c_uint64)]

class _Info(ctypes.Structure):
    _fields_ = [("protection",ctypes.c_int),("max_protection",ctypes.c_int),
                ("inheritance",ctypes.c_uint),("shared",ctypes.c_int),
                ("reserved",ctypes.c_int),("offset",ctypes.c_uint64),
                ("behavior",ctypes.c_int),("user_wired_count",ctypes.c_ushort)]

libc.mach_vm_region.restype = ctypes.c_int
libc.mach_vm_region.argtypes = [ctypes.c_uint, ctypes.POINTER(ctypes.c_uint64),
    ctypes.POINTER(ctypes.c_uint64), ctypes.c_int, ctypes.POINTER(_Info),
    ctypes.POINTER(ctypes.c_uint), ctypes.POINTER(ctypes.c_uint)]


def get_task(pid: int) -> int:
    t = ctypes.c_uint(0)
    assert libc.task_for_pid(libc.mach_task_self(), pid, ctypes.byref(t)) == 0
    return t.value


def read_mem(task: int, addr: int, size: int) -> bytes | None:
    buf = (ctypes.c_char * size)()
    out = ctypes.c_uint64(0)
    kr = libc.mach_vm_read_overwrite(task, addr, size, ctypes.addressof(buf), ctypes.byref(out))
    return bytes(buf[:out.value]) if kr == 0 else None


def read_i32(task: int, addr: int) -> int | None:
    d = read_mem(task, addr, 4)
    return struct.unpack("<i", d)[0] if d and len(d) >= 4 else None


def snapshot(task: int) -> list[tuple[int, bytes]]:
    """Snapshot all small RW regions."""
    regions = []
    addr = ctypes.c_uint64(1)
    while True:
        size = ctypes.c_uint64(0)
        info = _Info(); cnt = ctypes.c_uint(9); obj = ctypes.c_uint(0)
        kr = libc.mach_vm_region(task, ctypes.byref(addr), ctypes.byref(size), 9,
                                  ctypes.byref(info), ctypes.byref(cnt), ctypes.byref(obj))
        if kr != 0: break
        if (info.protection & 3) == 3 and 0 < size.value <= MAX_REGION:
            off, data = 0, b""
            while off < size.value:
                chunk = read_mem(task, addr.value + off, min(4*1024*1024, size.value - off))
                data += chunk if chunk else b"\x00" * min(4*1024*1024, size.value - off)
                off += 4*1024*1024
            regions.append((addr.value, data))
        addr.value += size.value
    return regions


def find_delta(snap_a, snap_b, delta: int) -> list[tuple[int, int, int]]:
    """Find addresses where value changed by exactly delta. Returns (addr, val_a, val_b)."""
    b_map = {base: data for base, data in snap_b}
    hits = []
    for base, data_a in snap_a:
        data_b = b_map.get(base)
        if data_b is None or len(data_b) != len(data_a):
            continue
        align = (4 - base % 4) % 4
        i = align
        while i + 4 <= len(data_a):
            va = struct.unpack_from("<i", data_a, i)[0]
            vb = struct.unpack_from("<i", data_b, i)[0]
            if vb - va == delta:
                hits.append((base + i, va, vb))
            i += 4
    return hits


def find_lumber_near(task: int, gold_addr: int, gold_enc: int,
                     actual_lumber: int) -> tuple[int, int] | None:
    """Scan ±STRUCT_SCAN bytes around gold_addr for lumber."""
    start = max(0, gold_addr - STRUCT_SCAN)
    data = read_mem(task, start, STRUCT_SCAN * 2)
    if not data:
        return None
    for i in range(0, len(data) - 3, 4):
        v = struct.unpack_from("<i", data, i)[0]
        enc = v - actual_lumber
        if ENCODE_MIN <= enc <= ENCODE_MAX:
            return (start + i, enc)
    return None


def watch(task: int, gold_addr: int, gold_enc: int,
          lumber_addr: int, lumber_enc: int, duration: int = 0):
    """Watch resources. Distinguish key rotation from actual changes."""
    print(f"\nWatching resources (Ctrl+C to stop) ...")
    print(f"  gold   @ 0x{gold_addr:x}  enc={gold_enc}")
    print(f"  lumber @ 0x{lumber_addr:x}  enc={lumber_enc}")
    print()

    last_g = read_i32(task, gold_addr)
    last_l = read_i32(task, lumber_addr)
    key_enc = gold_enc

    actual_g = last_g - key_enc
    actual_l = last_l - lumber_enc
    print(f"  initial: gold={actual_g}  lumber={actual_l}")

    start = time.time()
    last_print = 0.0
    try:
        while True:
            time.sleep(0.1)
            g_raw = read_i32(task, gold_addr)
            l_raw = read_i32(task, lumber_addr)
            if g_raw is None or l_raw is None:
                print("ERROR: lost process", file=sys.stderr)
                break

            dg = g_raw - last_g
            dl = l_raw - last_l
            now = time.time()

            if dg != 0 or dl != 0:
                if dg == dl and abs(dg) > 200:
                    key_enc += dg
                    print(f"  KEY ROTATION Δ={dg:+d}")
                actual_g = g_raw - key_enc
                actual_l = l_raw - lumber_enc
                print(f"  [{now - start:5.1f}s] gold={actual_g:>6}  lumber={actual_l:>5}")
                last_print = now
            elif now - last_print >= 2.0:
                actual_g = g_raw - key_enc
                actual_l = l_raw - lumber_enc
                print(f"  [{now - start:5.1f}s] gold={actual_g:>6}  lumber={actual_l:>5}  (no change)")
                last_print = now

            last_g, last_l = g_raw, l_raw
            if duration and (now - start) >= duration:
                break
    except KeyboardInterrupt:
        print("\nStopped.")


def main():
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--watch", action="store_true")
    ap.add_argument("--gold-addr",   type=lambda x: int(x, 16))
    ap.add_argument("--gold-enc",    type=int)
    ap.add_argument("--lumber-addr", type=lambda x: int(x, 16))
    ap.add_argument("--lumber-enc",  type=int)
    args = ap.parse_args()

    result = subprocess.run(["pgrep", "-x", TARGET], capture_output=True, text=True)
    if result.returncode != 0:
        print(f"error: '{TARGET}' not running", file=sys.stderr); return 1
    pid = int(result.stdout.strip().split("\n")[0])
    task = get_task(pid)
    print(f"Attached to {TARGET} PID={pid}")

    # If addresses provided, go straight to watch
    if args.watch and args.gold_addr:
        watch(task, args.gold_addr, args.gold_enc,
              args.lumber_addr, args.lumber_enc)
        return 0

    # ── Interactive scan ──────────────────────────────────────────────────────
    print("\nStep 1: Be IN a game with HUD visible.")
    print("        Miners don't matter — we scan fast.")
    input("Press Enter to take snapshot A ...")
    t0 = time.time()
    snap_a = snapshot(task)
    print(f"  snapshot A: {len(snap_a)} regions  ({time.time()-t0:.1f}s)")

    print("\nStep 2: Buy exactly 1 Peon RIGHT NOW, then press Enter.")
    input("Press Enter immediately after the purchase ...")
    t0 = time.time()
    snap_b = snapshot(task)
    print(f"  snapshot B: {len(snap_b)} regions  ({time.time()-t0:.1f}s)")

    print("\nFinding gold address ...")
    all_hits = find_delta(snap_a, snap_b, GOLD_DELTA)

    # Filter: heap range + plausible encode constant
    heap = [(a, va, vb) for a, va, vb in all_hits
            if ENCODE_MIN <= (va - 0) and 0x100000000 <= a <= 0x80000000000
            and ENCODE_MIN <= -(vb - va - GOLD_DELTA + va) or True]

    # Simpler filter: just reasonable encode (val_a is gold_before + encode_const)
    # We don't know gold_before, but encode_const should be ENCODE_MIN..ENCODE_MAX
    good = [(a, va, vb) for a, va, vb in all_hits if 0x100000000 <= a]
    print(f"  {len(all_hits)} total hits, {len(good)} with address > 1GB")

    if not good:
        print("ERROR: no candidates. Try again — buy peon faster after Enter.", file=sys.stderr)
        return 2

    # Show all candidates
    print("\nCandidates (va=stored_before, vb=stored_after):")
    for a, va, vb in good[:15]:
        print(f"  0x{a:016x}  stored: {va} → {vb}  encode_guess={va - 500:+d} (if gold was ~500)")

    raw = input("\nWhat is your gold RIGHT NOW (read from HUD)? ").strip()
    gold_now = int(raw)

    # Find which candidate is consistent with gold_now
    # Check: val_b (stored after peon) - gold_now ≈ encode (stable constant)
    # Also verify: val_a - encode ≈ gold_before = gold_now + 75
    gold_addr, gold_enc = None, None
    for a, va, vb in good:
        current = read_i32(task, a)
        if current is None: continue
        enc = vb - gold_now          # derive encode from snapshot-B value
        gold_before_check = va - enc # should be gold_now + 75
        if abs(gold_before_check - (gold_now + 75)) <= 10:  # allow ±10 for mining noise
            gold_addr, gold_enc = a, enc
            print(f"\nGold found: 0x{a:016x}  encode={enc}  gold_now={gold_now}")
            break

    if gold_addr is None:
        # Show all candidates with derived encode so user can pick
        print("\nAuto-match failed. Candidates with derived encode:")
        for a, va, vb in good:
            enc = vb - gold_now
            print(f"  0x{a:016x}  encode={enc}  gold_before_check={va-enc} (expected {gold_now+75})")
        pick = input("Enter address to use (hex) or blank to abort: ").strip()
        if not pick: return 2
        gold_addr = int(pick, 16)
        gold_enc = next(vb - gold_now for a, va, vb in good if a == gold_addr)
        print(f"Using: 0x{gold_addr:016x}  encode={gold_enc}")

    raw_l = input("What is your lumber RIGHT NOW (read from HUD)? ").strip()
    lumber_now = int(raw_l)

    result = find_lumber_near(task, gold_addr, gold_enc, lumber_now)
    if result:
        lumber_addr, lumber_enc = result
        print(f"Lumber found: 0x{lumber_addr:016x}  encode={lumber_enc}")
    else:
        print("Lumber not found near gold. Continuing with gold only.")
        lumber_addr, lumber_enc = gold_addr, gold_enc

    print(f"\nRun watcher:")
    print(f"  python3 scripts/find_resources.py --watch "
          f"--gold-addr {gold_addr:x} --gold-enc {gold_enc} "
          f"--lumber-addr {lumber_addr:x} --lumber-enc {lumber_enc}")

    watch(task, gold_addr, gold_enc, lumber_addr, lumber_enc)


if __name__ == "__main__":
    sys.exit(main())
