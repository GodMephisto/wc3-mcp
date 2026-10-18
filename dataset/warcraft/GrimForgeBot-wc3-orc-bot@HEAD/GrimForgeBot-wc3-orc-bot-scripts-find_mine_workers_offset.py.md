---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/find_mine_workers_offset.py"
ref: HEAD
---

"""find_mine_workers_offset.py — Scan WC3 process memory to locate the
gold-mine unit struct and identify the byte offset of the "assigned workers"
field (displayed as "5/5" in the WC3 UI overlay).

Usage:
    python3 scripts/find_mine_workers_offset.py

The script is READ-ONLY — it never writes to WC3 memory.

Workflow:
  Phase 1: locate candidate addresses where mine.gold lives in memory,
           cross-checked against mine.x (int32 or float32 nearby).
  Phase 2: interactive narrowing — user sends N peons to mine, presses Enter;
           script diffs the 512-byte window at each candidate to reveal which
           offset holds the worker count (changes 0→3→5 etc.).
"""
from __future__ import annotations

import ctypes
import ctypes.util
import struct
import sys
import math
import os
import time

# ── sys.path bootstrap so wc3bot package is importable ────────────────────────
_REPO = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
sys.path.insert(0, os.path.join(_REPO, "src"))

from wc3bot.core.process import get_pid, get_task
from wc3bot.observe.lua_reader import LuaStateReader
from wc3bot.observe.mem_utils import _read_mem, _iter_regions, _MAX_REGION
from wc3bot.observe.state import GameState, UnitRecord

# ── Constants ─────────────────────────────────────────────────────────────────

MINE_FOURCCS = {"ngol", "ngme"}   # gold mine FourCC codes (standard + haunted)
WINDOW       = 256                # bytes to read before/after a gold hit (512 total)
SMALL_INT_MAX = 6                 # worker count is at most 6 (5 standard + 1 margin)

# On Apple Silicon macOS the thread stacks live in 0x160000000–0x17FFFFFFFF.
# WC3 game objects (unit instances, resource amounts) are in the heap.
# Excluding the stack range dramatically reduces false hits.
_STACK_LO = 0x160000000
_STACK_HI = 0x17FFFFFFF


# ── Helpers ───────────────────────────────────────────────────────────────────

def _hex_dump(data: bytes, base_offset: int = 0) -> str:
    """Return a formatted 16-bytes-per-row hex dump with ASCII."""
    lines = []
    for row in range(0, len(data), 16):
        chunk = data[row:row + 16]
        offset_str = f"{base_offset + row:+05d}"
        hex_part   = " ".join(f"{b:02x}" for b in chunk)
        hex_part  += "   " * (16 - len(chunk))
        ascii_part = "".join(chr(b) if 32 <= b < 127 else "." for b in chunk)
        lines.append(f"  {offset_str}  {hex_part}  |{ascii_part}|")
    return "\n".join(lines)


def _read_window(task: int, addr: int, window: int = WINDOW) -> bytes | None:
    """Read 2*window bytes centred on addr.  Returns None on failure."""
    start = addr - window
    size  = window * 2
    if start < 0:
        return None
    return _read_mem(task, start, size)


def _search_int32(data: bytes, value: int) -> list[int]:
    """Return all byte offsets in data where value appears as little-endian int32."""
    needle = struct.pack("<i", value)
    offsets, start = [], 0
    while True:
        idx = data.find(needle, start)
        if idx == -1:
            break
        offsets.append(idx)
        start = idx + 1
    return offsets


def _search_float32(data: bytes, value: float, tolerance: float = 0.5) -> list[int]:
    """Return byte offsets where a float32 is within tolerance of value."""
    offsets = []
    for i in range(0, len(data) - 3):
        try:
            f = struct.unpack_from("<f", data, i)[0]
            if math.isfinite(f) and abs(f - value) <= tolerance:
                offsets.append(i)
        except struct.error:
            pass
    return offsets


