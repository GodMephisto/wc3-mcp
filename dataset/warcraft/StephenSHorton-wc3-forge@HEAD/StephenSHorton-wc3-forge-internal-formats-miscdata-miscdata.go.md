---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/miscdata/miscdata.go"
ref: HEAD
---

// Package miscdata parses + emits war3mapMisc.txt — the per-map gameplay-
// constants override file.
//
// WC3 ships its stock gameplay constants in Units/MiscData.txt and
// Units/MiscGame.txt inside the game CASC. A map may include its own
// war3mapMisc.txt at the archive root to override individual constants
// (e.g. MaxHeroLevel=10, BoneDecayTime=0.5). The override is per-key:
// any key not listed inherits the stock value.
//
// Format is INI-style with an optional UTF-8 BOM:
//
//	[Misc]
//	// Comment lines start with // or ;
//	BoneDecayTime=0.1
//	DamageBonusNormal=1.00,2.00,1.00,1.00,1.00,1.00,1.00,1.00
//	MaxHeroLevel=10
//
// Practical maps only ever populate [Misc], but the parser handles arbitrary
// section names since the format is generic INI.
//
// Round-trip: Parse → Encode preserves section order, entry order, and
// values verbatim. Comments are NOT preserved (the parser drops them on the
// way in); for an override file users edit through a UI, that's the desired
// behavior — we re-emit a clean canonical form.
package miscdata

import (
	"bufio"
	"bytes"
	"fmt"
	"strings"
)

// File is the parsed war3mapMisc.txt. Sections preserve original order so
// Parse→Encode round-trips reasonably. The default+only section in real
// maps is [Misc].
type File struct {
	Sections []*Section
}

// Section is one [Header] block plus its entries (in original order).
type Section struct {
	Name    string
	Entries []*Entry
}

// Entry is one key=value pair. Value is the raw string AFTER any trailing
// inline comment has been stripped and trailing whitespace trimmed. For
// list-valued keys (e.g. DamageBonusNormal=1,2,3,4) callers can split on
// commas themselves; the wire format puts no quoting around list elements,
// so a simple strings.Split(v, ",") is enough.
type Entry struct {
	Key   string
	Value string
}

// Parse decodes a war3mapMisc.txt payload. An empty or whitespace-only
// input yields an empty File (no sections), not an error — maps without
// any overrides legitimately have no entries.
//
// Duplicate keys within the same section: FIRST wins (matches HiveWE's
// behavior so we don't silently change semantics when a hand-edited map
// has accidental duplicates).
func Parse(data []byte) (*File, error) {
	if len(data) >= 3 && data[0] == 0xef && data[1] == 0xbb && data[2] == 0xbf {
		data = data[3:]
	}

	f := &File{}
	var cur *Section
	// Track per-section seen keys so dup detection is per-section (a key may
	// legitimately appear in two different sections).
	seen := map[string]map[string]struct{}{}

	scanner := bufio.NewScanner(bytes.NewReader(data))
	scanner.Buffer(make([]byte, 64*1024), 4*1024*1024)

	for scanner.Scan() {
		line := scanner.Text()
		trimmed := strings.TrimSpace(line)
		if trimmed == "" {
			continue
		}
		if strings.HasPrefix(trimmed, "//") || strings.HasPrefix(trimmed, ";") {
			continue
		}
		if strings.HasPrefix(trimmed, "[") && strings.HasSuffix(trimmed, "]") {
			name := strings.TrimSpace(trimmed[1 : len(trimmed)-1])
			cur = &Section{Name: name}
			f.Sections = append(f.Sections, cur)
			if _, ok := seen[name]; !ok {
				seen[name] = map[string]struct{}{}
			}
			continue
		}
		eq := strings.IndexByte(trimmed, '=')
		if eq < 0 {
			// Malformed line — ignore rather than error so a typo in one
			// row doesn't break the whole file.
			continue
		}
		key := strings.TrimSpace(trimmed[:eq])
		if key == "" {
			continue
		}
		value := stripInlineComment(trimmed[eq+1:])
		value = strings.TrimSpace(value)

		// Implicit default section if the file lacks a [Header] before
		// the first entry. Real war3mapMisc.txt always starts with [Misc]
		// but be tolerant.
		if cur == nil {
			cur = &Section{Name: "Misc"}
			f.Sections = append(f.Sections, cur)
			seen["Misc"] = map[string]struct{}{}
		}
		if _, dup := seen[cur.Name][key]; dup {
			continue
		}
		seen[cur.Name][key] = struct{}{}
		cur.Entries = append(cur.Entries, &Entry{Key: key, Value: value})
	}
	if err := scanner.Err(); err != nil {
		return nil, fmt.Errorf("scan miscdata: %w", err)
	}
	return f, nil
}

