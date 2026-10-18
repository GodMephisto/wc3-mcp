---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/test_primitives.py"
ref: HEAD
---

#!/usr/bin/env python3
"""test_primitives.py — Test bot primitives ONE AT A TIME.

Usage:
    python3 scripts/test_primitives.py --list
    python3 scripts/test_primitives.py --test T01_window
    python3 scripts/test_primitives.py --test T05_click_gh

Each test has a 5-second countdown — switch to WC3 before it fires.
Tests that read sidecar require a live WC3 game with sidecar injected.
"""
import argparse, sys, time
sys.path.insert(0, "src")


# ── Countdown ─────────────────────────────────────────────────────────────────

def countdown(n: int = 5) -> None:
    print(f"Switch to WC3 now!", end=" ")
    for i in range(n, 0, -1):
        print(i, end=" ", flush=True)
        time.sleep(1)
    print("GO")


# ── Helpers ───────────────────────────────────────────────────────────────────

def make_inp():
    from wc3bot.core.process import get_pid
    from wc3bot.action.input import WC3Input
    pid = get_pid()
    print(f"WC3 PID: {pid}")
    return WC3Input(pid=pid)


def activate_wc3():
    """Bring WC3 to foreground so game resumes (pauses on focus loss in SP)."""
    import subprocess
    subprocess.run(
        ["osascript", "-e",
         'tell application "System Events" to tell process "Warcraft III"'
         ' to set frontmost to true'],
        capture_output=True,
    )
    time.sleep(1.0)   # WC3 SP needs ~0.5-1s to unpause and write fresh ticks


def make_scanner():
    from wc3bot.core.process import get_pid, get_task
    from wc3bot.observe.lua_reader import LuaStateReader
    pid  = get_pid()
    task = get_task(pid)
    return LuaStateReader(task, pid=pid)


def read_state(scanner=None):
    """Read sidecar state. Creates scanner if not provided."""
    from wc3bot.observe.state import parse_state, fmt_state
    if scanner is None:
        scanner = make_scanner()
    activate_wc3()
    raw = scanner.find_live_game(verbose=True)
    if raw is None:
        print("[FAIL] No live game found — is sidecar injected?")
        return None
    # Use scanner.read_state() so _update_mine_workers (exact memory read) runs.
    # Fall back to parse_state(raw) only if read_state() returns None.
    state = scanner.read_state() or parse_state(raw)
    # find_live_game only re-scans regions from scan1; newly allocated Lua regions
    # (containing the full string with units) may be missed. full_scan covers all.
    if state and len(state.units) == 0 and len(state.buildings) == 0:
        raw2 = scanner.full_scan(verbose=False)
        if raw2:
            s2 = scanner.read_state() or parse_state(raw2)
            if s2 and s2.time >= state.time and (len(s2.units) > 0 or len(s2.buildings) > 0):
                state = s2
    print(f"State: {fmt_state(state)}")
    return state


def make_calib(state):
    from wc3bot.core.process import gh_click_pos
    from wc3bot.core.coords import (
        Calibration, find_nearest_mine, game_to_screen,
        CameraState, world_to_screen, scaled_iso, scaled_render_center,
    )
    cx, cy = gh_click_pos()
    gh_gx = state.gh_x or 0
    gh_gy = state.gh_y or 0
    mine  = find_nearest_mine(state, gh_gx, gh_gy)

    iso_h, iso_v, _ = scaled_iso()
    rcx, rcy = scaled_render_center()
    print(f"[coords] screen-scaled: ISO_H={iso_h:.4f} ISO_V={iso_v:.4f}  RCX={rcx} RCY={rcy}")

    if mine:
        cam = CameraState.from_game_state(state)
        # After BACKSPACE the camera target is always at GH — use as fallback
        # when sidecar didn't emit CAM (race-condition with old string in memory)
        if cam.cam_tx == 0.0 and cam.cam_ty == 0.0 and gh_gx != 0:
            cam = CameraState(
                cam_tx=float(gh_gx), cam_ty=float(gh_gy),
                cam_aoa=cam.cam_aoa, cam_fov=cam.cam_fov,
                cam_dist=cam.cam_dist, cam_rot=cam.cam_rot,
            )
        result = world_to_screen(mine.x, mine.y, 0.0, cam)
        if result is not None:
            mine_sx, mine_sy = result
            print(f"[coords] world_to_screen: cam_tx={cam.cam_tx:.0f} cam_ty={cam.cam_ty:.0f}  mine→({mine_sx},{mine_sy})")
        else:
            mine_sx, mine_sy = game_to_screen(mine.x, mine.y, gh_gx, gh_gy, cx, cy, iso_h, iso_v)
            print(f"[coords] fallback ISO: mine→({mine_sx},{mine_sy})")
    else:
        mine_sx, mine_sy = None, None

    calib = Calibration(gh_gx, gh_gy, cx, cy, mine_sx, mine_sy)
    return calib


# ── Tests ─────────────────────────────────────────────────────────────────────

TESTS = {}

def test(name, desc):
    """Decorator to register a test."""
    def deco(fn):
        TESTS[name] = (desc, fn)
        return fn
    return deco


@test("T00_gamestart", "Full gamestart: attach → SPACE loop → wait for live sidecar → print state")
def t00():
    """
    Run this while WC3 is on the loading screen or main menu.
    Bot will press SPACE every 2s until the sidecar reports a live game.
    No countdown — runs immediately.
    Expected output: game state with gold/food/units once game is live.
    """
    from wc3bot.core.process import get_pid, get_task
    from wc3bot.action.input import WC3Input
    from wc3bot.observe.lua_reader import LuaStateReader
    from wc3bot.observe.state import parse_state, fmt_state

    pid  = get_pid()
    task = get_task(pid)
    print(f"Attached — WC3 PID={pid}")
    inp     = WC3Input(pid=pid)
    scanner = LuaStateReader(task)

    print("Pressing SPACE every 2s until live game detected. Start a game in WC3 now.")
    print("Ctrl+C to abort.\n")
    attempt = 0
    try:
        while True:
            attempt += 1
            inp.activate()
            inp.key("SPACE")
            print(f"  [{attempt}] SPACE sent — polling sidecar...", end=" ", flush=True)
            raw = scanner.find_live_game(verbose=False)
            if raw is not None:
                state = parse_state(raw)
                if state is not None:
                    print("LIVE!")
                    inp.zoom_out(clicks=10)   # zoom out for better overview
                    time.sleep(0.3)
                    print(f"\nGame state: {fmt_state(state)}")
                    print(f"  GH=({state.gh_x},{state.gh_y})")
                    print(f"  units:    {[u.id for u in state.units]}")
                    print(f"  buildings:{[b.id for b in state.buildings]}")
                    print(f"  neutral:  {[n.id for n in state.neutral]}")
                    print("\n[PASS] Gamestart workflow complete.")
                    return
            print("not live yet")
            time.sleep(2)
    except KeyboardInterrupt:
        print("\nAborted.")


