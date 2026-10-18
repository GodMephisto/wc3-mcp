---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/unitsdoo/unitsdoo.go"
ref: HEAD
---

// Package unitsdoo parses the war3mapUnits.doo binary format used by Warcraft III
// maps to store placed units and items.
//
// File layout (little-endian throughout):
//
//	char[4]   magic        = "W3do"          (NB: lowercase 'do', case-sensitive)
//	uint32    version      = 7 (RoC) | 8 (TFT) | 9+ (Reforged)
//	uint32    subversion   = 9 | 11   (Reforged commonly 11; older Reforged 9)
//	uint32    entity_count
//	Entity[entity_count]
//
// Each Entity contains BOTH units and items — the file does not split them.
// Consumers must classify by looking the TypeID up in units_slk vs items_slk.
//
// Coordinate convention: positions are stored in raw GAME coordinates (floats,
// no scaling). HiveWE internally transforms these via `(file - terrain.offset)/128`
// for its own grid system; we explicitly DO NOT apply that transform — wc3-forge's
// wire format is game coords throughout.
//
// Reference: HiveWE src/base/units.ixx Units::load(), which this implementation
// ports field-for-field. Subversion-9 skin_id ambiguity is documented in
// HiveWE docs/protector-tricks.md (Trick 2).
package unitsdoo

import (
	"encoding/binary"
	"fmt"
	"io"
	"math"
)

// File is the parsed contents of a war3mapUnits.doo file.
type File struct {
	Format     [4]byte // always "W3do" for valid files
	Version    uint32  // 7=RoC, 8=TFT, 9+=Reforged
	SubVersion uint32  // commonly 9 or 11
	Entities   []Entity
}

