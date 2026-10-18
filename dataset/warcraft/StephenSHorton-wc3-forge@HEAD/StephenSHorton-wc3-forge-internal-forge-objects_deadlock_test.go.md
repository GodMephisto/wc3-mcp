---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/objects_deadlock_test.go"
ref: HEAD
---

package forge

import (
	"testing"
	"time"
)

// TestObjectMutators_ColdCache_NoDeadlock is the regression guard for the
// concurrency deadlock where the FIRST object-data mutation over the bridge on
// a freshly-loaded map permanently wedged the session.
//
// Mechanism reproduced here:
//   - Session.SetObjectField (and AddCustomObject / SetObjectFieldLevel) used to
//     take s.mu.Lock() and THEN call loadObjectBase(cfg).
//   - The first-ever loadObjectBase for a kind runs cache.once.Do → readBaseAsset
//     → the registered baseAssetReader, whose production form takes
//     Current.mu.RLock() (it does map-first lookup via Current.ReadFile).
//   - Go's sync.RWMutex is non-reentrant: a goroutine already holding the WRITE
//     lock can never acquire a READ lock on the same mutex, so the RLock blocks
//     forever → deadlock.
//
// This test installs a baseAssetReader that takes Current.mu.RLock() (exactly
// like the production reader) and forces a genuinely COLD per-kind cache, so the
// FIRST mutation is the one that triggers the one-time base load. A deadlock
// would hang the call forever, so we run each mutator in a goroutine and assert
// it completes within a timeout via select.
//
// We deliberately do NOT use setObjectBaseForTest here — that consumes the
// sync.Once and warms the cache, which would mask the very bug under test.
func TestObjectMutators_ColdCache_NoDeadlock(t *testing.T) {
	// The deadlock is between the mutator's s.mu write lock and the reader's
	// Current.mu read lock, so the mutator MUST run against the global Current
	// (the same Session the reader locks). Swap it for an isolated instance and
	// restore on exit.
	prevCurrent := Current
	prevReader := baseAssetReader
	t.Cleanup(func() {
		Current.Close()
		Current = prevCurrent
		baseAssetReader = prevReader
		// Leave the per-kind caches reset so a later test re-loads cleanly.
		resetObjectBaseCacheForTest("units")
		resetObjectBaseCacheForTest("destructables")
	})

	Current = &Session{}

	// Production-shaped reader: map-first lookup that grabs Current.mu.RLock().
	// This is the exact re-entrancy that wedged the session. We return
	// "asset absent" so loadObjectBase builds an empty (but valid) base — the
	// deadlock fires inside readBaseAsset BEFORE the asset bytes matter, so the
	// content is irrelevant to reproducing the hang.
	baseAssetReader = func(name string) ([]byte, bool, error) {
		_, ok, err := Current.ReadFile(name) // takes Current.mu.RLock()
		return nil, ok && false, err
	}

	tmp := minimalFolderMap(t)
	if err := Current.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}

	// completesWithin runs fn in a goroutine and fails the test if it doesn't
	// return before the deadline. A wedged mutator never returns, so a timeout
	// IS the deadlock signal.
	completesWithin := func(label string, d time.Duration, fn func()) {
		t.Helper()
		done := make(chan struct{})
		go func() {
			defer close(done)
			fn()
		}()
		select {
		case <-done:
		case <-time.After(d):
			t.Fatalf("%s did not complete within %s — object mutator deadlocked "+
				"on cold base cache (s.mu write lock vs Current.mu.RLock in readBaseAsset)", label, d)
		}
	}

	// FIRST operation on the fresh session with a COLD "units" cache: a
	// SetObjectField. On the buggy code this wedges at the in-lock
	// loadObjectBase; on the fixed code the up-front warm-up makes the in-lock
	// load a cache hit and the call returns (with an "unknown field" error,
	// since the stubbed-absent base has no rows — that's a COMPLETION, which is
	// all we assert).
	resetObjectBaseCacheForTest("units")
	completesWithin("SetObjectField(units)", 5*time.Second, func() {
		_ = Current.SetObjectField(UnitsConfig(), "hpea", "name", "Whatever")
	})

	// Separately exercise CreateCustomObject (AddCustomObject) on its own COLD
	// kind cache so this path's warm-up is covered independently.
	resetObjectBaseCacheForTest("destructables")
	completesWithin("AddCustomObject(destructables)", 5*time.Second, func() {
		_, _ = Current.AddCustomObject(DestructablesConfig(), "X000", "ATtr")
	})
}
