---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/parsers/common/bytesof.ts"
ref: HEAD
---

/**
 * 缓冲区转换工具
 */

import { encodeUtf8 } from './utf8';

/** 将给定缓冲区转换为 Uint8Array */
export function bytesOf(buffer: ArrayBuffer | Uint8Array | string | number[]): Uint8Array {
    if (buffer instanceof Uint8Array) {
        return buffer;
    } else if (typeof buffer === 'string') {
        return encodeUtf8(buffer);
    } else {
        return new Uint8Array(buffer);
    }
}