// Entity is one placed unit OR item. The .doo format stores both in the same
// stream with the same layout; the TypeID's presence in the unit-table vs
// item-table SLK is what disambiguates at the consumer level.
type Entity struct {
	TypeID    string     // FourCC, exactly 4 ASCII bytes
	Variation uint32     // model variation index
	Position  [3]float32 // game coords (x, y, z) — pass-through, no scaling
	Rotation  float32    // facing angle as stored in file. User-placed units appear to be degrees (270 = south); sloc entries appear to be radians (4.712389 ≈ 3π/2). Caller may need to disambiguate by TypeID.
	Scale     [3]float32 // natural scale (1.0 = default). On-disk values are stored multiplied by 128; Parse divides to match runtime convention.

	// SkinID is the Reforged "skin" override FourCC. It is present unconditionally
	// at subversion >= 11. At subversion 9 it MAY or MAY NOT be present depending
	// on whether the map was re-saved by the Reforged editor. The parser uses a
	// 4-byte FourCC peek heuristic at sub9 to detect it. Empty string means
	// "absent in file" (consumers should fall back to TypeID).
	SkinID string

	Flags uint8  // bitfield; 2 = default visible/selectable
	Player uint32 // owning player slot (0-based; 24 = neutral hostile, etc.)

	// UnknownByte1/2 are two opaque bytes between Player and HitPoints. HiveWE
	// preserves them verbatim on round-trip — naming them "Unknown*" makes it
	// explicit that they're known-unidentified rather than "didn't bother".
	UnknownByte1 uint8
	UnknownByte2 uint8

	HitPointsPct int32 // -1 = use unit default (stored as uint32 in file but signed by convention)
	ManaPct      int32 // -1 = use unit default

	// MapItemTable is an index into the map-level item-drop table (war3map.w3o /
	// item set definitions). -1 (0xFFFFFFFF) means "no map-table reference".
	// Only present at subversion >= 11.
	MapItemTable int32

	ItemDrops []ItemDrop // unit-local drop sets (each set rolls independently on death)

	GoldAmount        uint32  // gold-mine gold contents / gold-coin item amount
	TargetAcquisition float32 // override acquisition range; -1 = unit default

	HeroLevel uint32 // 1+ for heroes
	// HeroStr/Agi/Int are only present at subversion >= 11. At sub9 they remain zero.
	HeroStr uint32
	HeroAgi uint32
	HeroInt uint32

	Inventory            []InventorySlot // hero/unit pre-placed inventory
	AbilityModifications []AbilityMod    // per-ability autocast + level overrides

	// RandomType controls the layout of RandomData that follows:
	//   0 = "any" — RandomData is 4 bytes (level + 3 reserved)
	//   1 = "group" — RandomData is 8 bytes (group id + position-in-group, both uint32)
	//   2 = "custom group" — RandomData is N pairs of (chance: uint32, item_id: uint32),
	//       where N is a count prefix that immediately precedes the variable bytes.
	// We capture RandomData as raw bytes — consumers can re-interpret per type.
	RandomType uint32
	RandomData []byte

	CustomColor    int32  // override player color, -1 = use player slot color
	WaygateRegion  int32  // destination region id for waygates, -1 = none
	CreationNumber uint32 // stable HiveWE-assigned identity; survives save round-trip

	// --- Round-trip preservation (unexported, set by Parse) ---
	//
	// These fields capture on-disk shape that the public fields cannot represent,
	// so Encode can reproduce the original file byte-for-byte. None of them
	// participate in JSON (unexported) and consumers that hand-construct Entity
	// values can leave them zero — Encode falls back to deriving them from the
	// public fields.

	// scaleRaw is the unscaled scale as it appears in the file. Parse divides by
	// 128 into Scale, but that mapping is not strictly bidirectional on disk
	// (some entities — slocs — store raw 1.0, others store 128.0). Encode
	// prefers scaleRaw when non-zero, otherwise emits Scale * 128.
	scaleRaw [3]float32
	// skinIDPresent records whether the on-disk entity carried a skin_id chunk.
	// skin_id presence is NOT reliably keyed on the .doo subversion: most
	// subversion>=11 maps carry it, but some (e.g. Green Circle TD) are
	// subversion 11 yet omit it on every entity, and some subversion-9 maps
	// (Reforged re-saves) carry it. Parse therefore decides presence per FILE
	// by trial — see chooseSkinIDPresence — and records the decision here so
	// Encode reproduces the original layout byte-for-byte.
	//
	// For PARSED entities (parsed == true), Encode emits a skin_id chunk iff
	// this flag is set. For hand-constructed entities (parsed == false), this
	// flag is ignored and Encode falls back to the subversion rule
	// (>= 11 emits, < 11 omits) — see writeEntity.
	skinIDPresent bool
	// parsed marks entities produced by Parse (as opposed to hand-constructed
	// via CreateUnit etc.). It lets Encode trust the per-entity skinIDPresent
	// decision for round-tripped data while preserving the subversion-driven
	// default for freshly-constructed units. Snapshotted/restored verbatim by
	// the session delete/undo path along with the other preservation fields.
	parsed bool
	// itemDropSetSizes preserves the per-set entry counts so the encoder can
	// reconstruct the (set_count, [items_in_set_i]*) prefix structure the file
	// uses. Sum equals len(ItemDrops). If left nil/empty when ItemDrops is
	// non-empty, Encode emits a single set containing all drops (a defensible
	// default for hand-constructed entities).
	itemDropSetSizes []uint32
}

// ItemDrop is one entry within an item-drop set. Each set fires once on death;
// within a set, chances are weighted relative to each other.
type ItemDrop struct {
	ItemID string // FourCC of item to drop
	Chance uint32 // relative weight within the set
}

// InventorySlot is one pre-placed item in a unit's inventory. Slot is 0-5
// for the six standard inventory slots.
type InventorySlot struct {
	Slot   uint32
	ItemID string
}

// AbilityMod is a per-ability override applied on top of the unit's base abilities.
type AbilityMod struct {
	AbilityID string
	Autocast  bool // true = ability starts with autocast on
	Level     uint32
}

