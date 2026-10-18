---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/mpq_write_source.go"
ref: HEAD
---

package forge

import (
	"fmt"
	"log"
	"os"
	"strings"

	"github.com/StephenSHorton/wc3-forge/internal/formats/mpq"
)

// This file holds the MPQ-backed save path: how a *mpqSource (defined in
// session.go) turns its buffered pending writes + tombstones into a freshly
// repacked .w3x on disk. Kept out of session.go's body to limit churn in that
// hot file while several agents edit it in parallel.

// mpqNameKey produces the case/slash-insensitive comparison key the buffered
// pending/deleted maps are keyed by. Mirrors the reader's normalisation
// (forward slash -> backslash, ASCII upper-fold) so "war3map.w3i" and
// "WAR3MAP.W3I" address the same buffered entry. The original (un-folded) name
// is preserved separately for re-packing — see flush, which tracks display
// names alongside.
func mpqNameKey(name string) string {
	name = strings.TrimSpace(name)
	name = strings.ReplaceAll(name, "/", "\\")
	return strings.ToUpper(name)
}

// editInfo is one buffered edit (a pending write or a tombstone) hashed for
// matching against the source's unnamed RawEntries. The repack matches an edit
// to a raw entry by comparing the two verifier hashes (HashA/HashB), since the
// raw entries carry no filename.
type editInfo struct {
	hA, hB  uint32
	name    string // display name for the new archive + listfile
	data    []byte
	deleted bool
}

// repackCountCheck computes the file count the lossless repack EXPECTS to write
// and reports whether the count we're actually about to write diverges.
//
// expected = source blocks (minus the source listfile, which the writer
// regenerates) + brand-new pending files (an edit with no matching raw entry,
// net +1 each) - tombstones that matched a raw entry (net -1 each). A pending
// edit that REPLACES a matched raw entry is net 0 (dropped from keep, re-added
// via named). srcBlockCount is returned for the diagnostic log line.
//
// mismatch is true when written != expected. The DO-NO-HARM caller refuses the
// save only on the DROP direction (written < expected) — the 240->14 import-
// loss class — and keeps warn-and-write for a (non-lossy) count increase. This
// is a pure function over the same hash bookkeeping the copy-through used, so it
// is unit-testable with a synthetic rawEntries/edits set.
func repackCountCheck(rawEntries []mpq.RawEntry, edits []editInfo, written int, lfA, lfB uint32) (expected, srcBlockCount int, mismatch bool) {
	for _, re := range rawEntries {
		if re.HashA == lfA && re.HashB == lfB {
			continue // source listfile — not a user file, writer regenerates
		}
		srcBlockCount++
	}
	expected = srcBlockCount
	for i := range edits {
		matched := false
		for _, re := range rawEntries {
			if edits[i].hA == re.HashA && edits[i].hB == re.HashB {
				matched = true
				break
			}
		}
		switch {
		case edits[i].deleted && matched:
			expected-- // tombstone removed an existing file
		case !edits[i].deleted && !matched:
			expected++ // brand-new file
		}
	}
	return expected, srcBlockCount, written != expected
}

