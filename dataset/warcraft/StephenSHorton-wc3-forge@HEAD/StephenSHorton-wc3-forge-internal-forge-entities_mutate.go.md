---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/entities_mutate.go"
ref: HEAD
---

package forge

import (
	"fmt"

	"github.com/StephenSHorton/wc3-forge/internal/formats/doodadsdoo"
	"github.com/StephenSHorton/wc3-forge/internal/formats/unitsdoo"
)

// entities_mutate.go holds the placed-entity create/delete mutators (units +
// doodads) plus their undo Command types. Kept in its own file so it doesn't
// collide with the parallel agents editing session.go's body. All mutators
// here follow the established pattern: capture pre-state under the lock, mutate
// in place, recordCommand for undo, then fire dirty + entity-changed events
// after releasing the lock.
//
// CONTRACT (wire methods, handled in handlers_entities.go):
//   units.create    {type_id, player, x, y, z?, rotation?, scale?} -> {creation_number}
//   units.delete    {creation_number} -> {ok}
//   doodads.create  {type_id, x, y, z?, rotation?, scale?, variation?} -> {creation_number}
//   doodads.delete  {creation_number} -> {ok}
//
// Round-trip invariants honored:
//   - New units set RandomType=0 with a 4-byte RandomData block (unitsdoo.Encode
//     requires exactly 4 bytes for type-0), Scale defaults to [1,1,1] with
//     scaleRaw left zero so Encode emits Scale*128 (the unit convention).
//   - New doodads store Scale verbatim (no /128 — the doodad convention).
//   - Delete preserves the FULL removed record (including unexported
//     preservation fields scaleRaw/skinIDPresent/itemDropSetSizes) so Undo
//     re-inserts byte-identical state. We snapshot the value AND its original
//     slice index so Undo restores ordering too.

// ---------------------------------------------------------------------------
// Unit create / delete
// ---------------------------------------------------------------------------

// CreateUnit appends a new placed unit to war3mapUnits.doo at the given game
// coordinates. A fresh creation_number (max existing + 1, min 1) is allocated.
// Rotation defaults to 0; scale defaults to 1.0 on all axes. Returns the
// allocated creation_number.
//
// Game-coords contract: x/y/z are WC3 game coordinates (centered at 0,0),
// matching units.move. Stored verbatim.
func (s *Session) CreateUnit(typeID string, player uint32, pos [3]float32, rotation, scale float32) (uint32, error) {
	if len(typeID) != 4 {
		return 0, fmt.Errorf("type_id %q must be exactly 4 bytes", typeID)
	}
	s.mu.Lock()
	if s.units == nil {
		s.mu.Unlock()
		return 0, fmt.Errorf("no map loaded")
	}
	cn := nextUnitCreationNumber(s.units)
	if scale == 0 {
		scale = 1.0
	}
	e := unitsdoo.Entity{
		TypeID:    typeID,
		Variation: 0,
		Position:  pos,
		Rotation:  rotation,
		Scale:     [3]float32{scale, scale, scale},
		// SkinID defaults to TypeID — an un-reskinned unit's skin IS its type.
		// CRITICAL at subversion >= 11, where unitsdoo.Encode ALWAYS emits a
		// 4-byte skin_id chunk; an empty SkinID fails Encode and would corrupt
		// the file on save. At subversion 9 (skinIDPresent stays false) Encode
		// omits the chunk and this value is harmlessly ignored.
		SkinID:            typeID,
		Flags:             2, // 2 = default visible/selectable (matches WC3 editor placement)
		Player:            player,
		HitPointsPct:      -1, // -1 = use unit default
		ManaPct:           -1,
		MapItemTable:      -1,
		GoldAmount:        0,
		TargetAcquisition: -1, // -1 = unit default
		HeroLevel:         1,
		RandomType:        0,
		RandomData:        make([]byte, 4), // type-0 payload: 4 bytes (level + 3 reserved)
		CustomColor:       -1,
		WaygateRegion:     -1,
		CreationNumber:    cn,
	}
	wasDirty := s.anyDirtyLocked()
	s.units.Entities = append(s.units.Entities, e)
	s.dirtyUnits = true
	s.recordCommand(&createUnitCmd{cn: cn, entity: e})
	historyChanged := s.groupDepth == 0
	s.mu.Unlock()
	if !wasDirty {
		s.notifyDirty(true)
	}
	s.notifyEntityChanged(EntityChange{
		Kind:     "unit",
		ID:       cn,
		Field:    "created",
		Position: pos,
		Rotation: rotation,
		Scale:    [3]float32{scale, scale, scale},
	})
	if historyChanged {
		s.notifyHistoryChanged()
	}
	return cn, nil
}

