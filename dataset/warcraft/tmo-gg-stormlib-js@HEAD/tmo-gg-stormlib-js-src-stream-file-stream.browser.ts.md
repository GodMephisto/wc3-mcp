---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/stream/file-stream.browser.ts"
ref: HEAD
---

/**
 * Browser-safe stream abstraction backed by in-memory binary data.
 */

type BinaryInput = Buffer | Uint8Array | ArrayBuffer;

export class FileStream {
  private fileSize: bigint;
  private filePath: string;
  private memoryData: Buffer | null;

  private constructor(fileSize: bigint, filePath: string, memoryData: Buffer) {
    this.fileSize = fileSize;
    this.filePath = filePath;
    this.memoryData = memoryData;
  }

  /**
   * Path-based open is not available in browsers.
   */
  static open(_path: string): never {
    throw new Error('Path-based archive open is not available in browser runtime. Use MpqArchive.openFromBuffer().');
  }

  /**
   * Create an in-memory stream from binary data.
   */
  static fromBuffer(data: BinaryInput, filePath = '[buffer]'): FileStream {
    const buffer = FileStream.normalizeBuffer(data);
    return new FileStream(BigInt(buffer.length), filePath, buffer);
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

  /**
   * Read bytes at a given offset.
   */
  read(offset: bigint, length: number): Buffer {
    if (!this.memoryData) {
      return Buffer.alloc(0);
    }

    const start = Number(offset);
    if (start >= this.memoryData.length || length <= 0) {
      return Buffer.alloc(0);
    }

    const end = Math.min(start + length, this.memoryData.length);
    return this.memoryData.subarray(start, end);
  }

  /**
   * Read bytes into an existing buffer.
   */
  readInto(offset: bigint, buffer: Buffer, bufOffset: number, length: number): number {
    if (!this.memoryData) {
      return 0;
    }

    const start = Number(offset);
    if (start >= this.memoryData.length || length <= 0) {
      return 0;
    }

    const end = Math.min(start + length, this.memoryData.length);
    return this.memoryData.copy(buffer, bufOffset, start, end);
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
   * Close the stream.
   */
  close(): void {
    this.memoryData = null;
  }
}
