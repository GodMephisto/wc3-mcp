---
type: vendor-doc
source_repo: "https://github.com/lichenhao/war3map_parser"
source_path: "src/common/typesof.ts"
ref: HEAD
---

import { encodeUtf8 } from './utf8';

/**
 * Return the given buffer as a Uint8Array.
 * 
 * Strings are encoded as UTF8.
 */
export function bytesOf(buffer: ArrayBuffer | Uint8Array | string | number[]): Uint8Array {
  if (buffer instanceof Uint8Array) {
    return buffer;
  } else if (typeof buffer === 'string') {
    return encodeUtf8(buffer);
  } else {
    return new Uint8Array(buffer);
  }
}