// DeleteUnit removes the unit with the given creation_number from
// war3mapUnits.doo. The removed record (including its unexported on-disk
// preservation fields) is snapshotted so Undo re-inserts it byte-identically
// at its original index. Returns an error if no unit matches.
func (s *Session) DeleteUnit(creationNumber uint32) error {
	s.mu.Lock()
	if s.units == nil {
		s.mu.Unlock()
		return fmt.Errorf("no map loaded")
	}
	idx := -1
	for i := range s.units.Entities {
		if s.units.Entities[i].CreationNumber == creationNumber {
			idx = i
			break
		}
	}
	if idx < 0 {
		s.mu.Unlock()
		return fmt.Errorf("no unit with creation_number %d", creationNumber)
	}
	saved := s.units.Entities[idx]
	wasDirty := s.anyDirtyLocked()
	s.units.Entities = append(s.units.Entities[:idx], s.units.Entities[idx+1:]...)
	s.dirtyUnits = true
	s.recordCommand(&deleteUnitCmd{cn: creationNumber, index: idx, entity: saved})
	historyChanged := s.groupDepth == 0
	s.mu.Unlock()
	if !wasDirty {
		s.notifyDirty(true)
	}
	s.notifyEntityChanged(EntityChange{Kind: "unit", ID: creationNumber, Field: "deleted"})
	if historyChanged {
		s.notifyHistoryChanged()
	}
	return nil
}

// ---------------------------------------------------------------------------
// Per-instance unit field editing
//
// unitsdoo already PARSES and losslessly round-trips the per-placement fields
// (gold, hp%/mana%, owning player, hero level/stats, target acquisition, custom
// color, item drops, ability mods). units.get returns them. This mutator closes
// the read/write asymmetry: it lets both surfaces SET them, flowing through
// recordCommand → undo/redo + entity-changed like every other placed-entity
// edit.
//
// Undo is byte-faithful by SNAPSHOTTING THE WHOLE ENTITY (a struct copy carries
// the unexported preservation fields — scaleRaw/skinIDPresent/parsed/
// itemDropSetSizes — across the package boundary), then restoring it verbatim
// on Revert. This is the same "snapshot the full record" strategy DeleteUnit
// uses, and it sidesteps having to track per-field undo for the heterogeneous
// field set.
// ---------------------------------------------------------------------------

// UnitInstanceField names the per-placement field a SetUnitInstanceField call
// targets. Kept as a small closed set of string constants so both the wire
// handler (units.set_field) and the Wails App method validate against the same
// vocabulary.
type UnitInstanceField = string

const (
	UnitFieldGold              UnitInstanceField = "gold"               // GoldAmount (gold-mine / gold-coin amount)
	UnitFieldHPPct             UnitInstanceField = "hp_pct"             // HitPointsPct, -1 = unit default
	UnitFieldManaPct           UnitInstanceField = "mana_pct"           // ManaPct, -1 = unit default
	UnitFieldPlayer            UnitInstanceField = "player"             // owning player slot
	UnitFieldHeroLevel         UnitInstanceField = "hero_level"         // HeroLevel (1+)
	UnitFieldHeroStr           UnitInstanceField = "hero_str"           // HeroStr (sub>=11)
	UnitFieldHeroAgi           UnitInstanceField = "hero_agi"           // HeroAgi (sub>=11)
	UnitFieldHeroInt           UnitInstanceField = "hero_int"           // HeroInt (sub>=11)
	UnitFieldTargetAcquisition UnitInstanceField = "target_acquisition" // TargetAcquisition, -1 = unit default
	UnitFieldCustomColor       UnitInstanceField = "custom_color"       // CustomColor (player-color override), -1 = none
	UnitFieldItemDrops         UnitInstanceField = "item_drops"         // ItemDrops (flattened to a single set)
)

