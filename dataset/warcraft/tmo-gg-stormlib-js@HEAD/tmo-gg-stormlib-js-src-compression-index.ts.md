---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/compression/index.ts"
ref: HEAD
---

/**
 * MPQ compression/decompression dispatcher.
 *
 * MPQ sectors can use multiple compression algorithms chained together.
 * The first byte of a compressed sector is a bitmask indicating which
 * algorithms were applied. Decompression is applied in reverse order.
 */

import {
  MPQ_COMPRESSION_HUFFMANN,
  MPQ_COMPRESSION_ZLIB,
  MPQ_COMPRESSION_PKWARE,
  MPQ_COMPRESSION_BZIP2,
  MPQ_COMPRESSION_LZMA,
  MPQ_COMPRESSION_SPARSE,
  MPQ_COMPRESSION_ADPCM_MONO,
  MPQ_COMPRESSION_ADPCM_STEREO,
} from '../constants';
import { MpqCompressionError, MpqUnsupportedError } from '../errors';
import { decompressHuffman } from './huffman';
import { decompressPkware } from './pkware';
import { decompressAdpcmMono, decompressAdpcmStereo } from './adpcm';
import { decompressSparse } from './sparse';

// pako for zlib
import pako from 'pako';

type Decompressor = (inBuffer: Buffer, outSize: number) => Buffer;

/**
 * Decompression table entry. Order matters: decompression is applied
 * in this order (reverse of compression order).
 */
interface DecompressEntry {
  mask: number;
  decompress: Decompressor;
}

function decompressZlib(inBuffer: Buffer, outSize: number): Buffer {
  const result = pako.inflate(inBuffer);
  return Buffer.from(result.buffer, result.byteOffset, result.byteLength);
}

function decompressBzip2(_inBuffer: Buffer, _outSize: number): Buffer {
  throw new MpqUnsupportedError('BZIP2 decompression is not supported. Install a bzip2 package if needed.');
}

function decompressLzma(_inBuffer: Buffer, _outSize: number): Buffer {
  throw new MpqUnsupportedError('LZMA decompression is not supported. Install an LZMA package if needed.');
}

/** Decompression table in application order (reverse of compression order) */
const DECOMPRESS_TABLE: DecompressEntry[] = [
  { mask: MPQ_COMPRESSION_BZIP2,        decompress: decompressBzip2 },
  { mask: MPQ_COMPRESSION_PKWARE,        decompress: decompressPkware },
  { mask: MPQ_COMPRESSION_ZLIB,          decompress: decompressZlib },
  { mask: MPQ_COMPRESSION_HUFFMANN,      decompress: decompressHuffman },
  { mask: MPQ_COMPRESSION_ADPCM_STEREO,  decompress: decompressAdpcmStereo },
  { mask: MPQ_COMPRESSION_ADPCM_MONO,    decompress: decompressAdpcmMono },
  { mask: MPQ_COMPRESSION_SPARSE,        decompress: decompressSparse },
];

/**
 * Decompress an MPQ data block.
 *
 * @param inBuffer - Compressed data (first byte is compression bitmask)
 * @param outSize - Expected decompressed size
 * @returns Decompressed data
 */
export function decompress(inBuffer: Buffer, outSize: number): Buffer {
  // If sizes match, data is uncompressed
  if (inBuffer.length === outSize) {
    return Buffer.from(inBuffer);
  }

  if (inBuffer.length === 0) {
    throw new MpqCompressionError('Empty input buffer');
  }

  // First byte is the compression bitmask
  const compressionMask = inBuffer[0];
  let data = inBuffer.subarray(1);

  // Special case: LZMA is 0x12 which overlaps with BZIP2|ZLIB bits
  // It's handled as a special value, not a bitmask
  if (compressionMask === MPQ_COMPRESSION_LZMA) {
    return decompressLzma(data, outSize);
  }

  // Determine which decompression stages are needed
  const stages: DecompressEntry[] = [];
  let remainingMask = compressionMask;

  for (const entry of DECOMPRESS_TABLE) {
    if (remainingMask & entry.mask) {
      stages.push(entry);
      remainingMask &= ~entry.mask;
    }
  }

  if (stages.length === 0 || remainingMask !== 0) {
    throw new MpqCompressionError(
      `Unsupported compression mask: 0x${compressionMask.toString(16).padStart(2, '0')}`
    );
  }

  // Apply each decompression stage in order
  let currentData = data;
  let currentSize = outSize;

  for (let i = 0; i < stages.length; i++) {
    const isLast = i === stages.length - 1;
    const targetSize = isLast ? outSize : currentData.length * 4; // estimate for intermediate stages

    try {
      currentData = stages[i].decompress(
        Buffer.isBuffer(currentData) ? currentData : Buffer.from(currentData),
        isLast ? outSize : targetSize,
      );
    } catch (err) {
      if (err instanceof MpqUnsupportedError) throw err;
      throw new MpqCompressionError(
        `Decompression stage 0x${stages[i].mask.toString(16)} failed: ${err}`
      );
    }
  }

  return Buffer.isBuffer(currentData) ? currentData : Buffer.from(currentData);
}
