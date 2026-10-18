---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/diag_peon_positions.py"
ref: HEAD
---

#!/usr/bin/env python3
"""diag_peon_positions.py — Move cursor to each sidecar peon position (no click).

Uses world_to_screen (same as make_calib mine calculation) for accuracy.

Usage (fresh game, peons idle):
    python3 scripts/diag_peon_positions.py
"""
import sys, time
sys.path.insert(0, "src")

from wc3bot.core.process import get_pid, get_task, gh_click_pos
from wc3bot.observe.lua_reader import LuaStateReader
from wc3bot.observe.state import parse_state
from wc3bot.core.coords import (
    game_to_screen, ISO_H, ISO_V, find_nearest_mine,
    CameraState, world_to_screen,
)
from wc3bot.action.input import WC3Input, screen_size

import wc3bot.race.orc
from wc3bot.race import get_race

pid  = get_pid()
task = get_task(pid)
sc   = LuaStateReader(task, pid=pid)
cfg  = get_race("orc")

if sc.node_addr is None:
    print("[FAIL] bootstrap failed"); sys.exit(1)

raw = sc.hot_poll()
if raw is None:
    print("[FAIL] hot_poll returned None — is game running?"); sys.exit(1)
state = parse_state(raw)
if state is None:
    print("[FAIL] parse_state failed"); sys.exit(1)

peons = [u for u in state.units if u.id == cfg.worker_fcc]
if not peons:
    print("[FAIL] no peons in sidecar state"); sys.exit(1)

cx, cy = gh_click_pos()
gh_gx = state.gh_x or 0
gh_gy = state.gh_y or 0
sw, sh = screen_size()

# Build camera state (same as make_calib)
cam = CameraState.from_game_state(state)
if cam.cam_tx == 0.0 and cam.cam_ty == 0.0 and gh_gx != 0:
    cam = CameraState(cam_tx=float(gh_gx), cam_ty=float(gh_gy),
                      cam_aoa=cam.cam_aoa, cam_fov=cam.cam_fov,
                      cam_dist=cam.cam_dist, cam_rot=cam.cam_rot)

mine = find_nearest_mine(state, gh_gx, gh_gy)

print(f"GH game=({gh_gx},{gh_gy})  gh_click_pos=({cx},{cy})")
print(f"cam: tx={cam.cam_tx:.0f} ty={cam.cam_ty:.0f} rot={cam.cam_rot:.1f} aoa={cam.cam_aoa:.1f} dist={cam.cam_dist:.0f}")
if mine:
    w2s = world_to_screen(mine.x, mine.y, 0.0, cam, sw, sh)
    iso = game_to_screen(mine.x, mine.y, gh_gx, gh_gy, cx, cy, ISO_H, ISO_V)
    print(f"mine:   w2s={w2s}  iso={iso}")

print(f"\nPeons: {len(peons)}")
for i, p in enumerate(peons):
    w2s = world_to_screen(p.x, p.y, 0.0, cam, sw, sh)
    iso = game_to_screen(p.x, p.y, gh_gx, gh_gy, cx, cy, ISO_H, ISO_V)
    print(f"  peon[{i}]: game=({p.x},{p.y})  w2s={w2s}  iso={iso}")

print("\nCountdown 3s — switch to WC3...")
for i in range(3, 0, -1):
    print(f"  {i}..."); time.sleep(1.0)

inp = WC3Input()
inp.activate(); time.sleep(0.2)
inp.key("BACKSPACE"); time.sleep(0.5)

# Re-read after BACKSPACE
raw2 = sc.hot_poll()
if raw2:
    state2 = parse_state(raw2)
    if state2:
        peons2 = [u for u in state2.units if u.id == cfg.worker_fcc]
        cam2 = CameraState.from_game_state(state2)
        if cam2.cam_tx == 0.0 and cam2.cam_ty == 0.0 and gh_gx != 0:
            cam2 = CameraState(cam_tx=float(gh_gx), cam_ty=float(gh_gy),
                               cam_aoa=cam2.cam_aoa, cam_fov=cam2.cam_fov,
                               cam_dist=cam2.cam_dist, cam_rot=cam2.cam_rot)
        if peons2:
            peons = peons2
            cam = cam2

print(f"\nMoving cursor to each peon via world_to_screen (1s each)...")
for i, p in enumerate(peons):
    pos = world_to_screen(p.x, p.y, 0.0, cam, sw, sh)
    if pos is None:
        print(f"  peon[{i}]: world_to_screen returned None — skipping")
        continue
    sx, sy = pos
    print(f"  peon[{i}]: ({sx},{sy})", flush=True)
    inp.move_cursor(sx, sy)
    time.sleep(1.0)

print("Done — did cursor land on peons?")
