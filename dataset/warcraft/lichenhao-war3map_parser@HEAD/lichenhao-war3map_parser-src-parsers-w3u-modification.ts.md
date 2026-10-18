---
type: vendor-doc
source_repo: "https://github.com/lichenhao/war3map_parser"
source_path: "src/parsers/w3u/modification.ts"
ref: HEAD
---

import type BinaryStream from "../../common/binarystream";
import { byteLengthUtf8 } from "../../common/utf8";


/**
 * A modification.
 */
export default class Modification {
  id = '\0\0\0\0';
  variableType = 0;
  levelOrVariation = 0;
  dataPointer = 0;
  value: number | string = 0;
  u1 = 0;

  load(stream: BinaryStream, useOptionalInts: boolean): void {
    this.id = stream.readBinary(4);
    this.variableType = stream.readInt32();

    if (useOptionalInts) {
      this.levelOrVariation = stream.readInt32();
      this.dataPointer = stream.readInt32();
    }

    if (this.variableType === 0) {
      this.value = stream.readInt32();
    } else if (this.variableType === 1 || this.variableType === 2) {
      this.value = stream.readFloat32();
    } else if (this.variableType === 3) {
      this.value = stream.readNull();
    } else {
      throw new Error(`Modification: unknown variable type ${this.variableType}`);
    }

    this.u1 = stream.readInt32();
  }

  save(stream: BinaryStream, useOptionalInts: boolean): void {
    stream.writeBinary(this.id);
    stream.writeInt32(this.variableType);

    if (useOptionalInts) {
      stream.writeInt32(this.levelOrVariation);
      stream.writeInt32(this.dataPointer);
    }

    if (this.variableType === 0) {
      stream.writeInt32(this.value as number);
    } else if (this.variableType === 1 || this.variableType === 2) {
      stream.writeFloat32(this.value as number);
    } else if (this.variableType === 3) {
      stream.writeNull(this.value as string);
    } else {
      throw new Error(`Modification: unknown variable type ${this.variableType}`);
    }

    stream.writeInt32(this.u1);
  }

  getByteLength(useOptionalInts: boolean): number {
    let size = 12;

    if (useOptionalInts) {
      size += 8;
    }

    if (this.variableType === 3) {
      size += byteLengthUtf8(this.value as string) + 1;
    } else {
      size += 4;
    }

    return size;
  }
}
