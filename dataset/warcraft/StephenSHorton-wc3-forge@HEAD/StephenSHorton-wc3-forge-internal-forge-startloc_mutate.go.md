---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/startloc_mutate.go"
ref: HEAD
---

package forge

import (
	"fmt"
	"math"

	"github.com/StephenSHorton/wc3-forge/internal/formats/unitsdoo"
)

// startloc_mutate.go holds the start-location (sloc) create/move/delete
// mutators plus their undo Command types. Start locations are special units in
// war3mapUnits.doo whose TypeID is the literal FourCC "sloc"; they are NOT real
// units (stock UnitData.slk has no "sloc" row) — they're markers that say
// "player N starts here". The triggers codegen reads each sloc's Player field
// as the START-LOCATION INDEX (see triggers_codegen.go emitConfig:
// DefineStartLocation(e.Player, ...)) and emits gg_start_location_<index>, so
// the index IS the addressable identity, not the creation_number.
//
// CONTRACT (wire methods, handled in handlers_entities.go):
//   start_locations.create  {index, x, y, z?, rotation?} -> {creation_number}
//   start_locations.move     {index, x, y, z?}            -> {ok}
//   start_locations.delete   {index}                      -> {ok}
//   start_locations.list — read-only accessor (handlers_entities.go).
//
// All mutators follow the established placed-entity pattern (see
// entities_mutate.go): capture pre-state under the lock, mutate in place,
// recordCommand for undo, flip dirtyUnits, then fire dirty + entity-changed
// events after releasing the lock. The entity-changed Kind is "unit" (slocs ARE
// units on disk and the scene's sloc renderer registers them in the unit
// instance map) so the existing unit-create/delete viewport handling applies.
//
// Sloc on-disk conventions (mirroring real authored maps — see unitsdoo.go and
// sloc-markers.ts):
//   - Rotation defaults to 3π/2 (≈ 4.712389), the value Reforged writes for a
//     freshly-placed start location (faces "south").
//   - Scale stays 1.0 with scaleRaw forced to 1.0 (the sloc convention: slocs
//     store raw=1.0, real units store raw=128.0). Without SetScaleRaw, Encode
//     would emit Scale*128 and the next Parse would read it back as a 128×
//     unit. SetScaleRaw pins the byte-faithful sloc layout.
//   - Player carries the start-location index (NOT an owning slot).

// slocStartRotation is the facing Reforged writes for a freshly-placed start
// location: 3π/2 radians ("south"). Matches the radians convention the parser
// notes slocs use (vs degrees for user-placed units).
const slocStartRotation = float32(3 * math.Pi / 2)

// slocTypeID is the literal FourCC marking a start-location entity.
const slocTypeID = "sloc"

// CreateStartLocation appends a new start-location (sloc) entity to
// war3mapUnits.doo at the given game coordinates, owned by start-location
// index `index` (which becomes gg_start_location_<index> in the codegen).
// Rejects a duplicate index — WC3 expects at most one sloc per index, and the
// codegen would emit two DefineStartLocation calls for the same index
// otherwise. A fresh creation_number (max existing + 1, min 1) is allocated.
// Returns the allocated creation_number.
//
// Game-coords contract: x/y/z are WC3 game coordinates (centered at 0,0),
// matching units.move. Stored verbatim.
func (s *Session) CreateStartLocation(index uint32, pos [3]float32, rotation float32) (uint32, error) {
	s.mu.Lock()
	if s.units == nil {
		s.mu.Unlock()
		return 0, fmt.Errorf("no map loaded")
	}
	if idx := indexOfStartLocation(s.units, index); idx >= 0 {
		s.mu.Unlock()
		return 0, fmt.Errorf("start location %d already exists", index)
	}
	cn := nextUnitCreationNumber(s.units)
	if rotation == 0 {
		rotation = slocStartRotation
	}
	e := unitsdoo.Entity{
		TypeID:    slocTypeID,
		Variation: 0,
		Position:  pos,
		Rotation:  rotation,
		Scale:     [3]float32{1, 1, 1},
		// A sloc's "skin" is its own type — keeps unitsdoo.Encode happy at
		// subversion >= 11 (which always emits a 4-byte skin_id chunk; an empty
		// SkinID would fail Encode). Harmlessly ignored at subversion 9.
		SkinID:            slocTypeID,
		Flags:             2, // 2 = default visible/selectable
		Player:            index,
		HitPointsPct:      -1,
		ManaPct:           -1,
		MapItemTable:      -1,
		GoldAmount:        0,
		TargetAcquisition: -1,
		HeroLevel:         1,
		RandomType:        0,
		RandomData:        make([]byte, 4),
		CustomColor:       -1,
		WaygateRegion:     -1,
		CreationNumber:    cn,
	}
	// Pin the sloc scale convention (raw=1.0) so Encode reproduces the
	// byte-faithful sloc layout instead of emitting Scale*128 (the unit
	// convention) — see the file header note.
	unitsdoo.SetScaleRaw(&e, [3]float32{1, 1, 1})
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
		Scale:    [3]float32{1, 1, 1},
	})
	if historyChanged {
		s.notifyHistoryChanged()
	}
	return cn, nil
}

