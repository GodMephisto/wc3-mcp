---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/test_input.py"
ref: HEAD
---

#!/usr/bin/env python3
"""test_input.py — CGEvent keyboard + mouse PoC for WC3.

Sends keystrokes / mouse clicks to Warcraft III via macOS CoreGraphics.
Uses CGEventPostToPid so WC3 does NOT need to be the focused window.

Usage
─────
    python3 scripts/test_input.py              # send F1 once (select hero)
    python3 scripts/test_input.py --key P      # send 'P' (train peon hotkey)
    python3 scripts/test_input.py --key ENTER  # confirm dialog
    python3 scripts/test_input.py --mouse --x 960 --y 540   # left-click
    python3 scripts/test_input.py --key F1 --count 3 --interval 1.0

Permissions
───────────
CGEventPostToPid requires either:
  • The calling process has Accessibility access (System Settings → Privacy
    & Security → Accessibility → add Terminal / iTerm), OR
  • Running as root (sudo python3 scripts/test_input.py).
"""
import argparse, ctypes, subprocess, sys, time

TARGET = "Warcraft III"

# ── CoreGraphics / CoreFoundation ─────────────────────────────────────────────

_CG = ctypes.CDLL(
    "/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics"
)
_CF = ctypes.CDLL(
    "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation"
)

# Basic types
_CGEventRef       = ctypes.c_void_p
_CGEventSourceRef = ctypes.c_void_p
_CGKeyCode        = ctypes.c_uint16
_CGFloat          = ctypes.c_double
_pid_t            = ctypes.c_int32

class _CGPoint(ctypes.Structure):
    _fields_ = [("x", _CGFloat), ("y", _CGFloat)]

# CGEventCreateKeyboardEvent(source, virtualKey, keyDown) → CGEventRef
_CG.CGEventCreateKeyboardEvent.restype  = _CGEventRef
_CG.CGEventCreateKeyboardEvent.argtypes = [_CGEventSourceRef, _CGKeyCode,
                                            ctypes.c_bool]

# CGEventCreateMouseEvent(source, mouseType, pt, mouseButton) → CGEventRef
_CG.CGEventCreateMouseEvent.restype  = _CGEventRef
_CG.CGEventCreateMouseEvent.argtypes = [_CGEventSourceRef, ctypes.c_uint32,
                                         _CGPoint, ctypes.c_uint32]

# CGEventPostToPid(pid, event)  — sends to app regardless of focus
_CG.CGEventPostToPid.restype  = None
_CG.CGEventPostToPid.argtypes = [_pid_t, _CGEventRef]

# CGEventPost(tap, event)  — sends into global HID stream (needs focus)
_CG.CGEventPost.restype  = None
_CG.CGEventPost.argtypes = [ctypes.c_uint32, _CGEventRef]

# CFRelease
_CF.CFRelease.restype  = None
_CF.CFRelease.argtypes = [ctypes.c_void_p]

# Constants
kCGHIDEventTap        = 0
kCGEventLeftMouseDown = 1
kCGEventLeftMouseUp   = 2
kCGMouseButtonLeft    = 0

# ── macOS virtual-key codes (HIToolbox/Events.h) ──────────────────────────────
# These are hardware scan-codes, NOT ASCII / Unicode values.

VK: dict[str, int] = {
    # Letters
    "A": 0x00, "S": 0x01, "D": 0x02, "F": 0x03, "H": 0x04, "G": 0x05,
    "Z": 0x06, "X": 0x07, "C": 0x08, "V": 0x09, "B": 0x0B, "Q": 0x0C,
    "W": 0x0D, "E": 0x0E, "R": 0x0F, "Y": 0x10, "T": 0x11, "O": 0x1F,
    "U": 0x20, "I": 0x22, "P": 0x23, "L": 0x25, "J": 0x26, "K": 0x28,
    "N": 0x2D, "M": 0x2E,
    # Digits (top row)
    "1": 0x12, "2": 0x13, "3": 0x14, "4": 0x15, "6": 0x16, "5": 0x17,
    "9": 0x19, "7": 0x1A, "8": 0x1C, "0": 0x1D,
    # Special
    "ENTER":  0x24, "TAB":    0x30, "SPACE":  0x31, "BACKSPACE": 0x33,
    "ESCAPE": 0x35, "DELETE": 0x75,
    # Function keys
    "F1":  0x7A, "F2":  0x78, "F3":  0x63, "F4":  0x76,
    "F5":  0x60, "F6":  0x61, "F7":  0x62, "F8":  0x64,
    "F9":  0x65, "F10": 0x6D, "F11": 0x67, "F12": 0x6F,
}


# ── helpers ───────────────────────────────────────────────────────────────────

def get_pid(name: str) -> int:
    r = subprocess.run(["pgrep", "-x", name], capture_output=True, text=True)
    if r.returncode != 0:
        raise RuntimeError(f"'{name}' not running")
    return int(r.stdout.strip().split("\n")[0])


def _post(pid: int, ev) -> None:
    """Post event to WC3 by PID (no focus required)."""
    _CG.CGEventPostToPid(pid, ev)
    _CF.CFRelease(ev)


def send_key(pid: int, vk: int, hold: float = 0.05) -> None:
    """Key-down + key-up with a `hold` second gap between them."""
    _post(pid, _CG.CGEventCreateKeyboardEvent(None, vk, True))
    time.sleep(hold)
    _post(pid, _CG.CGEventCreateKeyboardEvent(None, vk, False))


def send_click(pid: int, x: float, y: float, hold: float = 0.05) -> None:
    """Left mouse button down + up at screen coords (x, y)."""
    pt = _CGPoint(x, y)
    _post(pid, _CG.CGEventCreateMouseEvent(
        None, kCGEventLeftMouseDown, pt, kCGMouseButtonLeft))
    time.sleep(hold)
    _post(pid, _CG.CGEventCreateMouseEvent(
        None, kCGEventLeftMouseUp, pt, kCGMouseButtonLeft))


# ── main ─────────────────────────────────────────────────────────────────────

def main() -> int:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--key",      default="F1",  help="Key name (default: F1)")
    ap.add_argument("--mouse",    action="store_true", help="Send mouse click instead of key")
    ap.add_argument("--x",        type=float, default=960, help="Click X (screen px)")
    ap.add_argument("--y",        type=float, default=540, help="Click Y (screen px)")
    ap.add_argument("--count",    type=int,   default=1,   help="Repetitions")
    ap.add_argument("--interval", type=float, default=1.0, help="Seconds between reps")
    args = ap.parse_args()

    pid = get_pid(TARGET)
    print(f"Attached to {TARGET}  PID={pid}")

    if args.mouse:
        print(f"Sending {args.count}× left-click at ({args.x:.0f}, {args.y:.0f})")
        for i in range(args.count):
            send_click(pid, args.x, args.y)
            print(f"  click {i + 1} sent", flush=True)
            if i < args.count - 1:
                time.sleep(args.interval)
    else:
        key = args.key.upper()
        if key not in VK:
            print(f"Unknown key '{key}'. Known: {', '.join(sorted(VK))}",
                  file=sys.stderr)
            return 1
        vk = VK[key]
        print(f"Sending {args.count}× '{key}' (vk=0x{vk:02x})")
        for i in range(args.count):
            send_key(pid, vk)
            print(f"  key {i + 1} sent", flush=True)
            if i < args.count - 1:
                time.sleep(args.interval)

    print("Done — check WC3 for response.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
