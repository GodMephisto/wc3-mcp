---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "src/wc3bot/observe/lua_reader.py"
ref: HEAD
---

"""wc3bot.observe.lua_reader — Fast WC3BOT_STATE reader via Lua pointer walking.

Architecture (Option B)
───────────────────────
Bootstrap (~2-5 s, once per WC3 session):
  1. Scan all readable memory for b"WC3BOT_STATE\\x00" → key TString address.
  2. Scan all readable memory for 8-byte pointers to that TString address
     followed by key tt_ = 0x44 (LUA_VSHRSTR collectable) → find the Lua
     global-table Node whose i_key points to "WC3BOT_STATE".
  3. Cache the Node base address.  The global table Node lives for the whole
     WC3 session — it never moves or gets GC'd.

Hot read (~1-5 ms, ~10 Hz):
  1. Read 16 bytes at node_addr  → val_ptr (8 bytes) + val_tt (4 bytes).
  2. Read 32 bytes at val_ptr    → TString header (to determine string length).
  3. Read <length> bytes at val_ptr + 24 → raw game-state bytes.
  Total: 3 x mach_vm_read calls.  No GC fragment issues, no full-heap scan.

Lua 5.4 TString memory layout (both short and long strings, offset from base):
  +0   next     (8 bytes, GCObject*)
  +8   tt       (1 byte, GCObject.tt:  0x04=short, 0x14=long)
  +9   marked   (1 byte)
  +10  extra    (1 byte)
  +11  shrlen   (1 byte, string length for SHORT strings only)
  +12  hash     (4 bytes, unsigned int)
  +16  u        (8 bytes: lnglen size_t for long; hnext TString* for short)
  +24  char data[] starts here (null-terminated)

TValue layout (16 bytes):
  +0   value_  (8 bytes, union — for GC objects: pointer to GCObject)
  +8   tt_     (4 bytes, int — 0x44=short string, 0x54=long string)
  +12  padding (4 bytes)

Node layout (32+ bytes):
  +0   i_val   TValue (16 bytes) — the value stored in the table slot
  +16  i_key   TKey (16+ bytes):
       +16  value_ (8 bytes, pointer to key TString for string keys)
       +24  tt_    (4 bytes, 0x44 for short-string key)
       +28  next   (4 bytes, Node* chain index)

Public API
──────────
LuaStateReader(task)          — bootstrap: find Node; raises if not found
  .hot_poll()  -> bytes|None  — fast read (≤5 ms); None = Lua wrote nil/not yet
  .find_live_game(verbose)    — detect liveness (two reads 350 ms apart)
  .full_scan(verbose)         — alias for find_live_game (API compat)
  .read_state() -> GameState|None
"""
from __future__ import annotations

import struct
import time
import json
import threading
from pathlib import Path
from typing import Iterator

from wc3bot.observe.state import SIDECAR_RE, parse_state, GameState
from wc3bot.observe.mem_utils import _read_mem, _iter_regions, _read_region, _MAX_REGION
from wc3bot.observe.mine_workers import MineWorkerReader

# ── Node address cache (survives across script runs within same WC3 session) ──
_CACHE_PATH = Path.home() / ".wc3bot_lua_node_cache.json"


def _load_node_cache(pid: int) -> int | None:
    """Return cached node_addr if it was saved for the current WC3 PID."""
    try:
        data = json.loads(_CACHE_PATH.read_text())
        if data.get("pid") == pid:
            return int(data["node_addr"])
    except Exception:
        pass
    return None


def _save_node_cache(pid: int, node_addr: int) -> None:
    try:
        _CACHE_PATH.write_text(json.dumps({"pid": pid, "node_addr": node_addr}))
    except Exception:
        pass


# ── Bootstrap helpers ─────────────────────────────────────────────────────────

_KEY_BYTES = b"WC3BOT_STATE\x00"   # 13 bytes: null-terminated key string

