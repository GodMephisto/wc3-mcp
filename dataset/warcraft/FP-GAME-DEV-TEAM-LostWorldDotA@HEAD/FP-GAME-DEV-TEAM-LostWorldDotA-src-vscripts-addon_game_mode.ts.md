---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "src/vscripts/addon_game_mode.ts"
ref: HEAD
---

import "./lib/timers";
import "./lib/popups";
import { GameMode } from "./GameMode";

// Connect GameMode.Activate and GameMode.Precache to the dota engine
Object.assign(getfenv(), {
    Activate: GameMode.Activate,
    Precache: GameMode.Precache,
});

if (GameRules.Addon !== undefined) {
    // This code is only run after script_reload, not at startup
    GameRules.Addon.Reload();
}
