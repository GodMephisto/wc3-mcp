---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/w3r/w3r.go"
ref: HEAD
---

// Package w3r parses war3map.w3r — the WC3 regions table. Read-only in
// Phase 2b2 (Trigger Editor needs region-name lookup for gg_rct_ resolution
// + the entity picker's region list). Encode is deferred until the Region
// Editor lands.
//
// Wire format: see HiveWE's src/base/regions.ixx. Layout (little-endian):
//
//	4   version           uint32, expected 5
//	4   region_count      uint32
//	[
//	  4 left, 4 bottom, 4 right, 4 top   (float32 game coords)
//	  cstr name
//	  4 creation_number    (int32)
//	  4 weather_id         (FourCC, 4 ASCII bytes)
//	  cstr ambient_id
//	  3 color              (u8 R, u8 G, u8 B)
//	  1 padding
//	] * region_count
//
// We preserve game-space coordinates verbatim — HiveWE normalizes by the
// terrain offset / 128 for in-editor display, but our consumer (the Trigger
// Editor picker + the resolver) doesn't care about display coords. If/when
// the Region Editor is added it can apply its own transform.
package w3r

import (
	"encoding/binary"
	"fmt"
	"io"
	"math"
)

// File holds the parsed regions table.
type File struct {
	Version uint32
	Regions []Region
}

// Region is one named rectangular bounded area. CreationNumber is the trigger-
// addressable id (gg_rct_<Name> uses Name for the global handle, but the
// creation_number is what HiveWE persists internally).
type Region struct {
	Left           float32
	Bottom         float32
	Right          float32
	Top            float32
	Name           string
	CreationNumber int32
	WeatherID      string // 4-byte FourCC, may be all-zero ("0000") when no weather
	AmbientID      string // C-string; may be empty
	Color          [3]uint8

	// pad is the single byte that follows the color triple. HiveWE treats it as
	// padding and discards it, but real files carry a non-zero value here (the
	// wc3-survival fixture stores 0xFF), so Parse preserves it to keep Encode
	// byte-faithful. Hand-constructed Regions leave it 0 (the conventional
	// padding value), which is what a fresh WorldEdit save writes.
	pad byte
}

// Parse decodes raw war3map.w3r bytes. Refuses absurd region counts (CHSV
// protection trick — see HiveWE's regions.ixx cap of 100,000).
func Parse(data []byte) (*File, error) {
	r := &reader{buf: data}
	version, err := r.readU32()
	if err != nil {
		return nil, fmt.Errorf("read version: %w", err)
	}
	count, err := r.readU32()
	if err != nil {
		return nil, fmt.Errorf("read region count: %w", err)
	}
	const maxRegions = 100_000
	if count > maxRegions {
		// Match HiveWE: treat as zero regions rather than erroring. Some
		// "protected" maps deliberately corrupt this field to OOM editors.
		return &File{Version: version}, nil
	}
	out := &File{Version: version, Regions: make([]Region, 0, count)}
	for i := uint32(0); i < count; i++ {
		var rg Region
		if rg.Left, err = r.readF32(); err != nil {
			return nil, fmt.Errorf("region %d left: %w", i, err)
		}
		if rg.Bottom, err = r.readF32(); err != nil {
			return nil, fmt.Errorf("region %d bottom: %w", i, err)
		}
		if rg.Right, err = r.readF32(); err != nil {
			return nil, fmt.Errorf("region %d right: %w", i, err)
		}
		if rg.Top, err = r.readF32(); err != nil {
			return nil, fmt.Errorf("region %d top: %w", i, err)
		}
		if rg.Name, err = r.readCString(); err != nil {
			return nil, fmt.Errorf("region %d name: %w", i, err)
		}
		cn, err := r.readU32()
		if err != nil {
			return nil, fmt.Errorf("region %d creation_number: %w", i, err)
		}
		rg.CreationNumber = int32(cn)
		wid, err := r.readBytes(4)
		if err != nil {
			return nil, fmt.Errorf("region %d weather_id: %w", i, err)
		}
		rg.WeatherID = string(wid)
		if rg.AmbientID, err = r.readCString(); err != nil {
			return nil, fmt.Errorf("region %d ambient_id: %w", i, err)
		}
		col, err := r.readBytes(3)
		if err != nil {
			return nil, fmt.Errorf("region %d color: %w", i, err)
		}
		rg.Color = [3]uint8{col[0], col[1], col[2]}
		// 1-byte padding after the color triple, per HiveWE's reader.advance(1).
		// Preserve it verbatim so Encode is byte-faithful — real files carry a
		// non-zero value here (see Region.pad).
		pad, err := r.readBytes(1)
		if err != nil {
			return nil, fmt.Errorf("region %d padding: %w", i, err)
		}
		rg.pad = pad[0]
		out.Regions = append(out.Regions, rg)
	}
	return out, nil
}

// reader is a tiny binary helper mirroring the style of internal/formats/wtg.
type reader struct {
	buf []byte
	off int
}

func (r *reader) readBytes(n int) ([]byte, error) {
	if r.off+n > len(r.buf) {
		return nil, io.ErrUnexpectedEOF
	}
	out := r.buf[r.off : r.off+n]
	r.off += n
	return out, nil
}

func (r *reader) readU32() (uint32, error) {
	b, err := r.readBytes(4)
	if err != nil {
		return 0, err
	}
	return binary.LittleEndian.Uint32(b), nil
}

func (r *reader) readF32() (float32, error) {
	v, err := r.readU32()
	if err != nil {
		return 0, err
	}
	return math.Float32frombits(v), nil
}

func (r *reader) readCString() (string, error) {
	start := r.off
	for r.off < len(r.buf) {
		if r.buf[r.off] == 0 {
			s := string(r.buf[start:r.off])
			r.off++
			return s, nil
		}
		r.off++
	}
	return "", io.ErrUnexpectedEOF
}
