---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "src/wc3bot/core/micro_loop.py"
ref: HEAD
---

"""MicroLoop — 10 Hz unit reactions and hero micro.

Phase 1b: hero skill leveling handled by MacroLoop._try_hero_skills (MICRO priority).
Phase 2:  BM harass, kiting, target-switch implemented here.

With ActionQueue, MicroLoop submits Actions at Priority.MICRO — these preempt
any MACRO or ARMY action between steps in the InputExecutor.
"""
from __future__ import annotations
from typing import TYPE_CHECKING
if TYPE_CHECKING:
    from wc3bot.observe.state import GameState
    from wc3bot.action.input import WC3Input
    from wc3bot.core.coords import Calibration
    from wc3bot.race.base import RaceConfig
    from wc3bot.core.action_queue import ActionQueue


class MicroLoop:
    """High-frequency reaction loop. Hero micro implemented in Phase 2."""

    def __init__(self, inp: "WC3Input", race: "RaceConfig",
                 calib: "Calibration", queue: "ActionQueue | None" = None,
                 dry: bool = False):
        self.inp   = inp
        self.race  = race
        self.calib = calib
        self.dry   = dry
        self._q    = queue

    def run_once(self, state: "GameState", now: float) -> None:
        pass   # Phase 2: BM harass, kiting, target-switch
