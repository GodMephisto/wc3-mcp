---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/terrain_brush.go"
ref: HEAD
---

package forge

import (
	"fmt"
	"math"

	"github.com/StephenSHorton/wc3-forge/internal/formats/w3e"
)

// Terrain BRUSH mutators — region edits over a circular/square footprint of
// corners, the engine behind the GUI Terrain Palette (and the matching
// terrain.*_brush MCP tools).
//
// Where terrain_mutate.go edits a single corner, this file edits every corner
// inside a brush footprint in ONE atomic command (one undo step, one
// entity-changed event). A click-drag "stroke" wraps many such dabs in a
// BeginUndoGroup/EndUndoGroup so the whole stroke collapses to a single Ctrl+Z.
//
// Four operations, mirroring WC3 World Editor / HiveWE:
//
//	PaintTileBrush  — set the ground tile (FourCC) of every footprint corner.
//	HeightBrush     — raise / lower / flatten / smooth the ground heightfield.
//	CliffBrush      — raise / lower / set the integer cliff LayerHeight (0..15),
//	                  setting CliffTexture so a cliff wall actually renders.
//	RampBrush       — toggle the ramp flag so a cliff step renders as a walkable
//	                  slope instead of a sheer wall.
//
// All four resolve the brush footprint Go-side from (centerCol, centerRow,
// radius, shape) so the wire stays tiny during a drag — the frontend streams
// one dab per corner the cursor crosses, not a corner list. radius is in corner
// units (0 = a single corner, 1 = a 3×3 neighborhood, …); shape is "circle"
// (euclidean) or "square" (chebyshev). col/row are 0-based corner grid indices,
// linear index row*Width+col, matching terrain_mutate.go and GetTerrain.
//
// Cliff geometry is fully derived from LayerHeight (+ ramp flags / CliffTexture)
// by GetTerrain's cliff-placement pass and the frontend cliff renderer, so these
// brushes only touch the packed .w3e fields — the walls recompute on re-render.

// Default per-application height step (game-space Z, WC3 world units) for the
// raise/lower height brush when the caller doesn't override strength. One WC3
// cliff step is 128 studs; a single dab nudges far less so repeated strokes
// build up gradually, matching the editor's feel.
const defaultHeightStep = 16.0

// maxLayerHeight is the inclusive ceiling for the packed 4-bit cliff layer.
const maxLayerHeight = 15

// terrainField identifies which packed Tilepoint field a single corner edit
// targets, so one compound command can carry mixed tile/height/layer/flag edits.
type terrainField uint8

const (
	fieldTile      terrainField = iota // GroundTexture (palette index)
	fieldHeightRaw                     // GroundHeight (raw int16)
	fieldLayer                         // LayerHeight (0..15)
	fieldCliffTex                      // CliffTexture (0..15)
	fieldFlags                         // Flags (ramp/blight/water/boundary nibble)
	fieldWaterRaw                      // WaterLevel (raw uint16, 14-bit water surface height)
)

// terrainCornerEdit is one field write on one corner, with the prior value so
// Undo round-trips without re-deriving anything. Values are widened to int32 so
// a single slot covers uint8 indices and int16 heights uniformly.
type terrainCornerEdit struct {
	idx    int
	field  terrainField
	oldVal int32
	newVal int32
}

// terrainBatchCmd is one brush dab: a list of per-corner field writes applied
// (Redo) or reverted (Undo) as a unit. Mirrors the per-mutator command pattern
// in terrain_mutate.go / history.go, but compound so a footprint is one step.
type terrainBatchCmd struct {
	label string
	edits []terrainCornerEdit
}

func (c *terrainBatchCmd) Label() string { return c.label }
func (c *terrainBatchCmd) Apply(s *Session) error {
	return applyTerrainEditsLocked(s, c.edits, false)
}
func (c *terrainBatchCmd) Revert(s *Session) error {
	return applyTerrainEditsLocked(s, c.edits, true)
}
func (c *terrainBatchCmd) Affected(s *Session) []EntityChange {
	return []EntityChange{{Kind: "terrain", ID: 0, Field: "brush"}}
}

