---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/parsers/common/typecast.ts"
ref: HEAD
---

/**
 * 类型转换工具 - 使用共享缓冲区实现高性能类型转换
 */

const buffer = new ArrayBuffer(8);
const int8 = new Int8Array(buffer);
const int16 = new Int16Array(buffer);
const int32 = new Int32Array(buffer);
const uint8 = new Uint8Array(buffer);
const uint16 = new Uint16Array(buffer);
const uint32 = new Uint32Array(buffer);
const float32 = new Float32Array(buffer);
const float64 = new Float64Array(buffer);

// ==================== Uint8 -> 有符号/无符号整数 ====================

export function uint8ToInt8(a: number): number {
    uint8[0] = a;
    return int8[0];
}

export function uint8ToInt16(a: number, b: number): number {
    uint8[0] = a;
    uint8[1] = b;
    return int16[0];
}

export function uint8ToInt24(a: number, b: number, c: number): number {
    uint8[0] = a;
    uint8[1] = b;
    uint8[2] = c;
    uint8[3] = 0;
    return int32[0];
}

export function uint8ToInt32(a: number, b: number, c: number, d: number): number {
    uint8[0] = a;
    uint8[1] = b;
    uint8[2] = c;
    uint8[3] = d;
    return int32[0];
}

export function uint8ToUint16(a: number, b: number): number {
    uint8[0] = a;
    uint8[1] = b;
    return uint16[0];
}

export function uint8ToUint24(a: number, b: number, c: number): number {
    uint8[0] = a;
    uint8[1] = b;
    uint8[2] = c;
    uint8[3] = 0;
    return uint32[0];
}

export function uint8ToUint32(a: number, b: number, c: number, d: number): number {
    uint8[0] = a;
    uint8[1] = b;
    uint8[2] = c;
    uint8[3] = d;
    return uint32[0];
}

// ==================== Uint8 -> 浮点数 ====================

export function uint8ToFloat32(a: number, b: number, c: number, d: number): number {
    uint8[0] = a;
    uint8[1] = b;
    uint8[2] = c;
    uint8[3] = d;
    return float32[0];
}

export function uint8ToFloat64(a: number, b: number, c: number, d: number, e: number, f: number, g: number, h: number): number {
    uint8[0] = a; uint8[1] = b; uint8[2] = c; uint8[3] = d;
    uint8[4] = e; uint8[5] = f; uint8[6] = g; uint8[7] = h;
    return float64[0];
}

// ==================== 整数 -> Uint8 ====================

export function int8ToUint8(a: number): number {
    uint8[0] = a;
    return int8[0];
}

export function int16ToUint8(out: Uint8Array, a: number): Uint8Array {
    int16[0] = a;
    out[0] = uint8[0];
    out[1] = uint8[1];
    return out;
}

export function int24ToUint8(out: Uint8Array, a: number): Uint8Array {
    int32[0] = a;
    out[0] = uint8[0];
    out[1] = uint8[1];
    out[2] = uint8[2];
    return out;
}

export function int32ToUint8(out: Uint8Array, a: number): Uint8Array {
    int32[0] = a;
    out[0] = uint8[0];
    out[1] = uint8[1];
    out[2] = uint8[2];
    out[3] = uint8[3];
    return out;
}

export function uint16ToUint8(out: Uint8Array, a: number): Uint8Array {
    uint16[0] = a;
    out[0] = uint8[0];
    out[1] = uint8[1];
    return out;
}

export function uint24ToUint8(out: Uint8Array, a: number): Uint8Array {
    uint32[0] = a;
    out[0] = uint8[0];
    out[1] = uint8[1];
    out[2] = uint8[2];
    return out;
}

export function uint32ToUint8(out: Uint8Array, a: number): Uint8Array {
    uint32[0] = a;
    out[0] = uint8[0];
    out[1] = uint8[1];
    out[2] = uint8[2];
    out[3] = uint8[3];
    return out;
}

// ==================== 浮点数 -> Uint8 ====================

export function float32ToUint8(out: Uint8Array, a: number): Uint8Array {
    float32[0] = a;
    out[0] = uint8[0];
    out[1] = uint8[1];
    out[2] = uint8[2];
    out[3] = uint8[3];
    return out;
}

export function float64ToUint8(out: Uint8Array, a: number): Uint8Array {
    float64[0] = a;
    out[0] = uint8[0]; out[1] = uint8[1]; out[2] = uint8[2]; out[3] = uint8[3];
    out[4] = uint8[4]; out[5] = uint8[5]; out[6] = uint8[6]; out[7] = uint8[7];
    return out;
}

// ==================== 数字/字符串转换 ====================

export function numberToUint32(number: number): number {
    uint32[0] = number;
    return uint32[0];
}

export function stringToBase256(string: string): number {
    let number = 0;
    for (const c of string) {
        number = number * 256 + c.charCodeAt(0);
    }
    return number;
}

export function base256ToString(number: number): string {
    const array: string[] = [];
    while (number > 0) {
        array.push(String.fromCharCode(number % 256));
        number = Math.floor(number / 256);
    }
    return array.reverse().join('');
}
