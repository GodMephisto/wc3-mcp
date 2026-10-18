---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/wct/wct.go"
ref: HEAD
---

// Package wct parses war3map.wct — the custom-script-text companion file to
// war3map.wtg. Each non-comment trigger in the wtg has a corresponding
// CustomText entry here (the JASS / Lua script body for script-mode triggers
// and the global JASS header that prefixes every trigger).
//
// Read-only in Phase 1a; encode lands in 2a alongside structural editing. The
// Session layer is responsible for binding wct entries to wtg triggers (this
// package is purely the binary decoder).
//
// Wire format: see HiveWE src/base/triggers/triggers.ixx L779-823.
package wct

import (
	"bytes"
	"encoding/binary"
	"fmt"
	"io"
	"log"
)

// File is the parsed war3map.wct contents.
//
// The IsPre131 / Version / SubVersion fields are preserved so the encoder
// can round-trip-byte-equal in Phase 2a. CustomTexts is the per-trigger
// script blob in the same order the wtg writes them — IMPORTANT: that order
// is "categories outer, triggers whose ParentID matches inner", NOT trigger
// insertion order (HiveWE triggers.ixx L924-938). The Session is responsible
// for mapping entries to triggers using the wtg category/trigger hierarchy.
//
// In the 1.31+ format, is_comment triggers are SKIPPED in the wct (no entry
// for them). Pre-1.31 includes every trigger.
type File struct {
	IsPre131          bool     `json:"is_pre_131,omitempty"`
	Version           uint32   `json:"version"`
	SubVersion        uint32   `json:"sub_version"`
	GlobalJASSComment string   `json:"global_jass_comment,omitempty"`
	GlobalJASS        string   `json:"global_jass,omitempty"`
	CustomTexts       []string `json:"custom_texts,omitempty"`
	// RawSizes parallels CustomTexts and preserves the on-disk size field
	// for each blob. Encode echoes these verbatim so maps that ship odd
	// trailing-NUL counts (size=2 for empty, size=N+2 for non-empty) still
	// round-trip byte-equal. For session-mutated text the size is
	// auto-derived as len(newText)+1 (one trailing NUL) at Encode time when
	// the slot's RawSize is 0.
	RawSizes []uint32 `json:"-"`
}

