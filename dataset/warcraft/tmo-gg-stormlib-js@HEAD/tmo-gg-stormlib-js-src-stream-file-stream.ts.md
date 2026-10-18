---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/stream/file-stream.ts"
ref: HEAD
---

/**
 * File stream abstraction for reading MPQ archives from disk.
 * Uses Node.js fs module for synchronous positioned reads.
 */

type BinaryInput = Buffer | Uint8Array | ArrayBuffer;
type FsModule = typeof import('fs');

export class FileStream {
  private fd: number;
  private fileSize: bigint;
  private filePath: string;
  private memoryData: Buffer | null;
  private static fsModule: FsModule | null = null;

  private constructor(fd: number, fileSize: bigint, filePath: string, memoryData: Buffer | null = null) {
    this.fd = fd;
    this.fileSize = fileSize;
    this.filePath = filePath;
    this.memoryData = memoryData;
  }

  /**
   * Open a file for reading.
   */
  static open(path: string): FileStream {
    const fs = FileStream.getFs();
    const fd = fs.openSync(path, 'r');
    const stat = fs.fstatSync(fd);
    return new FileStream(fd, BigInt(stat.size), path);
  }

  /**
   * Create an in-memory stream from binary data.
   */
  static fromBuffer(data: BinaryInput, filePath = '[buffer]'): FileStream {
    const buffer = FileStream.normalizeBuffer(data);
    return new FileStream(-1, BigInt(buffer.length), filePath, buffer);
  }

  private static normalizeBuffer(data: BinaryInput): Buffer {
    if (Buffer.isBuffer(data)) {
      return data;
    }

    if (data instanceof Uint8Array) {
      return Buffer.from(data.buffer, data.byteOffset, data.byteLength);
    }

    return Buffer.from(data);
  }

  private static getRequire(): ((moduleName: string) => unknown) | null {
    try {
      if (typeof require === 'function') {
        return require;
      }
    } catch {
      // ignore
    }

    try {
      return Function('return typeof require === "function" ? require : null;')() as
        | ((moduleName: string) => unknown)
        | null;
    } catch {
      return null;
    }
  }

  private static getFs(): FsModule {
    if (FileStream.fsModule) {
      return FileStream.fsModule;
    }

    const req = FileStream.getRequire();
    if (!req) {
      throw new Error('Node.js fs module is not available in this runtime. Use MpqArchive.openFromBuffer().');
    }

    const fs = req('fs');
    if (!fs) {
      throw new Error('Failed to load Node.js fs module.');
    }

    FileStream.fsModule = fs as FsModule;
    return FileStream.fsModule;
  }

  /**
   * Read bytes at a given offset.
   *
   * @param offset - Byte offset from the beginning of the file
   * @param length - Number of bytes to read
   * @returns Buffer containing the read data
   */
  read(offset: bigint, length: number): Buffer {
    if (this.memoryData) {
      const start = Number(offset);
      if (start >= this.memoryData.length || length <= 0) {
        return Buffer.alloc(0);
      }
      const end = Math.min(start + length, this.memoryData.length);
      return this.memoryData.subarray(start, end);
    }

    const fs = FileStream.getFs();
    const buffer = Buffer.alloc(length);
    const bytesRead = fs.readSync(this.fd, buffer, 0, length, Number(offset));
    if (bytesRead < length) {
      return buffer.subarray(0, bytesRead);
    }
    return buffer;
  }

  /**
   * Read bytes into an existing buffer.
   */
  readInto(offset: bigint, buffer: Buffer, bufOffset: number, length: number): number {
    if (this.memoryData) {
      const start = Number(offset);
      if (start >= this.memoryData.length || length <= 0) {
        return 0;
      }
      const end = Math.min(start + length, this.memoryData.length);
      return this.memoryData.copy(buffer, bufOffset, start, end);
    }

    const fs = FileStream.getFs();
    return fs.readSync(this.fd, buffer, bufOffset, length, Number(offset));
  }

  /**
   * Get the total file size.
   */
  getSize(): bigint {
    return this.fileSize;
  }

  /**
   * Get the file path.
   */
  getPath(): string {
    return this.filePath;
  }

  /**
   * Close the file stream.
   */
  close(): void {
    if (this.fd >= 0 && !this.memoryData) {
      const fs = FileStream.getFs();
      fs.closeSync(this.fd);
      this.fd = -1;
    }
    this.memoryData = null;
  }
}
