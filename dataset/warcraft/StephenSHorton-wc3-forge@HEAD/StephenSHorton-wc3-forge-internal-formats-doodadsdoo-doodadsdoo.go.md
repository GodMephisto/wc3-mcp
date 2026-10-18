---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/doodadsdoo/doodadsdoo.go"
ref: HEAD
---

// Package doodadsdoo parses the war3map.doo binary format used by Warcraft III
// maps to store placed doodads (decorative + destructible) and the trailing
// "special doodads" cliff/pillar table.
//
// File layout (little-endian throughout):
//
//	char[4]   magic        = "W3do"          (NB: lowercase 'do', case-sensitive)
//	uint32    version      = 7 (RoC) | 8 (TFT/Reforged)
//	uint32    subversion   = 9 | 11   (Reforged commonly 11; older Reforged 9)
//	uint32    doodad_count
//	Doodad[doodad_count]
//	uint32    special_format_version       (often 0)
//	uint32    special_count
//	SpecialDoodad[special_count]
//
// The same .doo container holds BOTH regular placed doodads (trees, bushes,
// destructible gates, random-set placeholders) AND a separate trailing
// "special doodads" array used for cliff/pillar decorations whose positions
// are grid-cell indices rather than world coordinates. Both sections are
// parsed; the special section is OFTEN empty (count=0) — that's normal.
//
// Coordinate convention: doodad positions stay in raw GAME coordinates (floats,
// no scaling). HiveWE internally transforms these via `(file - terrain.offset)/128`
// for its own grid system; we explicitly DO NOT apply that transform — wc3-forge's
// wire format is game coords throughout. (Mirror of unitsdoo behaviour.)
//
// Special doodad positions are stored as a glm::ivec2 — two int32 cell indices —
// and ARE NOT game coords. Document them as grid coordinates so callers don't
// accidentally pipe them into world-space APIs.
//
// Reference: HiveWE src/base/doodads.ixx Doodads::load() (this implementation
// ports it field-for-field) and src/base/doodad.ixx for the struct shapes.
// Subversion-9 skin_id ambiguity is documented in HiveWE docs/protector-tricks.md
// (Trick 2) and is handled identically to unitsdoo.
package doodadsdoo

import (
	"encoding/binary"
	"fmt"
	"io"
	"math"
)

// File is the parsed contents of a war3map.doo file.
type File struct {
	Format        [4]byte // always "W3do" for valid files
	Version       uint32  // 7=RoC, 8=TFT/Reforged
	SubVersion    uint32  // commonly 9 or 11
	Doodads       []Doodad
	SpecialFormat uint32 // version of the trailing special-doodads section (commonly 0)
	SpecialDoodads []SpecialDoodad
}

