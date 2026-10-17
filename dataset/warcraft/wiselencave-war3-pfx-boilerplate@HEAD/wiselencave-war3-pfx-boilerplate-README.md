---
type: vendor-doc
source_repo: "https://github.com/wiselencave/war3-pfx-boilerplate"
source_path: "README.md"
ref: HEAD
---

# Warcraft 3 PopcornFX Boilerplate

Ready-to-use PopcornFX 2.5 project for creating custom particle effects for Warcraft III: Reforged.

## What's inside

The project is pre-configured so you can start making effects right away without dealing with axis setup, texture formats, baking configs, and path issues.

- **Relocated Library** — `Library/` moved into `Popcorn/Library/` so baked paths match Reforged's expected `Popcorn/Library/PopcornFXCore/...` structure
- **BC3 textures** — all default `.dds` textures re-compressed to BC3 (DXT5) for Reforged compatibility
- **Coordinate system** — the project axes coincide with the Warcraft axes (not really, see [Axis convention](#axis-convention))
- **Baking config** — minimal setup targeting a map folder, ready to use with Quick Bake or `PopcornAssetBaker` CLI tool
- **Launch scripts** — `runWarcraft.bat` and `runWorldEditor.bat` for quick testing in game
- **Attribute templates** — `War3Game` and `War3Weather` public templates with standard runtime attributes

## Attribute templates

Two public templates in `Library/Wiselen/Templates/War3.pkfx` provide commonly used Reforged runtime attributes as reusable nodes.

To use them, add the `War3Game` or `War3Weather` node to your effect's particle graph and wire the output pins into your nodes. The attributes will automatically appear in the Effect Interface.

Detailed documentation on how these attributes map to Reforged's runtime (MDX tracks, engine behavior, etc.) will be available on [this page](https://wiselencave.github.io/war3-pfx-docs/reference/attributes/) soon.

## Quick start

1. Clone the repo
2. Open the `PFXReforged` project in PK-Editor v2.5
3. Create a new effect in `SharedFX/`
4. Add `War3Game` / `War3Weather` nodes if you need runtime attributes
5. Use **Quick Bake** to export into the map folder
6. Run `runWarcraft.bat` or `runWorldEditor.bat` to quick test in game

## Axis convention
 
When PopcornFX bakes effects for Warcraft III, the X and Y axes get swapped and inverted compared to the editor viewport. This means hardcoding vectors will not produce the expected result in-game.
 
Use `scene.axisForward`, `scene.axisSide`, and `scene.axisUp` nodes instead of hardcoded vectors. These return the correct directions regardless of the axis system configuration and keep your effects axis-system independent.

This applies to both directions and positions. To build an arbitrary offset, combine the axis nodes with scalar multipliers.

The axis nodes work correctly only when baking with `Quick Bake` from the editor (with the proper coordinate frame configured in project settings). The standalone **Asset Baker** ignores per-effect axis settings and will produce incorrect directions. See [Coordinate System & Axis Convention](https://wiselencave.github.io/war3-pfx-docs/reference/coordinate_system/) for a detailed explanation and workarounds.

## Baking

The baking config is located at `Popcorn/AssetBaker.pkcf`. It targets `map.w3x/` — an unpacked map folder at the project root, so baked files and assets end up directly inside the map.

Baking is also possible via the leaked CLI tool `PopcornAssetBaker.exe` for more advanced pipelines.
[Documentation](https://wiselencave.github.io/war3-pfx-docs/reference/assetbaker_basics/) for that workflow is in progress.

## Testing
 
`runWarcraft.bat` and `runWorldEditor.bat` launch the unpacked map folder via `-launch -loadfile`. The Warcraft III path is resolved from the Windows registry automatically.
 
Under the hood it's just `"Warcraft III.exe" -launch -loadfile "path\to\map.w3x"` — the same command that modders use to launch maps from IDEs and build tools. The bat files are included for quick testing. How you integrate PopcornFX into your project pipeline is up to you.

## Important notes

- PK-Editor version **2.5.x** is required. Other versions may not be compatible with Reforged's runtime.
- The project uses a slightly different PopcornFX build version than what ships with Reforged, but baked files still load correctly.
- Baked `.pkfx` and `.pkb` are the same binary format. See the [extension documentation](https://wiselencave.github.io/war3-pfx-docs/reference/extensions/).

## Links

- [Download PopcornFX Editor v2.5](https://wiki.popcornfx.com/index.php?title=PK-Editor_v2.5)
- [PopcornFX Documentation (v2.5)](https://documentation.popcornfx.com/PopcornFX/v2.5/index.html)
- [Tutorials: PopcornFX v2 - Beginners (YouTube)](https://www.youtube.com/playlist?list=PLcNLffz-XEF9q19-Rxs5eahGVf-HiWOZa)
- [Reforged FX. Tutorial 0: Basic Pipeline & First Bake (YouTube)](www.youtube.com/watch?v=dmr_SlTkBFM)
- [PopcornFX Basics for Reforged (HIVE)](https://www.hiveworkshop.com/threads/popcornfx-basics.353681/)

## License
 
Custom templates, scripts, and configuration in this project are free to use, modify, and distribute. 

The following directories contain third-party content:
 
- `Popcorn/Library/PopcornFXCore/` and `Editor/` — property of Persistant Studios, distributed with PK-Editor
- `map.w3x/` — generated by Warcraft III World Editor, subject to Blizzard Entertainment's EULA
