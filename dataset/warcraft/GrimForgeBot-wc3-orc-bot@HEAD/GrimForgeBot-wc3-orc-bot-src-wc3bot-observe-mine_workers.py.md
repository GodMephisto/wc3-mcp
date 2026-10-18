---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "src/wc3bot/observe/mine_workers.py"
ref: HEAD
---

"""wc3bot.observe.mine_workers — Direct memory read of WC3 mine worker count.

The WC3 UI shows "N/5" workers in a gold mine.  There is no JASS/Lua native
that exposes this value.  Reverse-engineering (find_mine_workers_offset.py)
found it at a fixed byte offset from the mine's gold-amount field:

    *(int32*)(gold_addr + WORKER_OFFSET)  ==  assigned_workers_count

Discovery session (2026-05-28):
    gold @ 0x136fc70d0  →  offset +48 tracked [0, 1, 3, 5] perfectly.

Usage:
    reader = MineWorkerReader(task)
    reader.calibrate(mine_gold)            # once at game start
    count = reader.read_workers(mine_gold) # fast, every poll tick; mine_gold
                                           # is used to eliminate false positives
"""
from __future__ import annotations

import struct
from typing import Optional

import numpy as np

from wc3bot.observe.mem_utils import _read_mem, _iter_regions, _MAX_REGION

# ── Constants ─────────────────────────────────────────────────────────────────

# Byte offset from the mine's gold int32 to the assigned-workers int32.
# Discovered by find_mine_workers_offset.py on 2026-05-28.
WORKER_OFFSET: int = 48

# On macOS/Apple Silicon, WC3 game object heap.
# Observed mine struct addresses: 0x136fc70d0, 0x144fefd1c — cover 0x100–0x1BF.
# Excludes thread stacks (0x160–0x17F) to avoid false positives.
_HEAP_SCAN_LO: int  = 0x100000000
_HEAP_SCAN_HI: int  = 0x15FFFFFFF   # stop before thread stacks
_HEAP_SCAN_LO2: int = 0x180000000   # upper heap (above stacks)
_HEAP_SCAN_HI2: int = 0x1BFFFFFFF


