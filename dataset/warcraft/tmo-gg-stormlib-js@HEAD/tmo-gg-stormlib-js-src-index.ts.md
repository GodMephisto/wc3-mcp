---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/index.ts"
ref: HEAD
---

/**
 * stormlib-js - Pure TypeScript implementation of StormLib for reading MPQ archives.
 *
 * @example
 * ```typescript
 * import { MpqArchive } from 'stormlib-js';
 *
 * const archive = MpqArchive.open('game.mpq');
 *
 * // List files
 * const files = archive.getFileList();
 * console.log(files);
 *
 * // Extract a file
 * const data = archive.extractFile('units\\terran\\marine.grp');
 *
 * // Search for files
 * const wavFiles = archive.findFiles('*.wav');
 *
 * archive.close();
 * ```
 */

// Main API
export { MpqArchive } from './archive/mpq-archive';
export { MpqFile } from './file/mpq-file';

// Types
export type {
  MpqHeader,
  MpqUserData,
  MpqHashEntry,
  MpqBlockEntry,
  FileEntry,
  FileFindData,
  OpenArchiveOptions,
  HetTableHeader,
  BetTableHeader,
} from './types';

// Errors
export {
  MpqError,
  MpqNotFoundError,
  MpqCorruptError,
  MpqUnsupportedError,
  MpqEncryptionError,
  MpqCompressionError,
} from './errors';

// Constants (for advanced usage)
export {
  ID_MPQ,
  ID_MPQ_USERDATA,
  MPQ_FORMAT_VERSION_1,
  MPQ_FORMAT_VERSION_2,
  MPQ_FORMAT_VERSION_3,
  MPQ_FORMAT_VERSION_4,
  MPQ_FILE_IMPLODE,
  MPQ_FILE_COMPRESS,
  MPQ_FILE_ENCRYPTED,
  MPQ_FILE_KEY_V2,
  MPQ_FILE_SINGLE_UNIT,
  MPQ_FILE_EXISTS,
  MPQ_COMPRESSION_HUFFMANN,
  MPQ_COMPRESSION_ZLIB,
  MPQ_COMPRESSION_PKWARE,
  MPQ_COMPRESSION_BZIP2,
  MPQ_COMPRESSION_LZMA,
  MPQ_COMPRESSION_SPARSE,
  MPQ_COMPRESSION_ADPCM_MONO,
  MPQ_COMPRESSION_ADPCM_STEREO,
  LISTFILE_NAME,
  SIGNATURE_NAME,
  ATTRIBUTES_NAME,
} from './constants';

// Low-level crypto (for advanced usage / testing)
export { hashString, hashTableIndex, hashNameA, hashNameB, hashFileKey, jenkinsHash } from './crypto/hash';
export { decryptBlock, encryptBlock, decryptFileKey } from './crypto/cipher';
export { getStormBuffer } from './crypto/storm-buffer';
