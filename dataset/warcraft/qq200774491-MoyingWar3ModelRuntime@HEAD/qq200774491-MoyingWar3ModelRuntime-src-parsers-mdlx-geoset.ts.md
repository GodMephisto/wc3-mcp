---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/parsers/mdlx/geoset.ts"
ref: HEAD
---

import BinaryStream from '../common/binarystream';
import TokenStream from './tokenstream';
import Extent from './extent';

function hasValidTailLayout(stream: BinaryStream, start: number, hasLod: boolean): boolean {
    const candidate = new BinaryStream(stream.uint8array.subarray(start));

    try {
        if (hasLod) {
            candidate.skip(84);
        }

        candidate.skip(28); // Extent.
        const sequenceExtentCount = candidate.readUint32();
        candidate.skip(sequenceExtentCount * 28);

        let tag = candidate.readBinary(4);
        while (tag === 'TANG' || tag === 'SKIN') {
            const count = candidate.readUint32();
            candidate.skip(tag === 'TANG' ? count * 16 : count);
            tag = candidate.readBinary(4);
        }

        if (tag !== 'UVAS') return false;

        const uvSetCount = candidate.readUint32();
        for (let i = 0; i < uvSetCount; i++) {
            if (candidate.readBinary(4) !== 'UVBS') return false;
            candidate.skip(candidate.readUint32() * 8);
        }

        return candidate.remaining === 0;
    } catch {
        return false;
    }
}

/**
 * 几何体集合。
 */
export default class Geoset {
    vertices: Float32Array = new Float32Array(0);
    normals: Float32Array = new Float32Array(0);
    faceTypeGroups: Uint32Array = new Uint32Array(0);
    faceGroups: Uint32Array = new Uint32Array(0);
    faces: Uint16Array = new Uint16Array(0);
    vertexGroups: Uint8Array = new Uint8Array(0);
    matrixGroups: Uint32Array = new Uint32Array(0);
    matrixIndices: Uint32Array = new Uint32Array(0);
    materialId = 0;
    selectionGroup = 0;
    selectionFlags = 0;
    /** 
     * @since 900
     */
    lod = -1;
    /** 
     * @since 900
     */
    lodName = '';
    extent = new Extent();
    sequenceExtents: Extent[] = [];
    /** 
     * @since 900
     */
    tangents: Float32Array = new Float32Array(0);
    /**
     * 骨骼索引和权重的数组。
     * 每8个连续元素描述以下内容：
     *    [B0, B1, B2, B3, W0, W1, W2, W3]
     * 其中：
     *     Bn 是骨骼索引。
     *     Wn 是权重，可以通过 Wn/255 进行归一化。
     *
     * @since 900
     */
    skin: Uint8Array = new Uint8Array(0);
    uvSets: Float32Array[] = [];

    readMdx(stream: BinaryStream, version: number): void {
        const size = stream.readUint32();
        if (size < 120) {
            throw new Error(`Invalid geoset size: ${size}`);
        }

        const geosetStream = stream.substream(size - 4);

        geosetStream.skip(4); // VRTX
        this.vertices = geosetStream.readFloat32Array(geosetStream.readUint32() * 3);
        geosetStream.skip(4); // NRMS
        this.normals = geosetStream.readFloat32Array(geosetStream.readUint32() * 3);
        geosetStream.skip(4); // PTYP
        this.faceTypeGroups = geosetStream.readUint32Array(geosetStream.readUint32());
        geosetStream.skip(4); // PCNT
        this.faceGroups = geosetStream.readUint32Array(geosetStream.readUint32());
        geosetStream.skip(4); // PVTX
        this.faces = geosetStream.readUint16Array(geosetStream.readUint32());
        geosetStream.skip(4); // GNDX
        this.vertexGroups = geosetStream.readUint8Array(geosetStream.readUint32());
        geosetStream.skip(4); // MTGC
        this.matrixGroups = geosetStream.readUint32Array(geosetStream.readUint32());
        geosetStream.skip(4); // MATS
        this.matrixIndices = geosetStream.readUint32Array(geosetStream.readUint32());
        this.materialId = geosetStream.readUint32();
        this.selectionGroup = geosetStream.readUint32();
        this.selectionFlags = geosetStream.readUint32();

        const tailStart = geosetStream.index;
        const classicLayout = hasValidTailLayout(geosetStream, tailStart, false);
        const reforgedLayout = version > 800 && hasValidTailLayout(geosetStream, tailStart, true);

        if (!classicLayout && !reforgedLayout) {
            throw new Error(`Invalid geoset tail layout (version ${version})`);
        }

        if (reforgedLayout) {
            this.lod = geosetStream.readInt32();
            this.lodName = geosetStream.read(80);
        }

        this.extent.readMdx(geosetStream);

        for (let i = 0, l = geosetStream.readUint32(); i < l; i++) {
            const extent = new Extent();

            extent.readMdx(geosetStream);

            this.sequenceExtents.push(extent);
        }

        let tag = geosetStream.readBinary(4);
        while (tag === 'TANG' || tag === 'SKIN') {
            if (tag === 'TANG') {
                this.tangents = geosetStream.readFloat32Array(geosetStream.readUint32() * 4);
            } else {
                this.skin = geosetStream.readUint8Array(geosetStream.readUint32());
            }
            tag = geosetStream.readBinary(4);
        }

        if (tag !== 'UVAS') {
            throw new Error(`Expected UVAS in geoset, got "${tag}"`);
        }

        for (let i = 0, l = geosetStream.readUint32(); i < l; i++) {
            const uvTag = geosetStream.readBinary(4);
            if (uvTag !== 'UVBS') {
                throw new Error(`Expected UVBS in geoset, got "${uvTag}"`);
            }
            this.uvSets.push(geosetStream.readFloat32Array(geosetStream.readUint32() * 2));
        }
    }

