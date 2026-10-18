---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/sidecar_reader.py"
ref: HEAD
---

#!/usr/bin/env python3
"""sidecar_reader.py — Read WC3 sidecar game state from Lua heap.

Architecture
────────────
Two-phase approach:

  FULL SCAN  (~1-2s, runs at startup and when hot cache misses):
    Scans all readable regions, collects every region that contains a
    sidecar string, records the maximum T value seen per region.
    Regions are sorted by max-T descending → newest strings first.
    Returns the globally highest-T match.

  HOT POLL  (~0.05-0.2s, runs every interval):
    Re-reads only the cached "hot regions" (those that contained recent
    strings in the last full scan).  Picks the highest-T match.
    If nothing newer than last_t is found, triggers a fresh full scan.

This avoids the 360-stale-string problem (Lua GC is lazy) by always
picking the HIGHEST T value, and stays fast by re-scanning only the
small set of regions where current-game strings live.

Usage
─────
    python3 scripts/sidecar_reader.py
    python3 scripts/sidecar_reader.py --interval 0.1   # 10 Hz (default)
    python3 scripts/sidecar_reader.py --json           # JSON lines output
"""
import argparse, ctypes, ctypes.util, json, re, struct, subprocess, sys, time

TARGET = "Warcraft III"

SIDECAR_RE = re.compile(
    rb'T:(\d+\.?\d*)\|G:(\d+)\|L:(\d+)\|FU:(\d+)\|FC:(\d+)'
    rb'(?:\|GH:(-?\d+):(-?\d+))?'
    rb'(?:\|CAM:(-?\d+):(-?\d+):(-?\d+\.?\d*):(-?\d+\.?\d*):(-?\d+\.?\d*):(-?\d+\.?\d*))?'
    rb'((?:\|[UBEN]:[a-zA-Z0-9]{4}:-?\d+:-?\d+:\d+)*)',
    re.ASCII,
)

# ── Mach API ──────────────────────────────────────────────────────────────────

libc = ctypes.CDLL(ctypes.util.find_library("c"))
libc.task_for_pid.restype  = ctypes.c_int
libc.task_for_pid.argtypes = [ctypes.c_uint, ctypes.c_int, ctypes.POINTER(ctypes.c_uint)]
libc.mach_task_self.restype  = ctypes.c_uint
libc.mach_task_self.argtypes = []
libc.mach_vm_read_overwrite.restype  = ctypes.c_int
libc.mach_vm_read_overwrite.argtypes = [
    ctypes.c_uint, ctypes.c_uint64, ctypes.c_uint64,
    ctypes.c_uint64, ctypes.POINTER(ctypes.c_uint64),
]

class _VMInfo(ctypes.Structure):
    _fields_ = [
        ("protection",       ctypes.c_int),
        ("max_protection",   ctypes.c_int),
        ("inheritance",      ctypes.c_uint),
        ("shared",           ctypes.c_int),
        ("reserved",         ctypes.c_int),
        ("offset",           ctypes.c_uint64),
        ("behavior",         ctypes.c_int),
        ("user_wired_count", ctypes.c_ushort),
    ]

libc.mach_vm_region.restype  = ctypes.c_int
libc.mach_vm_region.argtypes = [
    ctypes.c_uint, ctypes.POINTER(ctypes.c_uint64),
    ctypes.POINTER(ctypes.c_uint64), ctypes.c_int,
    ctypes.POINTER(_VMInfo), ctypes.POINTER(ctypes.c_uint), ctypes.POINTER(ctypes.c_uint),
]


def get_task(pid: int) -> int:
    t = ctypes.c_uint(0)
    r = libc.task_for_pid(libc.mach_task_self(), pid, ctypes.byref(t))
    assert r == 0, f"task_for_pid failed kr={r}"
    return t.value


def read_mem(task: int, addr: int, size: int) -> bytes | None:
    if size <= 0:
        return b""
    buf = (ctypes.c_char * size)()
    out = ctypes.c_uint64(0)
    kr  = libc.mach_vm_read_overwrite(task, addr, size,
                                       ctypes.addressof(buf), ctypes.byref(out))
    return bytes(buf[:out.value]) if kr == 0 else None


def iter_regions(task: int):
    addr = ctypes.c_uint64(1)
    while True:
        size = ctypes.c_uint64(0)
        info = _VMInfo(); cnt = ctypes.c_uint(9); obj = ctypes.c_uint(0)
        kr = libc.mach_vm_region(task, ctypes.byref(addr), ctypes.byref(size), 9,
                                  ctypes.byref(info), ctypes.byref(cnt), ctypes.byref(obj))
        if kr != 0:
            break
        yield addr.value, size.value, info.protection
        addr.value += size.value


