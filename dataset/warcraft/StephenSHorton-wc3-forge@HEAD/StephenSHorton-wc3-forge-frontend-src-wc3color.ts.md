---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/wc3color.ts"
ref: HEAD
---

// WC3 inline color-code parser.
//
// WC3 source strings (unit names, tooltips, ability descriptions) can carry
// inline color formatting:
//
//   |cAARRGGBB<colored text>|r
//
// where AA is alpha (the game ignores it at runtime but we preserve it for
// fidelity), RR/GG/BB are the channel bytes. The opening tag is
// case-insensitive on both the literal `c` and the hex digits (Reforged maps
// mix cases freely). Color runs can be unterminated — the implicit close at
// end-of-string is the standard WC3 behavior.
//
// This module turns one of those strings into an array of `{text, color}`
// segments so a UI layer can render each segment in its own color span. Plain
// text outside any color run gets `color: undefined`.

export interface ColorSegment {
  text: string
  /** CSS color string ("rgba(r, g, b, a/255)") or undefined for plain text. */
  color?: string
}

// Opening tag: |c then 8 hex digits, case-insensitive.
// Closing tag: |r, case-insensitive.
// We scan with a regex that matches EITHER; the parser uses lastIndex semantics
// to walk the string linearly.
const TAG_RE = /\|([cC][0-9a-fA-F]{8}|[rR])/g

/**
 * Parse a WC3-color-coded string into segments. Empty input yields one empty
 * plain segment so callers don't have to special-case length 0.
 *
 * Quirks handled:
 *   - Unterminated |c runs apply to the rest of the string.
 *   - Nested |c (rare but possible) follows last-write-wins — each new |c
 *     overrides the active color; |r resets to plain.
 *   - Empty colored bodies (e.g. "|cFFFF0000|r") produce no segment (we skip
 *     zero-length runs).
 *   - Malformed |c without 8 hex digits is treated as literal text — the regex
 *     simply won't match it.
 */
export function parseWC3Color(input: string): ColorSegment[] {
  if (!input) return [{ text: '' }]
  const out: ColorSegment[] = []
  let lastIndex = 0
  let currentColor: string | undefined = undefined
  let m: RegExpExecArray | null
  TAG_RE.lastIndex = 0
  while ((m = TAG_RE.exec(input)) !== null) {
    // Emit the chunk between the previous tag and this one in the current color.
    if (m.index > lastIndex) {
      const text = input.substring(lastIndex, m.index)
      if (text.length > 0) {
        out.push({ text, color: currentColor })
      }
    }
    const tag = m[1]
    if (tag[0] === 'r' || tag[0] === 'R') {
      currentColor = undefined
    } else {
      // |cAARRGGBB — first 2 hex are alpha, then RR GG BB.
      const hex = tag.substring(1) // 8 chars
      const a = parseInt(hex.substring(0, 2), 16) / 255
      const r = parseInt(hex.substring(2, 4), 16)
      const g = parseInt(hex.substring(4, 6), 16)
      const b = parseInt(hex.substring(6, 8), 16)
      currentColor = `rgba(${r}, ${g}, ${b}, ${a.toFixed(3)})`
    }
    lastIndex = m.index + m[0].length
  }
  // Trailing text after the last tag (or the entire input if none matched).
  if (lastIndex < input.length) {
    out.push({ text: input.substring(lastIndex), color: currentColor })
  }
  // Guarantee non-empty output so callers don't have to branch.
  if (out.length === 0) {
    out.push({ text: '' })
  }
  return out
}

/**
 * Strip all WC3 color tags from a string. Mirrors the Go-side
 * wts.StripColorCodes — used as a cheap inline alternative when the caller
 * just wants plain text.
 */
export function stripWC3Color(input: string): string {
  if (!input) return ''
  return input.replace(TAG_RE, '')
}
