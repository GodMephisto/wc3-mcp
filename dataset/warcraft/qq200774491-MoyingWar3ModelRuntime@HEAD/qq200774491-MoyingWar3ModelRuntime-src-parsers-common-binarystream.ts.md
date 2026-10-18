---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/parsers/common/binarystream.ts"
ref: HEAD
---

/**
 * 二进制流处理类 - 用于读写二进制数据
 */

import { bytesOf } from './bytesof';
import { boundIndexOf } from './searches';
import {
    uint8ToInt8, uint8ToInt16, uint8ToInt32,
    uint8ToUint16, uint8ToUint32, uint8ToFloat32, uint8ToFloat64,
    int8ToUint8, int16ToUint8, int32ToUint8,
    uint16ToUint8, uint32ToUint8, float32ToUint8, float64ToUint8
} from './typecast';
import { decodeUtf8, encodeUtf8 } from './utf8';

const uint8 = new Uint8Array(8);

export default class BinaryStream {
    buffer: ArrayBuffer;
    uint8array: Uint8Array;
    index = 0;
    byteLength: number;
    remaining: number;

    constructor(buffer: ArrayBuffer | Uint8Array, byteOffset?: number, byteLength?: number) {
        const bytes = bytesOf(buffer);
        byteOffset = byteOffset || 0;
        byteLength = byteLength || bytes.length;

        this.buffer = buffer instanceof Uint8Array ? buffer.buffer as ArrayBuffer : buffer;
        this.uint8array = bytes.subarray(byteOffset, byteOffset + byteLength);
        this.byteLength = byteLength;
        this.remaining = byteLength;
    }

    substream(byteLength: number): BinaryStream {
        if (this.remaining < byteLength) {
            throw new Error(`BinaryStream: want ${byteLength} bytes but have ${this.remaining}`);
        }
        const index = this.index;
        this.index += byteLength;
        this.remaining -= byteLength;
        return new BinaryStream(this.uint8array.subarray(index, index + byteLength));
    }

    skip(bytes: number): void {
        if (this.remaining < bytes) {
            throw new Error(`BinaryStream: want ${bytes} bytes but have ${this.remaining}`);
        }
        this.index += bytes;
        this.remaining -= bytes;
    }

    seek(index: number): void {
        this.index = index;
        this.remaining = this.byteLength - index;
    }

    // ==================== 读取字符串 ====================

    read(bytes: number): string {
        if (this.remaining < bytes) {
            throw new Error(`BinaryStream: want ${bytes} bytes but have ${this.remaining}`);
        }
        const uint8array = this.uint8array;
        const start = this.index;
        let end = boundIndexOf(uint8array, 0, start, bytes);
        if (end === -1) end = start + bytes;

        this.index += bytes;
        this.remaining -= bytes;
        return decodeUtf8(uint8array.subarray(start, end));
    }

    readNull(): string {
        if (this.remaining < 1) {
            throw new Error(`BinaryStream: need at least 1 byte`);
        }
        const uint8array = this.uint8array;
        const start = this.index;
        let end = uint8array.indexOf(0, start);
        if (end === -1) end = uint8array.length - 1;

        const bytes = end - start + 1;
        this.index += bytes;
        this.remaining -= bytes;
        return decodeUtf8(uint8array.subarray(start, end));
    }

    readBinary(bytes: number): string {
        if (this.remaining < bytes) {
            throw new Error(`BinaryStream: want ${bytes} bytes but have ${this.remaining}`);
        }
        const uint8array = this.uint8array;
        const index = this.index;
        let data = '';
        for (let i = 0; i < bytes; i++) {
            data += String.fromCharCode(uint8array[index + i]);
        }
        this.index += bytes;
        this.remaining -= bytes;
        return data;
    }

    // ==================== 读取整数 ====================

    readInt8(): number {
        if (this.remaining < 1) throw new Error(`BinaryStream: need 1 byte`);
        const data = uint8ToInt8(this.uint8array[this.index]);
        this.index += 1;
        this.remaining -= 1;
        return data;
    }

    readInt16(): number {
        if (this.remaining < 2) throw new Error(`BinaryStream: need 2 bytes`);
        const { index, uint8array } = this;
        const data = uint8ToInt16(uint8array[index], uint8array[index + 1]);
        this.index += 2;
        this.remaining -= 2;
        return data;
    }