// UnitInstanceItemDrop is the wire/binding shape of one item-drop entry for
// SetUnitInstanceItemDrops. Mirrors unitsdoo.ItemDrop with JSON tags so the MCP
// params and the Wails binding decode cleanly.
type UnitInstanceItemDrop struct {
	ItemID string `json:"item_id"`
	Chance uint32 `json:"chance"`
}

// SetUnitInstanceField sets a scalar per-instance field on the placed unit with
// the given creation_number. value carries the new value; its dynamic type must
// match the field (int32 for gold/hp_pct/.../custom_color via a numeric, uint32
// for player/hero_*). To keep the boundary simple and reuse one code path for
// both surfaces, value is accepted as a float64 (the JSON-number lowest common
// denominator) and converted per field. Item drops have their own typed setter
// (SetUnitInstanceItemDrops) because they carry a slice.
//
// Returns an error for an unknown field, an out-of-range value, or a missing
// creation_number. A no-op (new value equals current) returns nil without
// recording history or flipping the dirty flag — matching MoveUnit, so the Save
// pill doesn't go amber when the Properties panel commits an unchanged input.
func (s *Session) SetUnitInstanceField(creationNumber uint32, field UnitInstanceField, value float64) error {
	s.mu.Lock()
	if s.units == nil {
		s.mu.Unlock()
		return fmt.Errorf("no map loaded")
	}
	idx := -1
	for i := range s.units.Entities {
		if s.units.Entities[i].CreationNumber == creationNumber {
			idx = i
			break
		}
	}
	if idx < 0 {
		s.mu.Unlock()
		return fmt.Errorf("no unit with creation_number %d", creationNumber)
	}

	// Snapshot the WHOLE entity (incl. unexported preservation fields) so Revert
	// restores byte-identical state.
	before := s.units.Entities[idx]
	after := before

	switch field {
	case UnitFieldGold:
		v, err := toUint32(field, value)
		if err != nil {
			s.mu.Unlock()
			return err
		}
		after.GoldAmount = v
	case UnitFieldHPPct:
		after.HitPointsPct = clampPct(value)
	case UnitFieldManaPct:
		after.ManaPct = clampPct(value)
	case UnitFieldPlayer:
		v, err := toUint32(field, value)
		if err != nil {
			s.mu.Unlock()
			return err
		}
		after.Player = v
	case UnitFieldHeroLevel:
		v, err := toUint32(field, value)
		if err != nil {
			s.mu.Unlock()
			return err
		}
		if v < 1 {
			v = 1 // hero level is 1-based; 0 is not a valid placed level
		}
		after.HeroLevel = v
	case UnitFieldHeroStr:
		v, err := toUint32(field, value)
		if err != nil {
			s.mu.Unlock()
			return err
		}
		after.HeroStr = v
	case UnitFieldHeroAgi:
		v, err := toUint32(field, value)
		if err != nil {
			s.mu.Unlock()
			return err
		}
		after.HeroAgi = v
	case UnitFieldHeroInt:
		v, err := toUint32(field, value)
		if err != nil {
			s.mu.Unlock()
			return err
		}
		after.HeroInt = v
	case UnitFieldTargetAcquisition:
		after.TargetAcquisition = float32(value)
	case UnitFieldCustomColor:
		// -1 means "use the player slot color"; otherwise a 0-based color index.
		after.CustomColor = int32(value)
	case UnitFieldItemDrops:
		s.mu.Unlock()
		return fmt.Errorf("field %q must be set via SetUnitInstanceItemDrops", field)
	default:
		s.mu.Unlock()
		return fmt.Errorf("unknown unit instance field %q", field)
	}

	return s.applyUnitInstanceFieldLocked(creationNumber, idx, before, after, field)
}

