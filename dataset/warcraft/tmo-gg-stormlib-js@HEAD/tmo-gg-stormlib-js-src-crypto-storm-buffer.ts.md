---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/crypto/storm-buffer.ts"
ref: HEAD
---

import { STORM_BUFFER_SIZE } from '../constants';

/**
 * The StormBuffer is a 0x500 (1280) entry DWORD lookup table used by
 * all MPQ cryptographic operations. It is divided into 5 sections of 256
 * entries each, one for each hash type.
 *
 * Initialized using a Linear Congruential Generator (LCG).
 */
const stormBuffer = new Uint32Array(STORM_BUFFER_SIZE);
let initialized = false;

function initializeStormBuffer(): void {
  let seed = 0x00100001;

  for (let index1 = 0; index1 < 0x100; index1++) {
    let index2 = index1;
    for (let i = 0; i < 5; i++, index2 += 0x100) {
      seed = (seed * 125 + 3) % 0x2AAAAB;
      const temp1 = (seed & 0xFFFF) << 0x10;

      seed = (seed * 125 + 3) % 0x2AAAAB;
      const temp2 = seed & 0xFFFF;

      stormBuffer[index2] = (temp1 | temp2) >>> 0;
    }
  }

  initialized = true;
}

export function getStormBuffer(): Uint32Array {
  if (!initialized) {
    initializeStormBuffer();
  }
  return stormBuffer;
}
