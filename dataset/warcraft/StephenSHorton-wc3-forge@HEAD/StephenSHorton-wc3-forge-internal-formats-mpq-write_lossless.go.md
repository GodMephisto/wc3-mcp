---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/mpq/write_lossless.go"
ref: HEAD
---

// Pure-Go LOSSLESS MPQ writer (raw block copy-through).
//
// Build (write.go) rebuilds an archive from a flat name->bytes set. That's fine
// when you know every file's name — but a real WC3 map stores many files whose
// names we cannot resolve (custom imports under war3mapImported/, retextured
// stock assets, sounds). Rebuilding from names alone DROPS those files.
//
// BuildLossless instead reproduces the source archive's ENTIRE physical file set
// by copying each present block's raw stored bytes verbatim (see raw.go's
// RawEntries), then overlaying the caller's edits. It preserves the source hash
// table's SIZE and each raw entry's SLOT index, so name lookups (which forward-
// probe from lookupHash & mask) still resolve files we never named.
//
// What the caller passes:
//   - keep:  RawEntry values to copy through verbatim (the source's files minus
//            any that were edited or deleted). Each keeps its original Slot.
//   - named: FileEntry values to (re)write with fresh UNCOMPRESSED bytes —
//            edited files (whose old raw entry was dropped from keep) and brand-
//            new files. These are placed by ordinary open-addressed probing,
//            with any colliding slot already occupied by a kept raw entry
//            skipped over.
//   - hashCount: the hash-table size to preserve (the source's). Must be a power
//            of two and large enough to hold keep+named+listfile with room for
//            probing. BuildLossless grows it (doubling) if it's too tight.
//
// Storage model for both kinds:
//   - keep entries: emitted with their ORIGINAL flags + raw bytes + uSize. The
//     engine decompresses/decrypts them exactly as it did in the source.
//   - named entries: emitted UNCOMPRESSED + UNENCRYPTED, single-unit (same as
//     Build) — the simplest layout the reader handles verbatim.
//
// Offset pinning for position-dependent encryption: a keep entry with NeedsName
// (FLAG_KEY_ADJUSTED) has a decryption key derived from its file-relative block
// offset, so it CANNOT be relocated. BuildLossless pins every such entry back to
// its original Offset and packs all other blocks into the free spans around the
// pinned reservations. (Callers may also choose to drop key-adjusted entries and
// fall back; but pinning lets us preserve them losslessly, which is strictly
// better for the common case where the only key-adjusted file is an unnamed
// custom import sitting late in the data section.)
//
// A (listfile) is generated from the NAMED entries only (we don't know the names
// of keep entries). That's fine: List() unions the listfile with the standard
// WC3 set, and the unnamed customs are reached by the engine via the war3map.imp
// import table / direct path references, not the listfile.

package mpq

import (
	"bytes"
	"encoding/binary"
	"fmt"
)

// nextPow2AtLeast returns the smallest power of two >= n, with a floor of 4.
func nextPow2AtLeast(n uint32) uint32 {
	size := uint32(4)
	for size < n {
		size <<= 1
	}
	return size
}

// span is a reserved [start, end) byte range in the data section (used to pin
// offset-dependent blocks during lossless layout).
type span struct{ start, end uint32 }

// sortSpans sorts reserved offset ranges ascending by start. Tiny insertion
// sort — the pinned-block count is at most a handful for any real WC3 map.
func sortSpans(s []span) {
	for i := 1; i < len(s); i++ {
		for j := i; j > 0 && s[j].start < s[j-1].start; j-- {
			s[j], s[j-1] = s[j-1], s[j]
		}
	}
}

