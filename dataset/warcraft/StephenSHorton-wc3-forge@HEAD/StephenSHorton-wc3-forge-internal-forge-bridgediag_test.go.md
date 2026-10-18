---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/bridgediag_test.go"
ref: HEAD
---

package forge

import (
	"encoding/json"
	"errors"
	"testing"
)

// TestInstrumented_RecoversPanic asserts a panicking handler is converted to a
// normal error (not propagated to crash the bridge connection), counted, and
// that its in-flight entry is cleaned up.
func TestInstrumented_RecoversPanic(t *testing.T) {
	before := bridgeStats.panics.Load()
	h := instrumented("test.panic", func(json.RawMessage) (any, error) { panic("kaboom") })

	res, err := h(nil)
	if err == nil {
		t.Fatal("expected an error from a panicking handler, got nil")
	}
	if res != nil {
		t.Fatalf("expected nil result on panic, got %v", res)
	}
	if got := bridgeStats.panics.Load(); got != before+1 {
		t.Fatalf("panics counter = %d, want %d", got, before+1)
	}
	n := 0
	bridgeStats.inflight.Range(func(_, _ any) bool { n++; return true })
	if n != 0 {
		t.Fatalf("in-flight map not cleaned up after panic: %d entries", n)
	}
}

// TestInstrumented_PassesThroughError asserts the wrapper is transparent for a
// normal error return (no panic path).
func TestInstrumented_PassesThroughError(t *testing.T) {
	want := errors.New("ordinary failure")
	h := instrumented("test.err", func(json.RawMessage) (any, error) { return nil, want })
	_, err := h(nil)
	if !errors.Is(err, want) {
		t.Fatalf("error not passed through: got %v", err)
	}
}
