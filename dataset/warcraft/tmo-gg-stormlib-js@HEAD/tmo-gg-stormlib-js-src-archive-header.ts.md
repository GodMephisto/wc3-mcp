---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/archive/header.ts"
ref: HEAD
---

/**
 * MPQ header parsing for all format versions (V1-V4).
 *
 * The MPQ header is located by searching for the 'MPQ\x1A' signature.
 * Different versions extend the base header with additional fields.
 */

import {
  ID_MPQ,
  ID_MPQ_USERDATA,
  MPQ_FORMAT_VERSION_1,
  MPQ_FORMAT_VERSION_2,
  MPQ_FORMAT_VERSION_3,
  MPQ_FORMAT_VERSION_4,
  MPQ_HEADER_SIZE_V1,
  MPQ_HEADER_SIZE_V2,
  MPQ_HEADER_SIZE_V3,
  MPQ_HEADER_SIZE_V4,
  MPQ_HEADER_SEARCH_ALIGNMENT,
} from '../constants';
import type { MpqHeader, MpqUserData } from '../types';
import { MpqCorruptError, MpqNotFoundError } from '../errors';
import { FileStream } from '../stream/file-stream';

/**
 * Create a default (zeroed) MPQ header with all fields initialized.
 */
function createDefaultHeader(): MpqHeader {
  return {
    dwID: 0,
    dwHeaderSize: 0,
    dwArchiveSize: 0,
    wFormatVersion: 0,
    wSectorSize: 0,
    dwHashTablePos: 0,
    dwBlockTablePos: 0,
    dwHashTableSize: 0,
    dwBlockTableSize: 0,
    hiBlockTablePos64: 0n,
    wHashTablePosHi: 0,
    wBlockTablePosHi: 0,
    archiveSize64: 0n,
    betTablePos64: 0n,
    hetTablePos64: 0n,
    hashTableSize64: 0n,
    blockTableSize64: 0n,
    hiBlockTableSize64: 0n,
    hetTableSize64: 0n,
    betTableSize64: 0n,
    dwRawChunkSize: 0,
    md5BlockTable: new Uint8Array(16),
    md5HashTable: new Uint8Array(16),
    md5HiBlockTable: new Uint8Array(16),
    md5BetTable: new Uint8Array(16),
    md5HetTable: new Uint8Array(16),
    md5MpqHeader: new Uint8Array(16),
  };
}

/**
 * Parse a V1 MPQ header (32 bytes).
 */
function parseHeaderV1(data: Buffer, header: MpqHeader): void {
  header.dwID = data.readUInt32LE(0x00);
  header.dwHeaderSize = data.readUInt32LE(0x04);
  header.dwArchiveSize = data.readUInt32LE(0x08);
  header.wFormatVersion = data.readUInt16LE(0x0C);
  header.wSectorSize = data.readUInt16LE(0x0E);
  header.dwHashTablePos = data.readUInt32LE(0x10);
  header.dwBlockTablePos = data.readUInt32LE(0x14);
  header.dwHashTableSize = data.readUInt32LE(0x18);
  header.dwBlockTableSize = data.readUInt32LE(0x1C);

  // Normalize V1 to unified format
  header.archiveSize64 = BigInt(header.dwArchiveSize);
  header.hashTableSize64 = BigInt(header.dwHashTableSize * 16);
  header.blockTableSize64 = BigInt(header.dwBlockTableSize * 16);
}

/**
 * Parse V2 extensions (additional 12 bytes).
 */
function parseHeaderV2(data: Buffer, header: MpqHeader): void {
  if (data.length < MPQ_HEADER_SIZE_V2) return;

  header.hiBlockTablePos64 = data.readBigUInt64LE(0x20);
  header.wHashTablePosHi = data.readUInt16LE(0x28);
  header.wBlockTablePosHi = data.readUInt16LE(0x2A);
}

/**
 * Parse V3 extensions (additional 24 bytes).
 */
function parseHeaderV3(data: Buffer, header: MpqHeader): void {
  if (data.length < MPQ_HEADER_SIZE_V3) return;

  header.archiveSize64 = data.readBigUInt64LE(0x2C);
  header.betTablePos64 = data.readBigUInt64LE(0x34);
  header.hetTablePos64 = data.readBigUInt64LE(0x3C);
}

/**
 * Parse V4 extensions (additional 140 bytes).
 */
function parseHeaderV4(data: Buffer, header: MpqHeader): void {
  if (data.length < MPQ_HEADER_SIZE_V4) return;

  header.hashTableSize64 = data.readBigUInt64LE(0x44);
  header.blockTableSize64 = data.readBigUInt64LE(0x4C);
  header.hiBlockTableSize64 = data.readBigUInt64LE(0x54);
  header.hetTableSize64 = data.readBigUInt64LE(0x5C);
  header.betTableSize64 = data.readBigUInt64LE(0x64);
  header.dwRawChunkSize = data.readUInt32LE(0x6C);

  header.md5BlockTable = new Uint8Array(data.subarray(0x70, 0x80));
  header.md5HashTable = new Uint8Array(data.subarray(0x80, 0x90));
  header.md5HiBlockTable = new Uint8Array(data.subarray(0x90, 0xA0));
  header.md5BetTable = new Uint8Array(data.subarray(0xA0, 0xB0));
  header.md5HetTable = new Uint8Array(data.subarray(0xB0, 0xC0));
  header.md5MpqHeader = new Uint8Array(data.subarray(0xC0, 0xD0));
}

/**
 * Parse an MPQ header from raw bytes.
 */
