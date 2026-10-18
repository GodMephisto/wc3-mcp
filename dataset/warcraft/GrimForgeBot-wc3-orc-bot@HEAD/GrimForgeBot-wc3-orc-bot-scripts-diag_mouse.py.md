---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/diag_mouse.py"
ref: HEAD
---

#!/usr/bin/env python3
"""diag_mouse.py — Find which mouse-click method WC3 accepts.

Flow:
  1. 3s countdown → switch to WC3
  2. Backspace → camera on GH
  3. Move cursor to game-area centre
  4. Test 7 click methods in sequence (cursor stays put)

Usage:
    python3 scripts/diag_mouse.py
"""
import ctypes, subprocess, sys, time

_CG = ctypes.CDLL("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")
_CF = ctypes.CDLL("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")

class _CGPoint(ctypes.Structure):
    _fields_ = [("x", ctypes.c_double), ("y", ctypes.c_double)]

for fn, res, args in [
    ("CGEventCreateKeyboardEvent",   ctypes.c_void_p, [ctypes.c_void_p, ctypes.c_uint16, ctypes.c_bool]),
    ("CGEventCreateMouseEvent",      ctypes.c_void_p, [ctypes.c_void_p, ctypes.c_uint32, _CGPoint, ctypes.c_uint32]),
    ("CGEventSourceCreate",          ctypes.c_void_p, [ctypes.c_int32]),
    ("CGEventPost",                  None,            [ctypes.c_uint32, ctypes.c_void_p]),
    ("CGEventPostToPid",             None,            [ctypes.c_int32, ctypes.c_void_p]),
    ("CGEventSetIntegerValueField",  None,            [ctypes.c_void_p, ctypes.c_uint32, ctypes.c_int64]),
]:
    getattr(_CG, fn).restype  = res
    getattr(_CG, fn).argtypes = args
_CF.CFRelease.restype  = None
_CF.CFRelease.argtypes = [ctypes.c_void_p]

BACKSPACE_VK             = 0x33
kCGEventLeftMouseDown    = 1
kCGEventLeftMouseUp      = 2
kCGEventMouseMoved       = 5
kCGMouseButtonLeft       = 0
kCGHIDEventTap           = 0
kCGAnnotatedSessionEventTap = 2
kCGEventSourceStateCombined = 0
kCGMouseEventClickState  = 1

def get_pid(name="Warcraft III"):
    r = subprocess.run(["pgrep", "-x", name], capture_output=True, text=True)
    if r.returncode != 0:
        raise RuntimeError(f"'{name}' not running")
    return int(r.stdout.strip().split("\n")[0])

def activate():
    subprocess.run(["osascript", "-e", 'tell application "Warcraft III" to activate'],
                   capture_output=True)
    time.sleep(0.4)

def game_center():
    r = subprocess.run(
        ["osascript", "-e", 'tell application "Warcraft III" to get bounds of window 1'],
        capture_output=True, text=True)
    if r.returncode == 0 and r.stdout.strip():
        left, top, right, bottom = (int(x) for x in r.stdout.strip().split(","))
        h = bottom - top
        cx = (left + right) // 2
        cy = (top + bottom) // 2 - h * 14 // 100
        return cx, cy
    return 1280, 620  # fallback

def send_key(pid, vk):
    for down in (True, False):
        ev = _CG.CGEventCreateKeyboardEvent(None, vk, down)
        _CG.CGEventPostToPid(pid, ev)
        _CF.CFRelease(ev)
        time.sleep(0.05)

def move_cursor(x, y):
    pt = _CGPoint(x, y)
    ev = _CG.CGEventCreateMouseEvent(None, kCGEventMouseMoved, pt, kCGMouseButtonLeft)
    _CG.CGEventPost(kCGHIDEventTap, ev)
    _CF.CFRelease(ev)

def click_A(x, y, pid):
    pt = _CGPoint(x, y)
    for etype in (kCGEventLeftMouseDown, kCGEventLeftMouseUp):
        ev = _CG.CGEventCreateMouseEvent(None, etype, pt, kCGMouseButtonLeft)
        _CG.CGEventPost(kCGHIDEventTap, ev)
        _CF.CFRelease(ev)
        time.sleep(0.05)

