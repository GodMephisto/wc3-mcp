---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "scripts/find-doodad/main.go"
ref: HEAD
---

// Diagnostic: find every doodad/destructible row whose display name (after
// SkinStrings / unitSkin merges + WESTRING resolution) matches a substring.
// Prints fourcc, mdx file stem, num_var, max_pitch — enough for the probe
// pipeline to then extract the MDX and inspect sequences.
//
// Usage: go run ./scripts/find-doodad <name-substring>
package main

import (
	"fmt"
	"os"
	"path"
	"strconv"
	"strings"

	"github.com/StephenSHorton/wc3-forge/internal/casc"
	"github.com/StephenSHorton/wc3-forge/internal/formats/slk"
	"github.com/StephenSHorton/wc3-forge/internal/formats/wesstrings"
	"github.com/StephenSHorton/wc3-forge/internal/formats/wts"
)

func main() {
	if len(os.Args) < 2 {
		fmt.Fprintln(os.Stderr, "usage: find-doodad <name-substring>")
		os.Exit(1)
	}
	needle := strings.ToLower(os.Args[1])

	install := `C:\Program Files (x86)\Warcraft III\_retail_`
	stor, err := casc.Open(install)
	if err != nil {
		fmt.Fprintln(os.Stderr, "open casc:", err)
		os.Exit(1)
	}
	defer stor.Close()

	read := func(name string) ([]byte, bool) {
		clean := strings.ToLower(path.Clean(strings.ReplaceAll(name, "\\", "/")))
		data, ok, err := stor.ReadFile(clean)
		if err != nil || !ok {
			return nil, false
		}
		return data, true
	}

	doodads := slk.New()
	for _, n := range []string{"Doodads/Doodads.slk", "Units/DestructableData.slk"} {
		if d, ok := read(n); ok {
			if err := doodads.Load(d); err != nil {
				fmt.Fprintf(os.Stderr, "parse %s: %v\n", n, err)
			}
		}
	}
	for _, n := range []string{"Doodads/doodadSkins.txt", "Units/destructableSkin.txt"} {
		if d, ok := read(n); ok {
			if err := doodads.LoadINI(d); err != nil {
				fmt.Fprintf(os.Stderr, "parse %s: %v\n", n, err)
			}
		}
	}

	wes := wesstrings.New()
	for _, n := range []string{"UI/WorldEditStrings.txt", "UI/WorldEditGameStrings.txt"} {
		if d, ok := read(n); ok {
			if err := wes.Load(d); err != nil {
				fmt.Fprintf(os.Stderr, "parse %s: %v\n", n, err)
			}
		}
	}

	resolve := func(raw string) string {
		if raw == "" {
			return ""
		}
		v := raw
		if strings.HasPrefix(v, "WESTRING_") {
			v = wes.Resolve(v)
		}
		return strings.TrimSpace(wts.StripColorCodes(v))
	}

	hits := 0
	for id, row := range doodads.Rows {
		name := resolve(row["name"])
		if name == "" {
			continue
		}
		if !strings.Contains(strings.ToLower(name), needle) {
			continue
		}
		file := row["file"]
		numVar := row["numvar"]
		maxPitch := row["maxpitch"]
		maxRoll := row["maxroll"]
		fixedRot := row["fixedrot"]
		// emit
		nv, _ := strconv.Atoi(strings.TrimSpace(numVar))
		fmt.Printf("id=%-4s name=%q\n  file=%s\n  num_var=%d max_pitch=%s max_roll=%s fixed_rot=%s\n",
			id, name, file, nv, maxPitch, maxRoll, fixedRot)
		hits++
	}
	if hits == 0 {
		fmt.Println("no matches")
	}
}