// SetUnitInstanceItemDrops replaces the unit's item-drop list. The new list is
// flattened into a single drop set (unitsdoo + the trigger codegen already
// model the flattened list as one set), with each entry's FourCC validated.
// Passing an empty slice clears all drops.
func (s *Session) SetUnitInstanceItemDrops(creationNumber uint32, drops []UnitInstanceItemDrop) error {
	conv := make([]unitsdoo.ItemDrop, 0, len(drops))
	for i, d := range drops {
		if len(d.ItemID) != 4 {
			return fmt.Errorf("item_drops[%d].item_id %q must be exactly 4 bytes", i, d.ItemID)
		}
		conv = append(conv, unitsdoo.ItemDrop{ItemID: d.ItemID, Chance: d.Chance})
	}

	s.mu.Lock()
	if s.units == nil {
		s.mu.Unlock()
		return fmt.Errorf("no map loaded")
	}
	idx := -1
	for i := range s.units.Entities {
		if s.units.Entities[i].CreationNumber == creationNumber {
			idx = i
			break
		}
	}
	if idx < 0 {
		s.mu.Unlock()
		return fmt.Errorf("no unit with creation_number %d", creationNumber)
	}
	before := s.units.Entities[idx]
	after := before
	unitsdoo.SetItemDrops(&after, conv)
	return s.applyUnitInstanceFieldLocked(creationNumber, idx, before, after, UnitFieldItemDrops)
}

// applyUnitInstanceFieldLocked is the shared tail for the per-instance setters.
// Called with s.mu HELD; it releases the lock. It short-circuits no-op edits,
// writes `after`, records a snapshot-based undo command, and fires the standard
// dirty + entity-changed (+ history) notifications post-unlock.
func (s *Session) applyUnitInstanceFieldLocked(creationNumber uint32, idx int, before, after unitsdoo.Entity, field UnitInstanceField) error {
	if unitInstanceEqual(before, after) {
		s.mu.Unlock()
		return nil
	}
	wasDirty := s.anyDirtyLocked()
	s.units.Entities[idx] = after
	s.dirtyUnits = true
	s.recordCommand(&setUnitInstanceFieldCmd{cn: creationNumber, field: field, before: before, after: after})
	historyChanged := s.groupDepth == 0
	s.mu.Unlock()
	if !wasDirty {
		s.notifyDirty(true)
	}
	s.notifyEntityChanged(EntityChange{Kind: "unit", ID: creationNumber, Field: field})
	if historyChanged {
		s.notifyHistoryChanged()
	}
	return nil
}

// toUint32 converts a JSON-number value to uint32, rejecting negatives and
// fractional values (which would silently truncate). Field name is woven into
// the error so the surface reports which input was bad.
func toUint32(field UnitInstanceField, value float64) (uint32, error) {
	if value < 0 {
		return 0, fmt.Errorf("field %q must be >= 0, got %v", field, value)
	}
	if value > 4294967295 {
		return 0, fmt.Errorf("field %q out of uint32 range: %v", field, value)
	}
	return uint32(value), nil
}

// clampPct normalizes an HP/Mana percentage. Any negative input maps to -1
// ("use unit default", the on-disk sentinel); non-negative values pass through
// truncated to an integer percent.
func clampPct(value float64) int32 {
	if value < 0 {
		return -1
	}
	return int32(value)
}

// unitInstanceEqual reports whether two snapshots differ only in fields this
// mutator can touch. It compares just the editable public fields (the unexported
// preservation fields are carried verbatim in `after` from `before`, so they're
// equal by construction except item drops, which SetItemDrops rewrites — hence
// the explicit ItemDrops comparison).
func unitInstanceEqual(a, b unitsdoo.Entity) bool {
	if a.GoldAmount != b.GoldAmount ||
		a.HitPointsPct != b.HitPointsPct ||
		a.ManaPct != b.ManaPct ||
		a.Player != b.Player ||
		a.HeroLevel != b.HeroLevel ||
		a.HeroStr != b.HeroStr ||
		a.HeroAgi != b.HeroAgi ||
		a.HeroInt != b.HeroInt ||
		a.TargetAcquisition != b.TargetAcquisition ||
		a.CustomColor != b.CustomColor {
		return false
	}
	if len(a.ItemDrops) != len(b.ItemDrops) {
		return false
	}
	for i := range a.ItemDrops {
		if a.ItemDrops[i] != b.ItemDrops[i] {
			return false
		}
	}
	return true
}

// setUnitInstanceFieldCmd snapshots the full pre/post entity for one
// per-instance field edit. Apply/Revert overwrite the whole record (restoring
// the unexported preservation fields too), so a save after undo re-encodes to
// the original bytes.
type setUnitInstanceFieldCmd struct {
	cn            uint32
	field         UnitInstanceField
	before, after unitsdoo.Entity
}

