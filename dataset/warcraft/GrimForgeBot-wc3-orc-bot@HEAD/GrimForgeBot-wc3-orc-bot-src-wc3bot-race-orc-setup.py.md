---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "src/wc3bot/race/orc/setup.py"
ref: HEAD
---

"""wc3bot.race.orc.setup — Orc-specific setup and action execution."""
import time
import sys
sys.path.insert(0, "src")
from wc3bot.action.input import WC3Input
from wc3bot.core.coords import (Calibration, find_nearest_mine, game_to_screen,
                                 world_to_screen, CameraState, ISO_H, ISO_V)
from wc3bot.core.process import gh_click_pos, window_bounds


def _gs(state, key):
    """Duck-type helper: get attribute from GameState or dict."""
    return getattr(state, key, None) if hasattr(state, key) else state.get(key)


def _mine_screen_pos(mine_sx: int | None, mine_sy: int | None,
                     gh_sx: int, gh_sy: int,
                     mine_dy: int = 250) -> tuple[int, int]:
    """Return calibrated mine screen position, or GH-relative fallback."""
    if mine_sx is not None and mine_sy is not None:
        print(f"    mine: screen=({mine_sx},{mine_sy})")
        return mine_sx, mine_sy
    sx, sy = gh_sx, gh_sy - mine_dy
    print(f"    mine: fallback screen=({sx},{sy})  (tune mine_dy if wrong)")
    return sx, sy


