---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "src/wc3bot/race/orc/config.py"
ref: HEAD
---

from wc3bot.race import register_race
from wc3bot.race.orc.build_order import OrcBuildOrder
from wc3bot.race.orc.setup import orc_setup, execute_orc_action, make_orc_action


class OrcConfig:
    name              = "orc"
    worker_fcc        = "opeo"
    great_hall_fcc    = "ogre"
    mine_fcc          = "ngol"

    hotkey_train_worker = "Q"
    hotkey_build_menu   = "Z"
    hotkey_hero         = "Q"

    group_hall     = "5"
    group_workers  = "6"
    group_hero     = "4"
    group_barracks = "3"
    group_lumber   = "7"   # builder peon(s) assigned here for lumber routing

    worker_gold_cost = 75
    worker_food_cost = 1
    max_workers      = 12

    grunt_gold_cost  = 200
    grunt_food_cost  = 3
    max_grunts       = 6   # cap for Phase 1 (adjust later)

    barracks_build_time = 65.0   # seconds from order to ready

    # Building hotkeys (after build_menu Z opens sub-menu — Grid layout)
    # Verified from screenshot 2026-05-28: Q=Burrow, W=Altar, E=Barracks
    hotkey_altar       = "W"
    hotkey_burrow      = "Q"
    hotkey_barracks    = "E"
    hotkey_train_grunt = "G"

    # Screen offsets from GH anchor (pixels)
    # Derived via ISO formula: screen_dx = (wx-wy)*ISO_H, screen_dy = (wx+wy)*ISO_V
    # "Behind GH" = toward trees = negative screen_dy (upward on screen)
    # See .ai/research/2026-05-28-building-placement/findings.md for full derivation.
    # NOTE: these assume trees are "up-screen" from GH (verify per spawn on first game run).
    altar_offset    = (-485,  -64)   # world (0, +256) — 2 tiles behind GH into treeline
    burrow_offset   = (-607,  -16)   # world (-128, +192) — 1 tile left, 1.5 back
    burrow2_offset  = (-850,  -16)   # world (-256, +192) — 2 tiles left, 1.5 back
    barracks_offset = (+243,  -97)   # world (+256, +128) — 2 tiles right, 1 back
    lumber_offset   = (-250, -80)

    # Build costs / game constants
    peon_cost     = 75
    starting_food = 5    # STARTING_PEONS
    mine_dy       = 250  # fallback vertical offset

    burrow_lumber_cost = 40   # lumber cost per Burrow (0 gold)
    food_cap_max       = 60   # never build Burrows beyond this cap
    food_headroom      = 3    # build a Burrow when food_free drops to this

    # ── BM hero skill sequence (source: AMAI HeroSkills.txt, row 6) ──────────
    # Most common pattern across AMAI Orc strategies:
    # L1: CS → L2: WW → L3: CS → L4: WW → L5: CS → L6: BLADESTORM (ultimate)
    # L7: WW → L8–10: MIRROR_IMAGE
    bm_skill_sequence = [
        "CRITICAL_STRIKE", "WIND_WALK", "CRITICAL_STRIKE",
        "WIND_WALK",       "CRITICAL_STRIKE", "BLADE_STORM",
        "WIND_WALK",       "MIRROR_IMAGE",    "MIRROR_IMAGE", "MIRROR_IMAGE",
    ]
    # Hotkeys for BM abilities (Grid layout).
    # Set to None until verified via T17_verify_hero_hotkeys.
    # Grid layout positions: row1 = Q W E R; ultimates vary by hero.
    bm_skill_hotkeys: dict = {
        "CRITICAL_STRIKE": None,   # TODO: verify (likely Q in Grid)
        "WIND_WALK":       None,   # TODO: verify (likely W in Grid)
        "BLADE_STORM":     None,   # TODO: verify (ultimate — likely R in Grid)
        "MIRROR_IMAGE":    None,   # TODO: verify (likely E in Grid)
    }

    # ── Timing constants (source: AMAI global_build_sequence) ────────────────
    # BM harass window: Harass(4, PEONS, ..., 36, 150) → 36*6=216s to 150*6=900s
    bm_harass_start  = 216.0   # ~3.6 min: BM peon harassment begins
    bm_harass_end    = 900.0   # ~15 min: BM harass ends (hero skill = MINOR condition)
    # Safe expansion window: Wyvern harass starts ~13 min; below ~780s is safe
    expansion_safe_start = 780.0  # ~13 min

    def make_build_order(self) -> OrcBuildOrder:
        return OrcBuildOrder()

    def setup(self, inp, calib, state) -> None:
        orc_setup(inp, calib, state, self)

    def execute_action(self, action: str, inp, calib) -> None:
        execute_orc_action(action, inp, calib, self)

    def make_action(self, action: str, inp, calib) -> "Action | None":
        return make_orc_action(action, inp, calib, self)


register_race(OrcConfig())
