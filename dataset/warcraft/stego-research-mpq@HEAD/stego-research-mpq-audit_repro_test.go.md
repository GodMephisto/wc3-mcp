---
type: vendor-doc
source_repo: "https://github.com/stego-research/mpq"
source_path: "audit_repro_test.go"
ref: HEAD
---

package mpq

import (
	"bytes"
	"testing"
	"time"
)

// These tests pin two reliability bugs in FileByHash that the existing header
// DoS guards do NOT cover. Both are reachable from a crafted archive whose
// header passes every diveIn/validateReplayMPQHeader check (small, power-of-two
// table counts) but whose decrypted hash/block tables hold hostile values. The
// attacker fully controls those table bytes: the tables are encrypted with a
// fixed, publicly known key, so any desired plaintext can be produced.
//
// They assert the desired post-fix behavior (return, don't hang/panic) so they
// also serve as regression tests.

// TestFileByHashTerminatesWhenTableHasNoEmptySlot pins the hash-probe loop. The
// search in FileByHash only stops when it meets an entry whose fileBlockIndex is
// 0xFFFFFFFF (empty-and-always-empty). If every hash entry is occupied and none
// match the searched name, the loop wraps forever: a non-panic, non-recoverable
// infinite spin. rep.New calls FileByHash on every parse, so a tiny crafted
// replay hangs the parsing goroutine (and burns a core) permanently.
func TestFileByHashTerminatesWhenTableHasNoEmptySlot(t *testing.T) {
	const n = 16
	m := &MPQ{
		input:     bytes.NewReader(make([]byte, 64)),
		inputSize: 64,
		blockSize: 4096,
	}
	m.header.hashTableEntries = n
	m.hashTable = make([]hashEntry, n)
	for i := range m.hashTable {
		// Occupied (not 0xFFFFFFFF / not 0xFFFFFFFE) and never matching the
		// searched hashes below.
		m.hashTable[i] = hashEntry{filePathHashA: 1, filePathHashB: 1, fileBlockIndex: 7}
	}
	m.blockTable = []blockEntry{{flags: beFlagFile}}
	m.blockEntryIndices = []int{0}
	m.filesCount = 1

	done := make(chan struct{})
	go func() {
		// Search for a file whose hashes match no entry. Must terminate.
		m.FileByHash(0, 0xDEAD, 0xBEEF)
		close(done)
	}()

	select {
	case <-done:
		// ok: search terminated
	case <-time.After(2 * time.Second):
		t.Fatal("FileByHash did not terminate: infinite hash-probe loop (no probe bound)")
	}
}

// TestFileByHashRejectsOutOfRangeFileBlockIndex pins the counter loop. On a
// match, FileByHash runs `for j := 0; j < hashEntry.fileBlockIndex; j++` and
// indexes m.blockTable[j] every iteration. fileBlockIndex is attacker-controlled
// and unbounded, so a matching entry with a large index reads past the block
// table and panics (index out of range) before the post-loop bounds check at
// fileIndex >= filesCount can run.
func TestFileByHashRejectsOutOfRangeFileBlockIndex(t *testing.T) {
	m := &MPQ{
		input:     bytes.NewReader(make([]byte, 64)),
		inputSize: 64,
		blockSize: 4096,
	}
	m.header.hashTableEntries = 1
	// Matches the searched hashes, but points far past the 2-entry block table.
	m.hashTable = []hashEntry{{filePathHashA: 20, filePathHashB: 30, fileBlockIndex: 1 << 20}}
	m.blockTable = []blockEntry{{flags: beFlagFile}, {flags: 0}}
	m.blockEntryIndices = []int{0}
	m.filesCount = 1

	// Must not panic; a bogus index means "file not found".
	data, err := m.FileByHash(0, 20, 30)
	if data != nil {
		t.Fatalf("expected nil data for out-of-range fileBlockIndex, got %d bytes", len(data))
	}
	_ = err // nil or ErrInvalidArchive both acceptable; the point is no panic
}

// TestFileByHashRejectsOversizedOffsetTable pins the review fix that the packed
// block offset table cannot be larger than the stored block. The table is read
// from the front of the block; if it claims more entries than the block can
// hold, the reads run past the block into unrelated archive bytes and yield
// offsets from them. Here a compressed multi-sector block declares fileSize that
// needs a 3-entry (12-byte) offset table inside an 8-byte stored block.
func TestFileByHashRejectsOversizedOffsetTable(t *testing.T) {
	m := &MPQ{
		input:     bytes.NewReader(make([]byte, 8192)),
		inputSize: 8192,
		blockSize: 4096,
	}
	m.header.hashTableEntries = 1
	m.hashTable = []hashEntry{{filePathHashA: 20, filePathHashB: 30, fileBlockIndex: 0}}
	// fileSize 4097 -> blocksCount 2 -> 3-entry table (12 bytes) > blockSize 8.
	m.blockTable = []blockEntry{{blockOffset: 0, blockSize: 8, fileSize: 4097, flags: beFlagFile | beFlagCompressedMulti}}
	m.blockEntryIndices = []int{0}
	m.filesCount = 1

	data, err := m.FileByHash(0, 20, 30)
	if err != ErrInvalidArchive || data != nil {
		t.Fatalf("expected ErrInvalidArchive for oversized offset table, got data=%d bytes err=%v", len(data), err)
	}
}
