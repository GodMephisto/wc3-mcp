---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/debuglog.ts"
ref: HEAD
---

// Frontend → Go log bridge. Wails GUI apps have no devtools console in
// production builds, so we route diagnostics through a bound Go method
// that writes to a file next to the executable (wc3-forge.log).

import { LogJS } from '../wailsjs/go/main/App.js'

function fmt(args: unknown[]): string {
  return args
    .map(a => {
      if (a instanceof Error) return `${a.name}: ${a.message}\n${a.stack || ''}`
      if (typeof a === 'object') {
        try { return JSON.stringify(a) } catch { return String(a) }
      }
      return String(a)
    })
    .join(' ')
}

// Log levels, low→high verbosity. The active threshold gates which calls
// actually emit: a message is emitted when its level is <= the current
// threshold. Default is 'info' (error+warn+info); 'debug' (Verbose Logging
// toggle) additionally lets the noisy per-frame/per-asset traces through.
export type LogLevel = 'error' | 'warn' | 'info' | 'debug'
const LEVELS: Record<LogLevel, number> = { error: 0, warn: 1, info: 2, debug: 3 }
let threshold = LEVELS.info

export function setLogLevel(level: LogLevel) {
  threshold = LEVELS[level] ?? LEVELS.info
  ;(window as any).__wc3ForgeLogLevel = level
}
export function getLogLevel(): LogLevel {
  return (Object.keys(LEVELS) as LogLevel[]).find(k => LEVELS[k] === threshold) ?? 'info'
}

function emit(level: LogLevel, tag: string, args: unknown[]) {
  if (LEVELS[level] > threshold) return
  const msg = tag ? `${tag} ${fmt(args)}` : fmt(args)
  try { LogJS(msg) } catch { /* ignore */ }
  try { console.log(...(tag ? [tag, ...args] : args)) } catch { /* ignore */ }
}

// flog stays info-level for backwards compatibility with existing call sites.
export function flog(...args: unknown[]) { emit('info', '', args) }
export function flogError(...args: unknown[]) { emit('error', '[ERROR]', args) }
export function flogWarn(...args: unknown[]) { emit('warn', '[WARN]', args) }
export function flogInfo(...args: unknown[]) { emit('info', '', args) }
// Debug-level: silent unless Verbose Logging is on. Use for per-frame / per-
// asset traces that would otherwise flood the log.
export function flogDebug(...args: unknown[]) { emit('debug', '[DEBUG]', args) }

// Catch otherwise-silent module-load / runtime errors.
if (typeof window !== 'undefined') {
  window.addEventListener('error', e => {
    flog('window.error:', e.message, 'at', e.filename + ':' + e.lineno + ':' + e.colno, e.error)
  })
  window.addEventListener('unhandledrejection', e => {
    flog('unhandledrejection:', e.reason)
  })
}