@test("T01_window", "Print WC3 window bounds — no countdown needed")
def t01():
    from wc3bot.core.process import window_bounds, gh_click_pos
    b = window_bounds()
    print(f"window_bounds() = {b}")
    if b:
        wl, wt, wr, wb = b
        print(f"  Size: {wr-wl} × {wb-wt}")
    cx, cy = gh_click_pos()
    print(f"gh_click_pos()  = ({cx}, {cy})  ← only valid AFTER pressing BACKSPACE")


@test("T02_screen_size", "Print screen resolution")
def t02():
    from wc3bot.action.input import screen_size
    w, h = screen_size()
    print(f"screen_size() = {w} × {h}")


@test("T03_diag", "Diagnose: show large memory regions being skipped")
def t03_diag():
    from wc3bot.observe.sidecar_reader import iter_regions, best_match_in, read_region, SIDECAR_RE
    sc = make_scanner()
    print("Scanning all readable regions (showing large ones)...")
    skipped_big = []
    found_any = []
    for base, size, prot in iter_regions(sc.task):
        if not (prot & 1):
            continue
        if size > 512 * 1024 * 1024:
            skipped_big.append((base, size))
            continue
        data = read_region(sc.task, base, size)
        hit = best_match_in(data)
        if hit:
            t, m = hit
            found_any.append((base, size, t))

    print(f"\nRegions with sidecar strings: {len(found_any)}")
    for base, size, t in found_any:
        print(f"  base=0x{base:x}  size={size//1024//1024}MB  T={t:.1f}")

    print(f"\nSkipped (>512MB): {len(skipped_big)}")
    for base, size in skipped_big:
        print(f"  base=0x{base:x}  size={size//1024//1024}MB")


@test("T03_sidecar", "Read sidecar state — game must be running with sidecar")
def t03():
    from wc3bot.observe.state import fmt_state
    sc = make_scanner()
    state = read_state(sc)
    if state is None:
        return
    print(f"  gh_x={state.gh_x}  gh_y={state.gh_y}")
    if state.cam_tx is not None:
        print(f"  cam=({state.cam_tx},{state.cam_ty}) aoa={state.cam_aoa} fov={state.cam_fov} dist={state.cam_dist} rot={state.cam_rot}")
        print("  [CAM OK — perspective projection active]")
    else:
        print("  [CAM MISSING — run: python3 scripts/inject_sidecar.py]")
    print(f"  units={[u.id for u in state.units]}")
    print(f"  buildings={[b.id for b in state.buildings]}")
    print(f"  neutral_count={len(state.neutral)}")
    print("[PASS]")


@test("T03b_sidecar_v2", "Dump all v2 sidecar fields: heroes, mine workers, rally, queue, destr, cliff, SEL")
def t03b():
    sc = make_scanner()
    state = read_state(sc)
    if state is None:
        return

    # ── Heroes ──────────────────────────────────────────────────────────────
    if state.heroes:
        print(f"\n  HEROES ({len(state.heroes)}):")
        for h in state.heroes:
            items = [(i.slot, i.id, i.charges) for i in h.items]
            print(f"    {h.id}  L{h.level}  XP={h.xp}  SP={h.skill_points}"
                  f"  HP={h.hp}%  mana={h.mana}/{h.max_mana}  order={h.order}")
            if items:
                print(f"      items: {items}")
    else:
        print("  HEROES: none (no hero on map yet)")

    # ── Own units with orders ───────────────────────────────────────────────
    print(f"\n  OWN UNITS ({len(state.units)}):")
    for u in state.units:
        print(f"    {u.id}  ({u.x},{u.y})  hp={u.hp}%  order={u.order}")

    # ── Buildings: queue + rally ─────────────────────────────────────────────
    print(f"\n  BUILDINGS ({len(state.buildings)}):")
    for b in state.buildings:
        idle = b.queue_id in ("", "0000")
        q = "idle" if idle else f"training={b.queue_id}"
        rp_same = (b.rally_x == b.x and b.rally_y == b.y)
        rp = "(no rally set)" if rp_same else f"rally=({b.rally_x},{b.rally_y})"
        print(f"    {b.id}  ({b.x},{b.y})  hp={b.hp}%  queue={q}  {rp}")

    # ── Neutral: gold mines with worker count ────────────────────────────────
    print(f"\n  NEUTRAL ({len(state.neutral)}):")
    for n in state.neutral:
        if n.id == "ngol":
            total = state.mine_total_workers(n, total_at_start=5)
            print(f"    {n.id}  ({n.x},{n.y})  hp={n.hp}%"
                  f"  gold={n.gold}  workers_near={n.workers}  total_in_cycle={total}/5")
        else:
            print(f"    {n.id}  ({n.x},{n.y})  hp={n.hp}%")

    # ── Orc mine cycle summary ────────────────────────────────────────────────
    visible_peons = len([u for u in state.units if u.id == "opeo"])
    in_mine = state.worker_in_mine(total_at_start=5)
    print(f"\n  PEONS: visible={visible_peons}  worker_in_mine={in_mine}"
          f"  (Orc: max 1 inside; others queue in proximity)")

    # ── Destructibles ────────────────────────────────────────────────────────
    print(f"\n  DESTRUCTIBLES near camera: {len(state.destructibles)}")
    if state.destructibles:
        ids = {}
        for d in state.destructibles:
            ids[d.id] = ids.get(d.id, 0) + 1
        print(f"    types: {ids}")
        print(f"    first 3: {[(d.id, d.x, d.y, d.hp) for d in state.destructibles[:3]]}")

    # ── Cliff samples ────────────────────────────────────────────────────────
    print(f"\n  CLIFF SAMPLES: {len(state.cliff_samples)}")
    if state.cliff_samples:
        levels = set(c.level for c in state.cliff_samples)
        print(f"    distinct levels: {sorted(levels)}")
        high = [c for c in state.cliff_samples if c.level > min(levels)]
        print(f"    high-ground samples: {len(high)}")

    # ── Selected ─────────────────────────────────────────────────────────────
    print(f"\n  SELECTED: {state.selected}")

    print("\n[PASS] — verify counts and values look correct for your spawn")


@test("T04_mine_pos", "Compute mine screen position from sidecar (no click)")
def t04():
    sc    = make_scanner()
    state = read_state(sc)
    if not state:
        return
    calib = make_calib(state)
    from wc3bot.core.process import gh_click_pos
    cx, cy = gh_click_pos()
    print(f"GH  game=({calib.gh_gx},{calib.gh_gy})  screen=({cx},{cy})")
    if calib.mine_sx is not None:
        print(f"Mine screen=({calib.mine_sx},{calib.mine_sy})")
        print("  → verify: does this pixel look like the gold mine?")
    else:
        print("[WARN] mine not found in sidecar neutral list")