// applyTerrainEditsLocked writes the new (or old, when revert) value of each
// edit into the live terrain. Caller MUST hold s.mu. Flips dirtyTerrain so a
// post-undo Save still persists the reverted state. Out-of-range indices are
// skipped defensively (terrain dims don't change under these brushes, so this
// never fires in practice).
func applyTerrainEditsLocked(s *Session, edits []terrainCornerEdit, revert bool) error {
	if s.terrain == nil {
		return fmt.Errorf("no terrain loaded")
	}
	n := len(s.terrain.Tiles)
	for _, e := range edits {
		if e.idx < 0 || e.idx >= n {
			continue
		}
		v := e.newVal
		if revert {
			v = e.oldVal
		}
		tp := &s.terrain.Tiles[e.idx]
		switch e.field {
		case fieldTile:
			tp.GroundTexture = uint8(v)
		case fieldHeightRaw:
			tp.GroundHeight = int16(v)
		case fieldLayer:
			tp.LayerHeight = uint8(v)
		case fieldCliffTex:
			tp.CliffTexture = uint8(v)
		case fieldFlags:
			tp.Flags = uint8(v)
		case fieldWaterRaw:
			tp.WaterLevel = uint16(v)
		}
	}
	s.dirtyTerrain = true
	return nil
}

// terrainBrushCornersLocked returns the linear tile indices of every corner
// within `radius` (in corner units, fractional allowed) of (centerCol,
// centerRow), clipped to the grid. The footprint "reach" is radius+0.5 so small
// brushes are round, not star-shaped, AND a fractional radius grows the
// footprint smoothly: shape "square" includes corners within chebyshev reach
// (an integer radius reproduces the classic (2r+1)² block); anything else
// (default "circle") uses euclidean reach. Caller MUST hold s.mu. Returns nil if
// no terrain is loaded or the center is off-grid.
func (s *Session) terrainBrushCornersLocked(centerCol, centerRow int, radius float64, shape string) []int {
	if s.terrain == nil {
		return nil
	}
	w := int(s.terrain.Width)
	h := int(s.terrain.Height)
	if centerCol < 0 || centerCol >= w || centerRow < 0 || centerRow >= h {
		return nil
	}
	if radius < 0 {
		radius = 0
	}
	square := shape == "square"
	reach := radius + 0.5
	reach2 := reach * reach
	lim := int(math.Ceil(reach))
	out := make([]int, 0, (2*lim+1)*(2*lim+1))
	for dy := -lim; dy <= lim; dy++ {
		row := centerRow + dy
		if row < 0 || row >= h {
			continue
		}
		for dx := -lim; dx <= lim; dx++ {
			col := centerCol + dx
			if col < 0 || col >= w {
				continue
			}
			if square {
				if math.Abs(float64(dx)) > reach || math.Abs(float64(dy)) > reach {
					continue
				}
			} else if float64(dx*dx+dy*dy) > reach2 {
				continue
			}
			out = append(out, row*w+col)
		}
	}
	return out
}

// commitTerrainBrushLocked records a compound command for the given edits and
// returns whether history changed (so the caller can fire notifications after
// unlocking). No-op (returns the command nil) when edits is empty so a dab that
// changed nothing doesn't flip the Save pill or pollute history. Caller MUST
// hold s.mu and is responsible for the post-unlock notifications.
func (s *Session) commitTerrainBrushLocked(label string, edits []terrainCornerEdit) (historyChanged bool) {
	if len(edits) == 0 {
		return false
	}
	s.dirtyTerrain = true
	s.recordCommand(&terrainBatchCmd{label: label, edits: edits})
	return s.groupDepth == 0
}

// finishTerrainBrush fires the post-unlock notifications shared by every brush
// mutator: dirty pill (if it just flipped), one terrain entity-changed event,
// and history-changed when the edit landed outside a group. Call AFTER s.mu is
// released. `field` names what changed ("tile" / "height" / "cliff" / "ramp")
// so the frontend can re-render only the affected surface — a tile paint
// shouldn't trigger a cliff or water rebuild.
func (s *Session) finishTerrainBrush(wasDirty, changed, historyChanged bool, field string) {
	if !changed {
		return
	}
	if !wasDirty {
		s.notifyDirty(true)
	}
	s.notifyEntityChanged(EntityChange{Kind: "terrain", ID: 0, Field: field})
	if historyChanged {
		s.notifyHistoryChanged()
	}
}

