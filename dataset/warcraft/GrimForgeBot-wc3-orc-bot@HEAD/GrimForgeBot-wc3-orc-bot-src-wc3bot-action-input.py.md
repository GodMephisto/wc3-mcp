---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "src/wc3bot/action/input.py"
ref: HEAD
---

"""wc3bot.action.input — Send keyboard and mouse events to Warcraft III.

Uses CGEventPostToPid so WC3 does NOT need to be the focused window,
though in practice the bot keeps WC3 in the foreground.

Confirmed working on macOS (Darwin 25 / Reforged 2.x) — all CGEvent
methods succeeded in P0.7 PoC (2026-05-23).

Public API
──────────
    from wc3bot.action.input import WC3Input

    inp = WC3Input()          # attaches to running WC3 process
    inp.key("P")              # train peon
    inp.key("B")              # open build menu
    inp.click(960, 540)       # left-click at screen coords
    inp.right_click(800, 400) # right-click (move / attack-move target)
"""
import ctypes, subprocess, time

_CG = ctypes.CDLL("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")
_CF = ctypes.CDLL("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")

# ── CGEvent bindings ──────────────────────────────────────────────────────────

class _CGPoint(ctypes.Structure):
    _fields_ = [("x", ctypes.c_double), ("y", ctypes.c_double)]

_CG.CGEventCreateKeyboardEvent.restype  = ctypes.c_void_p
_CG.CGEventCreateKeyboardEvent.argtypes = [ctypes.c_void_p, ctypes.c_uint16, ctypes.c_bool]
_CG.CGEventSetFlags.restype  = None
_CG.CGEventSetFlags.argtypes = [ctypes.c_void_p, ctypes.c_uint64]
_CG.CGEventGetFlags.restype  = ctypes.c_uint64
_CG.CGEventGetFlags.argtypes = [ctypes.c_void_p]
_CG.CGEventCreateMouseEvent.restype  = ctypes.c_void_p
_CG.CGEventCreateMouseEvent.argtypes = [ctypes.c_void_p, ctypes.c_uint32, _CGPoint, ctypes.c_uint32]
_CG.CGEventPostToPid.restype  = None
_CG.CGEventPostToPid.argtypes = [ctypes.c_int32, ctypes.c_void_p]
_CG.CGEventPost.restype  = None
_CG.CGEventPost.argtypes = [ctypes.c_uint32, ctypes.c_void_p]
_CG.CGWarpMouseCursorPosition.restype  = ctypes.c_int
_CG.CGWarpMouseCursorPosition.argtypes = [_CGPoint]
_CG.CGEventCreateScrollWheelEvent.restype  = ctypes.c_void_p
_CG.CGEventCreateScrollWheelEvent.argtypes = [ctypes.c_void_p, ctypes.c_uint32, ctypes.c_uint32, ctypes.c_int32]
_CF.CFRelease.restype  = None
_CF.CFRelease.argtypes = [ctypes.c_void_p]

kCGHIDEventTap = 0   # global HID event tap — needed for mouse clicks

# Modifier flag masks
kCGEventFlagMaskControl   = 0x00040000
kCGEventFlagMaskShift     = 0x00020000
kCGEventFlagMaskAlternate = 0x00080000  # Option/Alt
kCGEventFlagMaskCommand   = 0x00100000

# Modifier VK codes
VK_MOD = {"CTRL": 0x3B, "SHIFT": 0x38, "ALT": 0x3A, "CMD": 0x37}
MOD_FLAG = {
    "CTRL":  kCGEventFlagMaskControl,
    "SHIFT": kCGEventFlagMaskShift,
    "ALT":   kCGEventFlagMaskAlternate,
    "CMD":   kCGEventFlagMaskCommand,
}

kCGEventLeftMouseDown    = 1
kCGEventLeftMouseUp      = 2
kCGEventRightMouseDown   = 3
kCGEventRightMouseUp     = 4
kCGEventLeftMouseDragged = 6
kCGMouseButtonLeft       = 0
kCGMouseButtonRight      = 1

