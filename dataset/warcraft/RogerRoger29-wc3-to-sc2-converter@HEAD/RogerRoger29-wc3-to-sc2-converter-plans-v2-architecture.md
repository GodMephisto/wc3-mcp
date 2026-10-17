---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "plans/v2-architecture.md"
ref: HEAD
---

# v2.0 Architecture Diagram

```mermaid
flowchart TB
    subgraph INPUT["User Input"]
        MDX["Drag .mdx onto .exe"]
        DIR["--batch directory/"]
        MPQ["--mpq archive"]
        WIZ["Interactive wizard"]
    end

    subgraph DISCOVERY["Auto-Discovery Module"]
        SCAN["scan for companion files"]
        BLP["find .blp textures"]
        BLENDER["find Blender install"]
    end

    subgraph DIAGNOSTICS["Pre-Flight Diagnostics"]
        direction TB
        CHECK["15+ automated checks"]
        AALPHA["auto-detect inverted alpha"]
        ASCALE["estimate optimal scale"]
        ANIM["fuzzy-match animation names"]
        BONES["detect hierarchy issues"]
        GEO["find degenerate geometry"]
        REPORT["generate REPORT.html + REPORT.json"]
        CHECK --> AALPHA
        CHECK --> ASCALE
        CHECK --> ANIM
        CHECK --> BONES
        CHECK --> GEO
        AALPHA --> REPORT
        ASCALE --> REPORT
        ANIM --> REPORT
        BONES --> REPORT
        GEO --> REPORT
    end

    subgraph HEALING["Self-Healing Engine"]
        FIX_ALPHA["auto-invert alpha channels"]
        FIX_SCALE["apply estimated scale"]
        FIX_BONES["repair bone hierarchy"]
        FIX_GEO["remove degenerate faces"]
        FIX_TEX["generate placeholder textures"]
    end

    subgraph CONVERT["Conversion Pipeline (existing)"]
        PARSE["mdx.py parse"]
        TEX["textures.py convert"]
        BLEND["Blender build_m3.py"]
        M3["export .m3"]
    end

    subgraph POST["Post-Conversion"]
        VALIDATE["validate output integrity"]
        PREVIEW["render preview .png"]
        ACTOR["generate SC2 actor XML"]
        REPORT2["final conversion report"]
    end

    subgraph OUTPUT["Output"]
        M3FILE["ModelName.m3"]
        DDS["texture .dds files"]
        XML["actor data .xml"]
        PNG["preview .png"]
        HTML["conversion report .html"]
    end

    INPUT --> DISCOVERY
    DISCOVERY --> DIAGNOSTICS
    DIAGNOSTICS -->|"issues found"| HEALING
    DIAGNOSTICS -->|"clean"| CONVERT
    HEALING --> CONVERT
    CONVERT --> POST
    POST --> OUTPUT
```

## Module Map

```
wc3toSC2/
├── convert.py              # CLI orchestrator (existing, enhanced)
├── mdx.py                  # MDX v800 parser (existing)
├── blp.py                  # BLP1 decoder (existing)
├── textures.py             # DDS converter (existing)
├── build_m3.py             # Blender M3 builder (existing)

├── discovery.py            # NEW: auto-detect textures, Blender, scale
├── diagnostics.py          # NEW: 15+ pre-flight checks + report generation
├── healer.py               # NEW: auto-fix alpha, bones, geometry, textures
├── fuzzy_anims.py          # NEW: Levenshtein-based animation name matching
├── mpq_reader.py           # NEW: MPQ archive extraction
├── actor_gen.py            # NEW: SC2 actor/data XML generation

├── templates/              # NEW: material & particle presets
│   ├── materials.json
│   └── particles.json
├── presets/                # NEW: conversion profiles
│   ├── human_units.json
│   ├── undead_units.json
│   └── default.json
├── reports/                # NEW: HTML report templates
│   └── report_template.html
└── tests/
    ├── test_mdx.py
    ├── test_blp.py
    ├── test_textures.py
    ├── test_discovery.py   # NEW
    ├── test_diagnostics.py # NEW
    └── test_healer.py      # NEW
```
