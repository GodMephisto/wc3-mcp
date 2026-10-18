---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/mapfile_ops.go"
ref: HEAD
---

package forge

import (
	"errors"
	"fmt"
	"os"
	"path"
	"path/filepath"
	"strings"
)

// mapfile_ops.go adds the on-the-wire "persist a map somewhere new" operations
// that complete the agent loop: SaveAs (export the current map to a fresh .w3x)
// and ExtractToFolder (unpack an MPQ-backed .w3x into the editable folder
// workflow). Map CREATION (map.new) lives in package main because it needs the
// CASC tileset palette + minimap bake; see new_map_bridge.go.

// SaveAs flushes any in-memory edits to the current source, then writes the
// current map as a standalone .w3x at newPath and repoints the session to it
// (subsequent saves go to newPath). newPath must end in .w3x or .w3m.
//
// Folder-backed sessions package the (now-flushed) folder via PackageFolderToW3X;
// MPQ-backed sessions copy the (now-repacked) source archive. Reopening newPath
// guarantees the session reflects exactly what was written. Not undoable — this
// is a file-system operation, not an entity mutation.
func (s *Session) SaveAs(newPath string) error {
	newPath = strings.TrimSpace(newPath)
	if newPath == "" {
		return errors.New("save_as: path is required")
	}
	abs, err := filepath.Abs(newPath)
	if err != nil {
		return fmt.Errorf("save_as: %w", err)
	}
	newPath = abs
	if ext := strings.ToLower(filepath.Ext(newPath)); ext != ".w3x" && ext != ".w3m" {
		return fmt.Errorf("save_as: path must end in .w3x or .w3m (got %q)", filepath.Base(newPath))
	}

	// Flush in-memory edits to the CURRENT source first so the export captures
	// the latest state. (Save is a no-op write for a clean folder map and a
	// repack for an MPQ map.)
	if err := s.Save(); err != nil {
		return fmt.Errorf("save_as: flush current map: %w", err)
	}

	s.mu.RLock()
	loaded := s.loaded
	curPath := s.path
	_, isFolder := s.source.(folderSource)
	s.mu.RUnlock()
	if !loaded {
		return errors.New("save_as: no map loaded")
	}

	if pathsEqual(curPath, newPath) {
		// Saving over the same .w3x — the flush above already did it.
		return nil
	}
	if dir := filepath.Dir(newPath); dir != "" {
		if err := os.MkdirAll(dir, 0o755); err != nil {
			return fmt.Errorf("save_as: %w", err)
		}
	}

	if isFolder {
		if err := s.PackageFolderToW3X(newPath); err != nil {
			return fmt.Errorf("save_as: package folder to .w3x: %w", err)
		}
	} else {
		if err := copyFile(curPath, newPath); err != nil {
			return fmt.Errorf("save_as: copy archive: %w", err)
		}
	}

	// Repoint the session to the freshly-written file (Save As semantics).
	if err := s.Open(newPath); err != nil {
		return fmt.Errorf("save_as: wrote %s but failed to reopen it: %w", newPath, err)
	}
	return nil
}

// ExtractToFolder unpacks the currently-loaded MPQ-backed map into a NEW folder
// at destDir (which must not already exist), writing every listed archive file
// (pending edits included, since reads shadow the archive) as loose files. After
// extraction the agent can map.open(destDir) to switch to the folder workflow.
//
// Only listed files are extractable — unnamed/unlisted blocks (rare custom
// imports with no listfile entry) can't be written to a named path; this matches
// every other folder-extraction tool. Errors if the session is already
// folder-backed (nothing to extract).
func (s *Session) ExtractToFolder(destDir string) error {
	destDir = strings.TrimSpace(destDir)
	if destDir == "" {
		return errors.New("extract_to_folder: path is required")
	}
	abs, err := filepath.Abs(destDir)
	if err != nil {
		return fmt.Errorf("extract_to_folder: %w", err)
	}
	destDir = abs

	s.mu.RLock()
	loaded := s.loaded
	mp, isMPQ := s.source.(*mpqSource)
	src := s.source
	s.mu.RUnlock()
	if !loaded {
		return errors.New("extract_to_folder: no map loaded")
	}
	if !isMPQ {
		return errors.New("extract_to_folder: map is already folder-backed (nothing to extract)")
	}

	// Don't clobber an existing destination — extraction should land in a fresh
	// folder so it can't mingle with unrelated files.
	if _, statErr := os.Stat(destDir); statErr == nil {
		return fmt.Errorf("extract_to_folder: destination already exists: %s", destDir)
	} else if !errors.Is(statErr, os.ErrNotExist) {
		return fmt.Errorf("extract_to_folder: %w", statErr)
	}
	if err := os.MkdirAll(destDir, 0o755); err != nil {
		return fmt.Errorf("extract_to_folder: %w", err)
	}

	written := 0
	for _, name := range mp.listNames() {
		// Skip MPQ internal files — they're archive metadata regenerated on
		// repack, not map content, and "(listfile)" isn't a valid filename.
		if strings.HasPrefix(name, "(") {
			continue
		}
		data, ok, readErr := src.read(name)
		if readErr != nil {
			return fmt.Errorf("extract_to_folder: read %q: %w", name, readErr)
		}
		if !ok {
			continue
		}
		rel := filepath.FromSlash(path.Clean(strings.ReplaceAll(name, `\`, "/")))
		if rel == "." || rel == ".." || strings.HasPrefix(rel, "..") || filepath.IsAbs(rel) {
			continue // defensive: never escape destDir
		}
		dst := filepath.Join(destDir, rel)
		if dir := filepath.Dir(dst); dir != "" {
			if err := os.MkdirAll(dir, 0o755); err != nil {
				return fmt.Errorf("extract_to_folder: mkdir %q: %w", dir, err)
			}
		}
		if err := os.WriteFile(dst, data, 0o644); err != nil {
			return fmt.Errorf("extract_to_folder: write %q: %w", rel, err)
		}
		written++
	}
	if written == 0 {
		return errors.New("extract_to_folder: archive has no listable files to extract (missing (listfile)?)")
	}
	return nil
}

// listNames returns the archive's listed file names minus tombstoned ones,
// taken under the source lock. Pending-only additions (a brand-new file written
// but absent from the listfile) are omitted; edits to listed files are captured
// because ExtractToFolder reads each name through the pending-aware read().
func (m *mpqSource) listNames() []string {
	m.mu.Lock()
	defer m.mu.Unlock()
	if m.a == nil {
		return nil
	}
	all := m.a.List()
	out := make([]string, 0, len(all))
	for _, n := range all {
		if m.deleted[mpqNameKey(n)] {
			continue
		}
		out = append(out, n)
	}
	return out
}

// (SaveAs reuses the package-level copyFile from triggers_convert.go for the
// MPQ-backed case, whose source archive is already a complete .w3x after the
// pre-flush repack.)

// pathsEqual compares two filesystem paths for equality, case-insensitively
// (Windows paths are case-insensitive; this is a safe superset elsewhere for the
// "saving over myself" short-circuit).
func pathsEqual(a, b string) bool {
	return strings.EqualFold(filepath.Clean(a), filepath.Clean(b))
}
