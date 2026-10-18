---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/unitsdoo/encode.go"
ref: HEAD
---

// Encode is the byte-faithful inverse of Parse. It mirrors readEntity
// field-for-field so that Parse → Encode reproduces the original .doo bytes
// exactly for any well-formed input.
//
// Round-trip relies on Entity's unexported preservation fields
// (scaleRaw, skinIDPresent, itemDropSetSizes) — see the Entity struct
// definition for the rationale.

package unitsdoo

import (
	"encoding/binary"
	"fmt"
	"math"
)

// Encode serializes f back to the on-disk war3mapUnits.doo binary format.
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

	// Header: "W3do" magic + version + subversion + entity count.
	if f.Format != [4]byte{} && string(f.Format[:]) != "W3do" {
		return nil, fmt.Errorf("encode: invalid magic %q, want \"W3do\"", string(f.Format[:]))
	}
	w.writeBytes([]byte("W3do"))
	w.writeU32(f.Version)
	w.writeU32(f.SubVersion)
	w.writeU32(uint32(len(f.Entities)))

	// skin_id presence is a FILE-level property, not a per-entity one: WC3's
	// parser (and ours) reads skin_id under a single uniform policy for the whole
	// record stream. A file must therefore be encoded with EVERY entity agreeing
	// on whether it carries a skin_id chunk — otherwise no uniform re-parse can
	// consume the stream and Parse misaligns (the classic symptom is entity 0
	// blowing up on a bogus drop_set[N], because a downstream entity's missing/
	// extra 4 bytes shifts every subsequent read).
	//
	// The danger case: a file that was Parsed (so its entities carry the resolved
	// per-file skinIDPresent decision) and then had a freshly-constructed unit
	// APPENDED (parsed == false). The subversion alone does NOT determine skin_id
	// presence (sub-9 Reforged re-saves carry it; some sub-11 maps omit it), so a
	// hand-constructed entity that fell back to the subversion rule could disagree
	// with the parsed entities around it. We resolve presence once, file-wide,
	// from the parsed entities (the authoritative on-disk evidence) and apply it
	// to every entity below. See fileSkinIDPolicy.
	emitSkin := fileSkinIDPolicy(f)
	for i, e := range f.Entities {
		if err := writeEntity(w, &e, f.SubVersion, emitSkin); err != nil {
			return nil, fmt.Errorf("entity %d: %w", i, err)
		}
	}

	return w.bytes(), nil
}

// fileSkinIDPolicy decides, for the whole file, whether every entity's skin_id
// chunk is emitted. It learns the answer from PARSED entities (those carry the
// authoritative per-file presence decision Parse resolved by trial); a single
// parsed entity with skinIDPresent == true means the on-disk file carried
// skin_id, so all entities — including freshly-appended hand-constructed ones —
// must emit it to keep the stream uniformly parseable.
//
// When the file has NO parsed entities to learn from (a fully hand-constructed
// File), it falls back to the subversion rule (>= 11 emits, < 11 omits), which
// matches the historical CreateUnit-then-save behavior for brand-new maps.
func fileSkinIDPolicy(f *File) bool {
	sawParsed := false
	for i := range f.Entities {
		if f.Entities[i].parsed {
			sawParsed = true
			if f.Entities[i].skinIDPresent {
				return true
			}
		}
	}
	if sawParsed {
		// Every parsed entity agreed skin_id is absent on disk; honor that even
		// at subversion >= 11 (e.g. Green Circle TD).
		return false
	}
	return f.SubVersion >= 11
}

