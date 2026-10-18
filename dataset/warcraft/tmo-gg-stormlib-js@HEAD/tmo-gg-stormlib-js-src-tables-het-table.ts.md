---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/tables/het-table.ts"
ref: HEAD
---

/**
 * HET (Hash Extended Table) for MPQ V3+ archives.
 *
 * The HET table uses Jenkins hash (upper 8 bits) for probing and
 * stores BET indices in a variable bit-width array.
 */

import { HET_TABLE_SIGNATURE, HET_ENTRY_FREE, HET_ENTRY_DELETED } from '../constants';
import { MpqCorruptError } from '../errors';
import { decryptBlock } from '../crypto/cipher';
import { hashString } from '../crypto/hash';
import { jenkinsHash } from '../crypto/hash';
import { FileStream } from '../stream/file-stream';

export interface HetTable {
  hashTableSize: number;
  maxFileCount: number;
  hashEntrySize: number;
  totalIndexSize: number;
  indexSizeExtra: number;
  indexSize: number;
  /** Hash values (upper 8 bits of Jenkins hash) */
  hetHashes: Uint8Array;
  /** BET indices encoded in variable bit-width */
  betIndices: number[];
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

/**
 * Load and parse the HET table from disk.
 *
 * @param stream - File stream
 * @param offset - Absolute offset of the HET table
 * @param compressedSize - Compressed size (from header)
 * @returns Parsed HET table
 */
export function loadHetTable(
  stream: FileStream,
  offset: bigint,
  compressedSize: bigint,
): HetTable | null {
  if (offset === 0n) return null;

  // Read the ext header (12 bytes)
  const extHeaderBuf = stream.read(offset, 12);
  if (extHeaderBuf.length < 12) return null;

  const signature = extHeaderBuf.readUInt32LE(0);
  if (signature !== HET_TABLE_SIGNATURE) return null;

  const version = extHeaderBuf.readUInt32LE(4);
  const dataSize = extHeaderBuf.readUInt32LE(8);

  // Read the HET data
  const dataBuf = stream.read(offset + 12n, dataSize);
  if (dataBuf.length < dataSize) return null;

  // Decrypt if needed (HET table key is derived from signature)
  // Note: HET/BET tables in practice are not encrypted in most MPQ files

  // Parse HET header fields (within dataBuf)
  let pos = 0;
  const hashTableSize = dataBuf.readUInt32LE(pos); pos += 4;
  const maxFileCount = dataBuf.readUInt32LE(pos); pos += 4;
  const hetHashTableSize = dataBuf.readUInt32LE(pos); pos += 4;
  const hashEntrySize = dataBuf.readUInt32LE(pos); pos += 4;
  const totalIndexSize = dataBuf.readUInt32LE(pos); pos += 4;
  const indexSizeExtra = dataBuf.readUInt32LE(pos); pos += 4;
  const indexSize = dataBuf.readUInt32LE(pos); pos += 4;
  const blockTableSize = dataBuf.readUInt32LE(pos); pos += 4;

  // Read het hash array
  const hetHashes = new Uint8Array(dataBuf.subarray(pos, pos + hetHashTableSize));
  pos += hetHashTableSize;

  // Read BET index array (variable bit-width packed)
  const betIndexBits = new Uint8Array(dataBuf.subarray(pos));
  const betIndices: number[] = new Array(hetHashTableSize);

  for (let i = 0; i < hetHashTableSize; i++) {
    betIndices[i] = readBits(betIndexBits, i * totalIndexSize, totalIndexSize);
  }

  return {
    hashTableSize: hetHashTableSize,
    maxFileCount,
    hashEntrySize,
    totalIndexSize,
    indexSizeExtra,
    indexSize,
    hetHashes,
    betIndices,
  };
}

/**
 * Look up a file in the HET table using Jenkins hash.
 *
 * @param het - HET table
 * @param fileName - File name to look up
 * @returns BET index if found, -1 otherwise
 */
export function findInHetTable(het: HetTable, fileName: string): number {
  if (!het || het.hashTableSize === 0) return -1;

  // Compute Jenkins hash and extract the upper 8 bits for HET lookup
  const fullHash = jenkinsHash(fileName);
  const hashByte = Number((fullHash >> 56n) & 0xFFn) | 0x80; // Set high bit

  const startIndex = Number(fullHash % BigInt(het.hashTableSize));
  let index = startIndex;

  for (;;) {
    const hetHash = het.hetHashes[index];

    if (hetHash === HET_ENTRY_FREE) {
      return -1; // Not found
    }

    if (hetHash === hashByte) {
      // Potential match - return the BET index for further verification
      return het.betIndices[index];
    }

    // Skip deleted entries, continue probing
    index = (index + 1) % het.hashTableSize;
    if (index === startIndex) return -1;
  }
}