@test("T04b_mine_cursor", "Move cursor to mine position (no click) — verify visually in WC3")
def t04b():
    sc    = make_scanner()
    state = read_state(sc)
    if not state:
        return
    from wc3bot.core.coords import find_nearest_mine
    import wc3bot.race.orc
    from wc3bot.race import get_race
    cfg = get_race("orc")

    # Show ALL ngol entries and their distances to GH
    gh_gx, gh_gy = state.gh_x or 0, state.gh_y or 0
    print(f"\nGH game=({gh_gx},{gh_gy})  GH screen=(1280,538)")
    print("All ngol entries:")
    for n in state.neutral:
        if n.id == "ngol":
            d = ((n.x-gh_gx)**2 + (n.y-gh_gy)**2)**0.5
            dx, dy = n.x-gh_gx, n.y-gh_gy
            print(f"  ngol game=({n.x},{n.y})  dx={dx} dy={dy}  dist={d:.0f}")

    mine = find_nearest_mine(state, gh_gx, gh_gy)
    if mine is None:
        print("[FAIL] no ngol found")
        return

    from wc3bot.core.coords import (game_to_screen, world_to_screen,
                                     CameraState, ISO_H, ISO_V,
                                     RENDER_CENTER_X, RENDER_CENTER_Y)
    from wc3bot.core.process import gh_click_pos
    from wc3bot.action.input import screen_size
    cx, cy = gh_click_pos()
    dx, dy = mine.x-gh_gx, mine.y-gh_gy
    print(f"\nNearest mine: game=({mine.x},{mine.y})  dx={dx} dy={dy}  z={mine.z:.1f}")

    # Method 1: ISO (building formula, gh_sy=525 anchor)
    iso_sx, iso_sy = game_to_screen(mine.x, mine.y, gh_gx, gh_gy, cx, cy, ISO_H, ISO_V)
    print(f"ISO    → screen=({iso_sx},{iso_sy})")

    # Method 2: world_to_screen (perspective projection, camera-correct — same as peons T14)
    cam = CameraState.from_game_state(state)
    sw, sh = screen_size()
    w2s = world_to_screen(mine.x, mine.y, mine.z, cam=cam, screen_w=sw, screen_h=sh)
    if w2s:
        print(f"w2s    → screen=({w2s[0]},{w2s[1]})  cam=({cam.cam_tx:.0f},{cam.cam_ty:.0f})"
              f"  dist={cam.cam_dist:.0f}  rot={cam.cam_rot:.1f}°")
    else:
        print("w2s    → None (behind camera?)")

    # Use world_to_screen when CAM data available (same approach as T14 peons PASS 5/5)
    mine_sx, mine_sy = (w2s if w2s else (iso_sx, iso_sy))
    print(f"Using  → screen=({mine_sx},{mine_sy})  ({'w2s' if w2s else 'ISO fallback'})")

    inp = make_inp()
    from wc3bot.core.process import window_bounds
    from wc3bot.action.input import mouse_pos
    wb = window_bounds()
    if wb:
        wl, wt, wr, wb2 = wb
        in_win = (wl <= mine_sx <= wr) and (wt <= mine_sy <= wb2)
        print(f"In window? {'YES' if in_win else 'NO!'}")
    print("BACKSPACE → cursor to mine (3s to check)")
    countdown(3)
    inp.activate()
    inp.key("BACKSPACE")
    time.sleep(0.6)
    # Re-read state after BACKSPACE so CAM reflects new camera position
    raw2 = sc.hot_poll()
    if raw2:
        from wc3bot.observe.state import parse_state
        s2 = parse_state(raw2)
        if s2 and s2.cam_tx is not None:
            cam2 = CameraState.from_game_state(s2)
            w2s2 = world_to_screen(mine.x, mine.y, mine.z, cam=cam2, screen_w=sw, screen_h=sh)
            if w2s2:
                mine_sx, mine_sy = w2s2
                print(f"Re-read after BACKSPACE → cam=({cam2.cam_tx:.0f},{cam2.cam_ty:.0f})"
                      f"  mine_screen=({mine_sx},{mine_sy})")
    inp.move_cursor(mine_sx, mine_sy)
    time.sleep(0.2)
    actual = mouse_pos()
    print(f"Cursor at ({actual[0]},{actual[1]}) — is it on the mine? (3s)")
    time.sleep(3)


@test("T04c_calibrate", "Manual calibration: hover over mine → compute correct ISO_H / ISO_V")
def t04c():
    """
    1. Script presses BACKSPACE to center camera.
    2. Hover cursor over GH center — 5s countdown.
    3. Hover cursor over the mine center — 5s countdown.
    Script reads cursor position both times and computes correct ISO constants.
    No clicking needed — just hover and hold.
    """
    sc    = make_scanner()
    state = read_state(sc)
    if not state:
        return
    from wc3bot.core.coords import find_nearest_mine, ISO_H, ISO_V, scaled_iso
    from wc3bot.core.process import gh_click_pos
    from wc3bot.action.input import mouse_pos

    gh_gx, gh_gy = state.gh_x or 0, state.gh_y or 0
    mine = find_nearest_mine(state, gh_gx, gh_gy)
    if mine is None:
        print("[FAIL] no ngol found in sidecar — run at T>10s")
        return
    dx, dy = mine.x - gh_gx, mine.y - gh_gy
    cx, cy = gh_click_pos()
    print(f"GH game=({gh_gx},{gh_gy})  GH screen approx=({cx},{cy})")
    print(f"Mine game=({mine.x},{mine.y})  dx={dx} dy={dy}")
    print(f"Current ISO: H={ISO_H} V={ISO_V}")

    inp = make_inp()
    # Step 1: center camera
    countdown(3)
    inp.activate()
    inp.key("BACKSPACE")
    time.sleep(0.6)

    # Step 2: measure GH screen position
    print("\nHover cursor over the CENTER of the Great Hall (the building itself).")
    print("Reading in:", end=" ")
    for i in range(5, 0, -1):
        print(i, end=" ", flush=True)
        time.sleep(1)
    print("GO")
    gh_sx, gh_sy = mouse_pos()
    print(f"GH screen measured: ({gh_sx},{gh_sy})  (gh_click_pos had {cx},{cy})")

    # Step 3: measure mine screen position
    print("\nNow hover cursor over the CENTER of the gold mine.")
    print("Reading in:", end=" ")
    for i in range(5, 0, -1):
        print(i, end=" ", flush=True)
        time.sleep(1)
    print("GO")
    mine_sx, mine_sy = mouse_pos()
    print(f"Mine screen measured: ({mine_sx},{mine_sy})")

    # Step 4: compute ISO constants
    sdx = mine_sx - gh_sx
    sdy = mine_sy - gh_sy
    print(f"\nScreen delta: dx={sdx} dy={sdy}")
    if (dx - dy) != 0 and (dx + dy) != 0:
        from wc3bot.action.input import screen_size
        sw, sh = screen_size()
        new_H = sdx / (dx - dy)
        new_V = sdy / (dx + dy)
        # RENDER_CENTER_X ≈ gh_sx (GH is the camera target, approx at render center)
        # RENDER_CENTER_Y ≈ gh_sy + 16px * (sh/1440)  (GH visual center is z≈25; z=0 is 16px lower)
        new_rcx = gh_sx
        new_rcy = gh_sy + int(16 * sh / 1440)
        # Back-convert to 2560×1440 reference values for storage in coords.py
        new_H_ref = new_H * (2560 / sw)
        new_V_ref = new_V * (1440 / sh)
        new_rcx_ref = int(new_rcx * 2560 / sw)
        new_rcy_ref = int(new_rcy * 1440 / sh)
        print(f"\n=== CALIBRATION RESULTS (current screen {sw}×{sh}) ===")
        print(f"GH screen:          ({gh_sx},{gh_sy})")
        print(f"Render center est:  ({new_rcx},{new_rcy})")
        print(f"ISO_H = {new_H:.4f}   ISO_V = {new_V:.4f}")
        print(f"\nUpdate src/wc3bot/core/coords.py (_CALIB_W={sw}, _CALIB_H={sh}):")
        print(f"  _CALIB_W     = {sw}")
        print(f"  _CALIB_H     = {sh}")
        print(f"  ISO_H        = {new_H:.4f}")
        print(f"  ISO_V        = {new_V:.4f}")
        print(f"  RENDER_CENTER_X = {new_rcx}")
        print(f"  RENDER_CENTER_Y = {new_rcy}")
        print(f"\n  (OR keep _CALIB_W=2560 _CALIB_H=1440 and use these reference values:)")
        print(f"  ISO_H        = {new_H_ref:.4f}")
        print(f"  ISO_V        = {new_V_ref:.4f}")
        print(f"  RENDER_CENTER_X = {new_rcx_ref}")
        print(f"  RENDER_CENTER_Y = {new_rcy_ref}")
    else:
        print("[FAIL] dx-dy or dx+dy is zero — can't compute ISO from this spawn")