export function parseHeader(data: Buffer): MpqHeader {
  if (data.length < MPQ_HEADER_SIZE_V1) {
    throw new MpqCorruptError('Header data too small');
  }

  const header = createDefaultHeader();
  parseHeaderV1(data, header);

  if (header.dwID !== ID_MPQ) {
    throw new MpqCorruptError(`Invalid MPQ signature: 0x${header.dwID.toString(16)}`);
  }

  // Parse version-specific extensions
  if (header.wFormatVersion >= MPQ_FORMAT_VERSION_2) {
    parseHeaderV2(data, header);
  }
  if (header.wFormatVersion >= MPQ_FORMAT_VERSION_3) {
    parseHeaderV3(data, header);
  }
  if (header.wFormatVersion >= MPQ_FORMAT_VERSION_4) {
    parseHeaderV4(data, header);
  }

  return header;
}

/**
 * Parse a user data header.
 */
export function parseUserData(data: Buffer): MpqUserData {
  return {
    dwID: data.readUInt32LE(0x00),
    cbUserDataSize: data.readUInt32LE(0x04),
    dwHeaderOffs: data.readUInt32LE(0x08),
    cbUserDataHeader: data.readUInt32LE(0x0C),
  };
}

/** Result of searching for the MPQ header in a file */
export interface HeaderSearchResult {
  header: MpqHeader;
  headerOffset: bigint;
  userData?: MpqUserData;
  userDataOffset?: bigint;
}

/**
 * Search for the MPQ header in a file stream.
 *
 * The MPQ header can be found at 0x200-byte aligned offsets.
 * User data (MPQ\x1B) may precede the actual header.
 *
 * @param stream - File stream to search
 * @param noHeaderSearch - If true, only check offset 0
 * @returns Header search result with header, offset, and optional user data
 */
export function findHeader(stream: FileStream, noHeaderSearch = false): HeaderSearchResult {
  const fileSize = stream.getSize();
  const searchLimit = noHeaderSearch ? 0n : fileSize;

  let userData: MpqUserData | undefined;
  let userDataOffset: bigint | undefined;

  for (let offset = 0n; offset <= searchLimit; offset += BigInt(MPQ_HEADER_SEARCH_ALIGNMENT)) {
    // Read enough for the signature check
    const sigBuf = stream.read(offset, 4);
    if (sigBuf.length < 4) break;

    const signature = sigBuf.readUInt32LE(0);

    if (signature === ID_MPQ_USERDATA && !noHeaderSearch) {
      // Found user data - read the full user data header
      const udBuf = stream.read(offset, 16);
      if (udBuf.length >= 16) {
        userData = parseUserData(udBuf);
        userDataOffset = offset;

        // Jump to the MPQ header indicated by the user data
        const mpqOffset = offset + BigInt(userData.dwHeaderOffs);
        const headerBuf = stream.read(mpqOffset, MPQ_HEADER_SIZE_V4);
        if (headerBuf.length >= MPQ_HEADER_SIZE_V1) {
          const headerSig = headerBuf.readUInt32LE(0);
          if (headerSig === ID_MPQ) {
            const header = parseHeader(headerBuf);
            return { header, headerOffset: mpqOffset, userData, userDataOffset };
          }
        }
      }
    } else if (signature === ID_MPQ) {
      // Found MPQ header directly
      const headerBuf = stream.read(offset, MPQ_HEADER_SIZE_V4);
      if (headerBuf.length >= MPQ_HEADER_SIZE_V1) {
        const header = parseHeader(headerBuf);
        return { header, headerOffset: offset, userData, userDataOffset };
      }
    }

    // If we don't search, only check offset 0
    if (noHeaderSearch) break;
  }

  throw new MpqNotFoundError('MPQ header not found in file');
}

/**
 * Compute absolute table positions from the header, considering
 * V2+ extended offsets.
 */
export function getHashTableOffset(header: MpqHeader, archiveOffset: bigint): bigint {
  let offset = BigInt(header.dwHashTablePos);
  if (header.wFormatVersion >= MPQ_FORMAT_VERSION_2) {
    offset |= BigInt(header.wHashTablePosHi) << 32n;
  }
  return archiveOffset + offset;
}

export function getBlockTableOffset(header: MpqHeader, archiveOffset: bigint): bigint {
  let offset = BigInt(header.dwBlockTablePos);
  if (header.wFormatVersion >= MPQ_FORMAT_VERSION_2) {
    offset |= BigInt(header.wBlockTablePosHi) << 32n;
  }
  return archiveOffset + offset;
}

export function getHiBlockTableOffset(header: MpqHeader, archiveOffset: bigint): bigint {
  if (header.wFormatVersion >= MPQ_FORMAT_VERSION_2 && header.hiBlockTablePos64 !== 0n) {
    return archiveOffset + header.hiBlockTablePos64;
  }
  return 0n;
}

export function getHetTableOffset(header: MpqHeader, archiveOffset: bigint): bigint {
  if (header.wFormatVersion >= MPQ_FORMAT_VERSION_3 && header.hetTablePos64 !== 0n) {
    return archiveOffset + header.hetTablePos64;
  }
  return 0n;
}

export function getBetTableOffset(header: MpqHeader, archiveOffset: bigint): bigint {
  if (header.wFormatVersion >= MPQ_FORMAT_VERSION_3 && header.betTablePos64 !== 0n) {
    return archiveOffset + header.betTablePos64;
  }
  return 0n;
}

/** Get the sector size from the header's sector size shift value */
export function getSectorSize(header: MpqHeader): number {
  return 512 << header.wSectorSize;
}