// PaintTileBrush sets the ground tile of every corner in the brush footprint to
// groundTileID (a FourCC already in the map's ground palette). One undo step,
// one re-render. No-op corners (already on that tile) are skipped.
func (s *Session) PaintTileBrush(centerCol, centerRow int, radius float64, shape, groundTileID string) error {
	s.mu.Lock()
	corners := s.terrainBrushCornersLocked(centerCol, centerRow, radius, shape)
	if corners == nil {
		s.mu.Unlock()
		return fmt.Errorf("brush center (%d,%d) out of range or no terrain loaded", centerCol, centerRow)
	}
	newIdx, err := s.resolveGroundPaletteIndexLocked(groundTileID)
	if err != nil {
		s.mu.Unlock()
		return err
	}
	wasDirty := s.anyDirtyLocked()
	edits := make([]terrainCornerEdit, 0, len(corners))
	for _, idx := range corners {
		old := s.terrain.Tiles[idx].GroundTexture
		if old == newIdx {
			continue
		}
		s.terrain.Tiles[idx].GroundTexture = newIdx
		edits = append(edits, terrainCornerEdit{idx: idx, field: fieldTile, oldVal: int32(old), newVal: int32(newIdx)})
	}
	historyChanged := s.commitTerrainBrushLocked("Paint terrain", edits)
	s.mu.Unlock()
	s.finishTerrainBrush(wasDirty, len(edits) > 0, historyChanged, "tile")
	return nil
}

// HeightBrush adjusts the ground heightfield over the footprint. mode:
//
//	"raise"   — add strength (game-Z units) to each corner.
//	"lower"   — subtract strength.
//	"flatten" — set every corner to `target` (game-Z); WC3's plateau tool.
//	"smooth"  — move each corner toward the average of its 4-neighbors by
//	            strength fraction (strength clamped to [0,1]; default 0.5).
//
// strength<=0 falls back to defaultHeightStep for raise/lower and 0.5 for
// smooth. Heights are quantized to the .w3e int16 grid; corners that don't
// actually move are skipped. One undo step, one re-render.
func (s *Session) HeightBrush(centerCol, centerRow int, radius float64, shape, mode string, strength, target float32) error {
	s.mu.Lock()
	corners := s.terrainBrushCornersLocked(centerCol, centerRow, radius, shape)
	if corners == nil {
		s.mu.Unlock()
		return fmt.Errorf("brush center (%d,%d) out of range or no terrain loaded", centerCol, centerRow)
	}
	w := int(s.terrain.Width)
	h := int(s.terrain.Height)
	wasDirty := s.anyDirtyLocked()
	edits := make([]terrainCornerEdit, 0, len(corners))
	for _, idx := range corners {
		oldRaw := s.terrain.Tiles[idx].GroundHeight
		var newZ float32
		switch mode {
		case "lower":
			step := strength
			if step <= 0 {
				step = defaultHeightStep
			}
			newZ = s.terrain.Tiles[idx].Z() - step
		case "flatten":
			newZ = target
		case "smooth":
			frac := strength
			if frac <= 0 {
				frac = 0.5
			}
			if frac > 1 {
				frac = 1
			}
			avg := neighborAvgZLocked(s, idx, w, h)
			cur := s.terrain.Tiles[idx].Z()
			newZ = cur + (avg-cur)*frac
		default: // "raise"
			step := strength
			if step <= 0 {
				step = defaultHeightStep
			}
			newZ = s.terrain.Tiles[idx].Z() + step
		}
		newRaw := gameZToRawHeight(newZ)
		if newRaw == oldRaw {
			continue
		}
		s.terrain.Tiles[idx].GroundHeight = newRaw
		edits = append(edits, terrainCornerEdit{idx: idx, field: fieldHeightRaw, oldVal: int32(oldRaw), newVal: int32(newRaw)})
	}
	historyChanged := s.commitTerrainBrushLocked("Edit terrain height", edits)
	s.mu.Unlock()
	s.finishTerrainBrush(wasDirty, len(edits) > 0, historyChanged, "height")
	return nil
}

