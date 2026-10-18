---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/package_folder.go"
ref: HEAD
---

package forge

import (
	"encoding/binary"
	"fmt"
	"io/fs"
	"os"
	"path/filepath"
	"strings"

	"github.com/StephenSHorton/wc3-forge/internal/formats/mpq"
)

// hm3wHeaderSize is the fixed size of the HM3W preheader WC3 expects in front of
// the MPQ archive in a .w3x map file. The engine reads the lobby name + player
// count from this block; the full picture is reconstructed from war3map.w3i at
// load time, so a minimal-but-correct HM3W is enough to make the file load.
const hm3wHeaderSize = 512

// hm3wNameCap caps the embedded NUL-terminated map name so it can never run past
// the fixed 512-byte block (leaving room for the NUL terminator + padding).
const hm3wNameCap = 240

// sanitizeMapFileName turns a map display name into a filesystem-safe base name
// for the packaged .w3x. Path-illegal characters (anything in <>:"/\|?* plus
// control chars) are replaced with underscores; surrounding whitespace/dots are
// trimmed. If the result is empty (e.g. a name made entirely of illegal chars),
// the caller's fallback (the folder base name) should be used instead — callers
// pass the already-resolved name here, so we additionally fall back to "map".
func sanitizeMapFileName(name string) string {
	var b strings.Builder
	for _, r := range name {
		switch {
		case r < 0x20, r == 0x7f:
			b.WriteRune('_')
		case strings.ContainsRune(`<>:"/\|?*`, r):
			b.WriteRune('_')
		default:
			b.WriteRune(r)
		}
	}
	out := strings.Trim(b.String(), " .")
	if out == "" {
		return "map"
	}
	return out
}

// buildHM3WHeader synthesizes the 512-byte little-endian HM3W preheader from the
// map's display name + max-player count. Layout (empirically known to load):
//
//	offset 0:  "HM3W" (4 bytes)
//	offset 4:  uint32 = 0
//	offset 8:  uint32 flags = 0
//	offset 12: uint32 maxPlayers
//	offset 16: UTF-8 name bytes, NUL-terminated (name capped at 240 bytes)
//	rest:      NUL padding to 512
func buildHM3WHeader(name string, maxPlayers int) []byte {
	if maxPlayers < 1 {
		maxPlayers = 1
	}
	if maxPlayers > 24 {
		maxPlayers = 24
	}
	nameBytes := []byte(name)
	if len(nameBytes) > hm3wNameCap {
		nameBytes = nameBytes[:hm3wNameCap]
	}

	h := make([]byte, hm3wHeaderSize)
	copy(h[0:4], "HM3W")
	binary.LittleEndian.PutUint32(h[4:8], 0)
	binary.LittleEndian.PutUint32(h[8:12], 0) // flags
	binary.LittleEndian.PutUint32(h[12:16], uint32(maxPlayers))
	copy(h[16:], nameBytes)
	// h[16+len(nameBytes)] stays 0 (the NUL terminator); the rest is already
	// zero from make, so the block is NUL-padded to 512 with no extra work.
	return h
}

// PackageFolderToW3X packages the currently-loaded FOLDER-backed map into a
// standalone .w3x file at outPath so WC3 can load it via -loadfile (the engine
// can't open a loose folder). It walks the folder root, stores every regular
// file uncompressed into a fresh MPQ, and prepends a synthesized 512-byte HM3W
// preheader built from the map's display name + player count.
//
// It errors if the session is not folder-backed: MPQ-backed maps already have a
// launchable .w3x on disk and don't need packaging.
func (s *Session) PackageFolderToW3X(outPath string) error {
	s.mu.RLock()
	loaded := s.loaded
	src := s.source
	info := s.info
	s.mu.RUnlock()

	if !loaded {
		return fmt.Errorf("package folder: no map loaded")
	}
	fsrc, ok := src.(folderSource)
	if !ok {
		return fmt.Errorf("package folder: session is not folder-backed (only folder maps need packaging; MPQ maps already have a .w3x)")
	}
	root := fsrc.root
	if root == "" {
		return fmt.Errorf("package folder: empty folder root")
	}

	// Resolve outPath to an absolute path so the in-walk self-skip comparison is
	// reliable regardless of how the caller phrased it.
	absOut, err := filepath.Abs(outPath)
	if err != nil {
		return fmt.Errorf("package folder: resolve out path: %w", err)
	}

	// Derive the lobby name + player count for the HM3W block. Fall back to the
	// folder's base name if the map has no display name; default to 12 players
	// when the player slot count is unknown (matches WC3's common max).
	name := filepath.Base(root)
	maxPlayers := 12
	if info != nil {
		if info.Name != "" {
			name = info.Name
		}
		if n := len(info.Players); n > 0 {
			maxPlayers = n
		}
	}
	hm3w := buildHM3WHeader(name, maxPlayers)

	// Walk the folder root, collecting every regular file as a named MPQ entry.
	// Names are stored relative to root with backslash separators (the on-disk
	// MPQ path convention the engine looks files up by).
	var named []mpq.FileEntry
	walkErr := filepath.WalkDir(root, func(p string, d fs.DirEntry, err error) error {
		if err != nil {
			return err
		}
		if d.IsDir() {
			return nil
		}
		if !d.Type().IsRegular() {
			// Skip symlinks / devices / sockets — not packageable map content.
			return nil
		}
		// Defensive self-skip: outPath lives outside root by design (sibling
		// out/ dir), so this normally never triggers, but guard anyway in case a
		// caller points it inside the tree.
		if ap, aerr := filepath.Abs(p); aerr == nil && ap == absOut {
			return nil
		}
		rel, rerr := filepath.Rel(root, p)
		if rerr != nil {
			return fmt.Errorf("relativize %q: %w", p, rerr)
		}
		data, derr := os.ReadFile(p)
		if derr != nil {
			return fmt.Errorf("read %q: %w", p, derr)
		}
		named = append(named, mpq.FileEntry{
			Name: strings.ReplaceAll(rel, string(filepath.Separator), `\`),
			Data: data,
		})
		return nil
	})
	if walkErr != nil {
		return fmt.Errorf("package folder: walk %q: %w", root, walkErr)
	}
	if len(named) == 0 {
		return fmt.Errorf("package folder: no files found under %q", root)
	}

	// Ensure the output directory exists (the sibling out/ dir is created on
	// first Test Map of a fresh folder map).
	if dir := filepath.Dir(absOut); dir != "" {
		if err := os.MkdirAll(dir, 0o755); err != nil {
			return fmt.Errorf("package folder: mkdir %q: %w", dir, err)
		}
	}

	// keep=nil (no source archive to copy through), hashCount=0 (auto-size),
	// sectorSizeShift=0 (default 4096-byte sectors — fine for all-named single-
	// unit entries), emitListfile=true.
	if err := mpq.WriteFileLossless(absOut, nil, named, 0, 0, true, hm3w); err != nil {
		return fmt.Errorf("package folder: write %q: %w", absOut, err)
	}
	return nil
}
