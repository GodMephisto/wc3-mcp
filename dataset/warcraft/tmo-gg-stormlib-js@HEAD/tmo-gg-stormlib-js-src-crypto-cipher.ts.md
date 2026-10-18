---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/crypto/cipher.ts"
ref: HEAD
---

import { MPQ_HASH_KEY2_MIX, MPQ_HASH_FILE_KEY, MPQ_FILE_KEY_V2 } from '../constants';
import { getStormBuffer } from './storm-buffer';
import { hashString, getPlainFileName } from './hash';

/**
 * Decrypt an MPQ data block in-place.
 * The buffer must be DWORD-aligned (length must be a multiple of 4).
 *
 * @param data - Buffer to decrypt (modified in place)
 * @param key - Primary encryption key (DWORD)
 */
export function decryptBlock(data: Buffer, key: number): void {
  const buffer = getStormBuffer();
  const dwCount = data.length >>> 2;
  let dwKey1 = key >>> 0;
  let dwKey2 = 0xEEEEEEEE;

  for (let i = 0; i < dwCount; i++) {
    dwKey2 = ((dwKey2 + buffer[MPQ_HASH_KEY2_MIX + (dwKey1 & 0xFF)]) & 0xFFFFFFFF) >>> 0;

    const encrypted = data.readUInt32LE(i * 4);
    const decrypted = (encrypted ^ ((dwKey1 + dwKey2) >>> 0)) >>> 0;
    data.writeUInt32LE(decrypted, i * 4);

    dwKey1 = (((~dwKey1 << 0x15) + 0x11111111) | (dwKey1 >>> 0x0B)) >>> 0;
    dwKey2 = ((decrypted + dwKey2 + (dwKey2 << 5) + 3) & 0xFFFFFFFF) >>> 0;
  }
}

/**
 * Encrypt an MPQ data block in-place.
 *
 * @param data - Buffer to encrypt (modified in place)
 * @param key - Primary encryption key (DWORD)
 */
export function encryptBlock(data: Buffer, key: number): void {
  const buffer = getStormBuffer();
  const dwCount = data.length >>> 2;
  let dwKey1 = key >>> 0;
  let dwKey2 = 0xEEEEEEEE;

  for (let i = 0; i < dwCount; i++) {
    dwKey2 = ((dwKey2 + buffer[MPQ_HASH_KEY2_MIX + (dwKey1 & 0xFF)]) & 0xFFFFFFFF) >>> 0;

    const plaintext = data.readUInt32LE(i * 4);
    const encrypted = (plaintext ^ ((dwKey1 + dwKey2) >>> 0)) >>> 0;
    data.writeUInt32LE(encrypted, i * 4);

    dwKey1 = (((~dwKey1 << 0x15) + 0x11111111) | (dwKey1 >>> 0x0B)) >>> 0;
    dwKey2 = ((plaintext + dwKey2 + (dwKey2 << 5) + 3) & 0xFFFFFFFF) >>> 0;
  }
}

/**
 * Calculate the decryption key for a file within an MPQ archive.
 *
 * @param fileName - Full path of the file within the archive
 * @param byteOffset - File's offset within the archive (lower 32 bits)
 * @param fileSize - Uncompressed file size
 * @param flags - File flags from block table
 * @returns The file decryption key
 */
export function decryptFileKey(
  fileName: string,
  byteOffset: bigint,
  fileSize: number,
  flags: number,
): number {
  const plainName = getPlainFileName(fileName);
  let key = hashString(plainName, MPQ_HASH_FILE_KEY);

  if (flags & MPQ_FILE_KEY_V2) {
    key = ((key + Number(byteOffset & 0xFFFFFFFFn)) ^ fileSize) >>> 0;
  }

  return key >>> 0;
}

/**
 * Attempt to detect a file's encryption key by analyzing the sector offset table.
 * The first DWORD of the sector offset table is known to be the size of the
 * sector offset table itself.
 *
 * @param encryptedData - First 8 bytes of encrypted sector offset table
 * @param sectorSize - Expected sector size
 * @param fileSize - Uncompressed file size
 * @returns The detected key, or 0 if detection failed
 */
export function detectFileKeyBySectorSize(
  encryptedData: Buffer,
  sectorSize: number,
  fileSize: number,
): number {
  const buffer = getStormBuffer();

  if (encryptedData.length < 8) return 0;

  const encrypted0 = encryptedData.readUInt32LE(0);
  const encrypted1 = encryptedData.readUInt32LE(4);

  // The first DWORD of the sector offset table is the number of sectors + 1,
  // multiplied by 4 (each offset is a DWORD)
  const sectorCount = Math.floor((fileSize + sectorSize - 1) / sectorSize);
  const expectedValue = (sectorCount + 1) * 4;

  // Try possible values for the first decrypted DWORD
  for (let dwDecrypted0 = expectedValue; dwDecrypted0 < expectedValue + 4; dwDecrypted0++) {
    const dwDecrypted1Max = sectorSize + dwDecrypted0;
    const dwKey1PlusKey2 = ((encrypted0 ^ dwDecrypted0) - 0xEEEEEEEE) >>> 0;

    for (let i = 0; i < 0x100; i++) {
      let dwKey1 = (dwKey1PlusKey2 - buffer[MPQ_HASH_KEY2_MIX + i]) >>> 0;
      let dwKey2 = 0xEEEEEEEE;

      // Verify first DWORD
      dwKey2 = ((dwKey2 + buffer[MPQ_HASH_KEY2_MIX + (dwKey1 & 0xFF)]) & 0xFFFFFFFF) >>> 0;
      const test0 = (encrypted0 ^ ((dwKey1 + dwKey2) >>> 0)) >>> 0;

      if (test0 === (dwDecrypted0 >>> 0)) {
        const saveKey1 = (dwKey1 + 1) >>> 0;

        // Advance key state
        dwKey1 = (((~dwKey1 << 0x15) + 0x11111111) | (dwKey1 >>> 0x0B)) >>> 0;
        dwKey2 = ((test0 + dwKey2 + (dwKey2 << 5) + 3) & 0xFFFFFFFF) >>> 0;
        dwKey2 = ((dwKey2 + buffer[MPQ_HASH_KEY2_MIX + (dwKey1 & 0xFF)]) & 0xFFFFFFFF) >>> 0;
        const test1 = (encrypted1 ^ ((dwKey1 + dwKey2) >>> 0)) >>> 0;

        // Second DWORD must be a valid sector offset
        if (test1 <= dwDecrypted1Max) {
          return saveKey1 >>> 0;
        }
      }
    }
  }

  return 0;
}
