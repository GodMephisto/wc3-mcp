---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "maps/sidecar/README.md"
ref: HEAD
---

# Sidecar Map — Build & Install

## What this is

`sidecar.lua` is a Lua trigger embedded in a custom WC3 melee map.
It writes game state (gold, lumber, food, units, buildings, visible enemies)
every 100ms to a file the Python bot can read — bypassing WC3's dynamic
memory encoding (Blizzard Patent US 11,273,380).

Output file on macOS:
```
~/Documents/Warcraft III/CustomMapData/wc3_orc_sidecar/sidecar.pld
```

## Build (one-time, Windows VM required)

1. Boot the Parallels Windows VM.
2. Open World Editor (`War3MapEditor.exe`).
3. File → Open → select a standard melee map, e.g.:
   `Maps/FrozenThrone/(4)LostTemple.w3m`
4. Trigger Editor (F4) → New Trigger:
   - Name: `Sidecar`
   - Event: `Map Initialization`
   - Action: `Custom Script` → paste the full contents of `sidecar.lua`
5. Map Properties → verify Game Type = Melee.
6. File → Save As → `LostTemple_Sidecar.w3x` (save to VM Desktop).
7. Copy `.w3x` to macOS via Parallels shared folder:
   ```
   ~/Library/Application Support/Warcraft III/Maps/BotMaps/LostTemple_Sidecar.w3x
   ```

## Play

1. WC3 → Single Player → Custom Game → find `LostTemple_Sidecar`.
2. Set up: Human (Orc) vs Computer (Insane), any race.
3. Start game.

## Verify sidecar is working

```bash
python3 src/wc3bot/observe/sidecar_reader.py
```

Should print live gold/lumber/food/unit counts every time something changes.

## Debug (in-game)

In `sidecar.lua`, uncomment the `BJDebugMsg` line to see Lua errors as
in-game text messages.

## Output format

Single pipe-delimited line, overwritten every ~100ms:

```
T:42.1|G:500|L:80|FU:6|FC:12|U:opeo:123:-50:100|U:ogru:130:-55:85|B:otrb:100:-20:100|E:hpea:-300:200:100
```

| Tag | Meaning |
|-----|---------|
| `T` | game time (seconds) |
| `G` | player gold |
| `L` | player lumber |
| `FU` | food used |
| `FC` | food cap |
| `U:id:x:y:hp` | own unit (hp = 0..100) |
| `B:id:x:y:hp` | own building (hp = construction progress %) |
| `E:id:x:y:hp` | visible enemy unit (fog-of-war respected) |

Common Orc unit IDs: `opeo` (peon), `ogru` (grunt), `ohun` (headhunter),
`orai` (raider), `okod` (kodo), `oshm` (shaman), `odoc` (witch doctor),
`otbk` (tauren), `ofar` (far seer hero), `obla` (blademaster hero).
