---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "src/wc3bot/bot.py"
ref: HEAD
---

"""WC3Bot — top-level orchestrator."""
from __future__ import annotations
import sys, time
sys.path.insert(0, "src")

from wc3bot.observe.state import parse_state, fmt_state, GameState
from wc3bot.observe.sidecar_reader import SidecarScanner
from wc3bot.action.input import WC3Input, screen_size
from wc3bot.core.coords import Calibration, ISO_H, ISO_V
from wc3bot.core.process import get_pid, get_task
from wc3bot.core.action_queue import ActionQueue
from wc3bot.core.input_executor import InputExecutor
from wc3bot.core.macro_loop import MacroLoop
from wc3bot.core.micro_loop import MicroLoop
from wc3bot.race.base import RaceConfig

# Optional GrimForge content pipeline — graceful no-op if not configured
try:
    sys.path.insert(0, ".")
    from tools.shorts_pipeline.session import ShortsSession
    _SHORTS_AVAILABLE = True
except Exception:
    _SHORTS_AVAILABLE = False

MICRO_HZ    = 10     # target frequency for micro loop
MACRO_EVERY = 5      # run macro every N micro ticks (~0.5 s)


class WC3Bot:
    def __init__(self, race: RaceConfig, dry: bool = False, record: bool = False):
        self.race = race
        self.dry  = dry
        self._session = ShortsSession(enabled=record and _SHORTS_AVAILABLE) if _SHORTS_AVAILABLE else None
        pid  = get_pid()
        task = get_task(pid)
        print(f"Attached to Warcraft III  PID={pid}")
        self.inp     = WC3Input(pid=pid)
        self.scanner = SidecarScanner(task)
        sw, sh       = screen_size()
        print(f"Screen: {sw}×{sh}")

    def wait_for_game(self) -> GameState:
        print("Waiting for live game… (pressing SPACE to skip loading screen)")
        raw = None
        while raw is None:
            if not self.dry:
                self.inp.activate()
                self.inp.key("SPACE")
            raw = self.scanner.find_live_game(verbose=True)
            if raw is None:
                time.sleep(2)
        state = parse_state(raw)
        print(f"Game found: {fmt_state(state)}")
        return state

    def setup(self, state: GameState) -> Calibration:
        if self.dry:
            sw, sh = screen_size()
            return Calibration(0, 0, sw // 2, sh // 2, None, None)
        calib = Calibration.__new__(Calibration)  # will be built in race.setup
        # race.setup does the full setup_groups + setup_initial_build
        # and returns nothing; it mutates inp/WC3 state
        # We build a temporary calib first so race.setup can use it
        from wc3bot.core.process import gh_click_pos
        from wc3bot.core.coords import find_nearest_mine, game_to_screen
        cx, cy = gh_click_pos()
        gh_gx  = state.gh_x or 0
        gh_gy  = state.gh_y or 0
        mine   = find_nearest_mine(state, gh_gx, gh_gy)
        if mine:
            mine_sx, mine_sy = game_to_screen(mine.x, mine.y,
                                              gh_gx, gh_gy, cx, cy,
                                              ISO_H, ISO_V)
        else:
            mine_sx, mine_sy = None, None
        calib = Calibration(gh_gx, gh_gy, cx, cy,
                            mine_sx, mine_sy, ISO_H, ISO_V)
        self.race.setup(self.inp, calib, state)
        return calib

    def run(self) -> None:
        state = self.wait_for_game()
        if self._session:
            self._session.on_game_start()
        calib = self.setup(state)

        # ── Action pipeline ───────────────────────────────────────────────────
        queue    = ActionQueue() if not self.dry else None
        executor = InputExecutor(queue) if queue is not None else None
        if executor is not None:
            executor.start()
            print("InputExecutor started.")

        macro = MacroLoop(self.inp, self.race, calib, queue=queue, dry=self.dry)
        micro = MicroLoop(self.inp, self.race, calib, queue=queue, dry=self.dry)

        print(f"\n{'[DRY RUN] ' if self.dry else ''}Bot running. Ctrl+C to stop.\n")
        time.sleep(1.0)

        tick       = 0
        last_print = 0.0
        interval   = 1.0 / MICRO_HZ

        try:
            while True:
                t0  = time.monotonic()
                raw = self.scanner.hot_poll()
                if raw is None:
                    raw = self.scanner.refresh_live()
                if raw is None:
                    raw = self.scanner.find_live_game(verbose=False)
                if raw is None:
                    print("  [game ended]")
                    if self._session:
                        self._session.on_game_end(result="unknown")
                    break

                state = parse_state(raw)
                if state is None:
                    time.sleep(interval)
                    tick += 1
                    continue

                now = time.time()
                # Decision threads (non-blocking — submit to queue, return immediately)
                micro.run_once(state, now)
                if tick % MACRO_EVERY == 0:
                    macro.run_once(state, now)

                if now - last_print >= 1.0:
                    q_info = f"  queue={queue.qsize()}" if queue else ""
                    ex_info = f"  exec={executor.current_action!r}" if executor else ""
                    print(f"{fmt_state(state)}{q_info}{ex_info}")
                    last_print = now

                tick += 1
                elapsed = time.monotonic() - t0
                time.sleep(max(0, interval - elapsed))

        except KeyboardInterrupt:
            print("\nStopped.")
            if self._session:
                self._session.on_game_end(result="unknown")
        finally:
            if executor is not None:
                executor.stop()