// stripInlineComment trims trailing "// ..." or "; ..." comments from a
// value. Conservative: only strips when the comment marker is preceded
// by whitespace OR is at column 0, so values like "C:\\path\\with;chars"
// or "1//2" (theoretical) are preserved.
func stripInlineComment(s string) string {
	for _, marker := range []string{"//", ";"} {
		idx := strings.Index(s, marker)
		if idx < 0 {
			continue
		}
		if idx == 0 || s[idx-1] == ' ' || s[idx-1] == '\t' {
			s = s[:idx]
		}
	}
	return s
}

// Encode serializes a File back to bytes. The output starts with a UTF-8
// BOM (HiveWE writes one and other tools tolerate it), then one section
// per [Header] followed by its entries. Trailing newline at EOF.
//
// Encode is total: any *File produced by Parse round-trips; a hand-built
// File round-trips as long as section + key names contain no control
// characters or '=' / '[' / ']'.
func Encode(f *File) ([]byte, error) {
	if f == nil {
		return nil, fmt.Errorf("encode miscdata: nil file")
	}
	var b bytes.Buffer
	b.Write([]byte{0xef, 0xbb, 0xbf})
	for i, s := range f.Sections {
		if s == nil {
			continue
		}
		if i > 0 {
			b.WriteByte('\n')
		}
		b.WriteByte('[')
		b.WriteString(s.Name)
		b.WriteByte(']')
		b.WriteByte('\n')
		for _, e := range s.Entries {
			if e == nil || e.Key == "" {
				continue
			}
			b.WriteString(e.Key)
			b.WriteByte('=')
			b.WriteString(e.Value)
			b.WriteByte('\n')
		}
	}
	return b.Bytes(), nil
}

// Get returns the value of key in the named section, or ("", false) if
// either the section or the key isn't present. Convenience for callers
// that don't want to walk the slice themselves.
func (f *File) Get(section, key string) (string, bool) {
	if f == nil {
		return "", false
	}
	for _, s := range f.Sections {
		if s == nil || s.Name != section {
			continue
		}
		for _, e := range s.Entries {
			if e != nil && e.Key == key {
				return e.Value, true
			}
		}
		return "", false
	}
	return "", false
}

// Set inserts or updates an entry. If the section doesn't exist yet, it's
// appended. If the key exists in that section, its value is replaced in
// place (preserving position); otherwise the entry is appended to the
// section. Returns the (possibly-new) Section pointer for chained calls.
func (f *File) Set(section, key, value string) *Section {
	if f == nil {
		return nil
	}
	var s *Section
	for _, candidate := range f.Sections {
		if candidate != nil && candidate.Name == section {
			s = candidate
			break
		}
	}
	if s == nil {
		s = &Section{Name: section}
		f.Sections = append(f.Sections, s)
	}
	for _, e := range s.Entries {
		if e != nil && e.Key == key {
			e.Value = value
			return s
		}
	}
	s.Entries = append(s.Entries, &Entry{Key: key, Value: value})
	return s
}

// Delete removes the entry (section, key). Returns true if anything was
// removed. An empty section is left in place so the [Misc] header still
// emits — that matches what users expect after deleting the last override.
func (f *File) Delete(section, key string) bool {
	if f == nil {
		return false
	}
	for _, s := range f.Sections {
		if s == nil || s.Name != section {
			continue
		}
		for i, e := range s.Entries {
			if e != nil && e.Key == key {
				s.Entries = append(s.Entries[:i], s.Entries[i+1:]...)
				return true
			}
		}
		return false
	}
	return false
}
