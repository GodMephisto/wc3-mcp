---
type: vendor-doc
source_repo: "https://github.com/lichenhao/war3map_parser"
source_path: "src/parsers/unitsdoo/droppeditem.ts"
ref: HEAD
---

import type BinaryStream from "../../common/binarystream";

/**
 * A dropped item.
 */
export default class DroppedItem {
  id = '\0\0\0\0';
  chance = 0;

  load(stream: BinaryStream): void {
    this.id = stream.readBinary(4);
    this.chance = stream.readInt32();
  }

  save(stream: BinaryStream): void {
    stream.writeBinary(this.id);
    stream.writeInt32(this.chance);
  }
}
