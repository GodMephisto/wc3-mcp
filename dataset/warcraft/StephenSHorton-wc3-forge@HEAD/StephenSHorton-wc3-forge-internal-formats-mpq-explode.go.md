---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/mpq/explode.go"
ref: HEAD
---

// PKWARE Data Compression Library "explode" decoder.
//
// Port of Mark Adler's blast.c (public domain, version 1.3) — the
// canonical reference implementation of the PKWARE DCL Implode format
// (described by Ben Rudiak-Gould in comp.compression, August 2001).
// Cross-checked against StormLib's explode.c.
//
// PKWARE Implode predates DEFLATE. Phil Katz designed it for his DCL
// library in 1991. Blizzard adopted it for early MPQ files and it's
// still the dominant compressor for Reign-of-Chaos-era WC3 maps. Modern
// Reforged map saves use ZLIB instead, but a single .w3x can mix
// compressions sector-by-sector.
//
// Stream layout, LSB-first:
//
//	header:
//	  byte 0: literal-encoding flag (0 = uncoded 8-bit, 1 = Huffman-coded)
//	  byte 1: log2(dictionary_size) - 6 (== 4, 5, or 6 -> 1024/2048/4096)
//
//	body: a sequence of tokens. Each token starts with 1 bit:
//	  0 -> literal byte (8 bits raw, OR Huffman-coded per the header)
//	  1 -> (length, distance) backreference
//
// Length encoding (16 symbols):
//   - Huffman code -> symbol 0..15
//   - base[symbol] is the base length (range 2..264 from a small table)
//   - extra[symbol] LSB bits added (0..8)
//   - Final length = base + extra. Range: 2..518.
//   - Special: length == 519 (achievable only as base[15]+0x100-1 etc.
//     — actually blast.c uses `len = base[symbol] + bits(...)`, then
//     checks `if (len == 519) break;` only in some forks. We follow
//     blast.c which uses end-of-stream detection by output count rather
//     than a 519-sentinel: it stops when we've decoded enough bytes.)
//
// Distance encoding (64 symbols):
//   - Huffman code -> top 6 bits of distance (0..63)
//   - Then 2 LSB bits if length==2, else (dict-shift) LSB bits
//   - Final distance = (top << low_bits) | low, plus 1 (distances 1-based).
//
// CRUCIAL bit-stream detail (gotcha that ate me on first attempt):
// Codes are stored bit-REVERSED relative to the simple integer ordering
// of canonical codes. The decoder reads one bit at a time, XOR'd with 1
// (i.e. inverted), accumulating LSB-into-low — this neatly reconstructs
// the canonical-code integer to compare against per-length thresholds.
// See blast.c decode() for the full explanation; this isn't a typo.
//
// CRUCIAL table-format detail: the litlen/lenlen/distlen arrays in the
// reference are run-length-compressed. Each byte packs (repeat-1) in
// the high 4 bits with code-length in the low 4 bits. We expand them
// before building the Huffman tables.

package mpq

import (
	"errors"
	"fmt"
	"io"
)

const explodeMaxBits = 13 // longest code we'll ever decode

// Packed bit lengths for the literal Huffman code (256 symbols total
// after RLE expansion). Source: blast.c (Adler).
var litlenPacked = []byte{
	11, 124, 8, 7, 28, 7, 188, 13, 76, 4, 10, 8, 12, 10, 12, 10, 8, 23, 8,
	9, 7, 6, 7, 8, 7, 6, 55, 8, 23, 24, 12, 11, 7, 9, 11, 12, 6, 7, 22, 5,
	7, 24, 6, 11, 9, 6, 7, 22, 7, 11, 38, 7, 9, 8, 25, 11, 8, 11, 9, 12,
	8, 12, 5, 38, 5, 38, 5, 11, 7, 5, 6, 21, 6, 10, 53, 8, 7, 24, 10, 27,
	44, 253, 253, 253, 252, 252, 252, 13, 12, 45, 12, 45, 12, 61, 12, 45,
	44, 173,
}

// Packed bit lengths for the length Huffman code (16 symbols).
var lenlenPacked = []byte{2, 35, 36, 53, 38, 23}

