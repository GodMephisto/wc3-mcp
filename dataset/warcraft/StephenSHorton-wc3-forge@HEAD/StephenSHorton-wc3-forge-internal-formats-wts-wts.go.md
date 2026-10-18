---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/wts/wts.go"
ref: HEAD
---

// Package wts parses war3map.wts — WC3's trigger-strings table.
//
// Format is a plain UTF-8 text file (often with a BOM), structured as a
// sequence of named blocks:
//
//	STRING 9661
//	{
//	Just another Warcraft III map
//	}
//
//	STRING 9664  // optional trailing comment
//	{
//	Map Author
//	}
//
// Most modern editors store map metadata (name, author, description, force
// names, player names, etc.) as integer IDs and stash the actual display
// strings here. The w3i parser surfaces those IDs as "TRIGSTR_<n>" strings;
// callers can pass them through [Strings.Resolve] to get the display value.
//
// Strings may span multiple lines. Comments outside string blocks (lines
// starting with "//") are ignored. Trailing whitespace on the closing "}"
// line is allowed.
package wts

import (
	"bufio"
	"bytes"
	"fmt"
	"regexp"
	"sort"
	"strconv"
	"strings"
)

// Strings maps a STRING id to its display value. Multi-line values keep
// their interior newlines verbatim (no normalization).
type Strings map[uint32]string

var (
	stringHeaderRe = regexp.MustCompile(`^STRING\s+(\d+)`)
	trigstrRe      = regexp.MustCompile(`^TRIGSTR_(\d+)$`)
	// WC3 inline color codes: opening |cAARRGGBB and closing |r.
	// Case-insensitive on the hex digits; Reforged maps mix cases.
	colorCodeRe = regexp.MustCompile(`\|[cC][0-9a-fA-F]{8}|\|[rR]`)
)

// Parse decodes a war3map.wts file. Returns an empty map (not nil) if the
// file is well-formed but contains no STRING blocks.
func Parse(data []byte) (Strings, error) {
	// Strip UTF-8 BOM if present — many editors prepend it.
	if len(data) >= 3 && data[0] == 0xef && data[1] == 0xbb && data[2] == 0xbf {
		data = data[3:]
	}

	out := Strings{}
	scanner := bufio.NewScanner(bytes.NewReader(data))
	// Some maps have very long description blocks; bump the buffer cap.
	scanner.Buffer(make([]byte, 64*1024), 4*1024*1024)

	var (
		currentID         uint32
		inString          bool
		awaitingOpenBrace bool
		body              strings.Builder
	)

	for scanner.Scan() {
		line := scanner.Text()
		trimmed := strings.TrimSpace(line)

		if !inString {
			if awaitingOpenBrace {
				if strings.HasPrefix(trimmed, "{") {
					inString = true
					awaitingOpenBrace = false
					body.Reset()
				}
				// Tolerate stray text between STRING header and "{".
				continue
			}
			if m := stringHeaderRe.FindStringSubmatch(trimmed); m != nil {
				id, err := strconv.ParseUint(m[1], 10, 32)
				if err != nil {
					continue
				}
				currentID = uint32(id)
				awaitingOpenBrace = true
			}
			continue
		}

		// Inside a string block.
		if trimmed == "}" || strings.HasPrefix(trimmed, "}") {
			out[currentID] = body.String()
			inString = false
			continue
		}
		if body.Len() > 0 {
			body.WriteByte('\n')
		}
		body.WriteString(line)
	}
	if err := scanner.Err(); err != nil {
		return nil, fmt.Errorf("scan wts: %w", err)
	}
	return out, nil
}

// Encode serializes a Strings table back to war3map.wts text in a canonical
// shape — one `STRING <id>\n{\n<value>\n}\n\n` block per entry, IDs ascending.
// It does NOT reproduce the source file byte-for-byte: a UTF-8 BOM, outside-block
// comments, and trailing-`}` whitespace are dropped (Parse already discards them
// too, so Parse(Encode(s)) round-trips the MAP even though Encode(Parse(file))
// won't match the original bytes). WC3 reads BOM-less UTF-8 fine.
//
// Multi-line values are written verbatim. A value containing a line whose first
// non-space char is `}` can't be represented (it would terminate the block on
// re-parse) — that's the same limitation Parse has and doesn't occur in real
// map metadata.
func Encode(s Strings) ([]byte, error) {
	ids := make([]uint32, 0, len(s))
	for id := range s {
		ids = append(ids, id)
	}
	sort.Slice(ids, func(i, j int) bool { return ids[i] < ids[j] })
	var b strings.Builder
	for _, id := range ids {
		fmt.Fprintf(&b, "STRING %d\n{\n%s\n}\n\n", id, s[id])
	}
	return []byte(b.String()), nil
}

// RefID reports whether ref is a "TRIGSTR_<n>" reference and, if so, the n.
// Mirrors the matching Resolve uses, exported so callers (the session's Map Info
// edit path) can detect a tokenized field and update the referenced wts entry
// instead of inlining a literal.
func RefID(ref string) (uint32, bool) {
	m := trigstrRe.FindStringSubmatch(ref)
	if m == nil {
		return 0, false
	}
	id, err := strconv.ParseUint(m[1], 10, 32)
	if err != nil {
		return 0, false
	}
	return uint32(id), true
}

// StripColorCodes removes inline WC3 color formatting (|cAARRGGBB ... |r)
// from a string. The game engine renders these as colored runs; in a plain
// text UI they're noise.
func StripColorCodes(s string) string {
	return colorCodeRe.ReplaceAllString(s, "")
}

// Display is the convenience the UI typically wants: resolve the TRIGSTR
// reference (if any), strip WC3 color codes, trim surrounding whitespace.
// All three transforms are no-ops for inputs that don't need them.
func (s Strings) Display(ref string) string {
	return strings.TrimSpace(StripColorCodes(s.Resolve(ref)))
}

// Resolve substitutes a TRIGSTR_<n> reference with its display value from
// the table. Strings that don't match the TRIGSTR pattern are returned
// unchanged — safe to call on any field whose origin you don't know.
// If the referenced id is missing from the table, returns ref unchanged
// rather than empty (so the missing id is still visible in the UI).
//
// A nil receiver is also safe — returns ref unchanged.
func (s Strings) Resolve(ref string) string {
	if s == nil {
		return ref
	}
	m := trigstrRe.FindStringSubmatch(ref)
	if m == nil {
		return ref
	}
	id, err := strconv.ParseUint(m[1], 10, 32)
	if err != nil {
		return ref
	}
	if v, ok := s[uint32(id)]; ok {
		return v
	}
	return ref
}
