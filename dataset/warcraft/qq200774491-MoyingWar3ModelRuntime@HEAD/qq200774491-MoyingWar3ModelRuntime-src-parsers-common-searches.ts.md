---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/parsers/common/searches.ts"
ref: HEAD
---

/**
 * 搜索工具函数
 */

/** 在字节数组中搜索字符串 */
export function isStringInBytes(buffer: Uint8Array, target: string, offset = 0, length = Infinity): boolean {
    const start = Math.max(offset, 0);
    const end = Math.min(start + length, buffer.length);
    let whichByte = 0;
    let targetByte = target.charCodeAt(0);

    for (let i = start; i < end; i++) {
        const byte = buffer[i];

        if (byte === targetByte) {
            whichByte += 1;
            if (whichByte === target.length) return true;
            targetByte = target.charCodeAt(whichByte);
        } else if (whichByte > 0) {
            whichByte = 0;
            targetByte = target.charCodeAt(0);
        }
    }

    return false;
}

/** 在字符串中搜索子字符串 */
export function isStringInString(buffer: string, target: string, offset = 0, length = Infinity): boolean {
    const start = Math.max(offset, 0);
    const end = Math.min(start + length, buffer.length);
    let whichByte = 0;
    let targetByte = target[0];

    for (let i = start; i < end; i++) {
        const byte = buffer[i];

        if (byte === targetByte) {
            whichByte += 1;
            if (whichByte === target.length) return true;
            targetByte = target[whichByte];
        } else if (whichByte > 0) {
            whichByte = 0;
            targetByte = target[0];
        }
    }

    return false;
}

/** 在字节数组中查找指定字节（带边界） */
export function boundIndexOf(buffer: Uint8Array, target: number, offset = 0, length = Infinity): number {
    const start = Math.max(offset, 0);
    const end = Math.min(start + length, buffer.length);

    for (let i = start; i < end; i++) {
        if (buffer[i] === target) return i;
    }

    return -1;
}
