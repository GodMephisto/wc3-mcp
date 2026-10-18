---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/types.ts"
ref: HEAD
---

/** Raw MPQ header as parsed from disk (normalized to V4 superset) */
export interface MpqHeader {
  /** 'MPQ\x1A' signature */
  dwID: number;
  /** Size of the header */
  dwHeaderSize: number;
  /** 32-bit archive size (V1) */
  dwArchiveSize: number;
  /** Format version: 0=V1, 1=V2, 2=V3, 3=V4 */
  wFormatVersion: number;
  /** Sector size shift (sector size = 512 << wSectorSize) */
  wSectorSize: number;
  /** Hash table offset (relative to archive start) */
  dwHashTablePos: number;
  /** Block table offset (relative to archive start) */
  dwBlockTablePos: number;
  /** Number of hash table entries */
  dwHashTableSize: number;
  /** Number of block table entries */
  dwBlockTableSize: number;

  // V2+ fields
  /** 64-bit hi-block table offset */
  hiBlockTablePos64: bigint;
  /** High 16 bits of hash table offset */
  wHashTablePosHi: number;
  /** High 16 bits of block table offset */
  wBlockTablePosHi: number;

  // V3+ fields
  /** 64-bit archive size */
  archiveSize64: bigint;
  /** 64-bit BET table position */
  betTablePos64: bigint;
  /** 64-bit HET table position */
  hetTablePos64: bigint;

  // V4+ fields
  /** Compressed hash table size */
  hashTableSize64: bigint;
  /** Compressed block table size */
  blockTableSize64: bigint;
  /** Compressed hi-block table size */
  hiBlockTableSize64: bigint;
  /** Compressed HET table size */
  hetTableSize64: bigint;
  /** Compressed BET table size */
  betTableSize64: bigint;
  /** Raw chunk size for MD5 */
  dwRawChunkSize: number;
  /** MD5 checksums (V4) */
  md5BlockTable: Uint8Array;
  md5HashTable: Uint8Array;
  md5HiBlockTable: Uint8Array;
  md5BetTable: Uint8Array;
  md5HetTable: Uint8Array;
  md5MpqHeader: Uint8Array;
}

/** User data header that may precede the MPQ header */
export interface MpqUserData {
  dwID: number;
  cbUserDataSize: number;
  dwHeaderOffs: number;
  cbUserDataHeader: number;
}

/** Hash table entry (16 bytes on disk) */
export interface MpqHashEntry {
  /** Hash of file path (method A) */
  dwName1: number;
  /** Hash of file path (method B) */
  dwName2: number;
  /** Windows LANGID locale */
  lcLocale: number;
  /** Platform (always 0) */
  platform: number;
  /** Index into block table (0xFFFFFFFF=free, 0xFFFFFFFE=deleted) */
  dwBlockIndex: number;
}

/** Block table entry (16 bytes on disk) */
export interface MpqBlockEntry {
  /** File data offset relative to archive start */
  dwFilePos: number;
  /** Compressed file size */
  dwCSize: number;
  /** Uncompressed file size */
  dwFSize: number;
  /** File flags */
  dwFlags: number;
}

/** Unified file entry (in-memory representation) */
export interface FileEntry {
  /** Jenkins hash (for BET table) */
  fileNameHash: bigint;
  /** File data offset relative to MPQ header */
  byteOffset: bigint;
  /** File time (FILETIME) from attributes */
  fileTime: bigint;
  /** Decompressed size */
  fileSize: number;
  /** Compressed size */
  cmpSize: number;
  /** File flags */
  flags: number;
  /** CRC32 from attributes */
  crc32: number;
  /** MD5 from attributes */
  md5: Uint8Array;
  /** File name (empty if unknown) */
  fileName: string;
}

/** HET table header */
export interface HetTableHeader {
  signature: number;
  version: number;
  dataSize: number;
  tableSize: number;
  maxFileCount: number;
  hashTableSize: number;
  hashEntrySize: number;
  totalIndexSize: number;
  indexSizeExtra: number;
  indexSize: number;
  blockTableSize: number;
}

/** BET table header */
export interface BetTableHeader {
  signature: number;
  version: number;
  dataSize: number;
  tableSize: number;
  maxFileCount: number;
  flagCount: number;
}

/** File search result */
export interface FileFindData {
  fileName: string;
  plainName: string;
  hashIndex: number;
  blockIndex: number;
  fileSize: number;
  compSize: number;
  fileFlags: number;
  locale: number;
}

/** Open archive flags */
export interface OpenArchiveOptions {
  noListfile?: boolean;
  noAttributes?: boolean;
  noHeaderSearch?: boolean;
  forceMpqV1?: boolean;
  checkSectorCrc?: boolean;
}
