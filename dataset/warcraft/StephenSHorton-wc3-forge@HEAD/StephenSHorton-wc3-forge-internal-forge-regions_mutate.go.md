---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/regions_mutate.go"
ref: HEAD
---

package forge

import (
	"fmt"

	"github.com/StephenSHorton/wc3-forge/internal/formats/w3r"
)

// regions_mutate.go holds the war3map.w3r region (rect) create/move/resize/
// rename/delete mutators plus their undo Command types. Regions are the
// foundational authoring primitive for nearly every playable custom map
// (spawn zones, "unit enters region", waygate destinations, cinematic bounds),
// so this is the first surface that lets a user OR an agent author them — until
// Phase 1 there were only the read-only triggers.list_regions pickers.
//
// Every mutator follows the established placed-entity pattern (see
// entities_mutate.go): capture pre-state under the lock, mutate in place,
// recordCommand for undo, flip dirtyRegions, then fire dirty + entity-changed
// events after releasing the lock. The entity-changed Kind is "region" and the
// ID is the region's creation_number (its stable, trigger-addressable id).
//
// CONTRACT (wire methods, handled in handlers_regions.go):
//   regions.create  {name, min_x, min_y, max_x, max_y, weather_id?, ambient_id?,
//                     color?[r,g,b]} -> {creation_number}
//   regions.move    {creation_number, dx, dy} -> {ok}
//   regions.resize  {creation_number, min_x, min_y, max_x, max_y} -> {ok}
//   regions.rename  {creation_number, name} -> {ok}
//   regions.delete  {creation_number} -> {ok}
//   regions.list / regions.get — read-only accessors (handlers_regions.go).
//
// Bounds contract: min_x/min_y/max_x/max_y are WC3 game coordinates (centered
// at 0,0), the SAME convention as units.move / the placed-entity surface. They
// map to the w3r Left/Bottom/Right/Top fields verbatim (Left=minX, Bottom=minY,
// Right=maxX, Top=maxY); Parse/Encode preserve them byte-for-byte. The mutators
// normalize so Left<=Right and Bottom<=Top regardless of input order.

// ---------------------------------------------------------------------------
// Region create / delete / mutate
// ---------------------------------------------------------------------------

// CreateRegion appends a new region to war3map.w3r with the given name and
// rectangular bounds (WC3 game coords). A fresh creation_number (max existing
// + 1, min 1) is allocated. weatherID is an optional 4-byte FourCC (empty →
// "no weather", encoded as 4 NULs); ambientID is an optional C-string; color
// defaults to opaque white so a fresh region renders visibly. Returns the
// allocated creation_number.
//
// If the loaded map shipped no war3map.w3r, an empty version-5 table is
// allocated on first create so the new region (and the file) persists on save.
func (s *Session) CreateRegion(name string, minX, minY, maxX, maxY float32, weatherID, ambientID string, color [3]uint8) (int32, error) {
	if name == "" {
		return 0, fmt.Errorf("region name is required")
	}
	if err := validateRegionStrings(name, weatherID, ambientID); err != nil {
		return 0, err
	}
	s.mu.Lock()
	if !s.loaded {
		s.mu.Unlock()
		return 0, fmt.Errorf("no map loaded")
	}
	if s.regions == nil {
		// Map shipped no regions table — start a fresh version-5 file. Version 5
		// is the only version WC3/HiveWE/our Parse expect for war3map.w3r.
		s.regions = &w3r.File{Version: 5}
	}
	left, bottom, right, top := normalizeBounds(minX, minY, maxX, maxY)
	cn := nextRegionCreationNumber(s.regions)
	rg := w3r.Region{
		Left:           left,
		Bottom:         bottom,
		Right:          right,
		Top:            top,
		Name:           name,
		CreationNumber: cn,
		WeatherID:      weatherID,
		AmbientID:      ambientID,
		Color:          color,
	}
	wasDirty := s.anyDirtyLocked()
	s.regions.Regions = append(s.regions.Regions, rg)
	s.dirtyRegions = true
	s.recordCommand(&createRegionCmd{cn: cn, region: rg})
	historyChanged := s.groupDepth == 0
	s.mu.Unlock()
	if !wasDirty {
		s.notifyDirty(true)
	}
	s.notifyEntityChanged(EntityChange{Kind: "region", ID: uint32(cn), Field: "created"})
	if historyChanged {
		s.notifyHistoryChanged()
	}
	return cn, nil
}

