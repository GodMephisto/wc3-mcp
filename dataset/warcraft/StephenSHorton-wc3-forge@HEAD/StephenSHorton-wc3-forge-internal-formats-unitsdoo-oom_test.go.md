---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/unitsdoo/oom_test.go"
ref: HEAD
---

package unitsdoo

import (
	"encoding/binary"
	"errors"
	"io"
	"testing"
	"time"
)

// These tests guard against the recurring windows-CI OOM (PRs #18/#21/#24):
// a malformed/truncated/fuzzed war3mapUnits.doo could carry a garbage count
// field (entity count, item-drop set count, inventory count, ability count, or
// random_type=2 pair count) that the decoder used to feed straight into a slice
// allocation. A bogus count (e.g. 0xFFFFFFFF) made the runtime attempt a
// multi-gigabyte VirtualAlloc -> "out of memory" fatal crash. The decoder now
// bounds every count against the bytes remaining before allocating, so these
// crafted inputs return a decode error promptly instead of OOMing.

// le appends v little-endian to b. Helper to keep the crafted-byte builders terse.
func le(b []byte, v uint32) []byte { return binary.LittleEndian.AppendUint32(b, v) }

// header builds a valid "W3do" header with the given subversion and entity count.
func header(sub, count uint32) []byte {
	b := []byte("W3do")
	b = le(b, 8)     // version
	b = le(b, sub)   // subversion
	b = le(b, count) // entity_count
	return b
}

// entityUpToDropSets emits one subversion-11 entity's bytes up to (but not
// including) the item_drop_set_count field, so a test can append a hostile
// count next. Mirrors readEntity's field order exactly.
func entityUpToDropSets() []byte {
	var b []byte
	b = append(b, []byte("hpea")...) // type_id
	b = le(b, 0)                     // variation
	b = le(b, 0)                     // pos x
	b = le(b, 0)                     // pos y
	b = le(b, 0)                     // pos z
	b = le(b, 0)                     // rotation
	b = le(b, 0x43000000)            // scale x = 128.0
	b = le(b, 0x43000000)            // scale y = 128.0
	b = le(b, 0x43000000)            // scale z = 128.0
	b = append(b, []byte("hpea")...) // skin_id (sub-11 present-first policy)
	b = append(b, 2)                 // flags
	b = le(b, 0)                     // player
	b = append(b, 0, 0)              // unknown1, unknown2
	b = le(b, 0xFFFFFFFF)            // hit_points_pct = -1
	b = le(b, 0xFFFFFFFF)            // mana_pct = -1
	b = le(b, 0xFFFFFFFF)            // map_item_table = -1 (sub-11)
	return b
}

// parseWithinDeadline runs Parse and fails the test if it does not return
// quickly. A successful bounds-check returns in microseconds; an unbounded
// allocation would either OOM-crash the process or spend seconds zeroing
// gigabytes. The generous 10s deadline still catches a regression while not
// flaking on a loaded CI box.
func parseWithinDeadline(t *testing.T, data []byte) error {
	t.Helper()
	type result struct{ err error }
	done := make(chan result, 1)
	go func() {
		_, err := Parse(data)
		done <- result{err}
	}()
	select {
	case r := <-done:
		return r.err
	case <-time.After(10 * time.Second):
		t.Fatal("Parse did not return within 10s — likely a multi-gigabyte allocation (the OOM regression)")
		return nil
	}
}

func TestParse_HugeCounts_NoOOM(t *testing.T) {
	const huge = 0xFFFFFFFF // ~4.3 billion; *anySize would be many GiB

	cases := []struct {
		name  string
		build func() []byte
	}{
		{
			// Bogus entity_count in the header, with almost no body. Old code
			// preallocated make([]Entity, 0, count) (~272 MB even after the 1M
			// cap); now bounded by remaining bytes.
			name: "entity_count",
			build: func() []byte {
				b := header(11, huge)
				b = append(b, 0x00) // 1 stray byte, nowhere near enough for an entity
				return b
			},
		},
		{
			// The classic 16 GiB trigger: a garbage item_drop_set_count.
			name: "item_drop_set_count",
			build: func() []byte {
				b := header(11, 1)
				b = append(b, entityUpToDropSets()...)
				b = le(b, huge) // item_drop_set_count
				return b
			},
		},
		{
			// Garbage per-set item count.
			name: "drop_set_item_count",
			build: func() []byte {
				b := header(11, 1)
				b = append(b, entityUpToDropSets()...)
				b = le(b, 1)    // item_drop_set_count = 1
				b = le(b, huge) // items_in_set[0]
				return b
			},
		},
		{
			// Garbage inventory_count.
			name: "inventory_count",
			build: func() []byte {
				b := header(11, 1)
				b = append(b, entityUpToDropSets()...)
				b = le(b, 0)    // item_drop_set_count = 0
				b = le(b, 0)    // gold_amount
				b = le(b, 0)    // target_acquisition
				b = le(b, 1)    // hero_level
				b = le(b, 0)    // hero_str (sub-11)
				b = le(b, 0)    // hero_agi
				b = le(b, 0)    // hero_int
				b = le(b, huge) // inventory_count
				return b
			},
		},
		{
			// Garbage ability_count.
			name: "ability_count",
			build: func() []byte {
				b := header(11, 1)
				b = append(b, entityUpToDropSets()...)
				b = le(b, 0)    // item_drop_set_count = 0
				b = le(b, 0)    // gold_amount
				b = le(b, 0)    // target_acquisition
				b = le(b, 1)    // hero_level
				b = le(b, 0)    // hero_str
				b = le(b, 0)    // hero_agi
				b = le(b, 0)    // hero_int
				b = le(b, 0)    // inventory_count = 0
				b = le(b, huge) // ability_count
				return b
			},
		},
		{
			// Garbage random_type=2 pair count. int(n)*8 would overflow on
			// 32-bit and feed a giant length to readBytes on 64-bit.
			name: "random_type2_count",
			build: func() []byte {
				b := header(11, 1)
				b = append(b, entityUpToDropSets()...)
				b = le(b, 0)    // item_drop_set_count = 0
				b = le(b, 0)    // gold_amount
				b = le(b, 0)    // target_acquisition
				b = le(b, 1)    // hero_level
				b = le(b, 0)    // hero_str
				b = le(b, 0)    // hero_agi
				b = le(b, 0)    // hero_int
				b = le(b, 0)    // inventory_count = 0
				b = le(b, 0)    // ability_count = 0
				b = le(b, 2)    // random_type = 2 (custom group)
				b = le(b, huge) // pair count
				return b
			},
		},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			err := parseWithinDeadline(t, tc.build())
			if err == nil {
				t.Fatalf("Parse accepted a malformed input with a bogus count; expected a decode error")
			}
			// The bounds checks wrap io.ErrUnexpectedEOF, consistent with the
			// package's truncation-error style.
			if !errors.Is(err, io.ErrUnexpectedEOF) {
				t.Logf("note: error did not wrap io.ErrUnexpectedEOF (still a clean rejection): %v", err)
			}
		})
	}
}
