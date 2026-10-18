---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/imports_mutate.go"
ref: HEAD
---

package forge

import (
	"fmt"
	"path"
	"strings"

	"github.com/StephenSHorton/wc3-forge/internal/formats/imp"
)

// imports_mutate.go holds the general Import Manager surface — add/remove/
// rename/list of ARBITRARY files in the loaded map's archive, registered in
// war3map.imp via the existing imp encoder. It generalizes what ImportModel
// (handlers_models.go) does for one converted model+textures: write the bytes
// through s.source, keep s.imp in lockstep, and let Save flush the table.
//
// NOT undoable — same rationale as ImportModel (see its doc comment): for a
// folder-backed map the bytes hit disk immediately, and rolling an import back
// would mean deleting files off the user's disk, which the editor deliberately
// doesn't do. These are file-system operations, not per-field map edits, so
// they live outside the recordCommand undo stack just like ImportModel. The
// dirty flag (dirtyImports) gates the war3map.imp re-encode at Save; the byte
// files themselves are written eagerly (folder) or buffered-then-flushed (MPQ)
// by the source, exactly as ImportModel does.
//
// CONTRACT (wire methods, handled in handlers_imports.go):
//   imports.list    {} -> {imports: [ImportEntry, ...]}
//   imports.add     {path, data_base64} -> {path}
//   imports.remove  {path} -> {ok}
//   imports.rename  {old_path, new_path} -> {ok}

// ImportEntry is one row of the war3map.imp import table as surfaced to the
// GUI Import Manager + the imports.list MCP accessor. Flag is the path-kind
// byte (see internal/formats/imp); Size is the on-source byte length (best
// effort — 0 + present=false when the file can't currently be read, e.g. an
// orphaned table entry whose backing file was removed out of band).
type ImportEntry struct {
	Path    string `json:"path"`
	Flag    uint8  `json:"flag"`
	Size    int    `json:"size"`
	Present bool   `json:"present"`
}

// ensureImpLocked lazily parses war3map.imp into s.imp (or starts a fresh v1
// table when the map ships none). Idempotent — a no-op once s.imp is set.
// Caller MUST hold s.mu. Shared by ImportModel and the Import Manager mutators
// so the lazy-parse logic lives in one place.
func (s *Session) ensureImpLocked(src fileSource) error {
	if s.imp != nil {
		return nil
	}
	if data, ok, err := src.read("war3map.imp"); err != nil {
		return fmt.Errorf("read war3map.imp: %w", err)
	} else if ok {
		parsed, perr := imp.Parse(data)
		if perr != nil {
			return fmt.Errorf("parse war3map.imp: %w", perr)
		}
		s.imp = parsed
	} else {
		s.imp = &imp.File{Version: 1}
	}
	return nil
}

// ListImports returns every file registered in war3map.imp, enriched with the
// on-source size when readable. Returns an empty slice (not nil) when no map is
// loaded or the map ships no import table — the GUI panel renders a clean empty
// list rather than an error. The lazy parse runs here too so the panel sees the
// real table even before the first add/remove this session.
func (s *Session) ListImports() []ImportEntry {
	s.mu.Lock()
	if !s.loaded || s.source == nil {
		s.mu.Unlock()
		return []ImportEntry{}
	}
	src := s.source
	if err := s.ensureImpLocked(src); err != nil {
		// A malformed import table is surfaced as "no imports" rather than a
		// panic; the user can still add new ones (which allocates a fresh table
		// only when s.imp is nil — here it stays nil so a later add reparses).
		s.mu.Unlock()
		return []ImportEntry{}
	}
	entries := make([]imp.Entry, len(s.imp.Entries))
	copy(entries, s.imp.Entries)
	s.mu.Unlock()

	out := make([]ImportEntry, 0, len(entries))
	for _, e := range entries {
		row := ImportEntry{Path: e.Path, Flag: e.Flag}
		// Size/presence is a best-effort read OUTSIDE the lock (ReadFile takes
		// its own locks). An orphaned table entry (file removed out of band)
		// reports present=false rather than failing the whole listing.
		if data, ok, err := s.ReadFile(e.Path); err == nil && ok {
			row.Size = len(data)
			row.Present = true
		}
		out = append(out, row)
	}
	return out
}

