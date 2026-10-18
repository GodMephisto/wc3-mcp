---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/tables/bet-table.ts"
ref: HEAD
---

/**
 * BET (Block Extended Table) for MPQ V3+ archives.
 *
 * The BET table stores file metadata in minimum bit-width packed arrays.
 * Each field (offset, size, compressed size, flags, etc.) uses only
 * the minimum number of bits needed to represent all values.
 */

import { BET_TABLE_SIGNATURE } from '../constants';
import type { FileEntry } from '../types';
import { FileStream } from '../stream/file-stream';

export interface BetTable {
  maxFileCount: number;
  flagCount: number;
  flags: number[];
  entries: BetFileEntry[];
}

interface BetFileEntry {
  fileNameHash: bigint;
  byteOffset: bigint;
  fileSize: number;
  cmpSize: number;
  flagIndex: number;
}

/**
 * Read bits from a byte array at arbitrary bit positions.
 */
function readBits(data: Uint8Array, bitOffset: number, numBits: number): number {
  let result = 0;
  for (let i = 0; i < numBits; i++) {
    const byteIdx = (bitOffset + i) >>> 3;
    const bitIdx = (bitOffset + i) & 7;
    if (byteIdx < data.length && (data[byteIdx] & (1 << bitIdx))) {
      result |= (1 << i);
    }
  }
  return result;
}

function readBits64(data: Uint8Array, bitOffset: number, numBits: number): bigint {
  let result = 0n;
  for (let i = 0; i < numBits; i++) {
    const byteIdx = (bitOffset + i) >>> 3;
    const bitIdx = (bitOffset + i) & 7;
    if (byteIdx < data.length && (data[byteIdx] & (1 << bitIdx))) {
      result |= (1n << BigInt(i));
    }
  }
  return result;
}

/**
 * Load and parse the BET table from disk.
 *
 * @param stream - File stream
 * @param offset - Absolute offset of the BET table
 * @param compressedSize - Compressed size (from header)
 * @returns Parsed BET table
 */
export function loadBetTable(
  stream: FileStream,
  offset: bigint,
  compressedSize: bigint,
): BetTable | null {
  if (offset === 0n) return null;

  // Read the ext header (12 bytes)
  const extHeaderBuf = stream.read(offset, 12);
  if (extHeaderBuf.length < 12) return null;

  const signature = extHeaderBuf.readUInt32LE(0);
  if (signature !== BET_TABLE_SIGNATURE) return null;

  const version = extHeaderBuf.readUInt32LE(4);
  const dataSize = extHeaderBuf.readUInt32LE(8);

  // Read the BET data
  const dataBuf = stream.read(offset + 12n, dataSize);
  if (dataBuf.length < dataSize) return null;

  let pos = 0;

  // BET header
  const tableSize = dataBuf.readUInt32LE(pos); pos += 4;
  const maxFileCount = dataBuf.readUInt32LE(pos); pos += 4;
  const flagCount = dataBuf.readUInt32LE(pos); pos += 4;

  // Bit sizes for each field
  const filePosBits = dataBuf.readUInt32LE(pos); pos += 4;
  const fileSizeBits = dataBuf.readUInt32LE(pos); pos += 4;
  const cmpSizeBits = dataBuf.readUInt32LE(pos); pos += 4;
  const flagIndexBits = dataBuf.readUInt32LE(pos); pos += 4;
  const unknownBits = dataBuf.readUInt32LE(pos); pos += 4;

  // Bit sizes for the hash portion
  const hashSizeBits = dataBuf.readUInt32LE(pos); pos += 4;
  const hashSizeTotal = dataBuf.readUInt32LE(pos); pos += 4;
  const hashSizeExtra = dataBuf.readUInt32LE(pos); pos += 4;
  const hashArraySize = dataBuf.readUInt32LE(pos); pos += 4;

  // Read flags array
  const flags: number[] = [];
  for (let i = 0; i < flagCount; i++) {
    flags.push(dataBuf.readUInt32LE(pos));
    pos += 4;
  }

  // Calculate total bits per file entry
  const bitsPerEntry = filePosBits + fileSizeBits + cmpSizeBits + flagIndexBits + unknownBits;

  // Read packed file entries
  const entryData = new Uint8Array(dataBuf.subarray(pos));
  const entries: BetFileEntry[] = [];

  for (let i = 0; i < maxFileCount; i++) {
    let bitOff = i * bitsPerEntry;

    const byteOffset = readBits64(entryData, bitOff, filePosBits);
    bitOff += filePosBits;

    const fileSize = readBits(entryData, bitOff, fileSizeBits);
    bitOff += fileSizeBits;

    const cmpSize = readBits(entryData, bitOff, cmpSizeBits);
    bitOff += cmpSizeBits;

    const flagIndex = readBits(entryData, bitOff, flagIndexBits);
    bitOff += flagIndexBits;

    // Skip unknown bits

    // Read file name hash from the hash array (follows the entry data)
    const hashBitOffset = maxFileCount * bitsPerEntry + i * hashSizeTotal;
    const fileNameHash = readBits64(entryData, hashBitOffset, hashSizeTotal);

    entries.push({
      fileNameHash,
      byteOffset,
      fileSize,
      cmpSize,
      flagIndex,
    });
  }

  return { maxFileCount, flagCount, flags, entries };
}

/**
 * Get a file entry from the BET table by index.
 */
export function getBetEntry(bet: BetTable, index: number): FileEntry | null {
  if (index < 0 || index >= bet.entries.length) return null;

  const entry = bet.entries[index];
  const flags = entry.flagIndex < bet.flags.length ? bet.flags[entry.flagIndex] : 0;

  return {
    fileNameHash: entry.fileNameHash,
    byteOffset: entry.byteOffset,
    fileTime: 0n,
    fileSize: entry.fileSize,
    cmpSize: entry.cmpSize,
    flags,
    crc32: 0,
    md5: new Uint8Array(16),
    fileName: '',
  };
}
