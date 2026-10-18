---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/mpq/write.go"
ref: HEAD
---

// Pure-Go MPQ archive WRITER.
//
// The rest of this package is a read-only reader (see mpq.go). This file adds
// the inverse: given a set of (name -> bytes) entries, produce a valid MPQ v0
// archive that THIS package's own reader — and, by construction, StormLib /
// the WC3 engine — can open and extract byte-identically.
//
// Scope + design choices (deliberately conservative; correctness over size):
//
//   - Format version 0 (the original MPQ layout WC3 maps use). No HET/BET,
//     no v2+ 64-bit table offsets. The whole archive fits well under 4 GiB
//     for any real WC3 map, so 32-bit offsets are safe.
//   - Files are stored UNCOMPRESSED and UNENCRYPTED. The block flags carry
//     FLAG_FILE | FLAG_SINGLE_UNIT only. This is the simplest layout the
//     reader's readBlock handles: a single-unit, non-compressed, non-encrypted
//     block whose on-disk size == uncompressed size, read verbatim. Storing
//     uncompressed is explicitly acceptable per the task; zlib is a future
//     bonus and is intentionally NOT done here to avoid any chance of a
//     subtle sector-table bug corrupting a map.
//   - The hash table and block table ARE encrypted with the well-known keys
//     ("(hash table)" / "(block table)"), because the reader unconditionally
//     decrypts them (see tables.go readHashTable/readBlockTable). That's the
//     ONE place we need the inverse cipher — encryptBlock below.
//   - We always include a "(listfile)" so other tools (and our own List())
//     enumerate the contents without relying on the standard-name fallback.
//
// On-disk layout produced (offsets relative to file start; archiveOffset==0,
// no HM3W preheader — see note in Write):
//
//	[ MPQ header        ] 32 bytes at offset 0
//	[ file data sectors ] each file's bytes, concatenated, in insertion order
//	[ hash table        ] hashTableEntries * 16 bytes, encrypted
//	[ block table       ] blockTableEntries * 16 bytes, encrypted
//
// archiveSize in the header covers header..end-of-block-table.
//
// WC3 maps normally carry a 512-byte HM3W preheader before the MPQ header.
// The reader auto-detects HM3W and scans onward, but a fresh archive written
// here does NOT prepend one: StormLib and the engine open a bare MPQ (magic at
// offset 0) just fine, and the lobby preview that HM3W carries is reconstructed
// from war3map.w3i by the engine. Preserving an existing HM3W block on
// re-save is the caller's job (see WriteFilePreservingHeader) — they pass the
// original 512-byte block through verbatim.

package mpq

import (
	"bytes"
	"encoding/binary"
	"fmt"
	"os"
	"path/filepath"
	"sort"
)

// FileEntry is one file to store in the archive.
type FileEntry struct {
	// Name is the storage path as the engine will look it up, e.g.
	// "war3map.w3i" or "war3map\\foo". Forward slashes are normalised to
	// backslashes before hashing (matching the reader). Case-insensitive.
	Name string
	// Data is the file's raw (uncompressed) bytes.
	Data []byte
}

// encryptBlock is the inverse of decryptBlock. The Storm cipher is NOT
// symmetric (decryptBlock feeds the *plaintext* word forward into seed2; on
// encrypt we feed the plaintext forward BEFORE we overwrite it with the
// ciphertext). Operates in place on 4-byte little-endian words; len(data) must
// be a multiple of 4 (callers — table writers — always satisfy this).
//
// Cross-check: encryptBlock(decryptBlock(x)) and decryptBlock(encryptBlock(x))
// are both identity. write_test.go asserts this round-trip on random data.
func encryptBlock(data []byte, key uint32) {
	seed1 := key
	seed2 := uint32(0xEEEEEEEE)
	for i := 0; i+4 <= len(data); i += 4 {
		seed2 += cryptTable[0x400+(seed1&0xFF)]
		// Plaintext word.
		plain := uint32(data[i]) |
			uint32(data[i+1])<<8 |
			uint32(data[i+2])<<16 |
			uint32(data[i+3])<<24
		// Ciphertext word.
		ch := plain ^ (seed1 + seed2)
		seed1 = ((^seed1 << 0x15) + 0x11111111) | (seed1 >> 0x0B)
		// seed2 advances using the PLAINTEXT, exactly as the decrypt path
		// uses its recovered plaintext. This is what makes the two inverse.
		seed2 = plain + seed2 + (seed2 << 5) + 3
		data[i] = byte(ch)
		data[i+1] = byte(ch >> 8)
		data[i+2] = byte(ch >> 16)
		data[i+3] = byte(ch >> 24)
	}
}