@test("T04d_verify_coords", "BACKSPACE → cursor moves to each peon (ISO then w2s) — compare visually")
def t04d():
    """
    Moves cursor (no click) to each peon via BOTH ISO and world_to_screen, 1.5s each.
    Shows which formula places the cursor on the peon more accurately.

    First pass: ISO (unit_to_screen)  — linear approximation, accurate near GH+mine
    Second pass: world_to_screen      — proper perspective, accurate at any distance
    """
    sc    = make_scanner()
    state = read_state(sc)
    if not state:
        return
    import wc3bot.race.orc
    from wc3bot.race import get_race
    from wc3bot.observe.state import parse_state
    from wc3bot.core.coords import (unit_to_screen, world_to_screen,
                                    CameraState, RENDER_CENTER_X, RENDER_CENTER_Y)
    cfg = get_race("orc")

    peons = [u for u in state.units if u.id == cfg.worker_fcc]
    if not peons:
        print("[FAIL] no peons in sidecar"); return

    inp = make_inp()
    countdown(3)
    inp.activate()
    inp.key("BACKSPACE"); time.sleep(0.6)

    activate_wc3()
    raw = sc.find_live_game(verbose=False)
    state2 = parse_state(raw) if raw else state
    peons2 = [u for u in state2.units if u.id == cfg.worker_fcc] if state2 else peons

    gh_gx = state2.gh_x or (state.gh_x or 0)
    gh_gy = state2.gh_y or (state.gh_y or 0)
    cam   = CameraState.from_game_state(state2)
    use_w2s = state2.cam_tx is not None

    print(f"render_center=({RENDER_CENTER_X},{RENDER_CENTER_Y})  GH=({gh_gx},{gh_gy})")
    if use_w2s:
        print(f"cam: tx={cam.cam_tx} ty={cam.cam_ty}  aoa={cam.cam_aoa:.4f}°  "
              f"fov={cam.cam_fov:.4f}°  dist={cam.cam_dist:.2f}  rot={cam.cam_rot:.4f}°")
        # Self-check: mine should project to ISO mine X
        from wc3bot.core.coords import ISO_H
        mine_iso_x = int(RENDER_CENTER_X + (704 - 320) * ISO_H)
        r_mine = world_to_screen(cam.cam_tx + 704, cam.cam_ty + 320, 0.0, cam=cam)
        print(f"  mine check: ISO_x={mine_iso_x}  w2s={r_mine}  "
              f"Δx={r_mine[0]-mine_iso_x if r_mine else '?'}")

    print("\n--- Pass 1: ISO (unit_to_screen) ---")
    for i, p in enumerate(peons2[:5]):
        sx, sy = unit_to_screen(p.x, p.y, gh_gx, gh_gy)
        inp.move_cursor(sx, sy)
        print(f"  peon[{i}] dx={p.x-gh_gx:+d} dy={p.y-gh_gy:+d} z={p.z:.1f}  ISO=({sx},{sy})", end="")
        if use_w2s:
            r = world_to_screen(p.x, p.y, p.z, cam=cam)
            w2s = f"({r[0]},{r[1]})" if r else "BEHIND"
            dx_diff = (r[0] - sx) if r else 0
            print(f"  w2s={w2s}  diff_x={dx_diff:+d}", end="")
        print()
        time.sleep(1.5)

    if use_w2s:
        print("\n--- Pass 2: world_to_screen (perspective-correct, wz=p.z) ---")
        for i, p in enumerate(peons2[:5]):
            r = world_to_screen(p.x, p.y, p.z, cam=cam)
            if r is None:
                print(f"  peon[{i}] BEHIND CAMERA"); continue
            inp.move_cursor(r[0], r[1])
            print(f"  peon[{i}] dx={p.x-gh_gx:+d} dy={p.y-gh_gy:+d} z={p.z:.1f}  w2s=({r[0]},{r[1]})")
            time.sleep(1.5)

    print("Done — which pass placed cursor accurately ON the peons?")


@test("T05_peon_pos", "Print sidecar peon screen positions (no click)")
def t05():
    sc    = make_scanner()
    state = read_state(sc)
    if not state:
        return
    calib = make_calib(state)
    import wc3bot.race.orc
    from wc3bot.race import get_race
    cfg = get_race("orc")
    peons = [u for u in state.units if u.id == cfg.worker_fcc]
    print(f"Found {len(peons)} peons in sidecar:")
    for i, p in enumerate(peons):
        sx, sy = calib.to_screen(p.x, p.y)
        print(f"  peon[{i}]  game=({p.x},{p.y})  screen=({sx},{sy})")
    if not peons:
        print("  (peons in mine are invisible to sidecar — normal if all are mining)")


@test("T06_click_gh", "BACKSPACE → click GH center — EXPECT: GH selected")
def t06():
    inp = make_inp()
    from wc3bot.core.process import gh_click_pos, window_bounds
    b = window_bounds()
    print(f"window_bounds = {b}")
    cx, cy = gh_click_pos()
    print(f"GH click target: ({cx},{cy})")
    countdown()
    inp.activate();    time.sleep(0.3)
    inp.key("BACKSPACE"); time.sleep(0.6)   # camera → GH first
    inp.click(cx, cy)
    print("Done — GH should now be selected (green circle + portrait)")


