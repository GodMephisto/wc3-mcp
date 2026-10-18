---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/constants.ts"
ref: HEAD
---

// MPQ Archive Signatures
export const ID_MPQ = 0x1A51504D;          // 'MPQ\x1A'
export const ID_MPQ_USERDATA = 0x1B51504D;  // 'MPQ\x1B'
export const ID_MPQ_FILE = 0x46494C45;      // 'FILE'

// MPQ Format Versions
export const MPQ_FORMAT_VERSION_1 = 0;
export const MPQ_FORMAT_VERSION_2 = 1;
export const MPQ_FORMAT_VERSION_3 = 2;
export const MPQ_FORMAT_VERSION_4 = 3;

// Header Sizes
export const MPQ_HEADER_SIZE_V1 = 0x20;
export const MPQ_HEADER_SIZE_V2 = 0x2C;
export const MPQ_HEADER_SIZE_V3 = 0x44;
export const MPQ_HEADER_SIZE_V4 = 0xD0;

// Hash Table Constants
export const HASH_TABLE_SIZE_MIN = 0x00000004;
export const HASH_TABLE_SIZE_DEFAULT = 0x00001000;
export const HASH_TABLE_SIZE_MAX = 0x00080000;
export const HASH_ENTRY_DELETED = 0xFFFFFFFE;
export const HASH_ENTRY_FREE = 0xFFFFFFFF;
export const HET_ENTRY_DELETED = 0x80;
export const HET_ENTRY_FREE = 0x00;

// Hash Types (offsets into StormBuffer)
export const MPQ_HASH_TABLE_INDEX = 0x000;
export const MPQ_HASH_NAME_A = 0x100;
export const MPQ_HASH_NAME_B = 0x200;
export const MPQ_HASH_FILE_KEY = 0x300;
export const MPQ_HASH_KEY2_MIX = 0x400;

// Storm Buffer
export const STORM_BUFFER_SIZE = 0x500;

// Well-Known Encryption Keys
export const MPQ_KEY_HASH_TABLE = 0xC3AF3770;
export const MPQ_KEY_BLOCK_TABLE = 0xEC83B3A3;

// File Flags (TMPQBlock.dwFlags)
export const MPQ_FILE_IMPLODE = 0x00000100;
export const MPQ_FILE_COMPRESS = 0x00000200;
export const MPQ_FILE_ENCRYPTED = 0x00010000;
export const MPQ_FILE_KEY_V2 = 0x00020000;
export const MPQ_FILE_PATCH_FILE = 0x00100000;
export const MPQ_FILE_SINGLE_UNIT = 0x01000000;
export const MPQ_FILE_DELETE_MARKER = 0x02000000;
export const MPQ_FILE_SECTOR_CRC = 0x04000000;
export const MPQ_FILE_SIGNATURE = 0x10000000;
export const MPQ_FILE_EXISTS = 0x80000000;
export const MPQ_FILE_COMPRESS_MASK = 0x0000FF00;

// Compression IDs (bitmask)
export const MPQ_COMPRESSION_HUFFMANN = 0x01;
export const MPQ_COMPRESSION_ZLIB = 0x02;
export const MPQ_COMPRESSION_PKWARE = 0x08;
export const MPQ_COMPRESSION_BZIP2 = 0x10;
export const MPQ_COMPRESSION_LZMA = 0x12;
export const MPQ_COMPRESSION_SPARSE = 0x20;
export const MPQ_COMPRESSION_ADPCM_MONO = 0x40;
export const MPQ_COMPRESSION_ADPCM_STEREO = 0x80;

// Open Archive Flags
export const MPQ_OPEN_NO_LISTFILE = 0x00010000;
export const MPQ_OPEN_NO_ATTRIBUTES = 0x00020000;
export const MPQ_OPEN_NO_HEADER_SEARCH = 0x00040000;
export const MPQ_OPEN_FORCE_MPQ_V1 = 0x00080000;
export const MPQ_OPEN_CHECK_SECTOR_CRC = 0x00100000;
export const MPQ_OPEN_READ_ONLY = 0x00000100;

// Table Signatures
export const HET_TABLE_SIGNATURE = 0x1A544548;  // 'HET\x1a'
export const BET_TABLE_SIGNATURE = 0x1A544542;  // 'BET\x1a'

// Block Index
export const BLOCK_INDEX_MASK = 0x0FFFFFFF;

// Internal File Names
export const LISTFILE_NAME = '(listfile)';
export const SIGNATURE_NAME = '(signature)';
export const ATTRIBUTES_NAME = '(attributes)';

// Invalid markers
export const SFILE_INVALID_SIZE = 0xFFFFFFFF;

// MPQ search alignment
export const MPQ_HEADER_SEARCH_ALIGNMENT = 0x200;
