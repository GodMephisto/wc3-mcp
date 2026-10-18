---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/mpq/header.go"
ref: HEAD
---

// MPQ header parsing and the WC3-specific HM3W shunt.
//
// Three things happen here:
//
//  1. We sniff the first 4 bytes. If it's "HM3W" (WC3 map shunt) the MPQ
//     archive starts at offset 512 — Blizzard reserves a fixed 512-byte
//     block at the head of every .w3x / .w3m for the lobby preview. The
//     name "WAR3" appears in some docs but is wrong for real maps; every
//     fixture we have on disk uses "HM3W".
//
//  2. If it's "MPQ\x1b" (user-data shunt) we follow the offset embedded
//     in that block to find the real header. This is SC2-era stuff but
//     StormLib supports it, and a handful of WC3 maps in the wild use it.
//
//  3. Otherwise (or if HM3W is present but the data at +512 isn't an MPQ
//     header for whatever reason — some custom packers), we walk the file
//     in 512-byte strides looking for "MPQ\x1a". StormLib does the same
//     thing. The archive offset stored in the returned header is the byte
//     offset within the file where the MPQ header lives.

package mpq

import (
	"encoding/binary"
	"errors"
	"fmt"
	"io"
)

// Magic bytes.
var (
	magicMPQ      = [4]byte{'M', 'P', 'Q', 0x1A} // archive header
	magicUserData = [4]byte{'M', 'P', 'Q', 0x1B} // optional user-data shunt
	magicHM3W     = [4]byte{'H', 'M', '3', 'W'}  // WC3 .w3x/.w3m fixed preheader
)

// HM3W preheader is exactly 512 bytes and contains:
//   - 4 bytes magic ("HM3W")
//   - 4 bytes unknown (always 0 in observed files)
//   - 4 bytes map flags
//   - 4 bytes loading-screen player-count (or unknown)
//   - 260 bytes UTF-8 map name (null-padded)
//   - up to 232 bytes padding
//
// We don't expose any of that — w3i parsing covers the same fields properly.
// The only thing we use it for is "skip 512 bytes and look for the MPQ
// header next".
const hm3wHeaderSize = 512

// mpqHeader is the parsed-and-normalised view of the MPQ archive header.
// Field names follow Zezula's spec. We only carry what we need for the
// read path; FormatVersion >= 2 (BC+ "MPQ v2", "Cataclysm v3", "v4")
// fields are present in some files but we don't consult them (WC3 never
// produced them and the WC3 reader-side path doesn't need them).
type mpqHeader struct {
	headerSize        uint32
	archiveSize       uint32
	formatVersion     uint16
	sectorSizeShift   uint16
	hashTableOffset   uint32
	blockTableOffset  uint32
	hashTableEntries  uint32
	blockTableEntries uint32

	// Format v1 (Burning Crusade) extension. Zero for v0 files.
	extBlockTableOffset uint64
	hashTableOffsetHi   uint16
	blockTableOffsetHi  uint16

	// archiveOffset is where in the underlying file the MPQ header lives.
	// All table offsets are relative to this.
	archiveOffset int64

	// sectorSize derived: 512 << sectorSizeShift. WC3 maps invariably use
	// shift=3 -> 4096-byte sectors but we don't hard-code that.
	sectorSize uint32
}

