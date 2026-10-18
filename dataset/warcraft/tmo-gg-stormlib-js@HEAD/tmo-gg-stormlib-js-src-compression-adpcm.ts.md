---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/compression/adpcm.ts"
ref: HEAD
---

/**
 * Blizzard's custom ADPCM decompression for MPQ audio data.
 * Ported from StormLib's adpcm.cpp.
 *
 * This handles both mono and stereo audio decompression.
 */

/** Step size index table for ADPCM */
const STEP_TABLE: number[] = [
      7,     8,     9,    10,    11,    12,    13,    14,
     16,    17,    19,    21,    23,    25,    28,    31,
     34,    37,    41,    45,    50,    55,    60,    66,
     73,    80,    88,    97,   107,   118,   130,   143,
    157,   173,   190,   209,   230,   253,   279,   307,
    337,   371,   408,   449,   494,   544,   598,   658,
    724,   796,   876,   963,  1060,  1166,  1282,  1411,
   1552,  1707,  1878,  2066,  2272,  2499,  2749,  3024,
   3327,  3660,  4026,  4428,  4871,  5358,  5894,  6484,
   7132,  7845,  8630,  9493, 10442, 11487, 12635, 13899,
  15289, 16818, 18500, 20350, 22385, 24623, 27086, 29794,
  32767,
];

/** Index adjustment table */
const INDEX_TABLE: number[] = [
  -1, 0, -1, 4, -1, 2, -1, 6,
  -1, 1, -1, 5, -1, 3, -1, 7,
  -1, 1, -1, 5, -1, 3, -1, 7,
  -1, 2, -1, 4, -1, 6, -1, 8,
];

interface AdpcmState {
  predSample: number;
  stepIndex: number;
}

function clampStepIndex(index: number): number {
  if (index < 0) return 0;
  if (index > 88) return 88;
  return index;
}

function clampSample(sample: number): number {
  if (sample > 32767) return 32767;
  if (sample < -32768) return -32768;
  return sample;
}

function decodeSample(state: AdpcmState, code: number): number {
  const step = STEP_TABLE[state.stepIndex];

  // Compute the difference
  let diff = step >>> 3;
  if (code & 4) diff += step;
  if (code & 2) diff += step >>> 1;
  if (code & 1) diff += step >>> 2;

  // Apply sign
  if (code & 8) {
    state.predSample = clampSample(state.predSample - diff);
  } else {
    state.predSample = clampSample(state.predSample + diff);
  }

  // Update step index
  state.stepIndex = clampStepIndex(state.stepIndex + INDEX_TABLE[code & 0x1F]);

  return state.predSample;
}

/**
 * Decompress mono ADPCM audio data.
 *
 * @param inBuffer - Compressed input data
 * @param outSize - Expected decompressed size
 * @returns Decompressed data (16-bit PCM samples, little-endian)
 */
export function decompressAdpcmMono(inBuffer: Buffer, outSize: number): Buffer {
  if (inBuffer.length < 4) {
    return Buffer.alloc(outSize);
  }

  const output = Buffer.alloc(outSize);
  let inPos = 0;
  let outPos = 0;

  // Read the initial shift value
  const shift = inBuffer[inPos++];

  // Read initial predicted sample (16-bit signed LE)
  const initialSample = inBuffer.readInt16LE(inPos);
  inPos += 2;

  const state: AdpcmState = {
    predSample: initialSample,
    stepIndex: STEP_TABLE.indexOf(Math.abs(initialSample)) || 0,
  };

  // Write initial sample
  if (outPos + 2 <= outSize) {
    output.writeInt16LE(state.predSample, outPos);
    outPos += 2;
  }

  // Decode remaining samples
  while (inPos < inBuffer.length && outPos + 2 <= outSize) {
    const byte = inBuffer[inPos++];

    // Low nibble
    const sample1 = decodeSample(state, byte & 0x0F);
    if (outPos + 2 <= outSize) {
      output.writeInt16LE(sample1, outPos);
      outPos += 2;
    }

    // High nibble
    if (outPos + 2 <= outSize) {
      const sample2 = decodeSample(state, (byte >>> 4) & 0x0F);
      output.writeInt16LE(sample2, outPos);
      outPos += 2;
    }
  }

  return output;
}

/**
 * Decompress stereo ADPCM audio data.
 *
 * @param inBuffer - Compressed input data
 * @param outSize - Expected decompressed size
 * @returns Decompressed data (interleaved 16-bit PCM samples, little-endian)
 */
export function decompressAdpcmStereo(inBuffer: Buffer, outSize: number): Buffer {
  if (inBuffer.length < 8) {
    return Buffer.alloc(outSize);
  }

  const output = Buffer.alloc(outSize);
  let inPos = 0;
  let outPos = 0;

  // Read the initial shift value
  const shift = inBuffer[inPos++];

  // Read initial samples for both channels (16-bit signed LE)
  const initialLeft = inBuffer.readInt16LE(inPos);
  inPos += 2;
  const initialRight = inBuffer.readInt16LE(inPos);
  inPos += 2;

  const stateL: AdpcmState = {
    predSample: initialLeft,
    stepIndex: 0,
  };
  const stateR: AdpcmState = {
    predSample: initialRight,
    stepIndex: 0,
  };

  // Write initial samples (interleaved L, R)
  if (outPos + 4 <= outSize) {
    output.writeInt16LE(stateL.predSample, outPos);
    outPos += 2;
    output.writeInt16LE(stateR.predSample, outPos);
    outPos += 2;
  }

  // Decode remaining samples
  let channel = 0; // 0 = left, 1 = right

  while (inPos < inBuffer.length && outPos + 2 <= outSize) {
    const byte = inBuffer[inPos++];

    // Low nibble
    const state = channel === 0 ? stateL : stateR;
    const sample1 = decodeSample(state, byte & 0x0F);
    if (outPos + 2 <= outSize) {
      output.writeInt16LE(sample1, outPos);
      outPos += 2;
    }
    channel ^= 1;

    // High nibble
    if (outPos + 2 <= outSize) {
      const state2 = channel === 0 ? stateL : stateR;
      const sample2 = decodeSample(state2, (byte >>> 4) & 0x0F);
      output.writeInt16LE(sample2, outPos);
      outPos += 2;
    }
    channel ^= 1;
  }

  return output;
}