# TString header offsets to probe (content start = TString_ptr + offset).
# WC3 Reforged Lua uses 32 bytes; standard Lua 5.4 uses 24 bytes.
# We probe all candidates so the code works across Lua builds.
_TSTR_HDR_CANDIDATES = (32, 24, 28, 20, 36, 16, 40)

_KEY_TT  = 0x44                    # ctb(LUA_VSHRSTR): short string in TValue
_VAL_TT_STR = frozenset((0x44, 0x54))  # short or long string in TValue

# Narrow address range where WC3 Reforged Lua objects live on macOS ARM.
# Observed addresses: keys 0x130…–0x15f…, nodes 0x14d…–0x165…
# Extend to 0x180000000 to cover nodes above 0x160000000 without full scan.
_NARROW_LO = 0x130000000
_NARROW_HI = 0x180000000


def _iter_regions_in_range(task: int, lo: int, hi: int):
    """Yield (base, size, protection) for VM regions overlapping [lo, hi)."""
    for base, size, prot in _iter_regions(task):
        region_end = base + size
        if region_end <= lo:
            continue
        if base >= hi:
            break
        # Clamp to [lo, hi)
        clamped_base = max(base, lo)
        clamped_size = min(region_end, hi) - clamped_base
        if clamped_size > 0:
            yield clamped_base, clamped_size, prot


def _scan_for_bytes(task: int, needle: bytes,
                    lo: int | None = None,
                    hi: int | None = None) -> list[int]:
    """Return all addresses where needle appears in readable VM regions.
    If lo/hi are given, restrict to that address range (fast path)."""
    addrs = []
    regions = (
        _iter_regions_in_range(task, lo, hi)
        if lo is not None and hi is not None
        else _iter_regions(task)
    )
    for base, size, prot in regions:
        if not (prot & 1):
            continue
        if size > _MAX_REGION:
            continue
        data = _read_region(task, base, size)
        off = 0
        while True:
            idx = data.find(needle, off)
            if idx == -1:
                break
            addr = base + idx
            if addr > 0:
                addrs.append(addr)
            off = idx + 1
    return addrs


def _find_key_tstring_addrs(task: int) -> list[int]:
    """
    Scan for WC3BOT_STATE key string content — narrow range first, then full scan.
    Returns list of RAW CONTENT addresses (where "WC3BOT_STATE\\x00" starts).
    """
    addrs = _scan_for_bytes(task, _KEY_BYTES, _NARROW_LO, _NARROW_HI)
    if not addrs:
        # Narrow range miss — fall back to full heap scan
        addrs = _scan_for_bytes(task, _KEY_BYTES)
    return addrs


def _find_node_addr(task: int, key_content_addrs: list[int],
                    verbose: bool = False) -> int | None:
    """
    Scan for an 8-byte pointer to a WC3BOT_STATE TString + tt_=0x44 byte.
    Tries narrow range first, falls back to full scan.
    """
    # Build the full set of candidate TString-base addresses to search for
    target_set: set[int] = set()
    for ca in key_content_addrs:
        for off in _TSTR_HDR_CANDIDATES:
            tstr_addr = ca - off
            if tstr_addr > 0:
                target_set.add(tstr_addr)

    def _scan_regions_for_node(regions) -> list[int]:
        found = []
        for base, size, prot in regions:
            if not (prot & 1):
                continue
            if size > _MAX_REGION:
                continue
            data = _read_region(task, base, size)
            for i in range(0, len(data) - 12, 4):
                if data[i + 8] != _KEY_TT:
                    continue
                ptr = struct.unpack_from("<Q", data, i)[0]
                if ptr not in target_set:
                    continue
                if i < 16:
                    continue
                node_base = base + i - 16
                hdr = _read_mem(task, node_base, 16)
                if hdr and len(hdr) >= 12 and hdr[8] in _VAL_TT_STR:
                    found.append(node_base)
        return found

    # Try narrow range first
    candidates = _scan_regions_for_node(
        _iter_regions_in_range(task, _NARROW_LO, _NARROW_HI)
    )
    if not candidates:
        # Fall back to full scan
        candidates = _scan_regions_for_node(_iter_regions(task))

    if not candidates:
        return None

    # Prefer confirmed node (i_val already contains sidecar data)
    for node_base in candidates:
        hdr = _read_mem(task, node_base, 16)
        if not hdr or len(hdr) < 9:
            continue
        val_ptr = struct.unpack_from("<Q", hdr, 0)[0]
        if not val_ptr:
            continue
        chunk = _read_mem(task, val_ptr, 16384)
        if chunk and SIDECAR_RE.search(chunk):
            return node_base

    return candidates[0]


