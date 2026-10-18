---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "src/wc3bot/race/base.py"
ref: HEAD
---

"""RaceConfig Protocol — duck-typed interface every race must satisfy."""
from __future__ import annotations
from typing import Protocol, runtime_checkable, TYPE_CHECKING, Any
if TYPE_CHECKING:
    from wc3bot.observe.state import GameState
    from wc3bot.action.input import WC3Input
    from wc3bot.core.coords import Calibration


@runtime_checkable
class RaceConfig(Protocol):
    # Identity
    name:             str
    worker_fcc:       str    # FourCC of the worker unit (e.g. "opeo")
    great_hall_fcc:   str    # FourCC of the main building (e.g. "ogre")
    mine_fcc:         str    # FourCC of gold mine (shared: "ngol")

    # Grid hotkeys
    hotkey_train_worker: str   # Key to train worker (GH selected)
    hotkey_build_menu:   str   # Key to open build sub-menu (worker selected)
    hotkey_hero:         str   # Key to train hero (altar selected)

    # Control groups
    group_hall:     str   # e.g. "5"
    group_workers:  str   # e.g. "6"
    group_hero:     str   # e.g. "4"
    group_barracks: str   # e.g. "3"

    # Worker costs
    worker_gold_cost: int
    worker_food_cost: int
    max_workers:      int

    # Army unit costs + timing (optional — MacroLoop checks hasattr)
    grunt_gold_cost:    int
    grunt_food_cost:    int
    barracks_build_time: float   # seconds from build order to ready

    def make_build_order(self) -> Any: ...
    def setup(self, inp: "WC3Input", calib: "Calibration",
              state: "GameState") -> None: ...
    def execute_action(self, action: str, inp: "WC3Input",
                       calib: "Calibration") -> None: ...
    def make_action(self, action: str, inp: "WC3Input",
                    calib: "Calibration") -> Any: ...   # returns Action | None
