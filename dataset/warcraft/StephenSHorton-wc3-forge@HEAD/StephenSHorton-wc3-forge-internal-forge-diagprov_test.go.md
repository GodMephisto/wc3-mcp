---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/diagprov_test.go"
ref: HEAD
---

package forge

import (
	"testing"
	"time"
)

// unregisterGoDiag removes a provider — test-only cleanup so registrations don't
// leak between tests.
func unregisterGoDiag(ns string) {
	goDiagMu.Lock()
	delete(goDiagProviders, ns)
	goDiagMu.Unlock()
}

// TestCollectGoDiag_DoesNotBlockOnSessionLock is the load-bearing guard: it
// holds Current.mu (the session write lock) for the whole collection and
// asserts collectGoDiag still returns promptly. If collectGoDiag — or any
// provider it calls — tried to read Current.mu, this would deadlock (caught by
// the test timeout). This is the structural defense against diagnostics.get
// wedging on the very lock contention it exists to observe.
func TestCollectGoDiag_DoesNotBlockOnSessionLock(t *testing.T) {
	RegisterGoDiag("test-ok", func() any { return map[string]any{"n": 1} })
	defer unregisterGoDiag("test-ok")

	Current.mu.Lock()
	defer Current.mu.Unlock()

	done := make(chan map[string]any, 1)
	go func() { done <- collectGoDiag() }()
	select {
	case out := <-done:
		if _, ok := out["test-ok"]; !ok {
			t.Fatalf("provider result missing: %v", out)
		}
	case <-time.After(time.Second):
		t.Fatal("collectGoDiag blocked while Current.mu was held — a provider must be reading the session lock")
	}
}

// TestCollectGoDiag_TimesOutWedgedProvider asserts a provider that hangs past
// goProviderTimeout is skipped with a sentinel rather than stalling the whole
// response.
func TestCollectGoDiag_TimesOutWedgedProvider(t *testing.T) {
	RegisterGoDiag("test-wedge", func() any { time.Sleep(2 * time.Second); return 1 })
	defer unregisterGoDiag("test-wedge")

	start := time.Now()
	out := collectGoDiag()
	elapsed := time.Since(start)

	if elapsed > 500*time.Millisecond {
		t.Fatalf("collectGoDiag took %v — timeout backstop did not fire", elapsed)
	}
	m, ok := out["test-wedge"].(map[string]any)
	if !ok || m["_providerTimeout"] != true {
		t.Fatalf("expected _providerTimeout sentinel, got %v", out["test-wedge"])
	}
}

// TestCollectGoDiag_RecoversPanic asserts a panicking provider is isolated to
// its namespace and the rest still collect.
func TestCollectGoDiag_RecoversPanic(t *testing.T) {
	RegisterGoDiag("test-panic", func() any { panic("boom") })
	RegisterGoDiag("test-fine", func() any { return 42 })
	defer unregisterGoDiag("test-panic")
	defer unregisterGoDiag("test-fine")

	out := collectGoDiag()
	m, ok := out["test-panic"].(map[string]any)
	if !ok || m["_providerError"] == nil {
		t.Fatalf("expected _providerError sentinel, got %v", out["test-panic"])
	}
	if out["test-fine"] != 42 {
		t.Fatalf("sibling provider should still collect, got %v", out["test-fine"])
	}
}