// Doodad is one placed regular doodad — decorative (tree, bush, rock) or
// destructible (gate, crate, mine entrance). The format does not distinguish
// the two at the wire level; callers classify by looking the TypeID up in
// doodads.slk (decorative) vs destructibles.slk (destructible).
type Doodad struct {
	TypeID    string     // FourCC, exactly 4 ASCII bytes. May be a random-set ID (e.g. "YOtf") rather than a real doodad — preserved verbatim.
	Variation uint32     // model variation index
	Position  [3]float32 // game coords (x, y, z) — pass-through, no scaling
	Rotation  float32    // facing angle as stored in file (radians for sloc-style / engine-placed; possibly degrees for user-placed entries — same documented ambiguity as unitsdoo)
	Scale     [3]float32 // natural scale (1.0 = default). On-disk values are stored multiplied by 128; Parse divides to match runtime convention.

	// SkinID is the Reforged "skin" override FourCC. Present unconditionally
	// at subversion >= 11. At subversion 9 it MAY or MAY NOT be present
	// depending on whether the map was re-saved by the Reforged editor; the
	// parser uses the same 4-byte printable-FourCC peek heuristic that HiveWE
	// uses to decide. Empty string means "absent in file" (consumers should
	// fall back to TypeID).
	SkinID string

	// Flags is the doodad's "state" byte — a bitmask combining visibility +
	// solidity. HiveWE names the bits roughly visible(1) | solid(2) (with bit
	// values composed; e.g. Enfo's FFB observes Flags=6 = visible|solid|??).
	// Real-world observed values cluster in 0..7. The peek-and-validate
	// heuristic below relies only on the format-guarantee that any valid
	// Flags byte stays well below 0x20 (the lowest printable-ASCII byte).
	Flags uint8

	// Life is the destructible HP percentage (0..100). Decorative doodads still
	// have this byte; HiveWE defaults it to 100. There is no documented sentinel
	// (HiveWE doesn't treat 0xFF specially) — callers wanting "destructible-only"
	// semantics should classify by TypeID in destructibles.slk.
	Life uint8

	// MapItemTable is an index into the map-level item-drop table (war3map.w3o).
	// -1 (0xFFFFFFFF on disk) means "no map-table reference". Present only at
	// version >= 8; at version 7 we sentinel to -1.
	MapItemTable int32

	// ItemDrops is the flattened list of per-set drop entries (each set fires
	// once on death). Present only at version >= 8. Set boundaries are not
	// preserved here — see unitsdoo's identical rationale.
	ItemDrops []ItemDrop

	CreationNumber uint32 // stable HiveWE-assigned identity; survives save round-trip

	// --- Round-trip preservation (unexported, set by Parse) ---
	//
	// These capture on-disk shape the public fields can't represent so Encode
	// can reproduce the file byte-for-byte. Mirrors unitsdoo.Entity's
	// preservation fields. Hand-constructed Doodad values can leave them zero;
	// Encode falls back to deriving from the public fields.

	// skinIDPresent records whether the on-disk doodad carried a skin_id chunk.
	// The parser uses a peek heuristic regardless of subversion (see the long
	// comment in readDoodad — X-Hero-Reborn is subversion 11 with NO skin_id),
	// so the absence vs presence decision is per-entity. Encode honors this
	// flag: present → emit SkinID, absent → omit. (For hand-constructed
	// Doodads, Encode falls back to emitting whenever SkinID is non-empty.)
	skinIDPresent bool

	// itemDropSetSizes preserves the per-set entry counts so the encoder can
	// reconstruct the (set_count, [items_in_set_i]*) prefix structure the file
	// uses. Sum equals len(ItemDrops). If left nil/empty when ItemDrops is
	// non-empty, Encode emits a single set containing all drops (a defensible
	// default for hand-constructed doodads). Mirrors the unitsdoo workaround.
	itemDropSetSizes []uint32
}

// SpecialDoodad is one entry in the trailing special-doodads array — used for
// cliff/pillar decorations whose positions are grid-cell indices, NOT world
// coordinates. Don't pipe X/Y into game-coord APIs.
type SpecialDoodad struct {
	TypeID    string // FourCC, exactly 4 ASCII bytes
	Variation uint32 // model variation index (some sources call this field "Z" / "height layer")
	X         int32  // grid cell column (NOT game coord)
	Y         int32  // grid cell row    (NOT game coord)
}

// ItemDrop is one entry within an item-drop set. Each set fires once on death;
// within a set, chances are weighted relative to each other.
type ItemDrop struct {
	ItemID string // FourCC of item to drop
	Chance uint32 // relative weight within the set
}

