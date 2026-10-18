---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/parsers/common/utf8.ts"
ref: HEAD
---

/**
 * UTF-8 编码解码工具
 */

const decoder = new TextDecoder();
const encoder = new TextEncoder();

/** 将字节数组解码为 UTF-8 字符串 */
export function decodeUtf8(buffer: ArrayBuffer | Uint8Array): string {
    return decoder.decode(buffer);
}

/** 将 UTF-8 字符串编码为字节数组 */
export function encodeUtf8(utf8: string): Uint8Array {
    return encoder.encode(utf8);
}

/** 计算 UTF-8 字符串的字节长度 */
export function byteLengthUtf8(str: string): number {
    let s = str.length;
    for (let i = str.length - 1; i >= 0; i--) {
        const code = str.charCodeAt(i);
        if (code > 0x7f && code <= 0x7ff) s++;
        else if (code > 0x7ff && code <= 0xffff) s += 2;
        if (code >= 0xDC00 && code <= 0xDFFF) i--;
    }
    return s;
}

/** 按 UTF-8 字节长度分割字符串 */
export function splitUtf8ByteLength(str: string, chunkBytelength: number): string[] {
    const chunks: string[] = [];
    let pos = 0;
    let bytes = 0;

    for (let i = 0, l = str.length; i < l; i++) {
        const code = str.charCodeAt(i);

        if (code < 0x80) bytes += 1;
        else if (code < 0x800) bytes += 2;
        else if (code < 0xd800 || code >= 0xe000) bytes += 3;
        else { i++; bytes += 4; }

        if (bytes >= chunkBytelength - 3) {
            chunks.push(str.substring(pos, i));
            pos = i;
            bytes = 0;
        }
    }

    if (bytes > 0) {
        chunks.push(str.substring(pos));
    }

    return chunks;
}