def _find_small_ints(data: bytes, base_offset: int = 0,
                     max_val: int = SMALL_INT_MAX) -> list[tuple[int, int]]:
    """Return (offset_in_data, value) for every int32 in [0, max_val]."""
    results = []
    for i in range(0, len(data) - 3, 1):
        val = struct.unpack_from("<i", data, i)[0]
        if 0 <= val <= max_val:
            results.append((base_offset + i, val))
    return results


# ── Phase 1: scan all readable regions ────────────────────────────────────────

def phase1_scan(task: int, mine: UnitRecord) -> list[dict]:
    """
    Scan heap memory for mine.gold as int32.

    WC3 uses component-based architecture — FourCC and gold are NOT adjacent.
    Strategy: find all gold int32 hits in heap regions (excluding thread stacks),
    then run Phase 2 diffs to identify which one has the worker-count field nearby.

    Returns a list of candidate dicts:
        addr        — absolute address of the gold int32
        window_base — addr - WINDOW
        gold_off    — offset of gold in window (always == WINDOW)
        window      — 2*WINDOW bytes centred on the gold value
    """
    candidates = []
    gold_needle = struct.pack("<i", mine.gold)

    print(f"\n[Phase 1] Scanning heap for gold={mine.gold} (excl. stack 0x{_STACK_LO:x}–0x{_STACK_HI:x}) …")
    print( "          (this may take 10-30 seconds)\n")

    scanned_bytes = 0
    hits_gold = 0

    for base, size, prot in _iter_regions(task):
        if not (prot & 1):
            continue
        if size > _MAX_REGION:
            continue
        # Skip Apple Silicon thread stacks
        if base >= _STACK_LO and base <= _STACK_HI:
            continue

        CHUNK = 4 * 1024 * 1024
        offset = 0
        while offset < size:
            n     = min(CHUNK, size - offset)
            chunk = _read_mem(task, base + offset, n)
            if not chunk:
                offset += n
                continue

            scanned_bytes += len(chunk)
            pos = 0
            while True:
                idx = chunk.find(gold_needle, pos)
                if idx == -1:
                    break
                abs_addr = base + offset + idx
                hits_gold += 1

                win = _read_window(task, abs_addr)
                if win is None or len(win) < WINDOW * 2:
                    pos = idx + 1
                    continue

                candidates.append({
                    "addr":        abs_addr,
                    "window_base": abs_addr - WINDOW,
                    "gold_off":    WINDOW,
                    "window":      win,
                })
                print(f"  gold hit @ 0x{abs_addr:016x}")

                pos = idx + 1
            offset += n

    print(f"\n  Scanned {scanned_bytes / 1024 / 1024:.1f} MB, "
          f"gold hits (heap-only)={hits_gold}")
    return candidates


def print_candidate(c: dict, label: str = "CANDIDATE") -> None:
    print(f"\n{'─' * 72}")
    print(f"  {label}  gold @ 0x{c['addr']:016x}")
    print(f"  Hex dump (window_base=0x{c['window_base']:016x}, ±{WINDOW} bytes):")
    print(_hex_dump(c["window"], base_offset=-WINDOW))


# ── Phase 2: interactive narrowing ────────────────────────────────────────────