// Parse decodes a complete war3map.doo file from raw bytes.
//
// Errors:
//   - io.ErrUnexpectedEOF wrapped with context, if the file is truncated.
//   - A descriptive error if the magic is not "W3do".
//   - Unknown version/subversion is tolerated (matches HiveWE); only
//     out-of-range reads will surface as errors.
func Parse(data []byte) (*File, error) {
	r := &reader{buf: data}

	f := &File{}
	magic, err := r.readBytes(4)
	if err != nil {
		return nil, fmt.Errorf("read magic: %w", err)
	}
	copy(f.Format[:], magic)
	if string(magic) != "W3do" {
		return nil, fmt.Errorf("invalid war3map.doo magic: got %q, want \"W3do\"", string(magic))
	}

	if f.Version, err = r.readU32(); err != nil {
		return nil, fmt.Errorf("read version: %w", err)
	}
	if f.SubVersion, err = r.readU32(); err != nil {
		return nil, fmt.Errorf("read subversion: %w", err)
	}

	count, err := r.readU32()
	if err != nil {
		return nil, fmt.Errorf("read doodad count: %w", err)
	}

	// Sanity-cap to defeat hostile/corrupted maps that ask for billions of
	// entries (see HiveWE docs/protector-tricks.md). Real maps top out at a
	// few thousand doodads; 1M is comfortably above any legitimate map.
	const maxDoodads = 1_000_000
	if count > maxDoodads {
		return nil, fmt.Errorf("doodad count %d exceeds sanity cap %d (possible corrupted/protected map)", count, maxDoodads)
	}

	f.Doodads = make([]Doodad, 0, count)
	for i := uint32(0); i < count; i++ {
		d, err := readDoodad(r, f.Version)
		if err != nil {
			return nil, fmt.Errorf("doodad %d: %w", i, err)
		}
		f.Doodads = append(f.Doodads, d)
	}

	// Trailing special-doodads section. The four-byte version is always
	// present even when the array is empty.
	if f.SpecialFormat, err = r.readU32(); err != nil {
		return nil, fmt.Errorf("read special_format: %w", err)
	}
	specialCount, err := r.readU32()
	if err != nil {
		return nil, fmt.Errorf("read special_count: %w", err)
	}
	const maxSpecial = 1_000_000
	if specialCount > maxSpecial {
		return nil, fmt.Errorf("special doodad count %d exceeds sanity cap %d (possible corrupted/protected map)", specialCount, maxSpecial)
	}

	f.SpecialDoodads = make([]SpecialDoodad, 0, specialCount)
	for i := uint32(0); i < specialCount; i++ {
		sd, err := readSpecialDoodad(r)
		if err != nil {
			return nil, fmt.Errorf("special doodad %d: %w", i, err)
		}
		f.SpecialDoodads = append(f.SpecialDoodads, sd)
	}

	return f, nil
}

