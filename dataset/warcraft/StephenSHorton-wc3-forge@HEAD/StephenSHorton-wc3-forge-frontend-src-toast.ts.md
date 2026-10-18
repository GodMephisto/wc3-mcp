---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/toast.ts"
ref: HEAD
---

// Transient operational error / info / warning notifications.
//
// Persistent state errors (e.g. "scene init failed") still live in the top
// error band inside App.svelte. Anything that fires from a user-triggered
// action (Save, Open, MoveUnit, toggleReforged…) is transient — it belongs
// in a toast that auto-dismisses, NOT a band that sticks until dismissed.
//
// API: showToast(message, severity?, durationMs?)
//   - severity defaults to 'info' (neutral)
//   - durationMs defaults to TOAST_DEFAULT_MS (5s); pass 0 to disable auto-dismiss
//   - returns the toast id (string | number) issued by svelte-sonner; pass it
//     to dismissToast() to remove the toast early.
//
// Implementation notes:
//   - Backed by svelte-sonner via shadcn-svelte's <Toaster> wrapper. The
//     <Toaster /> must be mounted exactly once in App.svelte (it lives in
//     $lib/components/ui/sonner). Sonner owns queueing, animation, stacking,
//     and auto-dismiss timers — we just dispatch and forget here.
//   - The Toast / ToastSeverity types stay exported for any consumers that
//     still reference them; the legacy `toasts` writable store and TOAST_MAX
//     cap are gone (Sonner manages visible-stack capping internally via its
//     `visibleToasts` prop on <Toaster>).

import { toast } from 'svelte-sonner'

export type ToastSeverity = 'success' | 'info' | 'warning' | 'error'

export interface Toast {
  id: string | number
  message: string
  severity: ToastSeverity
  durationMs: number  // 0 → no auto-dismiss
}

export const TOAST_DEFAULT_MS = 5000

export function showToast(
  message: string,
  severity: ToastSeverity = 'info',
  durationMs: number = TOAST_DEFAULT_MS,
): string | number {
  // Sonner reads `duration: Infinity` as "do not auto-dismiss". The legacy
  // contract used 0 for the same thing — map it here so callers don't have
  // to change.
  const duration = durationMs > 0 ? durationMs : Infinity
  const opts = { duration }
  switch (severity) {
    case 'success': return toast.success(message, opts)
    case 'warning': return toast.warning(message, opts)
    case 'error':   return toast.error(message, opts)
    case 'info':
    default:        return toast.info(message, opts)
  }
}

export function dismissToast(id: string | number): void {
  toast.dismiss(id)
}
