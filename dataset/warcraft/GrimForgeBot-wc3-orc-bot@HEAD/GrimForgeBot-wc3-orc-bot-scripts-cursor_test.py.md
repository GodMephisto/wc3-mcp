---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/cursor_test.py"
ref: HEAD
---

#!/usr/bin/env python3
"""cursor_test.py — Move cursor to target coordinates so you can see where it lands.

Does NOT click — just moves the cursor.  Activate WC3 first, hover over GH to
confirm position, then run this to see if the saved coordinate matches.

Usage:
    python3 scripts/cursor_test.py 872 879    # test a specific coordinate
    python3 scripts/cursor_test.py             # test current bot.py defaults
"""
import ctypes, sys, time, subprocess

_CG = ctypes.CDLL("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")
_CF = ctypes.CDLL("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")

class _CGPoint(ctypes.Structure):
    _fields_ = [("x", ctypes.c_double), ("y", ctypes.c_double)]

kCGEventMouseMoved = 5
kCGMouseButtonLeft = 0
kCGHIDEventTap = 0

_CG.CGEventCreateMouseEvent.restype  = ctypes.c_void_p
_CG.CGEventCreateMouseEvent.argtypes = [ctypes.c_void_p, ctypes.c_uint32, _CGPoint, ctypes.c_uint32]
_CG.CGEventPost.restype  = None
_CG.CGEventPost.argtypes = [ctypes.c_uint32, ctypes.c_void_p]
_CF.CFRelease.restype  = None
_CF.CFRelease.argtypes = [ctypes.c_void_p]

_CG.CGEventCreate.restype  = ctypes.c_void_p
_CG.CGEventCreate.argtypes = [ctypes.c_void_p]
_CG.CGEventGetLocation.restype  = _CGPoint
_CG.CGEventGetLocation.argtypes = [ctypes.c_void_p]

def get_mouse():
    ev = _CG.CGEventCreate(None)
    pt = _CG.CGEventGetLocation(ev)
    _CF.CFRelease(ev)
    return int(pt.x), int(pt.y)

def move_cursor(x, y):
    pt = _CGPoint(x, y)
    ev = _CG.CGEventCreateMouseEvent(None, kCGEventMouseMoved, pt, kCGMouseButtonLeft)
    _CG.CGEventPost(kCGHIDEventTap, ev)
    _CF.CFRelease(ev)

if len(sys.argv) == 3:
    tx, ty = int(sys.argv[1]), int(sys.argv[2])
else:
    tx, ty = 872, 879

print(f"Will move cursor to ({tx}, {ty}) in 3 seconds.")
print("Switch to WC3 now!\n")
for i in (3, 2, 1):
    print(f"  {i}…")
    time.sleep(1)

move_cursor(tx, ty)
print(f"Cursor moved to ({tx}, {ty}). Is it on the Great Hall?")
print("(press Ctrl+C or wait 5s)")
time.sleep(5)

cx, cy = get_mouse()
print(f"Current cursor pos: ({cx}, {cy})")
