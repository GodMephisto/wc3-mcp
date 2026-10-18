---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/terrain_mutate.go"
ref: HEAD
---

package forge

import (
	"fmt"

	"github.com/StephenSHorton/wc3-forge/internal/formats/w3e"
)

// Terrain brush mutators — minimal per-corner editing of the loaded
// war3map.w3e heightfield. Three operations land here:
//
//   SetTerrainTile   — set the ground tile of one w3e corner (col,row) to a
//                      FourCC resolved against the map's ground palette.
//   SetTerrainHeight — set the ground height of one corner (game-space Z).
//   GetTerrainTile   — read back {col,row,ground_tile_id,height} so edits are
//                      verifiable live over the wire.
//
// All mutators funnel through recordCommand so undo/redo + the existing
// dirty-tracking and entity-changed buses stay coherent with the rest of the
// editor (same pattern as MoveUnit / SwapTileset). The entity-changed event
// uses Kind="terrain", which the frontend already handles by rebuilding the
// terrain renderer (App.svelte's payload.kind==='terrain' branch) — so no
// frontend change is needed for the viewport to re-render after a brush edit.
//
// col/row are 0-based corner grid indices into the w3e vertex grid:
// col ∈ [0, Width), row ∈ [0, Height). The linear index is row*Width + col
// (row-major, matching the parser's layout and GetTerrain's stride).
//
// Scope is deliberately narrow (ground tile + ground height only). Cliff,
// water, blight, and pathing edits are out of scope for this slice so the
// change lands clean without touching the lossy cliff-remap or water paths.

// terrainCornerIndexLocked validates (col,row) against the loaded terrain's
// dimensions and returns the linear tile index. Caller MUST hold s.mu. Returns
// an error (and a zero index) when no terrain is loaded or the coordinates fall
// outside the vertex grid — never panics on a bad index.
func (s *Session) terrainCornerIndexLocked(col, row int) (int, error) {
	if s.terrain == nil {
		return 0, fmt.Errorf("no terrain loaded")
	}
	w := int(s.terrain.Width)
	h := int(s.terrain.Height)
	if col < 0 || col >= w || row < 0 || row >= h {
		return 0, fmt.Errorf("terrain corner (%d,%d) out of range [0,%d)x[0,%d)", col, row, w, h)
	}
	idx := row*w + col
	if idx < 0 || idx >= len(s.terrain.Tiles) {
		// Defensive: Width*Height should equal len(Tiles) for any parsed file,
		// but guard against a hand-constructed File with a mismatched slice.
		return 0, fmt.Errorf("terrain corner (%d,%d) index %d out of range [0,%d)", col, row, idx, len(s.terrain.Tiles))
	}
	return idx, nil
}

// w3eHeightBaseline is the engine's zero-Z baseline for the packed 16-bit
// ground-height field: raw = game_z*4 + 0x2000. Pulled out as a named constant
// so SetTerrainHeight and the read-back share one source of truth and match
// w3e.Tilepoint.Z()'s decode exactly.
const w3eHeightBaseline = 0x2000

// gameZToRawHeight converts a game-space Z (WC3 world units) into the packed
// int16 ground_height value the .w3e stores. Inverse of w3e.Tilepoint.Z().
// Clamps to the int16 range so a wildly out-of-range request can't wrap around
// silently — the resulting clamped height still encodes/parses cleanly.
func gameZToRawHeight(z float32) int16 {
	raw := int32(z*4.0) + w3eHeightBaseline
	if raw < -32768 {
		raw = -32768
	}
	if raw > 32767 {
		raw = 32767
	}
	return int16(raw)
}

// TerrainTileInfo is the read-back DTO for GetTerrainTile / terrain.get_tile.
// GroundTileID is the FourCC at the corner's palette slot (empty only if the
// stored index somehow points outside the palette — which a well-formed map
// never does). Height is the decoded game-space Z.
type TerrainTileInfo struct {
	Col          int     `json:"col"`
	Row          int     `json:"row"`
	GroundTileID string  `json:"ground_tile_id"`
	Height       float32 `json:"height"`
}