func (c *setUnitInstanceFieldCmd) Label() string { return "Edit unit " + c.field }
func (c *setUnitInstanceFieldCmd) Apply(s *Session) error {
	return overwriteUnitByCN(s, c.cn, c.after)
}
func (c *setUnitInstanceFieldCmd) Revert(s *Session) error {
	return overwriteUnitByCN(s, c.cn, c.before)
}
func (c *setUnitInstanceFieldCmd) Affected(s *Session) []EntityChange {
	return []EntityChange{{Kind: "unit", ID: c.cn, Field: c.field}}
}

// overwriteUnitByCN replaces the entity record (in place, preserving slice
// order) with the supplied snapshot. Called with s.mu held by Apply/Revert.
func overwriteUnitByCN(s *Session, cn uint32, e unitsdoo.Entity) error {
	if s.units == nil {
		return fmt.Errorf("no map loaded")
	}
	for i := range s.units.Entities {
		if s.units.Entities[i].CreationNumber == cn {
			s.units.Entities[i] = e
			s.dirtyUnits = true
			return nil
		}
	}
	return fmt.Errorf("no unit with creation_number %d", cn)
}

// ---------------------------------------------------------------------------
// Doodad create / delete
// ---------------------------------------------------------------------------

// CreateDoodad appends a new placed doodad to war3map.doo at the given game
// coordinates. A fresh creation_number (max existing + 1, min 1) is allocated.
// Rotation defaults to 0; scale defaults to 1.0; variation defaults to 0.
// Returns the allocated creation_number.
//
// Game-coords contract: x/y/z are WC3 game coordinates (centered at 0,0).
// Stored verbatim. Doodad Scale is written RAW (no /128 transform — the
// opposite convention from units).
func (s *Session) CreateDoodad(typeID string, pos [3]float32, rotation, scale float32, variation uint32) (uint32, error) {
	if len(typeID) != 4 {
		return 0, fmt.Errorf("type_id %q must be exactly 4 bytes", typeID)
	}
	s.mu.Lock()
	if s.doodads == nil {
		s.mu.Unlock()
		return 0, fmt.Errorf("no map loaded")
	}
	cn := nextDoodadCreationNumber(s.doodads)
	if scale == 0 {
		scale = 1.0
	}
	d := doodadsdoo.Doodad{
		TypeID:         typeID,
		Variation:      variation,
		Position:       pos,
		Rotation:       rotation,
		Scale:          [3]float32{scale, scale, scale},
		Flags:          2,   // 2 = default visible/solid (matches WC3 editor placement)
		Life:           100, // destructible HP percentage; HiveWE defaults to 100
		MapItemTable:   -1,
		CreationNumber: cn,
	}
	// Reforged-era maps (subversion >= 11) store a per-doodad skin_id; match the
	// surrounding doodads so WC3's peek-based parser stays aligned. An
	// un-reskinned doodad's skin IS its type. For older maps (sub < 11) leave
	// SkinID empty so Encode omits the chunk (doodadsdoo.Encode emits skin_id
	// only when SkinID != "" for hand-constructed doodads).
	if s.doodads.SubVersion >= 11 {
		d.SkinID = typeID
	}
	wasDirty := s.anyDirtyLocked()
	s.doodads.Doodads = append(s.doodads.Doodads, d)
	s.dirtyDoodads = true
	s.recordCommand(&createDoodadCmd{cn: cn, doodad: d})
	historyChanged := s.groupDepth == 0
	s.mu.Unlock()
	if !wasDirty {
		s.notifyDirty(true)
	}
	s.notifyEntityChanged(EntityChange{
		Kind:     "doodad",
		ID:       cn,
		Field:    "created",
		Position: pos,
		Rotation: rotation,
		Scale:    [3]float32{scale, scale, scale},
	})
	if historyChanged {
		s.notifyHistoryChanged()
	}
	return cn, nil
}

