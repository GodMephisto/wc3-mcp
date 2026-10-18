---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "src/wc3bot/core/macro_loop.py"
ref: HEAD
---

"""MacroLoop — 2 Hz build-order and worker-training decisions.

With ActionQueue
────────────────
When a queue is provided (live bot), all inp.* calls are submitted as Action
objects and executed by the InputExecutor thread.  MacroLoop itself never
blocks on input — it returns immediately after submitting.

Without ActionQueue (dry=True or queue=None)
────────────────────────────────────────────
Falls back to the legacy synchronous path for tests and dry runs.
"""
from __future__ import annotations
import time
from typing import TYPE_CHECKING
if TYPE_CHECKING:
    from wc3bot.observe.state import GameState
    from wc3bot.action.input import WC3Input
    from wc3bot.core.coords import Calibration
    from wc3bot.race.base import RaceConfig
    from wc3bot.core.action_queue import ActionQueue


class MacroLoop:
    def __init__(self, inp: "WC3Input", race: "RaceConfig",
                 calib: "Calibration", queue: "ActionQueue | None" = None,
                 dry: bool = False):
        self.inp   = inp
        self.race  = race
        self.calib = calib
        self.dry   = dry
        self._q    = queue   # None → legacy sync path

        self._bo         = race.make_build_order()
        self._action_ts: dict[str, float] = {}
        self._last_train       = 0.0
        self._last_grunt_train = 0.0
        self._last_burrow      = 0.0
        self._barracks_ordered_ts: float | None = None
        self._BO_COOLDOWN      = 5.0
        self._TRAIN_COOLDOWN   = 17.0   # peon train time ~20s; start next 3s early
        self._GRUNT_COOLDOWN   = 32.0   # Grunt training time ~30s
        self._BURROW_COOLDOWN  = 35.0   # Burrow build time ~25s + buffer

    def run_once(self, state: "GameState", now: float) -> None:
        self._try_build_order(state, now)
        self._try_train_worker(state, now)
        self._try_train_army(state, now)
        self._try_food_upgrade(state, now)
        self._try_hero_skills(state, now)

    # ── Build order ────────────────────────────────────────────────────────────

    def _try_build_order(self, state: "GameState", now: float) -> None:
        from wc3bot.race.orc.build_order import Step
        if self._bo.current == Step.DONE:
            return
        action = self._bo.next_action(state)
        if not action:
            return
        if now - self._action_ts.get(action, 0) < self._BO_COOLDOWN:
            return

        self._action_ts[action] = now

        if self.dry:
            print(f"  [DRY] BO: {action}")
        elif self._q is not None:
            act = self.race.make_action(action, self.inp, self.calib)
            if act is not None:
                self._q.submit(act)
        else:
            self.race.execute_action(action, self.inp, self.calib)

        if action == "build_barracks":
            self._barracks_ordered_ts = now
            print(f"  Barracks ordered at T={now:.1f}; "
                  f"ready ~T={now + self.race.barracks_build_time:.1f}")
        self._bo.advance()

    # ── Worker training ────────────────────────────────────────────────────────

    def _try_train_worker(self, state: "GameState", now: float) -> None:
        from wc3bot.core.action_queue import Action, ActionStep, Priority
        cfg     = self.race
        reserve = self._bo.gold_reserve()
        should  = (
            state.gold - cfg.worker_gold_cost >= reserve
            and state.food_free >= cfg.worker_food_cost
            and state.food_used < cfg.max_workers
            and now - self._last_train >= self._TRAIN_COOLDOWN
        )
        if not should:
            return

        self._last_train = now

        if self.dry:
            print(f"  [DRY] train worker  G={state.gold}  reserve={reserve}"
                  f"  food={state.food_used}/{state.food_cap}")
            return

        print(f"  → train worker  G={state.gold}  reserve={reserve}"
              f"  food={state.food_used}/{state.food_cap}")

        if self._q is not None:
            # Keyboard-only: no activate() needed (CGEventPostToPid = focus-independent)
            self._q.submit(Action(Priority.MACRO, "train_peon", [
                ActionStep(self.inp.key, (cfg.group_hall,), delay_after=0.030),
                ActionStep(self.inp.key, (cfg.hotkey_train_worker,)),
            ]))
        else:
            self.inp.activate()
            self.inp.key(cfg.group_hall)
            import time as _t; _t.sleep(0.030)
            self.inp.key(cfg.hotkey_train_worker)

    # ── Army training ──────────────────────────────────────────────────────────

    def _try_train_army(self, state: "GameState", now: float) -> None:
        """Train Grunts from Barracks once it's built and conditions are met."""
        cfg = self.race
        if not hasattr(cfg, "grunt_gold_cost"):
            return
        if self._barracks_ordered_ts is None:
            return
        if now - self._barracks_ordered_ts < cfg.barracks_build_time:
            return
        if now - self._last_grunt_train < self._GRUNT_COOLDOWN:
            return

        should = (
            state.gold >= cfg.grunt_gold_cost
            and state.food_free >= cfg.grunt_food_cost
        )
        if not should:
            return

        self._last_grunt_train = now

        if self.dry:
            print(f"  [DRY] train grunt  G={state.gold}"
                  f"  food={state.food_used}/{state.food_cap}")
            return

        print(f"  → train grunt  G={state.gold}"
              f"  food={state.food_used}/{state.food_cap}")

        if self._q is not None:
            act = self.race.make_action("train_grunt", self.inp, self.calib)
            if act is not None:
                self._q.submit(act)
        else:
            self.race.execute_action("train_grunt", self.inp, self.calib)

    # ── Food (Burrow) ──────────────────────────────────────────────────────────

    def _try_food_upgrade(self, state: "GameState", now: float) -> None:
        """Build a Burrow (via lumber peon) when food headroom runs low."""
        cfg = self.race
        if not hasattr(cfg, "food_headroom"):
            return
        if self._barracks_ordered_ts is None:
            return
        if state.food_cap >= cfg.food_cap_max:
            return
        if state.food_free > cfg.food_headroom:
            return
        if state.lumber < cfg.burrow_lumber_cost:
            return
        if now - self._last_burrow < self._BURROW_COOLDOWN:
            return

        self._last_burrow = now

        if self.dry:
            print(f"  [DRY] build_food_burrow  food={state.food_used}/{state.food_cap}"
                  f"  lumber={state.lumber}")
            return

        print(f"  → build_food_burrow  food={state.food_used}/{state.food_cap}"
              f"  lumber={state.lumber}")

        if self._q is not None:
            act = self.race.make_action("build_food_burrow", self.inp, self.calib)
            if act is not None:
                self._q.submit(act)
        else:
            self.race.execute_action("build_food_burrow", self.inp, self.calib)

    # ── Hero skills ────────────────────────────────────────────────────────────

    def _try_hero_skills(self, state: "GameState", now: float) -> None:
        """Level up BM following the AMAI skill sequence when skill_points > 0.

        Source: AMAI HeroSkills.txt row 6 (CS→WW→CS→WW→CS→BLADESTORM→WW→MI×3).
        Triggers immediately on detecting skill_points > 0 (no cooldown — leveling
        a hero takes one keypress and should happen as soon as possible).
        """
        from wc3bot.core.action_queue import Action, ActionStep, Priority
        cfg = self.race
        skill_seq = getattr(cfg, "bm_skill_sequence", None)
        if skill_seq is None:
            return
        heroes = getattr(state, "heroes", None)
        if not heroes:
            return
        bm = heroes[0]
        if bm.skill_points <= 0:
            return

        level = bm.level
        if level < 1 or level > len(skill_seq):
            return
        skill = skill_seq[level - 1]
        hotkey = cfg.bm_skill_hotkeys.get(skill) if hasattr(cfg, "bm_skill_hotkeys") else None

        if hotkey is None:
            print(f"  [WARN] BM L{level} {skill}: hotkey not set"
                  f" — configure bm_skill_hotkeys['{skill}'] in config.py")
            return

        if self.dry:
            print(f"  [DRY] hero skill L{level}: {skill} ({hotkey})")
            return

        print(f"  → hero skill L{level}: {skill} ({hotkey})")

        if self._q is not None:
            # Hero skills are MICRO priority — they preempt any queued MACRO action
            self._q.submit(Action(Priority.MICRO, f"hero_skill_L{level}_{skill}", [
                ActionStep(self.inp.key, (cfg.group_hero,), delay_after=0.030),
                ActionStep(self.inp.key, (hotkey,)),
            ]))
        else:
            self.inp.activate()
            self.inp.key(cfg.group_hero)
            import time as _t; _t.sleep(0.030)
            self.inp.key(hotkey)
