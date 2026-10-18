---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/file/mpq-file.ts"
ref: HEAD
---

/**
 * MpqFile represents an open file handle within an MPQ archive.
 * It provides methods to read file data.
 */

import {
  MPQ_FILE_ENCRYPTED,
  MPQ_FILE_SINGLE_UNIT,
  MPQ_FILE_EXISTS,
} from '../constants';
import type { FileEntry } from '../types';
import { decryptFileKey } from '../crypto/cipher';
import { readSectorFile, readSingleUnitFile } from './sector-reader';
import { FileStream } from '../stream/file-stream';

export class MpqFile {
  private stream: FileStream;
  private archiveOffset: bigint;
  private entry: FileEntry;
  private sectorSize: number;
  private fileKey: number;
  private closed = false;

  constructor(
    stream: FileStream,
    archiveOffset: bigint,
    entry: FileEntry,
    sectorSize: number,
  ) {
    this.stream = stream;
    this.archiveOffset = archiveOffset;
    this.entry = entry;
    this.sectorSize = sectorSize;

    // Calculate the file key if the file is encrypted
    if (entry.flags & MPQ_FILE_ENCRYPTED) {
      this.fileKey = decryptFileKey(
        entry.fileName,
        entry.byteOffset,
        entry.fileSize,
        entry.flags,
      );
    } else {
      this.fileKey = 0;
    }
  }

  /** Uncompressed file size */
  get size(): number {
    return this.entry.fileSize;
  }

  /** Compressed file size */
  get compressedSize(): number {
    return this.entry.cmpSize;
  }

  /** File flags */
  get flags(): number {
    return this.entry.flags;
  }

  /** File name */
  get name(): string {
    return this.entry.fileName;
  }

  /** The underlying file entry */
  get fileEntry(): FileEntry {
    return this.entry;
  }

  /**
   * Read the entire file contents.
   *
   * @returns Buffer containing the decompressed file data
   */
  read(): Buffer {
    if (this.closed) {
      throw new Error('File handle is closed');
    }

    if (this.entry.fileSize === 0) {
      return Buffer.alloc(0);
    }

    if (this.entry.flags & MPQ_FILE_SINGLE_UNIT) {
      return readSingleUnitFile(
        this.stream,
        this.archiveOffset,
        this.entry,
        this.fileKey,
      );
    }

    return readSectorFile(
      this.stream,
      this.archiveOffset,
      this.entry,
      this.sectorSize,
      this.fileKey,
    );
  }

  /**
   * Close the file handle.
   */
  close(): void {
    this.closed = true;
  }
}
