---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/archive/mpq-archive.browser.ts"
ref: HEAD
---

/**
 * MpqArchive - Main class for reading MPQ archives.
 *
 * This is the primary public API for opening and reading MPQ files.
 */

import {
  LISTFILE_NAME,
  ATTRIBUTES_NAME,
  MPQ_FILE_EXISTS,
  BLOCK_INDEX_MASK,
  MPQ_OPEN_NO_LISTFILE,
  MPQ_OPEN_NO_ATTRIBUTES,
  MPQ_OPEN_NO_HEADER_SEARCH,
  MPQ_OPEN_FORCE_MPQ_V1,
  MPQ_FORMAT_VERSION_1,
  MPQ_FORMAT_VERSION_3,
} from '../constants';
import type { MpqHeader, MpqHashEntry, MpqBlockEntry, FileEntry, FileFindData, OpenArchiveOptions } from '../types';
import type { FileStream as NodeFileStream } from '../stream/file-stream';
import { MpqNotFoundError, MpqError, MpqEncryptionError } from '../errors';
import { FileStream } from '../stream/file-stream.browser';
import {
  findHeader,
  getSectorSize,
  getHashTableOffset,
  getBlockTableOffset,
  getHiBlockTableOffset,
  getHetTableOffset,
  getBetTableOffset,
  type HeaderSearchResult,
} from './header';
import { loadHashTable, findHashEntry } from '../tables/hash-table';
import { loadBlockTable, loadHiBlockTable } from '../tables/block-table';
import { loadHetTable, findInHetTable, type HetTable } from '../tables/het-table';
import { loadBetTable, getBetEntry, type BetTable } from '../tables/bet-table';
import { buildFileTable, buildFileTableFromHetBet, applyFileNames } from '../tables/file-table';
import { parseListfile, parseAttributes, type FileAttributes } from './listfile';
import { MpqFile } from '../file/mpq-file.browser';

type BinaryInput = Buffer | Uint8Array | ArrayBuffer;
const asNodeStream = (stream: FileStream): NodeFileStream => stream as unknown as NodeFileStream;

export class MpqArchive {
  private stream: FileStream;
  private header: MpqHeader;
  private archiveOffset: bigint;
  private sectorSize: number;

  private hashTable: MpqHashEntry[] = [];
  private blockTable: MpqBlockEntry[] = [];
  private hiBlockTable: Uint16Array | null = null;
  private hetTable: HetTable | null = null;
  private betTable: BetTable | null = null;
  private fileEntries: FileEntry[] = [];
  private attributes: FileAttributes | null = null;
  private closed = false;

  private constructor(
    stream: FileStream,
    header: MpqHeader,
    archiveOffset: bigint,
  ) {
    this.stream = stream;
    this.header = header;
    this.archiveOffset = archiveOffset;
    this.sectorSize = getSectorSize(header);
  }

  /**
   * Open an MPQ archive from a file path.
   *
   * @param path - Path to the MPQ file
   * @param options - Open options
   * @returns MpqArchive instance
   */
  static open(path: string, options?: OpenArchiveOptions): MpqArchive {
    const stream = FileStream.open(path);
    return MpqArchive.openFromStream(stream, options);
  }

  /**
   * Open an MPQ archive from in-memory binary data.
   *
   * This API works in both Node.js and browser runtimes.
   *
   * @param data - MPQ binary contents
   * @param options - Open options
   * @returns MpqArchive instance
   */
  static openFromBuffer(data: BinaryInput, options?: OpenArchiveOptions): MpqArchive {
    const stream = FileStream.fromBuffer(data);
    return MpqArchive.openFromStream(stream, options);
  }

  private static openFromStream(stream: FileStream, options?: OpenArchiveOptions): MpqArchive {
    try {
      const noHeaderSearch = options?.noHeaderSearch ?? false;
      const { header, headerOffset } = findHeader(asNodeStream(stream), noHeaderSearch);

      // Force V1 if requested
      if (options?.forceMpqV1) {
        header.wFormatVersion = MPQ_FORMAT_VERSION_1;
      }

      const archive = new MpqArchive(stream, header, headerOffset);
      archive.loadTables();

      // Load (listfile) unless disabled
      if (!options?.noListfile) {
        archive.loadListfile();
      }

      // Load (attributes) unless disabled
      if (!options?.noAttributes) {
        archive.loadAttributes();
      }

      return archive;
    } catch (err) {
      stream.close();
      throw err;
    }
  }

