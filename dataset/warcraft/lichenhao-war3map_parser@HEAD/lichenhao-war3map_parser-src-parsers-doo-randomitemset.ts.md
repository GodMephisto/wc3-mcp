---
type: vendor-doc
source_repo: "https://github.com/lichenhao/war3map_parser"
source_path: "src/parsers/doo/randomitemset.ts"
ref: HEAD
---

import type BinaryStream from '../../common/binarystream';
import RandomItem from './randomitem.js';

/**
 * A random item set.
 */
export default class RandomItemSet {
  items: RandomItem[] = [];

  load(stream: BinaryStream): void {
    for (let i = 0, l = stream.readUint32(); i < l; i++) {
      const item = new RandomItem();

      item.load(stream);

      this.items.push(item);
    }
  }

  save(stream: BinaryStream): void {
    stream.writeUint32(this.items.length);

    for (const item of this.items) {
      item.save(stream);
    }
  }

  getByteLength(): number {
    return 4 + this.items.length * 8;
  }
}
