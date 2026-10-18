---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "reforged.go"
ref: HEAD
---

package main

import (
	"log"
	"sync/atomic"
)

// reforgedMode is the global HD ↔ SD toggle. atomic.Bool so any code path
// (asset handler HTTP goroutines, Wails RPC, the bridge) can read it
// without taking a mutex. Default false → SD/Classic. The CASC storage
// keeps its own (mirrored) copy because the prefix order is a hot path
// inside its mutex; we keep them in sync via SetReforgedMode.
var reforgedMode atomic.Bool

// ReforgedMode returns whether HD/Reforged graphics mode is active.
func ReforgedMode() bool { return reforgedMode.Load() }

// SetReforgedModeGo flips the global SD ↔ HD preference. Returns the
// new value (handy for the App method wrapper). Also propagates the
// flag down to CASC so its prefix-ordering reflects the new mode.
//
// Notes on what changes:
//   - CASC: SD prefixes `war3.w3mod:` first; HD `_hd.w3mod:` first.
//     This is what makes the same logical path resolve to SD vs HD assets.
//   - Asset handler BLP↔DDS sibling fallback: in HD mode the .dds form
//     is tried first when given .blp (and vice-versa for SD), since HD
//     models reference DDS textures and SD models reference BLP.
//   - The MDX handler's `reforged` flag (28 vs 16 team colors, .dds
//     suffix on the team-color URLs) lives entirely in JS; the JS side
//     re-creates the scene/handler when the toggle flips.
func SetReforgedModeGo(b bool) bool {
	old := reforgedMode.Swap(b)
	if old == b {
		return b
	}
	if c, err := getCASC(); err == nil && c != nil {
		c.SetReforged(b)
	}
	log.Printf("reforged-mode: %v -> %v", old, b)
	return b
}

// SetReforgedMode is the Wails-bound entry point — exposed as a JS
// function under wailsjs/go/main/App. The frontend calls this from the
// HD/SD toggle UI; on flip it tears down its scene and re-creates with
// the new flag, so the MDX handler re-runs with the new `reforged` arg
// (which controls 28 vs 16 team-color textures + .dds vs .blp URL form).
//
// Returns the post-set value so the JS side can confirm without an
// extra round-trip.
func (a *App) SetReforgedMode(b bool) bool {
	return SetReforgedModeGo(b)
}

// GetReforgedMode lets the frontend read the current state on startup so
// the toggle UI reflects what the Go side believes is active.
func (a *App) GetReforgedMode() bool { return ReforgedMode() }
