---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/diagprov.go"
ref: HEAD
---

package forge

import (
	"sync"
	"time"
)

// Go-side diagnostics provider registry. A Go subsystem (asset handler, CASC,
// bridge, map I/O, …) registers a named provider that returns a small JSON-able
// snapshot of its own counters; collectGoDiag merges them all under the "go"
// key of the diagnostics.get response. This lets each subsystem contribute
// debug signals WITHOUT editing the diagnostics handler, mirroring the frontend
// diag-registry.
//
// HARD INVARIANT — a provider MUST NOT acquire Current.mu (the session lock).
// diagnostics.get exists partly to DETECT lock wedges (e.g. the SetObjectField
// deadlock); if a provider blocked on Current.mu it would wedge the very tool
// meant to observe the wedge. Providers must read dedicated atomics or copy a
// snapshot under their OWN short-lived lock. The per-provider timeout below is
// the runtime backstop — a wedged provider is skipped, not allowed to hang the
// response — and diagprov_test.go asserts the timeout actually fires.
type GoDiagProvider func() any

var (
	goDiagMu        sync.RWMutex
	goDiagProviders = map[string]GoDiagProvider{}
)

// RegisterGoDiag registers (or replaces) a provider under a namespace. Call once
// at package init / first use. Safe for concurrent registration.
func RegisterGoDiag(ns string, p GoDiagProvider) {
	goDiagMu.Lock()
	goDiagProviders[ns] = p
	goDiagMu.Unlock()
}

// goProviderTimeout caps how long a single provider may run before
// collectGoDiag gives up on it and records a timeout sentinel. Generous enough
// for any honest atomic/snapshot read, tight enough that a misbehaving provider
// can't visibly stall diagnostics.get.
const goProviderTimeout = 50 * time.Millisecond

// collectGoDiag runs every registered provider with per-provider panic-recovery
// and a timeout, returning a map keyed by namespace. A panicking provider
// surfaces as {"_providerError": ...}; a slow/wedged one as
// {"_providerTimeout": true} — never a hang and never a crash.
func collectGoDiag() map[string]any {
	goDiagMu.RLock()
	snapshot := make(map[string]GoDiagProvider, len(goDiagProviders))
	for ns, p := range goDiagProviders {
		snapshot[ns] = p
	}
	goDiagMu.RUnlock()

	out := make(map[string]any, len(snapshot))
	for ns, p := range snapshot {
		out[ns] = runProvider(p)
	}
	return out
}

// runProvider executes one provider in a goroutine bounded by goProviderTimeout,
// recovering any panic. This is what makes the "never read Current.mu" rule a
// soft failure (timeout) rather than a hard hang if it's ever violated.
func runProvider(p GoDiagProvider) any {
	done := make(chan any, 1)
	go func() {
		defer func() {
			if r := recover(); r != nil {
				done <- map[string]any{"_providerError": toErrString(r)}
			}
		}()
		done <- p()
	}()
	select {
	case v := <-done:
		return v
	case <-time.After(goProviderTimeout):
		return map[string]any{"_providerTimeout": true}
	}
}

func toErrString(r any) string {
	if err, ok := r.(error); ok {
		return err.Error()
	}
	return "panic"
}