// Parse decodes war3map.wct bytes. The output's CustomTexts order is the
// order entries appeared on disk; the caller (Session) maps them to wtg
// triggers using the wtg's category/trigger hierarchy.
//
// 1.31+ format: u32 version=0x80000004, u32 sub_version (0|1). When
// sub_version==1, the global JASS comment + header precede the per-trigger
// blobs. Per-trigger blobs are u32 size + bytes, written ONLY for non-comment
// triggers (1.31+) or for every trigger (pre-1.31).
//
// Unknown sub-versions log a warning and continue (mirrors HiveWE L803-805).
func Parse(data []byte) (*File, error) {
	r := newReader(data)
	first := r.readU32()
	if r.err != nil {
		return nil, fmt.Errorf("wct.Parse: read version: %w", r.err)
	}

	f := &File{Version: first}

	if first == 0x80000004 {
		// 1.31+ format.
		f.SubVersion = r.readU32()
		if r.err != nil {
			return nil, fmt.Errorf("wct.Parse: read sub_version: %w", r.err)
		}
		if f.SubVersion != 0 && f.SubVersion != 1 {
			log.Printf("wct: unknown sub-version %d — trying anyway", f.SubVersion)
		}
		if f.SubVersion == 1 {
			f.GlobalJASSComment = r.readCString()
			size := r.readU32()
			if r.err != nil {
				return nil, fmt.Errorf("wct.Parse: read global_jass header: %w", r.err)
			}
			if size > 0 {
				f.GlobalJASS = string(r.readBytes(int(size)))
				if r.err != nil {
					return nil, fmt.Errorf("wct.Parse: read global_jass body: %w", r.err)
				}
			}
		}
	} else if first == 0 || first == 1 {
		// Pre-1.31 format.
		f.IsPre131 = true
		if first == 1 {
			f.GlobalJASSComment = r.readCString()
			size := r.readU32()
			if r.err != nil {
				return nil, fmt.Errorf("wct.Parse(pre-31): read global_jass header: %w", r.err)
			}
			if size > 0 {
				f.GlobalJASS = string(r.readBytes(int(size)))
				if r.err != nil {
					return nil, fmt.Errorf("wct.Parse(pre-31): read global_jass body: %w", r.err)
				}
			}
		}
		// HiveWE: reader.advance(4) — skip the unknown int between header and
		// the per-trigger blob array.
		r.advance(4)
		if r.err != nil {
			return nil, fmt.Errorf("wct.Parse(pre-31): skip unknown: %w", r.err)
		}
	} else {
		log.Printf("wct: probably invalid format (first u32 = 0x%X)", first)
		// HiveWE prints the warning and keeps reading; do the same so we get
		// SOMETHING for the user to inspect rather than failing the open.
	}

	// Per-trigger blobs — read until EOF. The caller knows how many triggers
	// (by walking the wtg) and pairs them off in order; reading until EOF lets
	// us tolerate trailing/missing entries gracefully.
	//
	// RawSizes preserves the on-disk size field per blob so Encode can echo
	// it verbatim — real maps sometimes ship size=2 (text + two NUL bytes)
	// or size=1 (just a NUL for an empty string), shapes our naive
	// "size=len(text)+1" encoder doesn't reproduce. Parse trims the trailing
	// NULs into a clean Go string; Encode pads back to the original size.
	for r.off < len(r.buf) {
		// Need at least 4 bytes for the size field; if fewer remain, bail
		// quietly — the file may have a trailing padding byte on some
		// World Editor versions.
		if r.off+4 > len(r.buf) {
			break
		}
		size := r.readU32()
		if r.err != nil {
			return nil, fmt.Errorf("wct.Parse: read entry %d size: %w", len(f.CustomTexts), r.err)
		}
		if size == 0 {
			f.CustomTexts = append(f.CustomTexts, "")
			f.RawSizes = append(f.RawSizes, 0)
			continue
		}
		buf := r.readBytes(int(size))
		if r.err != nil {
			return nil, fmt.Errorf("wct.Parse: read entry %d body (size=%d): %w", len(f.CustomTexts), size, r.err)
		}
		// HiveWE reads `size` bytes and treats them as the C string. World
		// Editor terminates them with a NUL so we trim trailing zeros for a
		// clean Go string.
		f.CustomTexts = append(f.CustomTexts, string(bytes.TrimRight(buf, "\x00")))
		f.RawSizes = append(f.RawSizes, size)
	}
	return f, nil
}

// reader is a minimal LE-binary reader, intentionally a duplicate of the wtg
// package's reader so the wct package stays import-cycle-free. Same shape:
// first I/O error sticks in r.err; subsequent reads short-circuit.
type reader struct {
	buf []byte
	off int
	err error
}

func newReader(buf []byte) *reader {
	return &reader{buf: buf}
}

func (r *reader) advance(n int64) {
	if r.err != nil {
		return
	}
	if int64(r.off)+n > int64(len(r.buf)) {
		r.err = io.ErrUnexpectedEOF
		return
	}
	r.off += int(n)
}

func (r *reader) readU32() uint32 {
	if r.err != nil {
		return 0
	}
	if r.off+4 > len(r.buf) {
		r.err = io.ErrUnexpectedEOF
		return 0
	}
	v := binary.LittleEndian.Uint32(r.buf[r.off:])
	r.off += 4
	return v
}

func (r *reader) readBytes(n int) []byte {
	if r.err != nil {
		return nil
	}
	if r.off+n > len(r.buf) {
		r.err = io.ErrUnexpectedEOF
		return nil
	}
	out := r.buf[r.off : r.off+n]
	r.off += n
	return out
}

func (r *reader) readCString() string {
	if r.err != nil {
		return ""
	}
	end := bytes.IndexByte(r.buf[r.off:], 0)
	if end < 0 {
		r.err = io.ErrUnexpectedEOF
		return ""
	}
	s := string(r.buf[r.off : r.off+end])
	r.off += end + 1
	return s
}