// BuildLossless serialises a lossless archive: every keep entry copied through
// verbatim at its original slot, every named entry (re)written uncompressed, a
// generated (listfile) for the named set. Returns the complete in-memory archive
// bytes (header at offset 0; prepend any HM3W preheader at the WriteFile layer).
//
// hashCount is the source hash-table size to preserve. If it can't hold all
// entries with probing headroom it is doubled until it can; keep slots are
// re-modulo'd into the (possibly larger) table by re-probing from slot&mask —
// which still keeps every kept file findable because the named-set lookups never
// collide structurally (see the placement loop).
// emitListfile controls whether a regenerated (listfile) is added. Pass true to
// preserve a source that already had one (or whenever named entries exist, so
// they're documented); pass false for a pure copy-through of a map that lacked
// a listfile, so the present-file count stays exactly the same. (Named entries
// always force a listfile regardless, since an undocumented edit is surprising.)
func BuildLossless(keep []RawEntry, named []FileEntry, hashCount uint32, sectorSizeShift uint16, emitListfile bool) ([]byte, error) {
	// The sector-size shift MUST match the source for copied-through multi-
	// sector files (their sector-offset table is keyed to it). Reject obviously
	// bogus values; fall back to the conventional WC3 4096-byte sectors if a
	// caller passes 0 (e.g. a synthetic archive with only single-unit/named
	// files, where sector size is immaterial).
	if sectorSizeShift == 0 {
		sectorSizeShift = blockSizeShift
	}
	if sectorSizeShift > 23 {
		return nil, fmt.Errorf("mpq: implausible sector_size_shift %d", sectorSizeShift)
	}
	// --- Validate + dedupe the named set (mirrors Build's contract). -------
	seen := make(map[string]struct{}, len(named))
	var content []FileEntry
	for _, f := range named {
		norm := normalizePath(f.Name)
		if norm == "" {
			return nil, fmt.Errorf("mpq: empty file name in lossless entry set")
		}
		key := upperASCII(norm)
		if key == upperASCII(listfileName) {
			continue // we synthesise our own listfile
		}
		if _, dup := seen[key]; dup {
			return nil, fmt.Errorf("mpq: duplicate named file %q (case/slash-insensitive)", f.Name)
		}
		seen[key] = struct{}{}
		content = append(content, FileEntry{Name: norm, Data: f.Data})
	}

	// Generate the (listfile) from the named content (CRLF, like Build), unless
	// the caller opted out for a pure copy-through of a listfile-less map. Named
	// entries always force a listfile (an undocumented edit is surprising).
	namedEntries := make([]FileEntry, 0, len(content)+1)
	namedEntries = append(namedEntries, content...)
	if emitListfile || len(content) > 0 {
		var lf bytes.Buffer
		for _, f := range content {
			lf.WriteString(f.Name)
			lf.WriteString("\r\n")
		}
		lf.WriteString(listfileName)
		lf.WriteString("\r\n")
		namedEntries = append(namedEntries, FileEntry{Name: listfileName, Data: lf.Bytes()})
	}

	// --- Size the hash table. Preserve the source size unless too tight. ----
	// Total slots we must hold: keep + named (incl. listfile). Need power-of-two
	// strictly greater than that for probing headroom (StormLib uses ~2x).
	totalFiles := uint32(len(keep) + len(namedEntries))
	if hashCount == 0 || (hashCount&(hashCount-1)) != 0 {
		hashCount = nextPow2AtLeast((totalFiles + 1) * 2)
	}
	// Guarantee at least one free slot beyond every occupant so forward-probe
	// search always terminates on a never-used slot.
	for hashCount <= totalFiles {
		hashCount <<= 1
	}
	mask := hashCount - 1

	// --- Assemble the block list + data section ----------------------------
	// We build a "payload" list (every block's bytes + flags + uSize + the hash
	// slot it occupies) then assign file-relative offsets in TWO passes:
	//   1. Pinned blocks (key-adjusted keep entries) reserve their ORIGINAL
	//      offset range verbatim.
	//   2. All other blocks are packed into the gaps before/between/after the
	//      pinned reservations, lowest free offset first.
	// The block table and hash table go after the highest data end.
	type payload struct {
		data    []byte
		flags   uint32
		uSize   uint32
		hA, hB  uint32
		locale  uint16
		plat    uint16
		probe   uint32 // hash slot to probe from (Slot&mask for keep, lookup&mask for named)
		pinned  bool
		pinAt   uint32 // valid when pinned
		offset  uint32 // assigned below
	}
	payloads := make([]payload, 0, int(totalFiles))

	for _, re := range keep {
		payloads = append(payloads, payload{
			data:   re.Raw,
			flags:  re.Flags,
			uSize:  re.USize,
			hA:     re.HashA,
			hB:     re.HashB,
			locale: re.Locale,
			plat:   re.Platform,
			probe:  re.Slot & mask,
			pinned: re.NeedsName,
			pinAt:  re.Offset,
		})
	}
	for _, e := range namedEntries {
		payloads = append(payloads, payload{
			data:   e.Data,
			flags:  flagFile | flagSingleUnit,
			uSize:  uint32(len(e.Data)),
			hA:     hashString(e.Name, hashNameA),
			hB:     hashString(e.Name, hashNameB),
			probe:  hashString(e.Name, hashOffset) & mask,
		})
	}

	// --- Offset assignment -------------------------------------------------
	// Collect pinned reservations and validate they don't collide with each
	// other (they never should — distinct files have distinct on-disk ranges).
	var reserved []span
	for i := range payloads {
		if !payloads[i].pinned {
			continue
		}
		size := uint32(len(payloads[i].data))
		if payloads[i].pinAt < headerSizeV0 {
			// A pinned block whose original offset overlaps the header is not
			// something we can reproduce. This should never happen for real
			// WC3 maps (key-adjusted files sit well into the data section).
			return nil, fmt.Errorf("mpq: pinned block offset %d overlaps header", payloads[i].pinAt)
		}
		payloads[i].offset = payloads[i].pinAt
		reserved = append(reserved, span{payloads[i].pinAt, payloads[i].pinAt + size})
	}
	sortSpans(reserved)
	for i := 1; i < len(reserved); i++ {
		if reserved[i].start < reserved[i-1].end {
			return nil, fmt.Errorf("mpq: pinned blocks overlap (%d..%d and %d..%d)",
				reserved[i-1].start, reserved[i-1].end, reserved[i].start, reserved[i].end)
		}
	}

	// Pack the non-pinned blocks into free space. cursor walks upward from the
	// end of the header, jumping over each reserved span it bumps into.
	cursor := uint32(headerSizeV0)
	advancePast := func(size uint32) uint32 {
		for {
			// Find a reserved span overlapping [cursor, cursor+size).
			bumped := false
			for _, r := range reserved {
				if cursor < r.end && cursor+size > r.start {
					cursor = r.end
					bumped = true
					break
				}
			}
			if !bumped {
				off := cursor
				cursor += size
				return off
			}
		}
	}
	for i := range payloads {
		if payloads[i].pinned {
			continue
		}
		payloads[i].offset = advancePast(uint32(len(payloads[i].data)))
	}

	// Highest data end across all payloads = where the tables start.
	maxEnd := cursor
	for _, r := range reserved {
		if r.end > maxEnd {
			maxEnd = r.end
		}
	}

	// --- Build block table + hash table from payloads ----------------------
	blocks := make([]blockEntry, len(payloads))
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
	// Place keep entries (the first len(keep) payloads) FIRST so they own their
	// slots; named entries probe around them.
	placeOrder := make([]int, 0, len(payloads))
	for i := 0; i < len(keep); i++ {
		placeOrder = append(placeOrder, i)
	}
	for i := len(keep); i < len(payloads); i++ {
		placeOrder = append(placeOrder, i)
	}
	for _, pi := range placeOrder {
		p := payloads[pi]
		blocks[pi] = blockEntry{
			offset: p.offset,
			cSize:  uint32(len(p.data)),
			uSize:  p.uSize,
			flags:  p.flags,
		}
		idx := p.probe
		placed := false
		for laps := uint32(0); laps <= mask; laps++ {
			if hashTable[idx].blockIndex == emptyNeverUsed {
				hashTable[idx] = hashEntry{
					hashA:      p.hA,
					hashB:      p.hB,
					locale:     p.locale,
					platform:   p.plat,
					blockIndex: uint32(pi),
				}
				placed = true
				break
			}
			idx = (idx + 1) & mask
		}
		if !placed {
			return nil, fmt.Errorf("mpq: lossless hash table full (cap=%d, files=%d)", hashCount, len(payloads))
		}
	}

	// --- Serialise the data section honouring assigned offsets -------------
	dataLen := maxEnd - headerSizeV0
	data := make([]byte, dataLen)
	for _, p := range payloads {
		copy(data[p.offset-headerSizeV0:], p.data)
	}

	blockCount := uint32(len(blocks))
	hashTableOffset := maxEnd
	blockTableOffset := hashTableOffset + hashCount*16
	archiveEnd := blockTableOffset + blockCount*16

	// --- Serialise + encrypt the two tables --------------------------------
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

	// --- Header ------------------------------------------------------------
	header := make([]byte, headerSizeV0)
	copy(header[0:4], magicMPQ[:])
	binary.LittleEndian.PutUint32(header[4:8], headerSizeV0)
	binary.LittleEndian.PutUint32(header[8:12], archiveEnd)
	binary.LittleEndian.PutUint16(header[12:14], 0) // formatVersion = 0
	binary.LittleEndian.PutUint16(header[14:16], sectorSizeShift)
	binary.LittleEndian.PutUint32(header[16:20], hashTableOffset)
	binary.LittleEndian.PutUint32(header[20:24], blockTableOffset)
	binary.LittleEndian.PutUint32(header[24:28], hashCount)
	binary.LittleEndian.PutUint32(header[28:32], blockCount)

	out := make([]byte, 0, int(archiveEnd))
	out = append(out, header...)
	out = append(out, data...)
	out = append(out, hashBytes...)
	out = append(out, blockBytes...)

	if uint32(len(out)) != archiveEnd {
		return nil, fmt.Errorf("mpq: lossless layout error: produced %d bytes, expected %d", len(out), archiveEnd)
	}
	return out, nil
}

// WriteFileLossless builds a lossless archive from keep + named entries and
// writes it to path atomically (sibling temp file + rename), preserving an
// optional HM3W preheader. Mirrors WriteFile's atomicity + do-no-harm contract:
// the destination is untouched unless the full write succeeds.
func WriteFileLossless(path string, keep []RawEntry, named []FileEntry, hashCount uint32, sectorSizeShift uint16, emitListfile bool, preHeader []byte) error {
	body, err := BuildLossless(keep, named, hashCount, sectorSizeShift, emitListfile)
	if err != nil {
		return err
	}
	return writeAtomic(path, preHeader, body)
}