@test("T07_group_hall", "Ctrl+5 — assign GH to group 5 (click GH first)")
def t07():
    inp = make_inp()
    from wc3bot.core.process import gh_click_pos
    cx, cy = gh_click_pos()
    countdown()
    inp.activate(); time.sleep(0.3)
    inp.click(cx, cy); time.sleep(0.4)
    inp.ctrl_key("5");  time.sleep(0.2)
    print("Done — group 5 icon should appear in bottom bar")


@test("T08_select_hall", "Press 5 to select group 5 — run T07 first")
def t08():
    inp = make_inp()
    countdown()
    inp.activate(); time.sleep(0.3)
    inp.key("5")
    print("Done — GH should be selected")


@test("T09_train_peon", "Group5 → Q — train one peon. EXPECT: queue shows peon icon")
def t09():
    inp = make_inp()
    countdown()
    inp.activate(); time.sleep(0.3)
    inp.key("5");   time.sleep(0.2)
    inp.key("Q")
    print("Done — peon should appear in GH training queue")


@test("T10_backspace", "BACKSPACE — camera jump to main hall")
def t10():
    inp = make_inp()
    countdown()
    inp.activate(); time.sleep(0.3)
    inp.key("BACKSPACE")
    print("Done — camera should jump to your GH")


@test("T11_right_click_mine", "Right-click calculated mine position — EXPECT: peons move to mine")
def t11():
    sc    = make_scanner()
    state = read_state(sc)
    if not state:
        return
    calib = make_calib(state)
    if calib.mine_sx is None:
        print("[FAIL] mine position unknown — sidecar has no neutral units")
        return
    inp = make_inp()
    print(f"Will right-click mine at screen=({calib.mine_sx},{calib.mine_sy})")
    print("Select some peons manually BEFORE the countdown fires, then watch them move.")
    countdown()
    inp.activate(); time.sleep(0.2)
    inp.right_click(calib.mine_sx, calib.mine_sy)
    print("Done — selected peons should walk to mine")


@test("T11b_send_to_mine", "Click all peons → right-click mine")
def t11b():
    """
    1. Countdown(3) — switch to WC3
    2. BACKSPACE → center camera on GH
    3. Click peon[0], Shift+click peons[1..n] to multi-select
    4. Right-click mine
    EXPECT: all peons walk to gold mine.
    """
    sc = make_scanner()
    state = read_state(sc)
    if not state:
        return

    import wc3bot.race.orc
    from wc3bot.race import get_race
    from wc3bot.core.coords import CameraState, world_to_screen, find_nearest_mine
    from wc3bot.action.input import screen_size

    cfg = get_race("orc")
    gh_gx = state.gh_x or 0
    gh_gy = state.gh_y or 0
    sw, sh = screen_size()

    # Always use GH as camera target — BACKSPACE puts camera there regardless of spawn.
    # Do NOT use sidecar camera state (read before BACKSPACE = wrong camera position).
    cam_defaults = CameraState.from_game_state(state)
    cam = CameraState(cam_tx=float(gh_gx), cam_ty=float(gh_gy),
                      cam_aoa=cam_defaults.cam_aoa, cam_fov=cam_defaults.cam_fov,
                      cam_dist=cam_defaults.cam_dist, cam_rot=cam_defaults.cam_rot)

    from wc3bot.core.process import gh_click_pos
    from wc3bot.observe.state import parse_state

    mine = find_nearest_mine(state, gh_gx, gh_gy)
    if mine is None:
        print("[FAIL] no ngol in state"); return
    mine_pos = world_to_screen(mine.x, mine.y, 0.0, cam, sw, sh)
    if mine_pos is None:
        print("[FAIL] world_to_screen returned None for mine"); return

    peons = [u for u in state.units if u.id == cfg.worker_fcc]
    if not peons:
        print("[FAIL] no peons in sidecar state"); return

    from wc3bot.core.process import gh_click_pos
    gh_sx, gh_sy = gh_click_pos()

    # Mine via ISO formula — spawn-independent (relative to GH screen position)
    from wc3bot.core.coords import game_to_screen, ISO_H, ISO_V
    mine_pos = game_to_screen(mine.x, mine.y, gh_gx, gh_gy, gh_sx, gh_sy, ISO_H, ISO_V)
    print(f"mine: game=({mine.x},{mine.y})  → screen={mine_pos}")

    inp = make_inp()
    countdown(3)
    inp.activate(); time.sleep(0.2)
    inp.key("BACKSPACE"); time.sleep(0.5)

    # Box-select: start in safe empty terrain (far left of GH), drag through GH area.
    # WC3 box-select NEVER selects buildings — only units. GH can be inside box safely.
    # Start far enough left that drag doesn't begin on any building.
    box_x1 = gh_sx - 600
    box_y1 = gh_sy - 200
    box_x2 = gh_sx + 300
    box_y2 = gh_sy + 200
    print(f"box-select: ({box_x1},{box_y1}) → ({box_x2},{box_y2})")
    inp.drag(box_x1, box_y1, box_x2, box_y2, hold=0.3)
    time.sleep(0.2)

    inp.right_click(*mine_pos)
    time.sleep(1.5)

    # Verify: peons entering mine disappear from sidecar — 0 visible = all in mine
    raw4 = sc.hot_poll()
    s4 = parse_state(raw4) if raw4 else None
    n_visible = len([u for u in s4.units if u.id == cfg.worker_fcc]) if s4 else "?"
    print(f"visible peons after 1.5s: {n_visible}  (fewer = going to mine)")
    print("Done")


@test("T12_box_select", "Drag box-select across whole screen — EXPECT: all visible units selected")
def t12():
    inp = make_inp()
    from wc3bot.core.process import window_bounds
    b = window_bounds()
    if b:
        wl, wt, wr, wb = b
        x1, y1 = wl + 20, wt + 40
        x2, y2 = wr - 20, wb - 80
    else:
        from wc3bot.action.input import screen_size
        sw, sh = screen_size()
        x1, y1 = 50,      50
        x2, y2 = sw - 50, sh - 100
    print(f"Box select: ({x1},{y1}) → ({x2},{y2})")
    countdown()
    inp.activate(); time.sleep(0.3)
    inp.drag(x1, y1, x2, y2, hold=0.8)
    print("Done — all visible friendly units should be selected")


@test("T13_group_workers", "Box-select + Ctrl+6 — assign peons to group 6 (run after T12)")
def t13():
    inp = make_inp()
    from wc3bot.core.process import window_bounds
    b = window_bounds()
    if b:
        wl, wt, wr, wb = b
        x1, y1 = wl + 20, wt + 40
        x2, y2 = wr - 20, wb - 80
    else:
        from wc3bot.action.input import screen_size
        sw, sh = screen_size()
        x1, y1 = 50,      50
        x2, y2 = sw - 50, sh - 100
    countdown()
    inp.activate(); time.sleep(0.3)
    inp.drag(x1, y1, x2, y2, hold=0.8); time.sleep(0.5)
    inp.ctrl_key("6")
    print("Done — group 6 should be set; press 6 to verify")