// MoveStartLocation relocates the start location with the given index to the
// supplied game coordinates. Looks the sloc up by start-location index (its
// trigger-addressable identity), not creation_number. A no-op (same position)
// doesn't flip dirty or fire events. Returns an error if no sloc has that index.
func (s *Session) MoveStartLocation(index uint32, pos [3]float32) error {
	s.mu.Lock()
	if s.units == nil {
		s.mu.Unlock()
		return fmt.Errorf("no map loaded")
	}
	i := indexOfStartLocation(s.units, index)
	if i < 0 {
		s.mu.Unlock()
		return fmt.Errorf("no start location %d", index)
	}
	cn := s.units.Entities[i].CreationNumber
	old := s.units.Entities[i].Position
	if old == pos {
		s.mu.Unlock()
		return nil
	}
	s.units.Entities[i].Position = pos
	wasDirty := s.anyDirtyLocked()
	s.dirtyUnits = true
	s.recordCommand(&moveUnitCmd{cn: cn, oldPos: old, newPos: pos})
	historyChanged := s.groupDepth == 0
	s.mu.Unlock()
	if !wasDirty {
		s.notifyDirty(true)
	}
	s.notifyEntityChanged(EntityChange{Kind: "unit", ID: cn, Field: "position", Position: pos})
	if historyChanged {
		s.notifyHistoryChanged()
	}
	return nil
}

// DeleteStartLocation removes the start location with the given index. Looks
// the sloc up by start-location index (not creation_number). The removed record
// is snapshotted (with its original slice index) so Undo re-inserts it
// byte-identically. Returns an error if no sloc has that index.
func (s *Session) DeleteStartLocation(index uint32) error {
	s.mu.Lock()
	if s.units == nil {
		s.mu.Unlock()
		return fmt.Errorf("no map loaded")
	}
	i := indexOfStartLocation(s.units, index)
	if i < 0 {
		s.mu.Unlock()
		return fmt.Errorf("no start location %d", index)
	}
	cn := s.units.Entities[i].CreationNumber
	saved := s.units.Entities[i]
	wasDirty := s.anyDirtyLocked()
	s.units.Entities = append(s.units.Entities[:i], s.units.Entities[i+1:]...)
	s.dirtyUnits = true
	s.recordCommand(&deleteUnitCmd{cn: cn, index: i, entity: saved})
	historyChanged := s.groupDepth == 0
	s.mu.Unlock()
	if !wasDirty {
		s.notifyDirty(true)
	}
	s.notifyEntityChanged(EntityChange{Kind: "unit", ID: cn, Field: "deleted"})
	if historyChanged {
		s.notifyHistoryChanged()
	}
	return nil
}

// StartLocationInfo is one start location, as surfaced to the read-only
// list accessor (the GUI Start-Location panel + the MCP start_locations.list
// tool). Index is the trigger-addressable start-location index (Player on the
// sloc entity); CreationNumber is the underlying war3mapUnits.doo identity.
type StartLocationInfo struct {
	Index          uint32     `json:"index"`
	CreationNumber uint32     `json:"creation_number"`
	Position       [3]float32 `json:"position"`
	Rotation       float32    `json:"rotation"`
}

// ListStartLocations returns every start location (sloc entity) in
// war3mapUnits.doo, sorted by start-location index. Empty slice when no map is
// loaded or the map has no slocs.
func (s *Session) ListStartLocations() []StartLocationInfo {
	s.mu.RLock()
	defer s.mu.RUnlock()
	if s.units == nil {
		return nil
	}
	out := make([]StartLocationInfo, 0)
	for i := range s.units.Entities {
		e := &s.units.Entities[i]
		if e.TypeID != slocTypeID {
			continue
		}
		out = append(out, StartLocationInfo{
			Index:          e.Player,
			CreationNumber: e.CreationNumber,
			Position:       e.Position,
			Rotation:       e.Rotation,
		})
	}
	// Insertion-sort by index — slocs are few (typically <= 24) so a simple
	// stable pass is plenty and keeps the list deterministic for the UI/tests.
	for a := 1; a < len(out); a++ {
		for b := a; b > 0 && out[b-1].Index > out[b].Index; b-- {
			out[b-1], out[b] = out[b], out[b-1]
		}
	}
	return out
}

// indexOfStartLocation returns the slice index of the sloc entity whose Player
// (start-location index) == index, or -1. Called with s.mu held (read or
// write).
func indexOfStartLocation(f *unitsdoo.File, index uint32) int {
	for i := range f.Entities {
		if f.Entities[i].TypeID == slocTypeID && f.Entities[i].Player == index {
			return i
		}
	}
	return -1
}
