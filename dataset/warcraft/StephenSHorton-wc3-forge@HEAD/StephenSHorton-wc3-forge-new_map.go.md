---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "new_map.go"
ref: HEAD
---

package main

import (
	"encoding/binary"
	"fmt"
	"log"
	"sort"
	"strings"

	"github.com/StephenSHorton/wc3-forge/internal/formats/doodadsdoo"
	"github.com/StephenSHorton/wc3-forge/internal/formats/mpq"
	"github.com/StephenSHorton/wc3-forge/internal/formats/unitsdoo"
	"github.com/StephenSHorton/wc3-forge/internal/formats/w3e"
	"github.com/StephenSHorton/wc3-forge/internal/formats/w3i"
	"github.com/StephenSHorton/wc3-forge/internal/forge"
	"github.com/wailsapp/wails/v2/pkg/runtime"
)

// NewMapParams is the JS-facing request shape for CreateNewMap. Width/Height
// are PLAYABLE tile counts (the editor's "map size", e.g. 64); Tileset is the
// single-letter code ("L" = Lordaeron Summer); Path is the destination .w3x.
type NewMapParams struct {
	Name    string `json:"name"`
	Width   int    `json:"width"`
	Height  int    `json:"height"`
	Tileset string `json:"tileset"`
	Path    string `json:"path"`
}

// Minimum/maximum playable dimension. WC3's World Editor allows 32..480; we
// keep the same envelope so a map made here is editable there too.
const (
	newMapMinDim = 8
	newMapMaxDim = 480
)

// NewMapDialog prompts for the destination of a brand-new map and returns the
// chosen .w3x path ("" if the user cancelled). Mirrors OpenMapDialog's shape
// but uses a save dialog so the user names the file up front — there's no
// in-memory "untitled" session in forge, so a new map needs a real path to
// live at from the moment it's created.
func (a *App) NewMapDialog(defaultName string) (string, error) {
	name := strings.TrimSpace(defaultName)
	if name == "" {
		name = "Untitled"
	}
	return runtime.SaveFileDialog(a.ctx, runtime.SaveDialogOptions{
		Title:           "Create new map",
		DefaultFilename: name + ".w3x",
		Filters: []runtime.FileFilter{
			{DisplayName: "Warcraft III map (*.w3x)", Pattern: "*.w3x"},
		},
	})
}

// CreateNewMap synthesises a minimal but complete WC3 map at p.Path and opens
// it as the current session. The map is a flat single-tileset square with no
// units or doodads — a clean canvas the user can immediately edit (place
// doodads, paint terrain, raise cliffs).
//
// Format choices target maximum compatibility: classic TFT (w3i v25, doodad
// subversion 9, w3e v11). Such a map opens in both classic WC3 and Reforged,
// and forge round-trips it on save. We write a real .w3x (via the from-scratch
// MPQ writer) rather than a folder so the result is launchable in WC3 and
// shareable as a single file.
func (a *App) CreateNewMap(p NewMapParams) (MapStatus, error) {
	if err := writeNewMap(p); err != nil {
		return MapStatus{}, err
	}
	if err := forge.Current.Open(p.Path); err != nil {
		return MapStatus{}, fmt.Errorf("created map but failed to open it: %w", err)
	}
	return a.Status(), nil
}

// WriteNewMap creates the .w3x at p.Path WITHOUT opening it in the current
// session. The frontend uses this when a map is already loaded: it writes the
// new map then opens it in a fresh window, leaving the current session (and any
// unsaved edits) untouched.
func (a *App) WriteNewMap(p NewMapParams) error {
	return writeNewMap(p)
}