class MineWorkerReader:
    """
    Locates a gold mine's worker-count field in WC3 process memory and polls it.

    Lifecycle:
        1. Create once per bot session.
        2. Call calibrate(mine_gold) after the first sidecar read.
        3. Call read_workers(mine_gold) on every tick; pass the current Lua-reported
           gold so false positives can be eliminated over successive reads.
        4. Call invalidate() if the mine changes (e.g. expansion) or gold drops to 0.

    False-positive elimination:
        calibrate() may return multiple candidate addresses that all happen to contain
        the same gold value with a plausible [0,5] value at +WORKER_OFFSET.  On each
        read_workers(current_gold) call, candidates whose gold field has diverged from
        current_gold by more than _GOLD_TOLERANCE are dropped.  The real mine struct
        tracks Lua gold exactly; false positives stay frozen or drift independently.
        Once only one candidate remains it is promoted to the fast-path _worker_addr.
    """

    _GOLD_TOLERANCE: int = 15   # max delta between mem gold and Lua gold to keep candidate
    # Peons take exactly 10 gold per trip.  The real struct tracks Lua within ±0-10
    # (one delivery lag).  False positives diverge after 2+ trips.

    def __init__(self, task: int) -> None:
        self.task = task
        self._candidates: list[int] = []         # gold_addr for each still-valid candidate
        self._worker_addr: Optional[int] = None  # finalized address (fast path)

    # ── Public API ────────────────────────────────────────────────────────────

    def calibrate(self, mine_gold: int, expected_workers: int = -1,
                  min_workers: int = 0, verbose: bool = True) -> bool:
        """
        Scan heap memory for mine_gold as int32, verify the worker count field
        at +WORKER_OFFSET, and store all plausible candidate gold_addrs.

        expected_workers: if >= 0, only keep candidates whose worker-count field
            equals this value exactly.  Pass -1 (default) to accept any value in
            [min_workers, 5]; false positives are then eliminated dynamically via
            read_workers().

        min_workers: lower bound for the workers field when expected_workers=-1.
            Pass 1 when you know at least one peon is already assigned (Lua heuristic
            workers > 0) to immediately filter candidates with workers=0.

        Returns True if at least one candidate was found.
        """
        if mine_gold <= 0:
            if verbose:
                print("[MineWorkerReader] calibrate: mine_gold=0, skipping scan.")
            return False

        self._candidates = []
        self._worker_addr = None

        # Range: scan for [mine_gold - GOLD_DRIFT, mine_gold + GOLD_DRIFT] to tolerate
        # peon deliveries that change the struct's gold value during the ~2s scan.
        # Each delivery takes 10 gold; at 5 workers, ~3 deliveries/s → 30 gold drift.
        GOLD_DRIFT: int = 40
        gold_lo = mine_gold - GOLD_DRIFT
        gold_hi = mine_gold + GOLD_DRIFT
        found: list[int] = []   # gold_addr values

        def _scan_region(base: int, size: int) -> None:
            chunk = _read_mem(self.task, base, size)
            if not chunk or len(chunk) < 4:
                return
            # Round down to 4-byte boundary for aligned scan.
            align_off = (4 - (base % 4)) % 4
            if align_off >= len(chunk):
                return
            # Use numpy for fast vectorised range check across all aligned int32s.
            arr = np.frombuffer(chunk[align_off:], dtype="<i4")
            hits = np.where((arr >= gold_lo) & (arr <= gold_hi))[0]
            for i in hits:
                gold_abs = base + align_off + i * 4
                wb = _read_mem(self.task, gold_abs + WORKER_OFFSET, 4)
                if wb and len(wb) == 4:
                    wc = struct.unpack("<i", wb)[0]
                    if expected_workers >= 0:
                        if wc == expected_workers:
                            found.append(gold_abs)
                    elif min_workers <= wc <= 5:
                        found.append(gold_abs)

        # Phase 1: narrow-range heap scan (fast).
        for base, size, prot in _iter_regions(self.task):
            if not (prot & 1):
                continue
            in_range = (
                (_HEAP_SCAN_LO <= base <= _HEAP_SCAN_HI) or
                (_HEAP_SCAN_LO2 <= base <= _HEAP_SCAN_HI2)
            )
            if not in_range:
                continue
            if size > _MAX_REGION:
                continue
            _scan_region(base, size)

        if verbose:
            print(f"[MineWorkerReader] calibrate: gold={mine_gold}, "
                  f"candidates={len(found)}, expected_workers={expected_workers}")

        # Phase 2 (fallback): full heap scan if narrow range found nothing.
        if not found:
            if verbose:
                print("[MineWorkerReader] no candidate in narrow range — "
                      "falling back to full heap scan (one-time, slower)")
            for base, size, prot in _iter_regions(self.task):
                if not (prot & 1):
                    continue
                if size > _MAX_REGION:
                    continue
                _scan_region(base, size)
            if verbose:
                print(f"[MineWorkerReader] full-scan candidates={len(found)}")
            if not found:
                return False

        # Store all candidates; read_workers() will prune via gold-tracking.
        self._candidates = found
        if verbose:
            print(f"[MineWorkerReader] {len(found)} candidate(s) stored; "
                  "will prune via gold-tracking on successive reads")
            for i, g in enumerate(found):
                g_now = _read_mem(self.task, g, 4)
                g_val = struct.unpack("<i", g_now)[0] if (g_now and len(g_now) == 4) else -1
                wb = _read_mem(self.task, g + WORKER_OFFSET, 4)
                wc = struct.unpack("<i", wb)[0] if (wb and len(wb) == 4) else -1
                print(f"  [{i}] gold_addr=0x{g:x}  gold={g_val}  workers={wc}")
        return True

    def read_workers(self, current_gold: Optional[int] = None) -> Optional[int]:
        """
        Return the current assigned-workers count (0–5), or None if not calibrated
        or if the memory read fails.

        current_gold: the mine's current gold as reported by Lua.  If provided,
            used to eliminate false-positive candidates whose gold field has drifted.
        """
        # Fast path: already narrowed to one address.
        if self._worker_addr is not None:
            data = _read_mem(self.task, self._worker_addr, 4)
            if not data or len(data) < 4:
                return None
            val = struct.unpack("<i", data)[0]
            if not (0 <= val <= 10):   # sanity: >10 = stale/relocated struct
                return None
            return val

        if not self._candidates:
            return None

        # Slow path: prune candidates by comparing their gold field to current_gold.
        valid: list[tuple[int, int]] = []   # (gold_addr, workers)
        for gold_addr in self._candidates:
            # Gold-tracking check.
            if current_gold is not None:
                g_data = _read_mem(self.task, gold_addr, 4)
                if g_data and len(g_data) == 4:
                    mem_gold = struct.unpack("<i", g_data)[0]
                    if abs(mem_gold - current_gold) > self._GOLD_TOLERANCE:
                        continue   # diverged — not the real mine struct
            # Workers sanity check.
            w_data = _read_mem(self.task, gold_addr + WORKER_OFFSET, 4)
            if w_data and len(w_data) == 4:
                wc = struct.unpack("<i", w_data)[0]
                if 0 <= wc <= 10:
                    valid.append((gold_addr, wc))

        self._candidates = [g for g, _ in valid]

        if not self._candidates:
            return None

        if len(self._candidates) == 1:
            # Converged — promote to fast path.
            self._worker_addr = self._candidates[0] + WORKER_OFFSET
            self._candidates = []
            return valid[0][1]

        # Still multiple: return the candidate with the highest workers value.
        # The real mine struct tracks assigned peons (can be 5); false positives
        # tend to have lower coincidental values.
        best = max(valid, key=lambda t: t[1])
        return best[1]

    def invalidate(self) -> None:
        """Clear all cached addresses (call when starting a new game or mine changes)."""
        self._candidates = []
        self._worker_addr = None

    @property
    def is_calibrated(self) -> bool:
        return self._worker_addr is not None or bool(self._candidates)