def _find_node_addr_parallel(task: int, key_content_addrs: list[int],
                              n_threads: int = 4,
                              verbose: bool = False) -> list[int]:
    """
    Parallel scan for ALL structurally-valid WC3BOT_STATE nodes.
    Returns a list sorted live-first (nodes with live sidecar data come first).
    Collects every candidate — never stops early — so that both the pre-game
    node and the in-game node are found even when the game hasn't started yet.
    """
    target_set: set[int] = set()
    for ca in key_content_addrs:
        for off in _TSTR_HDR_CANDIDATES:
            tstr_addr = ca - off
            if tstr_addr > 0:
                target_set.add(tstr_addr)

    all_regions = list(_iter_regions_in_range(task, _NARROW_LO, _NARROW_HI))
    if not all_regions:
        all_regions = list(_iter_regions(task))

    # Interleave regions so each thread covers the full address space evenly.
    chunks = [all_regions[i::n_threads] for i in range(n_threads)]

    all_found: list[int] = []
    lock = threading.Lock()

    def worker(regions: list) -> None:
        local: list[int] = []
        for base, size, prot in regions:
            if not (prot & 1):
                continue
            if size > _MAX_REGION:
                continue
            data = _read_region(task, base, size)
            for i in range(0, len(data) - 12, 4):
                if data[i + 8] != _KEY_TT:
                    continue
                ptr = struct.unpack_from("<Q", data, i)[0]
                if ptr not in target_set:
                    continue
                if i < 16:
                    continue
                node_base = base + i - 16
                hdr = _read_mem(task, node_base, 16)
                if not hdr or len(hdr) < 12 or hdr[8] not in _VAL_TT_STR:
                    continue
                local.append(node_base)
        if local:
            with lock:
                all_found.extend(local)

    threads = [threading.Thread(target=worker, args=(c,), daemon=True)
               for c in chunks if c]
    for t in threads:
        t.start()
    for t in threads:
        t.join()

    # Sort: live (contains sidecar data) first, cold (nil/empty) last.
    live, cold = [], []
    for nb in all_found:
        hdr = _read_mem(task, nb, 16)
        if not hdr or len(hdr) < 9:
            continue
        val_ptr = struct.unpack_from("<Q", hdr, 0)[0]
        if val_ptr:
            chunk = _read_mem(task, val_ptr, 16384)
            if chunk and SIDECAR_RE.search(chunk):
                live.append(nb)
                continue
        cold.append(nb)

    return live + cold


# ── LuaStateReader ────────────────────────────────────────────────────────────