def orc_setup(inp: WC3Input, calib: Calibration, state, cfg) -> None:
    """
    Combine setup_groups() + setup_initial_build() for Orc race.

    Backspace → camera on GH → select GH (Ctrl+cfg.group_hall) →
    box-select peons (Ctrl+cfg.group_workers).
    Then send individual peons: peon[0] → Burrow, peon[1] → Altar,
    peons[2..N] → mine.

    state: GameState or dict from sidecar.
    cfg: OrcConfig instance (provides hotkeys, offsets, constants).
    """
    # ── Extract unit/building info ─────────────────────────────────────────────
    units = _gs(state, "units") or []
    buildings = _gs(state, "buildings") or []
    neutral = _gs(state, "neutral") or []

    if hasattr(units[0] if units else None, "id"):
        # GameState — UnitRecord objects
        unit_ids = [u.id for u in units]
        bldg_ids = [b.id for b in buildings]
        neut_ids = [n.id for n in neutral]
    else:
        # dict list
        unit_ids = [u["id"] for u in units]
        bldg_ids = [b["id"] for b in buildings]
        neut_ids = [n["id"] for n in neutral]

    print(f"  Sidecar units={unit_ids}  buildings={bldg_ids}  neutral={neut_ids}")

    # GH position: prefer gh_x/gh_y field (from start location), fall back to ogre building.
    gh_gx = _gs(state, "gh_x") or 0
    gh_gy = _gs(state, "gh_y") or 0
    if gh_gx or gh_gy:
        print(f"  GH from start-location: game=({gh_gx},{gh_gy})")
    else:
        # Try to find GH in buildings
        gh_bldg = None
        for b in buildings:
            bid = b.id if hasattr(b, "id") else b["id"]
            if bid == cfg.great_hall_fcc:
                gh_bldg = b
                break
        if gh_bldg:
            if hasattr(gh_bldg, "x"):
                gh_gx, gh_gy = gh_bldg.x, gh_bldg.y
            else:
                gh_gx, gh_gy = gh_bldg["x"], gh_bldg["y"]
            print(f"  GH from buildings: game=({gh_gx},{gh_gy})")
        else:
            print("  [WARN] GH coords unknown — re-inject sidecar.lua")

    cx, cy = gh_click_pos()
    print(f"  GH game=({gh_gx},{gh_gy})  anchor screen=({cx},{cy})")

    # Mine: compute screen position from sidecar game coords + default ISO.
    mine = find_nearest_mine(state, gh_gx, gh_gy) if hasattr(state, "neutral") else None
    if mine is None:
        # dict-based state: manually find nearest mine
        best, best_d = None, float("inf")
        for n in neutral:
            nid = n.id if hasattr(n, "id") else n["id"]
            if nid != cfg.mine_fcc:
                continue
            nx = n.x if hasattr(n, "x") else n["x"]
            ny = n.y if hasattr(n, "y") else n["y"]
            d = (nx - gh_gx) ** 2 + (ny - gh_gy) ** 2
            if d < best_d:
                best_d, best = d, n
        mine = best

    iso_h, iso_v = calib.iso_h, calib.iso_v
    if mine:
        mx = mine.x if hasattr(mine, "x") else mine["x"]
        my = mine.y if hasattr(mine, "y") else mine["y"]
        mine_sx, mine_sy = game_to_screen(mx, my, gh_gx, gh_gy, cx, cy, iso_h, iso_v)
        print(f"  Mine: game=({mx},{my}) → screen=({mine_sx},{mine_sy})")
    else:
        mine_sx = cx
        mine_sy = cy - cfg.mine_dy
        print(f"  [WARN] Mine not in sidecar, fallback screen=({mine_sx},{mine_sy})")

    inp.activate()
    time.sleep(0.3)

    # ── WC3 input: camera + group setup ───────────────────────────────────────
    inp.key("BACKSPACE")
    time.sleep(0.4)

    inp.click(cx, cy)
    time.sleep(0.3)
    inp.ctrl_key(cfg.group_hall)
    time.sleep(0.2)
    print(f"  GH → group {cfg.group_hall}")

    # GH still selected → right-click mine sets the rally point.
    inp.right_click(mine_sx, mine_sy)
    time.sleep(0.2)
    print(f"  GH rally → mine ({mine_sx},{mine_sy})")

    b = window_bounds()
    print(f"  Window bounds: {b}")
    if b:
        wl, wt, wr, wb = b
        bx1, by1 = wl + 20, wt + 40
        bx2, by2 = wr - 20, wb - 80
    else:
        bx1, by1 = cx - 600, cy - 400
        bx2, by2 = cx + 600, cy + 300
    print(f"  Box-select: ({bx1},{by1}) → ({bx2},{by2})")
    inp.drag(bx1, by1, bx2, by2, hold=0.15)
    time.sleep(0.3)
    inp.ctrl_key(cfg.group_workers)
    time.sleep(0.2)
    print(f"  Peons → group {cfg.group_workers}")

    # ── Initial build: individual peon clicks ─────────────────────────────────
    peons = []
    for u in units:
        uid = u.id if hasattr(u, "id") else u["id"]
        if uid == cfg.worker_fcc:
            peons.append(u)

    if len(peons) < 3:
        print(f"  [WARN] Only {len(peons)} peons found, skipping individual setup")
        return

    # After BACKSPACE the camera target is always GH — use gh_gx/gy, not the
    # potentially stale cam_tx/ty from the sidecar read (which may predate BACKSPACE).
    # Preserve aoa/fov/dist/rot from state if available (they don't change).
    _cam_base = CameraState.from_game_state(state) if hasattr(state, "cam_tx") else CameraState()
    cam = CameraState(
        cam_tx=float(gh_gx), cam_ty=float(gh_gy),
        cam_aoa=_cam_base.cam_aoa, cam_fov=_cam_base.cam_fov,
        cam_dist=_cam_base.cam_dist, cam_rot=_cam_base.cam_rot,
    )

    def _peon_screen(p):
        px = p.x if hasattr(p, "x") else p["x"]
        py = p.y if hasattr(p, "y") else p["y"]
        pz = p.z if hasattr(p, "z") else 0.0
        # Use perspective projection (world_to_screen) — ISO is wrong for peons
        # spread across the map (same y-pos error). Verified T04d: w2s PASS.
        result = world_to_screen(px, py, pz, cam)
        if result is not None:
            return result
        # Fallback: should never happen if cam is valid
        from wc3bot.core.coords import unit_to_screen
        return unit_to_screen(px, py, calib.gh_gx, calib.gh_gy)

    ps = [_peon_screen(p) for p in peons[:5]]
    for i, (px, py) in enumerate(ps):
        raw_p = peons[i]
        px_g = raw_p.x if hasattr(raw_p, "x") else raw_p["x"]
        py_g = raw_p.y if hasattr(raw_p, "y") else raw_p["y"]
        print(f"    peon[{i}]  game=({px_g},{py_g})  screen=({px},{py})")

    # ── Peon 0 → Altar (highest priority — BM timing depends on it) ───────────
    print("  Setup: peon[0] → Altar")
    inp.click(*ps[0])
    time.sleep(0.15)
    inp.key(cfg.hotkey_build_menu)   # Z — open build sub-menu
    time.sleep(0.1)
    inp.key(cfg.hotkey_altar)        # W
    time.sleep(0.1)
    ax = calib.gh_sx + cfg.altar_offset[0]
    ay = calib.gh_sy + cfg.altar_offset[1]
    print(f"    Altar placement=({ax},{ay})")
    inp.click(ax, ay)
    time.sleep(0.25)

    # ── Peon 1 → Burrow ───────────────────────────────────────────────────────
    print("  Setup: peon[1] → Burrow")
    inp.click(*ps[1])
    time.sleep(0.15)
    inp.key(cfg.hotkey_build_menu)   # Z
    time.sleep(0.1)
    inp.key(cfg.hotkey_burrow)       # Q
    time.sleep(0.1)
    bx = calib.gh_sx + cfg.burrow_offset[0]
    by = calib.gh_sy + cfg.burrow_offset[1]
    print(f"    Burrow placement=({bx},{by})")
    inp.click(bx, by)
    time.sleep(0.1)
    # Assign Burrow-builder to lumber group — group assign doesn't cancel build
    inp.ctrl_key(cfg.group_lumber)
    print(f"    peon[1] → group {cfg.group_lumber} (lumber)")
    time.sleep(0.25)

    # ── Peons 2..N → mine ─────────────────────────────────────────────────────
    mine_ps = ps[2:]
    print(f"  Setup: peons[2..{len(ps)-1}] → mine")
    inp.click(*mine_ps[0])          # click first mine peon (new selection)
    time.sleep(0.1)
    for mpx, mpy in mine_ps[1:]:
        inp.shift_click(mpx, mpy)   # add remaining mine peons to selection
        time.sleep(0.1)
    print(f"    right-click mine screen=({mine_sx},{mine_sy})")
    inp.right_click(mine_sx, mine_sy)
    time.sleep(0.3)

    # Re-assign group to ONLY the mine peons (peons 0+1 keep build orders).
    inp.ctrl_key(cfg.group_workers)
    time.sleep(0.2)
    print(f"  Mine peons → group {cfg.group_workers}")


