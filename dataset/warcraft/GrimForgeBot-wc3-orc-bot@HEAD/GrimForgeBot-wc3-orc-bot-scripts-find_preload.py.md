---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/find_preload.py"
ref: HEAD
---

#!/usr/bin/env python3
"""find_preload.py — Locate WC3's Preload() C function via luaL_Reg table scan.

luaL_Reg is an array of {const char* name, lua_CFunction func} pairs.
On x86-64 macOS each entry is 16 bytes: 8-byte name ptr + 8-byte func ptr.

Strategy:
  1. Find all "Preload\0" strings in WC3 memory.
  2. Scan ALL readable RW sections for 8-byte pointers to those strings.
  3. For each hit: the 8 bytes immediately after = Preload() address.
  4. Verify the candidate is in an executable region.
  5. Disassemble first ~32 bytes to find the first CALL → lua_tolstring.

Usage:
    python3 scripts/find_preload.py
"""
import ctypes, ctypes.util, struct, subprocess, sys

TARGET = "Warcraft III"

libc = ctypes.CDLL(ctypes.util.find_library("c"))
libc.task_for_pid.restype  = ctypes.c_int
libc.task_for_pid.argtypes = [ctypes.c_uint, ctypes.c_int, ctypes.POINTER(ctypes.c_uint)]
libc.mach_task_self.restype  = ctypes.c_uint
libc.mach_task_self.argtypes = []
libc.mach_vm_read_overwrite.restype  = ctypes.c_int
libc.mach_vm_read_overwrite.argtypes = [ctypes.c_uint, ctypes.c_uint64, ctypes.c_uint64,
    ctypes.c_uint64, ctypes.POINTER(ctypes.c_uint64)]

class _Info(ctypes.Structure):
    _fields_ = [
        ("protection",      ctypes.c_int),
        ("max_protection",  ctypes.c_int),
        ("inheritance",     ctypes.c_uint),
        ("shared",          ctypes.c_int),
        ("reserved",        ctypes.c_int),
        ("offset",          ctypes.c_uint64),
        ("behavior",        ctypes.c_int),
        ("user_wired_count",ctypes.c_ushort),
    ]

libc.mach_vm_region.restype  = ctypes.c_int
libc.mach_vm_region.argtypes = [ctypes.c_uint, ctypes.POINTER(ctypes.c_uint64),
    ctypes.POINTER(ctypes.c_uint64), ctypes.c_int, ctypes.POINTER(_Info),
    ctypes.POINTER(ctypes.c_uint), ctypes.POINTER(ctypes.c_uint)]


def get_task(pid: int) -> int:
    t = ctypes.c_uint(0)
    r = libc.task_for_pid(libc.mach_task_self(), pid, ctypes.byref(t))
    assert r == 0, f"task_for_pid failed: {r}"
    return t.value


def read_mem(task: int, addr: int, size: int) -> bytes | None:
    if size <= 0:
        return b""
    buf = (ctypes.c_char * size)()
    out = ctypes.c_uint64(0)
    kr  = libc.mach_vm_read_overwrite(task, addr, size, ctypes.addressof(buf), ctypes.byref(out))
    return bytes(buf[:out.value]) if kr == 0 else None


def iter_regions(task: int):
    """Yield (base, size, protection) for every mapped region."""
    addr = ctypes.c_uint64(1)
    while True:
        size = ctypes.c_uint64(0)
        info = _Info(); cnt = ctypes.c_uint(9); obj = ctypes.c_uint(0)
        kr = libc.mach_vm_region(task, ctypes.byref(addr), ctypes.byref(size), 9,
                                  ctypes.byref(info), ctypes.byref(cnt), ctypes.byref(obj))
        if kr != 0:
            break
        yield addr.value, size.value, info.protection
        addr.value += size.value


def read_region(task: int, base: int, size: int) -> bytes:
    CHUNK = 4 * 1024 * 1024
    data = b""
    off  = 0
    while off < size:
        chunk = read_mem(task, base + off, min(CHUNK, size - off))
        data += chunk if chunk else b"\x00" * min(CHUNK, size - off)
        off  += CHUNK
    return data


def find_string_hits(task: int, needle: bytes) -> list[int]:
    """Find all occurrences of needle in any readable region."""
    hits = []
    for base, size, prot in iter_regions(task):
        if not (prot & 1):  # not readable
            continue
        if size > 256 * 1024 * 1024:
            continue
        data = read_region(task, base, size)
        off = 0
        while True:
            idx = data.find(needle, off)
            if idx == -1:
                break
            hits.append(base + idx)
            off = idx + 1
    return hits


def find_ptr_to(task: int, targets: list[int]) -> list[tuple[int, int]]:
    """
    Search ALL readable+writable regions for 8-byte little-endian pointers
    that point to any address in `targets`.
    Returns list of (ptr_addr, target_addr).
    """
    target_set = set(targets)
    results = []
    for base, size, prot in iter_regions(task):
        # Must be readable and writable (luaL_Reg is in data, not rodata)
        if (prot & 3) != 3:
            continue
        if size > 128 * 1024 * 1024:
            continue
        data = read_region(task, base, size)
        # Align to 8 bytes
        align = (8 - base % 8) % 8
        i = align
        while i + 8 <= len(data):
            v = struct.unpack_from("<Q", data, i)[0]
            if v in target_set:
                results.append((base + i, v))
            i += 8
    return results


def find_exec_region(task: int, addr: int) -> bool:
    """Return True if addr is in an executable region."""
    for base, size, prot in iter_regions(task):
        if base <= addr < base + size:
            return bool(prot & 4)
    return False


