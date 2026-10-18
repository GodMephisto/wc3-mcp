---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/hid_keyboard.py"
ref: HEAD
---

#!/usr/bin/env python3
"""hid_keyboard.py — Virtual HID keyboard for WC3 input injection.

Creates a virtual USB keyboard at the IOKit HID level.  SDL (which WC3
uses for input) enumerates IOKit HID devices, so it sees this as a real
keyboard and processes its reports — unlike CGEvent which SDL ignores.

Must run as root (sudo) for IOHIDUserDeviceCreate to succeed.

Usage
─────
    sudo python3 scripts/hid_keyboard.py              # test: types Enter
    sudo python3 scripts/hid_keyboard.py --key ENTER
    sudo python3 scripts/hid_keyboard.py --key F1
    sudo python3 scripts/hid_keyboard.py --key A
    sudo python3 scripts/hid_keyboard.py --chat hello  # open chat + type + send

HID usage codes differ from macOS VK codes — see HID_USAGE table below.
"""
import argparse, ctypes, ctypes.util, sys, time

# ── IOKit / CoreFoundation via ctypes ─────────────────────────────────────────

_IOKit = ctypes.CDLL("/System/Library/Frameworks/IOKit.framework/IOKit")
_CF    = ctypes.CDLL("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")

# CoreFoundation helpers
_CF.CFStringCreateWithCString.restype  = ctypes.c_void_p
_CF.CFStringCreateWithCString.argtypes = [ctypes.c_void_p, ctypes.c_char_p, ctypes.c_uint32]
_CF.CFDataCreate.restype  = ctypes.c_void_p
_CF.CFDataCreate.argtypes = [ctypes.c_void_p, ctypes.c_char_p, ctypes.c_long]
_CF.CFNumberCreate.restype  = ctypes.c_void_p
_CF.CFNumberCreate.argtypes = [ctypes.c_void_p, ctypes.c_int32, ctypes.c_void_p]
_CF.CFDictionaryCreate.restype  = ctypes.c_void_p
_CF.CFDictionaryCreate.argtypes = [
    ctypes.c_void_p, ctypes.POINTER(ctypes.c_void_p),
    ctypes.POINTER(ctypes.c_void_p), ctypes.c_long,
    ctypes.c_void_p, ctypes.c_void_p,
]
_CF.CFRelease.restype  = None
_CF.CFRelease.argtypes = [ctypes.c_void_p]
_CF.CFRunLoopGetCurrent.restype  = ctypes.c_void_p
_CF.CFRunLoopGetCurrent.argtypes = []
_CF.CFRunLoopRunInMode.restype  = ctypes.c_int32
_CF.CFRunLoopRunInMode.argtypes = [ctypes.c_void_p, ctypes.c_double, ctypes.c_bool]

# Pointer to kCFRunLoopDefaultMode string constant
_kCFRunLoopDefaultMode = ctypes.c_void_p.in_dll(_CF, "kCFRunLoopDefaultMode")

# kCFTypeDictionaryKeyCallBacks / ValueCallBacks (extern structs — pass by pointer)
_kCFTypeDictKCB = ctypes.c_void_p.in_dll(_CF, "kCFTypeDictionaryKeyCallBacks")
_kCFTypeDictVCB = ctypes.c_void_p.in_dll(_CF, "kCFTypeDictionaryValueCallBacks")

# IOHIDUserDevice
_IOKit.IOHIDUserDeviceCreate.restype  = ctypes.c_void_p
_IOKit.IOHIDUserDeviceCreate.argtypes = [ctypes.c_void_p, ctypes.c_void_p]
_IOKit.IOHIDUserDeviceScheduleWithRunLoop.restype  = None
_IOKit.IOHIDUserDeviceScheduleWithRunLoop.argtypes = [
    ctypes.c_void_p, ctypes.c_void_p, ctypes.c_void_p,
]
_IOKit.IOHIDUserDeviceHandleReport.restype  = ctypes.c_int   # IOReturn
_IOKit.IOHIDUserDeviceHandleReport.argtypes = [
    ctypes.c_void_p, ctypes.c_char_p, ctypes.c_long,
]

kCFNumberSInt32Type = 3
kCFStringEncodingUTF8 = 0x08000100

# ── HID report descriptor — standard 101-key USB keyboard ────────────────────