    readInt32(): number {
        if (this.remaining < 4) throw new Error(`BinaryStream: need 4 bytes`);
        const { index, uint8array } = this;
        const data = uint8ToInt32(uint8array[index], uint8array[index + 1], uint8array[index + 2], uint8array[index + 3]);
        this.index += 4;
        this.remaining -= 4;
        return data;
    }

    readUint8(): number {
        if (this.remaining < 1) throw new Error(`BinaryStream: need 1 byte`);
        const data = this.uint8array[this.index];
        this.index += 1;
        this.remaining -= 1;
        return data;
    }

    readUint16(): number {
        if (this.remaining < 2) throw new Error(`BinaryStream: need 2 bytes`);
        const { index, uint8array } = this;
        const data = uint8ToUint16(uint8array[index], uint8array[index + 1]);
        this.index += 2;
        this.remaining -= 2;
        return data;
    }

    readUint32(): number {
        if (this.remaining < 4) throw new Error(`BinaryStream: need 4 bytes`);
        const { index, uint8array } = this;
        const data = uint8ToUint32(uint8array[index], uint8array[index + 1], uint8array[index + 2], uint8array[index + 3]);
        this.index += 4;
        this.remaining -= 4;
        return data;
    }

    // ==================== 读取浮点数 ====================

    readFloat32(): number {
        if (this.remaining < 4) throw new Error(`BinaryStream: need 4 bytes`);
        const { index, uint8array } = this;
        const data = uint8ToFloat32(uint8array[index], uint8array[index + 1], uint8array[index + 2], uint8array[index + 3]);
        this.index += 4;
        this.remaining -= 4;
        return data;
    }

    readFloat64(): number {
        if (this.remaining < 8) throw new Error(`BinaryStream: need 8 bytes`);
        const { index, uint8array } = this;
        const data = uint8ToFloat64(
            uint8array[index], uint8array[index + 1], uint8array[index + 2], uint8array[index + 3],
            uint8array[index + 4], uint8array[index + 5], uint8array[index + 6], uint8array[index + 7]
        );
        this.index += 8;
        this.remaining -= 8;
        return data;
    }

    // ==================== 读取数组 ====================

    readInt8Array(view: number | Int8Array): Int8Array {
        if (!ArrayBuffer.isView(view)) view = new Int8Array(view);
        if (this.remaining < view.byteLength) throw new Error(`BinaryStream: need ${view.byteLength} bytes`);
        const { index, uint8array } = this;
        for (let i = 0, l = view.length; i < l; i++) {
            view[i] = uint8ToInt8(uint8array[index + i]);
        }
        this.index += view.byteLength;
        this.remaining -= view.byteLength;
        return view;
    }

    readInt16Array(view: number | Int16Array): Int16Array {
        if (!ArrayBuffer.isView(view)) view = new Int16Array(view);
        if (this.remaining < view.byteLength) throw new Error(`BinaryStream: need ${view.byteLength} bytes`);
        const { index, uint8array } = this;
        for (let i = 0, l = view.length; i < l; i++) {
            const offset = index + i * 2;
            view[i] = uint8ToInt16(uint8array[offset], uint8array[offset + 1]);
        }
        this.index += view.byteLength;
        this.remaining -= view.byteLength;
        return view;
    }

    readInt32Array(view: number | Int32Array): Int32Array {
        if (!ArrayBuffer.isView(view)) view = new Int32Array(view);
        if (this.remaining < view.byteLength) throw new Error(`BinaryStream: need ${view.byteLength} bytes`);
        const { index, uint8array } = this;
        for (let i = 0, l = view.length; i < l; i++) {
            const offset = index + i * 4;
            view[i] = uint8ToInt32(uint8array[offset], uint8array[offset + 1], uint8array[offset + 2], uint8array[offset + 3]);
        }
        this.index += view.byteLength;
        this.remaining -= view.byteLength;
        return view;
    }

    readUint8Array(view: number | Uint8Array): Uint8Array {
        if (!ArrayBuffer.isView(view)) view = new Uint8Array(view);
        if (this.remaining < view.byteLength) throw new Error(`BinaryStream: need ${view.byteLength} bytes`);
        const { index, uint8array } = this;
        for (let i = 0, l = view.length; i < l; i++) {
            view[i] = uint8array[index + i];
        }
        this.index += view.byteLength;
        this.remaining -= view.byteLength;
        return view;
    }

