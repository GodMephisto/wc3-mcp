---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/find_gh_pos.py"
ref: HEAD
---

#!/usr/bin/env python3
"""find_gh_pos.py — Report mouse position every second.

Run this, switch to WC3 at game start, hover your mouse over the
Great Hall centre, then come back and Ctrl+C.  The last printed
coordinates are the ones to use for gh_cx / gh_cy in bot.py.
"""
import ctypes, time

_CG = ctypes.CDLL("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")

class _CGPoint(ctypes.Structure):
    _fields_ = [("x", ctypes.c_double), ("y", ctypes.c_double)]

_CG.CGEventCreate.restype  = ctypes.c_void_p
_CG.CGEventCreate.argtypes = [ctypes.c_void_p]
_CG.CGEventGetLocation.restype  = _CGPoint
_CG.CGEventGetLocation.argtypes = [ctypes.c_void_p]
_CF = ctypes.CDLL("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")
_CF.CFRelease.restype  = None
_CF.CFRelease.argtypes = [ctypes.c_void_p]

def mouse_pos():
    ev = _CG.CGEventCreate(None)
    pt = _CG.CGEventGetLocation(ev)
    _CF.CFRelease(ev)
    return int(pt.x), int(pt.y)

print("Hover your mouse over the Great Hall centre in WC3, then read the coords below.")
print("Ctrl+C to stop.\n")
try:
    while True:
        x, y = mouse_pos()
        print(f"\r  mouse: ({x}, {y})    ", end="", flush=True)
        time.sleep(0.2)
except KeyboardInterrupt:
    x, y = mouse_pos()
    print(f"\n\nFinal position: ({x}, {y})")
    print(f"\nPaste into bot.py:  gh_cx, gh_cy = {x}, {y}")
