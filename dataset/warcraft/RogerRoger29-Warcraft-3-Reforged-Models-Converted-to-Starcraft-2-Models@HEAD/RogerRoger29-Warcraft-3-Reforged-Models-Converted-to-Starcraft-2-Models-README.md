---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/Warcraft-3-Reforged-Models-Converted-to-Starcraft-2-Models"
source_path: "README.md"
ref: HEAD
---

# Warcraft III Reforged models converted for StarCraft II

This repository is organized for model packages produced by the modified
[W3ModelViewer](https://github.com/Darithos/W3ModelViewer) Warcraft III to StarCraft II exporter.

The initial commit contains the repository structure and maintenance tools only. Model packages can
be added manually from a local conversion output.

## Model layout

Keep the original Warcraft III source hierarchy and one folder per source model:

```text
models/
  reforged/
    units/
      human/
        footman/
          footman__mdx/
            footman.m3
            .export.json
            textures/
              *.dds
```

Do not merge texture directories between packages. Texture paths inside each M3 are package-specific.
Keep `.export.json` with the model because it records the source path, selected LOD, conversion
quality, known omissions, texture count, and converter identity.

## Adding packages manually

1. Copy a complete model folder from the converter's `exports/models/reforged/` tree.
2. Paste it under the identical path in this repository's `models/reforged/` tree.
3. Run `powershell -ExecutionPolicy Bypass -File tools/Validate-Packages.ps1`.
4. Run `powershell -ExecutionPolicy Bypass -File tools/Refresh-Catalog.ps1`.
5. Review `CATALOG.csv`, then commit and push the added folder and updated catalog.

Git LFS is configured for `.m3` and `.dds` files. Install Git LFS before committing binary packages.
The complete local conversion is more than 61 GiB, so add and push manageable groups rather than one
enormous commit.

## Conversion status

The receipt quality values mean:

- `complete`: no tracked CORN, ribbon, light, unusable PRE2, or missing-texture omission.
- `partial`: the M3 was exported, but its receipt records one or more known omissions.

These labels describe what the converter tracked. They do not prove that a package has been visually
tested in the StarCraft II Editor.

## Rights and attribution

Warcraft III and StarCraft II are Blizzard Entertainment properties. The exporter source license
does not grant a license to Blizzard game assets. See [NOTICE.md](NOTICE.md) before adding content.
