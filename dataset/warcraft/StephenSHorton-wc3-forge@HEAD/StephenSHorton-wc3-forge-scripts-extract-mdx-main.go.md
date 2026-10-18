---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "scripts/extract-mdx/main.go"
ref: HEAD
---

// Diagnostic helper — extract specific MDX files from the local CASC install
// so we can inspect their bytes/test them against parsers in isolation.
//
// Usage: go run ./scripts/extract-mdx <path-in-casc> [<path-in-casc> ...]
// Outputs into ./tmp-mdx/ next to the working dir.
package main

import (
	"fmt"
	"os"
	"path/filepath"
	"strings"

	"github.com/StephenSHorton/wc3-forge/internal/casc"
)

func main() {
	if len(os.Args) < 2 {
		fmt.Fprintln(os.Stderr, "usage: extract-mdx <path-in-casc> [<path-in-casc> ...]")
		os.Exit(1)
	}
	install := `C:\Program Files (x86)\Warcraft III\_retail_`
	stor, err := casc.Open(install)
	if err != nil {
		fmt.Fprintln(os.Stderr, "open:", err)
		os.Exit(1)
	}
	defer stor.Close()
	if err := os.MkdirAll("tmp-mdx", 0o755); err != nil {
		fmt.Fprintln(os.Stderr, "mkdir:", err)
		os.Exit(1)
	}
	for _, p := range os.Args[1:] {
		data, ok, err := stor.ReadFile(p)
		if err != nil || !ok {
			fmt.Fprintf(os.Stderr, "%s: ok=%v err=%v\n", p, ok, err)
			continue
		}
		out := filepath.Join("tmp-mdx", strings.ReplaceAll(strings.ReplaceAll(p, "\\", "_"), "/", "_"))
		if err := os.WriteFile(out, data, 0o644); err != nil {
			fmt.Fprintf(os.Stderr, "%s write: %v\n", p, err)
			continue
		}
		fmt.Printf("ok: %s -> %s (%d bytes)\n", p, out, len(data))
	}
}
