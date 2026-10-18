---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "src/wc3bot/observe/sidecar_file.py"
ref: HEAD
---

"""wc3bot.observe.sidecar_file — Read WC3 sidecar state from the .pld file.

The Lua sidecar calls PreloadGenEnd every 100 ms, which overwrites
  ~/Documents/Warcraft III/CustomMapData/wc3_orc_sidecar/sidecar.pld

Reading a file is instant and completely reliable — no memory scanning,
no mach_vm_read, no Lua-GC stale strings, no WC3 pause risk.

Public API
----------
read_once()   -> GameState | None   (single read, <1 ms)
wait_live()   -> GameState | None   (poll until T advances = live game)
poll_loop()   -> iterator of GameState  (yields every new state)
"""
from __future__ import annotations
import re
import time
from pathlib import Path
from typing import Iterator

from wc3bot.observe.state import SIDECAR_RE, parse_state, GameState

# macOS path where WC3 writes the sidecar file via PreloadGenEnd.
SIDECAR_PATH: Path = (
    Path.home()
    / "Documents"
    / "Warcraft III"
    / "CustomMapData"
    / "wc3_orc_sidecar"
    / "sidecar.pld"
)


def read_once(path: Path = SIDECAR_PATH) -> GameState | None:
    """
    Read and parse the sidecar file once.

    Returns a GameState on success, None if the file is missing, unreadable,
    or does not yet contain a valid sidecar line.
    """
    try:
        data = path.read_bytes()
    except OSError:
        return None
    m = SIDECAR_RE.search(data)
    if m is None:
        return None
    return parse_state(m.group(0))


def wait_live(
    path: Path = SIDECAR_PATH,
    timeout: float = 60.0,
    poll: float = 0.15,
    verbose: bool = True,
) -> GameState | None:
    """
    Block until a live game is detected (T value advances between two reads),
    then return the latest GameState.  Returns None on timeout.

    Detection: read T1, wait ~300 ms, read T2.  If T2 > T1 + 0.05 the Lua
    timer is running → live game.  Uses only two file reads — no memory scan.
    """
    deadline = time.time() + timeout

    if verbose:
        print("  Waiting for live game (file)…", flush=True)

    while time.time() < deadline:
        s1 = read_once(path)
        if s1 is None:
            time.sleep(poll)
            continue

        time.sleep(0.30)   # Lua writes every 0.1 s → expect ≥ 0.1 s advance

        s2 = read_once(path)
        if s2 is not None and s2.time > s1.time + 0.05:
            if verbose:
                print(f"  Live! T={s2.time:.1f}", flush=True)
            return s2

        time.sleep(poll)

    if verbose:
        print("  Timeout — no live game found.", flush=True)
    return None


def poll_loop(
    path: Path = SIDECAR_PATH,
    interval: float = 0.1,
) -> Iterator[GameState]:
    """
    Yield a new GameState every time T advances.  Runs forever; caller breaks.

    Usage:
        for state in poll_loop():
            print(state.gold)
    """
    last_t = -1.0
    while True:
        state = read_once(path)
        if state is not None and state.time > last_t:
            last_t = state.time
            yield state
        time.sleep(interval)