  /**
   * Load all archive tables (hash, block, HET, BET).
   */
  private loadTables(): void {
    const h = this.header;
    const stream = asNodeStream(this.stream);

    // Load classic hash and block tables
    if (h.dwHashTableSize > 0) {
      const hashOffset = getHashTableOffset(h, this.archiveOffset);
      this.hashTable = loadHashTable(stream, hashOffset, h.dwHashTableSize);
    }

    if (h.dwBlockTableSize > 0) {
      const blockOffset = getBlockTableOffset(h, this.archiveOffset);
      this.blockTable = loadBlockTable(stream, blockOffset, h.dwBlockTableSize);
    }

    // Load hi-block table (V2+)
    const hiBlockOffset = getHiBlockTableOffset(h, this.archiveOffset);
    if (hiBlockOffset !== 0n && this.blockTable.length > 0) {
      this.hiBlockTable = loadHiBlockTable(stream, hiBlockOffset, this.blockTable.length);
    }

    // Load HET/BET tables (V3+)
    if (h.wFormatVersion >= MPQ_FORMAT_VERSION_3) {
      const hetOffset = getHetTableOffset(h, this.archiveOffset);
      if (hetOffset !== 0n) {
        this.hetTable = loadHetTable(stream, hetOffset, h.hetTableSize64);
      }

      const betOffset = getBetTableOffset(h, this.archiveOffset);
      if (betOffset !== 0n) {
        this.betTable = loadBetTable(stream, betOffset, h.betTableSize64);
      }
    }

    // Build the unified file table
    if (this.hetTable && this.betTable) {
      this.fileEntries = buildFileTableFromHetBet(this.hetTable, this.betTable);
    } else {
      this.fileEntries = buildFileTable(this.hashTable, this.blockTable, this.hiBlockTable);
    }
  }

  /**
   * Load and parse the (listfile) to populate file names.
   */
  private loadListfile(): void {
    try {
      const data = this.extractFileByName(LISTFILE_NAME);
      if (data) {
        const names = parseListfile(data);
        applyFileNames(this.fileEntries, this.hashTable, names);
      }
    } catch {
      // (listfile) may not exist - that's okay
    }
  }

  /**
   * Load and parse the (attributes) file.
   */
  private loadAttributes(): void {
    try {
      const data = this.extractFileByName(ATTRIBUTES_NAME);
      if (data) {
        this.attributes = parseAttributes(data, this.fileEntries.length);

        // Apply attributes to file entries
        if (this.attributes) {
          for (let i = 0; i < this.fileEntries.length; i++) {
            if (i < this.attributes.crc32s.length) {
              this.fileEntries[i].crc32 = this.attributes.crc32s[i];
            }
            if (i < this.attributes.fileTimes.length) {
              this.fileEntries[i].fileTime = this.attributes.fileTimes[i];
            }
            if (i < this.attributes.md5s.length) {
              this.fileEntries[i].md5 = this.attributes.md5s[i];
            }
          }
        }
      }
    } catch {
      // (attributes) may not exist - that's okay
    }
  }

  /**
   * Internal: extract a file by name without relying on the listfile.
   * Used for bootstrapping (listfile) and (attributes) loading.
   */
  private extractFileByName(fileName: string): Buffer | null {
    // Try hash table lookup first
    const hashEntry = findHashEntry(
      this.hashTable,
      fileName,
      0,
      this.fileEntries.length,
    );

    if (!hashEntry) return null;

    const blockIndex = hashEntry.dwBlockIndex & BLOCK_INDEX_MASK;
    if (blockIndex >= this.fileEntries.length) return null;

    const entry = this.fileEntries[blockIndex];
    if (!(entry.flags & MPQ_FILE_EXISTS)) return null;

    // Temporarily set the file name for key derivation
    const savedName = entry.fileName;
    entry.fileName = fileName;

    try {
      const file = new MpqFile(this.stream, this.archiveOffset, entry, this.sectorSize);
      const data = file.read();
      file.close();
      return data;
    } finally {
      entry.fileName = savedName;
    }
  }

  /**
   * Check if a file exists in the archive.
   */
  hasFile(name: string): boolean {
    this.ensureOpen();

    // Try HET table first (V3+)
    if (this.hetTable && this.betTable) {
      const betIndex = findInHetTable(this.hetTable, name);
      if (betIndex >= 0 && betIndex < this.fileEntries.length) {
        return !!(this.fileEntries[betIndex].flags & MPQ_FILE_EXISTS);
      }
    }

    // Fall back to hash table
    const hashEntry = findHashEntry(this.hashTable, name, 0, this.fileEntries.length);
    if (!hashEntry) return false;

    const blockIndex = hashEntry.dwBlockIndex & BLOCK_INDEX_MASK;
    return blockIndex < this.fileEntries.length &&
           !!(this.fileEntries[blockIndex].flags & MPQ_FILE_EXISTS);
  }

  /**
   * Open a file within the archive for reading.
   *
   * @param name - File path within the archive
   * @returns MpqFile handle
   */
  openFile(name: string): MpqFile {
    this.ensureOpen();

    const entry = this.resolveFile(name);
    if (!entry) {
      throw new MpqNotFoundError(`File not found: ${name}`);
    }

    return new MpqFile(this.stream, this.archiveOffset, entry, this.sectorSize);
  }

