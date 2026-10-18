---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "src/wc3bot/observe/state.py"
ref: HEAD
---

"""wc3bot.observe.state — Typed GameState dataclass + parsing + formatting."""
from __future__ import annotations
import re
from dataclasses import dataclass, field
from typing import Optional

# Default WC3 camera values used when the sidecar does not yet emit CAM fields.
_CAM_AOA_DEFAULT  = 304.0   # angle of attack (WC3 degrees; 304=default view)
_CAM_FOV_DEFAULT  =  70.0   # vertical FOV (degrees)
_CAM_DIST_DEFAULT = 1650.0  # distance from target (world units)
_CAM_ROT_DEFAULT  =  90.0   # azimuth (degrees; 90=faces North)

# ── Sidecar regex ─────────────────────────────────────────────────────────────
# Header: T/G/L/FU/FC (required) + GH (optional) + CAM (optional, 7 fields)
# Tail:   any number of |KEY:value entries where value contains no pipes.
#         Known prefixes: U B E N H HI D TK SEL
SIDECAR_RE = re.compile(
    rb'T:(\d+\.?\d*)\|G:(\d+)\|L:(\d+)\|FU:(\d+)\|FC:(\d+)'
    rb'(?:\|GH:(-?\d+):(-?\d+))?'
    rb'(?:\|CAM:(-?\d+):(-?\d+):(-?\d+\.?\d*):(-?\d+\.?\d*):(-?\d+\.?\d*):(-?\d+\.?\d*):(-?\d+\.?\d*))?'
    rb'((?:\|[A-Z]+:[^|\x00]*)*)',
    re.ASCII,
)


# ── Dataclasses ───────────────────────────────────────────────────────────────

@dataclass
class UnitRecord:
    id: str
    x: int
    y: int
    hp: int          # hp% (0-100); for buildings: construction progress
    z: float = 0.0   # terrain height at unit position (from BlzGetTerrainHeight)
    order: int = 0       # current order ID (0 = idle); for buildings: unused
    queue_id: str = ""   # FourCC of unit being trained/researched ("" or "0000" = idle)
    gold: int = 0        # mine gold remaining (neutrals only; 0 for non-resource units)
    workers: int = 0     # visible own workers within harvest radius (proximity count).
                         # For Orc/Human: does NOT include the 1 peon physically inside
                         # the mine (invisible to all enumeration).  Add +1 via
                         # GameState.worker_in_mine() for the full assigned count.
    rally_x: int = 0     # rally point X (buildings only; defaults to building X)
    rally_y: int = 0     # rally point Y (buildings only; defaults to building Y)


@dataclass
class ItemRecord:
    slot: int    # 1-indexed inventory slot
    id: str      # FourCC item type id
    charges: int


@dataclass
class HeroRecord:
    id: str
    x: int
    y: int
    hp: int          # hp%
    z: float = 0.0
    order: int = 0
    mana: int = 0
    max_mana: int = 0
    level: int = 1
    xp: int = 0
    skill_points: int = 0   # unspent skill points
    items: list[ItemRecord] = field(default_factory=list)


@dataclass
class DestructibleRecord:
    id: str    # FourCC type id (e.g. "ATtr" = tree)
    x: int
    y: int
    hp: int    # hp% remaining (proxy for lumber remaining in trees)


@dataclass
class CliffRecord:
    x: int
    y: int
    level: int   # 0-15; higher = higher ground; differences ≥1 = high-ground advantage