// hashTableSize returns the power-of-two hash-table capacity for n files plus
// the (listfile). StormLib grows the table to the next power of two that is at
// least 2x the file count, with a floor of 4. We keep a generous floor of 16
// so collisions are rare for the small WC3 map file set.
func hashTableSize(nFiles int) uint32 {
	// +1 for the (listfile) entry we always add.
	want := uint32(nFiles+1) * 2
	size := uint32(16)
	for size < want {
		size <<= 1
	}
	return size
}

const (
	// listfileName is the canonical internal listfile path.
	listfileName = "(listfile)"
	// headerSizeV0 is the size of a format-0 MPQ header.
	headerSizeV0 = 32
	// blockSizeShift gives 4096-byte logical sectors (512 << 3). WC3's
	// conventional value. Irrelevant to single-unit storage but written to
	// the header for fidelity.
	blockSizeShift = 3
)

// Build serialises the given files into a complete in-memory MPQ archive and
// returns the bytes. Files are stored uncompressed + unencrypted, single-unit.
// A "(listfile)" enumerating every name is appended automatically; any caller-
// supplied "(listfile)" entry is ignored in favour of the generated one.
//
// Determinism: files are emitted in the caller's slice order (stable); the
// listfile lists them in that same order. Duplicate names (case-insensitive,
// slash-normalised) are rejected — the hash table can't hold two entries that
// resolve to the same key chain reliably for our purposes, and a map with two
// "war3map.w3i" is a bug upstream we want to surface loudly.
func Build(files []FileEntry) ([]byte, error) {
	// Reject duplicates + the reserved listfile name (we generate it).
	seen := make(map[string]struct{}, len(files))
	var content []FileEntry
	for _, f := range files {
		norm := normalizePath(f.Name)
		if norm == "" {
			return nil, fmt.Errorf("mpq: empty file name in entry set")
		}
		key := upperASCII(norm)
		if key == upperASCII(listfileName) {
			// Skip a caller-provided listfile; we synthesise our own.
			continue
		}
		if _, dup := seen[key]; dup {
			return nil, fmt.Errorf("mpq: duplicate file name %q (case/slash-insensitive)", f.Name)
		}
		seen[key] = struct{}{}
		content = append(content, FileEntry{Name: norm, Data: f.Data})
	}

	// Build the (listfile): one normalised name per line, CRLF separated
	// (StormLib's convention). Order matches content order for determinism.
	var lf bytes.Buffer
	for _, f := range content {
		lf.WriteString(f.Name)
		lf.WriteString("\r\n")
	}
	lf.WriteString(listfileName)
	lf.WriteString("\r\n")

	// Final entry list: content + the listfile.
	entries := make([]FileEntry, 0, len(content)+1)
	entries = append(entries, content...)
	entries = append(entries, FileEntry{Name: listfileName, Data: lf.Bytes()})

	nFiles := len(entries)
	hashCount := hashTableSize(len(content))
	blockCount := uint32(nFiles)

	// --- Layout pass ---------------------------------------------------
	// File data starts right after the header. Each file occupies exactly
	// len(Data) bytes (single-unit, uncompressed). We track each entry's
	// file-relative offset for the block table.
	//
	// Empty files (len==0) are valid; the reader short-circuits on uSize==0
	// before any I/O, so a zero-length block at any offset is fine.
	blocks := make([]blockEntry, nFiles)
	dataOffset := uint32(headerSizeV0)
	var dataSection bytes.Buffer
	for i, e := range entries {
		size := uint32(len(e.Data))
		blocks[i] = blockEntry{
			offset: dataOffset,
			cSize:  size,
			uSize:  size,
			// FLAG_FILE | FLAG_SINGLE_UNIT. No compression, no encryption.
			// readBlock treats this as: single sector, on-disk size ==
			// expanded size -> stored raw -> verbatim copy.
			flags: flagFile | flagSingleUnit,
		}
		dataSection.Write(e.Data)
		dataOffset += size
	}

	hashTableOffset := dataOffset
	blockTableOffset := hashTableOffset + hashCount*16
	archiveEnd := blockTableOffset + blockCount*16

	// --- Hash table ----------------------------------------------------
	hashTable := make([]hashEntry, hashCount)
	for i := range hashTable {
		hashTable[i] = hashEntry{
			hashA:      0xFFFFFFFF,
			hashB:      0xFFFFFFFF,
			locale:     0,
			platform:   0,
			blockIndex: emptyNeverUsed,
		}
	}
	mask := hashCount - 1
	for blockIdx, e := range entries {
		start := hashString(e.Name, hashOffset) & mask
		hA := hashString(e.Name, hashNameA)
		hB := hashString(e.Name, hashNameB)
		// Open-addressed insert: walk forward to the first never-used slot.
		placed := false
		idx := start
		for laps := uint32(0); laps <= mask; laps++ {
			if hashTable[idx].blockIndex == emptyNeverUsed {
				hashTable[idx] = hashEntry{
					hashA:      hA,
					hashB:      hB,
					locale:     0,
					platform:   0,
					blockIndex: uint32(blockIdx),
				}
				placed = true
				break
			}
			idx = (idx + 1) & mask
		}
		if !placed {
			// Should be impossible — hashCount >= 2*(nFiles) — but fail
			// loudly rather than silently drop a file.
			return nil, fmt.Errorf("mpq: hash table full inserting %q (cap=%d, files=%d)", e.Name, hashCount, nFiles)
		}
	}

	// --- Serialise + encrypt the two tables ----------------------------
	hashBytes := make([]byte, hashCount*16)
	for i, he := range hashTable {
		b := hashBytes[i*16:]
		binary.LittleEndian.PutUint32(b[0:4], he.hashA)
		binary.LittleEndian.PutUint32(b[4:8], he.hashB)
		binary.LittleEndian.PutUint16(b[8:10], he.locale)
		binary.LittleEndian.PutUint16(b[10:12], he.platform)
		binary.LittleEndian.PutUint32(b[12:16], he.blockIndex)
	}
	encryptBlock(hashBytes, hashTableKey)

	blockBytes := make([]byte, blockCount*16)
	for i, be := range blocks {
		b := blockBytes[i*16:]
		binary.LittleEndian.PutUint32(b[0:4], be.offset)
		binary.LittleEndian.PutUint32(b[4:8], be.cSize)
		binary.LittleEndian.PutUint32(b[8:12], be.uSize)
		binary.LittleEndian.PutUint32(b[12:16], be.flags)
	}
	encryptBlock(blockBytes, blockTableKey)

	// --- Header --------------------------------------------------------
	header := make([]byte, headerSizeV0)
	copy(header[0:4], magicMPQ[:])
	binary.LittleEndian.PutUint32(header[4:8], headerSizeV0)
	binary.LittleEndian.PutUint32(header[8:12], archiveEnd) // archiveSize
	binary.LittleEndian.PutUint16(header[12:14], 0)         // formatVersion = 0
	binary.LittleEndian.PutUint16(header[14:16], blockSizeShift)
	binary.LittleEndian.PutUint32(header[16:20], hashTableOffset)
	binary.LittleEndian.PutUint32(header[20:24], blockTableOffset)
	binary.LittleEndian.PutUint32(header[24:28], hashCount)
	binary.LittleEndian.PutUint32(header[28:32], blockCount)

	// --- Assemble ------------------------------------------------------
	out := make([]byte, 0, int(archiveEnd))
	out = append(out, header...)
	out = append(out, dataSection.Bytes()...)
	out = append(out, hashBytes...)
	out = append(out, blockBytes...)

	if uint32(len(out)) != archiveEnd {
		return nil, fmt.Errorf("mpq: internal layout error: produced %d bytes, expected %d", len(out), archiveEnd)
	}
	return out, nil
}