// findHeader scans r to find the MPQ archive header. Returns the parsed
// header, with archiveOffset set to the file offset of the MPQ magic byte.
//
// Scan strategy mirrors StormLib's SFileOpenArchive:
//   - If HM3W is at offset 0, start scanning at offset 512.
//   - If MPQ\x1b (user-data) is at the scan position, follow its embedded
//     headerOffset (still relative to the user-data block start).
//   - Otherwise step forward in 512-byte increments looking for MPQ\x1a.
//
// We cap the scan at the file size; we don't impose an artificial limit
// like "first 1 MB" because real archives can have arbitrarily large
// preceding data (game installers, mod loaders, etc.).
func findHeader(r io.ReaderAt, size int64) (*mpqHeader, error) {
	var sniff [4]byte
	if _, err := r.ReadAt(sniff[:], 0); err != nil {
		return nil, fmt.Errorf("mpq: read first 4 bytes: %w", err)
	}

	scanStart := int64(0)
	if sniff == magicHM3W {
		scanStart = hm3wHeaderSize
	}

	for offset := scanStart; offset+32 <= size; offset += 512 {
		var magic [4]byte
		if _, err := r.ReadAt(magic[:], offset); err != nil {
			// Past EOF or unreadable — give up cleanly.
			return nil, fmt.Errorf("mpq: scan at offset %d: %w", offset, err)
		}
		switch magic {
		case magicMPQ:
			return parseHeader(r, offset)
		case magicUserData:
			// User-data shunt: read its embedded header offset and try
			// again from there. We follow at most one shunt — StormLib
			// does the same — because chained shunts have never been
			// observed in the wild.
			var buf [12]byte
			if _, err := r.ReadAt(buf[:], offset); err != nil {
				return nil, fmt.Errorf("mpq: read user-data shunt: %w", err)
			}
			hdrRelOffset := binary.LittleEndian.Uint32(buf[8:12])
			absOffset := offset + int64(hdrRelOffset)
			if absOffset+32 > size {
				return nil, fmt.Errorf("mpq: user-data shunt points past EOF (offset=%d)", absOffset)
			}
			var hmagic [4]byte
			if _, err := r.ReadAt(hmagic[:], absOffset); err != nil {
				return nil, fmt.Errorf("mpq: read header after user-data shunt: %w", err)
			}
			if hmagic != magicMPQ {
				return nil, fmt.Errorf("mpq: user-data shunt did not point to an MPQ header (found %q)", hmagic[:])
			}
			return parseHeader(r, absOffset)
		}
	}
	return nil, errors.New("mpq: no MPQ header found in file")
}

// parseHeader reads and validates an MPQ archive header at offset.
func parseHeader(r io.ReaderAt, offset int64) (*mpqHeader, error) {
	// v0 header is 32 bytes; v1 extends it to 44; v2+ further. We always
	// read the v1 footprint (44 bytes) and only consult the extra fields
	// if formatVersion >= 1.
	var raw [44]byte
	n, err := r.ReadAt(raw[:], offset)
	if err != nil && n < 32 {
		return nil, fmt.Errorf("mpq: read header at offset %d: %w", offset, err)
	}

	h := &mpqHeader{archiveOffset: offset}
	// Magic is already verified by the caller; skip raw[0:4].
	h.headerSize = binary.LittleEndian.Uint32(raw[4:8])
	h.archiveSize = binary.LittleEndian.Uint32(raw[8:12])
	h.formatVersion = binary.LittleEndian.Uint16(raw[12:14])
	h.sectorSizeShift = binary.LittleEndian.Uint16(raw[14:16])
	h.hashTableOffset = binary.LittleEndian.Uint32(raw[16:20])
	h.blockTableOffset = binary.LittleEndian.Uint32(raw[20:24])
	h.hashTableEntries = binary.LittleEndian.Uint32(raw[24:28])
	h.blockTableEntries = binary.LittleEndian.Uint32(raw[28:32])

	if h.formatVersion >= 1 && n >= 44 {
		h.extBlockTableOffset = binary.LittleEndian.Uint64(raw[32:40])
		h.hashTableOffsetHi = binary.LittleEndian.Uint16(raw[40:42])
		h.blockTableOffsetHi = binary.LittleEndian.Uint16(raw[42:44])
	}

	// Sanity checks. WC3 always uses shift=3 (4096-byte sectors) but the
	// spec allows up to shift=15 in theory. We accept anything <= 23 so a
	// caller with a weird custom archive isn't silently rejected, but cap
	// at something that won't overflow on multiply.
	if h.sectorSizeShift > 23 {
		return nil, fmt.Errorf("mpq: implausible sector_size_shift %d", h.sectorSizeShift)
	}
	h.sectorSize = 512 << h.sectorSizeShift

	// Power-of-two assertion on hashTableEntries. The spec mandates it and
	// the indexing math below depends on it (we use `hash & (N-1)` instead
	// of `hash % N`).
	if h.hashTableEntries == 0 || (h.hashTableEntries&(h.hashTableEntries-1)) != 0 {
		return nil, fmt.Errorf("mpq: hash_table_entries %d is not a power of two", h.hashTableEntries)
	}
	return h, nil
}

// hashTableAbsOffset returns the file-absolute byte offset of the hash table.
func (h *mpqHeader) hashTableAbsOffset() int64 {
	return h.archiveOffset + int64(h.hashTableOffset) + int64(h.hashTableOffsetHi)<<32
}

// blockTableAbsOffset returns the file-absolute byte offset of the block table.
func (h *mpqHeader) blockTableAbsOffset() int64 {
	return h.archiveOffset + int64(h.blockTableOffset) + int64(h.blockTableOffsetHi)<<32
}
