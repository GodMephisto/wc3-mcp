---
type: vendor-doc
source_repo: "https://github.com/stego-research/mpq"
source_path: "mpq_dos_test.go"
ref: HEAD
---

package mpq

import (
	"bytes"
	"encoding/binary"
	"runtime"
	"testing"
)

// These tests pin the memory-exhaustion (DoS) guards added to diveIn: a tiny
// crafted archive must not be able to drive a large allocation from the
// attacker-controlled header fields (user-data size, hash/block table entry
// counts, sector-size shift). They craft headers directly rather than using a
// real replay so the dangerous fields can be set to hostile values.

// craftHeader builds a minimal FormatVersion-0 MPQ header (no user-data
// section) with the given fields. The returned bytes are exactly 32 long: the
// 4-byte magic plus the eight v0 header fields.
func craftHeader(sectorSizeShift uint16, hashEntries, blockEntries uint32) []byte {
	var b bytes.Buffer
	b.Write(headerMagic[:])
	put32 := func(v uint32) { binary.Write(&b, binary.LittleEndian, v) }
	put16 := func(v uint16) { binary.Write(&b, binary.LittleEndian, v) }
	put32(32)              // size
	put32(0)               // archiveSize
	put16(0)               // formatVersion (0 => no extended fields)
	put16(sectorSizeShift) // sectorSizeShift
	put32(0)               // hashTableOffset
	put32(0)               // blockTableOffset
	put32(hashEntries)     // hashTableEntries
	put32(blockEntries)    // blockTableEntries
	return b.Bytes()
}

// craftUserData builds a user-data section ("MPQ\x1b") declaring the given
// user-data size, followed by some padding so the input is non-trivial.
func craftUserData(size, headerOffset uint32) []byte {
	var b bytes.Buffer
	b.Write(userDataMagic[:])
	binary.Write(&b, binary.LittleEndian, size)
	binary.Write(&b, binary.LittleEndian, headerOffset)
	b.Write(make([]byte, 16)) // a little real data
	return b.Bytes()
}

// allocDelta returns the number of bytes the runtime cumulatively allocated
// while fn ran. TotalAlloc is monotonic, so this captures transient
// allocations even if they are freed/GC'd before fn returns — exactly what we
// need to prove the parser never allocated the multi-GB buffer.
func allocDelta(fn func()) uint64 {
	var before, after runtime.MemStats
	runtime.ReadMemStats(&before)
	fn()
	runtime.ReadMemStats(&after)
	return after.TotalAlloc - before.TotalAlloc
}

const dosAllocCeiling = 64 << 20 // 64 MiB: generous; the real attack wants GBs

func mustReject(t *testing.T, name string, input []byte) {
	t.Helper()
	var m *MPQ
	var err error
	delta := allocDelta(func() {
		m, err = New(bytes.NewReader(input))
	})
	if err == nil || m != nil {
		t.Fatalf("%s: expected parse to fail, got m=%v err=%v", name, m, err)
	}
	if delta > dosAllocCeiling {
		t.Fatalf("%s: parser allocated %d bytes (> %d ceiling); DoS guard ineffective",
			name, delta, dosAllocCeiling)
	}
}

func TestRejectHugeHashTableEntries(t *testing.T) {
	// 0x10000000 entries * 16 bytes ≈ 4.3 GiB hashTable without the guard.
	mustReject(t, "huge hashTableEntries", craftHeader(3, 0x10000000, 1))
}

func TestRejectHugeBlockTableEntries(t *testing.T) {
	mustReject(t, "huge blockTableEntries", craftHeader(3, 1, 0x10000000))
}

func TestRejectTableLargerThanFile(t *testing.T) {
	// Within the absolute cap, but 4096 entries * 16 = 64 KiB can't fit in a
	// 32-byte file — rejected by the per-input-length bound.
	mustReject(t, "table larger than file", craftHeader(3, 4096, 4096))
}

func TestRejectHugeUserDataSize(t *testing.T) {
	// Declares a ~4 GiB user-data section in a 24-byte file.
	input := craftUserData(0xFFFFFFFF, 512)
	mustReject(t, "huge user-data size", input)
}

func TestRejectHugeSectorSizeShift(t *testing.T) {
	// 512 << 255 overflows the uint32 blockSize to 0; without the guard a later
	// divide-by-blockSize panics. diveIn itself must reject it cleanly. The
	// table counts are valid (1 is a power of two and fits) so the sector-shift
	// guard is what does the rejecting.
	mustReject(t, "huge sectorSizeShift", craftHeader(255, 1, 1))
}

func TestRejectZeroHashTableEntries(t *testing.T) {
	// hashTableEntries=0 makes FileByHash compute (0-1) -> 0xFFFFFFFF and then
	// index an empty hash table, panicking. The power-of-two guard rejects it.
	mustReject(t, "zero hashTableEntries", craftHeader(3, 0, 1))
}

func TestRejectNonPowerOfTwoHashTableEntries(t *testing.T) {
	// 3 is within the absolute cap and (padded) fits the file, but it is not a
	// power of two, so the (n-1) lookup mask would be invalid. Reject it.
	input := append(craftHeader(3, 3, 1), make([]byte, 64)...) // pad so size check passes
	mustReject(t, "non-power-of-two hashTableEntries", input)
}

// TestFileByHashRejectsHugeFileSize is a white-box check of the FileByHash
// allocation guard: a block-table entry whose (uncompressed) fileSize dwarfs
// the archive must be rejected before make([]byte, fileSize). Crafting a fully
// valid encrypted archive is impractical (there is no encrypt()), so we build
// the post-diveIn state directly.
func TestFileByHashRejectsHugeFileSize(t *testing.T) {
	m := &MPQ{
		input:     bytes.NewReader(make([]byte, 1024)),
		inputSize: 1024,
		blockSize: 4096,
	}
	m.header.hashTableEntries = 1
	m.hashTable = []hashEntry{{filePathHashA: 20, filePathHashB: 30, fileBlockIndex: 0}}
	m.blockTable = []blockEntry{{blockOffset: 0, blockSize: 16, fileSize: 0xFFFFFFFF, flags: beFlagFile}}
	m.blockEntryIndices = []int{0}
	m.filesCount = 1

	var data []byte
	var err error
	delta := allocDelta(func() {
		data, err = m.FileByHash(0, 20, 30) // h1=0 -> index 0, matches the hash entry
	})
	if err != ErrInvalidArchive || data != nil {
		t.Fatalf("expected ErrInvalidArchive, got data=%d bytes err=%v", len(data), err)
	}
	if delta > dosAllocCeiling {
		t.Fatalf("FileByHash allocated %d bytes (> %d ceiling); fileSize guard ineffective",
			delta, dosAllocCeiling)
	}
}

// TestValidReplaysStillParse is a focused regression that the bounds never
// reject a genuine replay and that file extraction still works end-to-end.
func TestValidReplaysStillParse(t *testing.T) {
	m, err := NewFromFile("reps/automm.SC2Replay")
	if err != nil {
		t.Fatalf("valid replay rejected by DoS guard: %v", err)
	}
	defer m.Close()
	data, err := m.FileByName("replay.details")
	if err != nil {
		t.Fatalf("FileByName failed on valid replay: %v", err)
	}
	if len(data) == 0 {
		t.Fatal("expected non-empty replay.details from valid replay")
	}
}
