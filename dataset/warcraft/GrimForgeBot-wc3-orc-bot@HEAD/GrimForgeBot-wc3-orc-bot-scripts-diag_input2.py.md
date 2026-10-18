---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/diag_input2.py"
ref: HEAD
---

#!/usr/bin/env python3
"""diag_input2.py — CGEvent test: can we open the WC3 chat box?

Activates WC3 programmatically before each attempt, then sends Enter.
Enter always opens the chat box in WC3 — unmissable visual confirmation.

Run with WC3 open and a game in progress.
"""
import ctypes, subprocess, time

_CG = ctypes.CDLL("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")
_CF = ctypes.CDLL("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")

_CG.CGEventCreateKeyboardEvent.restype  = ctypes.c_void_p
_CG.CGEventCreateKeyboardEvent.argtypes = [ctypes.c_void_p, ctypes.c_uint16, ctypes.c_bool]
_CG.CGEventSourceCreate.restype  = ctypes.c_void_p
_CG.CGEventSourceCreate.argtypes = [ctypes.c_int32]
_CG.CGEventPost.restype  = None
_CG.CGEventPost.argtypes = [ctypes.c_uint32, ctypes.c_void_p]
_CG.CGEventPostToPid.restype  = None
_CG.CGEventPostToPid.argtypes = [ctypes.c_int32, ctypes.c_void_p]
_CF.CFRelease.restype  = None
_CF.CFRelease.argtypes = [ctypes.c_void_p]

ENTER_VK = 0x24   # macOS VK for Return/Enter

kCGHIDEventTap               = 0
kCGAnnotatedSessionEventTap  = 2
kCGEventSourceStateCombined  = 0
kCGEventSourceStatePrivate   = -1

def get_pid(name="Warcraft III") -> int:
    r = subprocess.run(["pgrep", "-x", name], capture_output=True, text=True)
    if r.returncode != 0:
        raise RuntimeError(f"'{name}' not running")
    return int(r.stdout.strip().split("\n")[0])

def activate_wc3():
    subprocess.run(["osascript", "-e", 'tell application "Warcraft III" to activate'],
                   capture_output=True)
    time.sleep(0.4)

def send_global(vk: int, source_state: int):
    src = _CG.CGEventSourceCreate(source_state)
    for down in (True, False):
        ev = _CG.CGEventCreateKeyboardEvent(src, vk, down)
        _CG.CGEventPost(kCGHIDEventTap, ev)
        _CF.CFRelease(ev)
        time.sleep(0.05)
    if src: _CF.CFRelease(src)

def send_pid(pid: int, vk: int):
    for down in (True, False):
        ev = _CG.CGEventCreateKeyboardEvent(None, vk, down)
        _CG.CGEventPostToPid(pid, ev)
        _CF.CFRelease(ev)
        time.sleep(0.05)

def ask(q: str) -> bool:
    return input(q + " (y/n) ").strip().lower() == "y"

pid = get_pid()
print(f"WC3 PID = {pid}")
print("Watch WC3 — if Enter works, the chat box opens.\n")

results = {}

print("=== A: CGEventPost  source=combined ===")
activate_wc3()
send_global(ENTER_VK, kCGEventSourceStateCombined)
time.sleep(0.3)
results["A: Post(HID, combined-src)"] = ask("  Chat opened in WC3?")

print("\n=== B: CGEventPost  source=private ===")
activate_wc3()
send_global(ENTER_VK, kCGEventSourceStatePrivate)
time.sleep(0.3)
results["B: Post(HID, private-src)"] = ask("  Chat opened in WC3?")

print("\n=== C: CGEventPost  annotated-session tap ===")
activate_wc3()
src = _CG.CGEventSourceCreate(kCGEventSourceStateCombined)
for down in (True, False):
    ev = _CG.CGEventCreateKeyboardEvent(src, ENTER_VK, down)
    _CG.CGEventPost(kCGAnnotatedSessionEventTap, ev)
    _CF.CFRelease(ev)
    time.sleep(0.05)
if src: _CF.CFRelease(src)
time.sleep(0.3)
results["C: Post(annotatedSession)"] = ask("  Chat opened in WC3?")

print("\n=== D: CGEventPostToPid ===")
activate_wc3()
send_pid(pid, ENTER_VK)
time.sleep(0.3)
results["D: PostToPid"] = ask("  Chat opened in WC3?")

print("\n=== E: AppleScript key code 36 (Return) ===")
activate_wc3()
r = subprocess.run(["osascript", "-e",
                    'tell application "System Events" to key code 36'],
                   capture_output=True, text=True)
print(f"  exit={r.returncode}  err={r.stderr.strip()!r}")
time.sleep(0.3)
results["E: AppleScript key code 36"] = ask("  Chat opened in WC3?")

print("\n=== Summary ===")
for k, v in results.items():
    print(f"  {'✓' if v else '✗'}  {k}")
if any(results.values()):
    print("\n✓ CGEvent works — build action layer on the working method.")
else:
    print("\n✗ Nothing worked → need IOKit virtual HID device (hid_keyboard.py).")