_DESCRIPTOR = bytes([
    0x05, 0x01,        # Usage Page (Generic Desktop)
    0x09, 0x06,        # Usage (Keyboard)
    0xa1, 0x01,        # Collection (Application)
    # Modifier keys (8 bits)
    0x05, 0x07,        #   Usage Page (Key Codes)
    0x19, 0xe0,        #   Usage Minimum (224 = Left Ctrl)
    0x29, 0xe7,        #   Usage Maximum (231 = Right GUI)
    0x15, 0x00,        #   Logical Minimum (0)
    0x25, 0x01,        #   Logical Maximum (1)
    0x75, 0x01,        #   Report Size (1 bit)
    0x95, 0x08,        #   Report Count (8)
    0x81, 0x02,        #   Input (Data, Variable, Absolute)
    # Reserved byte
    0x95, 0x01,        #   Report Count (1)
    0x75, 0x08,        #   Report Size (8 bits)
    0x81, 0x01,        #   Input (Constant)
    # LED output (5 bits + 3 padding)
    0x95, 0x05,        #   Report Count (5)
    0x75, 0x01,        #   Report Size (1)
    0x05, 0x08,        #   Usage Page (LEDs)
    0x19, 0x01,        #   Usage Minimum (1)
    0x29, 0x05,        #   Usage Maximum (5)
    0x91, 0x02,        #   Output (Data, Variable, Absolute)
    0x95, 0x01,        #   Report Count (1)
    0x75, 0x03,        #   Report Size (3)
    0x91, 0x01,        #   Output (Constant)
    # Key array (6 simultaneous keys)
    0x95, 0x06,        #   Report Count (6)
    0x75, 0x08,        #   Report Size (8)
    0x15, 0x00,        #   Logical Minimum (0)
    0x25, 0x65,        #   Logical Maximum (101)
    0x05, 0x07,        #   Usage Page (Key Codes)
    0x19, 0x00,        #   Usage Minimum (0)
    0x29, 0x65,        #   Usage Maximum (101)
    0x81, 0x00,        #   Input (Data, Array)
    0xc0,              # End Collection
])

# ── HID usage codes (USB HID spec, NOT macOS VK codes) ───────────────────────

HID: dict[str, int] = {
    # Letters
    "A": 0x04, "B": 0x05, "C": 0x06, "D": 0x07, "E": 0x08, "F": 0x09,
    "G": 0x0A, "H": 0x0B, "I": 0x0C, "J": 0x0D, "K": 0x0E, "L": 0x0F,
    "M": 0x10, "N": 0x11, "O": 0x12, "P": 0x13, "Q": 0x14, "R": 0x15,
    "S": 0x16, "T": 0x17, "U": 0x18, "V": 0x19, "W": 0x1A, "X": 0x1B,
    "Y": 0x1C, "Z": 0x1D,
    # Digits (top row)
    "1": 0x1E, "2": 0x1F, "3": 0x20, "4": 0x21, "5": 0x22,
    "6": 0x23, "7": 0x24, "8": 0x25, "9": 0x26, "0": 0x27,
    # Special
    "ENTER":     0x28, "ESCAPE":    0x29, "BACKSPACE": 0x2A,
    "TAB":       0x2B, "SPACE":     0x2C, "DELETE":    0x4C,
    # Function keys
    "F1":  0x3A, "F2":  0x3B, "F3":  0x3C, "F4":  0x3D,
    "F5":  0x3E, "F6":  0x3F, "F7":  0x40, "F8":  0x41,
    "F9":  0x42, "F10": 0x43, "F11": 0x44, "F12": 0x45,
}


# ── CF helpers ────────────────────────────────────────────────────────────────

def _cfstr(s: str) -> ctypes.c_void_p:
    return _CF.CFStringCreateWithCString(None, s.encode(), kCFStringEncodingUTF8)

def _cfdata(b: bytes) -> ctypes.c_void_p:
    return _CF.CFDataCreate(None, b, len(b))

def _cfnum(n: int) -> ctypes.c_void_p:
    v = ctypes.c_int32(n)
    return _CF.CFNumberCreate(None, kCFNumberSInt32Type, ctypes.byref(v))

def _cfdict(mapping: dict) -> ctypes.c_void_p:
    ks = list(mapping.keys())
    vs = list(mapping.values())
    k_arr = (ctypes.c_void_p * len(ks))(*ks)
    v_arr = (ctypes.c_void_p * len(vs))(*vs)
    return _CF.CFDictionaryCreate(
        None, k_arr, v_arr, len(ks),
        ctypes.byref(_kCFTypeDictKCB),
        ctypes.byref(_kCFTypeDictVCB),
    )