// AddImportFile writes an arbitrary file into the loaded map's archive/folder
// source and registers its path in war3map.imp. This is the general-purpose
// sibling of ImportModel: where ImportModel takes converted MDX+BLP bytes, this
// takes ANY bytes under ANY in-archive path (a custom icon BLP, a loading
// screen, a sound, a replacement texture at a stock path, …).
//
// importPath is the in-archive path the file lands at (forward or back slashes
// accepted; normalized to the WC3 backslash convention for the table). It must
// be a relative path — absolute / drive-letter / traversal paths are rejected
// by the source's write guard, surfaced here as an error before any state
// change. Empty data is allowed (a zero-byte placeholder is a valid import).
//
// Re-adding an already-registered path overwrites the bytes and leaves the
// single table entry intact (imp.Add is idempotent). Returns the normalized
// in-archive path the file was written under. NOT undoable (see file header).
func (s *Session) AddImportFile(importPath string, data []byte) (string, error) {
	norm, err := validateImportPath(importPath)
	if err != nil {
		return "", err
	}

	s.mu.Lock()
	if !s.loaded {
		s.mu.Unlock()
		return "", fmt.Errorf("no map loaded")
	}
	src := s.source
	if src == nil {
		s.mu.Unlock()
		return "", fmt.Errorf("no source for writing")
	}
	if err := s.ensureImpLocked(src); err != nil {
		s.mu.Unlock()
		return "", err
	}
	if err := src.write(norm, data); err != nil {
		s.mu.Unlock()
		return "", fmt.Errorf("write %s: %w", norm, err)
	}
	s.imp.Add(norm)

	wasDirty := s.anyDirtyLocked()
	s.dirtyImports = true
	nowDirty := s.anyDirtyLocked()
	s.mu.Unlock()

	// This is a DIRECT write bypassing Save's batch commit. Imports normally land
	// under war3mapImported\ (not baselined), but the surface accepts any relative
	// path — if a caller writes one that collides with a baselined war3map.* name,
	// refresh its stamp so the next Save doesn't read our own write back as an
	// external change. Folder-only; a no-op for MPQ. See atomic_save.go.
	s.refreshBaselineEntry(src, norm)
	if !wasDirty && nowDirty {
		s.notifyDirty(true)
	}
	s.notifyEntityChanged(EntityChange{Kind: "import", ID: 0, Field: "added"})
	return norm, nil
}

// RemoveImport drops a file from the loaded map's source AND from war3map.imp.
// The path comparison is case- and slash-insensitive (WC3 archive convention).
// Removing a path that isn't in the table is still attempted on the source
// (so an orphaned on-disk file can be cleaned up); the call only errors if the
// source delete itself fails. NOT undoable (see file header).
func (s *Session) RemoveImport(importPath string) error {
	norm, err := validateImportPath(importPath)
	if err != nil {
		return err
	}

	s.mu.Lock()
	if !s.loaded {
		s.mu.Unlock()
		return fmt.Errorf("no map loaded")
	}
	src := s.source
	if src == nil {
		s.mu.Unlock()
		return fmt.Errorf("no source for writing")
	}
	if err := s.ensureImpLocked(src); err != nil {
		s.mu.Unlock()
		return err
	}
	if err := src.delete(norm); err != nil {
		s.mu.Unlock()
		return fmt.Errorf("delete %s: %w", norm, err)
	}
	removed := s.imp.Remove(norm)

	wasDirty := s.anyDirtyLocked()
	s.dirtyImports = true
	s.mu.Unlock()

	// Direct delete bypassing Save's batch commit: drop any baseline stamp for
	// the now-absent name so a later checkStaleness doesn't read a recorded-but-
	// gone file as an external delete. Folder-only; no-op for MPQ.
	s.refreshBaselineEntry(src, norm)
	if !wasDirty {
		s.notifyDirty(true)
	}
	s.notifyEntityChanged(EntityChange{Kind: "import", ID: 0, Field: "removed"})
	_ = removed // table removal is best-effort; source delete is the gate.
	return nil
}

