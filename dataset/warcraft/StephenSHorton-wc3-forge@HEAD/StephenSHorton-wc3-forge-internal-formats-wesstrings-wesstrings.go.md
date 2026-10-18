---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/wesstrings/wesstrings.go"
ref: HEAD
---

// Package wesstrings reads WC3's stock string tables. These live in
// CASC as INI-shaped files (e.g. UI/WorldEditStrings.txt,
// UI/WorldEditGameStrings.txt) with a single `[WorldEditStrings]` section
// followed by `WESTRING_FOO=Display Text` lines.
//
// The SLK pipeline often returns cell values like `WESTRING_UNITNAME_PALADIN`
// or `WESTRING_DTYPE_DESTRUCTABLE` — those are keys into this table.
// We expose a flat case-insensitive map and a Resolve helper that no-ops
// on non-WESTRING inputs so callers can pipe any cell value through it.
package wesstrings

import (
	"bufio"
	"bytes"
	"strings"
)

// Table holds the merged WESTRING_* → display-text mapping. Lookups are
// case-insensitive on the key — game data inconsistently mixes case
// (WESTRING_UnitName_Paladin vs WESTRING_UNITNAME_PALADIN).
type Table struct {
	m map[string]string
}

func New() *Table { return &Table{m: map[string]string{}} }

// Load merges one INI-shaped string file into t. Section headers are
// ignored — we only care about the `KEY=VALUE` lines. Lines starting
// with `//` or `;`, blank lines, and the UTF-8 BOM are tolerated.
// Values quoted in `"..."` get their wrapping quotes stripped.
func (t *Table) Load(buf []byte) error {
	buf = bytes.TrimPrefix(buf, []byte{0xEF, 0xBB, 0xBF})
	sc := bufio.NewScanner(bytes.NewReader(buf))
	sc.Buffer(make([]byte, 0, 64*1024), 1024*1024)
	for sc.Scan() {
		line := strings.TrimSpace(sc.Text())
		if line == "" || strings.HasPrefix(line, "//") || strings.HasPrefix(line, ";") || strings.HasPrefix(line, "[") {
			continue
		}
		eq := strings.IndexByte(line, '=')
		if eq < 0 {
			continue
		}
		key := strings.TrimSpace(line[:eq])
		val := strings.TrimSpace(line[eq+1:])
		if key == "" {
			continue
		}
		if len(val) >= 2 && val[0] == '"' && val[len(val)-1] == '"' {
			val = val[1 : len(val)-1]
		}
		t.m[strings.ToLower(key)] = val
	}
	return sc.Err()
}

// Lookup returns the display text for a WESTRING key (case-insensitive).
// Empty string if absent.
func (t *Table) Lookup(key string) string {
	if t == nil {
		return ""
	}
	return t.m[strings.ToLower(key)]
}

// Resolve substitutes a `WESTRING_FOO` reference with its display text.
// Returns the input unchanged for non-WESTRING strings or missing keys.
// Safe on a nil receiver.
func (t *Table) Resolve(s string) string {
	if t == nil || !strings.HasPrefix(s, "WESTRING_") {
		return s
	}
	if v, ok := t.m[strings.ToLower(s)]; ok && v != "" {
		return v
	}
	return s
}
