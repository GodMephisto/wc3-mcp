---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "tools/shorts_pipeline/session.py"
ref: HEAD
---

"""ShortsSession — wraps a bot game session for GrimForge pipeline.

Handles:
- OBS recording start/stop
- Bot log capture to file
- Session directory creation
- game_result.txt writing

Usage in bot.py:
    from tools.shorts_pipeline.session import ShortsSession
    session = ShortsSession()
    session.on_game_start()    # after wait_for_game() returns
    # ... bot runs, logs go to session.log_path ...
    session.on_game_end(result="win")
    # pipeline.py can then process session.session_dir
"""
from __future__ import annotations
import sys
import io
import time
import datetime
from pathlib import Path

from tools.shorts_pipeline.config import (
    RECORDINGS_DIR, OBS_HOST, OBS_PORT, OBS_PASSWORD, OBS_SCENE_GAMEPLAY
)


class _TeeLogger:
    """Writes to both original stdout and a log file simultaneously."""

    def __init__(self, log_path: Path) -> None:
        self._orig   = sys.stdout
        self._file   = open(log_path, "w", buffering=1, encoding="utf-8")

    def write(self, data: str) -> int:
        self._orig.write(data)
        self._file.write(data)
        return len(data)

    def flush(self) -> None:
        self._orig.flush()
        self._file.flush()

    def restore(self) -> None:
        sys.stdout = self._orig
        self._file.close()

    # Make it behave like a real file object
    def isatty(self) -> bool:
        return False

    def fileno(self):
        return self._orig.fileno()


class ShortsSession:
    """Manages one bot game session for the GrimForge content pipeline."""

    def __init__(self, enabled: bool = True) -> None:
        self.enabled      = enabled
        self._obs         = None
        self._tee         = None
        self.session_dir  = None
        self.log_path     = None
        self.recording_path: str | None = None
        self._start_ts    = None

    def on_game_start(self) -> None:
        if not self.enabled:
            return

        ts = datetime.datetime.now().strftime("%Y-%m-%d_%H-%M-%S")
        self.session_dir = RECORDINGS_DIR / ts
        self.session_dir.mkdir(parents=True, exist_ok=True)
        self.log_path    = self.session_dir / "bot_run.log"
        self._start_ts   = time.time()

        # Redirect stdout to tee logger
        self._tee = _TeeLogger(self.log_path)
        sys.stdout = self._tee
        print(f"  [Session] logging to {self.log_path}")

        # Start OBS
        try:
            from tools.shorts_pipeline.obs_controller import OBSController
            self._obs = OBSController(
                host=OBS_HOST, port=OBS_PORT, password=OBS_PASSWORD
            )
            self._obs.start_recording(scene=OBS_SCENE_GAMEPLAY)
        except Exception as e:
            print(f"  [Session] OBS init failed: {e}")
            self._obs = None

    def on_game_end(self, result: str = "unknown") -> None:
        """Call when game loop exits. result = 'win' | 'loss' | 'draw' | 'unknown'."""
        if not self.enabled or self.session_dir is None:
            return

        # Write game result
        result_file = self.session_dir / "game_result.txt"
        result_file.write_text(result)
        print(f"  [Session] game result: {result}")

        # Stop OBS
        if self._obs is not None:
            rec_path = self._obs.stop_recording()
            if rec_path:
                self.recording_path = rec_path
                # Write path so pipeline.py can find it
                (self.session_dir / "recording_path.txt").write_text(rec_path)

        # Restore stdout
        if self._tee is not None:
            self._tee.restore()
            self._tee = None

        duration = time.time() - self._start_ts if self._start_ts else 0
        print(f"  [Session] ended after {duration:.0f}s → {self.session_dir}")
        print(f"  [Session] run pipeline: python tools/shorts_pipeline/pipeline.py {self.session_dir}")
