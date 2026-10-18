---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "cmd/dumptriggerdata/main.go"
ref: HEAD
---

// dumptriggerdata extracts UI/TriggerData.txt and the en-US triggerstrings.txt
// from the local WC3 CASC install, writing them to
// internal/formats/wtg/data/. One-off bootstrap tool used to vendor the
// snapshot the wtg parser uses for ECA argc resolution; rerun whenever a
// future WC3 patch is suspected to have changed the vocabulary.
package main

import (
	"fmt"
	"os"
	"path/filepath"

	"github.com/StephenSHorton/wc3-forge/internal/casc"
)

func main() {
	installPath := os.Getenv("WC3FORGE_WC3_PATH")
	if installPath == "" {
		installPath = `C:\Program Files (x86)\Warcraft III`
	}
	outDir := "internal/formats/wtg/data"
	if len(os.Args) > 1 {
		outDir = os.Args[1]
	}
	if err := os.MkdirAll(outDir, 0o755); err != nil {
		fmt.Println("mkdir:", err)
		os.Exit(1)
	}

	storage, err := casc.Open(installPath)
	if err != nil {
		fmt.Println("CASC open:", err)
		os.Exit(1)
	}
	defer storage.Close()

	tries := []struct {
		cascPath string
		outName  string
	}{
		{"ui/triggerdata.txt", "triggerdata.txt"},
		{"_locales/enus.w3mod/ui/triggerstrings.txt", "triggerstrings.txt"},
	}
	for _, t := range tries {
		data, ok, err := storage.ReadFile(t.cascPath)
		if err != nil || !ok {
			fmt.Printf("read %s: ok=%v err=%v — trying alt path\n", t.cascPath, ok, err)
		}
		if !ok {
			// fallback alt path attempt
			alt := t.cascPath
			if t.cascPath == "_locales/enus.w3mod/ui/triggerstrings.txt" {
				alt = "ui/triggerstrings.txt"
			}
			data, ok, err = storage.ReadFile(alt)
			if err != nil || !ok {
				fmt.Printf("read %s: ok=%v err=%v\n", alt, ok, err)
				continue
			}
		}
		dest := filepath.Join(outDir, t.outName)
		if err := os.WriteFile(dest, data, 0o644); err != nil {
			fmt.Printf("write %s: %v\n", dest, err)
			continue
		}
		fmt.Printf("wrote %s (%d bytes)\n", dest, len(data))
	}
}
