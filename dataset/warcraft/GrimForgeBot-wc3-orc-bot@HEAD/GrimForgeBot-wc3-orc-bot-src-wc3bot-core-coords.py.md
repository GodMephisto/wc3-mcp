---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "src/wc3bot/core/coords.py"
ref: HEAD
---

from __future__ import annotations
import math
from dataclasses import dataclass
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from wc3bot.observe.state import GameState, UnitRecord

# ── Reference calibration resolution ───────────────────────────────────────────
# All constants below were measured on a 2560×1440 fullscreen display.
# Use scaled_iso() and scaled_render_center() for the actual current screen.
_CALIB_W: int = 2560
_CALIB_H: int = 1440

# ── Isometric coordinate transform constants (at 2560×1440) ────────────────────
# WC3 isometric: +gx → screen right+up, +gy → screen left+up.
#   screen_x = gh_sx + (dgx - dgy) * ISO_H
#   screen_y = gh_sy + (dgx + dgy) * ISO_V
# Calibrated 2026-05-24 via T04c_calibrate on 2560×1440:
#   GH screen=(1252,443)  mine game dx=704 dy=320  mine screen=(1980,267)
#   ISO_H = (1980-1252)/(704-320) = 728/384 = 1.8958
#   ISO_V = (267-443)/(704+320) = -176/1024 = -0.1719
ISO_H =  1.8958  # px per game unit, horizontal (gx-gy axis) @ 2560×1440
ISO_V = -0.2520  # px per game unit, vertical (gx+gy axis) — BUILDINGS @ 2560×1440
# ISO_V recalibrated 2026-05-28: gh_sy=525 (gh_click_pos actual), mine_sy=267
# (267 - 525) / (704 + 320) = -258 / 1024 = -0.252
# NOTE: ISO_V is calibrated between building visual centers (using gh_click_pos anchor).
# For ground-level units (z=0), use ISO_V_UNITS with RENDER_CENTER_Y anchor.
# Derived directly from mine z=0 measurement:
#   (267 - 541) / (704 + 320) = -274 / 1024 = -0.2676
ISO_V_UNITS = -0.2676  # px per game unit, vertical — UNITS at z=0 @ 2560×1440

# ── UI render center (at 2560×1440) ────────────────────────────────────────────
# After BACKSPACE the camera target is at GH (gh_gx, gh_gy, z=0).
# This is the screen pixel where z=0 of the camera target appears.
# Calibrated on 2560×1440 fullscreen WC3:
#   X = screen_w/2 - 28 = 1252  (GH is slightly left of geometric center)
#   Y = 541  (derived 2026-05-28: mine visual center ≈ z=0 → screen_y=267;
#             world_to_screen with RCY=541 gives mine_y=266.7 ✓ and GH@z=25 → 525 ✓)
# RENDER_CENTER_Y is the correct anchor for units (z=0).
# gh_click_pos()[1]=525 is only correct for building visual-center clicks (large targets).
RENDER_CENTER_X: int = 1252  # @ 2560×1440
RENDER_CENTER_Y: int = 541   # @ 2560×1440


def _screen_scales() -> tuple[float, float]:
    """Return (sx, sy) — current screen size divided by calibration resolution."""
    from wc3bot.action.input import screen_size
    w, h = screen_size()
    return w / _CALIB_W, h / _CALIB_H


def scaled_iso() -> tuple[float, float, float]:
    """Return (ISO_H, ISO_V, ISO_V_UNITS) scaled to the current screen resolution."""
    sx, sy = _screen_scales()
    return ISO_H * sx, ISO_V * sy, ISO_V_UNITS * sy


def scaled_render_center() -> tuple[int, int]:
    """Return (RENDER_CENTER_X, RENDER_CENTER_Y) for the current WC3 window.

    Derives from gh_click_pos() which already adapts to window bounds, then applies
    the z=0 ground offset (GH building visual center sits z≈25 above ground = ~16px
    at 1440p, scales with screen height).
    """
    from wc3bot.core.process import gh_click_pos
    from wc3bot.action.input import screen_size
    cx, cy = gh_click_pos()
    _, sh = screen_size()
    z0_offset = int(16 * sh / _CALIB_H)   # 16px at 1440p, scales linearly
    return cx, cy + z0_offset


