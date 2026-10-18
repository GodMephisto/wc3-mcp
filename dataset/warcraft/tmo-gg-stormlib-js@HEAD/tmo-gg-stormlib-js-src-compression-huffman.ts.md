---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/compression/huffman.ts"
ref: HEAD
---

/**
 * Blizzard's custom Huffman decompression, ported from StormLib's huffman.cpp.
 *
 * This is NOT standard Huffman coding. It uses a specific tree structure and
 * decomposition algorithm unique to MPQ archives (primarily used in
 * Diablo/StarCraft original).
 */

/** Huffman tree node */
interface HuffNode {
  weight: number;
  child0: number;   // index or -1
  child1: number;   // index or -1
  parent: number;   // index or -1
  value: number;    // byte value for leaf nodes, -1 for internal
}

/** Predefined weight tables for different compression types */
const WEIGHT_TABLES: Record<number, number[]> = {
  // Generic text compression (type 0)
  0: [
    0x0A0B, 0x0A08, 0x0A07, 0x0A06, 0x0A0A, 0x0A09, 0x0A05, 0x0A04,
    0x0A03, 0x0A02, 0x0A01, 0x0A00, 0x090F, 0x090E, 0x090D, 0x090C,
    0x090B, 0x090A, 0x0909, 0x0908, 0x0907, 0x0906, 0x0905, 0x0904,
    0x0903, 0x0902, 0x0901, 0x0900, 0x080F, 0x080E, 0x080D, 0x080C,
    0x080B, 0x080A, 0x0809, 0x0808, 0x0807, 0x0806, 0x0805, 0x0804,
    0x0803, 0x0802, 0x0801, 0x0800, 0x070F, 0x070E, 0x070D, 0x070C,
    0x070B, 0x070A, 0x0709, 0x0708, 0x0707, 0x0706, 0x0705, 0x0704,
    0x0703, 0x0702, 0x0701, 0x0700, 0x060F, 0x060E, 0x060D, 0x060C,
    0x060B, 0x060A, 0x0609, 0x0608, 0x0607, 0x0606, 0x0605, 0x0604,
    0x0603, 0x0602, 0x0601, 0x0600, 0x050F, 0x050E, 0x050D, 0x050C,
    0x050B, 0x050A, 0x0509, 0x0508, 0x0507, 0x0506, 0x0505, 0x0504,
    0x0503, 0x0502, 0x0501, 0x0500, 0x040F, 0x040E, 0x040D, 0x040C,
    0x040B, 0x040A, 0x0409, 0x0408, 0x0407, 0x0406, 0x0405, 0x0404,
    0x0403, 0x0402, 0x0401, 0x0400, 0x030F, 0x030E, 0x030D, 0x030C,
    0x030B, 0x030A, 0x0309, 0x0308, 0x0307, 0x0306, 0x0305, 0x0304,
    0x0303, 0x0302, 0x0301, 0x0300, 0x020F, 0x020E, 0x020D, 0x020C,
    0x020B, 0x020A, 0x0209, 0x0208, 0x0207, 0x0206, 0x0205, 0x0204,
    0x0203, 0x0202, 0x0201, 0x0200, 0x010F, 0x010E, 0x010D, 0x010C,
    0x010B, 0x010A, 0x0109, 0x0108, 0x0107, 0x0106, 0x0105, 0x0104,
    0x0103, 0x0102, 0x0101, 0x0100, 0x000F, 0x000E, 0x000D, 0x000C,
    0x000B, 0x000A, 0x0009, 0x0008, 0x0007, 0x0006, 0x0005, 0x0004,
    0x0003, 0x0002, 0x0001, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000,
    0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000,
    0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000,
    0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000,
    0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000,
    0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000,
    0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000,
    0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000,
    0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000,
    0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000,
    0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0002,
  ],
};

/** Bit reader for the Huffman stream */
class BitReader {
  private data: Buffer;
  private bitPos = 0;

  constructor(data: Buffer) {
    this.data = data;
  }