// flush repacks the entire archive and atomically replaces the .w3x on disk.
//
// Why repack-the-world rather than splice individual files: the MPQ writer in
// internal/formats/mpq builds a fresh, self-consistent archive (header + hash
// table + block table + data) from a flat name->bytes set. Splicing one file
// into an existing archive in place would mean rewriting the hash/block tables
// AND every offset anyway, so a clean rebuild is both simpler and safer.
//
// Source of the file set:
//   - Every name the open archive declares (a.List() — the union of its
//     internal (listfile) and the standard WC3 name set, filtered to names
//     that actually resolve).
//   - Overlaid with this source's pending writes (new/changed bytes).
//   - Minus this source's tombstones (deleted files).
//
// The original HM3W/user-data preheader (the bytes before the MPQ header) is
// copied through verbatim so the lobby preview and any custom shunt survive.
//
// Atomicity + DO-NO-HARM: the repack writes to a sibling temp file and renames
// over the original only on full success (mpq.WriteFile handles this). If ANY
// step fails the original .w3x is left byte-for-byte untouched and the buffered
// changes stay pending so a later flush can retry. On success the buffers are
// cleared and the in-process archive handle is reopened so subsequent reads see
// the committed bytes.
//
// flush takes the source lock and delegates to flushLocked. forceRepackAll also
// holds the lock while it pre-populates the buffer, so the actual repack work
// lives in flushLocked (which assumes the lock is already held — sync.Mutex is
// not reentrant).
func (m *mpqSource) flush() error {
	if m == nil {
		return fmt.Errorf("mpq flush: nil source")
	}
	m.mu.Lock()
	defer m.mu.Unlock()
	return m.flushLocked(false)
}

