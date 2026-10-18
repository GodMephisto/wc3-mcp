---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "src/wc3bot/action/build_order.py"
ref: HEAD
---

"""wc3bot.action.build_order — P1b: Standard Orc Build Order executor.

Build order (based on user specification):

  Setup phase (happens once at game start, before main loop):
    1 start peon  → build Burrow   (Z + A)
    1 start peon  → build Altar    (Z + S)
    3 start peons → gold mine      (right-click)

  Main loop (tracked here):
    1st trained peon  → gold mine  (food_used reaches 6)
    2nd trained peon  → Barracks   (food_used reaches 7, gold ≥ 180)
    3rd+ trained      → lumber     (ongoing)
    When gold ≥ 425   → train Blade Master from Altar

Grid build hotkeys (peon selected → Z opens build menu):
  Altar   : S   Burrow  : A   Barracks: W
"""
import time
from dataclasses import dataclass, field
from enum import Enum, auto


class Step(Enum):
    SEND_5TH_TO_GOLD  = auto()   # 1st trained peon → mine
    BUILD_BARRACKS    = auto()   # 2nd trained peon → build barracks
    TRAIN_HERO        = auto()   # gold ≥ 425 → Blade Master from Altar
    DONE              = auto()


# Peons present at game start (5 for standard Orc)
STARTING_PEONS = 5

# Gold to keep in reserve for each upcoming build step.
# Peon training is suppressed while gold - PEON_COST < reserve.
STEP_GOLD_RESERVE: dict[Step, int] = {
    Step.SEND_5TH_TO_GOLD: 0,    # peon already trained, no reserve needed
    Step.BUILD_BARRACKS:  180,   # save for Barracks while training peons
    Step.TRAIN_HERO:      425,   # save for Blade Master
    Step.DONE:              0,
}


@dataclass
class BuildOrder:
    """Tracks build-order progress and emits actions when conditions are met."""

    current: Step = Step.SEND_5TH_TO_GOLD

    # Timestamps of completed steps (for cooldown / duplicate-guard)
    completed: dict = field(default_factory=dict)

    def next_action(self, state: dict) -> str | None:
        """
        Given the current game state, return the next action name to execute,
        or None if nothing needs to be done right now.

        'trained' = peons trained since game start (food_used - STARTING_PEONS).
        """
        trained = max(0, state["food_used"] - STARTING_PEONS)

        if self.current == Step.SEND_5TH_TO_GOLD:
            # Wait for the 1st trained peon
            if trained >= 1:
                return self._emit("send_5th_peon_to_gold")

        elif self.current == Step.BUILD_BARRACKS:
            # Wait for 2nd trained peon and enough gold
            if trained >= 2 and state["gold"] >= 180:
                return self._emit("build_barracks")

        elif self.current == Step.TRAIN_HERO:
            if state["gold"] >= 425:
                return self._emit("train_hero")

        return None

    def _emit(self, action: str) -> str:
        self.completed[self.current] = time.time()
        return action

    def advance(self):
        """Move to the next build order step."""
        order = list(Step)
        idx = order.index(self.current)
        if idx + 1 < len(order):
            self.current = order[idx + 1]

    def gold_reserve(self) -> int:
        """Gold to keep in reserve for the current build step."""
        return STEP_GOLD_RESERVE.get(self.current, 0)

    def already_done(self, step: Step) -> bool:
        return step in self.completed

    def status(self) -> str:
        return f"BO step={self.current.name}"
