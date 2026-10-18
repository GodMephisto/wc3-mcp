---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/imp/imp.go"
ref: HEAD
---

// Package imp parses the war3map.imp import table — the registry Warcraft III's
// World Editor and the game engine consult to know which custom files a map has
// imported. Every asset placed under the map archive (models, textures, custom
// object-data shadows, etc.) is listed here so the editor surfaces it in the
// Import Manager and the engine resolves it at load time.
//
// File layout (little-endian throughout):
//
//	uint32  version            = 1 (the only version observed in the wild)
//	uint32  count              number of entries
//	Entry[count]
//
//	Entry:
//	  uint8   flag             path-kind flag (see below)
//	  char[]  path             NUL-terminated, backslash-separated
//
// # Flag-byte semantics (confirmed against real fixtures, NOT guessed)
//
// The single leading flag byte distinguishes how the engine resolves the path.
// Empirically (see testdata/war3map.imp lifted from HiveWE's "test map", plus
// larger Enfo / campaign import tables surveyed during development):
//
//   - 0x0D (13) — "custom / full path". The overwhelmingly common value the
//     World Editor writes for a fully-qualified path. Used for
//     war3mapImported\Foo.mdx, ReplaceableTextures\..., abilities\Spells\...,
//     and the war3mapSkin.* object shadows. This is the flag Add uses.
//   - 0x08 (8)  — "default directory". Used for BARE filenames (e.g.
//     "OrbOfFire.mdx", "Crystal.mdx") where the editor implicitly prepends
//     war3mapImported\ at load time.
//   - 0x05 (5)  — historically documented "standard" sibling of 0x08; not seen
//     in the surveyed fixtures but preserved verbatim if encountered.
//   - 0x0A (10), 0x1D (29) — other custom variants observed (0x1D appears in
//     the test-map fixture for "Crystal.mdx"). The engine treats anything with
//     bit 0x08 set as a literal/custom path; the exact byte is otherwise
//     cosmetic to the editor.
//
// Crucially, the only thing this package guarantees is byte-faithful
// round-tripping: Parse preserves each entry's flag verbatim and Encode writes
// it back unchanged, so resaving a map never disturbs the existing import
// table. Add (the only place that mints a new flag) uses the World-Editor
// standard 0x0D for the war3mapImported\ paths wc3-forge writes.
//
// Mirrors the sibling internal/formats/* packages (parser + encode.go +
// _test.go + testdata/), including the minimal little-endian cursor shape used
// by doodadsdoo / unitsdoo.
package imp

import (
	"encoding/binary"
	"fmt"
	"io"
)

// StandardFlag is the path-kind flag wc3-forge writes for a fully-qualified
// custom import path (the value the World Editor uses for war3mapImported\
// entries). See the package doc for the full flag-byte survey.
const StandardFlag byte = 0x0D

// Entry is one import-table row: a path-kind flag plus the NUL-terminated
// (on disk) backslash-separated path. Path is stored WITHOUT its trailing NUL.
type Entry struct {
	// Flag is the path-kind byte. Preserved verbatim across Parse→Encode so
	// resaving never rewrites an existing map's import flags. See package doc.
	Flag byte
	// Path is the import path, backslash-separated (e.g.
	// "war3mapImported\Foo.mdx"). No trailing NUL.
	Path string
}

// File is the parsed contents of a war3map.imp file.
type File struct {
	// Version is the table format version. Always 1 in practice; preserved so
	// Encode reproduces the original byte-for-byte.
	Version uint32
	Entries []Entry
}

// Parse decodes a complete war3map.imp file from raw bytes.
//
// Errors:
//   - io.ErrUnexpectedEOF wrapped with context if the file is truncated (a
//     missing header, or a path string with no terminating NUL).
//   - A descriptive error if the declared entry count exceeds a sanity cap
//     (defends against corrupted/protected maps; see doodadsdoo for the same
//     guard rationale).
func Parse(data []byte) (*File, error) {
	r := &reader{buf: data}

	f := &File{}
	var err error
	if f.Version, err = r.readU32(); err != nil {
		return nil, fmt.Errorf("read version: %w", err)
	}

	count, err := r.readU32()
	if err != nil {
		return nil, fmt.Errorf("read entry count: %w", err)
	}

	// Sanity-cap to defeat hostile/corrupted maps declaring billions of
	// entries. Real import tables top out in the low thousands; 1M is well
	// above any legitimate map (mirrors doodadsdoo's guard).
	const maxEntries = 1_000_000
	if count > maxEntries {
		return nil, fmt.Errorf("import entry count %d exceeds sanity cap %d (possible corrupted/protected map)", count, maxEntries)
	}

	f.Entries = make([]Entry, 0, count)
	for i := uint32(0); i < count; i++ {
		flag, err := r.readU8()
		if err != nil {
			return nil, fmt.Errorf("entry %d flag: %w", i, err)
		}
		path, err := r.readCString()
		if err != nil {
			return nil, fmt.Errorf("entry %d path: %w", i, err)
		}
		f.Entries = append(f.Entries, Entry{Flag: flag, Path: path})
	}

	return f, nil
}

// --- reader ----------------------------------------------------------------

// reader is a minimal little-endian cursor over a byte slice. Same shape as
// doodadsdoo / unitsdoo so the ports stay structurally aligned.
type reader struct {
	buf []byte
	pos int
}

func (r *reader) readU8() (uint8, error) {
	if r.pos+1 > len(r.buf) {
		return 0, fmt.Errorf("read u8 at offset %d: %w", r.pos, io.ErrUnexpectedEOF)
	}
	v := r.buf[r.pos]
	r.pos++
	return v, nil
}

func (r *reader) readU32() (uint32, error) {
	if r.pos+4 > len(r.buf) {
		return 0, fmt.Errorf("read u32 at offset %d: %w", r.pos, io.ErrUnexpectedEOF)
	}
	v := binary.LittleEndian.Uint32(r.buf[r.pos:])
	r.pos += 4
	return v, nil
}

// readCString reads a NUL-terminated string and consumes the terminator. The
// returned string excludes the NUL. A path with no terminating NUL before
// end-of-buffer is a truncation error.
func (r *reader) readCString() (string, error) {
	start := r.pos
	for r.pos < len(r.buf) {
		if r.buf[r.pos] == 0 {
			s := string(r.buf[start:r.pos])
			r.pos++ // consume NUL
			return s, nil
		}
		r.pos++
	}
	return "", fmt.Errorf("unterminated string starting at offset %d: %w", start, io.ErrUnexpectedEOF)
}