def phase2_narrow(task: int, candidates: list[dict], mine: UnitRecord,
                  reader: LuaStateReader) -> None:
    """
    Interactive loop.  User is prompted to put N peons in the mine, press Enter.

    Output is suppressed for candidates with no interesting changes — only
    candidates where an int32 changed TO the expected peon count are printed.
    This keeps output small regardless of the number of candidates.

    After all steps, prints a summary of offsets that tracked the peon count
    correctly across all transitions.
    """
    if not candidates:
        print("\n[Phase 2] No candidates to narrow — re-run after Phase 1 finds hits.")
        return

    steps = [0, 1, 3, 5]   # sequence of worker counts to ask for
    # tracking: addr → {offset_rel → [observed_values]}
    offset_history: dict[int, dict[int, list[int]]] = {
        c["addr"]: {} for c in candidates
    }
    prev_windows: dict[int, bytes] = {}

    print(f"\n[Phase 2] {len(candidates)} candidate address(es) to track.")
    print("  Output only shown for candidates where a field changes to the expected count.")

    for step_idx, n_workers in enumerate(steps):
        print("\n" + "=" * 72)
        if n_workers == 0:
            print(f"  STEP {step_idx + 1}/{len(steps)}: Confirm 0 peons are at the mine.")
            print("  → Switching to WC3 in 2 s — verify no peons mining, then Alt+Tab back.")
        else:
            print(f"  STEP {step_idx + 1}/{len(steps)}: Send exactly {n_workers} peon(s) to mine, wait ~5 s.")
            print("  → Switching to WC3 in 2 s — send peons, wait for them to arrive, then Alt+Tab back.")

        time.sleep(2)
        _focus_wc3()
        _focus_terminal()
        input("  Press ENTER when peons are in position …")

        state = reader.read_state()
        if state is not None:
            cur = _nearest_mine(state, mine)
            if cur:
                print(f"  Sidecar: mine.gold={cur.gold}  sidecar_workers={cur.workers}")

        interesting_this_step = 0
        for ci, c in enumerate(candidates):
            win = _read_window(task, c["addr"])
            if win is None or len(win) < WINDOW * 2:
                continue

            addr_key  = c["addr"]
            prev_win  = prev_windows.get(addr_key)
            hist      = offset_history[addr_key]

            if prev_win and len(prev_win) == len(win) and n_workers > 0:
                # Find int32 fields that changed exactly to n_workers
                checked: set[int] = set()
                for i in range(len(win)):
                    if win[i] == prev_win[i]:
                        continue
                    quad = (i // 4) * 4
                    if quad in checked or quad + 4 > len(win):
                        continue
                    checked.add(quad)
                    new_val = struct.unpack_from("<i", win,      quad)[0]
                    old_val = struct.unpack_from("<i", prev_win, quad)[0]
                    if new_val == n_workers:
                        rel = quad - WINDOW
                        if rel not in hist:
                            hist[rel] = []
                        hist[rel].append(new_val)
                        print(f"  *** MATCH  gold@0x{addr_key:016x}"
                              f"  offset {rel:+4d}  {old_val} → {new_val} (== {n_workers} peons)")
                        interesting_this_step += 1
            elif n_workers == 0:
                # Baseline: record all small-int values
                for quad in range(0, len(win) - 3, 4):
                    val = struct.unpack_from("<i", win, quad)[0]
                    if 0 <= val <= SMALL_INT_MAX:
                        rel = quad - WINDOW
                        if rel not in hist:
                            hist[rel] = []
                        hist[rel].append(val)

            prev_windows[addr_key] = win

        if n_workers > 0 and interesting_this_step == 0:
            print(f"  (no candidate had a field change exactly to {n_workers})")

    # ── Summary ───────────────────────────────────────────────────────────────
    print("\n" + "=" * 72)
    print("  SUMMARY — offsets that tracked peon count across all steps:")
    print("  (looking for 0 → 1 → 3 → 5 progression)\n")

    found_any = False
    for c in candidates:
        addr_key = c["addr"]
        hist     = offset_history[addr_key]
        for rel_off, vals in sorted(hist.items()):
            if vals == [0, 1, 3, 5] or vals == [1, 3, 5] or vals == [0, 5] or vals == [5]:
                print(f"  *** FOUND  gold@0x{addr_key:016x}  offset {rel_off:+4d}")
                print(f"       values observed: {vals}")
                print(f"       struct_base would be gold_addr - {WINDOW}  ({addr_key - WINDOW:#x})")
                print(f"       worker_count_offset from struct_base = {WINDOW + rel_off}")
                found_any = True

    if not found_any:
        print("  No clean 0→1→3→5 progression found.")
        print("  Check the *** MATCH lines above for partial matches.")
    print("=" * 72)


# ── Utility ───────────────────────────────────────────────────────────────────

def _focus_wc3() -> None:
    """Bring Warcraft III to the foreground."""
    import subprocess as _sp
    _sp.run(
        ["osascript", "-e",
         'tell application "System Events" to tell process "Warcraft III"'
         ' to set frontmost to true'],
        capture_output=True,
    )
    time.sleep(0.4)


def _focus_terminal() -> None:
    """Bring the frontmost Terminal / iTerm2 window back to the foreground."""
    import subprocess as _sp
    script = '''
tell application "System Events"
    set termApps to {"Terminal", "iTerm2", "iTerm", "Warp", "Alacritty", "kitty"}
    repeat with appName in termApps
        if exists (process appName) then
            set frontmost of process appName to true
            return
        end if
    end repeat
end tell
'''
    _sp.run(["osascript", "-e", script], capture_output=True)
    time.sleep(0.2)


def _nearest_mine(state: GameState, ref: UnitRecord) -> UnitRecord | None:
    """Return the neutral entry nearest to ref (by squared game-coord distance)."""
    best: UnitRecord | None = None
    best_d = float("inf")
    for n in state.neutral:
        if n.id not in MINE_FOURCCS:
            continue
        d = (n.x - ref.x) ** 2 + (n.y - ref.y) ** 2
        if d < best_d:
            best_d, best = d, n
    return best


def _pick_nearest_mine(state: GameState) -> UnitRecord | None:
    """Return the mine nearest to GH, or any mine if GH is unknown."""
    mines = [n for n in state.neutral if n.id in MINE_FOURCCS]
    if not mines:
        return None
    if state.gh_x is None or state.gh_y is None:
        return mines[0]
    best, best_d = mines[0], float("inf")
    for m in mines:
        d = (m.x - state.gh_x) ** 2 + (m.y - state.gh_y) ** 2
        if d < best_d:
            best_d, best = d, m
    return best


# ── Entry point ───────────────────────────────────────────────────────────────

def main() -> None:
    print("=" * 72)
    print("  find_mine_workers_offset.py")
    print("  READ-ONLY memory scanner — WC3 process is never modified.")
    print("=" * 72)

    # 1. Attach to WC3
    print("\n[Init] Getting WC3 PID and Mach task …")
    try:
        pid  = get_pid()
        task = get_task(pid)
    except RuntimeError as e:
        sys.exit(f"ERROR: {e}")
    print(f"  PID={pid}  task={task}")

    # 2. Bootstrap sidecar reader
    print("\n[Init] Bootstrapping LuaStateReader (finds WC3BOT_STATE node) …")
    reader = LuaStateReader(task, pid=pid)

    print("\n[Init] Reading current sidecar state …")
    state = reader.read_state()
    if state is None:
        print("  [WARN] Sidecar returned None — waiting for live game …")
        raw = reader.find_live_game(timeout=60.0)
        if raw is None:
            sys.exit("ERROR: No live game found within 60 s.  Is the sidecar Lua running?")
        from wc3bot.observe.state import parse_state
        state = parse_state(raw)

    if state is None:
        sys.exit("ERROR: Could not parse sidecar state.")

    # 3. Pick nearest mine
    mine = _pick_nearest_mine(state)
    if mine is None:
        sys.exit(
            "ERROR: No gold mine found in sidecar state.\n"
            "       Make sure the Lua sidecar reports neutral units (N: entries)."
        )

    print(f"\n  Nearest mine: id={mine.id}  x={mine.x}  y={mine.y}"
          f"  gold={mine.gold}  workers={mine.workers}")
    if state.gh_x is not None:
        d = math.sqrt((mine.x - state.gh_x) ** 2 + (mine.y - state.gh_y) ** 2)
        print(f"  Distance to GH: {d:.0f} units")

    if mine.gold <= 0:
        print("  [WARN] mine.gold is 0 — the scan will use 0 as needle, expect many false hits.")

    # 4. Phase 1 scan
    candidates = phase1_scan(task, mine)

    if not candidates:
        print("\n  No candidates found.  Possible causes:")
        print("   • mine.gold value is wrong or the mine has been depleted")
        print("   • WC3 stores gold as a different type (try restarting the game)")
        print("   • SIP / TCC may be blocking memory reads")
        sys.exit(1)

    print(f"\n  Found {len(candidates)} candidate(s).  Proceeding directly to Phase 2.")
    print("  (Use Ctrl+C to abort at any time)\n")

    # 5. Phase 2 — diff-based narrowing, output suppressed for non-matching candidates
    phase2_narrow(task, candidates, mine, reader)


if __name__ == "__main__":
    main()
