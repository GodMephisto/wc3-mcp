---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/handlers_gameplay_terrain.go"
ref: HEAD
---

package forge

import (
	"encoding/json"
	"fmt"
	"strings"

	"github.com/StephenSHorton/wc3-forge/internal/formats/miscdata"
)

// handlers_gameplay_terrain.go wires the MCP bridge surface for three editor
// operations that previously existed ONLY on the GUI (the Wails App methods),
// breaking the project invariant that every edit is reachable from both the
// GUI and an agent. These handlers call the SAME Session-level logic the GUI
// path now funnels through — no duplicated mutation logic:
//
//   gameplay_constants.get   {} -> {rows: [GameplayConstantRow, ...]}
//   gameplay_constants.apply {rows: [...]} -> {ok:true}
//   terrain.swap_tileset     {new_letter, new_ground_tilesets, new_cliff_tilesets,
//                             ground_from_to, cliff_from_to} -> {ok:true}
//
// (minimap.bake lives at the App/main layer because the bake function depends
// on top-level CASC-color helpers; it's registered there — see app.go.)

// GameplayConstantRow is the canonical wire shape for one per-map gameplay
// constant (war3mapMisc.txt). Section is the .ini section ([Misc] in practice);
// Value is the raw stored string (for list-valued constants like
// DamageBonusNormal, the comma-separated value verbatim). Shared by the MCP
// handlers here AND the Wails App methods (which delegate to GetGameplayConstants
// / ApplyGameplayConstants), so both surfaces agree byte-for-byte.
type GameplayConstantRow struct {
	Section string `json:"section"`
	Key     string `json:"key"`
	Value   string `json:"value"`
}

// GetGameplayConstants returns the per-map gameplay-constant overrides
// currently in memory (parsed from war3mapMisc.txt at Open plus any pending
// edits). Returns an empty slice (not an error) for maps that ship no
// overrides. Errors only when no map is loaded.
func (s *Session) GetGameplayConstants() ([]GameplayConstantRow, error) {
	if !s.IsLoaded() {
		return nil, fmt.Errorf("no map loaded")
	}
	g := s.Gameplay()
	if g == nil {
		return []GameplayConstantRow{}, nil
	}
	out := make([]GameplayConstantRow, 0, 32)
	for _, sec := range g.Sections {
		if sec == nil {
			continue
		}
		for _, e := range sec.Entries {
			if e == nil || e.Key == "" {
				continue
			}
			out = append(out, GameplayConstantRow{
				Section: sec.Name,
				Key:     e.Key,
				Value:   e.Value,
			})
		}
	}
	return out, nil
}

// ApplyGameplayConstants replaces the in-memory war3mapMisc.txt with the given
// FULL row set (snapshot replace, not a diff — the file is tiny). Rows are
// written in the order received; an empty `rows` clears all overrides (leaving
// an empty [Misc] section). Routes through MutateGameplay so dirty-tracking +
// the entity-changed event fire identically to the GUI path. This is the single
// implementation both the App.GameplayConstantsApply binding and the
// gameplay_constants.apply MCP handler call.
func (s *Session) ApplyGameplayConstants(rows []GameplayConstantRow) error {
	return s.MutateGameplay(func(f *miscdata.File) {
		// Replace contents wholesale, grouping rows by section while preserving
		// first-seen order so the caller's row order survives the round-trip.
		f.Sections = f.Sections[:0]
		sectionsByName := map[string]*miscdata.Section{}
		var order []string
		for _, r := range rows {
			section := strings.TrimSpace(r.Section)
			if section == "" {
				section = "Misc"
			}
			key := strings.TrimSpace(r.Key)
			if key == "" {
				continue
			}
			sec, ok := sectionsByName[section]
			if !ok {
				sec = &miscdata.Section{Name: section}
				sectionsByName[section] = sec
				order = append(order, section)
			}
			sec.Entries = append(sec.Entries, &miscdata.Entry{Key: key, Value: r.Value})
		}
		for _, name := range order {
			f.Sections = append(f.Sections, sectionsByName[name])
		}
		// Always guarantee a [Misc] section so a re-Get shows the canonical
		// structure even when every row was cleared.
		if _, ok := sectionsByName["Misc"]; !ok {
			f.Sections = append(f.Sections, &miscdata.Section{Name: "Misc"})
		}
	})
}

// --- MCP handlers ----------------------------------------------------------

func handleGameplayConstantsGet(_ json.RawMessage) (any, error) {
	rows, err := Current.GetGameplayConstants()
	if err != nil {
		return nil, err
	}
	return map[string]any{"rows": rows}, nil
}

type gameplayConstantsApplyParams struct {
	Rows []GameplayConstantRow `json:"rows"`
}

func handleGameplayConstantsApply(params json.RawMessage) (any, error) {
	var p gameplayConstantsApplyParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if err := Current.ApplyGameplayConstants(p.Rows); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true, "rows_applied": len(p.Rows)}, nil
}

// terrainSwapTilesetParams mirrors the GUI SwapTilesetRequest with JSON-wire
// types. new_letter is a single-char tileset code; the *_from_to tables remap
// each OLD palette slot to a slot index in the corresponding new palette (see
// Session.SwapTileset for the full validation list).
type terrainSwapTilesetParams struct {
	NewLetter         string   `json:"new_letter"`
	NewGroundTilesets []string `json:"new_ground_tilesets"`
	NewCliffTilesets  []string `json:"new_cliff_tilesets"`
	GroundFromTo      []int    `json:"ground_from_to"`
	CliffFromTo       []int    `json:"cliff_from_to"`
}

func handleTerrainSwapTileset(params json.RawMessage) (any, error) {
	var p terrainSwapTilesetParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if len(p.NewLetter) != 1 {
		return nil, fmt.Errorf("new_letter must be exactly one character")
	}
	req := SwapTilesetRequest{
		NewLetter:         p.NewLetter[0],
		NewGroundTilesets: p.NewGroundTilesets,
		NewCliffTilesets:  p.NewCliffTilesets,
		GroundFromTo:      p.GroundFromTo,
		CliffFromTo:       p.CliffFromTo,
	}
	if err := Current.SwapTileset(req); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}