// DeleteDoodad removes the doodad with the given creation_number from
// war3map.doo. The removed record (including unexported preservation fields)
// is snapshotted so Undo re-inserts it byte-identically at its original index.
// Returns an error if no doodad matches.
func (s *Session) DeleteDoodad(creationNumber uint32) error {
	s.mu.Lock()
	if s.doodads == nil {
		s.mu.Unlock()
		return fmt.Errorf("no map loaded")
	}
	idx := -1
	for i := range s.doodads.Doodads {
		if s.doodads.Doodads[i].CreationNumber == creationNumber {
			idx = i
			break
		}
	}
	if idx < 0 {
		s.mu.Unlock()
		return fmt.Errorf("no doodad with creation_number %d", creationNumber)
	}
	saved := s.doodads.Doodads[idx]
	wasDirty := s.anyDirtyLocked()
	s.doodads.Doodads = append(s.doodads.Doodads[:idx], s.doodads.Doodads[idx+1:]...)
	s.dirtyDoodads = true
	s.recordCommand(&deleteDoodadCmd{cn: creationNumber, index: idx, doodad: saved})
	historyChanged := s.groupDepth == 0
	s.mu.Unlock()
	if !wasDirty {
		s.notifyDirty(true)
	}
	s.notifyEntityChanged(EntityChange{Kind: "doodad", ID: creationNumber, Field: "deleted"})
	if historyChanged {
		s.notifyHistoryChanged()
	}
	return nil
}

// ---------------------------------------------------------------------------
// Creation-number allocators. Max existing + 1, floored at 1 (creation_number
// 0 is reserved/unused by convention). Called with s.mu held.
// ---------------------------------------------------------------------------

func nextUnitCreationNumber(f *unitsdoo.File) uint32 {
	var max uint32
	for i := range f.Entities {
		if f.Entities[i].CreationNumber > max {
			max = f.Entities[i].CreationNumber
		}
	}
	return max + 1
}

func nextDoodadCreationNumber(f *doodadsdoo.File) uint32 {
	var max uint32
	for i := range f.Doodads {
		if f.Doodads[i].CreationNumber > max {
			max = f.Doodads[i].CreationNumber
		}
	}
	return max + 1
}

// ---------------------------------------------------------------------------
// Undo commands. Create's Revert removes the entity; Delete's Revert re-adds
// the snapshotted record at its original index so a save-after-undo re-encodes
// to the original bytes. Apply (Redo) re-runs the original mutation.
//
// These low-level Apply/Revert helpers set s.dirtyUnits/s.dirtyDoodads
// directly (matching history.go's setUnitPos etc.) — the public dirty event
// fires from the outer Undo/Redo caller in history.go.
// ---------------------------------------------------------------------------

type createUnitCmd struct {
	cn     uint32
	entity unitsdoo.Entity
}

func (c *createUnitCmd) Label() string { return "Create unit" }
func (c *createUnitCmd) Apply(s *Session) error {
	if s.units == nil {
		return fmt.Errorf("no map loaded")
	}
	// Re-insert at the end (creation_number is unique, ordering of a re-created
	// entity is not load-bearing for a fresh create). Guard against a stale
	// redo re-adding a duplicate.
	for i := range s.units.Entities {
		if s.units.Entities[i].CreationNumber == c.cn {
			return nil
		}
	}
	s.units.Entities = append(s.units.Entities, c.entity)
	s.dirtyUnits = true
	return nil
}
func (c *createUnitCmd) Revert(s *Session) error { return removeUnitByCN(s, c.cn) }
func (c *createUnitCmd) Affected(s *Session) []EntityChange {
	// On Redo, the unit exists again (report position); on Revert, it's gone
	// (report a "deleted" event so subscribers drop it).
	if pos, ok := getUnitPos(s, c.cn); ok {
		return []EntityChange{{Kind: "unit", ID: c.cn, Field: "created", Position: pos}}
	}
	return []EntityChange{{Kind: "unit", ID: c.cn, Field: "deleted"}}
}

type deleteUnitCmd struct {
	cn     uint32
	index  int
	entity unitsdoo.Entity
}

func (c *deleteUnitCmd) Label() string           { return "Delete unit" }
func (c *deleteUnitCmd) Apply(s *Session) error  { return removeUnitByCN(s, c.cn) }
func (c *deleteUnitCmd) Revert(s *Session) error { return reinsertUnit(s, c.index, c.entity) }
func (c *deleteUnitCmd) Affected(s *Session) []EntityChange {
	if pos, ok := getUnitPos(s, c.cn); ok {
		return []EntityChange{{Kind: "unit", ID: c.cn, Field: "created", Position: pos}}
	}
	return []EntityChange{{Kind: "unit", ID: c.cn, Field: "deleted"}}
}