func readDoodad(r *reader, version uint32) (Doodad, error) {
	var d Doodad
	var err error

	if d.TypeID, err = r.readFourCC(); err != nil {
		return d, fmt.Errorf("type_id: %w", err)
	}
	if d.Variation, err = r.readU32(); err != nil {
		return d, fmt.Errorf("variation: %w", err)
	}

	// Position (vec3 float32). Stored as raw game coords; NO transform applied
	// (HiveWE's load() applies (file - terrain.offset)/128, but that's its
	// internal grid convention — wc3-forge's wire is game coords throughout).
	for i := 0; i < 3; i++ {
		if d.Position[i], err = r.readF32(); err != nil {
			return d, fmt.Errorf("position[%d]: %w", i, err)
		}
	}

	if d.Rotation, err = r.readF32(); err != nil {
		return d, fmt.Errorf("rotation: %w", err)
	}

	// Scale (vec3 float32). NOTE: contrary to units, doodads store the natural
	// scale DIRECTLY on disk — no /128 transform. Verified against HiveWE:
	// units.ixx reads `glm::vec3 / 128.f` (line 140) and writes `scale * 128.f`
	// (line 254); doodads.ixx reads `glm::vec3` raw (line 81) and writes it
	// raw (line 142). The /128 you see in doodad.ixx::update() is a render-
	// time divide combined with `base_scale` from the SLK, not a storage
	// convention. We therefore pass the on-disk value through unmodified;
	// a default-sized doodad reports Scale=[1,1,1].
	for i := 0; i < 3; i++ {
		if d.Scale[i], err = r.readF32(); err != nil {
			return d, fmt.Errorf("scale[%d]: %w", i, err)
		}
	}

	// skin_id — ambiguous Reforged-era field. HiveWE keys this off
	// war3map.w3i's game_version (>= 1.32 → read), NOT the .doo subversion.
	// We don't have access to w3i here, so we rely on a peek-and-validate
	// heuristic that works regardless of subversion.
	//
	// The peek is sound because the byte at the would-be skin_id position is
	// disjoint between the two cases:
	//   - skin_id present  → first byte is a printable FourCC byte (0x20..0x7E);
	//                        WC3 FourCCs are always ASCII letters/digits.
	//   - skin_id absent   → first byte is the `flags` field, a bitmask
	//                        (visible|solid|…); observed values cluster in
	//                        0..7 and the format never produces a Flags byte
	//                        as high as 0x20 (the lowest printable-ASCII byte).
	// A printable first byte is conclusive evidence of skin_id presence.
	//
	// Historically this heuristic was applied only at subversion 9 (the
	// "Reforged re-saved an old map" case). X_Hero_Reborn_1.3.w3x is a
	// real-world counter-example: subversion 11 with NO skin_id (its w3i
	// game_version is pre-1.32). Applying the peek at all subversions
	// handles that variant without losing any subver-11 maps that DO carry
	// skin_id.
	//
	// Ref: HiveWE src/base/doodads.ixx (`game_version_major*100 + minor >= 132`).
	if peek := r.peek(4); len(peek) == 4 && isPrintableFourCC(peek) {
		if d.SkinID, err = r.readFourCC(); err != nil {
			return d, fmt.Errorf("skin_id: %w", err)
		}
		d.skinIDPresent = true
	}

	if d.Flags, err = r.readU8(); err != nil {
		return d, fmt.Errorf("flags: %w", err)
	}
	if d.Life, err = r.readU8(); err != nil {
		return d, fmt.Errorf("life: %w", err)
	}

	// MapItemTable + per-doodad item drop sets are version >= 8 only.
	// At version 7 we sentinel MapItemTable to -1 and leave ItemDrops nil.
	if version >= 8 {
		mt, err := r.readU32()
		if err != nil {
			return d, fmt.Errorf("map_item_table: %w", err)
		}
		d.MapItemTable = int32(mt) // intentional reinterpret; -1 (0xFFFFFFFF) = no table

		setCount, err := r.readU32()
		if err != nil {
			return d, fmt.Errorf("item_drop_set_count: %w", err)
		}
		// Sanity-cap drop-set / per-set item counts. Real maps top out at a
		// handful of sets per doodad and a few items per set. A bogus value
		// here usually means we're misaligned (e.g., consumed a phantom
		// skin_id when none was present) and would otherwise wander far past
		// the buffer before erroring. Cap is generous: any realistic
		// authored data fits.
		const maxDropSets = 256
		const maxItemsPerSet = 256
		if setCount > maxDropSets {
			return d, fmt.Errorf("item_drop_set_count %d exceeds sanity cap %d (likely misalignment)", setCount, maxDropSets)
		}
		// Flatten all sets into a single []ItemDrop for convenient downstream
		// iteration. The per-set boundaries are preserved separately in the
		// unexported itemDropSetSizes so byte-faithful Encode can reconstruct
		// the (set_count, [items_in_set_i]*) prefix structure. Same shape of
		// workaround as unitsdoo.itemDropSetSizes.
		if setCount > 0 {
			d.itemDropSetSizes = make([]uint32, 0, setCount)
		}
		for s := uint32(0); s < setCount; s++ {
			itemCount, err := r.readU32()
			if err != nil {
				return d, fmt.Errorf("drop_set[%d] count: %w", s, err)
			}
			if itemCount > maxItemsPerSet {
				return d, fmt.Errorf("drop_set[%d] item_count %d exceeds sanity cap %d (likely misalignment)", s, itemCount, maxItemsPerSet)
			}
			d.itemDropSetSizes = append(d.itemDropSetSizes, itemCount)
			for it := uint32(0); it < itemCount; it++ {
				var drop ItemDrop
				if drop.ItemID, err = r.readFourCC(); err != nil {
					return d, fmt.Errorf("drop_set[%d].item[%d].id: %w", s, it, err)
				}
				if drop.Chance, err = r.readU32(); err != nil {
					return d, fmt.Errorf("drop_set[%d].item[%d].chance: %w", s, it, err)
				}
				d.ItemDrops = append(d.ItemDrops, drop)
			}
		}
	} else {
		d.MapItemTable = -1
	}

	if d.CreationNumber, err = r.readU32(); err != nil {
		return d, fmt.Errorf("creation_number: %w", err)
	}

	return d, nil
}