# ── Private action handlers ────────────────────────────────────────────────────

def _do_send_peons_to_gold(inp: WC3Input, calib: Calibration, cfg) -> None:
    mx, my = _mine_screen_pos(calib.mine_sx, calib.mine_sy,
                              calib.gh_sx, calib.gh_sy, cfg.mine_dy)
    print(f"  → peons to gold mine  screen=({mx},{my})")
    inp.activate()
    inp.key(cfg.group_workers)       # select all starting peons
    time.sleep(0.15)
    inp.right_click(mx, my)          # right-click mine → gather gold
    time.sleep(0.2)


def _do_build(inp: WC3Input, calib: Calibration, cfg,
              build_hotkey: str, screen_offset: tuple[int, int],
              label: str) -> None:
    """Select workers (group_workers) → Z (build menu) → hotkey → click placement."""
    ox, oy = screen_offset
    tx, ty = calib.gh_sx + ox, calib.gh_sy + oy
    print(f"  → {label}  placement=({tx}, {ty})")
    inp.activate()
    inp.key(cfg.group_workers)       # select all peons
    time.sleep(0.15)
    inp.key(cfg.hotkey_build_menu)   # Z = open build sub-menu
    time.sleep(0.15)
    inp.key(build_hotkey)            # S / A / W
    time.sleep(0.15)
    inp.click(tx, ty)                # place building
    time.sleep(0.3)


def _do_send_to_lumber(inp: WC3Input, calib: Calibration, cfg) -> None:
    """Right-click a lumber spot (lumber_offset from GH) with worker group."""
    lx = calib.gh_sx + cfg.lumber_offset[0]
    ly = calib.gh_sy + cfg.lumber_offset[1]
    print(f"  → lumber  screen=({lx},{ly})")
    inp.activate()
    inp.key(cfg.group_workers)
    time.sleep(0.15)
    inp.right_click(lx, ly)
    time.sleep(0.2)


