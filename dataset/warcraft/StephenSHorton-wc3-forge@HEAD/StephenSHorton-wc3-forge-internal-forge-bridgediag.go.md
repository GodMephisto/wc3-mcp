---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/bridgediag.go"
ref: HEAD
---

package forge

import (
	"sync"
	"sync/atomic"
	"time"
)

// Bridge dispatch health, surfaced under the "bridge" namespace of
// diagnostics.get's "go" key. Populated by the instrumented() handler wrapper.
// The two highest-value signals here:
//   - panics: a handler that panicked instead of returning an error. Before the
//     recover() in instrumented(), a panic killed the dispatch silently; this
//     counter (and the [ERROR] log) makes it visible.
//   - inflightSlow: any MCP call that has been running > bridgeSlowCall. This is
//     how an agent SEES a wedge — e.g. the SetObjectField deadlock would show
//     the offending method stuck for seconds, instead of diagnostics.get itself
//     just timing out with no explanation.
//
// All atomics + a sync.Map; the provider never touches Current.mu (diagprov.go
// invariant) — it Ranges its own in-flight map.
var bridgeStats struct {
	calls       atomic.Int64
	panics      atomic.Int64
	inflightSeq atomic.Uint64
	inflight    sync.Map // uint64 seq -> inflightCall
}

type inflightCall struct {
	method string
	start  time.Time
}

const bridgeSlowCall = 2 * time.Second

func init() {
	RegisterGoDiag("bridge", func() any {
		now := time.Now()
		slow := []map[string]any{}
		bridgeStats.inflight.Range(func(_, v any) bool {
			c := v.(inflightCall)
			if el := now.Sub(c.start); el >= bridgeSlowCall {
				slow = append(slow, map[string]any{"method": c.method, "elapsedMs": el.Milliseconds()})
			}
			return true
		})
		pid, port, _ := BridgeIdentity()
		return map[string]any{
			"pid":          pid,
			"port":         port,
			"calls":        bridgeStats.calls.Load(),
			"panics":       bridgeStats.panics.Load(), // ends in 's' — not auto-red; but nonzero means a handler crashed
			"inflightSlow": slow,
		}
	})
}
