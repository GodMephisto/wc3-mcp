---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/tables/file-table.ts"
ref: HEAD
---

/**
 * Unified file table that merges hash table, block table, hi-block table,
 * and optionally HET/BET tables into a single TFileEntry array.
 */

import { MPQ_FILE_EXISTS, HASH_ENTRY_FREE, HASH_ENTRY_DELETED, BLOCK_INDEX_MASK, MPQ_HASH_TABLE_INDEX, MPQ_HASH_NAME_A, MPQ_HASH_NAME_B } from '../constants';
import type { MpqHeader, MpqHashEntry, MpqBlockEntry, FileEntry } from '../types';
import type { HetTable } from './het-table';
import type { BetTable } from './bet-table';
import { getBetEntry } from './bet-table';
import { hashString } from '../crypto/hash';

/**
 * Build a unified file table from classic hash + block tables.
 *
 * @param hashTable - Loaded hash table entries
 * @param blockTable - Loaded block table entries
 * @param hiBlockTable - Optional hi-block table (V2+)
 * @returns Array of unified file entries indexed by block index
 */
export function buildFileTable(
  hashTable: MpqHashEntry[],
  blockTable: MpqBlockEntry[],
  hiBlockTable: Uint16Array | null,
): FileEntry[] {
  const fileCount = blockTable.length;
  const entries: FileEntry[] = new Array(fileCount);

  // Initialize all file entries from the block table
  for (let i = 0; i < fileCount; i++) {
    const block = blockTable[i];
    let byteOffset = BigInt(block.dwFilePos);

    // Apply hi-block offset if available
    if (hiBlockTable && i < hiBlockTable.length) {
      byteOffset |= BigInt(hiBlockTable[i]) << 32n;
    }

    entries[i] = {
      fileNameHash: 0n,
      byteOffset,
      fileTime: 0n,
      fileSize: block.dwFSize,
      cmpSize: block.dwCSize,
      flags: block.dwFlags,
      crc32: 0,
      md5: new Uint8Array(16),
      fileName: '',
    };
  }

  return entries;
}

/**
 * Build a unified file table from HET/BET tables (V3+).
 */
export function buildFileTableFromHetBet(
  het: HetTable,
  bet: BetTable,
): FileEntry[] {
  const entries: FileEntry[] = [];

  for (let i = 0; i < bet.entries.length; i++) {
    const entry = getBetEntry(bet, i);
    if (entry) {
      entries.push(entry);
    }
  }

  return entries;
}

/**
 * Map file names from the hash table onto the file entry array.
 * This associates hash table entries with their corresponding file entries.
 */
export function mapHashToFileEntries(
  hashTable: MpqHashEntry[],
  fileEntries: FileEntry[],
): void {
  // The hash table doesn't store names directly - we can only
  // establish the mapping. Names are applied later from (listfile).
  // This function is primarily used to verify the hash-to-block mapping.
}

/**
 * Apply file names from a parsed listfile to the file entries.
 *
 * @param fileEntries - File entry array
 * @param hashTable - Hash table for lookup
 * @param names - Array of file names from (listfile)
 */
export function applyFileNames(
  fileEntries: FileEntry[],
  hashTable: MpqHashEntry[],
  names: string[],
): void {
  const tableSize = hashTable.length;
  if (tableSize === 0) return;
  const mask = tableSize - 1;

  for (const name of names) {
    if (!name) continue;

    const startIndex = hashString(name, MPQ_HASH_TABLE_INDEX) & mask;
    const check1 = hashString(name, MPQ_HASH_NAME_A);
    const check2 = hashString(name, MPQ_HASH_NAME_B);

    let index = startIndex;
    for (;;) {
      const entry = hashTable[index];

      if (entry.dwName1 === check1 &&
          entry.dwName2 === check2 &&
          (entry.dwBlockIndex & BLOCK_INDEX_MASK) < fileEntries.length) {
        const blockIndex = entry.dwBlockIndex & BLOCK_INDEX_MASK;
        if (!fileEntries[blockIndex].fileName) {
          fileEntries[blockIndex].fileName = name;
        }
        break;
      }

      if (entry.dwBlockIndex === HASH_ENTRY_FREE) break;

      index = (index + 1) & mask;
      if (index === startIndex) break;
    }
  }
}
