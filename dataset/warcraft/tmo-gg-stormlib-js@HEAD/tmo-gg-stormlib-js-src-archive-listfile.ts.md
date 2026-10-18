---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/archive/listfile.ts"
ref: HEAD
---

/**
 * (listfile) parsing for MPQ archives.
 *
 * The (listfile) is a special internal file that contains a list of all
 * file names in the archive, one per line (separated by \r\n or \n).
 */

/**
 * Parse a (listfile) buffer into an array of file names.
 *
 * @param data - Raw (listfile) contents
 * @returns Array of file path strings
 */
export function parseListfile(data: Buffer): string[] {
  const text = data.toString('utf8');
  const names: string[] = [];

  for (const line of text.split(/\r?\n/)) {
    const trimmed = line.trim();
    if (trimmed.length > 0) {
      names.push(trimmed);
    }
  }

  return names;
}

/**
 * Parse the (attributes) file.
 *
 * Format:
 * - DWORD version (100)
 * - DWORD flags (bitmask: 1=CRC32, 2=FILETIME, 4=MD5)
 * - Then for each file in the block table (in order):
 *   - If flags & 1: DWORD CRC32
 *   - If flags & 2: QWORD FILETIME
 *   - If flags & 4: 16 bytes MD5
 *
 * @param data - Raw (attributes) contents
 * @param fileCount - Number of files in the block table
 * @returns Parsed attributes
 */
export interface FileAttributes {
  crc32s: number[];
  fileTimes: bigint[];
  md5s: Uint8Array[];
}

export function parseAttributes(data: Buffer, fileCount: number): FileAttributes | null {
  if (data.length < 8) return null;

  const version = data.readUInt32LE(0);
  const flags = data.readUInt32LE(4);

  if (version !== 100) return null;

  const result: FileAttributes = {
    crc32s: [],
    fileTimes: [],
    md5s: [],
  };

  let offset = 8;

  // CRC32 values
  if (flags & 0x01) {
    for (let i = 0; i < fileCount && offset + 4 <= data.length; i++) {
      result.crc32s.push(data.readUInt32LE(offset));
      offset += 4;
    }
  }

  // FILETIME values
  if (flags & 0x02) {
    for (let i = 0; i < fileCount && offset + 8 <= data.length; i++) {
      result.fileTimes.push(data.readBigUInt64LE(offset));
      offset += 8;
    }
  }

  // MD5 values
  if (flags & 0x04) {
    for (let i = 0; i < fileCount && offset + 16 <= data.length; i++) {
      result.md5s.push(new Uint8Array(data.subarray(offset, offset + 16)));
      offset += 16;
    }
  }

  return result;
}
