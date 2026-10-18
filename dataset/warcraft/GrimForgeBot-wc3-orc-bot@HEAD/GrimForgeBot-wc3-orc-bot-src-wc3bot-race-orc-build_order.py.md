---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "src/wc3bot/race/orc/build_order.py"
ref: HEAD
---

"""wc3bot.race.orc.build_order — P1b: Standard Orc Build Order executor.

Build order (based on user specification):

  Setup phase (happens once at game start, before main loop):
    1 start peon  → build Burrow   (Z + A) — assigned to group_lumber after placing
    1 start peon  → build Altar    (Z + S)
    3 start peons → gold mine      (right-click)

  Main loop (tracked here):
    T ≥ 35s       → builder peon → lumber (group_lumber → right-click trees)
    1st trained   → gold mine  (food_used reaches 6)
    2nd trained   → Barracks   (food_used reaches 7, gold ≥ 180, lumber ≥ 50)
    gold ≥ 425    → train Blade Master from Altar

Grid build hotkeys (peon selected → Z opens build menu):
  Altar   : S   Burrow  : A   Barracks: W
"""
import time
from dataclasses import dataclass, field
from enum import Enum, auto


class Step(Enum):
    SEND_BUILDER_TO_LUMBER = auto()  # Burrow complete → lumber peon starts cutting
    SEND_5TH_TO_GOLD       = auto()  # 1st trained peon → mine
    BUILD_BARRACKS         = auto()  # 2nd trained peon → build barracks
    TRAIN_HERO             = auto()  # gold≥425 + lumber≥100 + Altar done → Blade Master
    DONE                   = auto()


# WC3 building FCCs needed for completion detection
_FCC_BURROW = "ostr"
_FCC_ALTAR  = "oalt"


# Peons present at game start (5 for standard Orc)
STARTING_PEONS = 5

# Gold to keep in reserve for each upcoming build step.
# Peon training is suppressed while gold - PEON_COST < reserve.
STEP_GOLD_RESERVE: dict[Step, int] = {
    Step.SEND_BUILDER_TO_LUMBER: 0,
    Step.SEND_5TH_TO_GOLD:       0,   # peon already trained, no reserve needed
    Step.BUILD_BARRACKS:       180,   # save for Barracks while training peons
    Step.TRAIN_HERO:           425,   # save for Blade Master
    Step.DONE:                   0,
}

# Lumber needed before BUILD_BARRACKS fires (Barracks costs 50 lumber)
BARRACKS_LUMBER_COST = 50

# Seconds after game start before sending builder to lumber
# (Burrow build time ≈ 30s — wait a bit longer for safety)
LUMBER_SEND_TIME = 35.0


@dataclass
class OrcBuildOrder:
    """Tracks build-order progress and emits actions when conditions are met."""

    current: Step = Step.SEND_BUILDER_TO_LUMBER

    # Timestamps of completed steps (for cooldown / duplicate-guard)
    completed: dict = field(default_factory=dict)

    def next_action(self, state) -> str | None:
        """
        Given the current game state, return the next action name to execute,
        or None if nothing needs to be done right now.

        Accepts both dict and GameState via duck-typing.
        'trained' = peons trained since game start (food_used - STARTING_PEONS).
        """
        if hasattr(state, 'food_used'):
            food_used = state.food_used
            gold      = state.gold
            lumber    = state.lumber
            game_time = state.time
            buildings = state.buildings if hasattr(state, 'buildings') else []
        else:
            food_used = state["food_used"]
            gold      = state["gold"]
            lumber    = state.get("lumber", 0)
            game_time = state.get("time", 0.0)
            buildings = state.get("buildings", [])

        trained = max(0, food_used - STARTING_PEONS)

        def _bldg_complete(fcc: str) -> bool:
            """True if a building of this type exists and is fully constructed (hp=100)."""
            for b in buildings:
                bid = b.id if hasattr(b, "id") else b.get("id", "")
                bhp = b.hp if hasattr(b, "hp") else b.get("hp", 0)
                if bid == fcc and bhp >= 100:
                    return True
            return False

        if self.current == Step.SEND_BUILDER_TO_LUMBER:
            # Send builder to lumber once Burrow is complete.
            # Fallback: LUMBER_SEND_TIME (50s) if Burrow not yet visible in sidecar.
            if _bldg_complete(_FCC_BURROW) or game_time >= LUMBER_SEND_TIME:
                return self._emit("send_builder_to_lumber")

        elif self.current == Step.SEND_5TH_TO_GOLD:
            if trained >= 1:
                return self._emit("send_5th_peon_to_gold")

        elif self.current == Step.BUILD_BARRACKS:
            if trained >= 2 and gold >= 180 and lumber >= BARRACKS_LUMBER_COST:
                return self._emit("build_barracks")

        elif self.current == Step.TRAIN_HERO:
            # First hero is FREE (0 gold, 0 lumber) in WC3 — only gated by Altar completion.
            if _bldg_complete(_FCC_ALTAR):
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
