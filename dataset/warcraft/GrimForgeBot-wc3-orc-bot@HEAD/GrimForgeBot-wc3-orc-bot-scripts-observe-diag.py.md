---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/observe-diag.py"
ref: HEAD
---

"""Diagnose why scripts/observe-poc.py finds 0 active player slots even
though game.players_count > 0. Dumps the first N player slots' type + a
few key fields regardless of slot status.
"""

from __future__ import annotations

import mmap
import struct
import sys
from typing import Any

from war3structs.observer import ObserverGame, ObserverPlayer

SECTION_NAME = "War3StatsObserverSharedMemory"
GAME_OFFSET = 4
GAME_SIZE = ObserverGame.sizeof()
PLAYER_SIZE = ObserverPlayer.sizeof()
SLOTS_TO_DUMP = 24


def _open(offset: int, size: int) -> mmap.mmap:
    seek = offset % mmap.ALLOCATIONGRANULARITY
    return mmap.mmap(-1, size + seek, SECTION_NAME, offset=offset - seek, access=mmap.ACCESS_READ)


def _read(mm: mmap.mmap, offset: int) -> bytes:
    seek = offset % mmap.ALLOCATIONGRANULARITY
    mm.seek(seek)
    return mm.read()


def main() -> int:
    try:
        mm = _open(GAME_OFFSET, GAME_SIZE)
        game = ObserverGame.parse(_read(mm, GAME_OFFSET))
        mm.close()
    except OSError as exc:
        print(f"Cannot open section: {exc}", file=sys.stderr)
        return 1

    print(f"--- ObserverGame fields ---")
    for k in dir(game):
        if k.startswith("_"):
            continue
        try:
            v = getattr(game, k)
        except Exception:
            continue
        if callable(v):
            continue
        s = repr(v)
        if len(s) > 120:
            s = s[:120] + "..."
        print(f"  {k} = {s}")

    print(f"\n--- First {SLOTS_TO_DUMP} player slots (raw type field) ---")
    for i in range(SLOTS_TO_DUMP):
        offset = GAME_OFFSET + GAME_SIZE + PLAYER_SIZE * i
        mm = _open(offset, PLAYER_SIZE)
        try:
            player = ObserverPlayer.parse(_read(mm, offset))
        except Exception as exc:
            print(f"slot {i}: parse error {exc!r}")
            mm.close()
            continue
        finally:
            pass

        # Dump common identification fields no matter what type says
        try:
            t = player.type
        except Exception as exc:
            t = f"<err {exc!r}>"
        try:
            name = getattr(player, "name", "?")
        except Exception:
            name = "?"
        try:
            race = getattr(player, "race", "?")
        except Exception:
            race = "?"
        try:
            gold = getattr(player, "gold", "?")
        except Exception:
            gold = "?"
        try:
            lumber = getattr(player, "lumber", "?")
        except Exception:
            lumber = "?"

        print(f"slot {i}: type={t!r:>20}  race={race!r:>12}  name={name!r:<20}  gold={gold}  lumber={lumber}")
        mm.close()

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