// Packed bit lengths for the distance Huffman code (64 symbols).
var distlenPacked = []byte{2, 20, 53, 230, 247, 151, 248}

// Length base values for length code symbols 0..15.
var lenBase = [16]uint{3, 2, 4, 5, 6, 7, 8, 9, 10, 12, 16, 24, 40, 72, 136, 264}

// Extra bits to read after each length-code symbol.
var lenExtra = [16]uint{0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8}

// unpackLengths expands a run-length-compressed length array into raw
// per-symbol code lengths. Each input byte encodes (repeat-1) in the
// high 4 bits and the length value in the low 4 bits.
func unpackLengths(packed []byte) []byte {
	var out []byte
	for _, b := range packed {
		repeats := int(b>>4) + 1
		length := b & 0x0F
		for i := 0; i < repeats; i++ {
			out = append(out, length)
		}
	}
	return out
}

// huffTable holds the canonical decoder for a PKWARE Huffman code.
// count[i] is the number of codes of length i+1; symbol[] is the
// symbols sorted first by code length, then by their original index.
type huffTable struct {
	count  [explodeMaxBits + 1]int16
	symbol []int16
}

// buildHuff constructs a canonical-Huffman decoder from raw bit lengths.
// Panics on over-subscribed input; PKWARE's published tables are all
// valid so a panic here means the static data was corrupted.
func buildHuff(lengths []byte) *huffTable {
	t := &huffTable{symbol: make([]int16, len(lengths))}

	for _, ln := range lengths {
		if ln > explodeMaxBits {
			panic(fmt.Sprintf("mpq/explode: code length %d exceeds MAXBITS=%d", ln, explodeMaxBits))
		}
		t.count[ln]++
	}
	if t.count[0] == int16(len(lengths)) {
		return t // empty code (all-zero lengths) — decoder will refuse to use
	}
	// Validate completeness (informational; PKWARE tables are complete).
	left := 1
	for ln := 1; ln <= explodeMaxBits; ln++ {
		left <<= 1
		left -= int(t.count[ln])
		if left < 0 {
			panic(fmt.Sprintf("mpq/explode: over-subscribed code at length %d", ln))
		}
	}
	// Bucket symbols by length, preserving original-index order within
	// each length bucket.
	var offsets [explodeMaxBits + 2]int16
	for ln := 1; ln < explodeMaxBits; ln++ {
		offsets[ln+1] = offsets[ln] + t.count[ln]
	}
	for sym, ln := range lengths {
		if ln != 0 {
			t.symbol[offsets[ln]] = int16(sym)
			offsets[ln]++
		}
	}
	return t
}

// Decoders, built once.
var (
	litHuff  *huffTable
	lenHuff  *huffTable
	distHuff *huffTable
)

func init() {
	litHuff = buildHuff(unpackLengths(litlenPacked))
	lenHuff = buildHuff(unpackLengths(lenlenPacked))
	distHuff = buildHuff(unpackLengths(distlenPacked))
}

// bitReader pulls LSB-first bits out of a byte slice. We keep up to 32
// unread bits buffered so single-bit decode loops don't have to bound-
// check on every step.
type bitReader struct {
	src []byte
	pos int    // byte cursor into src
	buf uint32 // unread bits in low-order positions
	n   uint   // number of valid bits in buf
}

// fill ensures at least 'want' bits are available (want <= 25).
func (b *bitReader) fill(want uint) error {
	for b.n < want {
		if b.pos >= len(b.src) {
			return io.ErrUnexpectedEOF
		}
		b.buf |= uint32(b.src[b.pos]) << b.n
		b.pos++
		b.n += 8
	}
	return nil
}

// readBits returns the next n bits LSB-first (n in 0..24).
func (b *bitReader) readBits(n uint) (uint, error) {
	if n == 0 {
		return 0, nil
	}
	if err := b.fill(n); err != nil {
		return 0, err
	}
	v := uint(b.buf & ((1 << n) - 1))
	b.buf >>= n
	b.n -= n
	return v, nil
}

