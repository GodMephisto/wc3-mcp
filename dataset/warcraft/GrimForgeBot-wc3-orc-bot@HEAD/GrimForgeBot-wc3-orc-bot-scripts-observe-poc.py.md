---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/observe-poc.py"
ref: HEAD
---

"""P0.5 spike: minimal Warcraft III observer-shared-memory reader.

Opens the named section "War3StatsObserverSharedMemory" that Warcraft III
publishes during a live match, activates the observer API by writing a
refresh rate, and streams a few summary fields (game time, player count,
each player's gold/lumber/supply) to stdout once per second.

Run while a Warcraft III Custom Game vs CPU is in progress. The script
exits cleanly on Ctrl+C. Read-only with respect to game state — the
only write is the refresh-rate handshake required to start the API.
"""

from __future__ import annotations

import mmap
import struct
import sys
import time
from typing import Any

from war3structs.observer import ObserverGame, ObserverPlayer  # type: ignore[import-not-found]

SECTION_NAME = "War3StatsObserverSharedMemory"
GAME_OFFSET = 4  # first 4 bytes carry the refresh-rate handshake
REFRESH_RATE_MS = 1000
GAME_SIZE = ObserverGame.sizeof()
PLAYER_SIZE = ObserverPlayer.sizeof()
MAX_PLAYER_SLOTS = 24


def _open_section(offset: int, size: int, write: bool = False) -> mmap.mmap:
    """Open a view of the named section that respects ALLOCATIONGRANULARITY."""
    seek_offset = offset % mmap.ALLOCATIONGRANULARITY
    return mmap.mmap(
        -1,
        size + seek_offset,
        SECTION_NAME,
        offset=offset - seek_offset,
        access=mmap.ACCESS_WRITE if write else mmap.ACCESS_READ,
    )


def _read_view(mm: mmap.mmap, offset: int) -> bytes:
    seek_offset = offset % mmap.ALLOCATIONGRANULARITY
    mm.seek(seek_offset)
    return mm.read()


def activate(refresh_rate_ms: int = REFRESH_RATE_MS) -> None:
    """Tell Warcraft III to start publishing observer data at the given rate."""
    mm = _open_section(GAME_OFFSET, 4, write=True)
    mm.seek(GAME_OFFSET % mmap.ALLOCATIONGRANULARITY)
    mm.write(struct.pack("<I", refresh_rate_ms))
    mm.close()


def read_game() -> Any:
    mm = _open_section(GAME_OFFSET, GAME_SIZE)
    try:
        return ObserverGame.parse(_read_view(mm, GAME_OFFSET))
    finally:
        mm.close()


def read_players(count: int) -> list[Any]:
    players: list[Any] = []
    for i in range(MAX_PLAYER_SLOTS):
        offset = GAME_OFFSET + GAME_SIZE + PLAYER_SIZE * i
        mm = _open_section(offset, PLAYER_SIZE)
        try:
            player = ObserverPlayer.parse(_read_view(mm, offset))
        finally:
            mm.close()
        if player.type in ("PLAYER", "COMPUTER"):
            players.append(player)
            if len(players) >= count:
                break
    return players


def _summarise_player(p: Any) -> str:
    name = getattr(p, "name", "?") or "?"
    return (
        f"{p.race.ljust(7)} {name[:14].ljust(14)}"
        f" gold={p.gold:>5} lumber={p.lumber:>5} food={p.food_used}/{p.food_cap}"
    )


def main() -> int:
    try:
        activate()
    except OSError as exc:
        print(f"Cannot open section '{SECTION_NAME}': {exc}", file=sys.stderr)
        print("Is Warcraft III running?", file=sys.stderr)
        return 1

    print(f"Activated observer API (refresh={REFRESH_RATE_MS} ms). Ctrl+C to stop.")
    print(f"Game struct size = {GAME_SIZE} bytes, player struct = {PLAYER_SIZE} bytes.\n")

    try:
        while True:
            game = read_game()
            if not game.is_in_game:
                print("[waiting] not in a game yet...", flush=True)
                time.sleep(1.0)
                continue

            players = read_players(game.players_count)
            game_time = getattr(game, "game_time", "?")
            print(f"t={game_time}s  players={len(players)}/{game.players_count}", flush=True)
            for p in players:
                print(f"  {_summarise_player(p)}", flush=True)
            time.sleep(1.0)
    except KeyboardInterrupt:
        print("\nStopping.")
        return 0


if __name__ == "__main__":
    raise SystemExit(main())
