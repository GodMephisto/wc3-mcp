---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "scripts/find-map-doodad/main.go"
ref: HEAD
---

// Diagnostic: open a .w3x map directly (via MPQ) and dump every doodad
// instance whose resolved MDX file path matches a substring. Used during
// the Soul Discharge investigation to find which custom doodad type ID
// uses that .mdx and at what coordinates.
//
// Usage: go run ./scripts/find-map-doodad <path-to-.w3x> <mdx-substring>
package main

import (
	"fmt"
	"os"
	"path"
	"strings"

	"github.com/StephenSHorton/wc3-forge/internal/casc"
	"github.com/StephenSHorton/wc3-forge/internal/formats/doodadsdoo"
	"github.com/StephenSHorton/wc3-forge/internal/formats/mpq"
	"github.com/StephenSHorton/wc3-forge/internal/formats/slk"
	"github.com/StephenSHorton/wc3-forge/internal/formats/w3objmod"
)

func main() {
	if len(os.Args) < 3 {
		fmt.Fprintln(os.Stderr, "usage: find-map-doodad <map.w3x> <mdx-substring>")
		os.Exit(1)
	}
	mapPath := os.Args[1]
	needle := strings.ToLower(os.Args[2])

	archive, err := mpq.Open(mapPath)
	if err != nil {
		fmt.Fprintln(os.Stderr, "open mpq:", err)
		os.Exit(1)
	}

	read := func(name string) ([]byte, bool) {
		clean := strings.ToLower(path.Clean(strings.ReplaceAll(name, "\\", "/")))
		data, err := archive.Read(clean)
		if err != nil {
			return nil, false
		}
		return data, true
	}

	// CASC for base SLK + meta
	stor, err := casc.Open(`C:\Program Files (x86)\Warcraft III\_retail_`)
	if err != nil {
		fmt.Fprintln(os.Stderr, "casc:", err)
		os.Exit(1)
	}
	defer stor.Close()
	readBase := func(name string) ([]byte, bool) {
		// try map first, then CASC
		if d, ok := read(name); ok {
			return d, true
		}
		clean := strings.ToLower(path.Clean(strings.ReplaceAll(name, "\\", "/")))
		d, ok, err := stor.ReadFile(clean)
		if err != nil || !ok {
			return nil, false
		}
		return d, true
	}

	// Parse base doodad SLK + skin INI
	doodads := slk.New()
	for _, n := range []string{"Doodads/Doodads.slk", "Units/DestructableData.slk"} {
		if d, ok := readBase(n); ok {
			doodads.Load(d)
		}
	}
	for _, n := range []string{"Doodads/doodadSkins.txt", "Units/destructableSkin.txt"} {
		if d, ok := readBase(n); ok {
			doodads.LoadINI(d)
		}
	}

	// Parse w3d (custom doodad types)
	doodadMeta := slk.New()
	if d, ok := readBase("Doodads/DoodadMetaData.slk"); ok {
		doodadMeta.Load(d)
	}
	if d, ok := readBase("Units/DestructableMetaData.slk"); ok {
		doodadMeta.Load(d)
	}
	fieldMap := w3objmod.FieldMap{}
	for id, row := range doodadMeta.Rows {
		col := row.String("field")
		if col != "" {
			fieldMap[id] = strings.ToLower(col)
		}
	}

	customDoodads := map[string]w3objmod.Overrides{}
	customBase := map[string]string{}
	if d, ok := read("war3map.w3d"); ok {
		f, err := w3objmod.Parse(d, true, fieldMap)
		if err == nil {
			for _, c := range f.Customs {
				customDoodads[c.ID] = c.Overrides
				customBase[c.ID] = c.BaseID
			}
		}
	}
	if d, ok := read("war3map.w3b"); ok {
		f, err := w3objmod.Parse(d, false, fieldMap)
		if err == nil {
			for _, c := range f.Customs {
				customDoodads[c.ID] = c.Overrides
				customBase[c.ID] = c.BaseID
			}
		}
	}

	resolveFile := func(typeID string) string {
		// Custom overrides first
		if ov, ok := customDoodads[typeID]; ok {
			if v, has := ov["file"]; has && v != "" {
				return v
			}
			// Walk up to base
			if base, has := customBase[typeID]; has {
				if row := doodads.Row(base); row != nil {
					if v := row.String("file"); v != "" {
						return v
					}
				}
			}
		}
		if row := doodads.Row(typeID); row != nil {
			return row.String("file")
		}
		return ""
	}
	resolveName := func(typeID string) string {
		if ov, ok := customDoodads[typeID]; ok {
			if v, has := ov["name"]; has && v != "" {
				return v
			}
			if base, has := customBase[typeID]; has {
				if row := doodads.Row(base); row != nil {
					if v := row.String("name"); v != "" {
						return v
					}
				}
			}
		}
		if row := doodads.Row(typeID); row != nil {
			return row.String("name")
		}
		return ""
	}

	// Parse war3map.doo and search
	dooBytes, ok := read("war3map.doo")
	if !ok {
		fmt.Fprintln(os.Stderr, "no war3map.doo in map")
		os.Exit(1)
	}
	doo, err := doodadsdoo.Parse(dooBytes)
	if err != nil {
		fmt.Fprintln(os.Stderr, "parse .doo:", err)
		os.Exit(1)
	}

	hits := 0
	typeHits := map[string]int{}
	for _, d := range doo.Doodads {
		file := resolveFile(d.TypeID)
		if file == "" {
			continue
		}
		if !strings.Contains(strings.ToLower(file), needle) {
			continue
		}
		typeHits[d.TypeID]++
		if typeHits[d.TypeID] <= 3 {
			name := resolveName(d.TypeID)
			fmt.Printf("typeID=%s name=%q file=%s\n  cn=%d pos=(%.0f, %.0f, %.0f) rot=%.2f var=%d scale=(%.2f,%.2f,%.2f)\n",
				d.TypeID, name, file, d.CreationNumber, d.Position[0], d.Position[1], d.Position[2], d.Rotation, d.Variation, d.Scale[0], d.Scale[1], d.Scale[2])
		}
		hits++
	}
	fmt.Printf("\ntotal matches: %d across %d type IDs\n", hits, len(typeHits))
	for k, v := range typeHits {
		fmt.Printf("  %s: %d\n", k, v)
	}
}
