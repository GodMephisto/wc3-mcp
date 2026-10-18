---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/bot.py"
ref: HEAD
---

#!/usr/bin/env python3
"""bot.py — WC3 Orc Bot entry point.

Usage:
    python3 scripts/bot.py              # full bot (Orc race)
    python3 scripts/bot.py --dry        # observe only, no key presses
    python3 scripts/bot.py --race orc   # explicit race selection
"""
import argparse, sys
sys.path.insert(0, "src")

import wc3bot.race.orc          # registers OrcConfig automatically
from wc3bot.race import get_race
from wc3bot.bot  import WC3Bot


def main() -> int:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--dry",  action="store_true",
                    help="Observe only — no key presses")
    ap.add_argument("--race", default="orc",
                    help="Race to play (default: orc)")
    args = ap.parse_args()

    race = get_race(args.race)
    bot  = WC3Bot(race=race, dry=args.dry)
    bot.run()
    return 0


if __name__ == "__main__":
    sys.exit(main())