def game_to_screen(gx: int, gy: int,
                   gh_gx: int, gh_gy: int,
                   gh_sx: int, gh_sy: int,
                   iso_h: float = ISO_H,
                   iso_v: float = ISO_V) -> tuple[int, int]:
    """Convert WC3 game coordinates to screen coordinates (isometric transform).
    Use for buildings — gh_sy should be gh_click_pos()[1] (building visual center).
    For units use unit_to_screen() which uses RENDER_CENTER_Y (ground z=0 anchor).
    """
    dx = gx - gh_gx
    dy = gy - gh_gy
    return int(gh_sx + (dx - dy) * iso_h), int(gh_sy + (dx + dy) * iso_v)


def unit_to_screen(gx: int, gy: int,
                   gh_gx: int, gh_gy: int,
                   gh_sx: int | None = None) -> tuple[int, int]:
    """Convert unit game coordinates to screen coordinates using ground-level anchor.

    Scales ISO_H/V and RENDER_CENTER automatically to the current screen resolution.
    For buildings use game_to_screen() with gh_click_pos() anchor.
    """
    _iso_h, _iso_v, _iso_v_units = scaled_iso()
    _rcx, _rcy = scaled_render_center()
    _gh_sx = gh_sx if gh_sx is not None else _rcx
    dx = gx - gh_gx
    dy = gy - gh_gy
    return int(_gh_sx + (dx - dy) * _iso_h), int(_rcy + (dx + dy) * _iso_v_units)


def calibrate_iso(gh_gx: int, gh_gy: int, gh_sx: int, gh_sy: int,
                  ref_gx: int, ref_gy: int, ref_sx: int, ref_sy: int,
                  ) -> tuple[float, float]:
    """
    Compute ISO_H and ISO_V from two known anchor points.
    Anchor 1: GH game=(gh_gx,gh_gy) → screen=(gh_sx,gh_sy)
    Anchor 2: reference point (e.g. mine) game=(ref_gx,ref_gy) → screen=(ref_sx,ref_sy)
    """
    ddx = (ref_gx - gh_gx) - (ref_gy - gh_gy)   # game (dx - dy)
    ddy = (ref_gx - gh_gx) + (ref_gy - gh_gy)   # game (dx + dy)
    iso_h = (ref_sx - gh_sx) / ddx if ddx != 0 else ISO_H
    iso_v = (ref_sy - gh_sy) / ddy if ddy != 0 else ISO_V
    return iso_h, iso_v


# ── 3D perspective projection ──────────────────────────────────────────────────

@dataclass
class CameraState:
    """
    WC3 camera parameters needed for a proper perspective projection.

    All fields match the WC3 Lua native conventions:
      cam_tx / cam_ty : camera target position in world units
      cam_aoa         : angle of attack in WC3 degrees
                        (360=horizontal, 270=top-down; default ≈304 ≈ 56° below horizon)
      cam_fov         : field of view in degrees (WC3 Reforged default ≈70°)
                        GetCameraField(CAMERA_FIELD_FIELD_OF_VIEW) returns radians; parser * 180/π.
      cam_dist        : distance from camera eye to target in world units (default ≈1650)
      cam_rot         : azimuth in WC3 degrees (90 = camera faces +Y / North)
    """
    cam_tx:   float = 0.0
    cam_ty:   float = 0.0
    cam_tz:   float = 0.0   # terrain height at camera target (from GetCameraTargetPositionZ)
    cam_aoa:  float = 304.0
    cam_fov:  float = 70.0  # WC3 Reforged default ≈ 70° (1.2217 rad)
    cam_dist: float = 1650.0
    cam_rot:  float = 90.0

    @staticmethod
    def defaults() -> "CameraState":
        """Return a CameraState with WC3 default values (safe fallback)."""
        return CameraState()

    @staticmethod
    def from_game_state(state: "GameState") -> "CameraState":
        """
        Build a CameraState from a GameState.
        Falls back to WC3 defaults for any field the sidecar did not emit.
        """
        defaults = CameraState.defaults()
        return CameraState(
            cam_tx=float(state.cam_tx)   if state.cam_tx   is not None else defaults.cam_tx,
            cam_ty=float(state.cam_ty)   if state.cam_ty   is not None else defaults.cam_ty,
            cam_tz=float(state.cam_tz)   if state.cam_tz   is not None else defaults.cam_tz,
            cam_aoa=state.cam_aoa        if state.cam_aoa  is not None else defaults.cam_aoa,
            cam_fov=state.cam_fov        if state.cam_fov  is not None else defaults.cam_fov,
            cam_dist=state.cam_dist      if state.cam_dist is not None else defaults.cam_dist,
            cam_rot=state.cam_rot        if state.cam_rot  is not None else defaults.cam_rot,
        )


