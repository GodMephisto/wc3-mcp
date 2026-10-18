---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/index.browser.ts"
ref: HEAD
---

import { Buffer as BufferPolyfill } from 'buffer';

if (typeof globalThis !== 'undefined' && typeof (globalThis as { Buffer?: typeof BufferPolyfill }).Buffer === 'undefined') {
  (globalThis as { Buffer?: typeof BufferPolyfill }).Buffer = BufferPolyfill;
}

// Main API (browser-safe)
export { MpqArchive } from './archive/mpq-archive.browser';
export { MpqFile } from './file/mpq-file.browser';

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