  readBits(numBits: number): number {
    let result = 0;

    for (let i = 0; i < numBits; i++) {
      const byteIndex = this.bitPos >>> 3;
      const bitIndex = this.bitPos & 7;

      if (byteIndex >= this.data.length) return result;

      if (this.data[byteIndex] & (1 << bitIndex)) {
        result |= (1 << i);
      }
      this.bitPos++;
    }

    return result;
  }

  readBit(): number {
    const byteIndex = this.bitPos >>> 3;
    const bitIndex = this.bitPos & 7;

    if (byteIndex >= this.data.length) return 0;

    const bit = (this.data[byteIndex] >>> bitIndex) & 1;
    this.bitPos++;
    return bit;
  }

  get isEof(): boolean {
    return (this.bitPos >>> 3) >= this.data.length;
  }
}

const MAX_TREE_ITEMS = 0x203;
const HUFF_ITEM_UNUSED = -1;

/**
 * Blizzard Huffman tree decompression.
 */
class HuffmanTree {
  private nodes: HuffNode[] = [];
  private root = -1;
  private items256: number[] = new Array(256).fill(-1);

  constructor(compressionType: number) {
    this.buildTree(compressionType);
  }

  private buildTree(compressionType: number): void {
    const weights = WEIGHT_TABLES[compressionType] || WEIGHT_TABLES[0];

    // Initialize nodes for all 256 byte values + 1 sentinel
    for (let i = 0; i < 257; i++) {
      this.nodes.push({
        weight: (i < 256) ? (weights ? (weights[i] || 0) : 0) : 0,
        child0: -1,
        child1: -1,
        parent: -1,
        value: i,
      });
    }

    // Build the tree bottom-up using a priority-queue approach
    // Sort leaf nodes by weight
    const leafIndices = Array.from({ length: 257 }, (_, i) => i)
      .filter(i => this.nodes[i].weight > 0 || i === 256);

    leafIndices.sort((a, b) => this.nodes[a].weight - this.nodes[b].weight);

    // Build tree by repeatedly combining two lowest weight nodes
    const active = [...leafIndices];

    while (active.length > 1) {
      const idx0 = active.shift()!;
      const idx1 = active.shift()!;

      const parentIdx = this.nodes.length;
      this.nodes.push({
        weight: this.nodes[idx0].weight + this.nodes[idx1].weight,
        child0: idx0,
        child1: idx1,
        parent: -1,
        value: -1,
      });

      this.nodes[idx0].parent = parentIdx;
      this.nodes[idx1].parent = parentIdx;

      // Insert into active list maintaining sort order
      let insertAt = active.length;
      for (let j = 0; j < active.length; j++) {
        if (this.nodes[parentIdx].weight <= this.nodes[active[j]].weight) {
          insertAt = j;
          break;
        }
      }
      active.splice(insertAt, 0, parentIdx);
    }

    if (active.length > 0) {
      this.root = active[0];
    }

    // Build fast lookup for leaf nodes
    for (let i = 0; i < 256; i++) {
      if (this.nodes[i].weight > 0) {
        this.items256[i] = i;
      }
    }
  }

  decodeOne(reader: BitReader): number {
    if (this.root === -1) return -1;

    let current = this.root;

    while (this.nodes[current].value === -1) {
      const bit = reader.readBit();
      if (bit === 0) {
        current = this.nodes[current].child0;
      } else {
        current = this.nodes[current].child1;
      }
      if (current === -1) return -1;
    }

    return this.nodes[current].value;
  }
}

/**
 * Decompress data using Blizzard's custom Huffman algorithm.
 *
 * @param inBuffer - Compressed input data
 * @param outSize - Expected decompressed size
 * @returns Decompressed data
 */
export function decompressHuffman(inBuffer: Buffer, outSize: number): Buffer {
  if (inBuffer.length === 0) {
    return Buffer.alloc(outSize);
  }

  // First byte is the compression type for weight table selection
  const compressionType = inBuffer[0];
  const reader = new BitReader(inBuffer.subarray(1));

  const tree = new HuffmanTree(compressionType);
  const output = Buffer.alloc(outSize);
  let outPos = 0;

  while (outPos < outSize && !reader.isEof) {
    const value = tree.decodeOne(reader);
    if (value < 0 || value === 256) break; // 256 = end of stream
    output[outPos++] = value & 0xFF;
  }

  return output;
}