def world_to_screen(
    wx: float, wy: float, wz: float = 0.0,
    cam: "CameraState | None" = None,
    screen_w: int | None = None,
    screen_h: int | None = None,
    anchor_sx: int | None = None,
    anchor_sy: int | None = None,
) -> tuple[int, int] | None:
    """
    Convert WC3 world coordinates to screen pixel coordinates using a proper
    3D perspective projection.

    Parameters
    ----------
    wx, wy, wz  : world-space position (wz=0 for ground-level units)
    cam         : CameraState; if None, WC3 default camera values are used
    screen_w/h  : output resolution in pixels

    Returns (px, py) or None when the point is behind the camera.

    Camera model
    ------------
    WC3 places the camera eye at:
        eye = target + offset
    where offset is derived from cam_dist, cam_aoa and cam_rot:
        pitch   = radians(360 - cam_aoa)    # angle below horizontal
        azimuth = radians(cam_rot)
        eye_x = cam_tx - cam_dist * cos(pitch) * cos(azimuth)
        eye_y = cam_ty - cam_dist * cos(pitch) * sin(azimuth)
        eye_z = cam_tz + cam_dist * sin(pitch)   (cam_tz ≈ 0, target at terrain)

    The view matrix is built from Forward/Right/Up vectors derived from
    (target - eye), then a standard perspective divide is applied.
    """
    if cam is None:
        cam = CameraState.defaults()

    if screen_w is None or screen_h is None:
        from wc3bot.action.input import screen_size
        _sw, _sh = screen_size()
        screen_w = screen_w if screen_w is not None else _sw
        screen_h = screen_h if screen_h is not None else _sh

    cam_tz = cam.cam_tz  # real terrain height from GetCameraTargetPositionZ()

    # Convert WC3 AoA to standard pitch below horizontal
    pitch   = math.radians(360.0 - cam.cam_aoa)
    azimuth = math.radians(cam.cam_rot)

    # Camera eye position in world space
    eye_x = cam.cam_tx - cam.cam_dist * math.cos(pitch) * math.cos(azimuth)
    eye_y = cam.cam_ty - cam.cam_dist * math.cos(pitch) * math.sin(azimuth)
    eye_z = cam_tz      + cam.cam_dist * math.sin(pitch)

    # Vector from eye to world point
    dx = wx - eye_x
    dy = wy - eye_y
    dz = wz - eye_z

    # Forward = (target - eye), normalised
    fwd_x = cam.cam_tx - eye_x
    fwd_y = cam.cam_ty - eye_y
    fwd_z = cam_tz      - eye_z
    fwd_len = math.sqrt(fwd_x ** 2 + fwd_y ** 2 + fwd_z ** 2)
    if fwd_len == 0:
        return None
    fwd_x /= fwd_len
    fwd_y /= fwd_len
    fwd_z /= fwd_len

    # Right = Forward × WorldUp(0,0,1), normalised
    right_x =  fwd_y
    right_y = -fwd_x
    right_z =  0.0
    right_len = math.sqrt(right_x ** 2 + right_y ** 2)
    if right_len == 0:
        return None
    right_x /= right_len
    right_y /= right_len

    # Up = Right × Forward
    up_x = right_y * fwd_z - right_z * fwd_y
    up_y = right_z * fwd_x - right_x * fwd_z
    up_z = right_x * fwd_y - right_y * fwd_x

    # Project world delta into camera space
    cam_cx = dx * right_x + dy * right_y + dz * right_z
    cam_cy = dx * up_x    + dy * up_y    + dz * up_z
    cam_cz = dx * fwd_x   + dy * fwd_y   + dz * fwd_z

    if cam_cz <= 0:
        return None  # point is behind the camera

    # Perspective scale — ISO-calibrated (no FOV formula needed).
    # Derived from empirical anchors: GH→mine game offset dx=704, dy=320, ISO_H=1.8958.
    # For cam_rot≈90°: cam_cx_mine ≈ mine_dx = 704 (right component)
    #                  cam_cz_mine ≈ cam_dist + cos(pitch) * mine_dy = cam_dist + cos_p * 320
    # We know ISO projection: screen_x_offset = (mine_dx - mine_dy) * ISO_H
    # → scale_x = ISO_H * (mine_dx - mine_dy) * cam_cz_mine / (mine_dx * 0.5 * screen_w)
    # With defaults: scale_x = 1.8958*384*1829 / (704*1280) ≈ 1.476
    # Verified: mine screen X = 1252 + 704*1.476/1829*1280 ≈ 1979 ✓
    aspect   = screen_w / screen_h
    _cos_p   = math.cos(pitch)
    _mine_dx = 704.0   # mine game delta-x from GH (calibration anchor)
    _mine_dy = 320.0   # mine game delta-y from GH (calibration anchor)
    _cam_cz_mine = cam.cam_dist + _cos_p * _mine_dy
    scale_x  = (ISO_H * (_mine_dx - _mine_dy) * _cam_cz_mine) / (_mine_dx * 0.5 * screen_w)
    scale_y  = scale_x * aspect

    ndc_x = cam_cx * scale_x / cam_cz
    ndc_y = cam_cy * scale_y / cam_cz

    # NDC → pixels (y-axis flipped; screen origin is top-left).
    # The camera target always projects to RENDER_CENTER (the UI-corrected center).
    # anchor_sx/sy: override if you know the exact render center for this session.
    _rcx, _rcy = scaled_render_center()
    cx = anchor_sx if anchor_sx is not None else _rcx
    cy = anchor_sy if anchor_sy is not None else _rcy
    px = int(ndc_x * 0.5 * screen_w + cx)
    py = int(-ndc_y * 0.5 * screen_h + cy)
    return px, py