// writeNewMap synthesises and writes the .w3x at p.Path. Shared by CreateNewMap
// (create + open here) and WriteNewMap (create only).
func writeNewMap(p NewMapParams) error {
	if strings.TrimSpace(p.Path) == "" {
		return fmt.Errorf("destination path is required")
	}
	if p.Width < newMapMinDim || p.Height < newMapMinDim ||
		p.Width > newMapMaxDim || p.Height > newMapMaxDim {
		return fmt.Errorf("map size %dx%d out of range [%d, %d]",
			p.Width, p.Height, newMapMinDim, newMapMaxDim)
	}

	letter := byte('L')
	if t := strings.TrimSpace(p.Tileset); len(t) >= 1 {
		letter = t[0]
	}
	name := strings.TrimSpace(p.Name)
	if name == "" {
		name = "Untitled"
	}

	ground, cliff := tilesetPalette(letter)
	if len(ground) == 0 {
		// Unknown tileset / SLK unavailable — fall back to a single Lordaeron
		// dirt tile so Encode's "≥1 ground tileset" invariant still holds.
		ground = []string{"Ldrt"}
	}

	terrain := buildNewTerrain(p.Width, p.Height, letter, ground, cliff)
	w3eBytes, err := w3e.Encode(terrain)
	if err != nil {
		return fmt.Errorf("encode terrain: %w", err)
	}
	w3iBytes, err := w3i.Encode(buildNewInfo(name, p.Width, p.Height, letter))
	if err != nil {
		return fmt.Errorf("encode map info: %w", err)
	}
	// Empty placed-entity files at classic TFT versions. Subversion 9 keeps
	// the map classic-consistent (no per-doodad skin_id, which CreateDoodad
	// only emits at subversion ≥ 11).
	doodadBytes, err := doodadsdoo.Encode(&doodadsdoo.File{Version: 8, SubVersion: 9})
	if err != nil {
		return fmt.Errorf("encode doodads: %w", err)
	}
	unitBytes, err := unitsdoo.Encode(&unitsdoo.File{Version: 8, SubVersion: 9})
	if err != nil {
		return fmt.Errorf("encode units: %w", err)
	}

	files := []mpq.FileEntry{
		{Name: "war3map.w3i", Data: w3iBytes},
		{Name: "war3map.w3e", Data: w3eBytes},
		{Name: "war3map.doo", Data: doodadBytes},
		{Name: "war3mapUnits.doo", Data: unitBytes},
	}

	// Bake a minimap (war3mapMap.blp) from the terrain so the new map opens with
	// a real minimap instead of the "No minimap" placeholder — and so the image
	// is present in-game / on the loading screen too. A failure here is
	// non-fatal: the map is still valid and the panel falls back to an on-the-fly
	// generated preview (App.GenerateMinimapBytes).
	if mm, mmErr := BakeMinimapBLP(terrain); mmErr != nil {
		log.Printf("new map: minimap bake failed (continuing without baked minimap): %v", mmErr)
	} else if len(mm) > 0 {
		files = append(files, mpq.FileEntry{Name: "war3mapMap.blp", Data: mm})
	}

	if err := mpq.WriteFile(p.Path, files, buildHM3WHeader(name, 1)); err != nil {
		return fmt.Errorf("write .w3x: %w", err)
	}
	return nil
}

// tilesetPalette returns the ground and cliff FourCC palettes for a tileset
// letter, read from the same CASC SLKs the Swap Tileset dialog uses. Ground
// tiles group by the FourCC's first byte; cliff tiles by the SECOND byte (the
// shared 'C' prefix). Capped at the v11 w3e limits (16 ground, 16 cliff).
func tilesetPalette(letter byte) (ground, cliff []string) {
	if t, _ := loadTerrainSLK(); t != nil {
		for fourCC := range t.Rows {
			if len(fourCC) == 4 && fourCC[0] == letter {
				ground = append(ground, fourCC)
			}
		}
	}
	if c, _ := loadCliffTypesSLK(); c != nil {
		for fourCC := range c.Rows {
			if len(fourCC) == 4 && fourCC[1] == letter {
				cliff = append(cliff, fourCC)
			}
		}
	}
	sort.Strings(ground)
	sort.Strings(cliff)
	if len(ground) > 16 {
		ground = ground[:16]
	}
	if len(cliff) > 16 {
		cliff = cliff[:16]
	}
	return ground, cliff
}

