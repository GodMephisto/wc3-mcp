---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/diag_peon_offset.py"
ref: HEAD
---

#!/usr/bin/env python3
"""diag_peon_offset.py — Measure world_to_screen offset by reading actual cursor position.

Usage (fresh game, peons idle):
    python3 scripts/diag_peon_offset.py

Steps:
1. Computes peon[0] screen position via world_to_screen
2. Moves cursor there
3. Waits for you to move cursor onto the actual peon
4. Reads cursor position → computes correction offset
5. Shows corrected positions for all peons
"""
import sys, time
sys.path.insert(0, "src")

import subprocess  # noqa (unused after refactor)
from wc3bot.core.process import get_pid, get_task, gh_click_pos
from wc3bot.observe.lua_reader import LuaStateReader
from wc3bot.observe.state import parse_state
from wc3bot.core.coords import CameraState, world_to_screen, find_nearest_mine
from wc3bot.action.input import WC3Input, screen_size, mouse_pos

import wc3bot.race.orc
from wc3bot.race import get_race

def get_mouse_pos():
    return mouse_pos()

pid  = get_pid()
task = get_task(pid)
sc   = LuaStateReader(task, pid=pid)
cfg  = get_race("orc")

raw = sc.hot_poll()
state = parse_state(raw) if raw else None
if not state:
    print("[FAIL] no state"); sys.exit(1)

peons = [u for u in state.units if u.id == cfg.worker_fcc]
if not peons:
    print("[FAIL] no peons"); sys.exit(1)

gh_gx = state.gh_x or 0
gh_gy = state.gh_y or 0
sw, sh = screen_size()
cam = CameraState.from_game_state(state)
if cam.cam_tx == 0.0 and cam.cam_ty == 0.0 and gh_gx != 0:
    cam = CameraState(cam_tx=float(gh_gx), cam_ty=float(gh_gy),
                      cam_aoa=cam.cam_aoa, cam_fov=cam.cam_fov,
                      cam_dist=cam.cam_dist, cam_rot=cam.cam_rot)

p0 = peons[0]
computed = world_to_screen(p0.x, p0.y, 0.0, cam, sw, sh)
if computed is None:
    print("[FAIL] world_to_screen returned None"); sys.exit(1)

print(f"peon[0] computed position: {computed}")
print(f"\nCountdown 3s — switch to WC3...")
for i in range(3, 0, -1):
    print(f"  {i}..."); time.sleep(1.0)

inp = WC3Input()
inp.activate(); time.sleep(0.2)
inp.key("BACKSPACE"); time.sleep(0.5)

# Re-read state after BACKSPACE
raw2 = sc.hot_poll()
if raw2:
    s2 = parse_state(raw2)
    if s2:
        peons2 = [u for u in s2.units if u.id == cfg.worker_fcc]
        cam2 = CameraState.from_game_state(s2)
        if cam2.cam_tx == 0.0 and cam2.cam_ty == 0.0 and gh_gx != 0:
            cam2 = CameraState(cam_tx=float(gh_gx), cam_ty=float(gh_gy),
                               cam_aoa=cam2.cam_aoa, cam_fov=cam2.cam_fov,
                               cam_dist=cam2.cam_dist, cam_rot=cam2.cam_rot)
        if peons2:
            peons = peons2
            p0 = peons[0]
            cam = cam2
            computed = world_to_screen(p0.x, p0.y, 0.0, cam, sw, sh)
            print(f"peon[0] recomputed after BACKSPACE: {computed}")

inp.move_cursor(*computed)
print(f"\nCursor moved to computed peon[0] position {computed}")
print("Now MOVE your cursor onto the actual peon, then press Enter...")
input()

actual = get_mouse_pos()
offset_x = actual[0] - computed[0]
offset_y = actual[1] - computed[1]
print(f"\nActual cursor: {actual}")
print(f"Computed:      {computed}")
print(f"Offset: dx={offset_x:+d}  dy={offset_y:+d}")

print(f"\nCorrected positions for all peons:")
for i, p in enumerate(peons):
    pos = world_to_screen(p.x, p.y, 0.0, cam, sw, sh)
    if pos:
        corrected = (pos[0] + offset_x, pos[1] + offset_y)
        print(f"  peon[{i}]: raw={pos}  corrected={corrected}")

print(f"\nMoving to corrected positions (1s each) — confirm they land on peons...")
inp.activate(); time.sleep(0.5)
for i, p in enumerate(peons):
    pos = world_to_screen(p.x, p.y, 0.0, cam, sw, sh)
    if pos:
        cx = pos[0] + offset_x
        cy = pos[1] + offset_y
        print(f"  peon[{i}]: ({cx},{cy})", flush=True)
        inp.move_cursor(cx, cy)
        time.sleep(1.0)

print(f"\nDone. If positions were correct, add to world_to_screen calibration:")
print(f"  W2S_OFFSET_X = {offset_x}")
print(f"  W2S_OFFSET_Y = {offset_y}")