// GetTerrainTile reads back the ground tile + height of the corner at
// (col,row). Pure read — no mutation, no dirty flag, no history. Returns an
// error if no terrain is loaded or the coordinates are out of range.
func (s *Session) GetTerrainTile(col, row int) (TerrainTileInfo, error) {
	s.mu.RLock()
	defer s.mu.RUnlock()
	idx, err := s.terrainCornerIndexLocked(col, row)
	if err != nil {
		return TerrainTileInfo{}, err
	}
	tp := s.terrain.Tiles[idx]
	gid := ""
	if int(tp.GroundTexture) < len(s.terrain.GroundTilesets) {
		gid = s.terrain.GroundTilesets[tp.GroundTexture]
	}
	return TerrainTileInfo{
		Col:          col,
		Row:          row,
		GroundTileID: gid,
		Height:       tp.Z(),
	}, nil
}

// resolveGroundPaletteIndexLocked maps a 4-char ground-tile FourCC to its slot
// index in the loaded terrain's ground palette. Caller MUST hold s.mu. Returns
// a clear error if the FourCC isn't present so the wire surface can report
// "not in palette" rather than silently writing a wrong index. (Adding a tile
// to the palette is a separate, larger operation — SwapTileset territory — so
// this mutator only accepts FourCCs already in the map.)
func (s *Session) resolveGroundPaletteIndexLocked(groundTileID string) (uint8, error) {
	if s.terrain == nil {
		return 0, fmt.Errorf("no terrain loaded")
	}
	if len(groundTileID) != 4 {
		return 0, fmt.Errorf("ground_tile_id %q is not a 4-char FourCC", groundTileID)
	}
	for i, id := range s.terrain.GroundTilesets {
		if id == groundTileID {
			return uint8(i), nil
		}
	}
	return 0, fmt.Errorf("ground_tile_id %q not in map ground palette %v", groundTileID, s.terrain.GroundTilesets)
}

// SetTerrainTile sets the ground tile of the corner at (col,row) to the palette
// slot matching groundTileID (a 4-char FourCC). The FourCC must already exist
// in the map's ground palette — this brush does not grow the palette.
//
// Records an undo command (restoring the prior GroundTexture index), flips
// dirtyTerrain so Save persists war3map.w3e, and fires an entity-changed event
// (Kind="terrain") which the frontend turns into a terrain re-render.
//
// No-op short-circuit when the corner already uses that palette slot, matching
// the MoveUnit pattern (avoids flipping the Save pill for a redundant set).
func (s *Session) SetTerrainTile(col, row int, groundTileID string) error {
	s.mu.Lock()
	idx, err := s.terrainCornerIndexLocked(col, row)
	if err != nil {
		s.mu.Unlock()
		return err
	}
	newIdx, err := s.resolveGroundPaletteIndexLocked(groundTileID)
	if err != nil {
		s.mu.Unlock()
		return err
	}
	oldIdx := s.terrain.Tiles[idx].GroundTexture
	if oldIdx == newIdx {
		s.mu.Unlock()
		return nil
	}
	wasDirty := s.anyDirtyLocked()
	s.terrain.Tiles[idx].GroundTexture = newIdx
	s.dirtyTerrain = true
	s.recordCommand(&setTerrainTileCmd{tileIdx: idx, oldTex: oldIdx, newTex: newIdx})
	historyChanged := s.groupDepth == 0
	s.mu.Unlock()
	if !wasDirty {
		s.notifyDirty(true)
	}
	s.notifyEntityChanged(EntityChange{Kind: "terrain", ID: 0, Field: "tile"})
	if historyChanged {
		s.notifyHistoryChanged()
	}
	return nil
}

