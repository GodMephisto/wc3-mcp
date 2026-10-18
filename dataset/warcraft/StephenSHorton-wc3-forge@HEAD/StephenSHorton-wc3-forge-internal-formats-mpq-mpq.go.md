---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/mpq/mpq.go"
ref: HEAD
---

// Package mpq is a pure-Go read-only reader for Blizzard's MPQ (MoPaQ)
// archive format. It handles Warcraft III .w3x / .w3m maps and bare .mpq
// archives — anything that StormLib's MPQ v0/v1 path opens, with the
// exception of compressions other than ZLIB and PKWARE DCL Implode (see
// decompress.go for the gap list and rationale).
//
// What "read-only" means here: we parse the header, hash table, and block
// table; we look files up by name; we decrypt + decompress sectors; we
// return the bytes. We do NOT write, replace, append, or modify anything.
// Two consequences of that scope choice worth knowing:
//
//   - The HET/BET tables (introduced in MPQ v3 for Cataclysm) are not
//     parsed. If you ever encounter a WC3 file that uses them, Open will
//     reject the file with a clear error from header.go.
//   - We do NOT verify sector checksums even when FLAG_SECTOR_CRC is set.
//     The checksums sit in an extra entry at the end of the sector-offset
//     table; we skip past it cleanly but don't validate. In two decades
//     of MPQs in the wild there is no documented case of a checksum
//     mismatch on otherwise-readable data, and the overhead is real.
//
// Quick start:
//
//	a, err := mpq.Open(`C:\path\to\map.w3x`)
//	if err != nil { ... }
//	defer a.Close()
//	w3i, err := a.Read("war3map.w3i")
//
// Filename conventions match StormLib: case-insensitive, forward and
// backslashes equivalent. We normalise to backslash before hashing.
//
// References:
//   - Zezula: http://www.zezula.net/en/mpq/mpqformat.html
//   - StormLib (C++): https://github.com/ladislav-zezula/StormLib
//   - icza/mpq (Go, partial): structural inspiration for the read flow
//   - HiveWE scripts/extract_stormlib.py: the canonical WC3 file list
package mpq

import (
	"bytes"
	"encoding/binary"
	"errors"
	"fmt"
	"io"
	"os"
	"strings"
	"sync"
)

// standardWC3Files is the fallback name list when an archive has no
// "(listfile)". Mirrors HiveWE's scripts/extract_stormlib.py DEFAULT_FILES.
// Note we do NOT include "(listfile)" or "(attributes)" — those are
// metadata files the engine generates, not part of the map content.
var standardWC3Files = []string{
	"war3map.w3i",
	"war3map.w3e",
	"war3map.wpm",
	"war3map.shd",
	"war3map.doo",
	"war3mapUnits.doo",
	"war3map.w3a",
	"war3map.w3u",
	"war3map.w3t",
	"war3map.w3b",
	"war3map.w3d",
	"war3map.w3h",
	"war3map.w3q",
	"war3map.imp",
	"war3map.mmp",
	"war3map.w3c",
	"war3map.w3r",
	"war3map.w3s",
	"war3map.wtg",
	"war3map.wct",
	"war3map.wts",
	"war3map.j",
	"war3map.lua",
	"war3mapMap.blp",
	"war3mapPreview.tga",
}

// Archive is a read-only handle to an opened MPQ. Safe for concurrent
// reads from multiple goroutines (the underlying ReaderAt is consulted
// with absolute offsets; we keep no mutable cursor). NOT safe to use
// after Close.
type Archive struct {
	// Underlying source. file is non-nil only when Open created the
	// archive from a path — in which case Close shuts it. For
	// OpenReader-created archives we leave teardown to the caller.
	file *os.File
	src  io.ReaderAt
	size int64

	header     *mpqHeader
	hashTable  []hashEntry
	blockTable []blockEntry

	// listfile is the union of the standard WC3 file set and any names
	// parsed from an internal "(listfile)". Computed lazily by List() under
	// listMu so concurrent first-time callers don't race on the write.
	listMu        sync.Mutex
	listfileCache []string
}

// Open opens a .w3x / .w3m / .mpq file from path. The HM3W and user-data
// shunts are auto-detected.
func Open(path string) (*Archive, error) {
	f, err := os.Open(path)
	if err != nil {
		return nil, err
	}
	st, err := f.Stat()
	if err != nil {
		f.Close()
		return nil, err
	}
	a, err := OpenReader(f, st.Size())
	if err != nil {
		f.Close()
		return nil, err
	}
	a.file = f
	return a, nil
}

// OpenReader opens an MPQ from an existing ReaderAt + total size. The
// reader does not need to be positioned (it isn't — we use absolute
// offsets only). Both raw MPQ archives and HM3W-shunted WC3 maps are
// detected automatically.
func OpenReader(r io.ReaderAt, size int64) (*Archive, error) {
	if size < hm3wHeaderSize {
		return nil, errors.New("mpq: file too small to contain an MPQ archive")
	}
	h, err := findHeader(r, size)
	if err != nil {
		return nil, err
	}
	hash, err := readHashTable(r, h)
	if err != nil {
		return nil, err
	}
	block, err := readBlockTable(r, h)
	if err != nil {
		return nil, err
	}
	return &Archive{
		src:        r,
		size:       size,
		header:     h,
		hashTable:  hash,
		blockTable: block,
	}, nil
}