def read_region(task: int, base: int, size: int) -> bytes:
    CHUNK = 4 * 1024 * 1024
    parts, off = [], 0
    while off < size:
        n     = min(CHUNK, size - off)
        chunk = read_mem(task, base + off, n)
        parts.append(chunk if chunk else b"\x00" * n)
        off += n
    return b"".join(parts)


# ── Sidecar search ────────────────────────────────────────────────────────────

def best_match_in(data: bytes):
    """Return (T_value, match) with the highest T found in data, or None."""
    best_m, best_t = None, -1.0
    pos = 0
    while True:
        m = SIDECAR_RE.search(data, pos)
        if not m:
            break
        t = float(m.group(1))
        if t > best_t:
            best_t, best_m = t, m
        pos = m.start() + 1
    return (best_t, best_m) if best_m else None


# ── Scanner ───────────────────────────────────────────────────────────────────

class SidecarScanner:
    """
    Maintains a cache of "hot regions" — memory regions that recently
    contained sidecar strings — for fast incremental polling.
    """

    # Regions larger than this are skipped (huge anonymous mappings / GPU memory).
    MAX_REGION = 512 * 1024 * 1024

    def __init__(self, task: int):
        self.task        = task
        # hot_regions: list of (base, size, last_max_t), sorted by last_max_t desc
        self.hot_regions: list[tuple[int, int, float]] = []
        self.last_t      = -1.0

    # ── full scan ─────────────────────────────────────────────────────────────

    def full_scan(self, verbose: bool = True) -> bytes | None:
        """
        Read every eligible readable region.  Update hot_regions cache.
        Return the raw bytes of the highest-T match found, or None.
        """
        if verbose:
            print("  Full heap scan...", flush=True)
        t0 = time.time()

        new_hot: dict[tuple[int,int], float] = {}
        best_raw, best_t = None, -1.0

        for base, size, prot in iter_regions(self.task):
            if not (prot & 1):
                continue
            if size > self.MAX_REGION:
                continue
            data = read_region(self.task, base, size)
            hit  = best_match_in(data)
            if hit is None:
                continue
            t, m = hit
            new_hot[(base, size)] = t
            if t > best_t:
                best_t, best_raw = t, m.group(0)

        # Keep hot_regions that had a string within 120s of the global best
        cutoff = best_t - 120.0
        self.hot_regions = sorted(
            [(b, s, t) for (b, s), t in new_hot.items() if t >= cutoff],
            key=lambda r: r[2], reverse=True,
        )

        if verbose:
            elapsed = time.time() - t0
            if best_raw:
                print(f"  Found T={best_t:.1f}  {len(self.hot_regions)} hot regions"
                      f"  ({elapsed:.2f}s)", flush=True)
            else:
                print(f"  Not found ({elapsed:.2f}s)"
                      f" — is sidecar_bot.w3x loaded and game started?", flush=True)

        if best_raw:
            self.last_t = best_t
        return best_raw

    # ── hot poll ──────────────────────────────────────────────────────────────

    def hot_poll(self) -> bytes | None:
        """
        Fast poll: scan only cached hot regions.
        Returns raw bytes of the best (highest-T) match that is NEWER
        than last_t, or None (triggering a full rescan by the caller).
        """
        best_raw, best_t = None, self.last_t

        for base, size, cached_t in self.hot_regions:
            # Skip regions whose cached max-T is more than 5s behind our best —
            # they're unlikely to have anything newer.
            if cached_t < best_t - 5.0:
                break
            data = read_region(self.task, base, size)
            hit  = best_match_in(data)
            if hit is None:
                continue
            t, m = hit
            if t > best_t:
                best_t, best_raw = t, m.group(0)

        if best_raw:
            self.last_t = best_t
        return best_raw

    # ── fast liveness refresh ─────────────────────────────────────────────────

    def refresh_live(self) -> bytes | None:
        """
        Quick hot-miss recovery: re-read only the current hot_regions, verify
        T is still advancing (live game), and return the best live raw bytes.

        Unlike full_scan, this never touches stale regions from a previous game,
        so it cannot poison hot_regions with stale T values.

        Returns None if the live regions have gone cold (game ended or strings
        migrated to new regions); the caller should then fall back to
        find_live_game() for a fresh full search.
        """
        if not self.hot_regions:
            return None

        # Scan 1: max T per current hot region
        region_t1: dict[tuple[int, int], float] = {}
        for base, size, _ in self.hot_regions:
            data = read_region(self.task, base, size)
            hit  = best_match_in(data)
            if hit:
                region_t1[(base, size)] = hit[0]

        time.sleep(0.25)   # game writes every 0.1 s → expect ≥ 0.1 s advance

        # Scan 2: accept only regions where T advanced
        best_raw, best_t = None, -1.0
        new_hot: dict[tuple[int, int], float] = {}

        for (base, size), t1 in region_t1.items():
            data = read_region(self.task, base, size)
            if not data:
                continue
            hit = best_match_in(data)
            if hit is None:
                continue
            t2, m = hit
            if t2 > t1 + 0.05:
                new_hot[(base, size)] = t2
                if t2 > best_t:
                    best_t, best_raw = t2, m.group(0)

        if not best_raw:
            return None   # hot regions went cold

        cutoff = best_t - 120.0
        self.hot_regions = sorted(
            [(b, s, t) for (b, s), t in new_hot.items() if t >= cutoff],
            key=lambda r: r[2], reverse=True,
        )
        self.last_t = best_t
        return best_raw

    # ── liveness scan ─────────────────────────────────────────────────────────

    def find_live_game(self, verbose: bool = True) -> bytes | None:
        """
        Detect a live game by scanning the same memory regions twice, 350 ms
        apart, and checking whether the MAX T VALUE WITHIN EACH REGION advanced.

        Stale strings from a finished game have frozen T in their regions
        (the Lua VM stopped writing).  A live game's regions show T increasing
        at ~0.1 s / tick.  Cross-region comparison (old approach) fails because
        the stale corpus spans all T values 0.1…32.8, creating false positives.

        Returns raw bytes of the live state, or None if no game is active.
        Updates hot_regions and last_t on success.
        """
        if verbose:
            print("  Scanning for live game…", flush=True)

        # ── Scan 1: record max T per region ──────────────────────────────────
        region_t1: dict[tuple[int, int], float] = {}

        for base, size, prot in iter_regions(self.task):
            if not (prot & 1):
                continue
            if size > self.MAX_REGION:
                continue
            data = read_region(self.task, base, size)
            hit  = best_match_in(data)
            if hit:
                region_t1[(base, size)] = hit[0]

        if not region_t1:
            return None

        time.sleep(0.35)

        # ── Scan 2: accept regions where T advanced within that region ────────
        best_raw, best_t = None, -1.0
        new_hot: dict[tuple[int, int], float] = {}

        for (base, size), t1 in region_t1.items():
            data = read_region(self.task, base, size)
            if not data:
                continue
            hit = best_match_in(data)
            if hit is None:
                continue
            t2, m = hit
            # Live: the highest T in this specific region increased by ≥ 0.05 s.
            # Stale regions: T frozen → t2 == t1 → not live.
            if t2 > t1 + 0.05:
                new_hot[(base, size)] = t2
                if t2 > best_t:
                    best_t, best_raw = t2, m.group(0)

        if not best_raw:
            return None

        cutoff = best_t - 120.0
        self.hot_regions = sorted(
            [(b, s, t) for (b, s), t in new_hot.items() if t >= cutoff],
            key=lambda r: r[2], reverse=True,
        )
        self.last_t = best_t
        if verbose:
            print(f"  Live game at T={best_t:.1f}  "
                  f"{len(self.hot_regions)} hot regions", flush=True)
        return best_raw