@dataclass
class GameState:
    time:       float
    gold:       int
    lumber:     int
    food_used:  int
    food_cap:   int
    gh_x:       Optional[int]
    gh_y:       Optional[int]
    # Camera state — None when sidecar was built before CAM support was added.
    # Use wc3bot.core.coords.CameraState.defaults() for a safe fallback.
    cam_tx:     Optional[int]   = None   # camera target X (world units)
    cam_ty:     Optional[int]   = None   # camera target Y (world units)
    cam_tz:     Optional[float] = None   # camera target Z = terrain height (world units)
    cam_aoa:    Optional[float] = None   # angle of attack (WC3 degrees, default ≈304)
    cam_fov:    Optional[float] = None   # vertical FOV (degrees)
    cam_dist:   Optional[float] = None   # distance from target (world units)
    cam_rot:    Optional[float] = None   # azimuth (degrees, 90=faces North)
    # Unit lists
    units:        list[UnitRecord]       = field(default_factory=list)  # own non-hero units
    buildings:    list[UnitRecord]       = field(default_factory=list)  # own buildings
    enemies:      list[UnitRecord]       = field(default_factory=list)  # visible enemies
    neutral:      list[UnitRecord]       = field(default_factory=list)  # neutral structures
    heroes:       list[HeroRecord]       = field(default_factory=list)  # own heroes
    # World data
    destructibles: list[DestructibleRecord] = field(default_factory=list)
    cliff_samples: list[CliffRecord]        = field(default_factory=list)
    selected:      list[str]                = field(default_factory=list)  # selected type IDs

    @property
    def food_free(self) -> int:
        return self.food_cap - self.food_used

    def units_by_id(self, fcc: str) -> list[UnitRecord]:
        """Filter own non-hero units by FourCC id."""
        return [u for u in self.units if u.id == fcc]

    def buildings_by_id(self, fcc: str) -> list[UnitRecord]:
        """Filter buildings by FourCC id."""
        return [b for b in self.buildings if b.id == fcc]

    def nearest_neutral(self, fcc: str, gx: int, gy: int) -> UnitRecord | None:
        """Return nearest neutral entry with matching id, or None."""
        best: UnitRecord | None = None
        best_d = float("inf")
        for n in self.neutral:
            if n.id != fcc:
                continue
            d = (n.x - gx) ** 2 + (n.y - gy) ** 2
            if d < best_d:
                best_d, best = d, n
        return best

    def mine_total_workers(self, mine: "UnitRecord", worker_id: str = "opeo",
                           total_at_start: int = 5) -> int:
        """
        Total workers in the mining cycle for this mine.

        = mine.workers  (visible proximity count from sidecar)
        + 1             (if a worker is currently invisible inside the mine)

        For Orc/Human this will be 0–5.  Compare against total_at_start to detect
        economy damage (e.g. 3/5 → 2 peons killed → rebuild immediately).
        """
        inside = 1 if self.worker_in_mine(worker_id, total_at_start) else 0
        return mine.workers + inside

    def worker_in_mine(self, worker_id: str = "opeo", total_at_start: int = 5) -> bool:
        """
        Return True if a worker is currently inside a gold mine.

        WC3 mechanics (Orc/Human): only ONE worker can be inside a mine at a time.
        Workers disappear from all unit-enumeration APIs while inside — the only
        signal is a count drop.  GetUnitCargoCount returns 0 for mines (they don't
        use the cargo transport system).

            worker_in_mine = (visible_count < total_at_start)

        Inference only works correctly when no workers have died.
        total_at_start=5 is the standard Orc/Human melee opening count.
        """
        visible = sum(1 for u in self.units if u.id == worker_id)
        return visible < total_at_start

    def cliff_at(self, x: int, y: int) -> int | None:
        """
        Return the cliff level at the closest sampled grid point, or None if
        no cliff samples are available.
        """
        best = None
        best_d = float("inf")
        for c in self.cliff_samples:
            d = (c.x - x) ** 2 + (c.y - y) ** 2
            if d < best_d:
                best_d, best = d, c
        return best.level if best is not None else None


# ── Parser ────────────────────────────────────────────────────────────────────