class LuaStateReader:
    """
    Fast WC3BOT_STATE reader via cached Lua global-table Node pointer.

    Drop-in replacement for SidecarScanner in test_primitives.py.
    """

    def __init__(self, task: int, pid: int | None = None):
        self.task        = task
        self.pid         = pid
        self.node_addr:  int | None  = None   # primary (last confirmed live) node
        self.node_addrs: list[int]   = []     # all known candidate nodes
        self.last_t      = -1.0
        self._mine_worker_reader = MineWorkerReader(task)
        # Cached key-content address for the "WC3BOT_STATE" string.
        # Stays constant within a WC3 session; lets re-bootstraps skip the string scan.
        self._key_addrs: list[int] | None = None

        # Fast path: restore the last confirmed-live node from disk cache.
        if pid is not None:
            cached = _load_node_cache(pid)
            if cached is not None:
                hdr = _read_mem(self.task, cached, 16)
                if hdr and len(hdr) >= 9:
                    self.node_addr  = cached
                    self.node_addrs = [cached]
                    print(f"  [LuaReader] Node restored from cache @ 0x{cached:x}",
                          flush=True)
                    # Populate key-addr cache in background (completes in ~0.6s;
                    # done before find_live_game() needs to re-bootstrap).
                    def _bg_key_scan():
                        addrs = _find_key_tstring_addrs(self.task)
                        if addrs:
                            self._key_addrs = addrs
                    threading.Thread(target=_bg_key_scan, daemon=True).start()
                    return

        self._bootstrap()

    def _bootstrap(self, verbose: bool = True) -> bool:
        """Full memory scan to find and cache the Node address.  Returns True on success."""
        if verbose:
            print("  [LuaReader] bootstrap: searching for WC3BOT_STATE node…",
                  flush=True)
        t0 = time.time()

        # Reuse cached key address if available — the interned Lua string "WC3BOT_STATE"
        # stays at the same memory address for the whole WC3 session.
        if self._key_addrs:
            key_addrs = self._key_addrs
            if verbose:
                print(f"  [LuaReader] reusing cached key addr(s): "
                      + ", ".join(f"0x{a:x}" for a in key_addrs[:4]),
                      flush=True)
        else:
            key_addrs = _find_key_tstring_addrs(self.task)
            if not key_addrs:
                if verbose:
                    print("  [LuaReader] key string not found — is sidecar running?",
                          flush=True)
                return False
            self._key_addrs = key_addrs
            if verbose:
                print(f"  [LuaReader] found {len(key_addrs)} key content addr(s): "
                      + ", ".join(f"0x{a:x}" for a in key_addrs[:4]),
                      flush=True)

        nodes = _find_node_addr_parallel(self.task, key_addrs, verbose=verbose)
        if not nodes:
            if verbose:
                print("  [LuaReader] Node not found (key TStrings found but no Node pointer)",
                      flush=True)
            return False

        # Merge newly found candidates (avoid duplicates).
        existing = set(self.node_addrs)
        for n in nodes:
            if n not in existing:
                self.node_addrs.append(n)
                existing.add(n)

        # Primary node: live candidate first, else first candidate.
        self.node_addr = self.node_addrs[0]
        elapsed = time.time() - t0
        if verbose:
            addrs_str = ", ".join(f"0x{a:x}" for a in self.node_addrs[:4])
            print(f"  [LuaReader] {len(self.node_addrs)} node(s): [{addrs_str}]  ({elapsed:.1f}s)",
                  flush=True)

        # Note: disk cache is written only after the live node is CONFIRMED in
        # find_live_game() — not here — so we never cache a pre-game stale node.

        return True

    # ── Hot poll ──────────────────────────────────────────────────────────────

    def hot_poll_node(self, node_addr: int) -> bytes | None:
        """Fast 3-read poll for an explicit node address."""
        node_hdr = _read_mem(self.task, node_addr, 16)
        if not node_hdr or len(node_hdr) < 9:
            return None
        val_ptr = struct.unpack_from("<Q", node_hdr, 0)[0]
        val_tt  = node_hdr[8]
        if not val_ptr or val_tt not in _VAL_TT_STR:
            return None
        raw = _read_mem(self.task, val_ptr, 16384)
        if not raw:
            return None
        m = SIDECAR_RE.search(raw)
        return m.group(0) if m else None

    def hot_poll(self) -> bytes | None:
        """
        Fast 3-read poll via primary cached Node address.

        Returns raw game-state bytes matched by SIDECAR_RE, or None.
        """
        if self.node_addr is None:
            return None
        return self.hot_poll_node(self.node_addr)

    # ── Liveness detection ────────────────────────────────────────────────────

    def find_live_game(self, verbose: bool = True,
                       timeout: float = 60.0,
                       keep_alive_fn=None,
                       keep_alive_interval: float = 4.0) -> bytes | None:
        """
        Poll ALL known candidate nodes simultaneously until one shows T advancing.

        Architecture
        ────────────
        A single scan often finds 2 nodes: a pre-game Lua state node (nil value)
        and the in-game node.  By monitoring both, we detect whichever becomes live
        first without guessing which one is correct.

        Re-bootstrap (background parallel scan) fires only when ALL nodes have
        returned nil for > 8s — meaning WC3 reloaded or a new session started.
        Nil on individual nodes is normal while the game is loading and is never
        treated as a fault.

        keep_alive_fn: called every keep_alive_interval seconds to keep WC3 in
          foreground (SP games pause on focus loss).
        """
        if verbose:
            print("  [LuaReader] waiting for live game…", flush=True)

        if not self.node_addrs:
            if not self._bootstrap(verbose=verbose):
                return None

        deadline          = time.time() + timeout
        last_keepalive    = time.time()
        rebootstrap_count = 0
        _MAX_REBOOTSTRAPS = 4

        # Per-node state: last seen T, frozen-since timestamp.
        node_state: dict[int, dict] = {
            addr: {"last_t": -999.0, "t_frozen_since": time.time()}
            for addr in self.node_addrs
        }

        # Tracks when ANY node last returned non-nil (to detect full session death).
        any_nonnull_since = time.time()

        # Background re-bootstrap (parallel scan) — runs when all nodes are nil.
        _bg_event  = threading.Event()
        _bg_active = [False]

        def _kick_bg_rebootstrap(reason: str) -> bool:
            nonlocal rebootstrap_count
            if _bg_active[0] or rebootstrap_count >= _MAX_REBOOTSTRAPS:
                return False
            rebootstrap_count += 1
            _bg_event.clear()
            _bg_active[0] = True
            if verbose:
                print(f"  [LuaReader] {reason} — background re-bootstrap "
                      f"#{rebootstrap_count}", flush=True)
            def _run():
                self._bootstrap(verbose=verbose)
                # Merge any newly discovered candidates into node_state.
                for addr in self.node_addrs:
                    if addr not in node_state:
                        node_state[addr] = {"last_t": -999.0,
                                            "t_frozen_since": time.time()}
                _bg_active[0] = False
                _bg_event.set()
            threading.Thread(target=_run, daemon=True).start()
            return True

        while time.time() < deadline:
            # Keep WC3 focused for SP mode.
            if keep_alive_fn is not None:
                now = time.time()
                if now - last_keepalive >= keep_alive_interval:
                    keep_alive_fn()
                    last_keepalive = time.time()

            # Consume completed background scan (updates node_state).
            if _bg_event.is_set():
                _bg_event.clear()
                any_nonnull_since = time.time()  # reset — scan found new candidates

            now = time.time()
            got_any_nonnull = False

            # Poll every known candidate node.
            for addr in list(node_state.keys()):
                ns  = node_state[addr]
                raw1 = self.hot_poll_node(addr)

                if raw1 is None:
                    continue   # nil is normal for pre-game / loading nodes

                got_any_nonnull  = True
                any_nonnull_since = now

                m1 = SIDECAR_RE.match(raw1)
                if m1 is None:
                    continue

                t1 = float(m1.group(1))

                # Drop nodes whose T has been frozen for > 5s (stale old-game data).
                if t1 != ns["last_t"]:
                    ns["last_t"] = t1
                    ns["t_frozen_since"] = now
                elif (now - ns["t_frozen_since"]) > 5.0:
                    if verbose:
                        print(f"  [LuaReader] 0x{addr:x} T frozen at {t1:.1f} — dropping",
                              flush=True)
                    del node_state[addr]
                    if addr in self.node_addrs:
                        self.node_addrs.remove(addr)
                    if self.node_addr == addr:
                        self.node_addr = self.node_addrs[0] if self.node_addrs else None
                    continue

                # Liveness check: read again 350 ms later to confirm T is advancing.
                time.sleep(0.35)
                raw2 = self.hot_poll_node(addr)
                if raw2 is None:
                    continue
                m2 = SIDECAR_RE.match(raw2)
                if m2 is None:
                    continue
                t2 = float(m2.group(1))

                if t2 > t1 + 0.05:
                    # Confirmed live node — pin it and save to disk cache.
                    self.node_addr  = addr
                    self.node_addrs = [addr]
                    self.last_t     = t2
                    if self.pid is not None:
                        _save_node_cache(self.pid, addr)
                    if verbose:
                        print(f"  [LuaReader] live at T={t2:.1f}", flush=True)
                    return raw2

            if not got_any_nonnull:
                # All nodes returned nil this sweep.
                # Kick re-bootstrap only when this has lasted > 8s (full session death).
                if (now - any_nonnull_since) > 8.0:
                    if _kick_bg_rebootstrap("all nodes nil for 8s"):
                        _bg_event.wait(timeout=max(0.1, deadline - now))
                        any_nonnull_since = time.time()

            time.sleep(0.3)

        if verbose:
            print("  [LuaReader] timeout — no live game found", flush=True)
        return None

    def full_scan(self, verbose: bool = True) -> bytes | None:
        """API-compatible alias for find_live_game (used by test_primitives.py)."""
        return self.find_live_game(verbose=verbose)

    # ── High-level ────────────────────────────────────────────────────────────

    def read_state(self) -> GameState | None:
        """Fast single read → GameState.  Does NOT check liveness."""
        raw = self.hot_poll()
        if raw is None:
            return None
        state = parse_state(raw)
        if state is None:
            return None
        self._update_mine_workers(state)
        return state

    def _update_mine_workers(self, state: GameState) -> None:
        """
        Replace the Lua-heuristic workers count in state.neutral with the exact
        value read directly from WC3 process memory (gold_addr + WORKER_OFFSET).

        Always calibrates against the GH-nearest mine so we don't accidentally
        lock onto a remote untouched mine that happens to share the same gold value.
        """
        mines = [n for n in state.neutral if n.id in ("ngol", "ngme")]
        if not mines:
            return

        # Nearest mine to GH — this is both the calibration target and the read target.
        if state.gh_x is not None and state.gh_y is not None:
            target = min(mines, key=lambda m: (m.x - state.gh_x)**2 + (m.y - state.gh_y)**2)
        else:
            target = mines[0]

        mwr = self._mine_worker_reader
        if not mwr.is_calibrated:
            if target.gold > 1000:
                # Use Lua heuristic workers as a minimum filter: if the sidecar
                # already sees at least 1 peon cycling, the real struct must have
                # workers >= 1, so we can immediately drop candidates with workers=0.
                lua_workers = target.workers   # Lua heuristic value (pre-override)
                min_w = 1 if lua_workers > 0 else 0
                mwr.calibrate(target.gold, expected_workers=-1,
                               min_workers=min_w, verbose=True)

        if not mwr.is_calibrated:
            return

        exact = mwr.read_workers(current_gold=target.gold)
        if exact is None:
            return

        target.workers = exact

    def poll_loop(self, interval: float = 0.1) -> Iterator[GameState]:
        """Yield a new GameState every time T advances.  Runs forever."""
        while True:
            state = self.read_state()
            if state is not None and state.time > self.last_t:
                self.last_t = state.time
                yield state
            time.sleep(interval)