// Parse decodes a complete war3mapUnits.doo file from raw bytes.
//
// Errors:
//   - io.ErrUnexpectedEOF wrapped with context, if the file is truncated.
//   - A descriptive error if the magic is not "W3do".
//   - Unknown version/subversion is tolerated (matches HiveWE): we log into
//     the returned error message via a wrap, but still attempt the parse.
//     (Actually: we proceed; only out-of-range reads will surface as errors.)
func Parse(data []byte) (*File, error) {
	r := &reader{buf: data}

	f := &File{}
	magic, err := r.readBytes(4)
	if err != nil {
		return nil, fmt.Errorf("read magic: %w", err)
	}
	copy(f.Format[:], magic)
	if string(magic) != "W3do" {
		return nil, fmt.Errorf("invalid war3mapUnits.doo magic: got %q, want \"W3do\"", string(magic))
	}

	if f.Version, err = r.readU32(); err != nil {
		return nil, fmt.Errorf("read version: %w", err)
	}
	if f.SubVersion, err = r.readU32(); err != nil {
		return nil, fmt.Errorf("read subversion: %w", err)
	}

	count, err := r.readU32()
	if err != nil {
		return nil, fmt.Errorf("read entity count: %w", err)
	}

	// Sanity-cap entity count to avoid hostile "protected map" allocations.
	// WC3 caps placed units far below this; HiveWE uses a similar guard for
	// regions (see protector-tricks.md Trick 1). 1M is comfortably above
	// any legitimate map.
	const maxEntities = 1_000_000
	if count > maxEntities {
		return nil, fmt.Errorf("entity count %d exceeds sanity cap %d (possible corrupted/protected map)", count, maxEntities)
	}

	// Decide whether on-disk entities carry a skin_id chunk. This is NOT a
	// reliable function of the subversion (see Entity.skinIDPresent doc), so we
	// resolve it per-file by trial: parse all entities under each candidate
	// policy and keep the interpretation that consumes the record stream
	// exactly and yields only valid random_types. The body of the loop is the
	// same parse for every policy; only the skin_id read differs.
	entityStart := r.pos
	for _, withSkin := range chooseSkinIDPresence(f.SubVersion) {
		r.pos = entityStart
		entities, perr := parseEntities(r, f.SubVersion, count, withSkin)
		if perr != nil {
			continue
		}
		// A correct interpretation consumes the record stream exactly: every
		// entity parsed, no trailing bytes left over. Trailing bytes mean we
		// under-read (wrong, smaller interpretation); a parse error means we
		// over-read (wrong, larger interpretation). Either way, try the next
		// candidate before giving up.
		if r.pos != len(r.buf) {
			continue
		}
		f.Entities = entities
		return f, nil
	}

	// No candidate produced a clean full-file parse. Re-run the
	// subversion-implied default once more so the caller gets the most
	// informative error (matching prior behavior for genuinely malformed
	// files), rather than a generic "no interpretation worked".
	r.pos = entityStart
	def := f.SubVersion >= 11
	if entities, perr := parseEntities(r, f.SubVersion, count, def); perr != nil {
		return nil, perr
	} else {
		f.Entities = entities
		return f, nil
	}
}

// chooseSkinIDPresence returns the ordered list of skin_id presence policies to
// try for a file at the given subversion, most-likely first.
//
//   - subversion >= 11: the format normally carries skin_id, so try present
//     first, then absent (covers maps like Green Circle TD that omit it).
//   - subversion < 11: legacy maps normally omit skin_id, but Reforged re-saves
//     may insert it. Try absent first (the historical default), then present.
//     This subsumes the old per-entity peek heuristic: instead of guessing per
//     entity, we validate each whole-file interpretation and keep the one that
//     consumes the record stream exactly.
//
// Trying both and validating full-file consumption is what makes the parser
// robust to the documented subversion/skin_id ambiguity (X_Hero_Reborn_1.3 and
// Green Circle TD are the two counter-examples on record).
func chooseSkinIDPresence(subversion uint32) []bool {
	if subversion >= 11 {
		return []bool{true, false}
	}
	return []bool{false, true}
}

// parseEntities reads exactly count entities from r using the given skin_id
// presence policy. withSkin == true reads a 4-byte skin_id chunk on every
// entity; withSkin == false reads none. The policy is uniform across the file —
// the per-entity peek heuristic the parser used to apply at subversion 9 is
// replaced by Parse trying both policies and validating full-file consumption,
// which is strictly more robust (it can't drift mid-file). parseEntities
// returns the parsed entities or the first error encountered; the caller
// decides which policy "wins" by checking that r consumed the whole buffer.
func parseEntities(r *reader, subversion uint32, count uint32, withSkin bool) ([]Entity, error) {
	// Bound the entity count against the bytes actually remaining before
	// preallocating. Even though Parse already caps count at maxEntities (1M),
	// a small truncated/fuzzed file declaring a near-cap count would still make
	// us preallocate ~272 MB for a few hundred bytes of input. A real entity is
	// far larger than 4 bytes on disk, so remaining()/4 is a safe upper bound.
	if err := r.checkCount(count, 4, "entity"); err != nil {
		return nil, err
	}
	entities := make([]Entity, 0, count)
	for i := uint32(0); i < count; i++ {
		e, err := readEntity(r, subversion, withSkin)
		if err != nil {
			return nil, fmt.Errorf("entity %d: %w", i, err)
		}
		entities = append(entities, e)
	}
	return entities, nil
}