// neighborAvgZLocked returns the average game-Z of the up-to-4 orthogonal
// neighbors of corner idx (falling back to the corner itself at the grid edge),
// used by the smooth brush. Caller MUST hold s.mu.
func neighborAvgZLocked(s *Session, idx, w, h int) float32 {
	col := idx % w
	row := idx / w
	var sum float32
	var cnt int
	add := func(c, r int) {
		if c < 0 || c >= w || r < 0 || r >= h {
			return
		}
		sum += s.terrain.Tiles[r*w+c].Z()
		cnt++
	}
	add(col-1, row)
	add(col+1, row)
	add(col, row-1)
	add(col, row+1)
	if cnt == 0 {
		return s.terrain.Tiles[idx].Z()
	}
	return sum / float32(cnt)
}

// CliffBrush adjusts the integer cliff LayerHeight over the footprint. mode:
//
//	"raise" — LayerHeight += level (level<1 ⇒ 1), clamped to 15.
//	"lower" — LayerHeight -= level (level<1 ⇒ 1), clamped to 0.
//	"set"   — LayerHeight = level (clamped to [0,15]).
//
// For raise/lower, `level` is the number of cliff steps to move (how high/low);
// for set it's the absolute target layer. Either way the staircase ripple below
// keeps adjacent corners within one level.
//
// CRITICAL: WC3 cliff models only exist for a ONE-level step between adjacent
// corners (the Cliffs<ABCD>N.mdx names encode each corner's height above the
// cell minimum, and Blizzard ships only A/B = 0/1 patterns). A 2+ level step has
// no model → an invisible wall. So after editing the footprint we "ripple" the
// change outward (enforceCliffStaircaseLocked), pulling neighboring corners into
// a staircase where no two adjacent corners differ by more than one level —
// exactly what the World Editor does when you raise/lower a cliff.
//
// Every corner that ends up on a cliff (footprint + rippled) gets a valid
// CliffTexture (palette slot for cliffTileID, or slot 0) so a wall renders — a
// corner left at CliffTexture 15 ("no cliff") renders a void. One undo step.
func (s *Session) CliffBrush(centerCol, centerRow int, radius float64, shape, mode string, level int, cliffTileID string) error {
	s.mu.Lock()
	corners := s.terrainBrushCornersLocked(centerCol, centerRow, radius, shape)
	if corners == nil {
		s.mu.Unlock()
		return fmt.Errorf("brush center (%d,%d) out of range or no terrain loaded", centerCol, centerRow)
	}
	cliffTex := s.resolveCliffPaletteIndexLocked(cliffTileID)
	wasDirty := s.anyDirtyLocked()
	edits := make([]terrainCornerEdit, 0, len(corners)*2)
	footprint := make(map[int]bool, len(corners))
	layerChanged := false
	for _, idx := range corners {
		footprint[idx] = true
		oldLayer := int(s.terrain.Tiles[idx].LayerHeight)
		step := level
		if step < 1 {
			step = 1 // raise/lower default + guard against a 0/empty amount
		}
		var newLayer int
		switch mode {
		case "lower":
			newLayer = oldLayer - step
		case "set":
			newLayer = level
		default: // "raise"
			newLayer = oldLayer + step
		}
		if newLayer < 0 {
			newLayer = 0
		}
		if newLayer > maxLayerHeight {
			newLayer = maxLayerHeight
		}
		if newLayer != oldLayer {
			s.terrain.Tiles[idx].LayerHeight = uint8(newLayer)
			edits = append(edits, terrainCornerEdit{idx: idx, field: fieldLayer, oldVal: int32(oldLayer), newVal: int32(newLayer)})
			layerChanged = true
		}
		setCliffTexLocked(s, idx, cliffTex, &edits)
	}
	// Ripple into a valid 1-level staircase so multi-level steps don't render as
	// invisible walls (no Cliffs<C/D...>.mdx exists). Only when a layer actually
	// moved — a no-op cliff dab shouldn't reshape the whole map.
	if layerChanged {
		s.enforceCliffStaircaseLocked(footprint, cliffTex, &edits)
	}
	historyChanged := s.commitTerrainBrushLocked("Edit cliff", edits)
	s.mu.Unlock()
	s.finishTerrainBrush(wasDirty, len(edits) > 0, historyChanged, "cliff")
	return nil
}