def click_B(x, y, pid):
    pt = _CGPoint(x, y)
    src = _CG.CGEventSourceCreate(kCGEventSourceStateCombined)
    for etype in (kCGEventLeftMouseDown, kCGEventLeftMouseUp):
        ev = _CG.CGEventCreateMouseEvent(src, etype, pt, kCGMouseButtonLeft)
        _CG.CGEventPost(kCGHIDEventTap, ev)
        _CF.CFRelease(ev)
        time.sleep(0.05)
    if src: _CF.CFRelease(src)

def click_C(x, y, pid):
    pt = _CGPoint(x, y)
    for etype in (kCGEventLeftMouseDown, kCGEventLeftMouseUp):
        ev = _CG.CGEventCreateMouseEvent(None, etype, pt, kCGMouseButtonLeft)
        _CG.CGEventPost(kCGAnnotatedSessionEventTap, ev)
        _CF.CFRelease(ev)
        time.sleep(0.05)

def click_D(x, y, pid):
    pt = _CGPoint(x, y)
    for etype in (kCGEventLeftMouseDown, kCGEventLeftMouseUp):
        ev = _CG.CGEventCreateMouseEvent(None, etype, pt, kCGMouseButtonLeft)
        _CG.CGEventPostToPid(pid, ev)
        _CF.CFRelease(ev)
        time.sleep(0.05)

def click_E(x, y, pid):
    pt = _CGPoint(x, y)
    src = _CG.CGEventSourceCreate(kCGEventSourceStateCombined)
    for etype in (kCGEventLeftMouseDown, kCGEventLeftMouseUp):
        ev = _CG.CGEventCreateMouseEvent(src, etype, pt, kCGMouseButtonLeft)
        _CG.CGEventSetIntegerValueField(ev, kCGMouseEventClickState, 1)
        _CG.CGEventPost(kCGHIDEventTap, ev)
        _CF.CFRelease(ev)
        time.sleep(0.05)
    if src: _CF.CFRelease(src)

def click_F(x, y, pid):
    subprocess.run(
        ["osascript", "-e",
         f'tell application "System Events" to click at {{{int(x)}, {int(y)}}}'],
        capture_output=True)

def click_G(x, y, pid):
    r = subprocess.run(["cliclick", f"c:{int(x)},{int(y)}"], capture_output=True)
    if r.returncode != 0:
        print("  (cliclick not installed — skipping)")

METHODS = [
    ("A: CGEventPost HID, None src",        click_A),
    ("B: CGEventPost HID, combined src",    click_B),
    ("C: CGEventPost annotated-session",    click_C),
    ("D: CGEventPostToPid",                 click_D),
    ("E: CGEventPost + click-count=1",      click_E),
    ("F: AppleScript System Events",        click_F),
    ("G: cliclick",                         click_G),
]

# ── Setup ─────────────────────────────────────────────────────────────────────

pid = get_pid()
print(f"WC3 PID = {pid}")
print("Switching to WC3 in 3 s…")
for i in (3, 2, 1):
    print(f"  {i}…"); time.sleep(1)

activate()
send_key(pid, BACKSPACE_VK)   # camera → GH
time.sleep(0.4)

cx, cy = game_center()
print(f"Game-area centre: ({cx}, {cy})", flush=True)
move_cursor(cx, cy)
time.sleep(0.2)

# ── Test each method ─────────────────────────────────────────────────────────

results = {}
for label, fn in METHODS:
    print(f"\n=== {label} ===", flush=True)
    fn(cx, cy, pid)
    time.sleep(0.3)
    results[label] = input("  GH selected? (y/n) ").strip().lower() == "y"
    # Deselect for next round
    send_key(pid, 0x35)   # Escape VK
    time.sleep(0.2)

# ── Summary ───────────────────────────────────────────────────────────────────

print("\n=== Summary ===")
for k, v in results.items():
    print(f"  {'✓' if v else '✗'}  {k}")
working = [k for k, v in results.items() if v]
if working:
    print(f"\n→ Use: {working[0]}")
else:
    print("\n✗ Nothing worked → need IOKit HID virtual mouse (sudo)")