# ── State parsing ─────────────────────────────────────────────────────────────

def parse_raw(raw: bytes) -> dict | None:
    m = SIDECAR_RE.match(raw)
    if not m:
        return None
    gh_x = int(m.group(6)) if m.group(6) is not None else None
    gh_y = int(m.group(7)) if m.group(7) is not None else None
    # Groups 8-13: optional CAM fields (cam_tx, cam_ty, cam_aoa, cam_fov, cam_dist, cam_rot)
    cam_tx   = int(m.group(8))     if m.group(8)  is not None else None
    cam_ty   = int(m.group(9))     if m.group(9)  is not None else None
    cam_aoa  = float(m.group(10))  if m.group(10) is not None else None
    cam_fov  = float(m.group(11))  if m.group(11) is not None else None
    cam_dist = float(m.group(12))  if m.group(12) is not None else None
    cam_rot  = float(m.group(13))  if m.group(13) is not None else None
    state: dict = {
        "time":      float(m.group(1)),
        "gold":      int(m.group(2)),
        "lumber":    int(m.group(3)),
        "food_used": int(m.group(4)),
        "food_cap":  int(m.group(5)),
        "gh_x":      gh_x,    # GH game coord X (from start location)
        "gh_y":      gh_y,    # GH game coord Y (from start location)
        "cam_tx":    cam_tx,  # camera target X (world units)
        "cam_ty":    cam_ty,  # camera target Y (world units)
        "cam_aoa":   cam_aoa, # angle of attack (WC3 degrees, default ≈304)
        "cam_fov":   cam_fov, # vertical FOV (degrees)
        "cam_dist":  cam_dist,# distance from target (world units)
        "cam_rot":   cam_rot, # azimuth (degrees, 90=faces North)
        "units":     [],
        "buildings": [],
        "enemies":   [],
        "neutral":   [],
    }
    tail = m.group(14)
    if tail:
        for entry in tail.split(b"|"):
            if not entry:
                continue
            parts = entry.split(b":")
            if len(parts) != 5:
                continue
            pfx, uid, x, y, hp = parts
            rec = {"id": uid.decode(), "x": int(x), "y": int(y), "hp": int(hp)}
            {"U": state["units"], "B": state["buildings"],
             "E": state["enemies"], "N": state["neutral"]}.get(
                pfx.decode(), []
            ).append(rec)
    return state