def disasm_first_call(task: int, func_addr: int, scan_len: int = 64) -> int | None:
    """
    Scan first scan_len bytes of func_addr for a CALL rel32 (0xE8) or
    CALL r/m64 (0xFF /2) and return its target.
    Very naive — good enough for a short prologue.
    """
    data = read_mem(task, func_addr, scan_len)
    if not data:
        return None
    i = 0
    while i < len(data):
        b = data[i]
        if b == 0xE8 and i + 5 <= len(data):
            rel = struct.unpack_from("<i", data, i + 1)[0]
            target = (func_addr + i + 5 + rel) & 0xFFFFFFFFFFFFFFFF
            return target
        # FF D0 = CALL rax, FF D1 = CALL rcx, etc. — indirect, skip
        # FF 15 = CALL [rip+disp32]
        if b == 0xFF and i + 6 <= len(data) and data[i + 1] == 0x15:
            rel = struct.unpack_from("<i", data, i + 2)[0]
            slot = (func_addr + i + 6 + rel) & 0xFFFFFFFFFFFFFFFF
            ptr  = read_mem(task, slot, 8)
            if ptr and len(ptr) == 8:
                target = struct.unpack("<Q", ptr)[0]
                return target
            i += 6
            continue
        i += 1
    return None


def main():
    result = subprocess.run(["pgrep", "-x", TARGET], capture_output=True, text=True)
    if result.returncode != 0:
        print(f"error: '{TARGET}' not running", file=sys.stderr)
        return 1
    pid  = int(result.stdout.strip().split("\n")[0])
    task = get_task(pid)
    print(f"Attached to {TARGET}  PID={pid}")

    # ── Step 1: find "Preload\0" strings ──────────────────────────────────────
    needle = b"Preload\x00"
    print(f"\nSearching for {needle!r} in all readable regions ...")
    str_hits = find_string_hits(task, needle)
    if not str_hits:
        print("ERROR: 'Preload\\0' not found in memory. Is WC3 running?")
        return 2
    print(f"  {len(str_hits)} hit(s):")
    for h in str_hits:
        ctx = read_mem(task, h, 24) or b""
        print(f"    0x{h:016x}  {ctx!r}")

    # ── Step 2: find pointers to those strings in RW memory ───────────────────
    print(f"\nSearching RW regions for 8-byte pointers to those strings ...")
    ptr_hits = find_ptr_to(task, str_hits)
    if not ptr_hits:
        print("  No pointers found in RW regions.")
        print("  Trying ALL readable regions as fallback ...")
        # fallback: search everything readable
        ptr_hits_all = []
        for base, size, prot in iter_regions(task):
            if not (prot & 1) or size > 128 * 1024 * 1024:
                continue
            data = read_region(task, base, size)
            align = (8 - base % 8) % 8
            i = align
            while i + 8 <= len(data):
                v = struct.unpack_from("<Q", data, i)[0]
                if v in set(str_hits):
                    ptr_hits_all.append((base + i, v))
                i += 8
        if ptr_hits_all:
            print(f"  Found {len(ptr_hits_all)} pointer(s) in readable regions:")
            ptr_hits = ptr_hits_all
        else:
            print("  Still nothing. Something is wrong.")
            return 2

    print(f"  {len(ptr_hits)} pointer(s) found.")

    # ── Step 3: read the 8 bytes after each pointer → func ptr ────────────────
    print("\nLooking for function pointer in luaL_Reg entry (ptr+8) ...")
    candidates = []
    for ptr_addr, str_addr in ptr_hits:
        func_data = read_mem(task, ptr_addr + 8, 8)
        if not func_data or len(func_data) < 8:
            continue
        func_ptr = struct.unpack("<Q", func_data)[0]
        is_exec  = find_exec_region(task, func_ptr)
        print(f"  ptr @ 0x{ptr_addr:016x} → str=0x{str_addr:016x}  "
              f"func=0x{func_ptr:016x}  exec={is_exec}")
        if is_exec and func_ptr > 0x100000000:
            candidates.append((ptr_addr, func_ptr))

    if not candidates:
        print("\nNo executable function candidates found.")
        print("Dumping all pointer hits for manual inspection:")
        for ptr_addr, str_addr in ptr_hits:
            func_data = read_mem(task, ptr_addr + 8, 8)
            func_ptr  = struct.unpack("<Q", func_data)[0] if func_data and len(func_data) >= 8 else 0
            print(f"  0x{ptr_addr:016x}  str=0x{str_addr:016x}  next8=0x{func_ptr:016x}")
        return 2

    # ── Step 4: for each candidate, find first CALL = lua_tolstring ───────────
    print(f"\n{'='*60}")
    print(f"CANDIDATES ({len(candidates)}):")
    for ptr_addr, func_ptr in candidates:
        print(f"\n  Preload() @ 0x{func_ptr:016x}")
        first_bytes = read_mem(task, func_ptr, 16) or b""
        print(f"  First bytes: {first_bytes.hex()}")
        first_call = disasm_first_call(task, func_ptr)
        if first_call:
            print(f"  First CALL  → 0x{first_call:016x}  (likely lua_tolstring)")
        else:
            print(f"  First CALL  → not found in first 64 bytes")

    best_ptr, best_func = candidates[0]
    first_call = disasm_first_call(task, best_func)

    print(f"\n{'='*60}")
    print(f"RESULT:")
    print(f"  PRELOAD_ADDR     = 0x{best_func:016x}")
    if first_call:
        print(f"  LUA_TOLSTRING    = 0x{first_call:016x}")
    print()
    print("Pass these to inject_hook.py:")
    cmd = f"  python3 scripts/inject_hook.py --preload 0x{best_func:x}"
    if first_call:
        cmd += f" --lua-tolstring 0x{first_call:x}"
    print(cmd)


if __name__ == "__main__":
    sys.exit(main())