func readSpecialDoodad(r *reader) (SpecialDoodad, error) {
	var sd SpecialDoodad
	var err error

	if sd.TypeID, err = r.readFourCC(); err != nil {
		return sd, fmt.Errorf("type_id: %w", err)
	}
	if sd.Variation, err = r.readU32(); err != nil {
		return sd, fmt.Errorf("variation: %w", err)
	}
	// glm::ivec2 — two signed int32 grid-cell indices. Some upstream specs
	// describe these as uint32 but HiveWE reads them as signed; we follow
	// HiveWE since negative cell indices would indicate corruption anyway.
	x, err := r.readU32()
	if err != nil {
		return sd, fmt.Errorf("x: %w", err)
	}
	sd.X = int32(x)
	y, err := r.readU32()
	if err != nil {
		return sd, fmt.Errorf("y: %w", err)
	}
	sd.Y = int32(y)

	return sd, nil
}

// isPrintableFourCC reports whether all 4 bytes are in the printable ASCII
// range 0x20..0x7E. Mirrors HiveWE's `std::all_of(..., c >= 0x20 && c <= 0x7E)`.
func isPrintableFourCC(b []byte) bool {
	if len(b) != 4 {
		return false
	}
	for _, c := range b {
		if c < 0x20 || c > 0x7E {
			return false
		}
	}
	return true
}

// --- reader ----------------------------------------------------------------

// reader is a minimal little-endian cursor over a byte slice with a non-consuming
// peek. Same shape as unitsdoo's reader so the two ports stay structurally aligned.
type reader struct {
	buf []byte
	pos int
}

func (r *reader) readBytes(n int) ([]byte, error) {
	if n < 0 {
		return nil, fmt.Errorf("negative read length %d", n)
	}
	if r.pos+n > len(r.buf) {
		return nil, fmt.Errorf("read %d bytes at offset %d: %w", n, r.pos, io.ErrUnexpectedEOF)
	}
	out := make([]byte, n)
	copy(out, r.buf[r.pos:r.pos+n])
	r.pos += n
	return out, nil
}

func (r *reader) peek(n int) []byte {
	if r.pos+n > len(r.buf) {
		return nil
	}
	return r.buf[r.pos : r.pos+n]
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

func (r *reader) readF32() (float32, error) {
	if r.pos+4 > len(r.buf) {
		return 0, fmt.Errorf("read f32 at offset %d: %w", r.pos, io.ErrUnexpectedEOF)
	}
	bits := binary.LittleEndian.Uint32(r.buf[r.pos:])
	r.pos += 4
	return math.Float32frombits(bits), nil
}

// readFourCC reads exactly 4 bytes and returns them as a string. FourCCs in
// .doo files are stored as raw 4-byte tags (e.g. "LTlt", "B00P") — they are
// NOT null-terminated and must be preserved verbatim (including any non-letter
// bytes that random-set placeholders use).
func (r *reader) readFourCC() (string, error) {
	b, err := r.readBytes(4)
	if err != nil {
		return "", err
	}
	return string(b), nil
}