@test("T14_click_peon", "BACKSPACE → re-read → click each peon via world_to_screen — EXPECT: each peon selected")
def t14():
    """
    Uses world_to_screen with actual sidecar camera state (proper perspective projection).

    world_to_screen is geometrically correct for any unit position on the map — ISO is only
    accurate near its two calibration points (GH and mine).  At intermediate distances
    (~400 units from GH), ISO underestimates screen X → clicks land LEFT of the actual unit.

    Requires CAM data from sidecar (cam_tx, cam_ty, cam_aoa, cam_fov, cam_dist, cam_rot).
    Falls back to unit_to_screen (ISO) if cam data is unavailable.

    RENDER_CENTER (1252, 579) is the screen anchor: camera target (=GH after BACKSPACE)
    projects to this pixel.  world_to_screen maps all world positions relative to this anchor.
    """
    sc    = make_scanner()
    state = read_state(sc)
    if not state:
        return
    import wc3bot.race.orc
    from wc3bot.race import get_race
    from wc3bot.core.coords import (world_to_screen, unit_to_screen,
                                    CameraState, RENDER_CENTER_X, RENDER_CENTER_Y)
    from wc3bot.observe.state import parse_state
    cfg = get_race("orc")
    peons = [u for u in state.units if u.id == cfg.worker_fcc]
    if not peons:
        print("[FAIL] No peons visible in sidecar (all mining?) — use T05 first")
        return
    inp = make_inp()
    countdown(3)
    inp.activate(); time.sleep(0.2)
    inp.key("BACKSPACE"); time.sleep(0.6)
    activate_wc3()
    raw2 = sc.hot_poll()
    state2 = parse_state(raw2) if raw2 else state
    peons2 = [u for u in state2.units if u.id == cfg.worker_fcc] if state2 else peons
    if not peons2:
        print("[FAIL] no peons after re-read"); return

    # Build camera state from sidecar — after BACKSPACE cam_tx/ty == GH position
    cam = CameraState.from_game_state(state2)
    use_w2s = state2.cam_tx is not None
    if use_w2s:
        print(f"Camera: tx={cam.cam_tx} ty={cam.cam_ty}  aoa={cam.cam_aoa:.1f}°  "
              f"fov={cam.cam_fov:.1f}°  dist={cam.cam_dist:.0f}  rot={cam.cam_rot:.1f}°")
        print(f"Using world_to_screen (perspective-correct)")
    else:
        print("WARNING: no CAM data in sidecar — falling back to ISO (unit_to_screen)")

    print(f"render_center=({RENDER_CENTER_X},{RENDER_CENTER_Y})")
    print(f"Clicking all {len(peons2)} peons one by one (2s between each):\n")
    for i, p in enumerate(peons2):
        if use_w2s:
            result = world_to_screen(p.x, p.y, p.z, cam=cam)
            if result is None:
                print(f"  peon[{i}] BEHIND CAMERA — skip"); continue
            sx, sy = result
        else:
            sx, sy = unit_to_screen(p.x, p.y, cam.cam_tx or 0, cam.cam_ty or 0)
        print(f"  peon[{i}] game=({p.x},{p.y}) z={p.z:.1f}  screen=({sx},{sy})")
        inp.click(sx, sy)
        time.sleep(2.0)
    print("Done — each peon should have been selected in turn (check portrait each time)")


@test("T04e_calibrate_peon_offset", "Hover over each peon → Enter → computes ISO offset for peon clicks")
def t04e():
    """
    Calibration: for each peon, compute ISO screen position, then ask user to
    hover cursor over the actual peon center and press Enter.  Records the delta
    (actual - ISO) per peon, prints average offset, and suggests a correction.

    Flow:
      1. BACKSPACE → re-read peon positions (camera on GH, fresh coords)
      2. Per peon: print computed ISO pos, user hovers over peon in WC3, tabs out, Enter
      3. Print per-peon deltas + average → copy into PEON_OFFSET in coords.py if consistent
    """
    sc    = make_scanner()
    state = read_state(sc)
    if not state:
        return
    import wc3bot.race.orc
    from wc3bot.race import get_race
    from wc3bot.core.coords import ISO_H, ISO_V
    from wc3bot.core.process import gh_click_pos
    from wc3bot.action.input import mouse_pos
    from wc3bot.observe.state import parse_state

    cfg = get_race("orc")
    peons = [u for u in state.units if u.id == cfg.worker_fcc]
    if not peons:
        print("[FAIL] No peons in sidecar — must be fresh game with peons idle")
        return

    inp = make_inp()
    countdown(3)
    inp.activate(); time.sleep(0.2)
    inp.key("BACKSPACE"); time.sleep(0.6)
    activate_wc3()
    raw2 = sc.hot_poll()
    state2 = parse_state(raw2) if raw2 else state
    peons2 = [u for u in state2.units if u.id == cfg.worker_fcc] if state2 else peons
    if not peons2:
        print("[FAIL] no peons after re-read"); return

    gh_gx = state2.gh_x or state.gh_x or 0
    gh_gy = state2.gh_y or state.gh_y or 0
    cx, cy = gh_click_pos()
    print(f"\nGH game=({gh_gx},{gh_gy})  gh_click_pos=({cx},{cy})")
    print(f"ISO_H={ISO_H}  ISO_V={ISO_V}\n")

    deltas = []
    for i, p in enumerate(peons2):
        dx, dy = p.x - gh_gx, p.y - gh_gy
        iso_sx = int(cx + (dx - dy) * ISO_H)
        iso_sy = int(cy + (dx + dy) * ISO_V)
        print(f"Peon[{i}]  game=({p.x},{p.y})  ISO→screen=({iso_sx},{iso_sy})")
        print(f"  → Switch to WC3, hover cursor over peon[{i}] center, tab back here, press Enter")
        input("  [Enter when cursor is on peon] ")
        ax, ay = mouse_pos()
        ddx, ddy = ax - iso_sx, ay - iso_sy
        deltas.append((ddx, ddy))
        print(f"  actual=({ax},{ay})  delta=({ddx:+d},{ddy:+d})\n")

    if not deltas:
        return
    avg_dx = sum(d[0] for d in deltas) // len(deltas)
    avg_dy = sum(d[1] for d in deltas) // len(deltas)
    print("=" * 50)
    print(f"Per-peon deltas: {deltas}")
    print(f"Average offset: dx={avg_dx:+d}  dy={avg_dy:+d}")
    print()
    if abs(avg_dx) > 3 or abs(avg_dy) > 3:
        print("Suggested fix in coords.py — add PEON_OFFSET constant and apply in T14:")
        print(f"  PEON_CLICK_OFFSET_X = {avg_dx}")
        print(f"  PEON_CLICK_OFFSET_Y = {avg_dy}")
    else:
        print("Offset < 3px — ISO is accurate enough, no correction needed.")


