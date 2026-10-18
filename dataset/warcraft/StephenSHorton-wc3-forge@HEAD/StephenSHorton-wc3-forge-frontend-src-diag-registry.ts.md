---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/diag-registry.ts"
ref: HEAD
---

// diag-registry.ts — central registry so any frontend subsystem can contribute
// diagnostic signals WITHOUT editing the shared getDiagnostics()/overlay/handler
// code. A subsystem owns its own module-level counters and registers a provider
// once at import/init time; the signals then flow automatically through
// scene.getDiagnostics() → the 5Hz diagnostics push → the diagnostics.get MCP
// tool AND the DiagnosticsOverlay's generic section.
//
// PROVIDER CONTRACT (hard — keep these or you regress the very stalls this
// system exists to detect):
//   - O(1) over PRECOMPUTED counters. Do NOT scan unitInstances/doodadInstances
//     or anything proportional to scene size.
//   - MUST NOT call any gl.* method. GL reads (getError/getParameter) force a
//     CPU↔GPU sync; the render loop is 60–240fps. GL-touching probes belong in
//     the armed path inside the render loop, not here.
//   - Synchronous, no allocation storms. Return a small plain object.
//   - Own your counters; register at module init.
//
// The collector wraps every provider in try/catch so one broken provider
// surfaces as { _providerError } instead of throwing and freezing the 200ms
// diagnostics pump (which would itself look like a hang).

type DiagProvider = () => Record<string, unknown>
/** Armed providers run only when diagnostics mode is on (F9 / diagnostics.arm). */
type ArmedProvider = () => Record<string, unknown>

const providers = new Map<string, DiagProvider>()
const armedProviders = new Map<string, ArmedProvider>()

/** Register an always-on signal provider under a namespace (e.g. 'assets'). */
export function registerDiag(ns: string, p: DiagProvider): void {
  providers.set(ns, p)
}

/**
 * Register an armed-only provider — runs only when diagnostics mode is on, for
 * signals that are too expensive (or too noisy) to compute every pump tick.
 * Merged into the same namespace object as the always-on provider.
 */
export function registerArmedDiag(ns: string, p: ArmedProvider): void {
  armedProviders.set(ns, p)
}

/**
 * Collect every registered namespace into one object. When `armed`, also runs
 * the armed providers and merges their fields into the same namespaces. A
 * throwing provider is isolated to its namespace as _providerError/_armedError.
 */
export function collectDiag(armed: boolean): Record<string, unknown> {
  const out: Record<string, unknown> = {}
  for (const [ns, p] of providers) {
    try {
      out[ns] = p()
    } catch (e) {
      out[ns] = { _providerError: e instanceof Error ? e.message : String(e) }
    }
  }
  if (armed) {
    for (const [ns, p] of armedProviders) {
      try {
        const base = (out[ns] ??= {}) as Record<string, unknown>
        Object.assign(base, p())
      } catch (e) {
        ;(out[ns] as Record<string, unknown> | undefined ?? (out[ns] = {}) as Record<string, unknown>)._armedError =
          e instanceof Error ? e.message : String(e)
      }
    }
  }
  return out
}

/** Namespaces currently registered — handy for the overlay's generic section. */
export function diagNamespaces(): string[] {
  return [...new Set([...providers.keys(), ...armedProviders.keys()])].sort()
}