// buildNewTerrain makes a flat w3e: every vertex at ground height 0x2000
// (game-space Z = 0) and cliff layer 2 (the nominal-ground reference, so
// FinalZ = 0). width/height are playable tile counts; the vertex grid is one
// larger in each axis, centered on the world origin.
func buildNewTerrain(width, height int, letter byte, ground, cliff []string) *w3e.File {
	vw := uint32(width + 1)
	vh := uint32(height + 1)
	tiles := make([]w3e.Tilepoint, int(vw)*int(vh))
	for i := range tiles {
		tiles[i] = w3e.Tilepoint{GroundHeight: 0x2000, LayerHeight: 2}
	}
	return &w3e.File{
		Format:         w3e.Magic,
		Version:        11,
		Tileset:        letter,
		GroundTilesets: ground,
		CliffTilesets:  cliff,
		Width:          vw,
		Height:         vh,
		CenterOffset:   [2]float32{-float32(width) * 64, -float32(height) * 64},
		Tiles:          tiles,
	}
}

// buildNewInfo constructs a minimal classic-TFT (v25) war3map.w3i: one
// user-selectable human player, one all-inclusive force, camera bounds matching
// the playable extent, and otherwise neutral defaults.
func buildNewInfo(name string, width, height int, letter byte) *w3i.Info {
	half := float32(width) * 64
	halfY := float32(height) * 64
	return &w3i.Info{
		FileVersion:   w3i.FileVersionTFT, // 25
		MapVersion:    1,
		EditorVersion: 1,

		Name:             name,
		Author:           "wc3-forge",
		Description:      "Created with wc3-forge.",
		SuggestedPlayers: "1",

		CameraLeftBottom:  w3i.Vec2{X: -half, Y: -halfY},
		CameraRightTop:    w3i.Vec2{X: half, Y: halfY},
		CameraLeftTop:     w3i.Vec2{X: -half, Y: halfY},
		CameraRightBottom: w3i.Vec2{X: half, Y: -halfY},
		CameraComplements: w3i.IVec4{},

		PlayableWidth:  int32(width),
		PlayableHeight: int32(height),

		Flags:   w3i.Flags{},
		Tileset: letter,

		LoadingScreen:      w3i.LoadingScreen{Number: -1},
		GameDataSet:        0,
		Fog:                w3i.Fog{StartZ: 3000, EndZ: 5000, Density: 0.5},
		WeatherID:          0,
		CustomLightTileset: letter,
		WaterColor:         w3i.Color{R: 255, G: 255, B: 255, A: 255},

		Players: []w3i.Player{
			{
				InternalNumber:     0,
				Type:               w3i.PlayerHuman,
				Race:               w3i.RaceSelectable,
				FixedStartPosition: 0,
				Name:               "Player 1",
				StartingPosition:   w3i.Vec2{},
			},
		},
		Forces: []w3i.Force{
			{
				Flags:       w3i.ForceFlags{},
				PlayerMasks: 0xFFFFFFFF,
				Name:        "Force 1",
			},
		},
	}
}

// buildHM3WHeader returns the 512-byte HM3W file header that precedes the MPQ
// in a .w3x. WC3's lobby reads the map name + player count from here; the MPQ
// archive starts at offset 512 (mpq.WriteFile prepends this verbatim and keeps
// the MPQ table offsets relative to the MPQ header, so the archive stays valid
// at the shifted position).
func buildHM3WHeader(name string, maxPlayers uint32) []byte {
	buf := make([]byte, 512)
	copy(buf[0:4], "HM3W")
	// buf[4:8] = 0 (unknown / reserved)
	pos := 8
	pos += copy(buf[pos:], name)
	buf[pos] = 0 // null terminator for the name
	pos++
	// map flags (uint32, 0) then max players (uint32). Bounds are safe: names
	// are short and the buffer is 512 bytes.
	binary.LittleEndian.PutUint32(buf[pos:], 0)
	pos += 4
	binary.LittleEndian.PutUint32(buf[pos:], maxPlayers)
	return buf
}