// Close releases resources. Safe to call multiple times. If the archive
// was constructed via OpenReader the underlying source is not touched.
func (a *Archive) Close() error {
	if a.file == nil {
		return nil
	}
	err := a.file.Close()
	a.file = nil
	return err
}

// normalizePath converts forward slashes to backslashes (the form Storm
// hashes against) and trims whitespace. Case is left alone — hashString
// folds case internally.
func normalizePath(name string) string {
	name = strings.TrimSpace(name)
	if strings.ContainsRune(name, '/') {
		name = strings.ReplaceAll(name, "/", "\\")
	}
	return name
}

// Has reports whether the archive contains a file with the given name.
// Case-insensitive; slash and backslash are equivalent.
func (a *Archive) Has(name string) bool {
	name = normalizePath(name)
	he, ok := findHashEntry(a.hashTable, name)
	if !ok {
		return false
	}
	if int(he.blockIndex) >= len(a.blockTable) {
		return false
	}
	be := a.blockTable[he.blockIndex]
	if be.flags&flagFile == 0 || be.flags&flagDeleteMark != 0 {
		return false
	}
	return true
}

// Read returns the decompressed contents of the named file.
func (a *Archive) Read(name string) ([]byte, error) {
	name = normalizePath(name)
	he, ok := findHashEntry(a.hashTable, name)
	if !ok {
		return nil, fmt.Errorf("mpq: file not found: %s", name)
	}
	if int(he.blockIndex) >= len(a.blockTable) {
		return nil, fmt.Errorf("mpq: file %s has bad block index %d (table size %d)", name, he.blockIndex, len(a.blockTable))
	}
	be := a.blockTable[he.blockIndex]
	if be.flags&flagFile == 0 {
		return nil, fmt.Errorf("mpq: %s: entry is not a file (flags=0x%08x)", name, be.flags)
	}
	if be.flags&flagDeleteMark != 0 {
		return nil, fmt.Errorf("mpq: %s: entry is a delete marker", name)
	}
	return a.readBlock(name, be)
}

// readBlock pulls the raw bytes for one file out of the archive,
// decompressing as needed.
//
// Logical flow (handles all combinations of single-unit vs sectored,
// compressed/imploded vs raw, encrypted vs plain):
//
//  1. Compute the file's encryption key (basename hash + maybe-adjust)
//     if it's encrypted.
//  2. Build the "sector offset table". For sectored files this is read
//     from disk (first 4*(nSectors+1) bytes of the block; +1 extra if
//     FLAG_SECTOR_CRC); for single-unit files we synthesise a 2-entry
//     [0, cSize] table.
//  3. For each sector: ReadAt the raw bytes, decrypt if needed,
//     decompress per flags, append to output.
func (a *Archive) readBlock(name string, be blockEntry) ([]byte, error) {
	// Empty file: no I/O needed regardless of flags.
	if be.uSize == 0 {
		return []byte{}, nil
	}

	blockOffset := a.header.archiveOffset + int64(be.offset)
	encrypted := be.flags&flagEncrypted != 0
	singleUnit := be.flags&flagSingleUnit != 0

	var key uint32
	if encrypted {
		key = fileKey(name)
		if be.flags&flagKeyAdjusted != 0 {
			key = adjustedKey(key, be.offset, be.uSize)
		}
	}

	sectorSize := a.header.sectorSize

	// Determine sector layout.
	var nSectors uint32
	if singleUnit {
		nSectors = 1
	} else {
		nSectors = (be.uSize + sectorSize - 1) / sectorSize
	}

	// Build sector offset table. For single-unit files we synthesise
	// it; for sectored compressed/imploded files we read it from disk;
	// for sectored uncompressed files we synthesise it (each sector is
	// exactly sectorSize except possibly the last).
	hasSectorTable := !singleUnit && (be.flags&(flagCompressed|flagImploded) != 0)

	var sectorOffsets []uint32
	if hasSectorTable {
		// Number of offset entries: (nSectors + 1) plus an extra if
		// FLAG_SECTOR_CRC is set (the extra trailing entry points to
		// the sector-checksum block at end-of-file).
		entries := nSectors + 1
		if be.flags&flagSectorCRC != 0 {
			entries++
		}
		tableBytes := make([]byte, entries*4)
		if _, err := a.src.ReadAt(tableBytes, blockOffset); err != nil {
			return nil, fmt.Errorf("mpq: %s: read sector offset table: %w", name, err)
		}
		if encrypted {
			// Sector offset table uses key - 1. From StormLib:
			// DecryptMpqBlock(SectorOffsets, ..., key - 1).
			decryptBlock(tableBytes, key-1)
		}
		sectorOffsets = make([]uint32, entries)
		for i := uint32(0); i < entries; i++ {
			sectorOffsets[i] = binary.LittleEndian.Uint32(tableBytes[i*4 : i*4+4])
		}
		// Sanity: first offset should equal the table size; if not the
		// archive is corrupt or we miscounted CRC presence. We don't
		// hard-fail because some packers omit CRC entry even with flag.
		if sectorOffsets[0] != entries*4 && sectorOffsets[0] != (entries-1)*4 {
			return nil, fmt.Errorf("mpq: %s: implausible first sector offset %d (table=%d entries)",
				name, sectorOffsets[0], entries)
		}
	} else {
		// Synthetic table: byte ranges of each sector within the block.
		sectorOffsets = make([]uint32, nSectors+1)
		if singleUnit {
			sectorOffsets[0] = 0
			sectorOffsets[1] = be.cSize
		} else {
			for i := uint32(0); i < nSectors; i++ {
				sectorOffsets[i] = i * sectorSize
			}
			sectorOffsets[nSectors] = be.cSize
		}
	}

	out := make([]byte, 0, be.uSize)
	var inBuf []byte

	for s := uint32(0); s < nSectors; s++ {
		// Expanded size of this sector.
		var expanded uint32
		if singleUnit {
			expanded = be.uSize
		} else if s == nSectors-1 {
			expanded = be.uSize - sectorSize*s
		} else {
			expanded = sectorSize
		}

		// On-disk byte range of this sector within the block.
		startOff := sectorOffsets[s]
		endOff := sectorOffsets[s+1]
		if endOff < startOff {
			return nil, fmt.Errorf("mpq: %s: sector %d has negative size (%d -> %d)", name, s, startOff, endOff)
		}
		rawLen := endOff - startOff

		// Read.
		if cap(inBuf) < int(rawLen) {
			inBuf = make([]byte, rawLen)
		} else {
			inBuf = inBuf[:rawLen]
		}
		if _, err := a.src.ReadAt(inBuf, blockOffset+int64(startOff)); err != nil {
			return nil, fmt.Errorf("mpq: %s: read sector %d: %w", name, s, err)
		}

		// Decrypt — each sector is keyed by (baseKey + s).
		if encrypted {
			// Sector data must be uint32-aligned for the cipher; trailing
			// bytes (if any) are left untouched by StormLib. In practice
			// sector cSize is always uint32-aligned for encrypted files.
			alignedLen := len(inBuf) &^ 3
			decryptBlock(inBuf[:alignedLen], key+s)
		}

		// Decompress.
		decoded, err := decompressSector(inBuf, int(expanded), be.flags)
		if err != nil {
			return nil, fmt.Errorf("mpq: %s: sector %d: %w", name, s, err)
		}
		out = append(out, decoded...)
	}

	if uint32(len(out)) != be.uSize {
		return nil, fmt.Errorf("mpq: %s: extracted %d bytes, expected %d", name, len(out), be.uSize)
	}
	return out, nil
}

