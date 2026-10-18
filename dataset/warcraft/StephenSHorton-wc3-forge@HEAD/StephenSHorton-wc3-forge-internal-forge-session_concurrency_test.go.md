---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/session_concurrency_test.go"
ref: HEAD
---

package forge

import (
	"sync"
	"testing"
)

// TestSaveConcurrentMutation_NoRace exercises the encode-under-RLock fix: a
// goroutine repeatedly Save()s a folder-backed map while other goroutines
// concurrently MoveUnit. Before the fix, Save's Encode read shared slices
// UNLOCKED while the mutators wrote them in place, tearing the output (and
// tripping the race detector). With the fix, Encode holds the read lock so the
// mutators (write lock) block during serialization.
//
// Run with `go test -race` for the definitive check — the detector flags the
// unsynchronized read/write the fix removes. Without -race this still asserts
// no Save error and that the final saved war3mapUnits.doo re-parses cleanly
// (a torn encode would corrupt the entity count or trail).
func TestSaveConcurrentMutation_NoRace(t *testing.T) {
	tmp := copyFixtureToTemp(t, `C:\Users\4step\projects\wc3-survival-game\map\extracted`)
	s := &Session{}
	if err := s.Open(tmp); err != nil {
		t.Fatalf("Open: %v", err)
	}
	units := s.Units()
	if units == nil || len(units.Entities) < 4 {
		t.Skipf("fixture has %d entities, test wants ≥4", len(units.Entities))
	}
	cns := make([]uint32, 0, 4)
	for i := 0; i < 4; i++ {
		cns = append(cns, units.Entities[i].CreationNumber)
	}

	var wg sync.WaitGroup
	stop := make(chan struct{})

	// Saver: repeatedly flush to the folder while mutations are in flight.
	wg.Add(1)
	go func() {
		defer wg.Done()
		for i := 0; i < 40; i++ {
			select {
			case <-stop:
				return
			default:
			}
			if err := s.Save(); err != nil {
				t.Errorf("concurrent Save: %v", err)
				return
			}
		}
	}()

	// Mutators: each hammers a distinct unit's position.
	for _, cn := range cns {
		wg.Add(1)
		go func(cn uint32) {
			defer wg.Done()
			for i := 0; i < 60; i++ {
				select {
				case <-stop:
					return
				default:
				}
				_ = s.MoveUnit(cn, float32(i), float32(-i), 0)
			}
		}(cn)
	}

	wg.Wait()
	close(stop)

	// Capture the in-memory final position of each hammered unit (no mutators
	// running now). Then a final Save + re-open must show EXACTLY these — proving
	// (a) no torn encode (the file re-parses) and (b) no dropped dirty flag (the
	// last mutation before a concurrent Save's flag-clear isn't lost — the bug
	// the encode-under-write-lock + clear-under-lock fix prevents).
	want := map[uint32][3]float32{}
	for i := range s.Units().Entities {
		e := s.Units().Entities[i]
		for _, cn := range cns {
			if e.CreationNumber == cn {
				want[cn] = e.Position
			}
		}
	}
	if err := s.Save(); err != nil {
		t.Fatalf("final Save: %v", err)
	}
	dir := s.Path()
	s.Close()

	s2 := &Session{}
	if err := s2.Open(dir); err != nil {
		t.Fatalf("re-Open after concurrent saves: %v", err)
	}
	defer s2.Close()
	got := map[uint32][3]float32{}
	for i := range s2.Units().Entities {
		got[s2.Units().Entities[i].CreationNumber] = s2.Units().Entities[i].Position
	}
	for cn, p := range want {
		if got[cn] != p {
			t.Errorf("unit cn=%d persisted at %v, want %v (a concurrent Save dropped the dirty flag → lost edit)", cn, got[cn], p)
		}
	}
}