    writeMdx(stream: BinaryStream, version: number): void {
        stream.writeUint32(this.getByteLength(version));
        stream.writeBinary('VRTX');
        stream.writeUint32(this.vertices.length / 3);
        stream.writeFloat32Array(this.vertices);
        stream.writeBinary('NRMS');
        stream.writeUint32(this.normals.length / 3);
        stream.writeFloat32Array(this.normals);
        stream.writeBinary('PTYP');
        stream.writeUint32(this.faceTypeGroups.length);
        stream.writeUint32Array(this.faceTypeGroups);
        stream.writeBinary('PCNT');
        stream.writeUint32(this.faceGroups.length);
        stream.writeUint32Array(this.faceGroups);
        stream.writeBinary('PVTX');
        stream.writeUint32(this.faces.length);
        stream.writeUint16Array(this.faces);
        stream.writeBinary('GNDX');
        stream.writeUint32(this.vertexGroups.length);
        stream.writeUint8Array(this.vertexGroups);
        stream.writeBinary('MTGC');
        stream.writeUint32(this.matrixGroups.length);
        stream.writeUint32Array(this.matrixGroups);
        stream.writeBinary('MATS');
        stream.writeUint32(this.matrixIndices.length);
        stream.writeUint32Array(this.matrixIndices);
        stream.writeUint32(this.materialId);
        stream.writeUint32(this.selectionGroup);
        stream.writeUint32(this.selectionFlags);

        if (version > 800) {
            stream.writeInt32(this.lod);
            stream.skip(80 - stream.write(this.lodName));
        }

        this.extent.writeMdx(stream);

        stream.writeUint32(this.sequenceExtents.length);

        for (const sequenceExtent of this.sequenceExtents) {
            sequenceExtent.writeMdx(stream);
        }

        if (version > 800) {
            if (this.tangents.length) {
                stream.writeBinary('TANG');
                stream.writeUint32(this.tangents.length / 4);
                stream.writeFloat32Array(this.tangents);
            }

            if (this.skin.length) {
                stream.writeBinary('SKIN');
                stream.writeUint32(this.skin.length);
                stream.writeUint8Array(this.skin);
            }
        }

        stream.writeBinary('UVAS');
        stream.writeUint32(this.uvSets.length);

        for (const uvSet of this.uvSets) {
            stream.writeBinary('UVBS');
            stream.writeUint32(uvSet.length / 2);
            stream.writeFloat32Array(uvSet);
        }
    }

