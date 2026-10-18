---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/file/sector-reader.browser.ts"
ref: HEAD
---

/**
 * Sector-based reading pipeline for MPQ file data.
 *
 * MPQ files are divided into sectors of a fixed size (usually 4096 bytes).
 * Each sector can be independently compressed and/or encrypted.
 *
 * Reading pipeline per sector:
 * 1. Read raw bytes from file stream
 * 2. Decrypt if encrypted (key = fileKey + sectorIndex)
 * 3. Decompress if compressed (first byte = compression bitmask)
 */

import {
  MPQ_FILE_COMPRESS,
  MPQ_FILE_IMPLODE,
  MPQ_FILE_ENCRYPTED,
  MPQ_FILE_SINGLE_UNIT,
  MPQ_FILE_SECTOR_CRC,
} from '../constants';
import type { FileEntry } from '../types';
import { decryptBlock } from '../crypto/cipher';
import { decompress } from '../compression';
import { decompressPkware } from '../compression/pkware';
import { MpqCorruptError, MpqCompressionError } from '../errors';
import { FileStream } from '../stream/file-stream.browser';

/**
 * Read and decrypt the sector offset table for a file.
 *
 * The sector offset table is stored at the beginning of the file's data
 * and contains (sectorCount + 1) DWORDs indicating the offset of each
 * sector's data relative to the file's start position.
 *
 * @param stream - File stream
 * @param fileOffset - Absolute offset of the file data in the stream
 * @param fileEntry - File entry metadata
 * @param sectorSize - Sector size
 * @param fileKey - File encryption key (0 if not encrypted)
 * @returns Array of sector offsets
 */
export function readSectorOffsets(
  stream: FileStream,
  fileOffset: bigint,
  fileEntry: FileEntry,
  sectorSize: number,
  fileKey: number,
): Uint32Array {
  const sectorCount = Math.ceil(fileEntry.fileSize / sectorSize);

  // The sector offset table has (sectorCount + 1) entries
  // Plus 1 more if the file has sector CRC checksums
  let offsetCount = sectorCount + 1;
  if (fileEntry.flags & MPQ_FILE_SECTOR_CRC) {
    offsetCount++;
  }

  const tableSize = offsetCount * 4;
  const tableData = stream.read(fileOffset, tableSize);

  if (tableData.length < tableSize) {
    throw new MpqCorruptError('Sector offset table is truncated');
  }

  // Decrypt the sector offset table (key = fileKey - 1)
  if (fileEntry.flags & MPQ_FILE_ENCRYPTED) {
    decryptBlock(tableData, (fileKey - 1) >>> 0);
  }

  // Parse into array
  const offsets = new Uint32Array(offsetCount);
  for (let i = 0; i < offsetCount; i++) {
    offsets[i] = tableData.readUInt32LE(i * 4);
  }

  return offsets;
}

/**
 * Read a single sector of a file.
 *
 * @param stream - File stream
 * @param fileOffset - Absolute offset of the file data in the stream
 * @param sectorOffsets - Sector offset table
 * @param sectorIndex - Index of the sector to read
 * @param fileEntry - File entry metadata
 * @param sectorSize - Sector size
 * @param fileKey - File encryption key (0 if not encrypted)
 * @returns Decompressed sector data
 */
export function readSector(
  stream: FileStream,
  fileOffset: bigint,
  sectorOffsets: Uint32Array,
  sectorIndex: number,
  fileEntry: FileEntry,
  sectorSize: number,
  fileKey: number,
): Buffer {
  const rawOffset = sectorOffsets[sectorIndex];
  const rawEnd = sectorOffsets[sectorIndex + 1];
  const rawSize = rawEnd - rawOffset;

  // Calculate the expected uncompressed size of this sector
  const sectorStart = sectorIndex * sectorSize;
  const expectedSize = Math.min(sectorSize, fileEntry.fileSize - sectorStart);

  // Read raw sector data
  let sectorData = stream.read(fileOffset + BigInt(rawOffset), rawSize);

  if (sectorData.length < rawSize) {
    throw new MpqCorruptError(`Sector ${sectorIndex} data is truncated`);
  }

  // Step 1: Decrypt if encrypted
  if (fileEntry.flags & MPQ_FILE_ENCRYPTED) {
    // Make a mutable copy for decryption
    sectorData = Buffer.from(sectorData);
    decryptBlock(sectorData, (fileKey + sectorIndex) >>> 0);
  }

  // Step 2: Decompress if needed
  if (rawSize < expectedSize) {
    if (fileEntry.flags & MPQ_FILE_COMPRESS) {
      // Multi-algorithm compression (first byte = bitmask)
      sectorData = decompress(sectorData, expectedSize);
    } else if (fileEntry.flags & MPQ_FILE_IMPLODE) {
      // PKWARE DCL implode
      sectorData = decompressPkware(sectorData, expectedSize);
    }
  }

  return sectorData;
}

/**
 * Read an entire file using sector-based reading.
 *
 * @param stream - File stream
 * @param archiveOffset - Offset of the MPQ archive within the file
 * @param fileEntry - File entry metadata
 * @param sectorSize - Sector size
 * @param fileKey - File encryption key (0 if not encrypted)
 * @returns Complete file data
 */
export function readSectorFile(
  stream: FileStream,
  archiveOffset: bigint,
  fileEntry: FileEntry,
  sectorSize: number,
  fileKey: number,
): Buffer {
  const fileOffset = archiveOffset + fileEntry.byteOffset;

  // Read sector offset table
  const sectorOffsets = readSectorOffsets(stream, fileOffset, fileEntry, sectorSize, fileKey);

  // Read each sector and assemble the output
  const sectorCount = Math.ceil(fileEntry.fileSize / sectorSize);
  const output = Buffer.alloc(fileEntry.fileSize);
  let outPos = 0;

  for (let i = 0; i < sectorCount; i++) {
    const sectorData = readSector(
      stream, fileOffset, sectorOffsets, i,
      fileEntry, sectorSize, fileKey,
    );

    const copyLen = Math.min(sectorData.length, fileEntry.fileSize - outPos);
    sectorData.copy(output, outPos, 0, copyLen);
    outPos += copyLen;
  }

  return output;
}

/**
 * Read a single-unit file (no sector division).
 *
 * Files with MPQ_FILE_SINGLE_UNIT flag are stored as a single block
 * without sector offset tables.
 *
 * @param stream - File stream
 * @param archiveOffset - Offset of the MPQ archive within the file
 * @param fileEntry - File entry metadata
 * @param fileKey - File encryption key (0 if not encrypted)
 * @returns Complete file data
 */
export function readSingleUnitFile(
  stream: FileStream,
  archiveOffset: bigint,
  fileEntry: FileEntry,
  fileKey: number,
): Buffer {
  const fileOffset = archiveOffset + fileEntry.byteOffset;

  // Read the entire compressed/encrypted data
  let data = stream.read(fileOffset, fileEntry.cmpSize);

  if (data.length < fileEntry.cmpSize) {
    throw new MpqCorruptError('Single-unit file data is truncated');
  }

  // Decrypt if encrypted
  if (fileEntry.flags & MPQ_FILE_ENCRYPTED) {
    data = Buffer.from(data);
    decryptBlock(data, fileKey);
  }

  // Decompress if needed
  if (fileEntry.cmpSize < fileEntry.fileSize) {
    if (fileEntry.flags & MPQ_FILE_COMPRESS) {
      data = decompress(data, fileEntry.fileSize);
    } else if (fileEntry.flags & MPQ_FILE_IMPLODE) {
      data = decompressPkware(data, fileEntry.fileSize);
    }
  }

  return data;
}