// readEntity reads one entity from r. readSkinID is the per-FILE skin_id
// presence decision made by Parse (via chooseSkinIDPresence + full-file
// validation); when true the entity carries a 4-byte skin_id chunk after the
// scale vector, when false it does not. The decision is uniform across a file —
// Parse picks whichever policy makes every entity parse and consumes the record
// stream exactly.
func readEntity(r *reader, subversion uint32, readSkinID bool) (Entity, error) {
	var e Entity
	var err error
	e.parsed = true

	if e.TypeID, err = r.readFourCC(); err != nil {
		return e, fmt.Errorf("type_id: %w", err)
	}
	if e.Variation, err = r.readU32(); err != nil {
		return e, fmt.Errorf("variation: %w", err)
	}

	// Position (vec3 float32). Stored as raw game coords; NO transform applied
	// (HiveWE's load() applies (file - terrain.offset)/128, but that's its
	// internal grid convention — wc3-forge's wire is game coords throughout).
	for i := 0; i < 3; i++ {
		if e.Position[i], err = r.readF32(); err != nil {
			return e, fmt.Errorf("position[%d]: %w", i, err)
		}
	}

	if e.Rotation, err = r.readF32(); err != nil {
		return e, fmt.Errorf("rotation: %w", err)
	}

	// Scale (vec3 float32). The on-disk convention is split:
	//   - Normal units (heroes, buildings, creeps): natural scale × 128
	//     (raw=128.0 → natural=1.0). Storage matches HiveWE's load() which
	//     uniformly does `/ 128.f` and gets 1.0 for the common case.
	//   - Items + slocs: natural scale × 1 (raw=1.0 → natural=1.0).
	//     HiveWE happens to work because its render path for items pulls
	//     scale from ItemData.slk and ignores i.scale entirely
	//     (HiveWE/src/base/units.ixx:78-94). wc3-forge's render path
	//     multiplies the per-instance scale into the final render scale
	//     (so the gizmo's per-axis scale handles can edit it), which means
	//     uniformly dividing by 128 would render items at 1/128 of their
	//     intended size — invisible. Heuristic: raw < 2.0 is already a
	//     natural scale (items/slocs); raw >= 2.0 is 128-encoded (units).
	//     Real authored per-instance scales fall well outside the [0, 2)
	//     band on either convention.
	//
	// scaleRaw captures the on-disk value verbatim so byte-faithful Encode
	// can preserve the original convention without inferring it.
	for i := 0; i < 3; i++ {
		raw, err := r.readF32()
		if err != nil {
			return e, fmt.Errorf("scale[%d]: %w", i, err)
		}
		e.scaleRaw[i] = raw
		if raw < 2.0 {
			e.Scale[i] = raw
		} else {
			e.Scale[i] = raw / 128.0
		}
	}

	// skin_id — the ambiguous Reforged field. Whether it is present is a
	// per-file property decided by Parse (chooseSkinIDPresence + full-file
	// validation), NOT a function of the subversion: subversion-11 maps usually
	// carry it but some (Green Circle TD) omit it, and some subversion-9 maps
	// (Reforged re-saves) carry it. readSkinID is that resolved decision.
	if readSkinID {
		if e.SkinID, err = r.readFourCC(); err != nil {
			return e, fmt.Errorf("skin_id: %w", err)
		}
		e.skinIDPresent = true
	}

	if e.Flags, err = r.readU8(); err != nil {
		return e, fmt.Errorf("flags: %w", err)
	}
	if e.Player, err = r.readU32(); err != nil {
		return e, fmt.Errorf("player: %w", err)
	}
	// HiveWE has a `if (player > 11 && editor_version < 6060) player += 12;`
	// quirk to renumber older-editor player slots. We don't have editor_version
	// here (it lives in war3map.w3i) and modern maps don't need it — skip.
	// Consumers that care can apply the same fixup post-parse with w3i context.

	if e.UnknownByte1, err = r.readU8(); err != nil {
		return e, fmt.Errorf("unknown_byte1: %w", err)
	}
	if e.UnknownByte2, err = r.readU8(); err != nil {
		return e, fmt.Errorf("unknown_byte2: %w", err)
	}

	if hp, err := r.readU32(); err != nil {
		return e, fmt.Errorf("hit_points_pct: %w", err)
	} else {
		e.HitPointsPct = int32(hp) // intentional reinterpret; -1 (0xFFFFFFFF) = default
	}
	if mp, err := r.readU32(); err != nil {
		return e, fmt.Errorf("mana_pct: %w", err)
	} else {
		e.ManaPct = int32(mp)
	}

	// MapItemTable is subversion >= 11 only. At sub9 we sentinel it to -1.
	if subversion >= 11 {
		mt, err := r.readU32()
		if err != nil {
			return e, fmt.Errorf("map_item_table: %w", err)
		}
		e.MapItemTable = int32(mt)
	} else {
		e.MapItemTable = -1
	}

	// Item drop sets — unit-local drop tables. Count, then per-set count + entries.
	setCount, err := r.readU32()
	if err != nil {
		return e, fmt.Errorf("item_drop_set_count: %w", err)
	}
	// Bound the untrusted count before allocating: each set is at least its own
	// 4-byte item-count prefix, so a file can't hold more sets than it has
	// bytes/4. This rejects a garbage/misaligned setCount (billions) instead of
	// allocating gigabytes for the itemDropSetSizes slice.
	if err := r.checkCount(setCount, 4, "item_drop_set"); err != nil {
		return e, fmt.Errorf("item_drop_set_count: %w", err)
	}
	// Flatten all sets into a single []ItemDrop, but capture per-set sizes in
	// itemDropSetSizes so byte-faithful Encode can reconstruct the
	// (set_count, [items_in_set_i]*) prefix structure.
	if setCount > 0 {
		e.itemDropSetSizes = make([]uint32, 0, setCount)
	}
	for s := uint32(0); s < setCount; s++ {
		itemCount, err := r.readU32()
		if err != nil {
			return e, fmt.Errorf("drop_set[%d] count: %w", s, err)
		}
		// Each drop entry is 8 bytes (4-byte item id + 4-byte chance). Bound
		// itemCount before the inner read loop so a hostile value fails fast.
		if err := r.checkCount(itemCount, 8, "drop_set item"); err != nil {
			return e, fmt.Errorf("drop_set[%d] count: %w", s, err)
		}
		e.itemDropSetSizes = append(e.itemDropSetSizes, itemCount)
		for it := uint32(0); it < itemCount; it++ {
			var drop ItemDrop
			if drop.ItemID, err = r.readFourCC(); err != nil {
				return e, fmt.Errorf("drop_set[%d].item[%d].id: %w", s, it, err)
			}
			if drop.Chance, err = r.readU32(); err != nil {
				return e, fmt.Errorf("drop_set[%d].item[%d].chance: %w", s, it, err)
			}
			e.ItemDrops = append(e.ItemDrops, drop)
		}
	}

	if e.GoldAmount, err = r.readU32(); err != nil {
		return e, fmt.Errorf("gold_amount: %w", err)
	}
	if e.TargetAcquisition, err = r.readF32(); err != nil {
		return e, fmt.Errorf("target_acquisition: %w", err)
	}
	if e.HeroLevel, err = r.readU32(); err != nil {
		return e, fmt.Errorf("hero_level: %w", err)
	}

	// Hero stats are subversion >= 11 only.
	if subversion >= 11 {
		if e.HeroStr, err = r.readU32(); err != nil {
			return e, fmt.Errorf("hero_str: %w", err)
		}
		if e.HeroAgi, err = r.readU32(); err != nil {
			return e, fmt.Errorf("hero_agi: %w", err)
		}
		if e.HeroInt, err = r.readU32(); err != nil {
			return e, fmt.Errorf("hero_int: %w", err)
		}
	}

	// Inventory items.
	invCount, err := r.readU32()
	if err != nil {
		return e, fmt.Errorf("inventory_count: %w", err)
	}
	// Each inventory slot is 8 bytes (4-byte slot index + 4-byte item id).
	// Bound before allocating to reject a garbage count.
	if err := r.checkCount(invCount, 8, "inventory"); err != nil {
		return e, fmt.Errorf("inventory_count: %w", err)
	}
	e.Inventory = make([]InventorySlot, 0, invCount)
	for i := uint32(0); i < invCount; i++ {
		var slot InventorySlot
		if slot.Slot, err = r.readU32(); err != nil {
			return e, fmt.Errorf("inventory[%d].slot: %w", i, err)
		}
		if slot.ItemID, err = r.readFourCC(); err != nil {
			return e, fmt.Errorf("inventory[%d].item_id: %w", i, err)
		}
		e.Inventory = append(e.Inventory, slot)
	}

	// Ability modifications.
	abilCount, err := r.readU32()
	if err != nil {
		return e, fmt.Errorf("ability_count: %w", err)
	}
	// Each ability modification is 12 bytes (4-byte id + 4-byte autocast +
	// 4-byte level). Bound before allocating to reject a garbage count.
	if err := r.checkCount(abilCount, 12, "ability"); err != nil {
		return e, fmt.Errorf("ability_count: %w", err)
	}
	e.AbilityModifications = make([]AbilityMod, 0, abilCount)
	for i := uint32(0); i < abilCount; i++ {
		var mod AbilityMod
		if mod.AbilityID, err = r.readFourCC(); err != nil {
			return e, fmt.Errorf("ability[%d].id: %w", i, err)
		}
		autocast, err := r.readU32()
		if err != nil {
			return e, fmt.Errorf("ability[%d].autocast: %w", i, err)
		}
		mod.Autocast = autocast != 0
		if mod.Level, err = r.readU32(); err != nil {
			return e, fmt.Errorf("ability[%d].level: %w", i, err)
		}
		e.AbilityModifications = append(e.AbilityModifications, mod)
	}

	// Random type + variable-length data block.
	if e.RandomType, err = r.readU32(); err != nil {
		return e, fmt.Errorf("random_type: %w", err)
	}
	switch e.RandomType {
	case 0:
		// Any-unit-of-level — 4 bytes (level byte + 3 reserved/unused).
		if e.RandomData, err = r.readBytes(4); err != nil {
			return e, fmt.Errorf("random_data (type=0): %w", err)
		}
	case 1:
		// Random unit from group — 8 bytes (group_id uint32 + position uint32).
		if e.RandomData, err = r.readBytes(8); err != nil {
			return e, fmt.Errorf("random_data (type=1): %w", err)
		}
	case 2:
		// Custom group — uint32 count, then count * 8 bytes (chance + id pairs).
		n, err := r.readU32()
		if err != nil {
			return e, fmt.Errorf("random_data (type=2) count: %w", err)
		}
		// Each pair is 8 bytes (chance uint32 + item_id uint32). Bound n before
		// computing int(n)*8 — that multiply overflows on 32-bit platforms and
		// produces a bogus (possibly negative) length, and even on 64-bit a
		// hostile n would make readBytes attempt a huge allocation.
		if err := r.checkCount(n, 8, "random_data (type=2)"); err != nil {
			return e, fmt.Errorf("random_data (type=2) count: %w", err)
		}
		if e.RandomData, err = r.readBytes(int(n) * 8); err != nil {
			return e, fmt.Errorf("random_data (type=2) body: %w", err)
		}
	default:
		// Unknown random_type — best-effort: no payload. HiveWE doesn't handle
		// this case either; downstream reads would misalign. We surface an error
		// rather than silently corrupt subsequent entities.
		return e, fmt.Errorf("unknown random_type=%d", e.RandomType)
	}

	if cc, err := r.readU32(); err != nil {
		return e, fmt.Errorf("custom_color: %w", err)
	} else {
		e.CustomColor = int32(cc)
	}
	if wg, err := r.readU32(); err != nil {
		return e, fmt.Errorf("waygate: %w", err)
	} else {
		e.WaygateRegion = int32(wg)
	}
	if e.CreationNumber, err = r.readU32(); err != nil {
		return e, fmt.Errorf("creation_number: %w", err)
	}

	return e, nil
}