// WriteFile builds an archive from files and writes it to path. The write is
// atomic: bytes go to a sibling temp file first, then os.Rename replaces the
// destination. A failure at any step leaves the original file untouched.
//
// preHeader, if non-nil, is prepended verbatim before the MPQ header — pass
// the original 512-byte HM3W block from a re-saved WC3 map to preserve the
// lobby preview. When preHeader is supplied the MPQ table offsets are still
// computed relative to the MPQ header (which is what archiveOffset captures on
// read), so the archive remains valid at its shifted position.
func WriteFile(path string, files []FileEntry, preHeader []byte) (err error) {
	body, err := Build(files)
	if err != nil {
		return err
	}
	return writeAtomic(path, preHeader, body)
}

// writeAtomic prepends preHeader (if any) to body and writes the result to path
// atomically: bytes go to a sibling temp file, fsync, then os.Rename over the
// destination. On any failure the original file is left byte-for-byte untouched
// and the temp file is removed. Shared by WriteFile and WriteFileLossless.
func writeAtomic(path string, preHeader, body []byte) (err error) {
	var full []byte
	if len(preHeader) > 0 {
		full = make([]byte, 0, len(preHeader)+len(body))
		full = append(full, preHeader...)
		full = append(full, body...)
	} else {
		full = body
	}

	dir := filepath.Dir(path)
	tmp, err := os.CreateTemp(dir, ".mpqwrite-*.tmp")
	if err != nil {
		return fmt.Errorf("mpq: create temp file: %w", err)
	}
	tmpName := tmp.Name()
	// Clean up the temp file on any error path. On success it's been renamed
	// away so the Remove is a harmless no-op.
	defer func() {
		if err != nil {
			tmp.Close()
			os.Remove(tmpName)
		}
	}()

	if _, werr := tmp.Write(full); werr != nil {
		return fmt.Errorf("mpq: write temp file: %w", werr)
	}
	if serr := tmp.Sync(); serr != nil {
		return fmt.Errorf("mpq: sync temp file: %w", serr)
	}
	if cerr := tmp.Close(); cerr != nil {
		return fmt.Errorf("mpq: close temp file: %w", cerr)
	}

	if rerr := os.Rename(tmpName, path); rerr != nil {
		// On Windows, Rename onto an existing file can fail if the target is
		// open elsewhere. Surface it; the temp file is cleaned by the defer.
		err = fmt.Errorf("mpq: atomic replace %q: %w", path, rerr)
		return err
	}
	return nil
}

// upperASCII folds a string the way the Storm hash does (ASCII lowercase ->
// uppercase, '/' -> '\\'), giving a case/slash-insensitive comparison key.
// Used only for duplicate detection in Build — hashing itself folds internally.
func upperASCII(s string) string {
	b := []byte(s)
	for i := range b {
		b[i] = asciiToUpper[b[i]]
	}
	return string(b)
}

// ArchiveOffset returns the byte offset within the underlying file at which
// the MPQ header begins. For a bare .mpq this is 0; for a WC3 .w3x/.w3m with
// an HM3W preheader it is 512 (the preheader size). A repacking caller uses
// this to copy the original preheader bytes through verbatim so the lobby
// preview survives a re-save.
func (a *Archive) ArchiveOffset() int64 {
	return a.header.archiveOffset
}

// SortedNames is a small helper for tests/callers that want a deterministic
// name ordering independent of insertion order.
func SortedNames(files []FileEntry) []string {
	names := make([]string, len(files))
	for i, f := range files {
		names[i] = f.Name
	}
	sort.Strings(names)
	return names
}
