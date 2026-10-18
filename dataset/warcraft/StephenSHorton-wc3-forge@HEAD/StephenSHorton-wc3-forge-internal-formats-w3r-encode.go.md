---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/w3r/encode.go"
ref: HEAD
---

// Encode is the byte-faithful inverse of Parse. It mirrors the Parse field
// order exactly so that Parse → Encode reproduces the original war3map.w3r
// bytes for any well-formed input — the contract the Region Editor's first
// save depends on, so unmodified regions don't get silently dropped or
// corrupted on write-back.
//
// Wire format mirrors the package doc comment in w3r.go (and HiveWE's
// regions.ixx). The per-region trailing padding byte is round-tripped via the
// unexported Region.pad field; see that field's doc for why it can't be
// assumed zero.

package w3r

import (
	"encoding/binary"
	"fmt"
	"math"
	"strings"
)

// Encode serializes f back to the on-disk war3map.w3r binary format. For a
// file produced by Parse, Encode(Parse(data)) returns bytes equal to data. For
// a hand-constructed File, Encode emits a valid file that round-trips through
// Parse (the per-region pad byte defaults to 0, matching a fresh WorldEdit
// save).
func Encode(f *File) ([]byte, error) {
	if f == nil {
		return nil, fmt.Errorf("encode: nil file")
	}

	w := &writer{}
	w.writeU32(f.Version)
	w.writeU32(uint32(len(f.Regions)))

	for i := range f.Regions {
		if err := writeRegion(w, &f.Regions[i]); err != nil {
			return nil, fmt.Errorf("region %d: %w", i, err)
		}
	}

	return w.bytes(), nil
}

func writeRegion(w *writer, rg *Region) error {
	w.writeF32(rg.Left)
	w.writeF32(rg.Bottom)
	w.writeF32(rg.Right)
	w.writeF32(rg.Top)

	// C-string fields cannot contain an embedded NUL — that would change where
	// a re-parse finds the terminator and silently truncate the field.
	if strings.IndexByte(rg.Name, 0) >= 0 {
		return fmt.Errorf("name %q contains embedded NUL", rg.Name)
	}
	w.writeCString(rg.Name)

	w.writeU32(uint32(rg.CreationNumber))

	// weather_id is a fixed-width 4-byte FourCC, NOT a C-string. WC3 stores
	// all-zero bytes for "no weather"; Parse reads exactly 4 bytes, so Encode
	// must emit exactly 4. Empty is normalized to 4 NULs for hand-constructed
	// regions that left WeatherID unset.
	switch len(rg.WeatherID) {
	case 0:
		w.writeBytes([]byte{0, 0, 0, 0})
	case 4:
		w.writeBytes([]byte(rg.WeatherID))
	default:
		return fmt.Errorf("weather_id %q must be exactly 4 bytes (or empty)", rg.WeatherID)
	}

	if strings.IndexByte(rg.AmbientID, 0) >= 0 {
		return fmt.Errorf("ambient_id %q contains embedded NUL", rg.AmbientID)
	}
	w.writeCString(rg.AmbientID)

	w.writeBytes(rg.Color[:])
	// Trailing padding byte (see Region.pad).
	w.writeU8(rg.pad)
	return nil
}

// --- writer ----------------------------------------------------------------

// writer is a minimal little-endian byte accumulator mirroring reader's surface
// (and the unitsdoo encoder's writer).
type writer struct {
	buf []byte
}

func (w *writer) bytes() []byte { return w.buf }

func (w *writer) writeBytes(b []byte) { w.buf = append(w.buf, b...) }

func (w *writer) writeU8(v uint8) { w.buf = append(w.buf, v) }

func (w *writer) writeU32(v uint32) {
	var b [4]byte
	binary.LittleEndian.PutUint32(b[:], v)
	w.buf = append(w.buf, b[:]...)
}

func (w *writer) writeF32(v float32) {
	w.writeU32(math.Float32bits(v))
}

func (w *writer) writeCString(s string) {
	w.buf = append(w.buf, s...)
	w.buf = append(w.buf, 0)
}