// DeleteRegion removes the region with the given creation_number. The removed
// record is snapshotted (with its original slice index) so Undo re-inserts it
// at its original position, keeping the on-disk ordering stable. Returns an
// error if no region matches.
func (s *Session) DeleteRegion(creationNumber int32) error {
	s.mu.Lock()
	if s.regions == nil {
		s.mu.Unlock()
		return fmt.Errorf("no map loaded")
	}
	idx := indexOfRegion(s.regions, creationNumber)
	if idx < 0 {
		s.mu.Unlock()
		return fmt.Errorf("no region with creation_number %d", creationNumber)
	}
	saved := s.regions.Regions[idx]
	wasDirty := s.anyDirtyLocked()
	s.regions.Regions = append(s.regions.Regions[:idx], s.regions.Regions[idx+1:]...)
	s.dirtyRegions = true
	s.recordCommand(&deleteRegionCmd{cn: creationNumber, index: idx, region: saved})
	historyChanged := s.groupDepth == 0
	s.mu.Unlock()
	if !wasDirty {
		s.notifyDirty(true)
	}
	s.notifyEntityChanged(EntityChange{Kind: "region", ID: uint32(creationNumber), Field: "deleted"})
	if historyChanged {
		s.notifyHistoryChanged()
	}
	return nil
}

// MoveRegion translates the region's rectangle by (dx, dy) game units, keeping
// its width and height. A zero delta is a no-op (no dirty flip, no event) so a
// drag that ends where it started doesn't spuriously mark the map dirty.
func (s *Session) MoveRegion(creationNumber int32, dx, dy float32) error {
	s.mu.Lock()
	if s.regions == nil {
		s.mu.Unlock()
		return fmt.Errorf("no map loaded")
	}
	idx := indexOfRegion(s.regions, creationNumber)
	if idx < 0 {
		s.mu.Unlock()
		return fmt.Errorf("no region with creation_number %d", creationNumber)
	}
	if dx == 0 && dy == 0 {
		s.mu.Unlock()
		return nil
	}
	old := regionBounds(&s.regions.Regions[idx])
	newB := old
	newB[0] += dx // left
	newB[1] += dy // bottom
	newB[2] += dx // right
	newB[3] += dy // top
	setRegionBounds(&s.regions.Regions[idx], newB)
	wasDirty := s.anyDirtyLocked()
	s.dirtyRegions = true
	s.recordCommand(&regionBoundsCmd{cn: creationNumber, label: "Move region", oldB: old, newB: newB})
	historyChanged := s.groupDepth == 0
	s.mu.Unlock()
	if !wasDirty {
		s.notifyDirty(true)
	}
	s.notifyEntityChanged(EntityChange{Kind: "region", ID: uint32(creationNumber), Field: "bounds"})
	if historyChanged {
		s.notifyHistoryChanged()
	}
	return nil
}

// ResizeRegion sets the region's rectangle to the given absolute bounds (WC3
// game coords). Bounds are normalized (Left<=Right, Bottom<=Top). A no-op
// (bounds unchanged) doesn't flip dirty or fire events.
func (s *Session) ResizeRegion(creationNumber int32, minX, minY, maxX, maxY float32) error {
	left, bottom, right, top := normalizeBounds(minX, minY, maxX, maxY)
	s.mu.Lock()
	if s.regions == nil {
		s.mu.Unlock()
		return fmt.Errorf("no map loaded")
	}
	idx := indexOfRegion(s.regions, creationNumber)
	if idx < 0 {
		s.mu.Unlock()
		return fmt.Errorf("no region with creation_number %d", creationNumber)
	}
	old := regionBounds(&s.regions.Regions[idx])
	newB := [4]float32{left, bottom, right, top}
	if newB == old {
		s.mu.Unlock()
		return nil
	}
	setRegionBounds(&s.regions.Regions[idx], newB)
	wasDirty := s.anyDirtyLocked()
	s.dirtyRegions = true
	s.recordCommand(&regionBoundsCmd{cn: creationNumber, label: "Resize region", oldB: old, newB: newB})
	historyChanged := s.groupDepth == 0
	s.mu.Unlock()
	if !wasDirty {
		s.notifyDirty(true)
	}
	s.notifyEntityChanged(EntityChange{Kind: "region", ID: uint32(creationNumber), Field: "bounds"})
	if historyChanged {
		s.notifyHistoryChanged()
	}
	return nil
}

