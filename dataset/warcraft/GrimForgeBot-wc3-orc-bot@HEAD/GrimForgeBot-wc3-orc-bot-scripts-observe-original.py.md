---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/observe-original.py"
ref: HEAD
---

"""Canary: call war3observer's own Game.update() against the live WC3
match and dump the resulting state. If THIS returns empty players too,
the observer API itself is regressed on Reforged 2.0+ (not our bug).
"""

from __future__ import annotations

import json
import sys
import time
from pathlib import Path

# war3observer source lives at C:\tools\war3observer\war3observer
sys.path.insert(0, r"C:\tools\war3observer")

try:
    from war3observer.game import Game
except Exception as exc:
    print(f"could not import war3observer.game: {exc}", file=sys.stderr)
    raise SystemExit(2)


def _short(obj, max_chars=400):
    s = json.dumps(obj, default=str, indent=2)
    if len(s) > max_chars:
        return s[:max_chars] + "\n  ... (truncated)"
    return s


def main() -> int:
    game = Game()
    print("Game() constructed (refresh_rate handshake sent).")
    print("Waiting 3s for WC3 to populate the section, then calling update() 3x.\n")
    time.sleep(3)

    for i in range(3):
        state = game.update()
        n_players = len(state.get("players", []))
        in_game = state.get("game", {}).get("is_in_game", "?")
        game_time = state.get("game", {}).get("game_time", "?")
        print(f"--- update() #{i+1}: in_game={in_game}  game_time={game_time}  players={n_players}")
        print(_short(state, max_chars=800))
        print()
        time.sleep(1)

    game.close()
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
