---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/w3i/oom_test.go"
ref: HEAD
---

package w3i

import (
	"encoding/binary"
	"errors"
	"io"
	"testing"
	"time"
)

// This test guards against the OOM-crash class hotfixed three times for
// unitsdoo (PRs #18/#21/#24) and applied here as part of the Phase 0
// "never crash opening a map" floor. war3map.w3i is parsed on EVERY map open;
// a protected/corrupt/truncated file can carry a garbage uint32 count (player,
// force, upgrade, tech, random-unit/item-table) that the decoder used to feed
// straight into make([]T, n). A bogus count (e.g. 0xFFFFFFFF) made the runtime
// attempt a multi-gigabyte allocation -> "out of memory" fatal crash. The
// decoder now bounds every untrusted count against the bytes remaining (via
// reader.checkCount) before allocating, so these crafted inputs return a decode
// error promptly instead of OOMing.

// le appends v little-endian to b. Keeps the crafted-byte builders terse.
func le(b []byte, v uint32) []byte { return binary.LittleEndian.AppendUint32(b, v) }

// cstr appends a null-terminated string.
func cstr(b []byte, s string) []byte {
	b = append(b, []byte(s)...)
	return append(b, 0)
}

// v15Prefix emits a minimal-but-valid v15 (RoC) header up to — but not
// including — the player_count field, mirroring Parse's read order for the
// `default` (version < 18) branch. v15 is the shortest prefix to reach the
// count-bearing player section, so the crafted buffers stay tiny (a huge count
// over a tiny buffer is exactly the OOM trigger we're rejecting).
func v15Prefix() []byte {
	var b []byte
	b = le(b, FileVersionRoC) // file_version = 15
	// Name, Author, Description, SuggestedPlayers.
	b = cstr(b, "n")
	b = cstr(b, "a")
	b = cstr(b, "d")
	b = cstr(b, "s")
	// Camera: 4 vec2 (8 floats) + ivec4 (4 int32) = 12 * 4 bytes.
	for i := 0; i < 12; i++ {
		b = le(b, 0)
	}
	b = le(b, 64) // PlayableWidth
	b = le(b, 64) // PlayableHeight
	b = le(b, 0)  // flags
	b = append(b, 0) // tileset byte
	// default branch (version < 18): skip(1) + 3 cstr + skip(4).
	b = append(b, 0) // unknown loading-screen index
	b = cstr(b, "t") // loading text
	b = cstr(b, "T") // loading title
	b = cstr(b, "S") // loading subtitle
	b = le(b, 0)     // 4-byte unknown
	return b
}

// parseWithinDeadline runs Parse and fails the test if it does not return
// quickly. A successful bounds-check returns in microseconds; an unbounded
// allocation would either OOM-crash the process or spend seconds zeroing
// gigabytes. The generous 10s deadline still catches a regression while not
// flaking on a loaded CI box.
func parseWithinDeadline(t *testing.T, data []byte) (*Info, error) {
	t.Helper()
	type result struct {
		info *Info
		err  error
	}
	done := make(chan result, 1)
	go func() {
		info, err := Parse(data)
		done <- result{info, err}
	}()
	select {
	case r := <-done:
		return r.info, r.err
	case <-time.After(10 * time.Second):
		t.Fatal("Parse did not return within 10s — likely a multi-gigabyte allocation (the OOM regression)")
		return nil, nil
	}
}

func TestParse_HugeCounts_NoOOM(t *testing.T) {
	const huge = 0xFFFFFFFF // ~4.3 billion; *anySize would be many GiB

	cases := []struct {
		name  string
		build func() []byte
	}{
		{
			// Bogus player_count with almost no body. Old code did
			// make([]Player, 0, 0xFFFFFFFF) (~tens of GB) and OOM'd.
			name: "player_count",
			build: func() []byte {
				b := v15Prefix()
				b = le(b, huge) // player_count
				b = append(b, 0x00)
				return b
			},
		},
		{
			// Valid (zero) players, then a bogus force_count.
			name: "force_count",
			build: func() []byte {
				b := v15Prefix()
				b = le(b, 0)    // player_count = 0
				b = le(b, huge) // force_count
				b = append(b, 0x00)
				return b
			},
		},
		{
			// Valid players + forces, then a bogus upgrade_count.
			name: "upgrade_count",
			build: func() []byte {
				b := v15Prefix()
				b = le(b, 0)    // player_count = 0
				b = le(b, 0)    // force_count = 0
				b = le(b, huge) // upgrade_count
				b = append(b, 0x00)
				return b
			},
		},
		{
			// Valid through upgrades, then a bogus tech_count.
			name: "tech_count",
			build: func() []byte {
				b := v15Prefix()
				b = le(b, 0)    // player_count = 0
				b = le(b, 0)    // force_count = 0
				b = le(b, 0)    // upgrade_count = 0
				b = le(b, huge) // tech_count
				b = append(b, 0x00)
				return b
			},
		},
		{
			// Valid through tech, then a bogus random_unit_table count.
			name: "random_unit_table_count",
			build: func() []byte {
				b := v15Prefix()
				b = le(b, 0)    // player_count = 0
				b = le(b, 0)    // force_count = 0
				b = le(b, 0)    // upgrade_count = 0
				b = le(b, 0)    // tech_count = 0
				b = le(b, huge) // random_unit_table count
				b = append(b, 0x00)
				return b
			},
		},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			_, err := parseWithinDeadline(t, tc.build())
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

// TestParse_v15Prefix_Valid is a sanity check that v15Prefix + a zero
// player/force tail parses cleanly — proving the OOM cases above are rejected
// because of the hostile count, not because the crafted prefix is malformed.
func TestParse_v15Prefix_Valid(t *testing.T) {
	b := v15Prefix()
	b = le(b, 0) // player_count = 0
	b = le(b, 0) // force_count = 0
	// Tail (upgrades/tech/random tables) is absent — the parser is lenient and
	// records TruncatedAt rather than erroring.
	info, err := Parse(b)
	if err != nil {
		t.Fatalf("Parse(valid v15 prefix): unexpected error: %v", err)
	}
	if info.FileVersion != FileVersionRoC {
		t.Errorf("FileVersion = %d, want %d", info.FileVersion, FileVersionRoC)
	}
	if len(info.Players) != 0 {
		t.Errorf("Players = %d, want 0", len(info.Players))
	}
}
