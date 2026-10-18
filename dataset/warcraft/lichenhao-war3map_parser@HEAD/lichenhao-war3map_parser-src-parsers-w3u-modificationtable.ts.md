---
type: vendor-doc
source_repo: "https://github.com/lichenhao/war3map_parser"
source_path: "src/parsers/w3u/modificationtable.ts"
ref: HEAD
---


import type BinaryStream from "../../common/binarystream";
import ModifiedObject from "./modifiedobject";

/**
 * A modification table.
 */
export default class ModificationTable {
  objects: ModifiedObject[] = [];

  load(
    stream: BinaryStream,
    useOptionalInts: boolean,
    formatVersion: number
  ): void {
    for (let i = 0, l = stream.readUint32(); i < l; i++) {
      const object = new ModifiedObject();

      object.load(stream, useOptionalInts, formatVersion);

      this.objects[i] = object;
    }
  }

  save(
    stream: BinaryStream,
    useOptionalInts: boolean,
    formatVersion: number
  ): void {
    stream.writeUint32(this.objects.length);

    for (const object of this.objects) {
      object.save(stream, useOptionalInts, formatVersion);
    }
  }

  getByteLength(useOptionalInts: boolean, formatVersion: number): number {
    let size = 4;

    for (const object of this.objects) {
      size += object.getByteLength(useOptionalInts, formatVersion);
    }

    return size;
  }
}