// setCliffTexLocked sets one corner's CliffTexture (recording the edit) unless
// it already matches. Caller MUST hold s.mu.
func setCliffTexLocked(s *Session, idx int, cliffTex uint8, edits *[]terrainCornerEdit) {
	oldTex := s.terrain.Tiles[idx].CliffTexture
	if oldTex != cliffTex {
		s.terrain.Tiles[idx].CliffTexture = cliffTex
		*edits = append(*edits, terrainCornerEdit{idx: idx, field: fieldCliffTex, oldVal: int32(oldTex), newVal: int32(cliffTex)})
	}
}

// enforceCliffStaircaseLocked pulls every corner into a configuration where no
// two 4-adjacent corners differ by more than one cliff level, by propagating
// outward from the (pinned) footprint corners. Each adjusted corner is recorded
// in edits and given the cliff texture so its staircase step renders. Caller
// MUST hold s.mu.
//
// Termination: for a raise the footprint is the local maximum, so neighbors only
// ever move UP (toward it); for a lower, only DOWN. Either way each corner moves
// monotonically within [0,15], so the BFS settles. A hard cap guards the
// pathological "set" case against any cycle.
func (s *Session) enforceCliffStaircaseLocked(footprint map[int]bool, cliffTex uint8, edits *[]terrainCornerEdit) {
	w := int(s.terrain.Width)
	h := int(s.terrain.Height)
	queue := make([]int, 0, len(footprint)*4)
	for idx := range footprint {
		queue = append(queue, idx)
	}
	limit := w * h * (maxLayerHeight + 2) // generous bound; never reached in practice
	for qi := 0; qi < len(queue); qi++ {
		if qi > limit {
			break
		}
		idx := queue[qi]
		L := int(s.terrain.Tiles[idx].LayerHeight)
		col := idx % w
		row := idx / w
		neighbors := [4][2]int{{col - 1, row}, {col + 1, row}, {col, row - 1}, {col, row + 1}}
		for _, nb := range neighbors {
			c, r := nb[0], nb[1]
			if c < 0 || c >= w || r < 0 || r >= h {
				continue
			}
			nidx := r*w + c
			if footprint[nidx] {
				continue // pinned — the user's explicit edit
			}
			cur := int(s.terrain.Tiles[nidx].LayerHeight)
			target := cur
			if cur > L+1 {
				target = L + 1
			} else if cur < L-1 {
				target = L - 1
			}
			if target == cur {
				continue
			}
			s.terrain.Tiles[nidx].LayerHeight = uint8(target)
			*edits = append(*edits, terrainCornerEdit{idx: nidx, field: fieldLayer, oldVal: int32(cur), newVal: int32(target)})
			setCliffTexLocked(s, nidx, cliffTex, edits)
			queue = append(queue, nidx)
		}
	}
}

// RampBrush toggles the ramp flag (Flags bit 0x1) over the footprint. With
// on=true the cliff step under the footprint renders as a walkable slope; with
// on=false it reverts to a sheer wall. One undo step, one re-render.
func (s *Session) RampBrush(centerCol, centerRow int, radius float64, shape string, on bool) error {
	s.mu.Lock()
	corners := s.terrainBrushCornersLocked(centerCol, centerRow, radius, shape)
	if corners == nil {
		s.mu.Unlock()
		return fmt.Errorf("brush center (%d,%d) out of range or no terrain loaded", centerCol, centerRow)
	}
	wasDirty := s.anyDirtyLocked()
	edits := make([]terrainCornerEdit, 0, len(corners))
	for _, idx := range corners {
		oldFlags := s.terrain.Tiles[idx].Flags
		newFlags := oldFlags
		if on {
			newFlags |= 0x1
		} else {
			newFlags &^= 0x1
		}
		if newFlags == oldFlags {
			continue
		}
		s.terrain.Tiles[idx].Flags = newFlags
		edits = append(edits, terrainCornerEdit{idx: idx, field: fieldFlags, oldVal: int32(oldFlags), newVal: int32(newFlags)})
	}
	historyChanged := s.commitTerrainBrushLocked("Edit ramp", edits)
	s.mu.Unlock()
	s.finishTerrainBrush(wasDirty, len(edits) > 0, historyChanged, "ramp")
	return nil
}

