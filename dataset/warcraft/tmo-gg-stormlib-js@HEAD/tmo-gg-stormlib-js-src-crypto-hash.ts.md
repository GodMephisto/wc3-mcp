---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/crypto/hash.ts"
ref: HEAD
---

import {
  MPQ_HASH_TABLE_INDEX,
  MPQ_HASH_NAME_A,
  MPQ_HASH_NAME_B,
  MPQ_HASH_FILE_KEY,
} from '../constants';
import { getStormBuffer } from './storm-buffer';

/**
 * AsciiToUpperTable: converts a-z to A-Z and '/' (0x2F) to '\' (0x5C).
 * All other bytes pass through unchanged. This is essential for MPQ path normalization.
 */
const AsciiToUpperTable = new Uint8Array([
  0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
  0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F,
  0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x5C, // '/' -> '\'
  0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
  0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F,
  0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F,
  0x60, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F,
  0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F,
  0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F,
  0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9A, 0x9B, 0x9C, 0x9D, 0x9E, 0x9F,
  0xA0, 0xA1, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7, 0xA8, 0xA9, 0xAA, 0xAB, 0xAC, 0xAD, 0xAE, 0xAF,
  0xB0, 0xB1, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBA, 0xBB, 0xBC, 0xBD, 0xBE, 0xBF,
  0xC0, 0xC1, 0xC2, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0xCA, 0xCB, 0xCC, 0xCD, 0xCE, 0xCF,
  0xD0, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xDB, 0xDC, 0xDD, 0xDE, 0xDF,
  0xE0, 0xE1, 0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0xEA, 0xEB, 0xEC, 0xED, 0xEE, 0xEF,
  0xF0, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0xFA, 0xFB, 0xFC, 0xFD, 0xFE, 0xFF,
]);

/**
 * Compute MPQ hash of a string. The hashType selects which section of the
 * StormBuffer to use:
 * - MPQ_HASH_TABLE_INDEX (0x000): hash table slot
 * - MPQ_HASH_NAME_A (0x100): file name verification hash 1
 * - MPQ_HASH_NAME_B (0x200): file name verification hash 2
 * - MPQ_HASH_FILE_KEY (0x300): encryption key derivation
 */
export function hashString(str: string, hashType: number): number {
  const buffer = getStormBuffer();
  let seed1 = 0x7FED7FED;
  let seed2 = 0xEEEEEEEE;

  for (let i = 0; i < str.length; i++) {
    const ch = AsciiToUpperTable[str.charCodeAt(i) & 0xFF];

    seed1 = (buffer[hashType + ch] ^ ((seed1 + seed2) >>> 0)) >>> 0;
    seed2 = ((ch + seed1 + seed2 + (seed2 << 5) + 3) & 0xFFFFFFFF) >>> 0;
  }

  return seed1 >>> 0;
}

/** Convenience: compute hash table index */
export function hashTableIndex(fileName: string): number {
  return hashString(fileName, MPQ_HASH_TABLE_INDEX);
}

/** Convenience: compute name hash A */
export function hashNameA(fileName: string): number {
  return hashString(fileName, MPQ_HASH_NAME_A);
}

/** Convenience: compute name hash B */
export function hashNameB(fileName: string): number {
  return hashString(fileName, MPQ_HASH_NAME_B);
}

/** Convenience: compute file encryption key hash */
export function hashFileKey(fileName: string): number {
  return hashString(fileName, MPQ_HASH_FILE_KEY);
}

/**
 * Jenkins hashlittle2 - used for HET/BET tables (64-bit hash).
 * Produces a 64-bit hash as a BigInt.
 */
