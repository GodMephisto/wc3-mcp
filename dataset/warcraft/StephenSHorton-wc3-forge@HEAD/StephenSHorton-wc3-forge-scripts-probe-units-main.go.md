---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "scripts/probe-units/main.go"
ref: HEAD
---

// Integration probe: open a map, build the Object Editor's merged-units
// view, and print summary stats + a sampled unit's fields. Validates the
// whole base-SLK + INI + w3u-shadow pipeline end-to-end against real CASC
// and a real map.
//
// Usage:
//   go run ./scripts/probe-units                  # opens default sample map
//   go run ./scripts/probe-units <map-folder>     # explicit map
//   go run ./scripts/probe-units <map-folder> <unitID>   # dump one unit's fields
package main

import (
	"fmt"
	"log"
	"os"
	"path"
	"sort"
	"strings"

	"github.com/StephenSHorton/wc3-forge/internal/casc"
	"github.com/StephenSHorton/wc3-forge/internal/forge"
)

const defaultMap = `C:\Users\4step\projects\wc3-survival-game\map\extracted`

func main() {
	mapPath := defaultMap
	if len(os.Args) > 1 {
		mapPath = os.Args[1]
	}
	var probeID string
	if len(os.Args) > 2 {
		probeID = os.Args[2]
	}

	stor, err := casc.Open(`C:\Program Files (x86)\Warcraft III\_retail_`)
	if err != nil {
		log.Fatalf("open casc: %v", err)
	}
	defer stor.Close()

	// Wire the same map-first-then-CASC reader main.go uses. Map fallback
	// matters because some test maps re-export a stock SLK with their own
	// custom rows added — without map-first, those would be invisible.
	forge.SetBaseAssetReader(func(name string) ([]byte, bool, error) {
		clean := strings.ToLower(path.Clean(strings.ReplaceAll(name, "\\", "/")))
		if data, ok, err := forge.Current.ReadFile(clean); err != nil {
			return nil, false, err
		} else if ok {
			return data, true, nil
		}
		return stor.ReadFile(clean)
	})

	if err := forge.Current.Open(mapPath); err != nil {
		log.Fatalf("open map %q: %v", mapPath, err)
	}
	fmt.Printf("loaded map: %s\n\n", mapPath)

	merged, meta, err := forge.MergedUnits()
	if err != nil {
		log.Fatalf("MergedUnits: %v", err)
	}

	fmt.Printf("metadata: %d fields total\n", len(meta.Fields))
	unitFields := 0
	for i := range meta.Fields {
		if meta.Fields[i].AppliesToUnits() {
			unitFields++
		}
	}
	fmt.Printf("          %d apply to units\n\n", unitFields)

	var (
		stock, customs, editedStock int
		raceCounts                  = map[string]int{}
		kindCounts                  = map[string]int{}
	)
	for _, u := range merged {
		race := strings.ToLower(strings.TrimSpace(u.Fields["race"]))
		if race == "" {
			continue // item or other non-unit row
		}
		raceCounts[race]++
		// Same kind classification the handler uses; duplicated here to keep
		// the probe self-contained without exposing classifyKind publicly.
		kind := "unit"
		if strings.Contains(u.Fields["unitclass"], "Hero") {
			kind = "hero"
		} else if u.Fields["isbldg"] == "1" {
			kind = "building"
		} else if u.Fields["special"] == "1" {
			kind = "special"
		}
		kindCounts[kind]++
		switch {
		case u.IsCustom:
			customs++
		case u.IsEdited:
			editedStock++
		default:
			stock++
		}
	}
	fmt.Printf("units total (race-tagged rows): %d\n", stock+customs+editedStock)
	fmt.Printf("  stock unmodified:  %d\n", stock)
	fmt.Printf("  stock with edits:  %d\n", editedStock)
	fmt.Printf("  customs:           %d\n", customs)
	fmt.Println()
	fmt.Println("by race:")
	dumpCounts(raceCounts)
	fmt.Println()
	fmt.Println("by kind:")
	dumpCounts(kindCounts)
	fmt.Println()

	if customs > 0 {
		fmt.Println("first 5 customs:")
		shown := 0
		ids := make([]string, 0, len(merged))
		for id := range merged {
			ids = append(ids, id)
		}
		sort.Strings(ids)
		for _, id := range ids {
			u := merged[id]
			if !u.IsCustom {
				continue
			}
			name := u.Fields["name"]
			fmt.Printf("  %s (base=%s, %d overrides) name=%q\n", id, u.BaseID, len(u.Overridden), name)
			shown++
			if shown >= 5 {
				break
			}
		}
		fmt.Println()
	}

	if probeID != "" {
		u, ok := merged[probeID]
		if !ok {
			fmt.Printf("probe: %q not found\n", probeID)
			return
		}
		fmt.Printf("probe %s: name=%q base=%s custom=%v edited=%v\n",
			probeID, u.Fields["name"], u.BaseID, u.IsCustom, u.IsEdited)
		cols := make([]string, 0, len(u.Fields))
		for c := range u.Fields {
			cols = append(cols, c)
		}
		sort.Strings(cols)
		for _, c := range cols {
			marker := ""
			if u.Overridden[c] {
				marker = " *"
			}
			fmt.Printf("  %-24s = %q%s\n", c, u.Fields[c], marker)
		}
	}
}

func dumpCounts(counts map[string]int) {
	keys := make([]string, 0, len(counts))
	for k := range counts {
		keys = append(keys, k)
	}
	sort.Strings(keys)
	for _, k := range keys {
		fmt.Printf("  %-12s %d\n", k, counts[k])
	}
}
