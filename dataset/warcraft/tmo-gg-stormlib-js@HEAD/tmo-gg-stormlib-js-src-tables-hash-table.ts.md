---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/tables/hash-table.ts"
ref: HEAD
---

/**
 * MPQ Hash Table operations.
 *
 * The hash table maps file names to block table indices using open addressing
 * (linear probing). Each entry is 16 bytes containing two name verification
 * hashes, locale info, and a block index.
 */

import {
  MPQ_KEY_HASH_TABLE,
  MPQ_HASH_TABLE_INDEX,
  MPQ_HASH_NAME_A,
  MPQ_HASH_NAME_B,
  HASH_ENTRY_FREE,
  HASH_ENTRY_DELETED,
  BLOCK_INDEX_MASK,
} from '../constants';
import type { MpqHashEntry } from '../types';
import { decryptBlock } from '../crypto/cipher';
import { hashString } from '../crypto/hash';
import { FileStream } from '../stream/file-stream';

/**
 * Load and decrypt the hash table from disk.
 *
 * @param stream - File stream
 * @param offset - Absolute offset of the hash table
 * @param count - Number of hash table entries
 * @returns Array of parsed hash entries
 */
export function loadHashTable(stream: FileStream, offset: bigint, count: number): MpqHashEntry[] {
  const dataSize = count * 16;
  const data = stream.read(offset, dataSize);

  if (data.length < dataSize) {
    // Table may be truncated - adjust count
    count = Math.floor(data.length / 16);
  }

  // Decrypt the hash table
  decryptBlock(data, MPQ_KEY_HASH_TABLE);

  // Parse entries
  const entries: MpqHashEntry[] = new Array(count);
  for (let i = 0; i < count; i++) {
    const off = i * 16;
    entries[i] = {
      dwName1: data.readUInt32LE(off + 0x00),
      dwName2: data.readUInt32LE(off + 0x04),
      lcLocale: data.readUInt16LE(off + 0x08),
      platform: data[off + 0x0A],
      dwBlockIndex: data.readUInt32LE(off + 0x0C),
    };
  }

  return entries;
}

/**
 * Look up a file in the hash table.
 *
 * Uses three hashes: one for the table index, two for name verification.
 * Linear probing is used to handle collisions.
 *
 * @param hashTable - Loaded hash table entries
 * @param fileName - File name to look up
 * @param locale - Locale filter (0 = neutral / any)
 * @param maxBlockIndex - Maximum valid block index
 * @returns The hash entry if found, null otherwise
 */
export function findHashEntry(
  hashTable: MpqHashEntry[],
  fileName: string,
  locale: number,
  maxBlockIndex: number,
): MpqHashEntry | null {
  if (hashTable.length === 0) return null;

  const tableSize = hashTable.length;
  const mask = tableSize - 1; // Hash table size is always power of 2

  const startIndex = hashString(fileName, MPQ_HASH_TABLE_INDEX) & mask;
  const hashCheck1 = hashString(fileName, MPQ_HASH_NAME_A);
  const hashCheck2 = hashString(fileName, MPQ_HASH_NAME_B);

  let index = startIndex;

  for (;;) {
    const entry = hashTable[index];

    // Check if this entry matches
    if (entry.dwName1 === hashCheck1 &&
        entry.dwName2 === hashCheck2 &&
        (entry.dwBlockIndex & BLOCK_INDEX_MASK) < maxBlockIndex) {
      // Locale check: if looking for specific locale, match it;
      // otherwise accept any
      if (locale === 0 || entry.lcLocale === locale || entry.lcLocale === 0) {
        return entry;
      }
    }

    // Empty slot = file definitely not in table
    if (entry.dwBlockIndex === HASH_ENTRY_FREE) {
      return null;
    }

    // Deleted slot = skip and continue probing
    // Move to next slot (linear probing with wraparound)
    index = (index + 1) & mask;

    // Full circle = not found
    if (index === startIndex) {
      return null;
    }
  }
}

/**
 * Find all hash entries for a given file name (multiple locales).
 */
export function findAllHashEntries(
  hashTable: MpqHashEntry[],
  fileName: string,
  maxBlockIndex: number,
): MpqHashEntry[] {
  if (hashTable.length === 0) return [];

  const results: MpqHashEntry[] = [];
  const tableSize = hashTable.length;
  const mask = tableSize - 1;

  const startIndex = hashString(fileName, MPQ_HASH_TABLE_INDEX) & mask;
  const hashCheck1 = hashString(fileName, MPQ_HASH_NAME_A);
  const hashCheck2 = hashString(fileName, MPQ_HASH_NAME_B);

  let index = startIndex;

  for (;;) {
    const entry = hashTable[index];

    if (entry.dwName1 === hashCheck1 &&
        entry.dwName2 === hashCheck2 &&
        (entry.dwBlockIndex & BLOCK_INDEX_MASK) < maxBlockIndex) {
      results.push(entry);
    }

    if (entry.dwBlockIndex === HASH_ENTRY_FREE) break;

    index = (index + 1) & mask;
    if (index === startIndex) break;
  }

  return results;
}
