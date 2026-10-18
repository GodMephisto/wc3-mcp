---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/imp/encode.go"
ref: HEAD
---

// Encode is the byte-faithful inverse of Parse: for a file produced by Parse,
// Encode(Parse(data)) returns bytes equal to data. Each entry's flag and path
// are written back verbatim, so resaving a map never disturbs its existing
// import table.
//
// Add and Has provide the mutation surface wc3-forge needs when registering a
// newly-imported model/texture: Add appends a war3mapImported\ path under the
// World-Editor standard flag (idempotently, normalizing slashes), and Has
// reports membership with the case-insensitive, slash-insensitive comparison
// WC3 paths require.

package imp

import "strings"

// Encode serializes f back to the on-disk war3map.imp binary format.
func (f *File) Encode() []byte {
	w := &writer{}
	w.writeU32(f.Version)
	w.writeU32(uint32(len(f.Entries)))
	for _, e := range f.Entries {
		w.writeU8(e.Flag)
		w.writeBytes([]byte(e.Path))
		w.writeU8(0) // NUL terminator
	}
	return w.bytes()
}

// Add registers path as a custom import under StandardFlag, idempotently.
//
// The path is normalized (forward slashes → backslashes) before storage and
// comparison, since callers may pass either convention. If an entry with the
// same normalized path already exists (case-insensitively), Add is a no-op so
// repeated imports of the same asset don't bloat the table or create
// duplicates the World Editor would flag. New entries always use StandardFlag
// (0x0D) — the editor's value for a fully-qualified war3mapImported\ path —
// regardless of any differing flag a pre-existing duplicate happened to carry.
func (f *File) Add(path string) {
	norm := normalizePath(path)
	if f.Has(norm) {
		return
	}
	f.Entries = append(f.Entries, Entry{Flag: StandardFlag, Path: norm})
}

// Has reports whether path is already registered, comparing case-insensitively
// and treating forward and back slashes as equivalent (WC3 archive paths are
// both case- and separator-insensitive). The argument may use either slash
// style and any casing.
func (f *File) Has(path string) bool {
	want := canonical(path)
	for _, e := range f.Entries {
		if canonical(e.Path) == want {
			return true
		}
	}
	return false
}

// Remove drops the entry matching path (case- and slash-insensitively),
// reporting whether one was removed. A no-op (returns false) when path isn't
// registered, so callers can treat removal idempotently. Mirrors Has's
// comparison so a path added with either slash style is removable by either.
func (f *File) Remove(path string) bool {
	want := canonical(path)
	for i, e := range f.Entries {
		if canonical(e.Path) == want {
			f.Entries = append(f.Entries[:i], f.Entries[i+1:]...)
			return true
		}
	}
	return false
}

// Rename changes the registered path of an existing entry from oldPath to
// newPath, preserving the entry's original flag byte (so renaming a stock-flag
// import doesn't silently rewrite its path-kind). newPath is normalized
// (forward slashes → backslashes) before storage. Returns whether the rename
// occurred — false when oldPath isn't registered. If newPath already exists as
// a separate entry it is removed first so the table never ends up with two
// rows for the same canonical path.
func (f *File) Rename(oldPath, newPath string) bool {
	wantOld := canonical(oldPath)
	norm := normalizePath(newPath)
	wantNew := canonical(norm)
	idx := -1
	for i, e := range f.Entries {
		if canonical(e.Path) == wantOld {
			idx = i
			break
		}
	}
	if idx < 0 {
		return false
	}
	// Drop any pre-existing distinct entry at the new path so the rename can't
	// create a duplicate (skip if it IS the entry we're renaming).
	for i := len(f.Entries) - 1; i >= 0; i-- {
		if i != idx && canonical(f.Entries[i].Path) == wantNew {
			f.Entries = append(f.Entries[:i], f.Entries[i+1:]...)
			if i < idx {
				idx--
			}
		}
	}
	f.Entries[idx].Path = norm
	return true
}

// normalizePath converts forward slashes to backslashes for on-disk storage,
// matching the war3map.imp / WC3-archive convention. Casing is left intact so
// the stored path keeps the caller's intended display form.
func normalizePath(path string) string {
	return strings.ReplaceAll(path, "/", "\\")
}

// canonical folds a path to the form used for equality: backslashes unified to
// forward slashes and lowercased. WC3 treats archive paths as both case- and
// separator-insensitive, so this is the correct comparison key.
func canonical(path string) string {
	return strings.ToLower(strings.ReplaceAll(path, "\\", "/"))
}

// --- writer ----------------------------------------------------------------

// writer is a minimal little-endian byte accumulator mirroring reader's
// surface (same shape as doodadsdoo / unitsdoo).
type writer struct {
	buf []byte
}

func (w *writer) bytes() []byte { return w.buf }

func (w *writer) writeBytes(b []byte) { w.buf = append(w.buf, b...) }

func (w *writer) writeU8(v uint8) { w.buf = append(w.buf, v) }

func (w *writer) writeU32(v uint32) {
	w.buf = append(w.buf, byte(v), byte(v>>8), byte(v>>16), byte(v>>24))
}
