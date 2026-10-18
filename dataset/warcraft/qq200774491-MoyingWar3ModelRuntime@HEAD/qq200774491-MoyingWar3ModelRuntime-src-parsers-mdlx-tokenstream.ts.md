---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/parsers/mdlx/tokenstream.ts"
ref: HEAD
---

/**
 * 用于读取和写入 MDL 令牌的工具类。
 */
export default class TokenStream {
    buffer: string;
    index = 0;
    ident = 0;
    indentSpaces = 4;
    precision = 1000000; // 6 digits after the decimal point.

    constructor(buffer?: string) {
        this.buffer = buffer || '';
    }

    /**
     * 清空流中的缓冲区内容。
     */
    clear(): void {
        this.buffer = '';
        this.index = 0;
        this.ident = 0;
    }

    /**
     * 读取流中的下一个令牌。
     * 在字符串形式的 "" 之外，空白字符将被忽略。
     * // 形式的注释将被忽略。
     * 逗号和冒号也会被忽略。
     * 大括号用作分隔符，通常用于表示文本块。
     *
     * 例如，给定以下字符串：
     *
     *     Header "A String" {
     *         Name Value, // A Comment
     *     }
     *
     * Read 将按顺序返回以下值：
     *
     *     Header
     *     "A String"
     *     {
     *     Name
     *     Value
     *     }
     *
     * 下面有围绕 read 的包装器方法，可以帮助读取结构化代码，快去看看吧！
     */
    readToken(): string | undefined {
        const buffer = this.buffer;
        const length = buffer.length;
        let inComment = false;
        let inString = false;
        let token = '';

        while (this.index < length) {
            const c = buffer[this.index++];

            if (inComment) {
                if (c === '\n') {
                    inComment = false;
                }
            } else if (inString) {
                if (c === '\\') {
                    token += c + buffer[this.index++];
                } else if (c === '\n') {
                    token += '\\n';
                } else if (c === '\r') {
                    token += '\\r';
                } else if (c === '"') {
                    return token;
                } else {
                    token += c;
                }
            } else if (c === ' ' || c === ',' || c === '\t' || c === '\n' || c === ':' || c === '\r') {
                if (token.length) {
                    return token;
                }
            } else if (c === '{' || c === '}') {
                if (token.length) {
                    this.index--;
                    return token;
                } else {
                    return c;
                }
            } else if (c === '/' && buffer[this.index] === '/') {
                if (token.length) {
                    this.index--;
                    return token;
                } else {
                    inComment = true;
                }
            } else if (c === '"') {
                if (token.length) {
                    this.index--;
                    return token;
                } else {
                    inString = true;
                }
            } else {
                token += c;
            }
        }

        return;
    }

    /**
     * 与 readToken 相同，但如果遇到流的末尾，将抛出异常。
     */
    read(): string {
        const value = this.readToken();

        if (value === undefined) {
            throw new Error('End of stream reached prematurely');
        }

        return value;
    }

    /**
     * 读取下一个令牌而不推进流位置。
     */
    peek(): string {
        const index = this.index;
        const value = this.read();

        this.index = index;

        return value;
    }

    /**
     * 读取下一个令牌，并将其解析为整数。
     */
    readInt(): number {
        return parseInt(this.read());
    }

    /**
     * 读取下一个令牌，并将其解析为浮点数。
     */
    readFloat(): number {
        return parseFloat(this.read());
    }

    /**
     * { Number0, Number1, ..., NumberN }
     */
    readVector<T extends Uint8Array | Uint16Array | Uint32Array | Float32Array>(view: T): T {
        this.read(); // {

        for (let i = 0, l = view.length; i < l; i++) {
            view[i] = this.readFloat();
        }

        this.read(); // }

        return view;
    }

    /**
     * {
     *     { Value1, Value2, ..., ValueSize },
     *     { Value1, Value2, ..., ValueSize },
     *     ...
     * }
     */
    readVectorsBlock<T extends Uint16Array | Float32Array>(view: T, size: number): T {
        this.read(); // {

        for (let i = 0, l = view.length; i < l; i += size) {
            this.readVector(view.subarray(i, i + size));
        }

        this.read(); // }

        return view;
    }

    /**
     * 读取以下形式的颜色：
     *
     *      { R, G, B }
     *
     * 颜色会重新排列为 BGR。
     */
    readColor(view: Float32Array): Float32Array {
        this.read(); // {

        view[2] = this.readFloat();
        view[1] = this.readFloat();
        view[0] = this.readFloat();

        this.read(); // }

        return view;
    }