def fmt_state(s: dict) -> str:
    neutral_ids = [n["id"] for n in s.get("neutral", [])]
    gh = f"  GH=({s['gh_x']},{s['gh_y']})" if s.get("gh_x") is not None else ""
    return (f"T={s['time']:6.1f}  G={s['gold']:>5}  L={s['lumber']:>4}"
            f"  food={s['food_used']}/{s['food_cap']}"
            f"  units={len(s['units'])}  bldgs={len(s['buildings'])}"
            f"  enemies={len(s['enemies'])}"
            + gh
            + (f"  neutral={neutral_ids}" if neutral_ids else ""))


# ── Main ──────────────────────────────────────────────────────────────────────

def main():
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--interval", type=float, default=0.1,
                    help="Poll interval in seconds (default 0.1)")
    ap.add_argument("--json", action="store_true",
                    help="Output JSON lines")
    args = ap.parse_args()

    result = subprocess.run(["pgrep", "-x", TARGET], capture_output=True, text=True)
    if result.returncode != 0:
        print(f"error: '{TARGET}' not running", file=sys.stderr)
        return 1
    pid  = int(result.stdout.strip().split("\n")[0])
    task = get_task(pid)
    if not args.json:
        print(f"Attached to {TARGET}  PID={pid}")

    scanner = SidecarScanner(task)

    def wait_for_live_game(first: bool = True) -> tuple[dict, bytes]:
        """Block until a live game is detected; return (state, raw)."""
        label = "Searching for live game" if first else "Waiting for new game"
        while True:
            raw = scanner.find_live_game(verbose=not args.json)
            if raw is not None:
                state = parse_raw(raw)
                if state is not None:
                    return state, raw
            if not args.json:
                print(f"  [{label}… no active game found, retrying in 3s]",
                      flush=True)
            time.sleep(3)

    # ── Find initial live game ─────────────────────────────────────────────────
    last_state, _ = wait_for_live_game(first=True)

    if args.json:
        print(json.dumps(last_state), flush=True)
    else:
        print(f"Live: {fmt_state(last_state)}")
        print(f"Polling at up to {1/args.interval:.0f} Hz  "
              f"({len(scanner.hot_regions)} hot regions cached)\n")

    # ── Poll loop ──────────────────────────────────────────────────────────────
    # hot_misses: consecutive hot_poll() calls with no newer T found.
    # After HOT_MISS_LIMIT we run refresh_live() (re-reads hot regions +
    # liveness check) instead of full_scan(), so stale regions from a previous
    # game can never be injected back into the hot-region cache.
    # If refresh_live() also returns nothing, we fall back to find_live_game()
    # for a full search.  After GAME_END_LIMIT consecutive find_live_game()
    # failures we declare the game over.
    HOT_MISS_LIMIT  = 20   # ~2 s of hot-poll misses before refresh
    GAME_END_LIMIT  =  4   # 4 × find_live_game failures → game ended

    hot_misses      = 0
    game_end_misses = 0

    try:
        while True:
            time.sleep(args.interval)

            raw = scanner.hot_poll()

            if raw is None:
                hot_misses += 1
                if hot_misses < HOT_MISS_LIMIT:
                    continue
                hot_misses = 0

                # Fast refresh: only re-reads current hot regions
                raw = scanner.refresh_live()
                if raw is not None:
                    game_end_misses = 0
                else:
                    # Hot regions went cold → full liveness scan
                    raw = scanner.find_live_game(verbose=False)
                    if raw is None:
                        game_end_misses += 1
                        if game_end_misses >= GAME_END_LIMIT:
                            game_end_misses = 0
                            if not args.json:
                                print("  [game ended — waiting for new game…]",
                                      flush=True)
                            last_state, _ = wait_for_live_game(first=False)
                            if args.json:
                                print(json.dumps(last_state), flush=True)
                            else:
                                print(f"New game: {fmt_state(last_state)}\n")
                        continue
                    game_end_misses = 0

            hot_misses = 0
            state = parse_raw(raw)
            if state is None or state == last_state:
                continue

            last_state = state
            if args.json:
                print(json.dumps(state), flush=True)
            else:
                print(fmt_state(state))

    except KeyboardInterrupt:
        print("\nStopped.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
