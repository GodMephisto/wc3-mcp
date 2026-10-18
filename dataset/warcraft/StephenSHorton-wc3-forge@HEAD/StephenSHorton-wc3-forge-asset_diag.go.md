---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "asset_diag.go"
ref: HEAD
---

package main

import (
	"sync"
	"sync/atomic"
	"time"

	"github.com/StephenSHorton/wc3-forge/internal/forge"
)

// Asset-resolution health counters, surfaced under the "assets" namespace of
// diagnostics.get's "go" key (pull-only — never echoed through the 5Hz frontend
// push). These turn "why is this model invisible / this texture white?" into a
// single read: a non-zero readErrors means real I/O failures, and recent404
// lists exactly which paths resolved nowhere (the classic missing-texture /
// missing-model cause). All atomics + one short ring mutex; never touches
// forge.Current.mu (see diagprov.go invariant).
var assetStats struct {
	mapHits      atomic.Int64 // resolved from the loaded map's source
	cascHits     atomic.Int64 // resolved from the CASC mount
	siblingHits  atomic.Int64 // resolved only via the sibling-extension fallback
	missing      atomic.Int64 // 404 — not found in map OR CASC (informational; see recent404)
	readErrors   atomic.Int64 // map/CASC ReadFile returned an error (should stay 0)
	slowResolves atomic.Int64 // resolutions slower than assetSlowMs
	maxResolveMs atomic.Int64

	ringMu   sync.Mutex
	missRing []string // last assetMissRingMax 404 paths, for inspection
}

const (
	assetSlowMs      = 50
	assetMissRingMax = 64
)

func init() {
	forge.RegisterGoDiag("assets", func() any {
		assetStats.ringMu.Lock()
		ring := append([]string(nil), assetStats.missRing...)
		assetStats.ringMu.Unlock()
		return map[string]any{
			"mapHits":      assetStats.mapHits.Load(),
			"cascHits":     assetStats.cascHits.Load(),
			"siblingHits":  assetStats.siblingHits.Load(),
			"missing":      assetStats.missing.Load(),
			"readErrors":   assetStats.readErrors.Load(),
			"slowResolves": assetStats.slowResolves.Load(),
			"maxResolveMs": assetStats.maxResolveMs.Load(),
			"recent404":    ring,
		}
	})
}

func assetRecordHit(viaCASC, sibling bool) {
	if viaCASC {
		assetStats.cascHits.Add(1)
	} else {
		assetStats.mapHits.Add(1)
	}
	if sibling {
		assetStats.siblingHits.Add(1)
	}
}

func assetRecordReadError() { assetStats.readErrors.Add(1) }

func assetRecord404(p string) {
	assetStats.missing.Add(1)
	assetStats.ringMu.Lock()
	assetStats.missRing = append(assetStats.missRing, p)
	if len(assetStats.missRing) > assetMissRingMax {
		assetStats.missRing = assetStats.missRing[len(assetStats.missRing)-assetMissRingMax:]
	}
	assetStats.ringMu.Unlock()
}

func assetRecordResolveTime(start time.Time) {
	ms := time.Since(start).Milliseconds()
	if ms >= assetSlowMs {
		assetStats.slowResolves.Add(1)
	}
	for {
		cur := assetStats.maxResolveMs.Load()
		if ms <= cur || assetStats.maxResolveMs.CompareAndSwap(cur, ms) {
			break
		}
	}
}