def _do_build_food_burrow(inp: WC3Input, calib: Calibration, cfg,
                          burrow_idx: int = 1) -> None:
    """Use the lumber peon (group_lumber) to build an additional Burrow for food.

    burrow_idx=1 → burrow2_offset  (room for more if ever needed)
    """
    offsets = [cfg.burrow_offset, cfg.burrow2_offset]
    ox, oy = offsets[min(burrow_idx, len(offsets) - 1)]
    tx, ty = calib.gh_sx + ox, calib.gh_sy + oy
    print(f"  → food Burrow[{burrow_idx}]  placement=({tx},{ty})")
    inp.activate()
    inp.key(cfg.group_lumber)        # select lumber peon
    import time as _t; _t.sleep(0.1)
    inp.key(cfg.hotkey_build_menu)   # Z — open build sub-menu
    _t.sleep(0.15)
    inp.key(cfg.hotkey_burrow)       # A
    _t.sleep(0.15)
    inp.click(tx, ty)
    _t.sleep(0.3)


def _do_send_builder_to_lumber(inp: WC3Input, calib: Calibration, cfg) -> None:
    """Send the lumber-group peon (peon[0] after Burrow) to cut trees."""
    lx = calib.gh_sx + cfg.lumber_offset[0]
    ly = calib.gh_sy + cfg.lumber_offset[1]
    print(f"  → builder to lumber  screen=({lx},{ly})")
    inp.activate()
    inp.key(cfg.group_lumber)        # select lumber peon(s)
    import time as _t; _t.sleep(0.1)
    inp.right_click(lx, ly)         # right-click trees → gather lumber


def _do_train_grunt(inp: WC3Input, calib: Calibration, cfg) -> None:
    """Click Barracks building by screen position, press G to train a Grunt."""
    bx = calib.gh_sx + cfg.barracks_offset[0]
    by = calib.gh_sy + cfg.barracks_offset[1]
    print(f"  → train grunt  barracks_screen=({bx},{by})")
    inp.activate()
    inp.click(bx, by)               # select Barracks by clicking it on screen
    import time as _t; _t.sleep(0.15)
    inp.key(cfg.hotkey_train_grunt) # G = Grunt


def _do_train_hero(inp: WC3Input, calib: Calibration, cfg) -> None:
    """Click where Altar was placed (altar_offset from GH), press hero hotkey."""
    ax = calib.gh_sx + cfg.altar_offset[0]
    ay = calib.gh_sy + cfg.altar_offset[1]
    print(f"  → train hero from Altar  altar_screen=({ax}, {ay})")
    inp.activate()
    inp.click(ax, ay)           # select Altar
    time.sleep(0.2)
    inp.ctrl_key(cfg.group_hero)    # assign to hero group for future use
    time.sleep(0.1)
    inp.key(cfg.hotkey_hero)        # Q = Blade Master (grid: top-left slot on Altar)
    time.sleep(0.2)


def execute_orc_action(action: str, inp: WC3Input,
                       calib: Calibration, cfg) -> None:
    """Dispatch a build-order action string to the correct Orc handler.

    Legacy synchronous path — used by dry-run / tests that don't use the
    ActionQueue.  For the live bot, prefer make_orc_action() + ActionQueue.
    """
    if action == "train_grunt":
        _do_train_grunt(inp, calib, cfg)
    elif action == "send_builder_to_lumber":
        _do_send_builder_to_lumber(inp, calib, cfg)
    elif action == "build_food_burrow":
        _do_build_food_burrow(inp, calib, cfg, burrow_idx=1)
    elif action == "send_5th_peon_to_gold":
        _do_send_peons_to_gold(inp, calib, cfg)
    elif action == "build_barracks":
        _do_build(inp, calib, cfg, cfg.hotkey_barracks,
                  cfg.barracks_offset, "Barracks")
    elif action == "send_to_lumber":
        _do_send_to_lumber(inp, calib, cfg)
    elif action == "train_hero":
        _do_train_hero(inp, calib, cfg)
    else:
        print(f"  [WARN] Unknown BO action: {action!r}")