// SetTerrainHeight sets the ground height (game-space Z, WC3 world units) of the
// corner at (col,row). The value is packed into the .w3e int16 ground_height
// field via gameZToRawHeight (raw = z*4 + 0x2000); reading it back through
// GetTerrainTile / Tilepoint.Z() recovers the (quantized) game Z.
//
// Records undo (restoring the prior raw int16), flips dirtyTerrain, and fires
// an entity-changed event (Kind="terrain") for re-render. No-op when the new
// raw height equals the stored one (after quantization).
func (s *Session) SetTerrainHeight(col, row int, height float32) error {
	s.mu.Lock()
	idx, err := s.terrainCornerIndexLocked(col, row)
	if err != nil {
		s.mu.Unlock()
		return err
	}
	newRaw := gameZToRawHeight(height)
	oldRaw := s.terrain.Tiles[idx].GroundHeight
	if oldRaw == newRaw {
		s.mu.Unlock()
		return nil
	}
	wasDirty := s.anyDirtyLocked()
	s.terrain.Tiles[idx].GroundHeight = newRaw
	s.dirtyTerrain = true
	s.recordCommand(&setTerrainHeightCmd{tileIdx: idx, oldRaw: oldRaw, newRaw: newRaw})
	historyChanged := s.groupDepth == 0
	s.mu.Unlock()
	if !wasDirty {
		s.notifyDirty(true)
	}
	s.notifyEntityChanged(EntityChange{Kind: "terrain", ID: 0, Field: "height"})
	if historyChanged {
		s.notifyHistoryChanged()
	}
	return nil
}

// ---------------------------------------------------------------------------
// Undo commands. Both store the linear tile index (stable for the lifetime of
// the loaded map — terrain dimensions don't change under these brushes) plus
// the old/new scalar so Apply (Redo) and Revert (Undo) can round-trip without
// re-resolving the FourCC or re-quantizing the height. Mirrors the per-mutator
// command pattern in history.go.
// ---------------------------------------------------------------------------

type setTerrainTileCmd struct {
	tileIdx        int
	oldTex, newTex uint8
}

func (c *setTerrainTileCmd) Label() string { return "Set terrain tile" }
func (c *setTerrainTileCmd) Apply(s *Session) error {
	return setTerrainTexLocked(s, c.tileIdx, c.newTex)
}
func (c *setTerrainTileCmd) Revert(s *Session) error {
	return setTerrainTexLocked(s, c.tileIdx, c.oldTex)
}
func (c *setTerrainTileCmd) Affected(s *Session) []EntityChange {
	return []EntityChange{{Kind: "terrain", ID: 0, Field: "tile"}}
}

type setTerrainHeightCmd struct {
	tileIdx        int
	oldRaw, newRaw int16
}

func (c *setTerrainHeightCmd) Label() string { return "Set terrain height" }
func (c *setTerrainHeightCmd) Apply(s *Session) error {
	return setTerrainHeightLocked(s, c.tileIdx, c.newRaw)
}
func (c *setTerrainHeightCmd) Revert(s *Session) error {
	return setTerrainHeightLocked(s, c.tileIdx, c.oldRaw)
}
func (c *setTerrainHeightCmd) Affected(s *Session) []EntityChange {
	return []EntityChange{{Kind: "terrain", ID: 0, Field: "height"}}
}

// setTerrainTexLocked writes a ground-texture index into one tile. Called with
// s.mu held (from the mutator's recordCommand path and from Undo/Redo). Flips
// dirtyTerrain so a post-undo Save still persists the reverted state.
func setTerrainTexLocked(s *Session, tileIdx int, tex uint8) error {
	if s.terrain == nil {
		return fmt.Errorf("no terrain loaded")
	}
	if tileIdx < 0 || tileIdx >= len(s.terrain.Tiles) {
		return fmt.Errorf("terrain tile index %d out of range [0,%d)", tileIdx, len(s.terrain.Tiles))
	}
	s.terrain.Tiles[tileIdx].GroundTexture = tex
	s.dirtyTerrain = true
	return nil
}

// setTerrainHeightLocked writes a raw int16 ground height into one tile. Same
// locking + dirty contract as setTerrainTexLocked.
func setTerrainHeightLocked(s *Session, tileIdx int, raw int16) error {
	if s.terrain == nil {
		return fmt.Errorf("no terrain loaded")
	}
	if tileIdx < 0 || tileIdx >= len(s.terrain.Tiles) {
		return fmt.Errorf("terrain tile index %d out of range [0,%d)", tileIdx, len(s.terrain.Tiles))
	}
	s.terrain.Tiles[tileIdx].GroundHeight = raw
	s.dirtyTerrain = true
	return nil
}

// _ keeps the w3e import referenced even though every direct use sits behind
// the s.terrain field's package-qualified type. Compile-time guard against a
// future refactor dropping the import.
var _ = w3e.File{}