# ── Virtual keyboard device ───────────────────────────────────────────────────

class VirtualKeyboard:
    """IOKit HID virtual keyboard — visible to SDL as a real USB keyboard."""

    def __init__(self):
        # Build device properties dictionary
        props = _cfdict({
            _cfstr("Transport"):         _cfstr("USB"),
            _cfstr("VendorID"):          _cfnum(0x05AC),   # Apple
            _cfstr("ProductID"):         _cfnum(0x0267),
            _cfstr("VersionNumber"):     _cfnum(0x0001),
            _cfstr("Manufacturer"):      _cfstr("WC3Bot"),
            _cfstr("Product"):           _cfstr("WC3Bot Virtual Keyboard"),
            _cfstr("SerialNumber"):      _cfstr("WC3BOT-KB-1"),
            _cfstr("ReportDescriptor"):  _cfdata(_DESCRIPTOR),
        })

        self._dev = _IOKit.IOHIDUserDeviceCreate(None, props)
        _CF.CFRelease(props)

        if not self._dev:
            raise RuntimeError(
                "IOHIDUserDeviceCreate failed — are you running as root (sudo)?"
            )

        # Schedule on the current run loop so the device is active
        rl = _CF.CFRunLoopGetCurrent()
        _IOKit.IOHIDUserDeviceScheduleWithRunLoop(
            self._dev, rl, _kCFRunLoopDefaultMode
        )
        # Spin the run loop briefly so the OS registers the device
        _CF.CFRunLoopRunInMode(_kCFRunLoopDefaultMode, 0.1, False)
        print(f"  Virtual keyboard created (dev={self._dev:#x})", flush=True)

    def _send_report(self, modifiers: int, keycodes: list[int]) -> None:
        """Send an 8-byte HID keyboard report."""
        report = bytes([modifiers, 0x00] + keycodes[:6] + [0] * (6 - len(keycodes)))
        kr = _IOKit.IOHIDUserDeviceHandleReport(self._dev, report, len(report))
        if kr != 0:
            print(f"  [warn] IOHIDUserDeviceHandleReport kr=0x{kr:08x}", flush=True)

    def press(self, hid_code: int, modifiers: int = 0, hold: float = 0.05) -> None:
        """Press and release a single key."""
        self._send_report(modifiers, [hid_code])  # key down
        time.sleep(hold)
        self._send_report(0, [])                  # key up (all released)
        time.sleep(0.02)

    def type_string(self, text: str, delay: float = 0.05) -> None:
        """Type a string character by character (ASCII letters + digits only)."""
        for ch in text:
            code = HID.get(ch.upper())
            if code is None:
                print(f"  [skip] no HID code for {ch!r}")
                continue
            self.press(code, delay)
            time.sleep(delay)


# ── main ─────────────────────────────────────────────────────────────────────

def main() -> int:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--key",  default="ENTER", help="Key name (default: ENTER)")
    ap.add_argument("--chat", metavar="TEXT",  help="Open chat, type TEXT, send")
    ap.add_argument("--count",    type=int,   default=1)
    ap.add_argument("--interval", type=float, default=0.5)
    args = ap.parse_args()

    print("Creating virtual HID keyboard…")
    try:
        kb = VirtualKeyboard()
    except RuntimeError as e:
        print(f"ERROR: {e}", file=sys.stderr)
        return 1

    # Give SDL/WC3 a moment to enumerate the new device
    print("  Waiting 1 s for SDL to detect device…", flush=True)
    time.sleep(1.0)

    if args.chat:
        # Open chat → type message → send
        print(f"  Sending Enter (open chat)…", flush=True)
        kb.press(HID["ENTER"])
        time.sleep(0.15)
        print(f"  Typing {args.chat!r}…", flush=True)
        kb.type_string(args.chat)
        time.sleep(0.1)
        print(f"  Sending Enter (send message)…", flush=True)
        kb.press(HID["ENTER"])
    else:
        key = args.key.upper()
        if key not in HID:
            print(f"Unknown key '{key}'. Known: {', '.join(sorted(HID))}",
                  file=sys.stderr)
            return 1
        hid = HID[key]
        print(f"  Sending {args.count}× '{key}' (HID=0x{hid:02x})…", flush=True)
        for i in range(args.count):
            kb.press(hid)
            print(f"  [{i+1}/{args.count}] sent", flush=True)
            if i < args.count - 1:
                time.sleep(args.interval)

    print("Done — check WC3.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