type createDoodadCmd struct {
	cn     uint32
	doodad doodadsdoo.Doodad
}

func (c *createDoodadCmd) Label() string { return "Create doodad" }
func (c *createDoodadCmd) Apply(s *Session) error {
	if s.doodads == nil {
		return fmt.Errorf("no map loaded")
	}
	for i := range s.doodads.Doodads {
		if s.doodads.Doodads[i].CreationNumber == c.cn {
			return nil
		}
	}
	s.doodads.Doodads = append(s.doodads.Doodads, c.doodad)
	s.dirtyDoodads = true
	return nil
}
func (c *createDoodadCmd) Revert(s *Session) error { return removeDoodadByCN(s, c.cn) }
func (c *createDoodadCmd) Affected(s *Session) []EntityChange {
	if pos, ok := getDoodadPos(s, c.cn); ok {
		return []EntityChange{{Kind: "doodad", ID: c.cn, Field: "created", Position: pos}}
	}
	return []EntityChange{{Kind: "doodad", ID: c.cn, Field: "deleted"}}
}

type deleteDoodadCmd struct {
	cn     uint32
	index  int
	doodad doodadsdoo.Doodad
}

func (c *deleteDoodadCmd) Label() string           { return "Delete doodad" }
func (c *deleteDoodadCmd) Apply(s *Session) error  { return removeDoodadByCN(s, c.cn) }
func (c *deleteDoodadCmd) Revert(s *Session) error { return reinsertDoodad(s, c.index, c.doodad) }
func (c *deleteDoodadCmd) Affected(s *Session) []EntityChange {
	if pos, ok := getDoodadPos(s, c.cn); ok {
		return []EntityChange{{Kind: "doodad", ID: c.cn, Field: "created", Position: pos}}
	}
	return []EntityChange{{Kind: "doodad", ID: c.cn, Field: "deleted"}}
}

// ---------------------------------------------------------------------------
// Low-level slice mutators, all called with s.mu held.
// ---------------------------------------------------------------------------

func removeUnitByCN(s *Session, cn uint32) error {
	if s.units == nil {
		return fmt.Errorf("no map loaded")
	}
	for i := range s.units.Entities {
		if s.units.Entities[i].CreationNumber == cn {
			s.units.Entities = append(s.units.Entities[:i], s.units.Entities[i+1:]...)
			s.dirtyUnits = true
			return nil
		}
	}
	return fmt.Errorf("no unit with creation_number %d", cn)
}

func reinsertUnit(s *Session, index int, e unitsdoo.Entity) error {
	if s.units == nil {
		return fmt.Errorf("no map loaded")
	}
	// Clamp index in case the slice shrank since the delete (defensive — under
	// normal undo ordering the length matches). Insert preserves the original
	// position so a re-encode reproduces the original ordering.
	if index < 0 {
		index = 0
	}
	if index > len(s.units.Entities) {
		index = len(s.units.Entities)
	}
	s.units.Entities = append(s.units.Entities, unitsdoo.Entity{})
	copy(s.units.Entities[index+1:], s.units.Entities[index:])
	s.units.Entities[index] = e
	s.dirtyUnits = true
	return nil
}

func removeDoodadByCN(s *Session, cn uint32) error {
	if s.doodads == nil {
		return fmt.Errorf("no map loaded")
	}
	for i := range s.doodads.Doodads {
		if s.doodads.Doodads[i].CreationNumber == cn {
			s.doodads.Doodads = append(s.doodads.Doodads[:i], s.doodads.Doodads[i+1:]...)
			s.dirtyDoodads = true
			return nil
		}
	}
	return fmt.Errorf("no doodad with creation_number %d", cn)
}

func reinsertDoodad(s *Session, index int, d doodadsdoo.Doodad) error {
	if s.doodads == nil {
		return fmt.Errorf("no map loaded")
	}
	if index < 0 {
		index = 0
	}
	if index > len(s.doodads.Doodads) {
		index = len(s.doodads.Doodads)
	}
	s.doodads.Doodads = append(s.doodads.Doodads, doodadsdoo.Doodad{})
	copy(s.doodads.Doodads[index+1:], s.doodads.Doodads[index:])
	s.doodads.Doodads[index] = d
	s.dirtyDoodads = true
	return nil
}