# ── macOS virtual-key codes (HIToolbox) ───────────────────────────────────────

VK: dict[str, int] = {
    # Letters (scan codes, not ASCII)
    "A": 0x00, "S": 0x01, "D": 0x02, "F": 0x03, "H": 0x04, "G": 0x05,
    "Z": 0x06, "X": 0x07, "C": 0x08, "V": 0x09, "B": 0x0B, "Q": 0x0C,
    "W": 0x0D, "E": 0x0E, "R": 0x0F, "Y": 0x10, "T": 0x11, "O": 0x1F,
    "U": 0x20, "I": 0x22, "P": 0x23, "L": 0x25, "J": 0x26, "K": 0x28,
    "N": 0x2D, "M": 0x2E,
    # Digits
    "1": 0x12, "2": 0x13, "3": 0x14, "4": 0x15, "5": 0x17,
    "6": 0x16, "7": 0x1A, "8": 0x1C, "9": 0x19, "0": 0x1D,
    # Special
    "ENTER": 0x24, "TAB": 0x30, "SPACE": 0x31, "ESCAPE": 0x35,
    "BACKSPACE": 0x33, "DELETE": 0x75,
    # Function keys
    "F1": 0x7A, "F2": 0x78, "F3": 0x63, "F4": 0x76,
    "F5": 0x60, "F6": 0x61, "F7": 0x62, "F8": 0x64,
    "F9": 0x65, "F10": 0x6D, "F11": 0x67, "F12": 0x6F,
}

# Common WC3 Orc hotkeys — for documentation / readable call sites
WC3 = {
    # Hero / selection
    "select_hero":      "F1",
    "select_all_units": "F2",
    # Peon management
    "train_peon":       "P",   # Great Hall selected
    "build_menu":       "B",   # Peon selected
    # Buildings
    "build_burrow":     "R",   # in build menu
    "build_barracks":   "B",   # in build menu (second press after B)
    "build_altar":      "A",   # in build menu
    "build_beastiary":  "E",   # in build menu
    "build_spirit_lodge": "S", # in build menu
    "build_watch_tower":"T",   # in build menu
    # Unit training (barracks selected)
    "train_grunt":      "G",
    "train_raider":     "R",
    "train_headhunter": "H",
    "train_demolisher": "D",
    "train_shaman":     "S",
    "train_witch_doctor":"W",
    # Upgrades / research
    "upgrade_great_hall": "U",
    # Hero abilities (context-dependent)
    "hero_ability_1":   "Q",
    "hero_ability_2":   "W",
    "hero_ability_3":   "E",
    "hero_ability_4":   "R",
    "hero_ultimate":    "T",
    # Control
    "stop":     "S",
    "hold":     "H",
    "patrol":   "P",
    "attack":   "A",
    "move":     "M",
}


def mouse_pos() -> tuple[int, int]:
    """Return current screen position of the mouse cursor via CGEvent."""
    _CG.CGEventCreate.restype  = ctypes.c_void_p
    _CG.CGEventCreate.argtypes = [ctypes.c_void_p]

    class _CGPointLocal(ctypes.Structure):
        _fields_ = [("x", ctypes.c_double), ("y", ctypes.c_double)]

    _CG.CGEventGetLocation.restype  = _CGPointLocal
    _CG.CGEventGetLocation.argtypes = [ctypes.c_void_p]

    ev = _CG.CGEventCreate(None)
    pt = _CG.CGEventGetLocation(ev)
    _CF.CFRelease(ev)
    return int(pt.x), int(pt.y)