    /**
     * 块读取的辅助生成器。
     * 假设我们有这样的块：
     * 
     *     {
     *         Key1 Value1
     *         Key2 Value2
     *         ...
     *         KeyN ValueN
     *     }
     * 
     * 生成器逐个生成键，调用者需要根据键读取值。
     * 这用于大多数 MDL 块。
     */
    * readBlock(): Generator<string> {
        this.read(); // {

        let token = this.read();

        while (token !== '}') {
            yield token;

            token = this.read();
        }
    }

    /**
     * 将给定的字符串添加到缓冲区。
     * 会在前面添加当前的缩进级别，并在写入后换行。
     */
    writeLine(line: string): void {
        this.buffer += `${' '.repeat(this.ident * this.indentSpaces)}${line}\n`;
    }

    /**
     * Flag,
     */
    writeFlag(flag: string): void {
        this.writeLine(`${flag},`);
    }

    /**
     * Name Flag,
     */
    writeFlagAttrib(name: string, flag: string): void {
        this.writeLine(`${name} ${flag},`);
    }

    /**
     * Name Value,
     */
    writeNumberAttrib(name: string, value: number): void {
        this.writeLine(`${name} ${this.floatDecimals(value)},`);
    }

    /**
     * Name "Value",
     */
    writeStringAttrib(name: string, value: string): void {
        this.writeLine(`${name} "${value}",`);
    }

    /**
     * Name { Value0, Value1, ..., ValueN }
     */
    writeVectorAttrib(name: string, value: Uint8Array | Uint32Array | Float32Array): void {
        this.writeLine(`${name} { ${this.floatArrayDecimals(value)} },`);
    }

    /**
     * 写入以下形式的颜色：
     *
     *      { B, G, R }
     *
     * 颜色会重新排列为 RGB。
     * 名称可以是 "Color" 或 "static Color"，取决于上下文。
     */
    writeColor(name: string, value: Float32Array): void {
        const b = this.floatDecimals(value[0]);
        const g = this.floatDecimals(value[1]);
        const r = this.floatDecimals(value[2]);

        this.writeLine(`${name} { ${r}, ${g}, ${b} },`);
    }

    /**
     * { Value0, Value1, ..., ValueN },
     */
    writeVector(value: Uint16Array | Float32Array): void {
        this.writeLine(`{ ${this.floatArrayDecimals(value)} },`);
    }

    /**
     * Name Vectors {
     *     { Value1, Value2, ..., ValueSize },
     *     { Value1, Value2, ..., ValueSize },
     *     ...
     * }
     */
    writeVectorArrayBlock(name: string, view: Float32Array, size: number): void {
        this.startBlock(name, view.length / size);

        for (let i = 0, l = view.length; i < l; i += size) {
            this.writeVector(view.subarray(i, i + size));
        }

        this.endBlock();
    }

    /**
     * 开始一个新的块，形式如下：
     *
     *      Header1 Header2 ... HeaderN {
     *          ...
     *      }
     */
    startBlock(name: string, ...headers: (string | number)[]): void {
        if (headers.length) {
            name = `${name} ${headers.join(' ')}`;
        }

        this.writeLine(`${name} {`);
        this.ident += 1;
    }

    /**
     * 开始一个新的块，形式如下：
     *
     *      Header "Name" {
     *          ...
     *      }
     */
    startObjectBlock(header: string, name: string): void {
        // 事实证明，对象名称中可以有引号。
        this.writeLine(`${header} "${name.replace(/"/g, '\\"')}" {`);
        this.ident += 1;
    }

    /**
     * 结束之前开始的块，并处理缩进。
     */
    endBlock(): void {
        this.ident -= 1;
        this.writeLine('}');
    }

    /**
     * 结束之前开始的块，并处理缩进。
     * 在块结束后添加逗号。
     */
    endBlockComma(): void {
        this.ident -= 1;
        this.writeLine('},');
    }

    /**
     * 增加后续行写入的缩进级别。
     */
    indent(): void {
        this.ident += 1;
    }

    /**
     * 减少后续行写入的缩进级别。
     */
    unindent(): void {
        this.ident -= 1;
    }

    /**
   * 给定一个数字，截断小数点后的数字。
   * 给定的精度应该是以 10 为基数。
   * 例如，小数点后两位数字的精度应该是 100。
   * 结果以字符串形式返回。
   */
    floatDecimals(value: number): string {
        return `${Math.trunc(value * this.precision) / this.precision}`;
    }

    /**
     * 在类型化数组上使用 floatDecimals，并返回其字符串表示形式。
     */
    floatArrayDecimals(value: Uint8Array | Uint16Array | Uint32Array | Float32Array): string {
        if (value instanceof Float32Array) {
            const array: string[] = [];

            for (let i = 0, l = value.length; i < l; i++) {
                array[i] = this.floatDecimals(value[i]);
            }

            return array.join(', ');
        } else {
            return value.join(', ');
        }
    }
}