// RenameImport moves an imported file from oldPath to newPath in the source and
// updates its war3map.imp entry (preserving the original flag byte). Both paths
// are validated + normalized. The old file's bytes are read, written under the
// new path, then the old file is deleted — so a folder map's on-disk layout
// matches the table. Errors (without mutating) when the old file isn't present.
// NOT undoable (see file header).
func (s *Session) RenameImport(oldPath, newPath string) error {
	oldNorm, err := validateImportPath(oldPath)
	if err != nil {
		return fmt.Errorf("old_path: %w", err)
	}
	newNorm, err := validateImportPath(newPath)
	if err != nil {
		return fmt.Errorf("new_path: %w", err)
	}
	if canonImportPath(oldNorm) == canonImportPath(newNorm) {
		return nil // no-op rename
	}

	s.mu.Lock()
	if !s.loaded {
		s.mu.Unlock()
		return fmt.Errorf("no map loaded")
	}
	src := s.source
	if src == nil {
		s.mu.Unlock()
		return fmt.Errorf("no source for writing")
	}
	if err := s.ensureImpLocked(src); err != nil {
		s.mu.Unlock()
		return err
	}
	data, ok, rerr := src.read(oldNorm)
	if rerr != nil {
		s.mu.Unlock()
		return fmt.Errorf("read %s: %w", oldNorm, rerr)
	}
	if !ok {
		s.mu.Unlock()
		return fmt.Errorf("no imported file at %q", oldNorm)
	}
	if err := src.write(newNorm, data); err != nil {
		s.mu.Unlock()
		return fmt.Errorf("write %s: %w", newNorm, err)
	}
	if err := src.delete(oldNorm); err != nil {
		s.mu.Unlock()
		return fmt.Errorf("delete %s: %w", oldNorm, err)
	}
	// Update the table: rename if the entry existed, else register the new
	// path (an on-source file with no table row — defensive).
	if !s.imp.Rename(oldNorm, newNorm) {
		s.imp.Add(newNorm)
	}

	wasDirty := s.anyDirtyLocked()
	s.dirtyImports = true
	s.mu.Unlock()

	// Direct write+delete bypassing Save's batch commit: stamp the new name (now
	// present) and drop the old (now gone) so neither trips the next save's
	// change detection. Folder-only; no-op for MPQ.
	s.refreshBaselineEntry(src, newNorm)
	s.refreshBaselineEntry(src, oldNorm)
	if !wasDirty {
		s.notifyDirty(true)
	}
	s.notifyEntityChanged(EntityChange{Kind: "import", ID: 0, Field: "renamed"})
	return nil
}

// validateImportPath normalizes a caller-supplied import path to the WC3
// backslash convention and rejects the shapes the source's write guard would
// reject (absolute, drive-letter, traversal) — failing the mutator up-front so
// a bad path never partially mutates state. An empty path is rejected. Mirrors
// folderSource.write's traversal defense so the same string is rejected on both
// folder and MPQ sources regardless of host OS.
func validateImportPath(p string) (string, error) {
	if strings.TrimSpace(p) == "" {
		return "", fmt.Errorf("import path is required")
	}
	norm := strings.ReplaceAll(p, `/`, `\`)
	// Reuse path.Clean's traversal collapse on the forward-slash form (the same
	// check folderSource.write does) so "subdir\..\..\evil" and "C:\x" fail.
	clean := path.Clean(strings.ReplaceAll(norm, `\`, "/"))
	switch {
	case strings.HasPrefix(clean, "/"),
		clean == "..",
		strings.HasPrefix(clean, "../"),
		strings.Contains(clean, "/.."),
		len(clean) >= 2 && clean[1] == ':':
		return "", fmt.Errorf("unsafe import path %q (must be a relative in-archive path)", p)
	}
	return norm, nil
}

// canonImportPath folds a path to the comparison key WC3 archive paths use
// (backslashes → forward slashes, lowercased) — matching the imp package's
// internal canonicalization so the rename no-op check agrees with imp.Has/
// imp.Rename's matching.
func canonImportPath(p string) string {
	return strings.ToLower(strings.ReplaceAll(p, `\`, "/"))
}
