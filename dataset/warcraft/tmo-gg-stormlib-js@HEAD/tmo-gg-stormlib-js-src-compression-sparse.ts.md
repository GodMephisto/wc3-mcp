---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/compression/sparse.ts"
ref: HEAD
---

/**
 * Sparse decompression for MPQ archives.
 * Ported from StormLib's sparse.cpp.
 *
 * Sparse compression is a simple RLE-like encoding for files with
 * large runs of zero bytes.
 *
 * Format:
 * - Byte 0x00 followed by a length byte: emit that many zero bytes
 * - Any other byte: emit as-is
 */

/**
 * Decompress sparse-encoded data.
 *
 * @param inBuffer - Compressed input data
 * @param outSize - Expected decompressed size
 * @returns Decompressed data
 */
export function decompressSparse(inBuffer: Buffer, outSize: number): Buffer {
  const output = Buffer.alloc(outSize);
  let inPos = 0;
  let outPos = 0;

  while (inPos < inBuffer.length && outPos < outSize) {
    const byte = inBuffer[inPos++];

    if (byte !== 0x00) {
      // Literal byte
      output[outPos++] = byte;
    } else {
      // Zero run: next byte is the count
      if (inPos >= inBuffer.length) break;
      const count = inBuffer[inPos++];

      if (count === 0) {
        // Literal zero byte
        output[outPos++] = 0x00;
      } else {
        // Fill with zeros
        const end = Math.min(outPos + count, outSize);
        // Buffer is already zero-initialized, just advance position
        outPos = end;
      }
    }
  }

  return output;
}
