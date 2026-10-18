---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "tools/shorts_pipeline/obs_controller.py"
ref: HEAD
---

"""OBS WebSocket controller — start/stop recording, switch scenes.

Requires:  pip install obsws-python
OBS setup: Tools > WebSocket Server Settings > Enable (port 4455, set password)

Usage from bot:
    from tools.shorts_pipeline.obs_controller import OBSController
    obs = OBSController()
    obs.start_recording(scene="WC3 Gameplay")
    ...
    path = obs.stop_recording()   # returns output file path
"""
from __future__ import annotations
import time


class OBSController:
    """Thin wrapper around obsws-python for GrimForge pipeline."""

    def __init__(self, host: str = "localhost", port: int = 4455, password: str = "") -> None:
        try:
            import obsws_python as obs  # type: ignore
            self._cl = obs.ReqClient(host=host, port=port, password=password, timeout=5)
            self._available = True
            print(f"  [OBS] connected to {host}:{port}")
        except Exception as e:
            self._cl = None
            self._available = False
            print(f"  [OBS] not available ({e}) — recording disabled")

    @property
    def available(self) -> bool:
        return self._available

    def switch_scene(self, scene: str) -> None:
        if not self._available:
            return
        try:
            self._cl.set_current_program_scene(scene)
        except Exception as e:
            print(f"  [OBS] scene switch failed: {e}")

    def start_recording(self, scene: str | None = None) -> None:
        if not self._available:
            return
        try:
            if scene:
                self.switch_scene(scene)
            self._cl.start_record()
            print("  [OBS] recording started")
        except Exception as e:
            print(f"  [OBS] start_record failed: {e}")

    def stop_recording(self) -> str | None:
        """Stop recording. Returns the output file path or None."""
        if not self._available:
            return None
        try:
            resp = self._cl.stop_record()
            path = getattr(resp, "output_path", None)
            print(f"  [OBS] recording saved → {path}")
            return path
        except Exception as e:
            print(f"  [OBS] stop_record failed: {e}")
            return None

    def get_record_status(self) -> dict:
        if not self._available:
            return {"active": False}
        try:
            resp = self._cl.get_record_status()
            return {"active": resp.output_active, "paused": resp.output_paused}
        except Exception:
            return {"active": False}