// List returns the filenames the archive declares. The result is the
// union of:
//   - any names parsed from an internal "(listfile)" (if present)
//   - the standard WC3 file set
//
// Filtered to names that actually resolve in the hash table. Per the
// prompt's spec: maps without a (listfile) return ONLY the standard set,
// not the full block-table contents.
func (a *Archive) List() []string {
	a.listMu.Lock()
	defer a.listMu.Unlock()
	if a.listfileCache != nil {
		return a.listfileCache
	}

	seen := make(map[string]struct{})
	var out []string

	add := func(name string) {
		name = normalizePath(name)
		key := strings.ToUpper(name)
		if _, ok := seen[key]; ok {
			return
		}
		if !a.Has(name) {
			return
		}
		seen[key] = struct{}{}
		out = append(out, name)
	}

	if a.Has("(listfile)") {
		data, err := a.Read("(listfile)")
		if err == nil {
			// (listfile) is a CR/LF or LF-separated UTF-8 text file.
			for _, line := range bytes.FieldsFunc(data, func(r rune) bool {
				return r == '\r' || r == '\n' || r == ';'
			}) {
				add(string(line))
			}
		}
	}

	for _, n := range standardWC3Files {
		add(n)
	}
	a.listfileCache = out
	return out
}

// PresentFileCount returns the number of distinct files physically stored in
// the archive, counted from the hash table independent of whether their names
// are known. A slot counts when it references a present, non-deleted block
// (flagFile set, flagDeleteMark clear). MPQ hash tables don't store names, so
// this is the only way to know how many files an archive really holds when its
// (listfile) is absent or incomplete.
//
// It exists so the repack path can detect archives that contain more files
// than List() can name — a name-based repack would silently DROP those extra
// (typically custom-imported) files, corrupting the map. See the guard in
// internal/forge/mpq_write_source.go.
func (a *Archive) PresentFileCount() int {
	n := 0
	for _, he := range a.hashTable {
		if he.blockIndex == emptyNeverUsed || he.blockIndex == emptyDeleted {
			continue
		}
		if int(he.blockIndex) >= len(a.blockTable) {
			continue
		}
		be := a.blockTable[he.blockIndex]
		if be.flags&flagFile == 0 || be.flags&flagDeleteMark != 0 {
			continue
		}
		n++
	}
	return n
}