export function jenkinsHash(str: string): bigint {
  let a = 0xDEADBEEF + str.length;
  let b = a;
  let c = a;

  let i = 0;
  while (i + 12 <= str.length) {
    a = (a + str.charCodeAt(i)) >>> 0;
    a = (a + (str.charCodeAt(i + 1) << 8)) >>> 0;
    a = (a + (str.charCodeAt(i + 2) << 16)) >>> 0;
    a = (a + ((str.charCodeAt(i + 3) << 24) >>> 0)) >>> 0;
    b = (b + str.charCodeAt(i + 4)) >>> 0;
    b = (b + (str.charCodeAt(i + 5) << 8)) >>> 0;
    b = (b + (str.charCodeAt(i + 6) << 16)) >>> 0;
    b = (b + ((str.charCodeAt(i + 7) << 24) >>> 0)) >>> 0;
    c = (c + str.charCodeAt(i + 8)) >>> 0;
    c = (c + (str.charCodeAt(i + 9) << 8)) >>> 0;
    c = (c + (str.charCodeAt(i + 10) << 16)) >>> 0;
    c = (c + ((str.charCodeAt(i + 11) << 24) >>> 0)) >>> 0;

    // mix
    a = (a - c) >>> 0; a = (a ^ ((c << 4) | (c >>> 28))) >>> 0; c = (c + b) >>> 0;
    b = (b - a) >>> 0; b = (b ^ ((a << 6) | (a >>> 26))) >>> 0; a = (a + c) >>> 0;
    c = (c - b) >>> 0; c = (c ^ ((b << 8) | (b >>> 24))) >>> 0; b = (b + a) >>> 0;
    a = (a - c) >>> 0; a = (a ^ ((c << 16) | (c >>> 16))) >>> 0; c = (c + b) >>> 0;
    b = (b - a) >>> 0; b = (b ^ ((a << 19) | (a >>> 13))) >>> 0; a = (a + c) >>> 0;
    c = (c - b) >>> 0; c = (c ^ ((b << 4) | (b >>> 28))) >>> 0; b = (b + a) >>> 0;

    i += 12;
  }

  // Handle remaining bytes
  const remaining = str.length - i;
  switch (remaining) {
    case 12: c = (c + ((str.charCodeAt(i + 11) << 24) >>> 0)) >>> 0; // fallthrough
    case 11: c = (c + (str.charCodeAt(i + 10) << 16)) >>> 0;
    case 10: c = (c + (str.charCodeAt(i + 9) << 8)) >>> 0;
    case 9:  c = (c + str.charCodeAt(i + 8)) >>> 0;
    case 8:  b = (b + ((str.charCodeAt(i + 7) << 24) >>> 0)) >>> 0;
    case 7:  b = (b + (str.charCodeAt(i + 6) << 16)) >>> 0;
    case 6:  b = (b + (str.charCodeAt(i + 5) << 8)) >>> 0;
    case 5:  b = (b + str.charCodeAt(i + 4)) >>> 0;
    case 4:  a = (a + ((str.charCodeAt(i + 3) << 24) >>> 0)) >>> 0;
    case 3:  a = (a + (str.charCodeAt(i + 2) << 16)) >>> 0;
    case 2:  a = (a + (str.charCodeAt(i + 1) << 8)) >>> 0;
    case 1:  a = (a + str.charCodeAt(i)) >>> 0;
             break;
    case 0:  return (BigInt(c >>> 0) << 32n) | BigInt(b >>> 0);
  }

  // final mix
  c = (c ^ b) >>> 0; c = (c - ((b << 14) | (b >>> 18))) >>> 0;
  a = (a ^ c) >>> 0; a = (a - ((c << 11) | (c >>> 21))) >>> 0;
  b = (b ^ a) >>> 0; b = (b - ((a << 25) | (a >>> 7))) >>> 0;
  c = (c ^ b) >>> 0; c = (c - ((b << 16) | (b >>> 16))) >>> 0;
  a = (a ^ c) >>> 0; a = (a - ((c << 4) | (c >>> 28))) >>> 0;
  b = (b ^ a) >>> 0; b = (b - ((a << 14) | (a >>> 18))) >>> 0;
  c = (c ^ b) >>> 0; c = (c - ((b << 24) | (b >>> 8))) >>> 0;

  return (BigInt(c >>> 0) << 32n) | BigInt(b >>> 0);
}

/** Extract the plain file name from a path (after last '/' or '\') */
export function getPlainFileName(filePath: string): string {
  let plainStart = 0;
  for (let i = 0; i < filePath.length; i++) {
    const ch = filePath.charCodeAt(i);
    if (ch === 0x5C || ch === 0x2F) { // '\' or '/'
      plainStart = i + 1;
    }
  }
  return filePath.substring(plainStart);
}
