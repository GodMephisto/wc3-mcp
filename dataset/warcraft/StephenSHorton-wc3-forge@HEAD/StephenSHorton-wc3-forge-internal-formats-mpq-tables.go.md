---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/mpq/tables.go"
ref: HEAD
---

// Hash and block tables.
//
// Both tables sit at fixed offsets given by the header. Each is encrypted
// with a hardcoded key derived from a well-known string ("(hash table)" and
// "(block table)" respectively); StormLib has had these constants memoised
// for 20 years and we just re-derive them at init.
//
// Hash table semantics:
//   - Size is a power of two, typically max(32, nextPow2(numFiles * 2)).
//   - File lookup: hash the name with hashOffset, modulo by table size,
//     scan forward from that index. Stop on FFFFFFFF (empty/never-used)
//     or after a full lap. Two further hashes (nameA, nameB) disambiguate
//     collisions — the table never stores the literal filename.
//   - FFFFFFFE means "deleted but search continues". For READ purposes
//     we just skip those entries.
//
// Block table semantics:
//   - Parallel array indexed by hashEntry.blockIndex.
//   - flags tells us whether this entry is a real file, whether it's
//     compressed, encrypted, single-sector, has sector checksums, etc.

package mpq

import (
	"encoding/binary"
	"fmt"
	"io"
)

// Block flag bits (subset; we only honour those that affect read).
const (
	flagFile        uint32 = 0x80000000 // entry refers to an actual file
	flagDeleteMark  uint32 = 0x02000000 // file is a delete marker (patches)
	flagSingleUnit  uint32 = 0x01000000 // not split into sectors
	flagSectorCRC   uint32 = 0x04000000 // sector-checksum table at end of data
	flagKeyAdjusted uint32 = 0x00020000 // encryption key adjusted by offset+size
	flagEncrypted   uint32 = 0x00010000 // file payload is encrypted
	flagCompressed  uint32 = 0x00000200 // multi-compression (1-byte mask prefix)
	flagImploded    uint32 = 0x00000100 // PKWARE DCL implode (no mask prefix)
)

// hashEntry is a single 16-byte hash-table slot.
type hashEntry struct {
	hashA      uint32 // hashString(name, hashNameA)
	hashB      uint32 // hashString(name, hashNameB)
	locale     uint16 // Windows LANGID; 0 = neutral
	platform   uint16 // always 0 in practice
	blockIndex uint32 // index into blockTable, or 0xFFFFFFFE/FFFFFFFF
}

// blockEntry is a single 16-byte block-table slot.
type blockEntry struct {
	offset  uint32 // file-relative byte offset
	cSize   uint32 // compressed (on-disk) size
	uSize   uint32 // uncompressed (extracted) size
	flags   uint32
}

const (
	emptyNeverUsed uint32 = 0xFFFFFFFF // hash slot never held a file
	emptyDeleted   uint32 = 0xFFFFFFFE // hash slot once held a file (skip)
)

// keys for the two well-known tables. Computed at init for clarity (we
// could hardcode 0xC3AF3770 and 0xEC83B3A3 but the derivation IS the doc).
var (
	hashTableKey  uint32
	blockTableKey uint32
)

func init() {
	hashTableKey = hashString("(hash table)", hashFileKey)
	blockTableKey = hashString("(block table)", hashFileKey)
}

// readHashTable reads, decrypts, and parses the hash table.
func readHashTable(r io.ReaderAt, h *mpqHeader) ([]hashEntry, error) {
	n := int(h.hashTableEntries)
	if n <= 0 {
		return nil, fmt.Errorf("mpq: zero hash table entries")
	}
	buf := make([]byte, n*16)
	if _, err := r.ReadAt(buf, h.hashTableAbsOffset()); err != nil {
		return nil, fmt.Errorf("mpq: read hash table: %w", err)
	}
	decryptBlock(buf, hashTableKey)

	out := make([]hashEntry, n)
	for i := 0; i < n; i++ {
		b := buf[i*16:]
		out[i] = hashEntry{
			hashA:      binary.LittleEndian.Uint32(b[0:4]),
			hashB:      binary.LittleEndian.Uint32(b[4:8]),
			locale:     binary.LittleEndian.Uint16(b[8:10]),
			platform:   binary.LittleEndian.Uint16(b[10:12]),
			blockIndex: binary.LittleEndian.Uint32(b[12:16]),
		}
	}
	return out, nil
}

// readBlockTable reads, decrypts, and parses the block table.
func readBlockTable(r io.ReaderAt, h *mpqHeader) ([]blockEntry, error) {
	n := int(h.blockTableEntries)
	if n <= 0 {
		return nil, fmt.Errorf("mpq: zero block table entries")
	}
	buf := make([]byte, n*16)
	if _, err := r.ReadAt(buf, h.blockTableAbsOffset()); err != nil {
		return nil, fmt.Errorf("mpq: read block table: %w", err)
	}
	decryptBlock(buf, blockTableKey)

	out := make([]blockEntry, n)
	for i := 0; i < n; i++ {
		b := buf[i*16:]
		out[i] = blockEntry{
			offset: binary.LittleEndian.Uint32(b[0:4]),
			cSize:  binary.LittleEndian.Uint32(b[4:8]),
			uSize:  binary.LittleEndian.Uint32(b[8:12]),
			flags:  binary.LittleEndian.Uint32(b[12:16]),
		}
	}
	return out, nil
}

// adjustedKey applies the FLAG_KEY_ADJUSTED transform. From StormLib:
//
//	key = (baseKey + blockOffset) XOR fileSize
//
// where blockOffset is the file-relative (NOT archive-absolute) offset of
// the encrypted block. The intent of the flag is to make each file's key
// unique even if two files share the same basename and thus the same base
// fileKey hash — without it, "war3map.j" placed at two different offsets
// would encrypt identically.
func adjustedKey(baseKey, blockOffset, fileSize uint32) uint32 {
	return (baseKey + blockOffset) ^ fileSize
}

// findHashEntry walks the open-addressed hash table searching for name.
// Returns (entry, true) on success, (zero, false) on miss. Honours the
// locale of 0 (neutral) — we don't currently let callers request a
// localised variant since WC3 maps never store per-locale dupes.
func findHashEntry(table []hashEntry, name string) (hashEntry, bool) {
	if len(table) == 0 {
		return hashEntry{}, false
	}
	mask := uint32(len(table) - 1) // table size is power-of-two
	start := hashString(name, hashOffset) & mask
	hA := hashString(name, hashNameA)
	hB := hashString(name, hashNameB)

	// Walk forward with wraparound, terminating on emptyNeverUsed (FFFFFFFF)
	// or after a full lap. emptyDeleted (FFFFFFFE) we skip past.
	i := start
	for laps := uint32(0); laps <= mask; laps++ {
		e := table[i]
		if e.blockIndex == emptyNeverUsed {
			return hashEntry{}, false
		}
		if e.blockIndex != emptyDeleted && e.hashA == hA && e.hashB == hB {
			return e, true
		}
		i = (i + 1) & mask
	}
	return hashEntry{}, false
}
