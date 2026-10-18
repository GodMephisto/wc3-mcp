---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/doodadsdoo/encode.go"
ref: HEAD
---

// Encode is the byte-faithful inverse of Parse. It mirrors readDoodad +
// readSpecialDoodad field-for-field so that Parse → Encode reproduces the
// original .doo bytes exactly for any well-formed input.
//
// Round-trip relies on Doodad's unexported preservation fields
// (skinIDPresent, itemDropSetSizes) — see the Doodad struct definition for
// the rationale. Doodad.Scale is stored RAW on disk (no /128 transform, the
// OPPOSITE convention from unitsdoo), so no scale-preservation field is
// needed — Encode writes Scale verbatim.

package doodadsdoo

import (
	"encoding/binary"
	"fmt"
	"math"
)

// Encode serializes f back to the on-disk war3map.doo binary format.
// For a file produced by Parse, Encode(Parse(data)) returns bytes equal to
// data. For a hand-constructed File, Encode emits a valid file that will
// round-trip through Parse — though Encode-then-Parse-then-Encode is the
// guaranteed-stable equivalence (the first Parse populates the unexported
// preservation fields).
func Encode(f *File) ([]byte, error) {
	if f == nil {
		return nil, fmt.Errorf("encode: nil file")
	}

	w := &writer{}

	// Header: "W3do" magic + version + subversion + doodad count.
	if f.Format != [4]byte{} && string(f.Format[:]) != "W3do" {
		return nil, fmt.Errorf("encode: invalid magic %q, want \"W3do\"", string(f.Format[:]))
	}
	w.writeBytes([]byte("W3do"))
	w.writeU32(f.Version)
	w.writeU32(f.SubVersion)
	w.writeU32(uint32(len(f.Doodads)))

	for i, d := range f.Doodads {
		if err := writeDoodad(w, &d, f.Version); err != nil {
			return nil, fmt.Errorf("doodad %d: %w", i, err)
		}
	}

	// Trailing special-doodads section. The four-byte format-version is
	// always present even when the array is empty.
	w.writeU32(f.SpecialFormat)
	w.writeU32(uint32(len(f.SpecialDoodads)))
	for i, sd := range f.SpecialDoodads {
		if err := writeSpecialDoodad(w, &sd); err != nil {
			return nil, fmt.Errorf("special doodad %d: %w", i, err)
		}
	}

	return w.bytes(), nil
}

func writeDoodad(w *writer, d *Doodad, version uint32) error {
	// type_id: 4 bytes, exactly. Random-set placeholder FourCCs are preserved
	// verbatim (the parser doesn't validate them, neither do we).
	if len(d.TypeID) != 4 {
		return fmt.Errorf("type_id %q must be exactly 4 bytes", d.TypeID)
	}
	w.writeBytes([]byte(d.TypeID))

	w.writeU32(d.Variation)

	for i := 0; i < 3; i++ {
		w.writeF32(d.Position[i])
	}
	w.writeF32(d.Rotation)

	// Scale: RAW on disk for doodads — NO /128 transform applied at Parse,
	// so we write Scale verbatim. (Opposite convention from unitsdoo, which
	// stores Scale*128 and divides at Parse.) Verified against HiveWE
	// doodads.ixx (writes glm::vec3 raw, no multiply).
	for i := 0; i < 3; i++ {
		w.writeF32(d.Scale[i])
	}

	// skin_id: the parser decides presence via peek heuristic regardless of
	// subversion, so we trust skinIDPresent. For hand-constructed Doodads
	// without the flag set, fall back to "emit when SkinID is non-empty" —
	// that's a defensible default since an empty 4-char string is invalid
	// and would fail the validation below anyway.
	emitSkinID := d.skinIDPresent || (!d.skinIDPresent && d.SkinID != "")
	if emitSkinID {
		if len(d.SkinID) != 4 {
			return fmt.Errorf("skin_id %q must be exactly 4 bytes when emitted", d.SkinID)
		}
		w.writeBytes([]byte(d.SkinID))
	}

	w.writeU8(d.Flags)
	w.writeU8(d.Life)

	// MapItemTable + per-doodad item drop sets are version >= 8 only.
	if version >= 8 {
		// Reinterpret MapItemTable as uint32 to preserve the -1 sentinel
		// (0xFFFFFFFF) on the wire.
		w.writeU32(uint32(d.MapItemTable))

		// Reconstruct the per-set structure from itemDropSetSizes. For
		// hand-constructed doodads with no preserved sizes, emit one set
		// containing all drops (matches the unitsdoo defensible default).
		sizes := d.itemDropSetSizes
		if len(sizes) == 0 && len(d.ItemDrops) > 0 {
			sizes = []uint32{uint32(len(d.ItemDrops))}
		}
		// Validate sizes sum to len(ItemDrops) to catch misuse before
		// producing a file that won't round-trip.
		var sum uint32
		for _, n := range sizes {
			sum += n
		}
		if sum != uint32(len(d.ItemDrops)) {
			return fmt.Errorf("item_drop_set_sizes sum %d != ItemDrops len %d", sum, len(d.ItemDrops))
		}
		w.writeU32(uint32(len(sizes)))
		idx := 0
		for s, n := range sizes {
			w.writeU32(n)
			for i := uint32(0); i < n; i++ {
				drop := d.ItemDrops[idx]
				if len(drop.ItemID) != 4 {
					return fmt.Errorf("drop_set[%d].item[%d].id %q must be exactly 4 bytes", s, i, drop.ItemID)
				}
				w.writeBytes([]byte(drop.ItemID))
				w.writeU32(drop.Chance)
				idx++
			}
		}
	}

	w.writeU32(d.CreationNumber)
	return nil
}

func writeSpecialDoodad(w *writer, sd *SpecialDoodad) error {
	if len(sd.TypeID) != 4 {
		return fmt.Errorf("type_id %q must be exactly 4 bytes", sd.TypeID)
	}
	w.writeBytes([]byte(sd.TypeID))
	w.writeU32(sd.Variation)
	// X / Y are signed cell indices; cast back to uint32 for the wire — the
	// parser reads them as uint32 then reinterprets as int32, so the round
	// trip is bit-preserving.
	w.writeU32(uint32(sd.X))
	w.writeU32(uint32(sd.Y))
	return nil
}

// --- writer ----------------------------------------------------------------

// writer is a minimal little-endian byte accumulator mirroring reader's surface.
// Same shape as unitsdoo's writer so the two ports stay structurally aligned.
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