  /**
   * Extract a file's contents directly.
   *
   * @param name - File path within the archive
   * @returns Buffer with the file contents
   */
  extractFile(name: string): Buffer {
    const file = this.openFile(name);
    try {
      return file.read();
    } finally {
      file.close();
    }
  }

  /**
   * Get the list of all known file names (from listfile).
   */
  getFileList(): string[] {
    this.ensureOpen();
    return this.fileEntries
      .filter(e => e.fileName && (e.flags & MPQ_FILE_EXISTS))
      .map(e => e.fileName);
  }

  /**
   * Find files matching a wildcard pattern.
   *
   * @param mask - Wildcard pattern (supports '*' and '?')
   * @returns Array of matching file entries
   */
  findFiles(mask = '*'): FileFindData[] {
    this.ensureOpen();
    const regex = wildcardToRegex(mask);
    const results: FileFindData[] = [];

    for (let i = 0; i < this.fileEntries.length; i++) {
      const entry = this.fileEntries[i];
      if (!(entry.flags & MPQ_FILE_EXISTS)) continue;
      if (!entry.fileName) continue;

      if (regex.test(entry.fileName)) {
        const plainName = entry.fileName.replace(/^.*[\\/]/, '');
        results.push({
          fileName: entry.fileName,
          plainName,
          hashIndex: 0,
          blockIndex: i,
          fileSize: entry.fileSize,
          compSize: entry.cmpSize,
          fileFlags: entry.flags,
          locale: 0,
        });
      }
    }

    return results;
  }

  /**
   * Apply an external list of file names to resolve unnamed entries.
   * Equivalent to providing an external listfile to SFileFindFirstFile.
   *
   * @param names - Array of file names to try
   * @returns Number of newly resolved names
   */
  addListfile(names: string[]): number {
    this.ensureOpen();
    const before = this.fileEntries.filter(e => e.fileName).length;
    applyFileNames(this.fileEntries, this.hashTable, names);
    return this.fileEntries.filter(e => e.fileName).length - before;
  }

  /**
   * Enumerate all existing file entries, including those without names.
   * Unnamed entries get a synthetic name like "File00001234.xxx".
   */
  enumerateFiles(): FileFindData[] {
    this.ensureOpen();
    const results: FileFindData[] = [];

    for (let i = 0; i < this.fileEntries.length; i++) {
      const entry = this.fileEntries[i];
      if (!(entry.flags & MPQ_FILE_EXISTS)) continue;

      const fileName = entry.fileName || `File${String(i).padStart(8, '0')}.xxx`;
      const plainName = fileName.replace(/^.*[\\/]/, '');
      results.push({
        fileName,
        plainName,
        hashIndex: 0,
        blockIndex: i,
        fileSize: entry.fileSize,
        compSize: entry.cmpSize,
        fileFlags: entry.flags,
        locale: 0,
      });
    }

    return results;
  }

  /** Get the archive header */
  getHeader(): Readonly<MpqHeader> {
    return this.header;
  }

  /** Get the number of file entries */
  get fileCount(): number {
    return this.fileEntries.length;
  }

  /** Get the sector size */
  getSectorSize(): number {
    return this.sectorSize;
  }

  /**
   * Close the archive and release resources.
   */
  close(): void {
    if (!this.closed) {
      this.stream.close();
      this.closed = true;
    }
  }

  /**
   * Resolve a file name to its FileEntry.
   */
  private resolveFile(name: string): FileEntry | null {
    // Try HET table first (V3+)
    if (this.hetTable && this.betTable) {
      const betIndex = findInHetTable(this.hetTable, name);
      if (betIndex >= 0 && betIndex < this.fileEntries.length) {
        const entry = this.fileEntries[betIndex];
        if (entry.flags & MPQ_FILE_EXISTS) {
          if (!entry.fileName) entry.fileName = name;
          return entry;
        }
      }
    }

    // Fall back to hash table
    const hashEntry = findHashEntry(this.hashTable, name, 0, this.fileEntries.length);
    if (!hashEntry) return null;

    const blockIndex = hashEntry.dwBlockIndex & BLOCK_INDEX_MASK;
    if (blockIndex >= this.fileEntries.length) return null;

    const entry = this.fileEntries[blockIndex];
    if (!(entry.flags & MPQ_FILE_EXISTS)) return null;

    // Ensure file name is set for encryption key derivation
    if (!entry.fileName) {
      entry.fileName = name;
    }

    return entry;
  }

  private ensureOpen(): void {
    if (this.closed) {
      throw new MpqError('Archive is closed');
    }
  }
}

/**
 * Convert a simple wildcard pattern to a RegExp.
 * Supports '*' (any characters) and '?' (any single character).
 */
function wildcardToRegex(pattern: string): RegExp {
  const escaped = pattern
    .replace(/[.+^${}()|[\]\\]/g, '\\$&')
    .replace(/\*/g, '.*')
    .replace(/\?/g, '.');
  return new RegExp(`^${escaped}$`, 'i');
}