// ClearScaleRaw zeroes the unexported scaleRaw preservation field on e so that
// the next Encode call derives on-disk scale from e.Scale*128 rather than
// preserving the original raw bytes. Call this whenever the public Scale field
// is changed by a mutator (e.g. Session.ScaleUnit) — otherwise Encode silently
// keeps the OLD on-disk bits because it prefers scaleRaw when non-zero.
//
// This accessor is needed because scaleRaw is unexported (its only purpose is
// byte-faithful round-tripping of the original file; callers that hand-edit
// Scale must opt out of that preservation). Doodads do NOT have this issue —
// their Scale is stored raw on disk with no /128 divide at Parse.
func ClearScaleRaw(e *Entity) {
	e.scaleRaw = [3]float32{}
}

// ScaleRaw reads the unexported scaleRaw field. Used by undo/redo machinery
// to snapshot the pre-mutation raw bytes so a later Revert can restore both
// the public Scale field AND the on-disk byte preservation.
func ScaleRaw(e *Entity) [3]float32 {
	return e.scaleRaw
}

// SetScaleRaw writes the unexported scaleRaw field. Counterpart to ScaleRaw —
// used by undo/redo Revert to restore byte-faithful round-tripping after the
// public Scale has been rolled back. Zero values are valid (slocs originally
// store raw=1.0 → public 0.0078125, real units raw=128.0 → public 1.0; undo
// re-establishes whichever of those was in place pre-mutation).
func SetScaleRaw(e *Entity, raw [3]float32) {
	e.scaleRaw = raw
}