// RenameRegion sets the region's name. Renaming changes the gg_rct_<Name>
// codegen handle, so the Trigger Editor's region picker re-resolves after this.
// A no-op (same name) doesn't flip dirty or fire events. Rejects empty names
// and names with embedded NULs (which would corrupt the C-string on encode).
func (s *Session) RenameRegion(creationNumber int32, name string) error {
	if name == "" {
		return fmt.Errorf("region name is required")
	}
	if err := validateRegionStrings(name, "", ""); err != nil {
		return err
	}
	s.mu.Lock()
	if s.regions == nil {
		s.mu.Unlock()
		return fmt.Errorf("no map loaded")
	}
	idx := indexOfRegion(s.regions, creationNumber)
	if idx < 0 {
		s.mu.Unlock()
		return fmt.Errorf("no region with creation_number %d", creationNumber)
	}
	oldName := s.regions.Regions[idx].Name
	if oldName == name {
		s.mu.Unlock()
		return nil
	}
	s.regions.Regions[idx].Name = name
	wasDirty := s.anyDirtyLocked()
	s.dirtyRegions = true
	s.recordCommand(&renameRegionCmd{cn: creationNumber, oldName: oldName, newName: name})
	historyChanged := s.groupDepth == 0
	s.mu.Unlock()
	if !wasDirty {
		s.notifyDirty(true)
	}
	s.notifyEntityChanged(EntityChange{Kind: "region", ID: uint32(creationNumber), Field: "name"})
	if historyChanged {
		s.notifyHistoryChanged()
	}
	return nil
}

// ---------------------------------------------------------------------------
// Helpers (most called with s.mu held).
// ---------------------------------------------------------------------------

// validateRegionStrings rejects the field shapes Encode would reject, so the
// mutator fails up-front (before mutating state) rather than letting a bad
// value lie in memory until the next Save errors. weatherID must be empty or
// exactly 4 bytes (a FourCC); name + ambientID must not contain embedded NULs.
func validateRegionStrings(name, weatherID, ambientID string) error {
	if l := len(weatherID); l != 0 && l != 4 {
		return fmt.Errorf("weather_id %q must be exactly 4 bytes (or empty)", weatherID)
	}
	for _, s := range []struct{ label, v string }{{"name", name}, {"ambient_id", ambientID}} {
		for i := 0; i < len(s.v); i++ {
			if s.v[i] == 0 {
				return fmt.Errorf("%s %q contains embedded NUL", s.label, s.v)
			}
		}
	}
	return nil
}

// normalizeBounds orders the supplied corner coords so left<=right and
// bottom<=top. Callers may pass corners in any order (a rect-drag in the
// viewport can produce either); WC3 expects the canonical ordering.
func normalizeBounds(minX, minY, maxX, maxY float32) (left, bottom, right, top float32) {
	left, right = minX, maxX
	if left > right {
		left, right = right, left
	}
	bottom, top = minY, maxY
	if bottom > top {
		bottom, top = top, bottom
	}
	return
}

// nextRegionCreationNumber returns max existing creation_number + 1, floored at
// 1 (creation_number 0 is reserved/unused by convention, matching the unit +
// doodad allocators). Called with s.mu held.
func nextRegionCreationNumber(f *w3r.File) int32 {
	var max int32
	for i := range f.Regions {
		if f.Regions[i].CreationNumber > max {
			max = f.Regions[i].CreationNumber
		}
	}
	return max + 1
}

// indexOfRegion returns the slice index of the region with the given
// creation_number, or -1. Called with s.mu held.
func indexOfRegion(f *w3r.File, cn int32) int {
	for i := range f.Regions {
		if f.Regions[i].CreationNumber == cn {
			return i
		}
	}
	return -1
}

// regionBounds / setRegionBounds pack/unpack the [left, bottom, right, top]
// tuple the bounds commands snapshot. Called with s.mu held.
func regionBounds(rg *w3r.Region) [4]float32 {
	return [4]float32{rg.Left, rg.Bottom, rg.Right, rg.Top}
}

func setRegionBounds(rg *w3r.Region, b [4]float32) {
	rg.Left, rg.Bottom, rg.Right, rg.Top = b[0], b[1], b[2], b[3]
}

// ---------------------------------------------------------------------------
// Undo commands. Create's Revert removes the region; Delete's Revert re-adds
// the snapshotted record at its original index; bounds/rename commands store
// before+after so Apply (Redo) and Revert (Undo) both reproduce exact state.
//
// These low-level Apply/Revert helpers set s.dirtyRegions directly (matching
// entities_mutate.go's create/delete commands) — the public dirty event fires
// from the outer Undo/Redo caller in history.go.
// ---------------------------------------------------------------------------

type createRegionCmd struct {
	cn     int32
	region w3r.Region
}

func (c *createRegionCmd) Label() string { return "Create region" }
func (c *createRegionCmd) Apply(s *Session) error {
	if s.regions == nil {
		return fmt.Errorf("no map loaded")
	}
	// Guard against a stale redo re-adding a duplicate.
	if indexOfRegion(s.regions, c.cn) >= 0 {
		return nil
	}
	s.regions.Regions = append(s.regions.Regions, c.region)
	s.dirtyRegions = true
	return nil
}
func (c *createRegionCmd) Revert(s *Session) error { return removeRegionByCN(s, c.cn) }
func (c *createRegionCmd) Affected(s *Session) []EntityChange {
	if indexOfRegion(s.regions, c.cn) >= 0 {
		return []EntityChange{{Kind: "region", ID: uint32(c.cn), Field: "created"}}
	}
	return []EntityChange{{Kind: "region", ID: uint32(c.cn), Field: "deleted"}}
}