    readMdl(stream: TokenStream): void {
        for (let token of stream.readBlock()) {
            if (token === 'Vertices') {
                this.vertices = stream.readVectorsBlock(new Float32Array(stream.readInt() * 3), 3);
            } else if (token === 'Normals') {
                this.normals = stream.readVectorsBlock(new Float32Array(stream.readInt() * 3), 3);
            } else if (token === 'TVertices') {
                this.uvSets.push(stream.readVectorsBlock(new Float32Array(stream.readInt() * 2), 2));
            } else if (token === 'VertexGroup') {
                // 顶点组存储在没有计数的块中，暂时无法分配缓冲区。
                const vertexGroups: number[] = [];

                for (const vertexGroup of stream.readBlock()) {
                    vertexGroups.push(parseInt(vertexGroup));
                }

                this.vertexGroups = new Uint8Array(vertexGroups);
            } else if (token === 'Tangents') {
                this.tangents = stream.readVectorsBlock(new Float32Array(stream.readInt() * 4), 4);
            } else if (token === 'SkinWeights') {
                this.skin = stream.readVector(new Uint8Array(stream.readInt() * 8));
            } else if (token === 'Faces') {
                // 现在硬编码为三角形，直到我看到具有不同形状的模型。
                this.faceTypeGroups = new Uint32Array([4]);

                // 索引分布的向量数量。
                const vectors = stream.readInt();

                // 索引总数。
                const count = stream.readInt();

                stream.read(); // {
                stream.read(); // Triangles

                this.faces = stream.readVectorsBlock(new Uint16Array(count), count / vectors);

                // 声明所有面都在一个组中以符合MDX规范。
                this.faceGroups = new Uint32Array([count]);

                stream.read(); // }
            } else if (token === 'Groups') {
                const indices: number[] = [];
                const groups: number[] = [];

                stream.readInt(); // 矩阵数量
                stream.readInt(); // 总索引数

                for (const _ of stream.readBlock()) {
                    let size = 0;

                    for (const index of stream.readBlock()) {
                        indices.push(parseInt(index));
                        size += 1;
                    }

                    groups.push(size);
                }

                this.matrixIndices = new Uint32Array(indices);
                this.matrixGroups = new Uint32Array(groups);
            } else if (token === 'MinimumExtent') {
                stream.readVector(this.extent.min);
            } else if (token === 'MaximumExtent') {
                stream.readVector(this.extent.max);
            } else if (token === 'BoundsRadius') {
                this.extent.boundsRadius = stream.readFloat();
            } else if (token === 'Anim') {
                const extent = new Extent();

                for (token of stream.readBlock()) {
                    if (token === 'MinimumExtent') {
                        stream.readVector(extent.min);
                    } else if (token === 'MaximumExtent') {
                        stream.readVector(extent.max);
                    } else if (token === 'BoundsRadius') {
                        extent.boundsRadius = stream.readFloat();
                    }
                }

                this.sequenceExtents.push(extent);
            } else if (token === 'MaterialID') {
                this.materialId = stream.readInt();
            } else if (token === 'SelectionGroup') {
                this.selectionGroup = stream.readInt();
            } else if (token === 'Unselectable') {
                this.selectionFlags = 4;
            } else if (token === 'LevelOfDetail') {
                this.lod = stream.readInt();
            } else if (token === 'Name') {
                this.lodName = stream.read();
            } else {
                throw new Error(`几何体中的未知标记: "${token}"`);
            }
        }
    }

    writeMdl(stream: TokenStream, version: number): void {
        stream.startBlock('Geoset');

        stream.writeVectorArrayBlock('Vertices', this.vertices, 3);
        stream.writeVectorArrayBlock('Normals', this.normals, 3);

        for (const uvSet of this.uvSets) {
            stream.writeVectorArrayBlock('TVertices', uvSet, 2);
        }

        if (version > 800 && this.tangents.length) {
            stream.writeVectorArrayBlock('Tangents', this.tangents, 4);
        }

        // 如果顶点组存在，始终写入。
        if (this.vertexGroups.length) {
            stream.startBlock('VertexGroup');

            for (let i = 0, l = this.vertexGroups.length; i < l; i++) {
                stream.writeLine(`${this.vertexGroups[i]},`);
            }

            stream.endBlock();
        }

        // 如果这是重制版HD几何体，写入皮肤。
        if (version > 800 && this.skin.length) {
            stream.startBlock('SkinWeights', this.skin.length / 8);

            for (let i = 0, l = this.skin.length; i < l; i += 8) {
                stream.writeLine(`${this.skin.subarray(i, i + 8).join(', ')},`);
            }

            stream.endBlock();
        }

        // 现在硬编码为三角形，直到我看到具有不同形状的模型。
        stream.startBlock('Faces', 1, this.faces.length);
        stream.startBlock('Triangles');
        stream.writeVector(this.faces);
        stream.endBlock();
        stream.endBlock();

        stream.startBlock('Groups', this.matrixGroups.length, this.matrixIndices.length);
        let index = 0;
        for (const groupSize of this.matrixGroups) {
            stream.writeVectorAttrib('Matrices', this.matrixIndices.subarray(index, index + groupSize));
            index += groupSize;
        }
        stream.endBlock();

        this.extent.writeMdl(stream);

        for (const sequenceExtent of this.sequenceExtents) {
            stream.startBlock('Anim');
            sequenceExtent.writeMdl(stream);
            stream.endBlock();
        }

        stream.writeNumberAttrib('MaterialID', this.materialId);
        stream.writeNumberAttrib('SelectionGroup', this.selectionGroup);

        if (this.selectionFlags === 4) {
            stream.writeFlag('Unselectable');
        }

        if (version > 800) {
            stream.writeNumberAttrib('LevelOfDetail', this.lod);

            if (this.lodName.length) {
                stream.writeStringAttrib('Name', this.lodName);
            }
        }

        stream.endBlock();
    }

    getByteLength(version: number): number {
        let size = 120 + this.vertices.byteLength + this.normals.byteLength + this.faceTypeGroups.byteLength + this.faceGroups.byteLength + this.faces.byteLength + this.vertexGroups.byteLength + this.matrixGroups.byteLength + this.matrixIndices.byteLength + this.sequenceExtents.length * 28;

        if (version > 800) {
            size += 84;

            if (this.tangents.length) {
                size += 8 + this.tangents.byteLength;
            }

            if (this.skin.length) {
                size += 8 + this.skin.byteLength;
            }
        }

        for (const uvSet of this.uvSets) {
            size += 8 + uvSet.byteLength;
        }

        return size;
    }
}