    readUint16Array(view: number | Uint16Array): Uint16Array {
        if (!ArrayBuffer.isView(view)) view = new Uint16Array(view);
        if (this.remaining < view.byteLength) throw new Error(`BinaryStream: need ${view.byteLength} bytes`);
        const { index, uint8array } = this;
        for (let i = 0, l = view.length; i < l; i++) {
            const offset = index + i * 2;
            view[i] = uint8ToUint16(uint8array[offset], uint8array[offset + 1]);
        }
        this.index += view.byteLength;
        this.remaining -= view.byteLength;
        return view;
    }

    readUint32Array(view: number | Uint32Array): Uint32Array {
        if (!ArrayBuffer.isView(view)) view = new Uint32Array(view);
        if (this.remaining < view.byteLength) throw new Error(`BinaryStream: need ${view.byteLength} bytes`);
        const { index, uint8array } = this;
        for (let i = 0, l = view.length; i < l; i++) {
            const offset = index + i * 4;
            view[i] = uint8ToUint32(uint8array[offset], uint8array[offset + 1], uint8array[offset + 2], uint8array[offset + 3]);
        }
        this.index += view.byteLength;
        this.remaining -= view.byteLength;
        return view;
    }

    readFloat32Array(view: number | Float32Array): Float32Array {
        if (!ArrayBuffer.isView(view)) view = new Float32Array(view);
        if (this.remaining < view.byteLength) throw new Error(`BinaryStream: need ${view.byteLength} bytes`);
        const { index, uint8array } = this;
        for (let i = 0, l = view.length; i < l; i++) {
            const offset = index + i * 4;
            view[i] = uint8ToFloat32(uint8array[offset], uint8array[offset + 1], uint8array[offset + 2], uint8array[offset + 3]);
        }
        this.index += view.byteLength;
        this.remaining -= view.byteLength;
        return view;
    }

    readFloat64Array(view: number | Float64Array): Float64Array {
        if (!ArrayBuffer.isView(view)) view = new Float64Array(view);
        if (this.remaining < view.byteLength) throw new Error(`BinaryStream: need ${view.byteLength} bytes`);
        const { index, uint8array } = this;
        for (let i = 0, l = view.length; i < l; i++) {
            const offset = index + i * 8;
            view[i] = uint8ToFloat64(
                uint8array[offset], uint8array[offset + 1], uint8array[offset + 2], uint8array[offset + 3],
                uint8array[offset + 4], uint8array[offset + 5], uint8array[offset + 6], uint8array[offset + 7]
            );
        }
        this.index += view.byteLength;
        this.remaining -= view.byteLength;
        return view;
    }

    // ==================== 写入字符串 ====================

    write(utf8: string): number {
        const bytes = encodeUtf8(utf8);
        this.writeUint8Array(bytes);
        return bytes.length;
    }

    writeNull(utf8: string): number {
        const bytes = this.write(utf8);
        this.index++;
        this.remaining--;
        return bytes + 1;
    }

    writeBinary(value: string): void {
        const { index, uint8array } = this;
        const count = value.length;
        for (let i = 0; i < count; i++) {
            uint8array[index + i] = value.charCodeAt(i);
        }
        this.index += count;
    }

    // ==================== 写入整数 ====================

    writeInt8(value: number): void {
        this.uint8array[this.index] = int8ToUint8(value);
        this.index += 1;
    }

    writeInt16(value: number): void {
        const { index, uint8array } = this;
        int16ToUint8(uint8, value);
        uint8array[index] = uint8[0];
        uint8array[index + 1] = uint8[1];
        this.index += 2;
    }

    writeInt32(value: number): void {
        const { index, uint8array } = this;
        int32ToUint8(uint8, value);
        uint8array[index] = uint8[0];
        uint8array[index + 1] = uint8[1];
        uint8array[index + 2] = uint8[2];
        uint8array[index + 3] = uint8[3];
        this.index += 4;
    }

    writeUint8(value: number): void {
        this.uint8array[this.index] = value;
        this.index += 1;
    }

