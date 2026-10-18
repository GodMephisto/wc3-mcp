---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "src/wc3bot/core/process.py"
ref: HEAD
---

import subprocess, sys
import ctypes, ctypes.util
sys.path.insert(0, "src")
from wc3bot.action.input import screen_size

TARGET = "Warcraft III"

# ── Mach task handle (re-exported from sidecar_reader for convenience) ─────────
_libc = ctypes.CDLL(ctypes.util.find_library("c"))
_libc.task_for_pid.restype  = ctypes.c_int
_libc.task_for_pid.argtypes = [ctypes.c_uint, ctypes.c_int, ctypes.POINTER(ctypes.c_uint)]
_libc.mach_task_self.restype  = ctypes.c_uint
_libc.mach_task_self.argtypes = []


def get_task(pid: int) -> int:
    """Return the Mach task handle for the given PID."""
    t = ctypes.c_uint(0)
    r = _libc.task_for_pid(_libc.mach_task_self(), pid, ctypes.byref(t))
    assert r == 0, f"task_for_pid failed kr={r}"
    return t.value


def get_pid(target: str = TARGET) -> int:
    r = subprocess.run(["pgrep", "-x", target], capture_output=True, text=True)
    if r.returncode != 0:
        raise RuntimeError(f"'{target}' not running")
    return int(r.stdout.strip().split("\n")[0])


def window_bounds(target: str = TARGET) -> tuple[int, int, int, int] | None:
    """Return (left, top, right, bottom) of the WC3 window via System Events."""
    r = subprocess.run(
        ["osascript", "-e",
         f'tell application "System Events" to tell process "{target}"'
         f' to get {{position, size}} of window 1'],
        capture_output=True, text=True,
    )
    if r.returncode == 0 and r.stdout.strip():
        x, y, w, h = (int(v) for v in r.stdout.strip().split(","))
        return (x, y, x + w, y + h)   # left, top, right, bottom
    return None


def gh_click_pos(target: str = TARGET) -> tuple[int, int]:
    """Screen position of GH center after Backspace (camera snap to main hall).

    Calibrated 2026-05-24 via T04c on 2560×1440 fullscreen:
      cx = center_x - 1.1% of width  (GH is slightly left of screen center)
      cy = center_y - 13.6% of height (GH is ~36.4% from top; UI at bottom pushes it up)
    """
    b = window_bounds(target)
    if b:
        left, top, right, bottom = b
        w = right - left
        h = bottom - top
        cx = (left + right) // 2 - w // 91      # ~1.1% left of centre
        cy = (top + bottom) // 2 - h * 136 // 1000  # ~13.6% above centre
        return cx, cy
    sw, sh = screen_size()
    return sw // 2 - sw // 91, sh * 364 // 1000  # 36.4% from top (consistent with main formula)
