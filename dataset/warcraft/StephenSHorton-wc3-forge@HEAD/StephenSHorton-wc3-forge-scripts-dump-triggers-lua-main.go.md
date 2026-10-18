---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "scripts/dump-triggers-lua/main.go"
ref: HEAD
---

// Diagnostic: open a .w3x via forge.Session, generate war3map.lua via the
// Phase-3 codegen, and write the result to stdout (or `-out path`). Used
// for smoke-testing the trigger Lua emitter on real maps without
// launching the editor.
//
// Usage: go run ./scripts/dump-triggers-lua "<path/to/Map.w3x>" [out.lua]
package main

import (
	"fmt"
	"os"

	"github.com/StephenSHorton/wc3-forge/internal/forge"
)

func main() {
	if len(os.Args) < 2 {
		fmt.Fprintln(os.Stderr, "usage: dump-triggers-lua <map.w3x> [out.lua]")
		os.Exit(2)
	}
	mapPath := os.Args[1]
	outPath := ""
	if len(os.Args) >= 3 {
		outPath = os.Args[2]
	}
	if err := forge.Current.Open(mapPath); err != nil {
		fmt.Fprintln(os.Stderr, "open:", err)
		os.Exit(1)
	}
	text, err := forge.Current.GenerateTriggerScript()
	if err != nil {
		fmt.Fprintln(os.Stderr, "generate:", err)
		os.Exit(1)
	}
	if outPath == "" {
		fmt.Println(text)
	} else {
		if err := os.WriteFile(outPath, []byte(text), 0o644); err != nil {
			fmt.Fprintln(os.Stderr, "write:", err)
			os.Exit(1)
		}
		fmt.Fprintf(os.Stderr, "wrote %d bytes to %s\n", len(text), outPath)
	}
}
