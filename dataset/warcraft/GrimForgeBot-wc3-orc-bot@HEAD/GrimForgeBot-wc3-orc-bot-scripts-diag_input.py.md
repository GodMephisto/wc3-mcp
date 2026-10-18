---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/diag_input.py"
ref: HEAD
---

#!/usr/bin/env python3
"""diag_input.py — Diagnose why CGEvent isn't reaching WC3.

Tests three approaches in sequence and reports which (if any) works:
  1. Accessibility permission check
  2. CGEventPostToPid  (background-safe, app event queue)
  3. CGEventPost global HID tap  (requires WC3 in foreground)
  4. Same via AppleScript (fallback path, always requires focus)

Run with WC3 in the background first, then repeat with WC3 in the foreground.
"""
import ctypes, subprocess, sys, time

TARGET = "Warcraft III"

_CG = ctypes.CDLL("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")
_CF = ctypes.CDLL("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")
_AS = ctypes.CDLL("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")

# types
class _CGPoint(ctypes.Structure):
    _fields_ = [("x", ctypes.c_double), ("y", ctypes.c_double)]

_CG.CGEventCreateKeyboardEvent.restype  = ctypes.c_void_p
_CG.CGEventCreateKeyboardEvent.argtypes = [ctypes.c_void_p, ctypes.c_uint16, ctypes.c_bool]
_CG.CGEventPostToPid.restype  = None
_CG.CGEventPostToPid.argtypes = [ctypes.c_int32, ctypes.c_void_p]
_CG.CGEventPost.restype  = None
_CG.CGEventPost.argtypes = [ctypes.c_uint32, ctypes.c_void_p]
_CF.CFRelease.restype  = None
_CF.CFRelease.argtypes = [ctypes.c_void_p]
_AS.AXIsProcessTrusted.restype  = ctypes.c_bool
_AS.AXIsProcessTrusted.argtypes = []

kCGHIDEventTap = 0
F1_VK = 0x7A   # F1 — snaps WC3 camera to hero; safe, observable, no side-effects

def get_pid() -> int:
    r = subprocess.run(["pgrep", "-x", TARGET], capture_output=True, text=True)
    if r.returncode != 0:
        raise RuntimeError(f"'{TARGET}' not running")
    return int(r.stdout.strip().split("\n")[0])

def make_key(vk: int, down: bool):
    return _CG.CGEventCreateKeyboardEvent(None, vk, down)

def release(ev):
    _CF.CFRelease(ev)

# ── 1. Accessibility check ────────────────────────────────────────────────────
print("=== 1. Accessibility permission ===")
trusted = _AS.AXIsProcessTrusted()
print(f"  AXIsProcessTrusted() = {trusted}")
if not trusted:
    print("  ✗ NOT trusted — CGEventPostToPid will silently fail.")
    print("    Fix: System Settings → Privacy & Security → Accessibility")
    print("         → add Terminal (or iTerm / your terminal app) and re-run.")
else:
    print("  ✓ Trusted — synthetic events allowed.")

pid = get_pid()
print(f"\nWC3 PID = {pid}\n")

# ── 2. CGEventPostToPid ───────────────────────────────────────────────────────
print("=== 2. CGEventPostToPid (no focus needed) ===")
print("  Sending F1 down+up to WC3 via PostToPid …")
ev = make_key(F1_VK, True);  _CG.CGEventPostToPid(pid, ev); release(ev)
time.sleep(0.05)
ev = make_key(F1_VK, False); _CG.CGEventPostToPid(pid, ev); release(ev)
print("  Sent. Did WC3 camera snap to hero? (y/n)", end=" ", flush=True)
r2 = input().strip().lower()

# ── 3. CGEventPost global HID ────────────────────────────────────────────────
print("\n=== 3. CGEventPost global HID tap (WC3 must be focused NOW) ===")
print("  Switch to WC3 within 3 seconds …", flush=True)
time.sleep(3)
print("  Sending F1 …")
ev = make_key(F1_VK, True);  _CG.CGEventPost(kCGHIDEventTap, ev); release(ev)
time.sleep(0.05)
ev = make_key(F1_VK, False); _CG.CGEventPost(kCGHIDEventTap, ev); release(ev)
time.sleep(1)
print("  Did WC3 camera snap to hero? (y/n)", end=" ", flush=True)
r3 = input().strip().lower()

# ── 4. AppleScript fallback ───────────────────────────────────────────────────
print("\n=== 4. AppleScript keystroke (WC3 focused) ===")
print("  Switch to WC3 within 3 seconds …", flush=True)
time.sleep(3)
script = '''
tell application "System Events"
    tell process "Warcraft III"
        key code 122
    end tell
end tell
'''
result = subprocess.run(["osascript", "-e", script], capture_output=True, text=True)
print(f"  osascript exit={result.returncode}")
if result.stderr:
    print(f"  stderr: {result.stderr.strip()}")
time.sleep(1)
print("  Did WC3 camera snap to hero? (y/n)", end=" ", flush=True)
r4 = input().strip().lower()

# ── Summary ───────────────────────────────────────────────────────────────────
print("\n=== Summary ===")
print(f"  Accessibility trusted  : {trusted}")
print(f"  CGEventPostToPid works : {r2 == 'y'}")
print(f"  CGEventPost HID works  : {r3 == 'y'}")
print(f"  AppleScript works      : {r4 == 'y'}")

if r2 == 'y':
    print("\n✓ Best path: CGEventPostToPid — use directly, no focus required.")
elif r3 == 'y':
    print("\n→ Use CGEventPost (global HID) — WC3 must be the active window.")
elif r4 == 'y':
    print("\n→ AppleScript works — can use osascript as action backend.")
else:
    print("\n✗ Nothing worked.")
    print("  WC3 may use SDL/custom HID polling that ignores synthetic events.")
    print("  Next option: CGEventTap (input monitor) or IOKit HID device injection.")