// flushLocked is the repack body. Caller MUST hold m.mu. When force is true the
// archive is repacked even with no buffered edits (a pure lossless copy-through)
// — used by forceRepackAll for the clean-save path.
func (m *mpqSource) flushLocked(force bool) error {
	// Fast path: nothing buffered means the on-disk archive already matches
	// the in-memory view; no repack needed (unless force-repacking).
	if !force && len(m.pending) == 0 && len(m.deleted) == 0 {
		return nil
	}
	if m.path == "" {
		return fmt.Errorf("%w: mpq source has no path to write to", ErrMPQRepackFailed)
	}
	if m.a == nil {
		return fmt.Errorf("%w: mpq source has no open archive to repack", ErrMPQRepackFailed)
	}

	// LOSSLESS REPACK (raw block copy-through). MPQ hash tables don't store
	// filenames, so a name-based rebuild would SILENTLY DROP every file the
	// editor can't name — most custom imports: models, textures, sounds,
	// war3mapImported/* (observed: a 240-file custom .w3x shrinking to 14).
	//
	// Instead we start from the source's RawEntries (every PRESENT block's
	// identity + on-disk stored bytes, copied verbatim) and overlay this
	// session's edits:
	//   - a pending edit replaces the matching raw entry (matched by name-hash)
	//     with fresh, uncompressed bytes stored as a NAMED entry;
	//   - a tombstone drops the matching raw entry;
	//   - a brand-new pending file with no source match is added as a named
	//     entry.
	// Unmatched raw entries pass through byte-for-byte. The writer regenerates
	// the (listfile), so the source listfile entry is dropped from copy-through.
	rawEntries, err := m.a.RawEntries()
	if err != nil {
		return fmt.Errorf("%w: read raw entries for lossless repack of %q: %v", ErrMPQRepackFailed, m.path, err)
	}

	// Hash the (listfile) name once so we can recognise + drop the source copy.
	_, lfA, lfB := mpq.HashName("(listfile)")

	// Index this session's edits by name-hash so we can match them against the
	// unnamed raw entries. Each pending/tombstone key is a normalised name; we
	// re-hash it with the reader's algorithm (via mpq.HashName) and compare to
	// each raw entry's HashA/HashB.
	edits := make([]editInfo, 0, len(m.pending)+len(m.deleted))
	for key, data := range m.pending {
		name := displayNameForKey(key)
		_, hA, hB := mpq.HashName(name)
		edits = append(edits, editInfo{hA: hA, hB: hB, name: name, data: append([]byte(nil), data...)})
	}
	for key := range m.deleted {
		name := displayNameForKey(key)
		_, hA, hB := mpq.HashName(name)
		edits = append(edits, editInfo{hA: hA, hB: hB, name: name, deleted: true})
	}

	// Pass 1: copy-through. Keep every raw entry that is NOT the source
	// listfile, NOT replaced by a pending edit, and NOT tombstoned. (Matching is
	// by name-hash: a pending/tombstone whose hashes equal the raw entry's
	// addresses the same file.)
	keep := make([]mpq.RawEntry, 0, len(rawEntries))
	for _, re := range rawEntries {
		if re.HashA == lfA && re.HashB == lfB {
			continue // source (listfile) — writer regenerates
		}
		drop := false
		for i := range edits {
			if edits[i].hA == re.HashA && edits[i].hB == re.HashB {
				drop = true // replaced (pending) or removed (tombstone)
				break
			}
		}
		if !drop {
			keep = append(keep, re)
		}
	}

	// Pass 2: named entries = every pending edit (whether it replaced a source
	// entry or is brand new). Tombstones produce no named entry. The source
	// listfile is regenerated by the writer, so a pending edit naming
	// "(listfile)" is intentionally skipped (BuildLossless also guards this).
	named := make([]mpq.FileEntry, 0, len(edits))
	for i := range edits {
		if edits[i].deleted {
			continue
		}
		if edits[i].hA == lfA && edits[i].hB == lfB {
			continue
		}
		named = append(named, mpq.FileEntry{Name: edits[i].name, Data: edits[i].data})
	}

	if len(keep) == 0 && len(named) == 0 {
		return fmt.Errorf("%w: refusing to write an empty archive (no files collected)", ErrMPQRepackFailed)
	}

	// mapIO diagnostics + DO-NO-HARM guard for the historical import-drop class
	// (a 240-file custom .w3x silently repacking down to 14 —
	// feedback_wc3_forge_mpq_writer_listfile). repackCountCheck computes the file
	// count we EXPECT to write from the same hash-match bookkeeping the copy-
	// through used and compares it to what we're about to write (keep + named).
	written := len(keep) + len(named)
	expected, srcBlockCount, mismatch := repackCountCheck(rawEntries, edits, written, lfA, lfB)
	if mismatch {
		// Always bump the diagnostic counter so any mismatch stays observable in
		// diagnostics.get (preserves the U9 instrumentation contract).
		mapIODiag.repackFileMismatch.Add(1)
		log.Printf("[WARN] mpq flush %q: file-count mismatch (writing %d, expected %d from %d source blocks) — possible import drop",
			m.path, written, expected, srcBlockCount)
		// REFUSE-the-save on a file-count DROP. Writing fewer files than the
		// source declared is the 240->14 import-loss class: it would drop custom
		// imports (models/textures/sounds the editor can't name) and overwrite the
		// user's only copy with a lossy archive. We return BEFORE the destructive
		// rename (the archive handle is still open and the original .w3x is byte-
		// for-byte untouched), and we keep the buffered edits pending so the
		// session stays usable. This is the DO-NO-HARM floor: a lossy save is worse
		// than no save. A count INCREASE is not a loss, so we keep the historical
		// warn-and-write behaviour there (the counter above still records it).
		if written < expected {
			return fmt.Errorf("%w: refusing to save %q — the repack would write only %d of %d files, dropping %d custom import(s); your map's only copy is left untouched. Workaround: extract the map to a folder and save that instead",
				ErrMPQRepackFailed, m.path, written, expected, expected-written)
		}
	}

	// Capture the source's hash-table size + sector-size shift BEFORE closing —
	// the lossless writer must reproduce both (slot placement + sector layout).
	// Also note whether the source carried a (listfile) so a pure copy-through
	// preserves that property (and doesn't change the file count by +1).
	hashCount := m.a.HashTableSize()
	sectorShift := m.a.SectorSizeShift()
	emitListfile := m.a.Has("(listfile)")

	// Preserve the original preheader (HM3W / user-data shunt) verbatim. We
	// read it from disk NOW, while the archive handle is still open and the
	// file definitely exists, and stash the bytes in memory.
	var preHeader []byte
	if off := m.a.ArchiveOffset(); off > 0 {
		raw, rerr := os.ReadFile(m.path)
		if rerr != nil {
			return fmt.Errorf("read original preheader from %q: %w", m.path, rerr)
		}
		if int64(len(raw)) < off {
			return fmt.Errorf("original file %q shorter (%d) than its archive offset (%d)", m.path, len(raw), off)
		}
		preHeader = append([]byte(nil), raw[:off]...)
	}

	// CRITICAL (Windows): the reader holds an open os.File handle on m.path.
	// Windows refuses to os.Rename a temp file ONTO a path that has an open
	// handle ("Access is denied"). All file bytes are already collected into
	// memory above, so we can safely close the archive here, releasing the
	// handle, before WriteFileLossless's atomic rename. We reopen afterwards.
	//
	// DO NO HARM: WriteFileLossless writes to a sibling temp file and only
	// renames on full success. If it fails, m.path is untouched; we reopen the
	// (unchanged) original below so the session stays usable and the buffered
	// edits remain pending for a retry. The genuinely-unpreservable case
	// (e.g. a pinned key-adjusted block whose offset can't be honoured) surfaces
	// as a BuildLossless error wrapped in ErrMPQRepackFailed — the
	// graceful fallback the PRIME DIRECTIVE calls for.
	_ = m.a.Close()
	m.a = nil

	writeErr := mpq.WriteFileLossless(m.path, keep, named, hashCount, sectorShift, emitListfile, preHeader)

	// Reopen the archive (the new bytes on success, the untouched original on
	// failure) so subsequent reads have a live handle again.
	reopened, openErr := mpq.Open(m.path)
	if openErr == nil {
		m.a = reopened
	}

	if writeErr != nil {
		// Leave buffers in place for a retry. Wrap in ErrMPQRepackFailed
		// so the handler maps it to the graceful "extract to a folder" message
		// rather than a raw internal error — this is the DO-NO-HARM fallback for
		// the rare genuinely-unpreservable archive (e.g. a pinned key-adjusted
		// block whose offset can't be honoured). The original .w3x is untouched.
		return fmt.Errorf("%w: lossless repack of %q failed: %v", ErrMPQRepackFailed, m.path, writeErr)
	}
	if openErr != nil {
		// The repack succeeded on disk but we couldn't reopen — surface it so
		// the caller knows the in-process handle is stale (the file itself is
		// correct and a fresh Open of the map will work).
		return fmt.Errorf("repack %q succeeded but reopen failed: %w", m.path, openErr)
	}

	m.pending = make(map[string][]byte)
	m.deleted = make(map[string]bool)
	return nil
}

