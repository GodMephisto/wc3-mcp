---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "cmd/converterrcount/main.go"
ref: HEAD
---

// Command converterrcount is a headless measurement harness for the
// Convert-to-Lua pipeline. It opens a .w3x (or extracted map folder), runs the
// exact same read-only TranspilePreview() the convert dialog uses, and counts
// the inline `error("jass2lua failed` markers the emitter writes whenever it
// cannot translate a statement.
//
// The total it prints is the regression metric for cross-trigger vJASS work:
// the Enfo FFB test map sits at 514 on origin/main, and that number must not go
// up. The counting method mirrors aggregate.mjs (regex over each section's
// .Transpiled, summed across all sections) so the figure is comparable.
//
// Usage:
//
//	go run ./cmd/converterrcount <map.w3x|map-dir>
//	go run ./cmd/converterrcount -v <map.w3x>   # also print per-section breakdown
package main

import (
	"flag"
	"fmt"
	"os"
	"regexp"
	"sort"

	"github.com/StephenSHorton/wc3-forge/internal/forge"
)

// inlineRe matches the inline marker the transpiler emits for an untranslatable
// statement. Identical to aggregate.mjs's INLINE_RE so the count is comparable.
var inlineRe = regexp.MustCompile(`error\("jass2lua failed`)

func inlineCount(s string) int {
	return len(inlineRe.FindAllString(s, -1))
}

func main() {
	verbose := flag.Bool("v", false, "print per-section inline breakdown (descending)")
	flag.Parse()
	if flag.NArg() != 1 {
		fmt.Fprintln(os.Stderr, "usage: converterrcount [-v] <map.w3x|map-dir>")
		os.Exit(2)
	}
	path := flag.Arg(0)

	if err := forge.Current.Open(path); err != nil {
		fmt.Fprintf(os.Stderr, "open %q: %v\n", path, err)
		os.Exit(1)
	}

	preview, err := forge.Current.TranspilePreview()
	if err != nil {
		fmt.Fprintf(os.Stderr, "transpile preview: %v\n", err)
		os.Exit(1)
	}

	type row struct {
		id     int32
		label  string
		kind   string
		inline int
	}
	var rows []row
	totalInline := 0
	failingSections := 0
	for i := range preview.Sections {
		sec := &preview.Sections[i]
		// Markers are now stripped from .Transpiled and surfaced as structured
		// Diagnostics; fall back to the text scan for older payloads.
		n := len(sec.Diagnostics)
		if n == 0 {
			n = inlineCount(sec.Transpiled)
		}
		totalInline += n
		if n > 0 {
			failingSections++
		}
		rows = append(rows, row{id: sec.ID, label: sec.Label, kind: sec.Kind, inline: n})
	}

	if *verbose {
		sort.SliceStable(rows, func(a, b int) bool { return rows[a].inline > rows[b].inline })
		fmt.Println("=== per-section inline (descending, inline>0 only) ===")
		for _, r := range rows {
			if r.inline == 0 {
				break
			}
			fmt.Printf("  inline=%-4d kind=%-12s id=%-6d %q\n", r.inline, r.kind, r.id, r.label)
		}
	}

	fmt.Printf("total sections:       %d\n", len(preview.Sections))
	fmt.Printf("failing (inline>0):   %d\n", failingSections)
	fmt.Printf("TOTAL inline error(): %d\n", totalInline)
}