// SetItemDrops replaces the entity's drop list AND reconstructs the unexported
// itemDropSetSizes so Encode stays consistent. The original file groups drops
// into N independently-rolled sets; Parse flattens them into a single
// []ItemDrop and stashes the per-set counts in itemDropSetSizes for
// byte-faithful round-tripping. When a caller HAND-EDITS the drop list there is
// no way to know the user's intended set boundaries, so we collapse the new
// list into a single set (sizes = [len]) — the same convention the trigger
// codegen already uses for flattened drops (see triggers_codegen.go). An empty
// list clears both fields so Encode emits zero sets.
//
// This accessor is needed because itemDropSetSizes is unexported; without
// resetting it Encode's sum-check (sizes must total len(ItemDrops)) would fail
// after any drop edit that changes the entry count.
func SetItemDrops(e *Entity, drops []ItemDrop) {
	e.ItemDrops = drops
	if len(drops) == 0 {
		e.itemDropSetSizes = nil
		return
	}
	e.itemDropSetSizes = []uint32{uint32(len(drops))}
}

// --- reader ----------------------------------------------------------------

// reader is a minimal little-endian cursor over a byte slice. Mirrors the
// surface of HiveWE's BinaryReader so the port reads 1:1.
type reader struct {
	buf []byte
	pos int
}