// Water-surface constants, mirroring HiveWE's CellOperator (terrain_operators.cpp).
// When "add" places water over a corner that has none, the new surface sits a
// fixed amount above the corner's *final* rendered ground Z. HiveWE expresses
// this in cell units (1 cell = 128 studs): WATER_GROUND_ZERO (0.70) +
// WATER_HEIGHT (0.25) = 0.95 cells above ground → ~121.6 studs. Kept as a single
// stud constant so the brush and any future water-tool share one source of truth.
const waterSurfaceAboveGroundStuds = (0.70 + 0.25) * 128.0

// w3eWaterMaxRaw is the inclusive ceiling for the packed 14-bit water-height
// field (bits 0..13 of the .w3e water_and_flags word). The top two bits hold the
// boundary marker + are reserved, so the raw water value must stay in [0,0x3FFF].
const w3eWaterMaxRaw = 0x3FFF

// gameZToRawWater converts a game-space water-surface Z (WC3 world units) into
// the packed 14-bit WaterLevel the .w3e stores. Same baseline/scale as the
// ground-height field (raw = game_z*4 + 0x2000), inverse of w3e.Tilepoint.WaterZ(),
// but clamped to the unsigned 14-bit range instead of int16 — the high bits are
// owned by the boundary marker and must never be touched here.
func gameZToRawWater(z float32) uint16 {
	raw := int32(z*4.0) + w3eHeightBaseline
	if raw < 0 {
		raw = 0
	}
	if raw > w3eWaterMaxRaw {
		raw = w3eWaterMaxRaw
	}
	return uint16(raw)
}