// decodeHuff walks t one bit at a time. Each bit is inverted (XOR'd
// with 1) before being shifted into the accumulator — see blast.c
// decode() for the why. Returns the decoded symbol.
func (b *bitReader) decodeHuff(t *huffTable) (int16, error) {
	var code int
	var first int
	var index int
	for length := 1; length <= explodeMaxBits; length++ {
		bit, err := b.readBits(1)
		if err != nil {
			return 0, err
		}
		// Invert the bit before OR-ing — PKWARE codes are stored
		// bit-reversed in the stream.
		code |= int(bit) ^ 1
		count := int(t.count[length])
		if code < first+count {
			return t.symbol[index+(code-first)], nil
		}
		index += count
		first = (first + count) << 1
		code <<= 1
	}
	return 0, errors.New("mpq/explode: invalid Huffman code")
}

// explode decodes a PKWARE DCL implode stream and returns exactly
// `expanded` bytes. Errors on truncated input, oversized output,
// invalid headers, or out-of-range backreferences.
func explode(src []byte, expanded int) ([]byte, error) {
	if len(src) < 2 {
		return nil, errors.New("mpq/explode: input too short for header")
	}

	br := &bitReader{src: src}
	lit, err := br.readBits(8)
	if err != nil {
		return nil, fmt.Errorf("mpq/explode: read literal flag: %w", err)
	}
	if lit > 1 {
		return nil, fmt.Errorf("mpq/explode: invalid literal flag %d", lit)
	}
	dictBits, err := br.readBits(8)
	if err != nil {
		return nil, fmt.Errorf("mpq/explode: read dict-size byte: %w", err)
	}
	if dictBits < 4 || dictBits > 6 {
		return nil, fmt.Errorf("mpq/explode: invalid dict shift %d (want 4..6)", dictBits)
	}

	out := make([]byte, 0, expanded)

	for len(out) < expanded {
		tok, err := br.readBits(1)
		if err != nil {
			return nil, fmt.Errorf("mpq/explode: read token bit: %w", err)
		}
		if tok == 1 {
			// Backreference.
			lsym, err := br.decodeHuff(lenHuff)
			if err != nil {
				return nil, fmt.Errorf("mpq/explode: length code: %w", err)
			}
			length := lenBase[lsym]
			if lenExtra[lsym] > 0 {
				extra, err := br.readBits(lenExtra[lsym])
				if err != nil {
					return nil, fmt.Errorf("mpq/explode: length extra bits: %w", err)
				}
				length += extra
			}
			// blast.c sentinel: length 519 marks end-of-stream. Without
			// this, we'd just rely on `len(out) == expanded` — which is
			// usually correct, but a defensive break preserves bytes
			// after the sentinel for chained streams.
			if length == 519 {
				break
			}
			// Distance.
			dtop, err := br.decodeHuff(distHuff)
			if err != nil {
				return nil, fmt.Errorf("mpq/explode: distance code: %w", err)
			}
			var lowBits uint
			if length == 2 {
				lowBits = 2
			} else {
				lowBits = dictBits
			}
			dlow, err := br.readBits(lowBits)
			if err != nil {
				return nil, fmt.Errorf("mpq/explode: distance low bits: %w", err)
			}
			dist := (uint(dtop) << lowBits) | dlow
			dist++ // 1-based
			if int(dist) > len(out) {
				return nil, fmt.Errorf("mpq/explode: backref dist=%d exceeds output so far (%d bytes)", dist, len(out))
			}
			start := len(out) - int(dist)
			// Overlapping LZ77 copies are common (e.g. dist=1 length=N
			// is RLE). The byte-by-byte loop handles them correctly.
			for i := uint(0); i < length && len(out) < expanded; i++ {
				out = append(out, out[start+int(i)])
			}
		} else {
			// Literal.
			var b byte
			if lit == 0 {
				v, err := br.readBits(8)
				if err != nil {
					return nil, fmt.Errorf("mpq/explode: uncoded literal: %w", err)
				}
				b = byte(v)
			} else {
				sym, err := br.decodeHuff(litHuff)
				if err != nil {
					return nil, fmt.Errorf("mpq/explode: coded literal: %w", err)
				}
				b = byte(sym)
			}
			out = append(out, b)
		}
	}

	if len(out) != expanded {
		return nil, fmt.Errorf("mpq/explode: produced %d bytes, expected %d", len(out), expanded)
	}
	return out, nil
}