@test("T15_build_burrow", "Select peon[0] → Z + A + click placement (run early in game)")
def t15():
    sc    = make_scanner()
    state = read_state(sc)
    if not state:
        return
    import wc3bot.race.orc
    from wc3bot.race import get_race
    cfg   = get_race("orc")
    peons = [u for u in state.units if u.id == cfg.worker_fcc]
    if not peons:
        print("[FAIL] No peons visible in sidecar")
        return
    calib = make_calib(state)
    inp   = make_inp()
    p  = peons[0]
    sx, sy = calib.to_screen(p.x, p.y)
    bx = calib.gh_sx + cfg.burrow_offset[0]
    by = calib.gh_sy + cfg.burrow_offset[1]
    print(f"Peon[0] screen=({sx},{sy})")
    print(f"Burrow placement=({bx},{by})")
    print("EXPECT: peon selected → build ghost appears → Burrow placed")
    countdown()
    inp.activate(); time.sleep(0.3)
    inp.click(sx, sy);             time.sleep(0.3)
    inp.key(cfg.hotkey_build_menu); time.sleep(0.2)  # Z
    inp.key(cfg.hotkey_burrow);     time.sleep(0.2)  # A
    inp.click(bx, by)
    print("Done — Burrow ghost should appear then construction starts")


@test("T16_macro_start", "SPACE-start → peons→mine + GH-group + rally + train×2 in <5s")
def t16():
    """
    Ausgangszustand: WC3 Ladebildschirm / Spielauswahl (fresh game).

    Phase 1 — SPACE-Loop bis Spiel live (nicht in 5s-Timing):
      Drückt SPACE alle 2s bis Sidecar ein T>0 meldet.

    Phase 2 — Sequence (Ziel: <5s gesamt):
      1. BACKSPACE       → Kamera auf GH
      2. Box-select      → alle Peons markieren
      3. Rechts-klick    → Peons zur Mine schicken
      4. Klick GH        → GH auswählen
      5. Ctrl+<group>    → GH in Gruppe einteilen
      6. Rechts-klick    → Rallyepunkt auf Mine setzen
      7. Gruppe → Q      → Peon 1 trainieren
      8. Gruppe → Q      → Peon 2 trainieren (Queue)

    EXPECT nach Test:
      - Alle Peons laufen zur Mine
      - GH-Gruppe im Hotbar sichtbar
      - Rallyepunkt-Flagge auf Mine
      - GH Queue zeigt 2 Peons
    """
    from wc3bot.core.process import get_pid, get_task, gh_click_pos, window_bounds
    from wc3bot.action.input import WC3Input
    from wc3bot.observe.lua_reader import LuaStateReader
    from wc3bot.observe.state import parse_state
    from wc3bot.core.coords import CameraState, world_to_screen, find_nearest_mine
    from wc3bot.race import get_race
    import wc3bot.race.orc

    cfg  = get_race("orc")
    pid  = get_pid()
    task = get_task(pid)
    inp  = WC3Input(pid=pid)
    scanner = LuaStateReader(task, pid=pid)

    # ── Phase 1: Koordinaten VOR SPACE vorberechnen ───────────────────────────
    # gh_click_pos() + window_bounds() brauchen kein laufendes Spiel.
    gh_sx, gh_sy = gh_click_pos()
    b = window_bounds()
    if b:
        wl, wt, wr, wb = b
        box_x1, box_y1 = wl + 20,  wt + 40
        box_x2, box_y2 = wr - 20,  wb - 80
    else:
        box_x1, box_y1 = gh_sx - 700, gh_sy - 350
        box_x2, box_y2 = gh_sx + 700, gh_sy + 350
    print(f"GH=({gh_sx},{gh_sy})  Box=({box_x1},{box_y1})→({box_x2},{box_y2})")

    # ── Phase 2: Warten auf Live-Spiel ────────────────────────────────────────
    # SPACE via PostToPid funktioniert nicht für WC3-Menüs → User startet manuell.
    # WICHTIG: activate() vor find_live_game — WC3 muss im Vordergrund sein,
    # damit SP nicht pausiert und Sidecar weiter schreibt.
    # find_live_game hat internen Re-Bootstrap (nil >8s ODER T-frozen >5s).
    print("\n>>> JETZT in WC3 ein neues Spiel starten! <<<")
    print("    (Sidecar muss injiziert sein)")
    print("Warte auf Sidecar-Signal...\n")
    inp.activate()               # WC3 in Vordergrund vor dem Scan
    raw = scanner.find_live_game(verbose=True, timeout=300.0,
                                 keep_alive_fn=inp.activate,
                                 keep_alive_interval=4.0)
    if not raw:
        print("[FAIL] Kein Live-Spiel in 300s gefunden — Sidecar injiziert?")
        return
    state = parse_state(raw)
    if not state or state.time <= 0:
        print(f"[FAIL] parse_state lieferte kein gültiges State-Objekt")
        return
    print(f"LIVE!  T={state.time:.1f}s  G={state.gold}")

    # ── Phase 3: Mine + Peon-Koordinaten berechnen ───────────────────────────
    gh_gx = state.gh_x or 0
    gh_gy = state.gh_y or 0
    _cd = CameraState.from_game_state(state)

    # cam_actual: echte Kamera aus Sidecar bei T=13.9 — für Peon/Mine VOR BACKSPACE.
    # cam_gh: GH-zentrierte Kamera — für Mine-Rally NACH BACKSPACE.
    cam_actual = _cd
    cam_gh = CameraState(cam_tx=float(gh_gx), cam_ty=float(gh_gy),
                         cam_aoa=_cd.cam_aoa, cam_fov=_cd.cam_fov,
                         cam_dist=_cd.cam_dist, cam_rot=_cd.cam_rot)
    print(f"  cam_actual=({_cd.cam_tx:.0f},{_cd.cam_ty:.0f})  GH=({gh_gx},{gh_gy})")

    mine = find_nearest_mine(state, gh_gx, gh_gy)
    if mine is None:
        print("[FAIL] keine Mine in Sidecar"); return
    mine_pos_pre  = world_to_screen(mine.x, mine.y, 0.0, cam_actual)  # vor BACKSPACE
    mine_pos_post = world_to_screen(mine.x, mine.y, 0.0, cam_gh)      # nach BACKSPACE
    if mine_pos_pre is None or mine_pos_post is None:
        print("[FAIL] world_to_screen None"); return

    peons = [u for u in state.units if u.id == cfg.worker_fcc]
    peon_pos = None
    if peons:
        p = peons[0]
        pz = getattr(p, "z", 0.0)
        peon_pos = world_to_screen(p.x, p.y, 0.0, cam_actual)         # vor BACKSPACE
        print(f"  Peon[0]: game=({p.x},{p.y}) z={pz:.1f} → screen={peon_pos}")
        for i, pp in enumerate(peons[1:], 1):
            print(f"  Peon[{i}]: game=({pp.x},{pp.y}) z={getattr(pp,'z',0):.1f}")
    print(f"Mine(pre)={mine_pos_pre}  Mine(post)={mine_pos_post}  ({len(peons)} peons)")

    # ── Phase 4: Sequence via ActionQueue + InputExecutor ─────────────────────
    from wc3bot.core.action_queue import ActionQueue, Action, ActionStep, Priority
    from wc3bot.core.input_executor import InputExecutor
    import threading

    inp.activate()
    time.sleep(0.5)

    # Build the full macro-start as one Action (all steps must run in order).
    steps = []
    if peon_pos is not None:
        steps.append(ActionStep(inp.ctrl_click, peon_pos))               # alle Peons
    else:
        steps.append(ActionStep(inp.drag,
                                (box_x1, box_y1, box_x2, box_y2),
                                {"hold": 0.1}))                          # Fallback
    steps += [
        ActionStep(inp.right_click, mine_pos_pre),                       # Peons→Mine
        ActionStep(inp.key,         ("BACKSPACE",), delay_after=0.15),   # Kamera→GH
        ActionStep(inp.click,       (gh_sx, gh_sy)),                     # GH selektieren
        ActionStep(inp.ctrl_key,    (cfg.group_hall,)),                   # Gruppe
        ActionStep(inp.right_click, mine_pos_post),                      # Rally
        ActionStep(inp.key,         (cfg.group_hall,)),                   # Gruppe wählen
        ActionStep(inp.key,         (cfg.hotkey_train_worker,)),          # Peon 1
        ActionStep(inp.key,         (cfg.group_hall,)),                   # Gruppe wählen
        ActionStep(inp.key,         (cfg.hotkey_train_worker,)),          # Peon 2
    ]

    done_ev = threading.Event()

    class _TimedExecutor(InputExecutor):
        """Wraps InputExecutor to record per-step timestamps."""
        def __init__(self, q, labels):
            super().__init__(q)
            self._labels = labels
            self._step_ts: list[tuple[str, float]] = []
            self._t0 = 0.0

        def _execute(self, action):
            self._t0 = time.perf_counter()
            for i, step in enumerate(action.steps):
                step.fn(*step.args, **step.kwargs)
                ts = time.perf_counter() - self._t0
                label = self._labels[i] if i < len(self._labels) else f"step_{i}"
                self._step_ts.append((label, ts))
                if step.delay_after > 0:
                    time.sleep(step.delay_after)
            self._q.task_done()
            done_ev.set()

    step_labels = [
        "ctrl_click / drag",
        "right_click(mine_pre)",
        "BACKSPACE",
        "click(GH)",
        "ctrl_key(group_hall)",
        "right_click(mine_post)",
        "key(group_hall) #1",
        "key(train_worker) #1",
        "key(group_hall) #2",
        "key(train_worker) #2",
    ]

    q  = ActionQueue()
    ex = _TimedExecutor(q, step_labels)
    ex.start()

    print("Sequence startet JETZT")
    t0 = time.perf_counter()
    q.submit(Action(Priority.MACRO, "macro_start", steps))
    done_ev.wait(timeout=10.0)
    elapsed = time.perf_counter() - t0

    ex.stop()
    ex.join(timeout=2.0)

    status = "PASS ✓" if elapsed < 1.0 else "OK" if elapsed < 5.0 else "SLOW ✗"
    print(f"\n[{status}]  Sequence: {elapsed:.2f}s  |  Spiel live bei T={state.time:.1f}s")
    prev = 0.0
    for label, ts in ex._step_ts:
        print(f"  {label:<28} {ts*1000:6.0f}ms  (+{(ts-prev)*1000:.0f}ms)")
        prev = ts
    print("EXPECT: Peons→Mine | Gruppe gesetzt | Rally auf Mine | 2 Peons in Queue")


