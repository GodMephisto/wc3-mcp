---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/mpq/decompress.go"
ref: HEAD
---

// Sector decompression.
//
// An MPQ sector is at most sectorSize bytes uncompressed. Its on-disk form
// is either:
//
//   - Raw (no flag): copied verbatim.
//   - FLAG_IMPLODED (block flag 0x100): the entire sector body is a PKWARE
//     DCL "implode" stream — there is NO leading method-mask byte.
//   - FLAG_COMPRESSED (block flag 0x200): the first byte is a bitmask of
//     compression methods applied (LSB = first applied = last to undo).
//     The rest is the multi-compressed body. In practice WC3 maps only ever
//     use a single bit at a time — usually zlib (0x02) or pkware (0x08).
//
// What we support (read path only):
//   - 0x02 ZLIB (compress/zlib in stdlib)
//   - 0x08 PKWARE DCL implode (handpicked port below; explode-only)
//
// What we DON'T support and will error on cleanly:
//   - 0x01 Huffman (used for WAVE files, never seen in WC3 maps)
//   - 0x10 BZIP2 (Reforged sometimes; pure-Go bzip2 exists in stdlib but
//          no observed WC3 use; left for a future maintainer)
//   - 0x20 SPARSE / 0x40 ADPCM mono / 0x80 ADPCM stereo (audio)
//   - 0x12 LZMA (SC2-era only)
//
// Two compressions combined (e.g. SPARSE+ZLIB) are rare and not seen in
// WC3 maps. We could implement this by chaining decoders in mask-bit order,
// but until we encounter a fixture that needs it, returning an error
// preserves "loud and obvious" failure semantics.

package mpq

import (
	"bytes"
	"compress/zlib"
	"fmt"
	"io"
)

// Compression-mask bits used as the first byte of a FLAG_COMPRESSED sector.
const (
	cmpHuffman  byte = 0x01
	cmpZlib     byte = 0x02
	cmpPKWare   byte = 0x08
	cmpBzip2    byte = 0x10
	cmpSparse   byte = 0x20
	cmpADPCMmono   byte = 0x40
	cmpADPCMstereo byte = 0x80
	cmpLZMA     byte = 0x12 // technically not a mask bit — LZMA is mutually exclusive
)

// decompressSector returns the uncompressed contents of one sector. It
// receives the raw on-disk bytes (already decrypted if applicable), the
// expected uncompressed size, and the flags from the parent block entry.
//
// The "no-op" case Blizzard relies on for incompressible sectors: if the
// on-disk size equals the expected uncompressed size, the sector is stored
// raw even if a compression flag is set. We detect this BEFORE looking at
// any compression mask, matching StormLib's behaviour.
func decompressSector(raw []byte, expanded int, flags uint32) ([]byte, error) {
	if len(raw) == expanded {
		// Stored raw — return a copy so callers can hold onto it after
		// the inBuffer is reused.
		out := make([]byte, expanded)
		copy(out, raw)
		return out, nil
	}

	switch {
	case flags&flagImploded != 0:
		// FLAG_IMPLODED: no method byte, the whole thing is a DCL stream.
		return explode(raw, expanded)

	case flags&flagCompressed != 0:
		if len(raw) < 2 {
			return nil, fmt.Errorf("mpq: compressed sector too small (%d bytes)", len(raw))
		}
		mask := raw[0]
		body := raw[1:]
		return decompressMulti(mask, body, expanded)

	default:
		// No compression flag set but on-disk size != expanded size.
		// That's a malformed sector.
		return nil, fmt.Errorf("mpq: sector size mismatch (got %d, want %d) with no compression flag", len(raw), expanded)
	}
}

// decompressMulti handles the FLAG_COMPRESSED multi-byte-mask case. The
// mask byte names which compressions were applied; we undo them in the
// order documented in StormLib (LZMA exclusive, otherwise sparse, then
// pkware/zlib/bzip2/huffman, then ADPCM). In WC3 maps the mask is always
// a single bit, so we keep this simple: dispatch on the only set bit and
// reject combos with a clear error.
func decompressMulti(mask byte, body []byte, expanded int) ([]byte, error) {
	switch mask {
	case cmpZlib:
		return inflateZlib(body, expanded)
	case cmpPKWare:
		return explode(body, expanded)
	case cmpHuffman, cmpBzip2, cmpSparse, cmpADPCMmono, cmpADPCMstereo, cmpLZMA:
		return nil, fmt.Errorf("mpq: unsupported compression mask 0x%02X", mask)
	default:
		// Combos like 0x41 (ADPCM+Huffman, used for WAVs) or unknown bits.
		return nil, fmt.Errorf("mpq: unsupported compression mask 0x%02X (combo or unknown)", mask)
	}
}

// inflateZlib runs stdlib zlib over body and returns exactly expanded
// bytes. Errors if the stream is short, malformed, or longer than
// expected (truncated reads would be silent data corruption).
func inflateZlib(body []byte, expanded int) ([]byte, error) {
	zr, err := zlib.NewReader(bytes.NewReader(body))
	if err != nil {
		return nil, fmt.Errorf("mpq: zlib header: %w", err)
	}
	defer zr.Close()
	out := make([]byte, expanded)
	if _, err := io.ReadFull(zr, out); err != nil {
		return nil, fmt.Errorf("mpq: zlib decode: %w", err)
	}
	// Confirm no trailing bytes. zlib.Reader returns EOF cleanly after the
	// adler32 footer; a non-EOF here means the encoder claimed more than
	// expanded bytes, which is an upstream bug we want to surface.
	var tail [1]byte
	if n, _ := zr.Read(tail[:]); n != 0 {
		return nil, fmt.Errorf("mpq: zlib stream produced more than %d bytes", expanded)
	}
	return out, nil
}
