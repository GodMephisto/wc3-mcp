---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "scripts/dump-unitmeta/main.go"
ref: HEAD
---

// Diagnostic: load UnitMetaData.slk from CASC and dump a summary of its
// structure (column headers, unique categories, unique types, sample rows).
// One-shot validation that the Object editor's field-metadata spec matches
// the bytes Blizzard actually ships.
//
// Usage: go run ./scripts/dump-unitmeta
package main

import (
	"fmt"
	"os"
	"path"
	"sort"
	"strings"

	"github.com/StephenSHorton/wc3-forge/internal/casc"
	"github.com/StephenSHorton/wc3-forge/internal/formats/slk"
)

func main() {
	install := `C:\Program Files (x86)\Warcraft III\_retail_`
	stor, err := casc.Open(install)
	if err != nil {
		fmt.Fprintln(os.Stderr, "open casc:", err)
		os.Exit(1)
	}
	defer stor.Close()

	// Try both spelling/case variants — Reforged moved things around.
	candidates := []string{
		"Units/UnitMetaData.slk",
		"units/unitmetadata.slk",
		"Units/unitmetadata.slk",
	}

	var found string
	var data []byte
	for _, cand := range candidates {
		clean := strings.ToLower(path.Clean(strings.ReplaceAll(cand, "\\", "/")))
		d, ok, err := stor.ReadFile(clean)
		if err == nil && ok {
			data = d
			found = cand
			break
		}
	}
	if data == nil {
		fmt.Fprintln(os.Stderr, "UnitMetaData.slk not found at any candidate path")
		os.Exit(1)
	}
	fmt.Printf("loaded %s (%d bytes)\n\n", found, len(data))

	m := slk.New()
	if err := m.Load(data); err != nil {
		fmt.Fprintln(os.Stderr, "parse:", err)
		os.Exit(1)
	}
	fmt.Printf("rows: %d\n\n", len(m.Rows))

	// Discover all columns by sampling rows.
	colSet := map[string]struct{}{}
	for _, row := range m.Rows {
		for c := range row {
			colSet[c] = struct{}{}
		}
	}
	cols := make([]string, 0, len(colSet))
	for c := range colSet {
		cols = append(cols, c)
	}
	sort.Strings(cols)
	fmt.Println("columns:")
	for _, c := range cols {
		fmt.Printf("  %s\n", c)
	}
	fmt.Println()

	// Tabulate field counts by category + type.
	catCounts := map[string]int{}
	typeCounts := map[string]int{}
	slkCounts := map[string]int{}
	for _, row := range m.Rows {
		catCounts[row["category"]]++
		typeCounts[row["type"]]++
		slkCounts[row["slk"]]++
	}
	dumpCounts := func(label string, counts map[string]int) {
		fmt.Printf("%s:\n", label)
		keys := make([]string, 0, len(counts))
		for k := range counts {
			keys = append(keys, k)
		}
		sort.Strings(keys)
		for _, k := range keys {
			fmt.Printf("  %-30s %d\n", fmt.Sprintf("%q", k), counts[k])
		}
		fmt.Println()
	}
	dumpCounts("categories", catCounts)
	dumpCounts("types", typeCounts)
	dumpCounts("slk", slkCounts)

	// Show 10 sample rows in full to ground the spec.
	fmt.Println("sample rows (first 10 keys):")
	keys := make([]string, 0, len(m.Rows))
	for k := range m.Rows {
		keys = append(keys, k)
	}
	sort.Strings(keys)
	for i, k := range keys {
		if i >= 10 {
			break
		}
		row := m.Rows[k]
		fmt.Printf("  key=%s\n", k)
		for _, col := range []string{"id", "field", "slk", "index", "category", "displayname", "type", "data", "minval", "maxval", "useunit", "usehero", "usebuilding", "useitem", "usespecific"} {
			if v, ok := row[col]; ok && v != "" {
				fmt.Printf("    %-12s = %q\n", col, v)
			}
		}
		fmt.Println()
	}
}