// WaterBrush adds or removes water over the footprint. mode:
//
//	"add"    — flag every footprint corner as having water (Flags bit 0x4) and set
//	           the water surface to a single flat height. That height is `height`
//	           when `hasHeight` is true (the caller supplies the surface Z in game
//	           units), otherwise it is auto-derived from THIS dab's center corner
//	           ground + waterSurfaceAboveGroundStuds. Either way ALL footprint
//	           corners get the SAME surface, so the sheet is flat — and the GUI
//	           captures one height at stroke start (see WaterAddHeightAt) and passes
//	           it for every dab, so a click-drag paints one flat lake the way HiveWE
//	           does, instead of draping over the terrain.
//
//	           `overwrite` controls existing visible water: when false (Auto mode),
//	           a corner that already holds water ABOVE its ground keeps its height,
//	           so auto-fill never lowers a deep lake you brush over (HiveWE's
//	           behavior). When true (Fixed-height mode), every painted corner is set
//	           to the new surface, raising OR lowering existing water — the user
//	           asked for that exact level. Corners with no water or with water at/
//	           below ground always take the new surface regardless.
//	"remove" — clear the water flag and zero the stored level.
//
// Mirrors HiveWE's CellOperator add_water/remove_water. Each corner records up to
// two edits (the flag word + the water level) so Undo restores both exactly. One
// undo step, one re-render. WC3 renders a water quad for any cell with at least
// one watered corner, so a corner footprint fills ponds the same way the World
// Editor's cell brush does.
func (s *Session) WaterBrush(centerCol, centerRow int, radius float64, shape, mode string, height float32, hasHeight, overwrite bool) error {
	s.mu.Lock()
	corners := s.terrainBrushCornersLocked(centerCol, centerRow, radius, shape)
	if corners == nil {
		s.mu.Unlock()
		return fmt.Errorf("brush center (%d,%d) out of range or no terrain loaded", centerCol, centerRow)
	}
	remove := mode == "remove"
	// Resolve a SINGLE surface height for the whole footprint so every corner that
	// gains water lands on one flat plane. Explicit when the caller passes one;
	// otherwise auto-derived from this dab's center corner ground.
	var rawSurface uint16
	if !remove {
		surfaceZ := height
		if !hasHeight {
			center := centerRow*int(s.terrain.Width) + centerCol
			surfaceZ = s.terrain.Tiles[center].FinalZ() + waterSurfaceAboveGroundStuds
		}
		rawSurface = gameZToRawWater(surfaceZ)
	}
	wasDirty := s.anyDirtyLocked()
	edits := make([]terrainCornerEdit, 0, len(corners)*2)
	for _, idx := range corners {
		tp := &s.terrain.Tiles[idx]
		oldFlags := tp.Flags
		oldWater := tp.WaterLevel

		var newFlags uint8
		var newWater uint16
		if remove {
			newFlags = oldFlags &^ 0x4
			newWater = 0
		} else {
			newFlags = oldFlags | 0x4
			// In Auto mode (overwrite=false) keep an existing, already-visible
			// surface so we don't lower a deep lake brushed over; only set the flat
			// stroke height when the corner was dry or its water sat at/below the
			// ground (a void). In Fixed-height mode (overwrite=true) the user dialed
			// an exact level, so always apply it. waterAboveGroundLocked matches
			// HiveWE's water_above_ground test.
			if !overwrite && tp.HasWater() && waterAboveGroundLocked(tp) {
				newWater = oldWater
			} else {
				newWater = rawSurface
			}
		}

		if newFlags != oldFlags {
			tp.Flags = newFlags
			edits = append(edits, terrainCornerEdit{idx: idx, field: fieldFlags, oldVal: int32(oldFlags), newVal: int32(newFlags)})
		}
		if newWater != oldWater {
			tp.WaterLevel = newWater
			edits = append(edits, terrainCornerEdit{idx: idx, field: fieldWaterRaw, oldVal: int32(oldWater), newVal: int32(newWater)})
		}
	}
	historyChanged := s.commitTerrainBrushLocked("Edit water", edits)
	s.mu.Unlock()
	s.finishTerrainBrush(wasDirty, len(edits) > 0, historyChanged, "water")
	return nil
}

// waterAboveGroundLocked reports whether a corner's stored water surface sits
// above its final rendered ground Z (i.e. the water is actually visible). Used
// by WaterBrush to decide whether to preserve an existing water height or raise
// it. Mirrors HiveWE CellOperator::water_above_ground. Caller MUST hold s.mu.
func waterAboveGroundLocked(tp *w3e.Tilepoint) bool {
	return tp.WaterZ() > tp.FinalZ()
}

// WaterAddHeightAt returns the water-surface Z (game units) the "add" brush
// would place at corner (col,row) in auto mode — the corner's final ground Z plus
// the fixed shore offset. The Terrain Palette calls this once at stroke start so
// every dab in a click-drag paints to one flat level, mirroring HiveWE's
// capture-the-height-at-stroke-begin behavior. Pure read: no mutation/history.
func (s *Session) WaterAddHeightAt(col, row int) (float32, error) {
	s.mu.Lock()
	defer s.mu.Unlock()
	idx, err := s.terrainCornerIndexLocked(col, row)
	if err != nil {
		return 0, err
	}
	return s.terrain.Tiles[idx].FinalZ() + waterSurfaceAboveGroundStuds, nil
}

// resolveCliffPaletteIndexLocked maps a 4-char cliff FourCC to its slot index in
// the map's cliff palette, returning 0 when the id is empty or not found (so the
// cliff brush always has a renderable texture rather than the 15 "no cliff"
// sentinel). Caller MUST hold s.mu. Distinct from the ground-palette resolver:
// cliff edits should degrade gracefully to slot 0 instead of erroring, since the
// brush's job is to raise terrain, not to validate a tileset choice.
func (s *Session) resolveCliffPaletteIndexLocked(cliffTileID string) uint8 {
	if s.terrain == nil {
		return 0
	}
	if len(cliffTileID) == 4 {
		for i, id := range s.terrain.CliffTilesets {
			if id == cliffTileID && i <= maxLayerHeight {
				return uint8(i)
			}
		}
	}
	return 0
}