def find_nearest_mine(state: "GameState", gh_gx: int, gh_gy: int) -> "UnitRecord | None":
    """Return the nearest gold mine (ngol) to the GH from sidecar neutral list."""
    best, best_d = None, float("inf")
    for n in state.neutral:
        if n.id != "ngol":
            continue
        d = (n.x - gh_gx) ** 2 + (n.y - gh_gy) ** 2
        if d < best_d:
            best_d, best = d, n
    return best


@dataclass
class Calibration:
    """
    All coordinate data needed to convert game coords → screen coords.

    gh_gx/gh_gy : GH position in WC3 game units (from sidecar buildings)
    gh_sx/gh_sy : GH position on screen (screen centre after Backspace)
    mine_sx/sy  : Gold mine screen position (from sidecar neutral or user click)
    iso_h/iso_v : Isometric scale factors (calibrated if mine game coords known)
    """
    __slots__ = ("gh_gx", "gh_gy", "gh_sx", "gh_sy", "mine_sx", "mine_sy", "iso_h", "iso_v")

    def __init__(self,
                 gh_gx: int, gh_gy: int,
                 gh_sx: int, gh_sy: int,
                 mine_sx: int | None, mine_sy: int | None,
                 iso_h: float | None = None, iso_v: float | None = None):
        _iso_h, _iso_v, _ = scaled_iso()
        self.gh_gx   = gh_gx;  self.gh_gy  = gh_gy
        self.gh_sx   = gh_sx;  self.gh_sy  = gh_sy
        self.mine_sx = mine_sx; self.mine_sy = mine_sy
        self.iso_h   = iso_h if iso_h is not None else _iso_h
        self.iso_v   = iso_v if iso_v is not None else _iso_v

    def to_screen(self, gx: int, gy: int) -> tuple[int, int]:
        """Convert any game coordinate to screen coordinate."""
        return game_to_screen(gx, gy, self.gh_gx, self.gh_gy,
                              self.gh_sx, self.gh_sy,
                              self.iso_h, self.iso_v)

    def offset(self, dx_px: int, dy_px: int) -> tuple[int, int]:
        """Return screen position offset from GH anchor by (dx_px, dy_px) pixels."""
        return (self.gh_sx + dx_px, self.gh_sy + dy_px)
