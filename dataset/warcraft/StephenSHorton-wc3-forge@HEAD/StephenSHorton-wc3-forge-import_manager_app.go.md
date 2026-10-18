---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "import_manager_app.go"
ref: HEAD
---

package main

import (
	"fmt"
	"os"
	"path/filepath"
	"strings"

	"github.com/StephenSHorton/wc3-forge/internal/forge"
	"github.com/wailsapp/wails/v2/pkg/runtime"
)

// import_manager_app.go holds the Wails App bindings for the general Import
// Manager panel (frontend/src/ImportManager.svelte). They are thin adapters
// over the forge.Session mutators in internal/forge/imports_mutate.go — the
// SAME methods the imports.* MCP handlers call — so both surfaces share one
// implementation and one dirty/event path.
//
// The "add" flow mirrors ConvertAndImportModel: the Go method opens the native
// OS file picker, reads the chosen file's bytes, derives a default in-archive
// path (war3mapImported\<basename> unless the caller overrides), and registers
// it. A cancelled dialog returns an empty ImportEntry + nil error (the same
// empty-path cancel convention OpenMapFileDialog / ConvertAndImportModel use).

// ListImports returns every file registered in war3map.imp (path, flag, size,
// presence). Empty slice when no map is loaded or the map ships no imports —
// the panel renders a clean empty list. Mirrors the imports.list MCP accessor.
func (a *App) ListImports() []forge.ImportEntry {
	return forge.Current.ListImports()
}

// AddImportFile presents an OS file picker for ANY file and imports it into the
// loaded map under war3mapImported\<basename> (registering it in war3map.imp).
// Returns the resulting ImportEntry, or a zero entry (empty Path) + nil error
// when the user cancels the dialog. The dirty/entity-changed notifications fire
// from the forge mutator, which the App's startup() listeners forward to the
// frontend, so no explicit emit is needed here.
//
// targetDir lets the caller place the file somewhere other than the default
// war3mapImported\ (e.g. "" for the default, or a stock path like
// "ReplaceableTextures\\..." to override a stock texture). The chosen file's
// basename is appended to targetDir.
func (a *App) AddImportFile(targetDir string) (forge.ImportEntry, error) {
	srcPath, err := runtime.OpenFileDialog(a.ctx, runtime.OpenDialogOptions{
		Title: "Select a file to import (icon, loading screen, sound, texture, …)",
		Filters: []runtime.FileFilter{
			{DisplayName: "All files (*.*)", Pattern: "*.*"},
		},
	})
	if err != nil {
		return forge.ImportEntry{}, err
	}
	if srcPath == "" {
		return forge.ImportEntry{}, nil // cancelled
	}
	data, err := os.ReadFile(srcPath)
	if err != nil {
		return forge.ImportEntry{}, fmt.Errorf("read %s: %w", srcPath, err)
	}
	importPath := importPathFor(targetDir, filepath.Base(srcPath))
	written, err := forge.Current.AddImportFile(importPath, data)
	if err != nil {
		return forge.ImportEntry{}, err
	}
	return forge.ImportEntry{Path: written, Size: len(data), Present: true}, nil
}

// RemoveImport drops the imported file at path (and its war3map.imp entry).
// Mirrors the imports.remove MCP verb.
func (a *App) RemoveImport(path string) error {
	return forge.Current.RemoveImport(path)
}

// RenameImport moves an imported file from oldPath to newPath in the archive +
// the import table. Mirrors the imports.rename MCP verb.
func (a *App) RenameImport(oldPath, newPath string) error {
	return forge.Current.RenameImport(oldPath, newPath)
}

// importPathFor builds the in-archive path for a freshly-picked file. An empty
// dir defaults to the World-Editor convention (war3mapImported\). A non-empty
// dir is used verbatim (trailing slash tolerated) so callers can target stock
// override paths. Always uses the WC3 backslash separator; the forge mutator
// re-validates + normalizes.
func importPathFor(dir, base string) string {
	d := strings.TrimSpace(dir)
	if d == "" {
		d = `war3mapImported`
	}
	d = strings.TrimRight(strings.ReplaceAll(d, "/", `\`), `\`)
	return d + `\` + base
}