func writeEntity(w *writer, e *Entity, subversion uint32, emitSkinID bool) error {
	// type_id: 4 bytes, exactly.
	if len(e.TypeID) != 4 {
		return fmt.Errorf("type_id %q must be exactly 4 bytes", e.TypeID)
	}
	w.writeBytes([]byte(e.TypeID))

	w.writeU32(e.Variation)

	for i := 0; i < 3; i++ {
		w.writeF32(e.Position[i])
	}
	w.writeF32(e.Rotation)

	// Scale: prefer scaleRaw when Parse populated it. Otherwise derive from
	// Scale * 128 (HiveWE storage convention) — covers hand-constructed
	// entities. A zero scaleRaw means "not set"; this is safe because real
	// .doo entities never carry scale=0 (it would render the unit invisible).
	for i := 0; i < 3; i++ {
		if e.scaleRaw[i] != 0 {
			w.writeF32(e.scaleRaw[i])
		} else {
			w.writeF32(e.Scale[i] * 128.0)
		}
	}

	// skin_id: present whenever the FILE carries it. emitSkinID is the single
	// file-wide decision resolved by fileSkinIDPolicy (see Encode) — every entity
	// in a file MUST agree, because the parser reads skin_id under one uniform
	// policy for the whole record stream. Per-entity disagreement (e.g. a parsed
	// sub-9 Reforged re-save that carries skin_id, with a freshly-appended unit
	// that the old subversion rule would have omitted it for) produces a stream
	// no uniform re-parse can consume, misaligning every entity after the first
	// divergence. Resolving presence file-wide is what keeps the append-a-unit
	// save round-trippable.
	if emitSkinID {
		if len(e.SkinID) != 4 {
			return fmt.Errorf("skin_id %q must be exactly 4 bytes when emitted", e.SkinID)
		}
		w.writeBytes([]byte(e.SkinID))
	}

	w.writeU8(e.Flags)
	w.writeU32(e.Player)
	w.writeU8(e.UnknownByte1)
	w.writeU8(e.UnknownByte2)

	w.writeU32(uint32(e.HitPointsPct))
	w.writeU32(uint32(e.ManaPct))

	// MapItemTable is subversion >= 11 only.
	if subversion >= 11 {
		w.writeU32(uint32(e.MapItemTable))
	}

	// Item drop sets: write per-set counts followed by the items in that set,
	// reconstructed from the flattened ItemDrops + itemDropSetSizes captured
	// by Parse. For hand-constructed entities without itemDropSetSizes, emit
	// a single set containing all drops (when any) — a defensible default.
	sizes := e.itemDropSetSizes
	if len(sizes) == 0 && len(e.ItemDrops) > 0 {
		sizes = []uint32{uint32(len(e.ItemDrops))}
	}
	// Validate sizes sum to len(ItemDrops) to catch misuse before producing
	// a file that won't round-trip.
	var sum uint32
	for _, n := range sizes {
		sum += n
	}
	if sum != uint32(len(e.ItemDrops)) {
		return fmt.Errorf("item_drop_set_sizes sum %d != ItemDrops len %d", sum, len(e.ItemDrops))
	}
	w.writeU32(uint32(len(sizes)))
	idx := 0
	for s, n := range sizes {
		w.writeU32(n)
		for i := uint32(0); i < n; i++ {
			d := e.ItemDrops[idx]
			if len(d.ItemID) != 4 {
				return fmt.Errorf("drop_set[%d].item[%d].id %q must be exactly 4 bytes", s, i, d.ItemID)
			}
			w.writeBytes([]byte(d.ItemID))
			w.writeU32(d.Chance)
			idx++
		}
	}

	w.writeU32(e.GoldAmount)
	w.writeF32(e.TargetAcquisition)
	w.writeU32(e.HeroLevel)
	if subversion >= 11 {
		w.writeU32(e.HeroStr)
		w.writeU32(e.HeroAgi)
		w.writeU32(e.HeroInt)
	}

	// Inventory.
	w.writeU32(uint32(len(e.Inventory)))
	for i, slot := range e.Inventory {
		if len(slot.ItemID) != 4 {
			return fmt.Errorf("inventory[%d].item_id %q must be exactly 4 bytes", i, slot.ItemID)
		}
		w.writeU32(slot.Slot)
		w.writeBytes([]byte(slot.ItemID))
	}

	// Ability modifications.
	w.writeU32(uint32(len(e.AbilityModifications)))
	for i, m := range e.AbilityModifications {
		if len(m.AbilityID) != 4 {
			return fmt.Errorf("ability[%d].id %q must be exactly 4 bytes", i, m.AbilityID)
		}
		w.writeBytes([]byte(m.AbilityID))
		var ac uint32
		if m.Autocast {
			ac = 1
		}
		w.writeU32(ac)
		w.writeU32(m.Level)
	}

	// Random type + variable payload.
	w.writeU32(e.RandomType)
	switch e.RandomType {
	case 0:
		if len(e.RandomData) != 4 {
			return fmt.Errorf("random_data (type=0) must be 4 bytes, got %d", len(e.RandomData))
		}
		w.writeBytes(e.RandomData)
	case 1:
		if len(e.RandomData) != 8 {
			return fmt.Errorf("random_data (type=1) must be 8 bytes, got %d", len(e.RandomData))
		}
		w.writeBytes(e.RandomData)
	case 2:
		// Variable: uint32 count, then count * 8 bytes.
		if len(e.RandomData)%8 != 0 {
			return fmt.Errorf("random_data (type=2) must be multiple of 8 bytes, got %d", len(e.RandomData))
		}
		w.writeU32(uint32(len(e.RandomData) / 8))
		w.writeBytes(e.RandomData)
	default:
		return fmt.Errorf("unknown random_type=%d", e.RandomType)
	}

	w.writeU32(uint32(e.CustomColor))
	w.writeU32(uint32(e.WaygateRegion))
	w.writeU32(e.CreationNumber)
	return nil
}

// --- writer ----------------------------------------------------------------

// writer is a minimal little-endian byte accumulator mirroring reader's surface.
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