def make_orc_action(action: str, inp: WC3Input,
                    calib: Calibration, cfg) -> "Action | None":
    """Build an Action object for the given action string.

    Returns None for unknown actions.
    The caller submits the returned Action to an ActionQueue;
    the InputExecutor executes it asynchronously.

    Keyboard-only actions (train_peon, hero_skill) skip inp.activate() —
    CGEventPostToPid reaches the process regardless of window focus.
    Mouse actions (building placement) include inp.activate() as first step.
    """
    from wc3bot.core.action_queue import Action, ActionStep, Priority

    if action == "build_barracks":
        tx = calib.gh_sx + cfg.barracks_offset[0]
        ty = calib.gh_sy + cfg.barracks_offset[1]
        print(f"  → [queue] build_barracks  placement=({tx},{ty})")
        return Action(Priority.MACRO, "build_barracks", [
            ActionStep(inp.activate),
            ActionStep(inp.key, (cfg.group_workers,), delay_after=0.15),
            ActionStep(inp.key, (cfg.hotkey_build_menu,), delay_after=0.15),
            ActionStep(inp.key, (cfg.hotkey_barracks,), delay_after=0.15),
            ActionStep(inp.click, (tx, ty), delay_after=0.3),
        ])

    if action == "train_grunt":
        bx = calib.gh_sx + cfg.barracks_offset[0]
        by = calib.gh_sy + cfg.barracks_offset[1]
        print(f"  → [queue] train_grunt  barracks=({bx},{by})")
        return Action(Priority.ARMY, "train_grunt", [
            ActionStep(inp.activate),
            ActionStep(inp.click, (bx, by), delay_after=0.15),
            ActionStep(inp.key, (cfg.hotkey_train_grunt,)),
        ])

    if action == "build_food_burrow":
        ox, oy = cfg.burrow2_offset
        tx, ty = calib.gh_sx + ox, calib.gh_sy + oy
        print(f"  → [queue] build_food_burrow  placement=({tx},{ty})")
        return Action(Priority.MACRO, "build_food_burrow", [
            ActionStep(inp.activate),
            ActionStep(inp.key, (cfg.group_lumber,), delay_after=0.10),
            ActionStep(inp.key, (cfg.hotkey_build_menu,), delay_after=0.15),
            ActionStep(inp.key, (cfg.hotkey_burrow,), delay_after=0.15),
            ActionStep(inp.click, (tx, ty), delay_after=0.3),
        ])

    if action == "send_5th_peon_to_gold":
        mx, my = _mine_screen_pos(calib.mine_sx, calib.mine_sy,
                                   calib.gh_sx, calib.gh_sy, cfg.mine_dy)
        print(f"  → [queue] send_5th_peon_to_gold  mine=({mx},{my})")
        return Action(Priority.ECONOMY, "send_5th_peon_to_gold", [
            ActionStep(inp.key, (cfg.group_workers,), delay_after=0.15),
            ActionStep(inp.right_click, (mx, my)),
        ])

    if action == "send_to_lumber":
        lx = calib.gh_sx + cfg.lumber_offset[0]
        ly = calib.gh_sy + cfg.lumber_offset[1]
        print(f"  → [queue] send_to_lumber  screen=({lx},{ly})")
        return Action(Priority.ECONOMY, "send_to_lumber", [
            ActionStep(inp.key, (cfg.group_workers,), delay_after=0.15),
            ActionStep(inp.right_click, (lx, ly)),
        ])

    if action == "send_builder_to_lumber":
        lx = calib.gh_sx + cfg.lumber_offset[0]
        ly = calib.gh_sy + cfg.lumber_offset[1]
        print(f"  → [queue] send_builder_to_lumber  screen=({lx},{ly})")
        return Action(Priority.ECONOMY, "send_builder_to_lumber", [
            ActionStep(inp.key, (cfg.group_lumber,), delay_after=0.10),
            ActionStep(inp.right_click, (lx, ly)),
        ])

    if action == "train_hero":
        ax = calib.gh_sx + cfg.altar_offset[0]
        ay = calib.gh_sy + cfg.altar_offset[1]
        print(f"  → [queue] train_hero  altar=({ax},{ay})")
        return Action(Priority.MACRO, "train_hero", [
            ActionStep(inp.activate),
            ActionStep(inp.click, (ax, ay), delay_after=0.2),
            ActionStep(inp.ctrl_key, (cfg.group_hero,), delay_after=0.1),
            ActionStep(inp.key, (cfg.hotkey_hero,), delay_after=0.2),
        ])

    print(f"  [WARN] make_orc_action: unknown action {action!r}")
    return None
