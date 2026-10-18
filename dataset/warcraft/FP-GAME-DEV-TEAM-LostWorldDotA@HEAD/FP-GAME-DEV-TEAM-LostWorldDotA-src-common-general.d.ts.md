---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "src/common/general.d.ts"
ref: HEAD
---

/**
 * This file contains some general types related to your game that can be shared between
 * front-end (Panorama) and back-end (VScripts). Only put stuff in here you need to share.
 */

interface Color {
    r: number,
    g: number,
    b: number
}

interface UnitData {
    name: string,
    level: number
}