def parse_state(raw: bytes) -> GameState | None:
    """Parse sidecar bytes into a typed GameState, or None if no match."""
    m = SIDECAR_RE.match(raw)
    if not m:
        return None

    gh_x = int(m.group(6)) if m.group(6) is not None else None
    gh_y = int(m.group(7)) if m.group(7) is not None else None

    # Groups 8-14: optional CAM block (cam_tx, cam_ty, cam_tz, cam_aoa, cam_fov, cam_dist, cam_rot)
    cam_tx   = int(m.group(8))    if m.group(8)  is not None else None
    cam_ty   = int(m.group(9))    if m.group(9)  is not None else None
    cam_tz   = float(m.group(10)) if m.group(10) is not None else None
    import math as _math
    # GetCameraField returns radians — convert to degrees for CameraState/world_to_screen
    cam_aoa  = float(m.group(11)) * (180.0 / _math.pi) if m.group(11) is not None else None
    cam_fov  = float(m.group(12)) * (180.0 / _math.pi) if m.group(12) is not None else None
    cam_dist = float(m.group(13))                       if m.group(13) is not None else None
    cam_rot  = float(m.group(14)) * (180.0 / _math.pi) if m.group(14) is not None else None

    state = GameState(
        time=float(m.group(1)),
        gold=int(m.group(2)),
        lumber=int(m.group(3)),
        food_used=int(m.group(4)),
        food_cap=int(m.group(5)),
        gh_x=gh_x,
        gh_y=gh_y,
        cam_tx=cam_tx,
        cam_ty=cam_ty,
        cam_tz=cam_tz,
        cam_aoa=cam_aoa,
        cam_fov=cam_fov,
        cam_dist=cam_dist,
        cam_rot=cam_rot,
    )

    tail = m.group(15)
    if not tail:
        return state

    last_hero: HeroRecord | None = None

    for entry in tail.split(b"|"):
        if not entry:
            continue

        colon = entry.find(b":")
        if colon < 0:
            continue
        pfx  = entry[:colon]
        rest = entry[colon + 1:]
        parts = rest.split(b":")

        if pfx == b"U":
            # U:id:x:y:hp[:z[:order]]
            if len(parts) < 4:
                continue
            state.units.append(UnitRecord(
                id=parts[0].decode(),
                x=int(parts[1]),
                y=int(parts[2]),
                hp=int(parts[3]),
                z=float(parts[4]) if len(parts) > 4 else 0.0,
                order=int(parts[5]) if len(parts) > 5 else 0,
            ))
            last_hero = None

        elif pfx == b"H":
            # H:id:x:y:hp:z:order:mana:maxmana:lvl:xp:sp
            if len(parts) < 4:
                continue
            rec = HeroRecord(
                id=parts[0].decode(),
                x=int(parts[1]),
                y=int(parts[2]),
                hp=int(parts[3]),
                z=float(parts[4]) if len(parts) > 4 else 0.0,
                order=int(parts[5]) if len(parts) > 5 else 0,
                mana=int(parts[6]) if len(parts) > 6 else 0,
                max_mana=int(parts[7]) if len(parts) > 7 else 0,
                level=int(parts[8]) if len(parts) > 8 else 1,
                xp=int(parts[9]) if len(parts) > 9 else 0,
                skill_points=int(parts[10]) if len(parts) > 10 else 0,
            )
            state.heroes.append(rec)
            last_hero = rec

        elif pfx == b"HI":
            # HI:slot:id:charges  — immediately follows its H entry
            if len(parts) < 3 or last_hero is None:
                continue
            last_hero.items.append(ItemRecord(
                slot=int(parts[0]),
                id=parts[1].decode(),
                charges=int(parts[2]),
            ))

        elif pfx == b"B":
            # B:id:x:y:hp[:z[:queueid[:rp_x[:rp_y]]]]
            if len(parts) < 4:
                continue
            bx = int(parts[1])
            by = int(parts[2])
            state.buildings.append(UnitRecord(
                id=parts[0].decode(),
                x=bx,
                y=by,
                hp=int(parts[3]),
                z=float(parts[4]) if len(parts) > 4 else 0.0,
                queue_id=parts[5].decode() if len(parts) > 5 else "",
                rally_x=int(parts[6]) if len(parts) > 6 else bx,
                rally_y=int(parts[7]) if len(parts) > 7 else by,
            ))
            last_hero = None

        elif pfx == b"E":
            # E:id:x:y:hp[:z]
            if len(parts) < 4:
                continue
            state.enemies.append(UnitRecord(
                id=parts[0].decode(),
                x=int(parts[1]),
                y=int(parts[2]),
                hp=int(parts[3]),
                z=float(parts[4]) if len(parts) > 4 else 0.0,
            ))
            last_hero = None

        elif pfx == b"N":
            # N:id:x:y:hp[:z[:gold[:workers]]]
            if len(parts) < 4:
                continue
            state.neutral.append(UnitRecord(
                id=parts[0].decode(),
                x=int(parts[1]),
                y=int(parts[2]),
                hp=int(parts[3]),
                z=float(parts[4]) if len(parts) > 4 else 0.0,
                gold=int(parts[5]) if len(parts) > 5 else 0,
                workers=int(parts[6]) if len(parts) > 6 else 0,
            ))
            last_hero = None

        elif pfx == b"D":
            # D:id:x:y:hp%
            if len(parts) < 4:
                continue
            state.destructibles.append(DestructibleRecord(
                id=parts[0].decode(),
                x=int(parts[1]),
                y=int(parts[2]),
                hp=int(parts[3]),
            ))

        elif pfx == b"TK":
            # TK:x:y:level
            if len(parts) < 3:
                continue
            state.cliff_samples.append(CliffRecord(
                x=int(parts[0]),
                y=int(parts[1]),
                level=int(parts[2]),
            ))

        elif pfx == b"SEL":
            # SEL:id1,id2,...  (comma-separated type IDs)
            state.selected = [s.decode() for s in rest.split(b",") if s]

    return state


# ── Formatter ─────────────────────────────────────────────────────────────────

def fmt_state(s: GameState) -> str:
    """Format a GameState as a human-readable string."""
    neutral_ids = [n.id for n in s.neutral]
    gh = f"  GH=({s.gh_x},{s.gh_y})" if s.gh_x is not None else ""

    hero_strs = []
    for h in s.heroes:
        items = ",".join(i.id for i in h.items) if h.items else "-"
        hero_strs.append(
            f"{h.id}(L{h.level} {h.mana}/{h.max_mana}mp hp={h.hp}% items=[{items}])"
        )
    heroes_part = "  heroes=[" + ";".join(hero_strs) + "]" if hero_strs else ""

    sel_part = f"  sel={s.selected}" if s.selected else ""

    return (
        f"T={s.time:6.1f}  G={s.gold:>5}  L={s.lumber:>4}"
        f"  food={s.food_used}/{s.food_cap}"
        f"  units={len(s.units)}  bldgs={len(s.buildings)}"
        f"  enemies={len(s.enemies)}"
        + gh
        + (f"  neutral={neutral_ids}" if neutral_ids else "")
        + heroes_part
        + (f"  destr={len(s.destructibles)}" if s.destructibles else "")
        + sel_part
    )