def screen_size() -> tuple[int, int]:
    """Return (width, height) of the main display in points."""
    _CG.CGMainDisplayID.restype  = ctypes.c_uint32
    _CG.CGMainDisplayID.argtypes = []

    class _CGSize(ctypes.Structure):
        _fields_ = [("width", ctypes.c_double), ("height", ctypes.c_double)]
    class _CGOrigin(ctypes.Structure):
        _fields_ = [("x", ctypes.c_double), ("y", ctypes.c_double)]
    class _CGRect(ctypes.Structure):
        _fields_ = [("origin", _CGOrigin), ("size", _CGSize)]

    _CG.CGDisplayBounds.restype  = _CGRect
    _CG.CGDisplayBounds.argtypes = [ctypes.c_uint32]

    display = _CG.CGMainDisplayID()
    bounds  = _CG.CGDisplayBounds(display)
    return int(bounds.size.width), int(bounds.size.height)


class WC3Input:
    """Send keyboard and mouse input to the running WC3 process."""

    TARGET = "Warcraft III"

    def __init__(self, pid: int | None = None):
        if pid is None:
            pid = self._find_pid()
        self.pid = pid

    @staticmethod
    def _find_pid() -> int:
        r = subprocess.run(["pgrep", "-x", WC3Input.TARGET],
                           capture_output=True, text=True)
        if r.returncode != 0:
            raise RuntimeError(f"'{WC3Input.TARGET}' not running")
        return int(r.stdout.strip().split("\n")[0])

    def activate(self) -> None:
        """Bring WC3 to the foreground via System Events."""
        subprocess.run(
            ["osascript", "-e",
             f'tell application "System Events" to tell process "{self.TARGET}"'
             f' to set frontmost to true'],
            capture_output=True,
        )
        time.sleep(0.3)

    # ── internals ─────────────────────────────────────────────────────────────

    def _post(self, ev: int) -> None:
        _CG.CGEventPostToPid(self.pid, ev)
        _CF.CFRelease(ev)

    # ── keyboard ──────────────────────────────────────────────────────────────

    def key(self, name: str, hold: float = 0.012) -> None:
        """Press and release a key by name (e.g. 'P', 'F1', 'ENTER').

        name can also be a WC3 action alias from the WC3 dict above,
        e.g. inp.key('train_peon').

        hold: key-down duration in seconds.
              Default 0.012s (~1 game frame @ 60fps) — WC3 registers reliably.
              Previous default was 0.050s (conservative); reduced for higher APM.
        """
        resolved = WC3.get(name, name).upper()
        vk = VK.get(resolved)
        if vk is None:
            raise ValueError(f"Unknown key: {name!r} (resolved to {resolved!r})")
        self._post(_CG.CGEventCreateKeyboardEvent(None, vk, True))
        time.sleep(hold)
        self._post(_CG.CGEventCreateKeyboardEvent(None, vk, False))
        time.sleep(0.008)   # short gap before next event

    def ctrl_key(self, name: str, hold: float = 0.012) -> None:
        """Press Ctrl+key (e.g. ctrl_key('1') assigns control group 1)."""
        resolved = WC3.get(name, name).upper()
        vk = VK.get(resolved)
        if vk is None:
            raise ValueError(f"Unknown key: {name!r}")
        flag = kCGEventFlagMaskControl
        # Ctrl down
        ev = _CG.CGEventCreateKeyboardEvent(None, VK_MOD["CTRL"], True)
        _CG.CGEventSetFlags(ev, flag)
        self._post(ev)
        time.sleep(0.010)
        # Key down with Ctrl flag
        ev = _CG.CGEventCreateKeyboardEvent(None, vk, True)
        _CG.CGEventSetFlags(ev, flag)
        self._post(ev)
        time.sleep(hold)
        # Key up
        ev = _CG.CGEventCreateKeyboardEvent(None, vk, False)
        _CG.CGEventSetFlags(ev, flag)
        self._post(ev)
        time.sleep(0.010)
        # Ctrl up
        ev = _CG.CGEventCreateKeyboardEvent(None, VK_MOD["CTRL"], False)
        self._post(ev)
        time.sleep(0.008)

    def key_chord(self, *names: str, hold: float = 0.05) -> None:
        """Press multiple keys simultaneously (e.g. Shift+click emulation)."""
        vks = []
        for name in names:
            resolved = WC3.get(name, name).upper()
            vk = VK[resolved]
            vks.append(vk)
            self._post(_CG.CGEventCreateKeyboardEvent(None, vk, True))
        time.sleep(hold)
        for vk in reversed(vks):
            self._post(_CG.CGEventCreateKeyboardEvent(None, vk, False))
        time.sleep(0.02)

    # ── mouse ─────────────────────────────────────────────────────────────────

    def _post_mouse(self, ev: int) -> None:
        """Post a mouse event globally (CGEventPostToPid ignores mouse in WC3)."""
        _CG.CGEventPost(kCGHIDEventTap, ev)
        _CF.CFRelease(ev)

    def scroll(self, delta: int, steps: int = 1, delay: float = 0.05) -> None:
        """Scroll mouse wheel. delta>0 = away from user (zoom out in WC3)."""
        for _ in range(steps):
            ev = _CG.CGEventCreateScrollWheelEvent(None, 1, 1, delta)
            self._post_mouse(ev)
            time.sleep(delay)

    def zoom_out(self, clicks: int = 10) -> None:
        """Zoom out camera by scrolling wheel away (positive delta) N times."""
        self.scroll(3, steps=clicks, delay=0.03)

    def move_cursor(self, x: float, y: float) -> None:
        """Warp cursor to (x, y) — visual verification only, not a game input event."""
        pt = _CGPoint(x, y)
        _CG.CGWarpMouseCursorPosition(pt)

    def click(self, x: float, y: float, hold: float = 0.012) -> None:
        """Left-click at screen coordinates (x, y).

        Move cursor first (required — WC3 uses physical cursor position),
        then fire CGEventPost HID tap (confirmed working in diag_mouse.py).

        hold: mouse-down duration. Default 0.012s; previous default was 0.050s.
        """
        pt = _CGPoint(x, y)
        # 1. Move cursor to target
        mv = _CG.CGEventCreateMouseEvent(None, 5, pt, kCGMouseButtonLeft)  # 5=MouseMoved
        _CG.CGEventPost(kCGHIDEventTap, mv)
        _CF.CFRelease(mv)
        time.sleep(0.020)   # cursor-settle: 1-2 frames; was 0.050s
        # 2. Click at that position
        for etype in (kCGEventLeftMouseDown, kCGEventLeftMouseUp):
            ev = _CG.CGEventCreateMouseEvent(None, etype, pt, kCGMouseButtonLeft)
            _CG.CGEventPost(kCGHIDEventTap, ev)
            _CF.CFRelease(ev)
            time.sleep(hold if etype == kCGEventLeftMouseDown else 0.008)

    def right_click(self, x: float, y: float, hold: float = 0.012) -> None:
        """Right-click at screen coordinates (x, y)."""
        pt = _CGPoint(x, y)
        mv = _CG.CGEventCreateMouseEvent(None, 5, pt, kCGMouseButtonLeft)
        _CG.CGEventPost(kCGHIDEventTap, mv)
        _CF.CFRelease(mv)
        time.sleep(0.020)   # cursor-settle; was 0.050s
        for etype, btn in ((kCGEventRightMouseDown, kCGMouseButtonRight),
                           (kCGEventRightMouseUp,   kCGMouseButtonRight)):
            ev = _CG.CGEventCreateMouseEvent(None, etype, pt, btn)
            _CG.CGEventPost(kCGHIDEventTap, ev)
            _CF.CFRelease(ev)
            time.sleep(hold if etype == kCGEventRightMouseDown else 0.008)

    def shift_click(self, x: float, y: float, hold: float = 0.012) -> None:
        """Shift+left-click — adds the unit under cursor to the current selection."""
        pt = _CGPoint(x, y)
        flag = kCGEventFlagMaskShift
        # Move cursor
        mv = _CG.CGEventCreateMouseEvent(None, 5, pt, kCGMouseButtonLeft)
        _CG.CGEventPost(kCGHIDEventTap, mv)
        _CF.CFRelease(mv)
        time.sleep(0.020)
        # Shift key down
        ev = _CG.CGEventCreateKeyboardEvent(None, VK_MOD["SHIFT"], True)
        _CG.CGEventSetFlags(ev, flag)
        self._post(ev)
        time.sleep(0.008)
        # Click down + up with Shift flag
        for etype in (kCGEventLeftMouseDown, kCGEventLeftMouseUp):
            ev = _CG.CGEventCreateMouseEvent(None, etype, pt, kCGMouseButtonLeft)
            _CG.CGEventSetFlags(ev, flag)
            _CG.CGEventPost(kCGHIDEventTap, ev)
            _CF.CFRelease(ev)
            time.sleep(hold if etype == kCGEventLeftMouseDown else 0.008)
        # Shift key up
        ev = _CG.CGEventCreateKeyboardEvent(None, VK_MOD["SHIFT"], False)
        self._post(ev)
        time.sleep(0.008)

    def ctrl_click(self, x: float, y: float, hold: float = 0.012) -> None:
        """Ctrl+left-click — selects all visible units of the same type (WC3 mechanic)."""
        pt = _CGPoint(x, y)
        flag = kCGEventFlagMaskControl
        mv = _CG.CGEventCreateMouseEvent(None, 5, pt, kCGMouseButtonLeft)
        _CG.CGEventPost(kCGHIDEventTap, mv)
        _CF.CFRelease(mv)
        time.sleep(0.020)
        ev = _CG.CGEventCreateKeyboardEvent(None, VK_MOD["CTRL"], True)
        _CG.CGEventSetFlags(ev, flag)
        self._post(ev)
        time.sleep(0.008)
        for etype in (kCGEventLeftMouseDown, kCGEventLeftMouseUp):
            ev = _CG.CGEventCreateMouseEvent(None, etype, pt, kCGMouseButtonLeft)
            _CG.CGEventSetFlags(ev, flag)
            _CG.CGEventPost(kCGHIDEventTap, ev)
            _CF.CFRelease(ev)
            time.sleep(hold if etype == kCGEventLeftMouseDown else 0.008)
        ev = _CG.CGEventCreateKeyboardEvent(None, VK_MOD["CTRL"], False)
        self._post(ev)
        time.sleep(0.008)

    def double_click(self, x: float, y: float) -> None:
        self.click(x, y)
        time.sleep(0.05)
        self.click(x, y)

    def drag(self, x1: float, y1: float, x2: float, y2: float,
             hold: float = 0.08) -> None:
        """Click-drag from (x1,y1) to (x2,y2). Used for box-select."""
        p1 = _CGPoint(x1, y1)
        p2 = _CGPoint(x2, y2)
        # Move cursor to start
        mv = _CG.CGEventCreateMouseEvent(None, 5, p1, kCGMouseButtonLeft)
        _CG.CGEventPost(kCGHIDEventTap, mv)
        _CF.CFRelease(mv)
        time.sleep(0.05)
        # Mouse down at start
        ev = _CG.CGEventCreateMouseEvent(None, kCGEventLeftMouseDown, p1, kCGMouseButtonLeft)
        _CG.CGEventPost(kCGHIDEventTap, ev)
        _CF.CFRelease(ev)
        time.sleep(hold)
        # Drag to end: a few steps so WC3 sees the drag movement
        steps = 5
        for i in range(1, steps + 1):
            xi = x1 + (x2 - x1) * i / steps
            yi = y1 + (y2 - y1) * i / steps
            pd = _CGPoint(xi, yi)
            ev = _CG.CGEventCreateMouseEvent(None, kCGEventLeftMouseDragged, pd, kCGMouseButtonLeft)
            _CG.CGEventPost(kCGHIDEventTap, ev)
            _CF.CFRelease(ev)
            time.sleep(0.01)
        # Mouse up at end
        ev = _CG.CGEventCreateMouseEvent(None, kCGEventLeftMouseUp, p2, kCGMouseButtonLeft)
        _CG.CGEventPost(kCGHIDEventTap, ev)
        _CF.CFRelease(ev)
        time.sleep(0.05)
