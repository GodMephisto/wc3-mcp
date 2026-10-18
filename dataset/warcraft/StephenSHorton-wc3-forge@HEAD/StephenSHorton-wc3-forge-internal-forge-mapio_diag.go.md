---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/mapio_diag.go"
ref: HEAD
---

package forge

import (
	"sync"
	"sync/atomic"
)

// Map open/save health, surfaced under the "mapIO" namespace of
// diagnostics.get's "go" key. This is U9 of the diagnostics-instrumentation
// pass: it makes the historically-silent failure modes of the Open/Save path
// observable WITHOUT editing the diagnostics handler.
//
// What it captures:
//   - a per-open MANIFEST (path, source kind, which war3map.* files were present
//     and their sizes, how long Open took). Pull-only snapshot under mapIODiag.mu
//     — NOT Current.mu — so the provider never wedges on the session lock.
//   - shadowMapRecovered: the count + last-error of war3map.shd parses that
//     failed and were recovered with a zero-fill (previously swallowed silently
//     in Session.Open).
//   - a per-save TRACE (seq, which dirty files were written, bytes, duration,
//     ok/error) plus a monotone saveSeq.
//   - repackFileMismatch: a counter incremented when an MPQ flush writes a file
//     count that diverges from the source's own block count (the historical
//     240->14 import-drop class). COUNTER ONLY — no blocking re-parse on save.
//   - mpqReadErrors: mpqSource.read I/O/decompress failures.
//   - triggerLoadStatus: an enum captured at Open describing what trigger files
//     resolved (none / wtg-only / wtg+wct / error).
//
// HARD INVARIANT (diagprov.go): the provider MUST NOT read Current.mu. It reads
// dedicated atomics + copies the manifest/save-trace snapshots under mapIODiag's
// OWN short-lived mutex. mapIODiag.mu is a leaf lock taken only by the Open/Save
// recorders and the provider — it never nests another session lock under it.
var mapIODiag struct {
	mu sync.Mutex // guards the manifest + last save trace snapshots ONLY

	// manifest is the snapshot of the most recent successful Session.Open.
	manifest *mapOpenManifest
	// lastSave is the trace of the most recent Session.Save.
	lastSave *mapSaveTrace
	// triggerLoadStatus is the enum from the most recent Open's trigger load:
	// "" (never) | "none" | "wtg-only" | "wtg+wct" | "error".
	triggerLoadStatus string

	// Atomic counters — read lock-free by the provider.
	opens              atomic.Int64  // successful Open count
	openFails          atomic.Int64  // Open returning an error (pre-swap aborts)
	shadowMapFails     atomic.Int64  // war3map.shd parses that failed + recovered
	saves              atomic.Int64  // Save count (start)
	saveFails          atomic.Int64  // Save returning an error
	saveSeq            atomic.Uint64 // monotone per-Save sequence id
	mpqReadErrors      atomic.Int64  // mpqSource.read errors
	repackFileMismatch atomic.Int64  // flush wrote != source block count

	// Last-error strings, swapped atomically so the provider reads without the mu.
	lastShadowMapErr atomic.Value // string
}

// mapFilePresence records one war3map.* file that was present at Open and its
// uncompressed byte size.
type mapFilePresence struct {
	Name  string `json:"name"`
	Bytes int    `json:"bytes"`
}

// mapOpenManifest is the per-open snapshot. Copied out wholesale by the provider
// under mapIODiag.mu so the returned value is never aliased to live state.
type mapOpenManifest struct {
	Path       string            `json:"path"`
	SourceKind string            `json:"sourceKind"` // "folder" | "mpq"
	Files      []mapFilePresence `json:"files"`
	OpenMs     int64             `json:"openMs"`
}

// mapSaveTrace is the per-save snapshot.
type mapSaveTrace struct {
	Seq     uint64   `json:"seq"`
	Written []string `json:"written"` // war3map.* files written this save
	Bytes   int64    `json:"bytes"`   // total bytes written
	SaveMs  int64    `json:"saveMs"`
	OK      bool     `json:"ok"`
	Error   string   `json:"error"` // "" on success
}

// recordOpenManifest stores the manifest + triggerLoadStatus for one successful
// Open and bumps the success counter. Called by Session.Open after the state
// swap. Pull-only: the provider copies this out, never holds a pointer into it.
func recordOpenManifest(m *mapOpenManifest, triggerStatus string) {
	mapIODiag.mu.Lock()
	mapIODiag.manifest = m
	mapIODiag.triggerLoadStatus = triggerStatus
	mapIODiag.mu.Unlock()
	mapIODiag.opens.Add(1)
}

// recordShadowMapFail captures a recovered/failed war3map.shd parse so the
// recovery is no longer silent. err is the parse error message.
func recordShadowMapFail(err string) {
	mapIODiag.shadowMapFails.Add(1)
	mapIODiag.lastShadowMapErr.Store(err)
}

// recordSaveTrace stores the most recent save trace and bumps saves/saveFails.
func recordSaveTrace(t *mapSaveTrace) {
	mapIODiag.mu.Lock()
	mapIODiag.lastSave = t
	mapIODiag.mu.Unlock()
	if !t.OK {
		mapIODiag.saveFails.Add(1)
	}
}

func init() {
	RegisterGoDiag("mapIO", func() any {
		mapIODiag.mu.Lock()
		manifest := mapIODiag.manifest
		lastSave := mapIODiag.lastSave
		triggerStatus := mapIODiag.triggerLoadStatus
		mapIODiag.mu.Unlock()

		shadowErr, _ := mapIODiag.lastShadowMapErr.Load().(string)

		out := map[string]any{
			"opens":              mapIODiag.opens.Load(),
			"openFails":          mapIODiag.openFails.Load(),
			"saves":              mapIODiag.saves.Load(),
			"saveFails":          mapIODiag.saveFails.Load(),
			"saveSeq":            mapIODiag.saveSeq.Load(),
			"shadowMapFails":     mapIODiag.shadowMapFails.Load(),
			"mpqReadErrors":      mapIODiag.mpqReadErrors.Load(),
			"repackFileMismatch": mapIODiag.repackFileMismatch.Load(),
			"triggerLoadStatus":  triggerStatus,
		}
		if shadowErr != "" {
			out["lastShadowMapErr"] = shadowErr
		}
		if manifest != nil {
			out["manifest"] = manifest
		}
		if lastSave != nil {
			out["lastSave"] = lastSave
		}
		return out
	})
}
