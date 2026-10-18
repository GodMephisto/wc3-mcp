---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/diag_scan4.py"
ref: HEAD
---

#!/usr/bin/env python3
"""diag_scan4.py — Read _G.WC3BOT_STATE string directly.

From diag_scan3 Step 5, the global table Node for WC3BOT_STATE is at 0x15f4e6e10:
  i_val.value_ = 0x000000012a832960  (pointer to TString)
  i_val.tt_    = 0x44               (LUA_VSHRSTR = short collectable string)
  i_key.value_ = 0x000000015f4e7360  (pointer to "WC3BOT_STATE" TString)

The TString at 0x12a832960 should contain the game state string.
Short TString header in Lua 5.4 = 24 bytes:
  next (8) + tt+marked+extra+shrlen (4) + hash (4) + hnext (8) = 24
Char data starts at 0x12a832960 + 24 = 0x12a832978.

This script:
  1. Reads 128 bytes at 0x12a832960 and prints them.
  2. Tries offsets +16, +20, +24, +28, +32 to find the string start.
  3. Polls the VALUE pointer address (0x15f4e6e10) every 0.1s to watch the
     string change as the game progresses.
"""
import ctypes, ctypes.util, struct, subprocess, sys, re, time

TARGET = "Warcraft III"

# From Step 5: Node is 32 bytes; i_val starts at Node base.
# Key ptr found at 0x15f4e6e20 → Node base = 0x15f4e6e20 - 16 (TValue size)
NODE_BASE     = 0x000000015f4e6e10   # start of Node (i_val TValue)
VAL_PTR_ADDR  = NODE_BASE            # address of the 8-byte value pointer
VAL_TT_ADDR   = NODE_BASE + 8        # address of the 4-byte type tag
STR_TSTR_ADDR = 0x000000012a832960   # the TString pointed to by the value

libc = ctypes.CDLL(ctypes.util.find_library("c"))
libc.task_for_pid.restype  = ctypes.c_int
libc.task_for_pid.argtypes = [ctypes.c_uint, ctypes.c_int, ctypes.POINTER(ctypes.c_uint)]
libc.mach_task_self.restype  = ctypes.c_uint
libc.mach_task_self.argtypes = []
libc.mach_vm_read_overwrite.restype  = ctypes.c_int
libc.mach_vm_read_overwrite.argtypes = [ctypes.c_uint, ctypes.c_uint64, ctypes.c_uint64,
    ctypes.c_uint64, ctypes.POINTER(ctypes.c_uint64)]

def get_task(pid):
    t = ctypes.c_uint(0)
    assert libc.task_for_pid(libc.mach_task_self(), pid, ctypes.byref(t)) == 0
    return t.value

def read_mem(task, addr, size):
    if size <= 0: return b""
    buf = (ctypes.c_char * size)()
    out = ctypes.c_uint64(0)
    kr  = libc.mach_vm_read_overwrite(task, addr, size, ctypes.addressof(buf), ctypes.byref(out))
    return bytes(buf[:out.value]) if kr == 0 else None

def hexdump(data, base_addr=0, indent="  "):
    for row in range(0, len(data), 16):
        chunk = data[row:row+16]
        hex_part  = " ".join(f"{b:02x}" for b in chunk)
        safe_part = "".join(chr(b) if 32 <= b < 127 else '.' for b in chunk)
        print(f"{indent}0x{base_addr+row:016x}  {hex_part:<48}  {safe_part}")

def read_tstring_data(task, tstr_addr):
    """Try header offsets 16,20,24,28,32 to find printable string data."""
    raw = read_mem(task, tstr_addr, 160)
    if not raw:
        return None, -1
    for hdr in [16, 20, 24, 28, 32]:
        chunk = raw[hdr:hdr+120]
        # Count printable ascii bytes before first null
        chars = []
        for b in chunk:
            if b == 0: break
            if 32 <= b < 127:
                chars.append(chr(b))
            else:
                chars = []  # reset if non-printable hit
                break
        if len(chars) >= 5:
            return "".join(chars), hdr
    # Fallback: find longest printable run
    for hdr in [24, 32, 20, 16, 28]:
        chunk = raw[hdr:hdr+120]
        s = bytes(b if 32 <= b < 127 else 0 for b in chunk)
        parts = [p for p in s.split(b'\x00') if len(p) >= 5]
        if parts:
            return parts[0].decode(), hdr
    return None, -1


def main():
    result = subprocess.run(["pgrep", "-x", TARGET], capture_output=True, text=True)
    if result.returncode != 0:
        print(f"error: '{TARGET}' not running"); return 1
    pid  = int(result.stdout.strip().split("\n")[0])
    task = get_task(pid)
    print(f"Attached to {TARGET}  PID={pid}\n")

    # ── 1. Dump TString at 0x12a832960 ───────────────────────────────────────
    print(f"=== TString @ 0x{STR_TSTR_ADDR:x} ===")
    raw = read_mem(task, STR_TSTR_ADDR, 160)
    if raw:
        hexdump(raw, STR_TSTR_ADDR)
        print()
        for hdr in [16, 20, 24, 28, 32]:
            chunk = raw[hdr:hdr+120]
            safe = bytes(b if 32 <= b < 127 else ord('.') for b in chunk)
            print(f"  offset +{hdr}: {safe[:80]!r}")
    else:
        print("  UNREADABLE")

    # ── 2. Read the live value pointer (watches as game runs) ─────────────────
    print(f"\n=== Reading live Node @ 0x{NODE_BASE:x} ===")
    node_data = read_mem(task, NODE_BASE, 16)
    if node_data and len(node_data) >= 12:
        val_ptr = struct.unpack_from("<Q", node_data, 0)[0]
        val_tt  = struct.unpack_from("<I", node_data, 8)[0]
        print(f"  value ptr = 0x{val_ptr:016x}  tt = 0x{val_tt:02x} ({'string' if val_tt in (0x44,0x54) else 'other'})")
        if val_ptr and val_tt in (0x44, 0x54):
            s, hdr = read_tstring_data(task, val_ptr)
            if s:
                print(f"  String (hdr={hdr}): {s!r}")
            else:
                print(f"  Could not extract string from 0x{val_ptr:x}")
    else:
        print("  Could not read Node")

    # ── 3. Poll loop ──────────────────────────────────────────────────────────
    print(f"\n=== Polling _G.WC3BOT_STATE every 0.1s (Ctrl+C to stop) ===")
    last = None
    sidecar_re = re.compile(r'T:\d+\.?\d*\|G:\d+\|L:\d+\|FU:\d+\|FC:\d+')
    while True:
        try:
            time.sleep(0.1)
            node_data = read_mem(task, NODE_BASE, 16)
            if not node_data or len(node_data) < 12:
                continue
            val_ptr = struct.unpack_from("<Q", node_data, 0)[0]
            val_tt  = struct.unpack_from("<I", node_data, 8)[0]
            if not val_ptr or val_tt not in (0x44, 0x54, 4, 0x04):
                print(f"  value is nil or non-string (tt=0x{val_tt:x})")
                time.sleep(1)
                continue
            s, hdr = read_tstring_data(task, val_ptr)
            if s and s != last:
                m = sidecar_re.search(s)
                if m:
                    print(f"  SIDECAR: {s[:200]}")
                else:
                    print(f"  STRING (hdr={hdr}): {s[:200]!r}")
                last = s
        except KeyboardInterrupt:
            print("\nStopped.")
            break
    return 0


if __name__ == "__main__":
    sys.exit(main())
