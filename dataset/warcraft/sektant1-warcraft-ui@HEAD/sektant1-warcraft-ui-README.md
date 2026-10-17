---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "README.md"
ref: HEAD
---

# @sektant1/warcraft-ui

A React component library that faithfully recreates Warcraft III: Reforged UI elements using original BLP textures rendered on HTML5 Canvas.

## Installation

```bash
npm install @sektant1/warcraft-ui
```

> Published on GitHub Packages. Add to your `.npmrc`:
>
> ```
> @sektant1:registry=https://npm.pkg.github.com
> ```

## Game Assets (Required)

This library renders Warcraft III UI elements from original game textures in **BLP format**. Due to copyright, these assets are **not included** in the package. You must extract them yourself from a licensed copy of Warcraft III.

### How to obtain the assets

1. Install [CascView](http://www.zezula.net/en/casc/main.html) or [CascLib](https://github.com/ladislav-zezula/CascLib)
2. Open the Warcraft III CASC storage (typically at `C:\Program Files (x86)\Warcraft III\Data`)
3. Extract the required textures from the game data

### Where to place the assets

Serve the assets from your public/static directory with this structure:

```
public/
  bars/               # StatBar fill/border/highlight textures
  borders/            # Nine-slice border atlases
  buttons/
    checkbox/         # EscCheckbox textures
    command/          # CommandCard button icons (BTN*.blp)
    esc/              # EscOptionButton / MenuPanel per-race skins
      human/
      orc/
      nightelf/
      undead/
    glue/             # GlueScreenButton / GlueMenuButton nine-slice textures
    radio/            # EscRadioButton textures
    slider/           # EscSlider track + knob textures
  console/            # BottomHud tile textures per race
  console-buttons/    # Minimap/formation button textures
  cursor/             # Race cursor atlas (e.g. HumanCursor.blp)
  fonts/              # Friz Quadrata font (FRIZQT__.TTF)
  loading/            # LoadingBar textures (BarBackground, BarFill, etc.)
  models/             # MDX/BLP for HeroPortraitModel, WorkerUnitModel
  resources/          # ResourceGold.blp, ResourceLumber.blp, ResourceSupply.blp
  tooltips/           # Tooltip background + border textures
```

### Custom asset URL

If your assets are hosted elsewhere (CDN, different path), configure the base URL:

```tsx
import { setAssetsBaseUrl } from "@sektant1/warcraft-ui";

setAssetsBaseUrl("https://cdn.example.com/wc3-assets/");
```

## Quick Start

```tsx
import {
  GlueScreenButton,
  EscCheckbox,
  StatBar,
  CommandCard,
  createEmptySlots,
  setCurrentRace,
} from "@sektant1/warcraft-ui";
import "@sektant1/warcraft-ui/style.css";

function App() {
  // Set the active race (affects race-specific textures globally)
  setCurrentRace("Human");

  return (
    <div>
      <GlueScreenButton onClick={() => alert("For Lordaeron!")}>
        Single Player
      </GlueScreenButton>

      <EscCheckbox label="Show Health Bars" onChange={(v) => console.log(v)} />

      <StatBar type="health" fillPercent={75} maxValue={1200} label="HP" />

      <CommandCard slots={createEmptySlots()} />
    </div>
  );
}
```

## Race System

Many components change their appearance based on the active race. There are two ways to control this:

### Global race (default)

```tsx
import { setCurrentRace, useCurrentRace } from "@sektant1/warcraft-ui";

// Set the active race for all components
setCurrentRace("Orc");

// Read the current race in a React component
const race = useCurrentRace(); // "Orc"
```

### Per-component race override

All race-dependent components accept an optional `race` prop that overrides the global setting:

```tsx
<EscCheckbox label="Human style" race="Human" />
<EscSlider label="Volume" race="Undead" />
<StatBar type="health" fillPercent={80} race="NightElf" />
<MenuPanel race="Orc">Panel content</MenuPanel>
```

Available races: `"Human"` | `"Orc"` | `"NightElf"` | `"Undead"`

## Components

### Glue Buttons (Main Menu)

| Component | Description |
|---|---|
| `GlueScreenButton` | 9-slice composited main menu button |
| `GlueBorderedButton` | Bordered variant of the screen button |
| `GlueMenuButton` | Framed menu button (OK/Cancel style) |
| `GlueCampaignButton` | Campaign selection button |
| `GlueSmallButton` | Compact utility button |

### ESC Menu Controls

| Component | Description |
|---|---|
| `EscCheckbox` | Race-specific checkbox with check mark |
| `EscRadioButton` | Radio button with dot indicator |
| `EscSlider` | Slider with track border + race knob |
| `EscOptionButton` | Race-specific option/action button |
| `InputBox` | Text input with WC3 styling |
| `BnetEditBox` | Battle.net style edit box |

### Bars & Panels

| Component | Description |
|---|---|
| `StatBar` | Health/Mana/XP/Build progress bar with race textures |
| `LoadingBar` | Animated loading bar with glow effect |
| `MenuPanel` | Race-specific ESC menu panel (nine-slice border) |
| `Tooltip` | Game tooltip with optional icon |

### Data Display

| Component | Description |
|---|---|
| `CommandCard` | 4x3 command button grid |
| `ResourceCounter` | Gold/Lumber/Food resource display |
| `BlpIcon` | Renders any BLP file as an icon (with `?` fallback) |
| `Heading` | WC3-styled heading (H1-H4) with optional icon |
| `SectionTitle` | Section label with bottom border |
| `GlueDropdown` | Dropdown select |
| `GlueListBox` | Scrollable list with `GlueScrollbar` |

### 3D Models

| Component | Description |
|---|---|
| `HeroPortraitModel` | Animated hero portrait (WebGL) |
| `WorkerUnitModel` | Animated worker unit model |
| `ItemModel` | Inventory item 3D model |
| `TimeIndicatorModel` | Day/night cycle indicator |

### HUD

| Component | Description |
|---|---|
| `TopHudWebGL` | Top HUD bar (WebGL rendered) |
| `BottomHud` | Bottom HUD tile bar |
| `CursorOverlay` | Race-specific game cursor |

## CommandCard Usage

```tsx
import { CommandCard, createEmptySlots } from "@sektant1/warcraft-ui";
import type { CommandSlot } from "@sektant1/warcraft-ui";

const mySlots: CommandSlot[] = [
  { hotkey: "Q", label: "Move", iconPath: "buttons/command/BTNMove.blp", state: "ready" },
  { hotkey: "W", label: "Stop", iconPath: "buttons/command/BTNStop.blp", state: "ready" },
  { hotkey: "E", label: "Attack", iconPath: "buttons/command/BTNAttack.blp", state: "ready" },
  ...createEmptySlots(9), // fill remaining slots
];

<CommandCard slots={mySlots} cellSize={48} />
```

Slot states: `"ready"` | `"passive"` | `"levelup"` | `"active"` | `"empty"`

## Development

```bash
npm run dev          # Start dev server
npm run build        # Build library
npm run build:demo   # Build demo site
npm run test         # Run tests
npm run test:watch   # Run tests in watch mode
```

## Tech Stack

- React 18/19 + TypeScript
- HTML5 Canvas for texture rendering
- BLP texture decoding (Blizzard image format)
- WebGL for 3D model rendering (war3-model)
- Vite for bundling
- Vitest for testing
