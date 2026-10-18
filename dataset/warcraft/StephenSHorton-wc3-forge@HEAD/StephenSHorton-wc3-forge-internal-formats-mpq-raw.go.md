---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/mpq/raw.go"
ref: HEAD
---

// Raw block copy-through support for LOSSLESS repacking.
//
// The plain reader (mpq.go) looks files up by NAME, decrypts, and decompresses.
// That's perfect for editing a file — but useless for the many files a WC3 map
// stores that have NO name we can resolve: custom-imported models, textures,
// and sounds under war3mapImported/ that aren't enumerated by any (listfile)
// and aren't in the standard WC3 set. The hash table never stores filenames, so
// a name-based repack would silently DROP every such file and corrupt the map.
//
// RawEntries solves that by exposing, for every PRESENT hash slot, the file's
// on-disk identity (hashA/hashB/locale/platform) plus its raw stored block bytes
// verbatim — still compressed/encrypted exactly as they sit on disk. The writer
// (write.go) can re-emit those bytes byte-for-byte at a fresh offset and place
// the hash entry at the slot derived from (hashA, hashB), WITHOUT ever knowing
// the filename. This is precisely how StormLib/HiveWE preserve files they don't
// rewrite.
//
// The one case raw copy-through CANNOT preserve safely is a block whose
// encryption key is FILE-OFFSET dependent (FLAG_KEY_ADJUSTED, 0x00020000): the
// engine recomputes its decryption key from the block's file-relative offset, so
// relocating the raw bytes to a new offset would break FIX_KEY decryption — and
// we can't re-key it without the basename we don't have. RawEntry.NeedsName
// flags that case so the caller can fall back gracefully rather than emit a
// broken archive. WC3 maps virtually never hit this (their files are plain or
// merely FLAG_ENCRYPTED, whose key is offset-independent).

package mpq

// RawEntry is one physically-present file pulled out of the archive verbatim,
// identified only by its hash-table coordinates (no filename required).
type RawEntry struct {
	// Slot is the entry's index in the SOURCE hash table. The lossless writer
	// preserves the source hash-table size and re-places each raw entry at this
	// exact slot, so name lookup (which forward-probes from lookupHash & mask)
	// still resolves it without us ever knowing the name or its lookup hash.
	Slot uint32

	// HashA/HashB are the two name-verification hashes from the hash table
	// slot. The reader matches on these (plus reaching the slot via forward
	// probing). Preserved verbatim.
	HashA uint32
	HashB uint32

	// Locale/Platform preserve the slot's localisation tag (0/0 for the
	// neutral files WC3 maps use). Carried through so a localised variant
	// keeps its tag.
	Locale   uint16
	Platform uint16

	// Flags is the block-table flags word, copied unchanged so the engine
	// decompresses/decrypts the re-emitted bytes exactly as before.
	Flags uint32

	// USize is the decompressed (logical) file size. FileSize in the spec.
	USize uint32

	// Offset is the entry's ORIGINAL file-relative block offset (relative to
	// the MPQ header, NOT the file start). The lossless writer pins entries
	// that need it (see NeedsName / FLAG_KEY_ADJUSTED) back to this exact
	// offset so their offset-dependent decryption key stays valid.
	Offset uint32

	// Raw is the on-disk stored block bytes (compressed/encrypted as-is),
	// copied directly from the source at the block's offset for cSize bytes.
	// The writer emits these verbatim.
	Raw []byte

	// NeedsName is true when this entry uses position-dependent encryption
	// (FLAG_KEY_ADJUSTED): its decryption key is bound to the block's
	// file-relative offset, so the raw bytes cannot be relocated without
	// re-keying — which requires the filename we don't have. A lossless
	// copy-through writer should refuse rather than relocate such an entry.
	NeedsName bool
}

// RawEntries returns one RawEntry per physically-present, non-deleted file in
// the archive, read straight from disk at each block's offset (NOT decompressed,
// NOT decrypted). The result lets a writer reproduce the archive's entire file
// set losslessly, including files whose names are unknown.
//
// Slots that don't reference a present file (never-used, deleted, out-of-range
// block index, non-file or delete-marker flags) are skipped, mirroring
// PresentFileCount's accounting exactly. Empty files (uSize==0) are included
// with a zero-length Raw slice — the writer reproduces them as empty blocks.
//
// Each RawEntry owns its own copy of the bytes (a fresh slice per entry), so the
// caller may retain them after the Archive is closed.
func (a *Archive) RawEntries() ([]RawEntry, error) {
	out := make([]RawEntry, 0, len(a.blockTable))
	for slot, he := range a.hashTable {
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

		var raw []byte
		if be.uSize != 0 && be.cSize != 0 {
			raw = make([]byte, be.cSize)
			off := a.header.archiveOffset + int64(be.offset)
			if _, err := a.src.ReadAt(raw, off); err != nil {
				return nil, err
			}
		} else {
			raw = []byte{}
		}

		out = append(out, RawEntry{
			Slot:      uint32(slot),
			HashA:     he.hashA,
			HashB:     he.hashB,
			Locale:    he.locale,
			Platform:  he.platform,
			Flags:     be.flags,
			USize:     be.uSize,
			Offset:    be.offset,
			Raw:       raw,
			NeedsName: be.flags&flagKeyAdjusted != 0,
		})
	}
	return out, nil
}

// HashTableSize returns the number of slots in the archive's hash table (a
// power of two). The lossless writer reuses this exact size so it can place
// each RawEntry at its original Slot index, keeping forward-probe lookups valid
// for files whose names it doesn't know.
func (a *Archive) HashTableSize() uint32 {
	return uint32(len(a.hashTable))
}

// SectorSizeShift returns the archive's logical sector-size shift (sectorSize ==
// 512 << shift). The lossless writer MUST reproduce this exact value: copied-
// through multi-sector files carry a sector-offset table whose layout is keyed
// to the source sector size, so a writer that defaulted to a different shift
// would make the reader miscount sectors and read those files as garbage.
func (a *Archive) SectorSizeShift() uint16 {
	return a.header.sectorSizeShift
}

// HashName exposes the three Storm name hashes (lookup index, verifier A,
// verifier B) for a storage path. The lossless flush uses this to match a
// pending edit (whose NAME it knows) against a RawEntry (whose name it does not)
// by comparing the verifier hashes (HashA/HashB). Exposed here rather than as a
// raw call to hashString so callers stay within the package's public surface.
func HashName(name string) (lookup, a, b uint32) {
	name = normalizePath(name)
	return hashString(name, hashOffset), hashString(name, hashNameA), hashString(name, hashNameB)
}