# ── Entry point ───────────────────────────────────────────────────────────────

def fresh_game_start() -> None:
    """Press SPACE to start WC3 game and wait for sidecar to go live."""
    from wc3bot.core.process import get_pid, get_task
    from wc3bot.action.input import WC3Input
    from wc3bot.observe.lua_reader import LuaStateReader
    from wc3bot.observe.state import parse_state
    pid     = get_pid()
    task    = get_task(pid)
    inp     = WC3Input(pid=pid)
    scanner = LuaStateReader(task, pid=pid)
    print("--- FRESH START: pressing SPACE until live game detected ---")
    attempt = 0
    while True:
        attempt += 1
        inp.activate()
        inp.key("SPACE")
        print(f"  [{attempt}] SPACE sent — polling sidecar...", end=" ", flush=True)
        raw = scanner.find_live_game(verbose=False)
        if raw is not None:
            state = parse_state(raw)
            if state is not None:
                print(f"LIVE! T={state.time:.1f}  G={state.gold}  units={len(state.units)}")
                inp.activate()
                time.sleep(1.5)
                return
        print("not live yet")
        time.sleep(2)


def main():
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--list",  action="store_true", help="List all tests")
    ap.add_argument("--test",  metavar="NAME",       help="Run a specific test")
    ap.add_argument("--from",  metavar="NAME",       dest="from_test",
                    help="Run all tests >= NAME in sorted order with Enter between each")
    ap.add_argument("--fresh", action="store_true",
                    help="Press SPACE to start fresh game before running test")
    args = ap.parse_args()

    if args.list or (not args.test and not args.from_test):
        print("Available tests:")
        for name, (desc, _) in sorted(TESTS.items()):
            print(f"  {name:20s}  {desc}")
        return

    if args.fresh:
        fresh_game_start()

    if args.from_test:
        sequence = sorted(n for n in TESTS if n >= args.from_test)
        if not sequence:
            print(f"No tests >= {args.from_test!r}")
            sys.exit(1)
        print(f"Running {len(sequence)} tests starting from {args.from_test}: {', '.join(sequence)}")
        for i, name in enumerate(sequence):
            desc, fn = TESTS[name]
            print(f"\n{'='*60}")
            print(f"TEST {i+1}/{len(sequence)}: {name} — {desc}")
            print(f"{'='*60}\n")
            fn()
            if i < len(sequence) - 1:
                try:
                    input(f"\n--- Press ENTER for next test ({sequence[i+1]}) or Ctrl+C to stop ---\n")
                except KeyboardInterrupt:
                    print("\nStopped.")
                    return
        return

    if args.test not in TESTS:
        print(f"Unknown test: {args.test!r}")
        print("Run --list to see available tests.")
        sys.exit(1)

    desc, fn = TESTS[args.test]
    print(f"\n{'='*60}")
    print(f"TEST: {args.test} — {desc}")
    print(f"{'='*60}\n")
    fn()


if __name__ == "__main__":
    main()
