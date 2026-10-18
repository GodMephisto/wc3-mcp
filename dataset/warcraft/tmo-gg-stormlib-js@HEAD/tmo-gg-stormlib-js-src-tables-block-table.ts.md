---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/tables/block-table.ts"
ref: HEAD
---

/**
 * MPQ Block Table operations.
 *
 * The block table stores metadata for each file in the archive:
 * offset, compressed size, uncompressed size, and flags.
 * Each entry is 16 bytes.
 *
 * V2+ archives also have a Hi-Block table storing the upper 16 bits
 * of the file offset for 64-bit addressing.
 */

import { MPQ_KEY_BLOCK_TABLE } from '../constants';
import type { MpqBlockEntry } from '../types';
import { decryptBlock } from '../crypto/cipher';
import { FileStream } from '../stream/file-stream';

/**
 * Load and decrypt the block table from disk.
 *
 * @param stream - File stream
 * @param offset - Absolute offset of the block table
 * @param count - Number of block table entries
 * @returns Array of parsed block entries
 */
export function loadBlockTable(stream: FileStream, offset: bigint, count: number): MpqBlockEntry[] {
  const dataSize = count * 16;
  const data = stream.read(offset, dataSize);

  if (data.length < dataSize) {
    count = Math.floor(data.length / 16);
  }

  // Decrypt the block table
  decryptBlock(data, MPQ_KEY_BLOCK_TABLE);

  // Parse entries
  const entries: MpqBlockEntry[] = new Array(count);
  for (let i = 0; i < count; i++) {
    const off = i * 16;
    entries[i] = {
      dwFilePos: data.readUInt32LE(off + 0x00),
      dwCSize: data.readUInt32LE(off + 0x04),
      dwFSize: data.readUInt32LE(off + 0x08),
      dwFlags: data.readUInt32LE(off + 0x0C),
    };
  }

  return entries;
}

/**
 * Load the Hi-Block table (V2+).
 * Contains the upper 16 bits of each file's offset.
 *
 * @param stream - File stream
 * @param offset - Absolute offset of the hi-block table
 * @param count - Number of entries (same as block table)
 * @returns Array of high offset values (upper 16 bits)
 */
export function loadHiBlockTable(stream: FileStream, offset: bigint, count: number): Uint16Array {
  const dataSize = count * 2;
  const data = stream.read(offset, dataSize);

  const hiOffsets = new Uint16Array(count);
  const actualCount = Math.min(count, Math.floor(data.length / 2));

  for (let i = 0; i < actualCount; i++) {
    hiOffsets[i] = data.readUInt16LE(i * 2);
  }

  return hiOffsets;
}