type deleteRegionCmd struct {
	cn     int32
	index  int
	region w3r.Region
}

func (c *deleteRegionCmd) Label() string           { return "Delete region" }
func (c *deleteRegionCmd) Apply(s *Session) error  { return removeRegionByCN(s, c.cn) }
func (c *deleteRegionCmd) Revert(s *Session) error { return reinsertRegion(s, c.index, c.region) }
func (c *deleteRegionCmd) Affected(s *Session) []EntityChange {
	if indexOfRegion(s.regions, c.cn) >= 0 {
		return []EntityChange{{Kind: "region", ID: uint32(c.cn), Field: "created"}}
	}
	return []EntityChange{{Kind: "region", ID: uint32(c.cn), Field: "deleted"}}
}

// regionBoundsCmd backs both Move and Resize — both are "set bounds" with a
// distinct undo label. oldB/newB are the full [left,bottom,right,top] tuples so
// Apply/Revert are symmetric regardless of how the new bounds were derived.
type regionBoundsCmd struct {
	cn         int32
	label      string
	oldB, newB [4]float32
}

func (c *regionBoundsCmd) Label() string { return c.label }
func (c *regionBoundsCmd) Apply(s *Session) error {
	return setRegionBoundsByCN(s, c.cn, c.newB)
}
func (c *regionBoundsCmd) Revert(s *Session) error {
	return setRegionBoundsByCN(s, c.cn, c.oldB)
}
func (c *regionBoundsCmd) Affected(s *Session) []EntityChange {
	return []EntityChange{{Kind: "region", ID: uint32(c.cn), Field: "bounds"}}
}

type renameRegionCmd struct {
	cn               int32
	oldName, newName string
}

func (c *renameRegionCmd) Label() string { return "Rename region" }
func (c *renameRegionCmd) Apply(s *Session) error {
	return setRegionNameByCN(s, c.cn, c.newName)
}
func (c *renameRegionCmd) Revert(s *Session) error {
	return setRegionNameByCN(s, c.cn, c.oldName)
}
func (c *renameRegionCmd) Affected(s *Session) []EntityChange {
	return []EntityChange{{Kind: "region", ID: uint32(c.cn), Field: "name"}}
}

// ---------------------------------------------------------------------------
// Low-level slice mutators, all called with s.mu held.
// ---------------------------------------------------------------------------

func removeRegionByCN(s *Session, cn int32) error {
	if s.regions == nil {
		return fmt.Errorf("no map loaded")
	}
	idx := indexOfRegion(s.regions, cn)
	if idx < 0 {
		return fmt.Errorf("no region with creation_number %d", cn)
	}
	s.regions.Regions = append(s.regions.Regions[:idx], s.regions.Regions[idx+1:]...)
	s.dirtyRegions = true
	return nil
}

func reinsertRegion(s *Session, index int, rg w3r.Region) error {
	if s.regions == nil {
		return fmt.Errorf("no map loaded")
	}
	// Clamp index in case the slice shrank since the delete (defensive — under
	// normal undo ordering the length matches). Insert preserves the original
	// position so a re-encode reproduces the original ordering.
	if index < 0 {
		index = 0
	}
	if index > len(s.regions.Regions) {
		index = len(s.regions.Regions)
	}
	s.regions.Regions = append(s.regions.Regions, w3r.Region{})
	copy(s.regions.Regions[index+1:], s.regions.Regions[index:])
	s.regions.Regions[index] = rg
	s.dirtyRegions = true
	return nil
}

func setRegionBoundsByCN(s *Session, cn int32, b [4]float32) error {
	if s.regions == nil {
		return fmt.Errorf("no map loaded")
	}
	idx := indexOfRegion(s.regions, cn)
	if idx < 0 {
		return fmt.Errorf("no region with creation_number %d", cn)
	}
	setRegionBounds(&s.regions.Regions[idx], b)
	s.dirtyRegions = true
	return nil
}

func setRegionNameByCN(s *Session, cn int32, name string) error {
	if s.regions == nil {
		return fmt.Errorf("no map loaded")
	}
	idx := indexOfRegion(s.regions, cn)
	if idx < 0 {
		return fmt.Errorf("no region with creation_number %d", cn)
	}
	s.regions.Regions[idx].Name = name
	s.dirtyRegions = true
	return nil
}