// forceRepackAll rewrites the archive even when nothing is buffered. Used by
// the clean-save path so a "Save" on an unedited .w3x still produces a real,
// freshly-packed archive at the source path (rather than a misleading no-op).
//
// With the lossless repack this is simply a force-flush: every present block is
// copied through verbatim (preserving unlisted custom imports and compression),
// overlaid with whatever edits happen to be buffered. The old approach (reading
// every NAMED file into pending) is gone — it would have dropped unnamed custom
// imports and needlessly decompressed every encrypted file. If the repack fails
// the original .w3x is left untouched (DO NO HARM).
func (m *mpqSource) forceRepackAll() error {
	if m == nil {
		return fmt.Errorf("%w: nil source", ErrMPQRepackFailed)
	}
	m.mu.Lock()
	defer m.mu.Unlock()
	if m.a == nil {
		return fmt.Errorf("%w: no archive to repack", ErrMPQRepackFailed)
	}
	return m.flushLocked(true)
}

// displayNameForKey renders a normalised (uppercase, backslash) name key back
// into a friendly storage path for files that originated as pending writes
// without a known original casing. We special-case the well-known war3map.*
// lowercase convention so a freshly-added script lists as "war3map.lua" rather
// than "WAR3MAP.LUA"; everything else keeps the key. Case is irrelevant to the
// engine (it folds on lookup) — this is purely cosmetic for the (listfile).
func displayNameForKey(key string) string {
	// The key uses backslash separators already.
	if strings.HasPrefix(key, "WAR3MAP") {
		return strings.ToLower(key)
	}
	return key
}