// remaining reports how many unread bytes are left in the buffer.
func (r *reader) remaining() int {
	if r.pos >= len(r.buf) {
		return 0
	}
	return len(r.buf) - r.pos
}

// checkCount validates an untrusted element count read from the byte stream
// before it is used to size an allocation. A count is implausible if the bytes
// it would consume (count * minElemBytes) exceed the bytes remaining in the
// buffer — no legitimate file can describe more elements than it has bytes for.
// Rejecting up front turns a malformed/truncated/fuzzed length field into a
// fast decode error instead of an attempted multi-gigabyte allocation (the
// war3mapUnits.doo OOM-on-malformed-input class of bug). minElemBytes must be
// the smallest on-disk footprint of one element (>= 1).
//
// what/index are used only to build a descriptive, wrapped error matching the
// package's existing error style.
func (r *reader) checkCount(count uint32, minElemBytes int, what string) error {
	if minElemBytes < 1 {
		minElemBytes = 1
	}
	// remaining()/minElemBytes is an upper bound on how many elements could
	// possibly follow; anything beyond it is corrupt. Using division (rather
	// than count*minElemBytes) avoids any multiply overflow on a hostile count.
	if uint64(count) > uint64(r.remaining()/minElemBytes) {
		return fmt.Errorf("%s count %d exceeds %d bytes remaining (min %d bytes/element) at offset %d: %w",
			what, count, r.remaining(), minElemBytes, r.pos, io.ErrUnexpectedEOF)
	}
	return nil
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
// .doo files are stored as raw 4-byte tags (e.g. "sloc", "hpea") — they are NOT
// null-terminated. This is distinct from BinaryReader::read_string(size) in
// HiveWE which truncates at a NUL — we don't, because FourCCs may legitimately
// contain printable chars only and truncation would silently corrupt them.
func (r *reader) readFourCC() (string, error) {
	b, err := r.readBytes(4)
	if err != nil {
		return "", err
	}
	return string(b), nil
}