    writeUint16(value: number): void {
        const { index, uint8array } = this;
        uint16ToUint8(uint8, value);
        uint8array[index] = uint8[0];
        uint8array[index + 1] = uint8[1];
        this.index += 2;
    }

    writeUint32(value: number): void {
        const { index, uint8array } = this;
        uint32ToUint8(uint8, value);
        uint8array[index] = uint8[0];
        uint8array[index + 1] = uint8[1];
        uint8array[index + 2] = uint8[2];
        uint8array[index + 3] = uint8[3];
        this.index += 4;
    }

    // ==================== 写入浮点数 ====================

    writeFloat32(value: number): void {
        const { index, uint8array } = this;
        float32ToUint8(uint8, value);
        uint8array[index] = uint8[0];
        uint8array[index + 1] = uint8[1];
        uint8array[index + 2] = uint8[2];
        uint8array[index + 3] = uint8[3];
        this.index += 4;
    }

    writeFloat64(value: number): void {
        const { index, uint8array } = this;
        float64ToUint8(uint8, value);
        for (let i = 0; i < 8; i++) {
            uint8array[index + i] = uint8[i];
        }
        this.index += 8;
    }

    // ==================== 写入数组 ====================

    writeInt8Array(view: Int8Array): void {
        const { index, uint8array } = this;
        for (let i = 0, l = view.length; i < l; i++) {
            uint8array[index + i] = int8ToUint8(view[i]);
        }
        this.index += view.byteLength;
    }

    writeInt16Array(view: Int16Array): void {
        const { index, uint8array } = this;
        for (let i = 0, l = view.length; i < l; i++) {
            const offset = index + i * 2;
            int16ToUint8(uint8, view[i]);
            uint8array[offset] = uint8[0];
            uint8array[offset + 1] = uint8[1];
        }
        this.index += view.byteLength;
    }

    writeInt32Array(view: Int32Array): void {
        const { index, uint8array } = this;
        for (let i = 0, l = view.length; i < l; i++) {
            const offset = index + i * 4;
            int32ToUint8(uint8, view[i]);
            uint8array[offset] = uint8[0];
            uint8array[offset + 1] = uint8[1];
            uint8array[offset + 2] = uint8[2];
            uint8array[offset + 3] = uint8[3];
        }
        this.index += view.byteLength;
    }

    writeUint8Array(view: Uint8Array): void {
        const { index, uint8array } = this;
        for (let i = 0, l = view.length; i < l; i++) {
            uint8array[index + i] = view[i];
        }
        this.index += view.byteLength;
    }

    writeUint16Array(view: Uint16Array): void {
        const { index, uint8array } = this;
        for (let i = 0, l = view.length; i < l; i++) {
            const offset = index + i * 2;
            uint16ToUint8(uint8, view[i]);
            uint8array[offset] = uint8[0];
            uint8array[offset + 1] = uint8[1];
        }
        this.index += view.byteLength;
    }

    writeUint32Array(view: Uint32Array): void {
        const { index, uint8array } = this;
        for (let i = 0, l = view.length; i < l; i++) {
            const offset = index + i * 4;
            uint32ToUint8(uint8, view[i]);
            uint8array[offset] = uint8[0];
            uint8array[offset + 1] = uint8[1];
            uint8array[offset + 2] = uint8[2];
            uint8array[offset + 3] = uint8[3];
        }
        this.index += view.byteLength;
    }

    writeFloat32Array(view: Float32Array): void {
        const { index, uint8array } = this;
        for (let i = 0, l = view.length; i < l; i++) {
            const offset = index + i * 4;
            float32ToUint8(uint8, view[i]);
            uint8array[offset] = uint8[0];
            uint8array[offset + 1] = uint8[1];
            uint8array[offset + 2] = uint8[2];
            uint8array[offset + 3] = uint8[3];
        }
        this.index += view.byteLength;
    }

    writeFloat64Array(view: Float64Array): void {
        const { index, uint8array } = this;
        for (let i = 0, l = view.length; i < l; i++) {
            const offset = index + i * 8;
            float64ToUint8(uint8, view[i]);
            for (let j = 0; j < 8; j++) {
                uint8array[offset + j] = uint8[j];
            }
        }
        this.index += view.byteLength;
    }
}
