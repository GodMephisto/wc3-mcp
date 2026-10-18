---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/mpq/crypto.go"
ref: HEAD
---

// Storm hash + table decryption primitives.
//
// MPQ uses an idiosyncratic hash family that Blizzard implemented decades ago.
// Both lookup-by-name AND table decryption derive from the SAME function
// (hashString) seeded with different "hash types". The cryptTable is a single
// 1280-entry uint32 array initialised once at process start by a closed-form
// LCG (see init() below) — every other algorithm in this package consumes it.
//
// References:
//   - Zezula's spec: http://www.zezula.net/en/mpq/techinfo.html
//   - StormLib SBaseCommon.cpp -> InitializeMpqCryptography / HashString / DecryptMpqBlock
//
// Why three hash types matter (and the "key trick" the doc string in this
// package's prompt mentions):
//
//	hashOffset (0x000) -> hash table lookup index
//	hashNameA  (0x100) -> hash table verifier #1
//	hashNameB  (0x200) -> hash table verifier #2
//	hashFileKey (0x300) -> per-file encryption key (only if FLAG_ENCRYPTED is set)
//
// The same string ("war3map.j") therefore yields FOUR distinct values used at
// different stages. To find a file you hash the name twice (offset + nameA/B).
// To DECRYPT that file's data you hash the basename — without any folder
// prefix — once more with hashFileKey. Getting any of those wrong silently
// corrupts the output; the integrity check is just sector checksums, which
// most maps don't enable.

package mpq

// Hash-type offsets into cryptTable. Each is a 256-entry stride. The library
// historically calls these MPQ_HASH_TABLE_OFFSET / NAME_A / NAME_B / FILE_KEY.
const (
	hashOffset  uint32 = 0x000
	hashNameA   uint32 = 0x100
	hashNameB   uint32 = 0x200
	hashFileKey uint32 = 0x300
)

// asciiToUpper folds a byte to uppercase the way StormLib does it: ASCII
// lowercase -> uppercase, and forward slash -> backslash (so "War3Map.j" and
// "war3map.j" hash identically, and so do "war3map/foo" and "war3map\foo").
// Non-ASCII bytes pass through unchanged. The high 24 bits of the result are
// always zero (the table index in hashString is just this byte's value).
var asciiToUpper = func() [256]byte {
	var t [256]byte
	for i := 0; i < 256; i++ {
		c := byte(i)
		switch {
		case c >= 'a' && c <= 'z':
			c -= 32
		case c == '/':
			c = '\\'
		}
		t[i] = c
	}
	return t
}()

// cryptTable is the 0x500-entry seed table the Storm hash + decrypt algorithms
// share. Populated once at init(). Indices are hashType + asciiToUpper[c] for
// hashing, or 0x400 + (seed & 0xff) for decryption.
var cryptTable [0x500]uint32

func init() {
	// The original BlizzWeird LCG. Same constants as every other MPQ port.
	var seed uint32 = 0x00100001
	for index1 := uint32(0); index1 < 0x100; index1++ {
		index2 := index1
		for i := 0; i < 5; i++ {
			seed = (seed*125 + 3) % 0x2AAAAB
			temp1 := (seed & 0xFFFF) << 0x10
			seed = (seed*125 + 3) % 0x2AAAAB
			temp2 := seed & 0xFFFF
			cryptTable[index2] = temp1 | temp2
			index2 += 0x100
		}
	}
}

// hashString computes a Storm hash of name with the given hash-type offset
// (one of hashOffset / hashNameA / hashNameB / hashFileKey). Case is folded
// and slash is normalised to backslash before hashing, so callers don't have
// to pre-normalise.
func hashString(name string, hashType uint32) uint32 {
	var seed1 uint32 = 0x7FED7FED
	var seed2 uint32 = 0xEEEEEEEE
	for i := 0; i < len(name); i++ {
		ch := uint32(asciiToUpper[name[i]])
		seed1 = cryptTable[hashType+ch] ^ (seed1 + seed2)
		seed2 = ch + seed1 + seed2 + (seed2 << 5) + 3
	}
	return seed1
}

// decryptBlock decrypts data in place using the Storm symmetric cipher keyed
// by key. Operates on 4-byte little-endian words; the length of data MUST be
// a multiple of 4 (callers always satisfy this — hash/block table entries are
// 16 bytes, sector-offset tables are uint32-aligned, sectors round up to a
// uint32 boundary on encrypt and the engine pads accordingly).
//
// "Decrypt" and "encrypt" are NOT the same operation here despite the cipher
// being symmetric in spirit — the cleartext word feeds back into seed2, so
// you have to decrypt the ciphertext to drive seed2 forward correctly. Don't
// reuse this for encryption.
func decryptBlock(data []byte, key uint32) {
	seed1 := key
	seed2 := uint32(0xEEEEEEEE)
	for i := 0; i+4 <= len(data); i += 4 {
		seed2 += cryptTable[0x400+(seed1&0xFF)]
		ch := uint32(data[i]) |
			uint32(data[i+1])<<8 |
			uint32(data[i+2])<<16 |
			uint32(data[i+3])<<24
		ch ^= seed1 + seed2
		seed1 = ((^seed1 << 0x15) + 0x11111111) | (seed1 >> 0x0B)
		seed2 = ch + seed2 + (seed2 << 5) + 3
		data[i] = byte(ch)
		data[i+1] = byte(ch >> 8)
		data[i+2] = byte(ch >> 16)
		data[i+3] = byte(ch >> 24)
	}
}

// fileKey computes the decryption key for a file with the given storage path.
// StormLib uses just the basename ("after the last backslash") because the
// engine doesn't know its own folder prefix at the time a file is added. We
// match. Returns the base key; callers must additionally adjust it for the
// FLAG_KEY_ADJUSTED case (key = (base + blockOffset) ^ fileSize) when that
// flag is set on the block entry. See tables.go for that adjustment.
func fileKey(storagePath string) uint32 {
	// Find the last separator. Both \ and / are valid; the name we receive
	// may already have been normalised but we belt-and-suspenders here so a
	// caller passing "war3map.j" or "(listfile)" works correctly.
	base := storagePath
	for i := len(storagePath) - 1; i >= 0; i-- {
		if storagePath[i] == '\\' || storagePath[i] == '/' {
			base = storagePath[i+1:]
			break
		}
	}
	return hashString(base, hashFileKey)
}
