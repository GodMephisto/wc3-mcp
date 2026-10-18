---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/wtg/oom_test.go"
ref: HEAD
---

package wtg

import (
	"encoding/binary"
	"errors"
	"io"
	"testing"
	"time"
)

// This test guards against the OOM-crash class hotfixed three times for
// unitsdoo (PRs #18/#21/#24) and applied here as part of the Phase 0
// "never crash opening a map" floor. war3map.wtg is parsed on every
// GUI-trigger map open; a protected/corrupt/truncated file can carry a garbage
// uint32 count (deleted-block ids, element count, category/variable/trigger
// count, ECA/child-ECA count) that the decoder used to feed straight into
// make([]T, n). A bogus count (e.g. 0xFFFFFFFF) made the runtime attempt a
// multi-gigabyte allocation -> "out of memory" fatal crash. Worse than
// unitsdoo: the count is read with the reader's sticky-error API, so a benign
// truncation that still yields a huge (but successfully-read) count would OOM
// before the next read failed. The decoder now bounds every untrusted count
// against the bytes remaining (via reader.checkCount) before allocating, so
// these crafted inputs return a decode error promptly instead of OOMing.

func leW(b []byte, v uint32) []byte { return binary.LittleEndian.AppendUint32(b, v) }

// parseWithinDeadline runs Parse and fails the test if it does not return
// quickly. A successful bounds-check returns in microseconds; an unbounded
// allocation would either OOM-crash the process or spend seconds zeroing
// gigabytes.
func parseWithinDeadline(t *testing.T, data []byte) error {
	t.Helper()
	done := make(chan error, 1)
	go func() {
		_, err := Parse(data, map[string]int{})
		done <- err
	}()
	select {
	case err := <-done:
		return err
	case <-time.After(10 * time.Second):
		t.Fatal("Parse did not return within 10s — likely a multi-gigabyte allocation (the OOM regression)")
		return nil
	}
}

// pre31Header emits the version-4 (legacy, no WTG! magic) header. Parse
// re-interprets the first 4 bytes as the version when the magic is absent.
func pre31Header() []byte {
	return leW(nil, 4) // version 4 (pre-1.31), not 7 (so categories have no IsComment u32)
}

// v131Header emits the 1.31+ header (WTG! magic + version + sub_version) plus
// the seven empty deleted-id blocks, ending right before Unknown1. sub_version
// 4 keeps the per-element layout minimal.
func v131Header() []byte {
	b := []byte(wtgMagic)
	b = leW(b, 0x80000004) // version
	b = leW(b, 4)          // sub_version = 4
	for i := 0; i < 7; i++ {
		b = leW(b, 0) // count
		b = leW(b, 0) // count2 (n = 0 → no ids)
	}
	return b
}

func TestParse_HugeCounts_NoOOM(t *testing.T) {
	const huge = 0xFFFFFFFF

	cases := []struct {
		name  string
		build func() []byte
	}{
		{
			// pre-1.31: bogus category count.
			name: "pre31_category_count",
			build: func() []byte {
				b := pre31Header()
				b = leW(b, huge) // category count
				b = append(b, 0x00)
				return b
			},
		},
		{
			// pre-1.31: zero categories + unknown, then a bogus variable count.
			name: "pre31_variable_count",
			build: func() []byte {
				b := pre31Header()
				b = leW(b, 0)    // category count = 0
				b = leW(b, 0)    // PreCategoryUnknown
				b = leW(b, huge) // variable count
				b = append(b, 0x00)
				return b
			},
		},
		{
			// pre-1.31: through variables, then a bogus trigger count.
			name: "pre31_trigger_count",
			build: func() []byte {
				b := pre31Header()
				b = leW(b, 0)    // category count = 0
				b = leW(b, 0)    // PreCategoryUnknown
				b = leW(b, 0)    // variable count = 0
				b = leW(b, huge) // trigger count
				b = append(b, 0x00)
				return b
			},
		},
		{
			// 1.31+: bogus deleted-block id count (count2). This is the
			// allocation that fired BEFORE any later read error could surface.
			name: "v131_deleted_block_count",
			build: func() []byte {
				b := []byte(wtgMagic)
				b = leW(b, 0x80000004) // version
				b = leW(b, 4)          // sub_version
				b = leW(b, 0)          // block[0] count
				b = leW(b, huge)       // block[0] count2 (n) → make([]int32, huge)
				b = append(b, 0x00)
				return b
			},
		},
		{
			// 1.31+: zero deleted blocks + header, then a bogus element count.
			name: "v131_element_count",
			build: func() []byte {
				b := v131Header()
				b = leW(b, 0)    // Unknown1
				b = leW(b, 0)    // Unknown2
				b = leW(b, 0)    // TrigDefVer
				b = leW(b, 0)    // variable_count = 0
				b = leW(b, huge) // element_count
				b = append(b, 0x00)
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
			if !errors.Is(err, io.ErrUnexpectedEOF) {
				t.Logf("note: error did not wrap io.ErrUnexpectedEOF (still a clean rejection): %v", err)
			}
		})
	}
}

// TestParse_v131Header_Valid sanity-checks that v131Header + a zero
// variable/element tail parses cleanly — proving the v131 OOM cases above are
// rejected because of the hostile count, not a malformed prefix.
func TestParse_v131Header_Valid(t *testing.T) {
	b := v131Header()
	b = leW(b, 0) // Unknown1
	b = leW(b, 0) // Unknown2
	b = leW(b, 0) // TrigDefVer
	b = leW(b, 0) // variable_count = 0
	b = leW(b, 0) // element_count = 0
	tr, err := Parse(b, map[string]int{})
	if err != nil {
		t.Fatalf("Parse(valid v131 header): unexpected error: %v", err)
	}
	if tr.Version != 0x80000004 {
		t.Errorf("Version = 0x%X, want 0x80000004", tr.Version)
	}
	if len(tr.Categories) != 0 || len(tr.Variables) != 0 || len(tr.Triggers) != 0 {
		t.Errorf("expected empty tree, got cats=%d vars=%d trigs=%d",
			len(tr.Categories), len(tr.Variables), len(tr.Triggers))
	}
}
